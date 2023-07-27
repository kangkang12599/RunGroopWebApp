using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IDashboardRepository
    {
        Task<IEnumerable<Race>> GetAllUserRaces();
        Task<IEnumerable<Club>> GetAllUserClubs();
        Task<AppUser?> GetUserById(string id);
        bool Save();
    }
}
