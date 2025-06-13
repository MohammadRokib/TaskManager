using OfficeOpenXml;
using TaskManager.DataAccess.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManager.Services.IServices;

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
                "Status", "Priority", "Severity", "Is Parent", "Responsible",
                "Client", "Parent Task ID"
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

        private void PopulateTaskRow(ExcelWorksheet worksheet, int row, Models.Entities.Task task)
        {
            worksheet.Cells[row, 1].Value = task.TaskId;
            worksheet.Cells[row, 2].Value = task.Title;
            worksheet.Cells[row, 3].Value = task.Description;
            worksheet.Cells[row, 4].Value = task.Duration;
            worksheet.Cells[row, 5].Value = task.Status.ToString();
            worksheet.Cells[row, 6].Value = task.Priority.ToString();
            worksheet.Cells[row, 7].Value = task.Severity.ToString();
            worksheet.Cells[row, 8].Value = task.IsParent ? "Yes" : "No";
            worksheet.Cells[row, 9].Value = task.User?.Name ?? "N/A";
            worksheet.Cells[row, 10].Value = task.Client?.ClientFullName ?? "N/A";
            worksheet.Cells[row, 11].Value = task.ParentTaskId;
        }
    }
}
