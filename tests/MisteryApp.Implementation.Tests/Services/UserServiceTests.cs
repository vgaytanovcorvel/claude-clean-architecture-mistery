using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Implementation.Services;

namespace MisteryApp.Implementation.Tests.Services;

[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> userRepositoryMock = new(MockBehavior.Strict);
    private Mock<UserService> userServiceMock = null!;

    private readonly CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);

        userServiceMock = new Mock<UserService>(
            () => new UserService(userRepositoryMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers_WhenUsersExist()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new() { Id = 1, Name = "Alice", AvatarUrl = "https://example.com/alice.png" },
            new() { Id = 2, Name = "Bob", AvatarUrl = "https://example.com/bob.png" },
        };

        userServiceMock
            .Setup(s => s.GetAllUsersAsync(cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserGetAllAsync(cancellationToken))
            .ReturnsAsync(expectedUsers)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetAllUsersAsync(cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedUsers);

        userServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var expectedUsers = new List<User>();

        userServiceMock
            .Setup(s => s.GetAllUsersAsync(cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserGetAllAsync(cancellationToken))
            .ReturnsAsync(expectedUsers)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetAllUsersAsync(cancellationToken);

        // Assert
        result.Should().BeEmpty();

        userServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User { Id = userId, Name = "Alice", AvatarUrl = "https://example.com/alice.png" };

        userServiceMock
            .Setup(s => s.GetUserByIdAsync(userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByIdAsync(userId, cancellationToken))
            .ReturnsAsync(expectedUser)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetUserByIdAsync(userId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Name.Should().Be("Alice");

        userServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;

        userServiceMock
            .Setup(s => s.GetUserByIdAsync(userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByIdAsync(userId, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetUserByIdAsync(userId, cancellationToken);

        // Assert
        result.Should().BeNull();

        userServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }
}
