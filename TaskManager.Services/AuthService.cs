using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;
using TaskManager.Services.IServices;

namespace TaskManager.Services
{
    public class AuthService(SignInManager<User> signInManager, UserManager<User> userManager) : IAuthService
    {
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<string?> LoginAsync(LoginViewModel request)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, false);
                if (result.Succeeded)
                    return null;

                return "Email or Password is incorrect";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Something went wrong";
            }
        }

        public async Task<string?> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Something went wrong";
            }
            return null;
        }

        public async Task<(User?, string?)> RegisterAsync(RegisterViewModel request)
        {
            User newUser = new User()
            {
                Email = request.Email,
                UserName = request.Email,
                Name = request.Name
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser, request.Password);
                if (result.Succeeded)
                    return (newUser, null);
                
                StringBuilder err = new StringBuilder();
                foreach (var error in result.Errors)
                    err.Append(error.Description);
                
                return (null, err.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (null, "Someting went wrong");
            }
        }
    }
}
