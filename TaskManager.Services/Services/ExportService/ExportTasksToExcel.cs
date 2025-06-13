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

                var totalCount = await _context.Tasks
                    .Where(t => t.UserId == userId)
                    .CountAsync(cancellationToken);

                _logger.LogInformation($"Starting export of {totalCount} tasks for user {userId}");

                var currentWorksheet = 1;
                var currentRowInSheet = 2;
                var processedCount = 0;

                ExcelWorksheet worksheet = null;

                while (processedCount < totalCount)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (worksheet == null || currentRowInSheet > MAX_ROWS_PER_SHEET)
                    {
                        var sheetName = currentRowInSheet == 1 ? "Tasks" : $"Tasks_{currentWorksheet}";
                        worksheet = package.Workbook.Worksheets.Add(sheetName);
                        SetupWorkSheetHeaders(worksheet);
                        currentRowInSheet = 2;
                        currentWorksheet++;
                    }

                    var batch = await GetTaskBatchAsync(userId, processedCount, BATCH_SIZE, cancellationToken);
                    foreach (var task in batch)
                    {
                        if (currentRowInSheet > MAX_ROWS_PER_SHEET)
                        {
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            var sheetName = $"Tasks_{currentWorksheet}";
                            worksheet = package.Workbook.Worksheets.Add(sheetName);
                            SetupWorkSheetHeaders(worksheet);
                            currentRowInSheet = 2;
                            currentWorksheet++;
                        }
                        
                        PopulateTaskRow(worksheet, currentRowInSheet, task);
                        currentRowInSheet++;
                    }

                    processedCount += batch.Count;
                    _logger.LogInformation($"Process {processedCount}/{totalCount} tasks");

                    if (batch.Count < BATCH_SIZE)
                        break;
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
