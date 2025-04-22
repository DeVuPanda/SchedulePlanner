using Moq;
using ScheduleDomain.Model;
using ScheduleInfrastructure.Services;
using System.Collections.Generic;
using Xunit;

namespace MyProject.Tests
{
    public class UserServiceMockTests
    {
        [Fact]
        public void GetUserByEmail_ReturnsCorrectUser()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.GetUserByEmail("john@uni.com"))
                .Returns(new User { FullName = "John Smith", Email = "john@uni.com" });

            var result = mock.Object.GetUserByEmail("john@uni.com");

            Assert.NotNull(result);
            Assert.Equal("John Smith", result.FullName);
            Assert.Equal("john@uni.com", result.Email);
        }

        [Fact]
        public void AddUser_ShouldBeCalledOnce()
        {
            var mock = new Mock<IUserService>();

            var user = new User { FullName = "New User", Email = "new@uni.com" };
            mock.Object.AddUser(user);

            mock.Verify(s => s.AddUser(It.Is<User>(u => u.Email == "new@uni.com")), Times.Once);
        }

        [Fact]
        public void DeleteUser_CalledWithCorrectId_InOrder()
        {
            var mock = new Mock<IUserService>();

            var sequence = new MockSequence();
            mock.InSequence(sequence).Setup(s => s.DeleteUser(1));
            mock.InSequence(sequence).Setup(s => s.DeleteUser(2));

            mock.Object.DeleteUser(1);
            mock.Object.DeleteUser(2);

            mock.Verify(s => s.DeleteUser(1), Times.Once);
            mock.Verify(s => s.DeleteUser(2), Times.Once);
        }

        [Fact]
        public void GetAllTeachers_ReturnsDifferentResultsPerCall()
        {
            var mock = new Mock<IUserService>();
            mock.SetupSequence(s => s.GetAllTeachers())
                .Returns(new List<User> { new User { FullName = "A" } })
                .Returns(new List<User> { new User { FullName = "B" } });

            var first = mock.Object.GetAllTeachers();
            var second = mock.Object.GetAllTeachers();

            Assert.Equal("A", first.First().FullName);
            Assert.Equal("B", second.First().FullName);
        }

        [Fact]
        public void GetUserByEmail_ThrowsException_WhenEmailIsNull()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.GetUserByEmail(null!))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => mock.Object.GetUserByEmail(null!));
        }
    }
}
