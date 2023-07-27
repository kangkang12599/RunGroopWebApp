using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.Services;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            var races = await _raceRepository.GetAll();
            return View(races);
        }

        public async Task<IActionResult> Details(int id)
        {
            Race race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }

        [Authorize]
        public IActionResult Create()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var raceVM = new CreateRaceVM()
            {
                AppUserId = currentUserId
            };
            return View(raceVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceVM raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);
                if (result.Error != null)
                {
                    ModelState.AddModelError("Image", result.Error.Message);
                    return View();
                }
                Race race = new Race
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    ImageUrl = result.Uri.ToString(),
                    Address = raceVM.Address,
                    AppUserId = raceVM.AppUserId,
                };
                _raceRepository.Add(race);

                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                ModelState.AddModelError("", "The race is not found.");
                return View("Error");
            }

            EditRaceVM raceVM = new EditRaceVM()
            {
                Title = race.Title,
                Description = race.Description,
                Address = race.Address,
                ImageUrl = race.ImageUrl,
                RaceCategory = race.RaceCategory
            };

            return View(raceVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceVM raceVM)
        {
            if (ModelState.IsValid)
            {
                var race = await _raceRepository.GetByIdAsync(id);

                if (race == null)
                {
                    ModelState.AddModelError("", "The club is not found.");
                    return View(raceVM);
                }

                ImageUploadResult? imageUploadResult = null;

                if (raceVM.Image != null)
                {
                    try
                    {
                        await _photoService.DeletePhotoAsync(race.ImageUrl);
                        imageUploadResult = await _photoService.AddPhotoAsync(raceVM.Image);

                        if (imageUploadResult.Error != null)
                        {
                            ModelState.AddModelError("Image", imageUploadResult.Error.Message);
                            return View(raceVM);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Unable to update image.");
                        return View(raceVM);
                    }
                }

                race.Title = raceVM.Title;
                race.Description = raceVM.Description;
                if (imageUploadResult != null)
                {
                    race.ImageUrl = imageUploadResult.SecureUri.ToString();
                }
                race.Address = raceVM.Address;
                race.RaceCategory = raceVM.RaceCategory;

                _raceRepository.Save();

                return RedirectToAction("Index");
            }

            return View(raceVM);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                return View("Error");
            }
            return View(race);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteRace(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                return View("Error");
            }
            _raceRepository.Delete(race);
            return RedirectToAction("Index");
        }
    }
}
