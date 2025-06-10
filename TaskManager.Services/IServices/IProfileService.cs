using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.IServices
{
    public interface IProfileService
    {
        Task<string?> UpdateUserAsync(UserProfileViewModel request);
    }
}
