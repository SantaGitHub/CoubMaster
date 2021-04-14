using System.Threading.Tasks;
using JewishCat.DiscordBot.Models;

namespace JewishCat.DiscordBot.EFCore.Interface
{
    public interface IUserRepository
    {
        public Task SaveEntity();
        public Task AddUser(User user);
        public Task RemoveUser(User user);
        public Task<User> SingleOrDefaultUser(ulong id);
        public Task<bool> CheckIgnoringUser(ulong id);
    }
}