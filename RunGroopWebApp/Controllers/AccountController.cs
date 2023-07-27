using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController (UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, true, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Race");
                    }
                }
            }

            ModelState.AddModelError("", "Wrong email or password. Please try again.");
            return View();
           
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register (RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(registerVM.Email);

            if (user != null)
            { 
                ModelState.AddModelError("", "This email is already in use.");
                return View();
            }

            var newUser = new AppUser()
            {
                Email = registerVM.Email,
                UserName = registerVM.Email,
            };

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                ViewBag.SuccessMessage = "Your account has beeen registered, please login.";
                return View();
            }else
            {
                foreach(var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                    {
                        ModelState.AddModelError("", "The password must contain at least 6 characters, 1 numeric character, 1 lowercase, 1 uppercase and 1 special character.");
                        return View();
                    }
                }
            }
            ModelState.AddModelError("", "System error, please try again later.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Race");
        }
    }
}
