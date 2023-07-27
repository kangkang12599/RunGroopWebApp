using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using System.Security.Claims;

namespace RunGroopWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) { 
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<Club>> GetAllUserClubs()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;
            var userClubs = await _context.Clubs.Where(c => c.AppUser.Id == currentUser.GetUserId()).ToListAsync();
            return userClubs;
        }

        public async Task<IEnumerable<Race>> GetAllUserRaces()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;
            var userRaces = await _context.Races.Where(r => r.AppUser.Id == currentUser.GetUserId()).ToListAsync();
            return userRaces;
        }

        public async Task<AppUser?> GetUserById(string id)
        {
            return await _context.Users.Include(u => u.Address).Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
