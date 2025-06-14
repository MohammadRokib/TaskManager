using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Data;

namespace TaskManager.Services.Services.ExportService
{
    public partial class TaskExportService
    {
        public async Task<string> ExportTasksToExcelSpAsync(string userId, CancellationToken cancellationToken = default)
        {
            var fileName = $"Tasks_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            var filePath = Path.Combine(_tempFolderPath, fileName);

            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("MohammadRokib");
                using var package = new ExcelPackage();

                var totalCount = await GetTaskCountAsync(userId, cancellationToken);
                _logger.LogInformation($"Starting export of {totalCount} tasks for user {userId}");

                var currentWorksheet = 1;
                var currentRowInSheet = 2;
                var processedCount = 0;
                var lastTaskId = 0;
                var hasMoreData = true;

                ExcelWorksheet worksheet = null;

                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync(cancellationToken);

                while (hasMoreData)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (worksheet == null || currentRowInSheet > MAX_ROWS_PER_SHEET)
                    {
                        if (worksheet != null && worksheet.Dimension != null)
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        var sheetName = currentRowInSheet == 1 ? "Tasks" : $"Tasks_{currentWorksheet}";
                        worksheet = package.Workbook.Worksheets.Add(sheetName);
                        SetupWorkSheetHeaders(worksheet);
                        currentRowInSheet = 2;
                        currentWorksheet++;
                    }

                    using var command = new SqlCommand("sp_ExportTasksForUser", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 600
                    };

                    command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar, 450)
                    {
                        Value = string.IsNullOrEmpty(userId) ? DBNull.Value : userId
                    });
                    command.Parameters.Add(new SqlParameter("@BatchSize", SqlDbType.Int) { Value = BATCH_SIZE });
                    command.Parameters.Add(new SqlParameter("@LastTaskId", SqlDbType.Int) { Value = lastTaskId });

                    using var reader = await command.ExecuteReaderAsync(cancellationToken);
                    var batchCount = 0;

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (currentRowInSheet > MAX_ROWS_PER_SHEET)
                        {
                            if (worksheet != null && worksheet.Dimension != null)
                                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            var sheetName = $"Tasks_{currentWorksheet}";
                            worksheet = package.Workbook.Worksheets.Add(sheetName);
                            SetupWorkSheetHeaders(worksheet);
                            currentRowInSheet = 2;
                            currentWorksheet++;
                        }

                        PopulateTaskRowFromReader(worksheet, currentRowInSheet, reader);
                        currentRowInSheet++;
                        batchCount++;
                        lastTaskId = reader.GetInt32("TaskId");
                    }

                    processedCount += batchCount;
                    hasMoreData = batchCount >= BATCH_SIZE;
                    _logger.LogInformation($"Processed {processedCount} tasks (batch: {batchCount})");

                    if (hasMoreData && batchCount > 0)
                        await System.Threading.Tasks.Task.Delay(100, cancellationToken);
                }

                if (worksheet != null && worksheet.Dimension != null)
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileInfo = new FileInfo(filePath);
                await package.SaveAsAsync(fileInfo, cancellationToken);

                _logger.LogInformation($"Excel export completed with {currentWorksheet - 1} worksheets. File saved: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error exporting tasks for user {userId}");
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogError(deleteEx, $"Failed to delete incomplete file: {filePath}");
                    }
                    throw;
                }
            }
            return string.Empty;
        }
    }
}
