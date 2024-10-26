using ConsoleApp1.Entity;

namespace ConsoleApp1.Service
{
    public interface IUserService
    {
        void Register(User user);
        User? Login(string email, string password);
        User GetUserByEmail(string email);
        User GetUserById(int id);
    }
}
