using PlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data {
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreatePlatform(Platform plat)
        {
            if(plat == null) {
                throw new ArgumentNullException(nameof(plat));
            }

            await _context.Platforms.AddAsync(plat);
        }

        public async Task<IEnumerable<Platform>> GetAllPlatforms()
        {
            return await _context.Platforms.ToListAsync();
        }

        public async Task<Platform> GetPlatformById(int id)
        {
            return await _context.Platforms.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> SaveChanges()
        {
            return ((await _context.SaveChangesAsync()) >= 0);
        }
    }
}