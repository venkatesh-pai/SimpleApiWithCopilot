namespace SimpleApiWithCopilot
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>();

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await Task.FromResult(_users);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            return await Task.FromResult(user);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1; // Auto-increment ID
            _users.Add(user);
            return await Task.FromResult(user);
        }

        public async Task<User> UpdateUserAsync(int userId, User updatedUser)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.Age = updatedUser.Age;
            }

            return await Task.FromResult(user);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                _users.Remove(user);
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }
    }
}
