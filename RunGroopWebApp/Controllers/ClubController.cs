using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            var club = await _clubRepository.GetAll();
            return View(club);
        }

        public async Task<IActionResult> Details(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }

        [Authorize]
        public IActionResult Create()
        {
            var currenUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var clubVM = new CreateClubVM()
            {
                AppUserId = currenUserId
            };
            
            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubVM clubVM)
        {
            if(ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(clubVM.Image);
                if(result.Error != null)
                {
                    ModelState.AddModelError("Image", result.Error.Message);
                    return View();
                }
                Club club = new Club
                {
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    ImageUrl = result.Uri.ToString(),
                    Address = clubVM.Address,
                    AppUserId = clubVM.AppUserId,
                };

                _clubRepository.Add(club);

                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return View("Error");
            }

            EditClubVM clubVM = new EditClubVM()
            {
                Title= club.Title,
                Description = club.Description,
                Address = club.Address,
                ImageUrl = club.ImageUrl,
                ClubCategory = club.ClubCategory
            };
            ModelState.AddModelError("", "The club is not found.");

            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubVM clubVM)
        {
            if (ModelState.IsValid)
            {
                var club = await _clubRepository.GetByIdAsync(id);

                if (club == null)
                {
                    ModelState.AddModelError("", "The club is not found.");
                    return View(clubVM);
                }

                ImageUploadResult? imageUploadResult = null;

                if (clubVM.Image != null)
                {
                    try 
                    {
                        await _photoService.DeletePhotoAsync(club.ImageUrl);
                        imageUploadResult = await _photoService.AddPhotoAsync(clubVM.Image);

                        if (imageUploadResult.Error != null)
                        {
                            ModelState.AddModelError("Image", imageUploadResult.Error.Message);
                            return View(clubVM);
                        }
                    } 
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Unable to update image.");
                        return View(clubVM);
                    }
                }

                club.Title = clubVM.Title;
                club.Description = clubVM.Description;
                if (imageUploadResult != null)
                {
                    club.ImageUrl = imageUploadResult.SecureUri.ToString();
                }
                club.Address = clubVM.Address;
                club.ClubCategory = clubVM.ClubCategory;

                _clubRepository.Save();

                return RedirectToAction("Index");
            }

            return View(clubVM);
        }

        [Authorize]
        public async Task<IActionResult> Delete (int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return View("Error");
            }
            return View(club);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub (int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return View("Error");
            }
            _clubRepository.Delete(club);
            return RedirectToAction("Index");
        }
    }
}
