using OfficeOpenXml;
using TaskManager.DataAccess.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManager.Services.IServices;
using Microsoft.Data.SqlClient;
using System.Data;
using TaskManager.Models.Constants;

namespace TaskManager.Services.Services.ExportService
{
    public partial class TaskExportService : ITaskExportService
    {
        private const int BATCH_SIZE = 10000;
        private const int MAX_ROWS_PER_SHEET = 1048575;

        private readonly string _tempFolderPath;
        private readonly TaskManagerDbContext _context;
        private readonly ILogger<TaskExportService> _logger;

        public TaskExportService(TaskManagerDbContext context, ILogger<TaskExportService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _tempFolderPath = configuration["TempFilePath"] ?? Path.GetTempPath();

            Directory.CreateDirectory(_tempFolderPath);
        }

        public async Task CleanupExportFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    await System.Threading.Tasks.Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation($"Cleaned up export file: {filePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to cleanup export file: {filePath}");
            }
        }

        private void SetupWorkSheetHeaders(ExcelWorksheet worksheet)
        {
            var headers = new[]
            {
                "Task ID", "Title", "Description", "Duration (Hours)",
                "Status", "Priority", "Severity", "Responsible", "Client",
                 "Is Parent", "Parent Task ID"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }
        }

        private async Task<List<Models.Entities.Task>> GetTaskBatchAsync(
            string userId,
            int skip,
            int take,
            CancellationToken cancellationToken)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .Include(t => t.User)
                .Include(t => t.Client)
                .OrderBy(t => t.TaskId)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        private async Task<int> GetTaskCountAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync(cancellationToken);

                using var command = new SqlCommand("sp_GetTaskCountForUser", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 300
                };

                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar, 450)
                {
                    Value = string.IsNullOrEmpty(userId) ? DBNull.Value : userId
                });

                var result = await command.ExecuteScalarAsync(cancellationToken);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task count");
                return 0;
            }
        }

        private void PopulateTaskRow(ExcelWorksheet worksheet, int row, Models.Entities.Task task)
        {
            worksheet.Cells[row, 1].Value = task.TaskId;
            worksheet.Cells[row, 2].Value = task.Title;
            worksheet.Cells[row, 3].Value = task.Description;
            worksheet.Cells[row, 4].Value = task.Duration;
            worksheet.Cells[row, 5].Value = task.Status.ToString();
            worksheet.Cells[row, 6].Value = task.Priority.ToString();
            worksheet.Cells[row, 7].Value = task.Severity.ToString();
            worksheet.Cells[row, 9].Value = task.User?.Name ?? "N/A";
            worksheet.Cells[row, 10].Value = task.Client?.ClientFullName ?? "N/A";
            worksheet.Cells[row, 8].Value = task.IsParent ? "Yes" : "No";
            worksheet.Cells[row, 11].Value = task.ParentTaskId;
        }

        private void PopulateTaskRowFromReader(ExcelWorksheet worksheet, int row, SqlDataReader reader)
        {
            worksheet.Cells[row, 1].Value = reader.GetInt32("TaskId");
            worksheet.Cells[row, 2].Value = reader.IsDBNull("Title") ? "" : reader.GetString("Title");
            worksheet.Cells[row, 3].Value = reader.IsDBNull("Description") ? "" : reader.GetString("Description");
            worksheet.Cells[row, 4].Value = reader.IsDBNull("Duration") ? 0 : reader.GetDouble("Duration");
            worksheet.Cells[row, 5].Value = GetEnumNameOrDefault<Models.Constants.TaskStatus>(reader, "Status");
            worksheet.Cells[row, 6].Value = GetEnumNameOrDefault<Priority>(reader, "Priority");
            worksheet.Cells[row, 7].Value = GetEnumNameOrDefault<Severity>(reader, "Severity");
            worksheet.Cells[row, 8].Value = reader.IsDBNull("UserName") ? "N/A" : reader.GetString("UserName");
            worksheet.Cells[row, 9].Value = reader.IsDBNull("ClientFullName") ? "N/A" : reader.GetString("ClientFullName");
            worksheet.Cells[row, 10].Value = reader.IsDBNull("IsParent") ? "No" : (reader.GetBoolean("IsParent") ? "Yes" : "No");
            worksheet.Cells[row, 11].Value = reader.IsDBNull("ParentTaskId") ? (int?)null : reader.GetInt32("ParentTaskId");
        }

        private string GetEnumNameOrDefault<TEnum>(SqlDataReader reader, string columnName) where TEnum : Enum
        {
            if (!reader.IsDBNull(columnName))
            {
                int value = reader.GetInt32(reader.GetOrdinal(columnName));
                return Enum.IsDefined(typeof(TEnum), value)
                    ? ((TEnum)(object)value).ToString()
                    : $"Unkonw ({value})";
            }
            return "";
        }
    }
}
