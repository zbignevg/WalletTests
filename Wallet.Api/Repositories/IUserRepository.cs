using Wallet.Models;

namespace Wallet.Api.Repositories
{   
    public interface IUserRepository
    {
        public Task<User?> GetAsync(string id);
        public Task<User?> GetByEmailAsync(string email);
        public Task CreateAsync(User user);
        public Task UpdateAsync(string id, User user);
        public Task RemoveAsync(string id);
        public Task<User?> GetUserByEmailAndPasswordAsync(string email, string password);
    }
}
