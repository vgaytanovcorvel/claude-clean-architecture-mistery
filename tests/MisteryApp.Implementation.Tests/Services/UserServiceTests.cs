using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Implementation.Services;
using Moq;

namespace MisteryApp.Implementation.Tests.Services;

[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> userRepositoryMock = new(MockBehavior.Strict);
    private FakeTimeProvider timeProvider = null!;
    private Mock<UserService> userServiceMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        userRepositoryMock = new(MockBehavior.Strict);
        timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero));

        userServiceMock = new Mock<UserService>(
            () => new UserService(userRepositoryMock.Object, timeProvider),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenUserExists()
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
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByUsernameAsync(username, cancellationToken))
            .ReturnsAsync(expectedUser)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetUserByUsernameAsync(username, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(1);
        result.Username.Should().Be(username);
        result.DisplayName.Should().Be("John Doe");

        userServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "nonexistent";

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync(username, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByUsernameAsync(username, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetUserByUsernameAsync(username, cancellationToken);

        // Assert
        result.Should().BeNull();

        userServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetOrCreateUserAsync_ShouldReturnExistingUser_WhenUsernameExists()
    {
        // Arrange
        var username = "existinguser";
        var displayName = "Existing User";
        var existingUser = new User
        {
            UserId = 5,
            Username = username,
            DisplayName = displayName,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetOrCreateUserAsync(username, displayName, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByUsernameAsync(username, cancellationToken))
            .ReturnsAsync(existingUser)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetOrCreateUserAsync(username, displayName, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(5);
        result.Username.Should().Be(username);
        result.DisplayName.Should().Be(displayName);

        userServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetOrCreateUserAsync_ShouldCreateAndReturnNewUser_WhenUsernameDoesNotExist()
    {
        // Arrange
        var username = "newuser";
        var displayName = "New User";
        var expectedCreatedAt = timeProvider.GetUtcNow();
        var createdUser = new User
        {
            UserId = 10,
            Username = username,
            DisplayName = displayName,
            CreatedAt = expectedCreatedAt
        };

        userServiceMock
            .Setup(s => s.GetOrCreateUserAsync(username, displayName, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByUsernameAsync(username, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserAddAsync(
                It.Is<User>(u =>
                    u.Username == username &&
                    u.DisplayName == displayName &&
                    u.CreatedAt == expectedCreatedAt),
                cancellationToken))
            .ReturnsAsync(createdUser)
            .Verifiable(Times.Once());

        // Act
        var result = await userServiceMock.Object.GetOrCreateUserAsync(username, displayName, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(10);
        result.Username.Should().Be(username);
        result.DisplayName.Should().Be(displayName);
        result.CreatedAt.Should().Be(expectedCreatedAt);

        userServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }
}
