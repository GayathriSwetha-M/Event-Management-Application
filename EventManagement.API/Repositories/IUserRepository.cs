namespace EventManagement.API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<bool> ExistsByEmailOrPhoneAsync(string emailOrPhone);
    }
}

