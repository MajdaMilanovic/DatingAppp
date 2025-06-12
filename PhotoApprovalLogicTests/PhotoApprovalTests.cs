//using API.Entities;
//using API.Interfaces;
//using API.Services;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace PhotoApprovalLogicTests
//{
//    public class PhotoApprovalTests
//    {

//        //XUnit and Moq

//        [Fact]
//            public async Task ApprovePhotoAsync_SetsIsApprovedToTrue()
//        {
//            // Arrange
//            var mockRepo = new Mock<IPhotoRepository>();

//            var photo = new Photo
//            {
//                Id = 1,
//                IsApproved = false,
//                Url = "http://example.com/photo.jpg",
//                AppUserId = 1
//            };

//            var user = new AppUser
//            {
//                Id = 1,
//                UserName = "testuser",
//                MainPhotoId = null,
//                KnownAs = "test",
//                Gender = "F",
//                City = "Mostar",
//                Country = "BiH"
//            };

//            mockRepo.Setup(r => r.GetPhotoById(1)).ReturnsAsync(photo);
//            mockRepo.Setup(r => r.GetUserByPhotoId(1)).ReturnsAsync(user);
//            mockRepo.Setup(r => r.SaveChangesAsync());

//            var service = new PhotoService(mockRepo.Object);

//            // Act
//            await service.ApprovePhotoAsync(1);

//            // Assert
//            Assert.True(photo.IsApproved);
//            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
//        }


//        [Fact]
//        public async Task ApprovePhotoAsync_SetsMainPhotoIfNoneExists()
//        {
//            // Arrange
//            var mockRepo = new Mock<IPhotoRepository>();
//            var photo = new Photo { 
//                Id = 2,
//                IsApproved = false,
//                Url = "http://example.com/photo.jpg"
//            };
//            var user = new AppUser {
//                Id = 1,
//                MainPhotoId = null,
//                UserName = "TestUser",
//                KnownAs = "test",
//                Gender = "F",
//                City = "Mostar",
//                Country = "BiH"
//            };

//            mockRepo.Setup(r => r.GetPhotoById(2)).ReturnsAsync(photo);
//            mockRepo.Setup(r => r.GetUserByPhotoId(2)).ReturnsAsync(user);

//            var service = new PhotoService(mockRepo.Object);

//            // Act
//            await service.ApprovePhotoAsync(2);

//            // Assert
//            Assert.Equal(2, user.MainPhotoId);
//        }

//        [Fact]
//        public async Task ApprovePhotoAsync_DoesNotChangeMainPhotoIfAlreadySet()
//        {
//            // Arrange
//            var mockRepo = new Mock<IPhotoRepository>();
//            var photo = new Photo {
//                Id = 3, 
//                IsApproved = false,
//                Url = "http://example.com/photo.jpg"
//            };
//            var user = new AppUser {
//                Id = 1,
//                MainPhotoId = 99,
//                UserName = "TestUser",
//                KnownAs = "test",
//                Gender = "F",
//                City = "Mostar",
//                Country = "BiH"
//            };

//            mockRepo.Setup(r => r.GetPhotoById(3)).ReturnsAsync(photo);
//            mockRepo.Setup(r => r.GetUserByPhotoId(3)).ReturnsAsync(user);

//            var service = new PhotoService(mockRepo.Object);

//            // Act
//            await service.ApprovePhotoAsync(3);

//            // Assert
//            Assert.Equal(99, user.MainPhotoId);
//        }


//    }
//}
