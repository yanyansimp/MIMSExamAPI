using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        void RegisterUser(User user);
    }
}
