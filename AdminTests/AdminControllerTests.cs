using API.Controllers;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminTests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<IPhotoRepository> _photoRepoMock = null!;
        private Mock<IUserRepository> _userRepoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private AdminConroller _controller = null!;
        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _photoRepoMock = new Mock<IPhotoRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock.Setup(u => u.PhotoRepository).Returns(_photoRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Complete()).ReturnsAsync(true);


            _controller = new AdminConroller(null!, _unitOfWorkMock.Object, null!, null!, mapper: _mapperMock.Object);

        }

        [Test]
        public async Task ApprovePhoto_Should_SetIsApprovedToTrue_And_SetAsMainIfNone()
        {
            //Arrange
                       var photo = new Photo { Id = 1, Url = "https://nekilink.com/", IsApproved = false };
            var user = new AppUser
            {
                Id = 1,
                Photos = new List<Photo> { photo },
                KnownAs = "testing",
                City = "Mostar",
                Country = "BiH",
                Gender = "female"

            };
            _photoRepoMock.Setup(r => r.GetPhotoById(1)).ReturnsAsync(photo);
            _userRepoMock.Setup(r => r.GetUserByPhotoId(1)).ReturnsAsync(user);
            // Act
            var result = await _controller.ApprovePhoto(1);
            // Assert
            ClassicAssert.IsInstanceOf<OkResult>(result);
            ClassicAssert.IsTrue(photo.IsApproved);
            ClassicAssert.IsTrue(photo.IsMain);
        }
    }

}
