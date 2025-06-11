using API.Controllers;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UsersControllerTests
{
    public class UserControllerTests
    {
        private UsersController _controller = null!;
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<IPhotoRepository> _photoRepoMock = null!;
        private Mock<IUserRepository> _userRepoMock = null!;
        private Mock<IPhotoService> _photoServiceMock = null!;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _photoRepoMock = new Mock<IPhotoRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _photoServiceMock = new Mock<IPhotoService>();

            _unitOfWorkMock.Setup(u => u.PhotoRepository).Returns(_photoRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Complete()).ReturnsAsync(true);

            _controller = new UsersController(_unitOfWorkMock.Object, null!, _photoServiceMock.Object, null!);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {

                new Claim(ClaimTypes.NameIdentifier, "testuser"),

                new Claim(ClaimTypes.Name, "testuser")

            }, "mock"));



            _controller.ControllerContext = new ControllerContext
            {

                HttpContext = new DefaultHttpContext { User = user }

            };

        }
        [Test]

        public async Task DeletePhoto_Should_RemovePhoto_IfNotMain()

        {
            // Arrange
            var photo = new Photo
            {
                Id = 1,
                IsMain = false,
                Url = "https://nekilink.com/test.jpg"
            };

            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                KnownAs = "Test",
                Gender = "M",
                City = "vakuf",
                Country = "BiH",
                Photos = new List<Photo> { photo }
            };

            _photoRepoMock.Setup(p => p.GetPhotoById(1)).ReturnsAsync(photo);
            _userRepoMock.Setup(u => u.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

            // Act
            var result = await _controller.DeletePhoto(1);
            // Assert

            ClassicAssert.IsInstanceOf<OkResult>(result);

            ClassicAssert.IsFalse(user.Photos.Contains(photo));

        }

    }
}
