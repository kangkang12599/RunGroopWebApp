using RunGroopWebApp.Data.Enum;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.ViewModels
{
    public class EditRaceVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RaceCategory RaceCategory { get; set; }
        public Address Address { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
    }
}
