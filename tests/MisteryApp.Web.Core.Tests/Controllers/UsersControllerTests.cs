using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Abstractions.Models.Requests;
using MisteryApp.Web.Core.Controllers;
using Moq;

namespace MisteryApp.Web.Core.Tests.Controllers;

[TestClass]
public class UsersControllerTests
{
    private Mock<IUserService> userServiceMock = new(MockBehavior.Strict);
    private UsersController controller = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        userServiceMock = new(MockBehavior.Strict);
        controller = new UsersController(userServiceMock.Object);
    }

    [TestMethod]
    public async Task Login_ShouldReturnOkWithUser_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateUserRequest("johndoe", "John Doe");
        var expectedUser = new User
        {
            UserId = 1,
            Username = "johndoe",
            DisplayName = "John Doe",
            CreatedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetOrCreateUserAsync("johndoe", "John Doe", cancellationToken))
            .ReturnsAsync(expectedUser)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Login(request, cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as ApiResponse<User>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data!.UserId.Should().Be(1);
        response.Data.Username.Should().Be("johndoe");

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetByUsername_ShouldReturnOkWithUser_WhenUserExists()
    {
        // Arrange
        var username = "johndoe";
        var expectedUser = new User
        {
            UserId = 1,
            Username = username,
            DisplayName = "John Doe",
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync(username, cancellationToken))
            .ReturnsAsync(expectedUser)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.GetByUsername(username, cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as ApiResponse<User>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data!.Username.Should().Be(username);

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetByUsername_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "nonexistent";

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync(username, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.GetByUsername(username, cancellationToken);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        var response = notFoundResult!.Value as ApiResponse<User>;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetByUsername_ShouldReturnNotFoundWithMessage_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "ghost";

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync(username, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.GetByUsername(username, cancellationToken);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        var response = notFoundResult!.Value as ApiResponse<User>;
        response.Should().NotBeNull();
        response!.Error.Should().Be($"User not found (Username: {username}).");

        userServiceMock.VerifyAll();
    }
}
