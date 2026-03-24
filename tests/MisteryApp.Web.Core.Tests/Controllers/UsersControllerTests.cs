using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Web.Core.Controllers;

namespace MisteryApp.Web.Core.Tests.Controllers;

[TestClass]
public class UsersControllerTests
{
    private Mock<IUserService> userServiceMock = new(MockBehavior.Strict);
    private UsersController controller = null!;

    private readonly CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        controller = new UsersController(userServiceMock.Object);
    }

    [TestMethod]
    public async Task GetUsers_ShouldReturnOkWithUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Name = "Alice" },
            new() { Id = 2, Name = "Bob" },
        };

        userServiceMock
            .Setup(s => s.GetAllUsersAsync(cancellationToken))
            .ReturnsAsync(users)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.GetUsers(cancellationToken);

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IReadOnlyList<User>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetUser_ShouldReturnOkWithUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Name = "Alice", AvatarUrl = "https://example.com/alice.png" };

        userServiceMock
            .Setup(s => s.GetUserByIdAsync(userId, cancellationToken))
            .ReturnsAsync(user)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.GetUser(userId, cancellationToken);

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<User>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(userId);

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;

        userServiceMock
            .Setup(s => s.GetUserByIdAsync(userId, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.GetUser(userId, cancellationToken);

        // Assert
        var notFoundResult = actionResult.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse<User>>().Subject;
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Error.Should().Contain("999");

        userServiceMock.VerifyAll();
    }
}
