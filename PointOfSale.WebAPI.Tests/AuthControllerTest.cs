using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PointOfSale.WebAPI.Configuration;
using PointOfSale.WebAPI.Controllers;
using PointOfSale.WebAPI.ViewModels.Requests;
using PointOfSale.WebAPI.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PointOfSale.WebAPI.Tests
{
    public class AuthControllerTest
    {
        private Mock<IOptions<JwtBearerTokenSettings>> _mockJwtBearerTokenSettings;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<IPasswordHasher<IdentityUser>> _mockPasswordHasher;

        public AuthControllerTest()
        {
            _mockJwtBearerTokenSettings = new Mock<IOptions<JwtBearerTokenSettings>>();
            _mockUserManager = GetUserManagerMock<IdentityUser>();
            _mockPasswordHasher = new Mock<IPasswordHasher<IdentityUser>>();
        }

        [Fact]
        public async Task RegisterReturnOk()
        {
            // Arrange
            _mockUserManager
                .Setup(userManager => userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Verifiable();
            _mockUserManager
                .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser());

            // Act
            var controller = new AuthController(_mockJwtBearerTokenSettings.Object, _mockUserManager.Object);
            var result = await controller.Register(new RegisterRequest()
            {
                Email = "email@gmail.com",
                UserName = "email",
                Password = "1234"
            });

            // Assert
            _mockUserManager.Verify(userManager => userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task RegisterReturnBadRequestUserNull()
        {
            // Arrange
            _mockUserManager
                .Setup(userManager => userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Verifiable();
            _mockUserManager
                .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser());

            // Act
            var controller = new AuthController(_mockJwtBearerTokenSettings.Object, _mockUserManager.Object);
            var result = await controller.Register(null);

            // Assert
            _mockUserManager.Verify(userManager => userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response>(badRequestObjectResult?.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task RegisterReturnBadRequestCreateAsyncFail()
        {
            // Arrange
            _mockUserManager
                .Setup(userManager => userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed()).Verifiable();
            _mockUserManager
                .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser());

            // Act
            var controller = new AuthController(_mockJwtBearerTokenSettings.Object, _mockUserManager.Object);
            var result = await controller.Register(new RegisterRequest()
            { 
                Email = "email@gmail.com",
                UserName = "email",
                Password = "password"
            });

            // Assert
            _mockUserManager.Verify(userManager => userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response>(badRequestObjectResult?.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task LoginReturnOk()
        {
            // Arrange
            _mockUserManager
                .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser()
                    { 
                        Email = "mail@gmail.com",
                        UserName = "username"
                    }).Verifiable();

            _mockPasswordHasher.Setup(passwordHasher => passwordHasher.VerifyHashedPassword(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success).Verifiable();

            _mockUserManager.Object.PasswordHasher = _mockPasswordHasher.Object;

            _mockJwtBearerTokenSettings
                .SetupGet(jwt => jwt.Value)
                .Returns(new JwtBearerTokenSettings()
                    {
                        SecretKey = "asd2lkj545f45halS8989D3d3skFGjGfhlBskWd227K3jfhVFs5dYjfh",
                        Audience = "https://localhost:44395/", 
                        Issuer = "https://localhost:44395/",
                        ExpiryTimeInSeconds = 60000
                }).Verifiable();


            // Act
            var controller = new AuthController(_mockJwtBearerTokenSettings.Object, _mockUserManager.Object);
            var result = await controller.Login(new LoginRequest()
            {
                Username = "username",
                Password = "password"
            });

            // Assert
            _mockUserManager.Verify(userManager => userManager.FindByNameAsync(It.IsAny<string>()), Times.Once);
            _mockPasswordHasher.Verify(passwordHasher => passwordHasher.VerifyHashedPassword(
                It.IsAny<IdentityUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response>(okObjectResult?.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task LoginReturnBadRequestPassword()
        {
            // Arrange
            _mockUserManager
                .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser()
                {
                    Email = "mail@gmail.com",
                    UserName = "username"
                }).Verifiable();

            _mockPasswordHasher.Setup(passwordHasher => passwordHasher.VerifyHashedPassword(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Failed).Verifiable();

            _mockUserManager.Object.PasswordHasher = _mockPasswordHasher.Object;

            _mockJwtBearerTokenSettings
                .SetupGet(jwt => jwt.Value)
                .Returns(new JwtBearerTokenSettings()
                {
                    SecretKey = "asd2lkj545f45halS8989D3d3skFGjGfhlBskWd227K3jfhVFs5dYjfh",
                    Audience = "https://localhost:44395/",
                    Issuer = "https://localhost:44395/",
                    ExpiryTimeInSeconds = 60000
                });


            // Act
            var controller = new AuthController(_mockJwtBearerTokenSettings.Object, _mockUserManager.Object);
            var result = await controller.Login(new LoginRequest()
            {
                Username = "username",
                Password = "password"
            });

            // Assert
            _mockUserManager.Verify(userManager => userManager.FindByNameAsync(It.IsAny<string>()), Times.Once);
            _mockPasswordHasher.Verify(passwordHasher => passwordHasher.VerifyHashedPassword(
                It.IsAny<IdentityUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            var badObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response>(badObjectResult?.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task LoginReturnBadRequestUserNotExists()
        {
            // Arrange
            _mockUserManager
                .Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null).Verifiable();

            _mockPasswordHasher.Setup(passwordHasher => passwordHasher.VerifyHashedPassword(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Failed).Verifiable();

            _mockUserManager.Object.PasswordHasher = _mockPasswordHasher.Object;

            _mockJwtBearerTokenSettings
                .SetupGet(jwt => jwt.Value)
                .Returns(new JwtBearerTokenSettings()
                {
                    SecretKey = "asd2lkj545f45halS8989D3d3skFGjGfhlBskWd227K3jfhVFs5dYjfh",
                    Audience = "https://localhost:44395/",
                    Issuer = "https://localhost:44395/",
                    ExpiryTimeInSeconds = 60000
                });


            // Act
            var controller = new AuthController(_mockJwtBearerTokenSettings.Object, _mockUserManager.Object);
            var result = await controller.Login(new LoginRequest()
            {
                Username = "username",
                Password = "password"
            });

            // Assert
            _mockUserManager.Verify(userManager => userManager.FindByNameAsync(It.IsAny<string>()), Times.Once);
            _mockPasswordHasher.Verify(passwordHasher => passwordHasher.VerifyHashedPassword(
                It.IsAny<IdentityUser>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
            var badObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response>(badObjectResult?.Value);
            Assert.False(response.Success);
        }

        private Mock<UserManager<TIDentityUser>> GetUserManagerMock<TIDentityUser>() where TIDentityUser : IdentityUser
        {
            return new Mock<UserManager<TIDentityUser>>(
                    new Mock<IUserStore<TIDentityUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<TIDentityUser>>().Object,
                    new IUserValidator<TIDentityUser>[0],
                    new IPasswordValidator<TIDentityUser>[0],
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<TIDentityUser>>>().Object);
        }

        private Mock<RoleManager<TIdentityRole>> GetRoleManagerMock<TIdentityRole>() where TIdentityRole : IdentityRole
        {
            return new Mock<RoleManager<TIdentityRole>>(
                    new Mock<IRoleStore<TIdentityRole>>().Object,
                    new IRoleValidator<TIdentityRole>[0],
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<ILogger<RoleManager<TIdentityRole>>>().Object);
        }
    }
}
