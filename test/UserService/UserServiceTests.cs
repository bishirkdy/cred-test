using Moq;
using UserService.Interfaces;
using UserService.Models;
using UserService.Services;
using Xunit;


namespace UserServices.Test
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mokRepo;
        private readonly UserService.Services.UserService _userService;

        public UserServiceTests()
        {
            _mokRepo = new Mock<IUserRepository>();
            _userService = new UserService.Services.UserService(_mokRepo.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllUsers()
        {
            var users = new List<User>
            {
                new() { Id = 1, Name = "bishir" },
                new() { Id = 2, Name = "roufiya"}
            };

            _mokRepo.Setup(r => r.GetAll()).Returns(users);
            var result = _userService.GetAll();

            Assert.Equal(2 , result.Count);
            Assert.Equal("bishir", result[0].Name);
            Assert.Equal("roufiya", result[1].Name);
        }

        [Fact]
        public void GetAll_WhenEmpty_ShouldReturnEmptyList()
        {
            _mokRepo.Setup(r => r.GetAll()).Returns(new List<User>());
            var result = _userService.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetById_ValidId_ShouldReturnUser()
        {
            var user = new User { Id = 1, Name = "bishir" };

            _mokRepo.Setup(r => r.GetById(user.Id)).Returns(user);

            var result = _userService.GetById(user.Id);
            Assert.NotNull(result);
            Assert.Equal(1 , result.Id);
            Assert.Equal("bishir" , result.Name);
        }

        [Theory]
        [InlineData("" , "bishir@gmail.com")]
        [InlineData("bishir" , "")]
        [InlineData(" " , "bishir@gmail.com")]
        [InlineData(null , "bishir@gmail.com")]
        [InlineData("bishir" , null)]
        [InlineData("bishir" , " ")]
        public void Add_InvalidUser_ShouldThrowException(string? name , string? email)
        {
            var user = new User { Name = name! , Email = email! };

            Assert.Throws<ArgumentException>(() => _userService.Add(user));
            _mokRepo.Verify(r => r.Add(It.IsAny<User>()) , Times.Never());
        }

        [Fact]
        public void Add_NullUser_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => _userService.Add(null));
            _mokRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Never());
        }

        [Fact]
        public void Add_DuplicateEmail_ShouldThrowException()
        {
            var existing = new User { Id = 1, Name = "Rahul", Email = "same@example.com" };
            var newcomer = new User { Id = 2, Name = "Bishir", Email = "same@example.com" };
            _mokRepo.Setup(r => r.GetByEmail("same@example.com")).Returns(existing);

            Assert.Throws<InvalidOperationException>(() => _userService.Add(newcomer));
            _mokRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Never());

        }

        [Fact]
        public void Update_ExistingUser_ShouldCallRepositoryUpdate()
        {
            var oldUser = new User { Id = 1, Name = "Old Name", Email = "old@example.com" };
            var updated = new User { Id = 1, Name = "Bishir", Email = "new@example.com" };
            _mokRepo.Setup(r => r.GetById(1)).Returns(oldUser);

            _userService.Update(updated);

            _mokRepo.Verify(r => r.Update(It.Is<User>(u =>
              u.Id == 1 && u.Name == "Bishir" && u.Email == "new@example.com")), Times.Once());
            
        }
        [Fact]
        public void Update_UserNotFound_ShouldThrowException()
        {
            var user = new User { Id = 99, Name = "Bishir", Email = "bishir@example.com" };
            _mokRepo.Setup(r => r.GetById(99)).Returns((User?)null);

            Assert.Throws<KeyNotFoundException>(() => _userService.Update(user));
            _mokRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never());
        }

        [Fact]
        public void Remove_ExistingUser_ShouldDeleteUser()
        {
            _mokRepo.Setup(r => r.GetById(1)).Returns(new User { Id = 1, Name = "Bishir" });

            _userService.Remove(1);

            _mokRepo.Verify(r => r.Delete(1), Times.Once());
        }

        [Fact]
        public void Remove_InvalidId_ShouldNotCallDelete()
        {
            Assert.Throws<ArgumentException>(() => _userService.Remove(-1));
            _mokRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void Remove_UserNotFound_ShouldNotDelete()
        {
            _mokRepo.Setup(r => r.GetById(99)).Returns((User?)null);

            Assert.Throws<KeyNotFoundException>(() => _userService.Remove(99));
            _mokRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never());
        }

    }
}
