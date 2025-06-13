using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Services.IServices
{
    public interface ITaskExportService
    {
        Task<string> ExportTasksToExcelAsync(string userId, CancellationToken cancellationToken);
        Task CleanupExportFileAsync(string filePath);
    }
}
