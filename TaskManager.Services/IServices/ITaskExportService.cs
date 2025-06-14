namespace TaskManager.Services.IServices
{
    public interface ITaskExportService
    {
        Task<string> ExportTasksToExcelSpAsync(string userId, CancellationToken cancellationToken);
        Task<string> ExportTasksToExcelAsync(string userId, CancellationToken cancellationToken);
        Task CleanupExportFileAsync(string filePath);
    }
}
