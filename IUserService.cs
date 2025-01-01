namespace SimpleApiWithCopilot
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(int userId, User updatedUser);
        Task<bool> DeleteUserAsync(int userId);
    }
}
