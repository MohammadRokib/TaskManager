using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace TaskManager.Services.Services.ExportService
{
    public partial class TaskExportService
    {
        public async Task<string> ExportTasksToExcelAsync(string userId, CancellationToken cancellationToken = default)
        {
            var fileName = $"Tasks_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            var filePath = Path.Combine(_tempFolderPath, fileName);

            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("MohammadRokib");

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Tasks");

                SetupWorkSheetHeaders(worksheet);

                var totalCount = await _context.Tasks
                    .Where(t => t.UserId == userId)
                    .CountAsync(cancellationToken);

                _logger.LogInformation($"Starting export of {totalCount} tasks for user {userId}");

                var currentRow = 2;
                var processedCount = 0;

                while (processedCount < totalCount)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var batch = await GetTaskBatchAsync(userId, processedCount, BATCH_SIZE, cancellationToken);
                    foreach (var task in batch)
                    {
                        PopulateTaskRow(worksheet, currentRow, task);
                        currentRow++;
                    }

                    processedCount += batch.Count;
                    _logger.LogInformation($"Process {processedCount}/{totalCount} tasks");

                    if (batch.Count < BATCH_SIZE)
                        break;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileInfo = new FileInfo(filePath);
                await package.SaveAsAsync(fileInfo, cancellationToken);

                _logger.LogInformation($"Excel export completed. File saved: {filePath}");
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
