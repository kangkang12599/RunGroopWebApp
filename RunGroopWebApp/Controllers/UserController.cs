using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;

namespace RunGroopWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository) { 
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            return View(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user.ImageUrl == null || user.ImageUrl == "")
            {
                user.ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRlTGvbQ2lOzbDJdw1tCCqraoPCIyfAIY0EHg&usqp=CAU";
            }
            return View(user);
        }
    }
}
