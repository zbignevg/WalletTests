using Wallet.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wallet.Api.Repositories;
using Wallet.Api.Services;

namespace Wallet.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRespository)
        {
            _userRepository = userRespository;
        }

        public async Task<User?> Get(string id) => 
            await _userRepository.GetAsync(id);

        public async Task<User?> GetByEmail(string email) => 
            await _userRepository.GetByEmailAsync(email);

        public async Task Create(User user) =>
            await _userRepository.CreateAsync(user);

        public async Task Update(string id, User user) => 
            await _userRepository.UpdateAsync(id, user);

        public async Task Remove(string id) => 
            await _userRepository.RemoveAsync(id);

        public async Task<User?> GetByEmailAndPassword(string email, string password) => 
            await _userRepository.GetUserByEmailAndPasswordAsync(email, password);
    }
}
