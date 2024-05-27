using Bloggie.web.Models.ViewModel;
using Bloggie.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bloggie.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<IdentityUser> userManager;

        public AdminUsersController(IUserRepository userRepository, UserManager<IdentityUser> userManager)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task<IActionResult> List()
        {
            var users = await userRepository.getAll();
            var userViewModel = MapToUserViewModel(users);
            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> List(userviewModel userViewModel)
        {
            var identityResult = await CreateUserAsync(userViewModel);
            if (identityResult.Succeeded)
            {
                TempData["success"] = "User created successfully.";
                return RedirectToAction("List");
            }

            TempData["error"] = "Failed to create user.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                TempData["error"] = "User not found.";
                return RedirectToAction("List");
            }

            var identityResult = await userManager.DeleteAsync(user);
            if (identityResult.Succeeded)
            {
                TempData["success"] = "User deleted successfully.";
                return RedirectToAction("List");
            }

            TempData["error"] = "Failed to delete user.";
            return RedirectToAction("List");
        }

        private userviewModel MapToUserViewModel(IEnumerable<IdentityUser> users)
        {
            var userViewModel = new userviewModel
            {
                Users = new List<User>()
            };

            foreach (var user in users)
            {
                userViewModel.Users.Add(new Models.ViewModel.User
                {
                    Id = Guid.Parse(user.Id),
                    Username = user.UserName,
                    EmailAddress = user.Email
                });
            }

            return userViewModel;
        }

        private async Task<IdentityResult> CreateUserAsync(userviewModel userViewModel)
        {
            var identityUser = new IdentityUser
            {
                UserName = userViewModel.Username,
                Email = userViewModel.Email
            };

            var identityResult = await userManager.CreateAsync(identityUser, userViewModel.Password);
            if (!identityResult.Succeeded)
                return identityResult;

            var roles = new List<string> { "User" };
            if (userViewModel.AdminRoleCheckbox)
            {
                roles.Add("Admin");
            }

            return await userManager.AddToRolesAsync(identityUser, roles);
        }
    }
}
