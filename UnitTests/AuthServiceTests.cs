using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System.Text;

namespace UnitTests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Test]
        public void Login_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            string username = "bob";
            string password = "password";
            string hashedPassword = HashPassword(password);

            var user = new User { Username = username, Password = hashedPassword };
            _mockUserRepository.Setup(repo => repo.GetUserByUsername(username)).Returns(user);

            var inputUser = new User { Username = username, Password = password };

            // Act
            var result = _userService.UserCanLogin(inputUser);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(username, result.Username);
        }

        [Test]
        public void Login_WithInvalidCredentials_ShouldReturnNull()
        {
            // Arrange
            string username = "bob";
            string wrongPassword = "wrongpassword";
            string hashedPassword = HashPassword("password");

            var user = new User { Username = username, Password = hashedPassword };
            _mockUserRepository.Setup(repo => repo.GetUserByUsername(username)).Returns(user);

            var inputUser = new User { Username = username, Password = wrongPassword };

            // Act
            var result = _userService.UserCanLogin(inputUser);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Login_WithNonExistentUser_ShouldReturnNull()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByUsername("nonexistent")).Returns((User)null);

            var inputUser = new User { Username = "nonexistent", Password = "password" };

            // Act
            var result = _userService.UserCanLogin(inputUser);

            // Assert
            Assert.IsNull(result);
        }

        //
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
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
    }

}
