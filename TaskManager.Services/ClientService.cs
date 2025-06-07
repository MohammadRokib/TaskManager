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

namespace TaskManager.Services
{
    public class ClientService(TaskManagerDbContext context) : IClientService
    {
        private readonly TaskManagerDbContext _context = context;
        public async Task<(List<ClientDashboardViewModel>?, string?)> GetClientDashBoardAsync()
        {
            try
            {
                List<ClientDashboardViewModel>? clients = await _context.Clients
                    .Select(client => new ClientDashboardViewModel()
                    {
                        ShortName = client.ClientShortName,
                        FullName = client.ClientFullName,
                        Address = client.ClientAddress,
                        OrderedTasks = 0,
                        PendingTasks = 0,
                        CompletedTasks = 0
                    })
                    .ToListAsync();

                return (clients, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (null, "Something went wrong");
            }
        }

        public async Task<string?> AddClient(AddClientViewModel request)
        {
            Client newClient = new Client()
            {
                ClientFullName = request.ClientFullName,
                ClientShortName = request.ClientShortName,
                ClientAddress = request.ClientAddress
            };

            try
            {
                await _context.Clients.AddAsync(newClient);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Something went wrong";
            }
            return null;
        }
        //public async Task<string?> LoginAsync(LoginViewModel request)
        //{
        //    try
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, false);
        //        if (result.Succeeded)
        //            return null;

        //        return "Email or Password is incorrect";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        return "Something went wrong";
        //    }
        //}
    }
}
