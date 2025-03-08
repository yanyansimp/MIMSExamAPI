using Domain.Entities;
using Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }

        public User UserCanLogin(User user)
        {
            var _user = _userRepository.GetUserByUsername(user.Username);

            if(_user != null && VerifyPassword(user.Password, _user.Password))
                return _user;

            return null;
        }

        public void RegisterUser(User user)
        {
            // Hash password before storing
            user.Password = HashPassword(user.Password);
            _userRepository.RegisterUser(user);
        }

        // Simple hashing
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashedInput = HashPassword(inputPassword);
            return hashedInput == storedHash;
        }
    }

}
