namespace RunGroopWebApp.ViewModels
{
    public class EditProfileVM
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Pace { get; set; }
        public int? Mileage { get; set; }
        public string? ImageUrl { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public IFormFile? Image { get; set; }
    }
}
