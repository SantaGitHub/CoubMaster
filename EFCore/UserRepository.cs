using System.Linq;
using System.Threading.Tasks;
using JewishCat.DiscordBot.EFCore.Interface;
using JewishCat.DiscordBot.Models;

namespace JewishCat.DiscordBot.EFCore
{
    public class UserRepository : IUserRepository
    {
        private readonly LocalDbContext _localDbContext;

        public UserRepository(LocalDbContext localDbContext)
        {
            _localDbContext = localDbContext;
        }

        public async Task SaveEntity()
        {
            await _localDbContext.SaveChangesAsync();
        }

        public Task AddUser(User user)
        {
            _localDbContext.Users.Add(user);
            return Task.CompletedTask;
        }

        public Task RemoveUser(User user)
        {
            _localDbContext.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<User> SingleOrDefaultUser(ulong id)
        {
            return await _localDbContext.Users.SingleOrDefaultAsync(item => item.Id == id);
        }

        public async Task<bool> CheckIgnoringUser(ulong id)
        {
            var user = await _localDbContext.Users.SingleOrDefaultAsync(item => item.Id == id);
            return user is {Ignore: true};
        }
    }
}