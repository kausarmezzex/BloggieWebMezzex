using Bloggie.web.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bloggie.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser
                {
                    UserName = registerViewModel.UserName,
                    Email = registerViewModel.Email,
                };
                var identityResult = await userManager.CreateAsync(identityUser, registerViewModel.Password);
                if (identityResult.Succeeded)
                {
                    var roleIdentity = await userManager.AddToRoleAsync(identityUser, "User");
                    if (roleIdentity.Succeeded)
                    {
                        TempData["success"] = "Registration successful. Please log in.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["error"] = "Failed to assign role to the user.";
                    }
                }
                else
                {
                    TempData["error"] = string.Join("; ", identityResult.Errors.Select(e => e.Description));
                }
            }
            return View(registerViewModel);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, false, false);
                if (signInResult.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(loginViewModel.UserName);
                    if (await userManager.IsInRoleAsync(user, "Admin"))
                    {
                        TempData["success"] = "Login successful. Welcome, Admin!";
                        return RedirectToAction("Index", "AdminDashboard");
                    }
                    else
                    {
                        TempData["success"] = "Login successful. Welcome!";
                        if (!string.IsNullOrWhiteSpace(loginViewModel.ReturnUrl))
                        {
                            return Redirect(loginViewModel.ReturnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (signInResult.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "User is not allowed to sign in.");
                }
                else if (signInResult.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "User account is locked out.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            TempData["success"] = "You have successfully logged out.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            TempData["error"] = "Access Denied. You do not have permission to access this resource.";
            return View();
        }
    }
}
