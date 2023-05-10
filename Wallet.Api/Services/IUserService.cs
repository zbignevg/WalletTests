using Wallet.Api.Repositories;
using Wallet.Models;

namespace Wallet.Api.Services
{
    public interface IUserService
    {
        public Task<User?> Get(string id);
        public Task<User?> GetByEmail(string email);
        public Task Create(User user);
        public Task Update(string id, User user);
        public Task Remove(string id);
        public Task<User?> GetByEmailAndPassword(string email, string password);
    }
}
