using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Data;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;
using TaskManager.Services.IServices;

namespace TaskManager.Services.Services.ProfileService
{
    public class ProfileService(TaskManagerDbContext context) : IProfileService
    {
        private readonly TaskManagerDbContext _context = context;

        public async Task<string?> UpdateUserAsync(UserProfileViewModel request)
        {
            UserProfileViewModel newReq = request;
            Console.WriteLine(newReq);
            try
            {
                User? userObj = await _context.Users
                    .Where(u => u.Id == request.Id)
                    .FirstOrDefaultAsync();

                if (userObj is null)
                    return "User not found";

                userObj.Name = request.Name;
                userObj.Email = request.Email;
                userObj.PhoneNumber = request.Phone;
                userObj.Address = request.Address;
                userObj.Department = request.Department;
                userObj.Designation = request.Designation;

                _context.Users.Update(userObj);
                await _context.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Something went wrong";
            }
        }
    }
}
