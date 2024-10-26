using ConsoleApp1.Entity;

namespace ConsoleApp1.Repository
{
    public interface IUserRepository
    {
        void CreateUser(User user);
        User GetUserByEmail(string email);
        User GetUserById(int id);
        void UpdateUser(User user);
    }
}
