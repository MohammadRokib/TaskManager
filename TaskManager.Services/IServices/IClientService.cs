using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.IServices
{
    public interface IClientService
    {
        Task<(List<ClientDashboardViewModel>?, string?)> GetClientDashBoardAsync();
        Task<string?> AddClient(AddClientViewModel request);
    }
}
