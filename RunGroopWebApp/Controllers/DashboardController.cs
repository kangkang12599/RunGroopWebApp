using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;

        public DashboardController(IDashboardRepository dashboardRepository, IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
        {
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
        }

        [Authorize]
        public async Task<IActionResult> Index(string category)
        {
            if (category == null || category == "club")
            {
                var userClubs = await _dashboardRepository.GetAllUserClubs();
                var dashboard = new DashboardVM()
                {
                    Clubs = userClubs.ToList(),
                    Races = new List<Race>()
                };
                ViewBag.Category = "Your Clubs";
                return View(dashboard);
            }

            if (category == "race")
            {
                var userRaces = await _dashboardRepository.GetAllUserRaces();
                var dashboard = new DashboardVM()
                {
                    Clubs = new List<Club>(),
                    Races = userRaces.ToList(),
                };
                ViewBag.Category = "Your Races";
                return View(dashboard);
            }

            return View("Error");
        }

        [Authorize]
        public async Task<IActionResult> EditUserProfile()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var user = await _dashboardRepository.GetUserById(currentUserId!);

            if (user == null)
            {
                return View("Error");
            }

            string? imageUrl = user.ImageUrl;
            if (user.ImageUrl == null)
            {
                imageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRlTGvbQ2lOzbDJdw1tCCqraoPCIyfAIY0EHg&usqp=CAU";
            }

            var editProfileVM = new EditProfileVM()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.UserName,
                Pace = user.Pace,
                Mileage = user.Mileage,
                Street = user.Address?.Street,
                City = user.Address?.City,
                State = user.Address?.State,
                ImageUrl = imageUrl
            };

            return View(editProfileVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditProfileVM editProfileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(editProfileVM);
            }

            var user = await _dashboardRepository.GetUserById(editProfileVM.Id);
            if (user == null)
            {
                ModelState.AddModelError("", "System error, please try again later.");
                return View(editProfileVM);
            }

            user.UserName = editProfileVM.Name;
            user.Pace = editProfileVM.Pace;
            user.Mileage = editProfileVM.Mileage;

            if (user.Address != null)
            {
                user.Address.Street = editProfileVM.Street;
                user.Address.City = editProfileVM.City;
                user.Address.State = editProfileVM.State;
            }
            else
            {
                user.Address = new Address()
                {
                    Street = editProfileVM.Street,
                    City = editProfileVM.City,
                    State = editProfileVM.State
                }; 
            }

            if (editProfileVM.Image != null)
            {
                try
                {
                    var uploadResult = await _photoService.AddPhotoAsync(editProfileVM.Image);

                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError("Image", uploadResult.Error.Message);
                        return View(editProfileVM);
                    }

                    if (user.ImageUrl != "" && user.ImageUrl != null)
                    {
                        var deletionResult = await _photoService.DeletePhotoAsync(user.ImageUrl!);
                        if (deletionResult.Error != null)
                        {
                            ModelState.AddModelError("", "Fail to upload your image, please try again later.");
                            return View(editProfileVM);
                        }
                    }

                    user.ImageUrl = editProfileVM.ImageUrl = uploadResult.Url.ToString();

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Fail to upload your image, please try again later.");
                    return View(editProfileVM);
                }
            }

            _dashboardRepository.Save();
            ViewBag.success = "Your profile is updated.";
            return View(editProfileVM);
        }
    }
}
