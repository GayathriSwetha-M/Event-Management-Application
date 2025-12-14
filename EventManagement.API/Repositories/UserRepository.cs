using Microsoft.EntityFrameworkCore;
using EventManagement.API.Data;

namespace EventManagement.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.EmailOrPhone == emailOrPhone);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsByEmailOrPhoneAsync(string emailOrPhone)
        {
            return await _context.Users
                .AnyAsync(u => u.EmailOrPhone == emailOrPhone);
        }
    }
}

