using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.IServices
{
    public interface IAuthService
    {
        Task<(User?, string?)> RegisterAsync(RegisterViewModel request);
        Task<string?> LoginAsync(LoginViewModel request);
        Task<string?> LogoutAsync();
    }
}
