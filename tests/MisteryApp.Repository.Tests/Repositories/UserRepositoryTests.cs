using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Models;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Entities;
using MisteryApp.Repository.Repositories;
using Moq;
using Moq.Protected;

namespace MisteryApp.Repository.Tests.Repositories;

[TestClass]
public class UserRepositoryTests
{
    private Mock<IDbContextFactory<ApplicationDbContext>> contextFactoryMock = new(MockBehavior.Strict);
    private Mock<UserRepository> userRepositoryMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        contextFactoryMock = new(MockBehavior.Strict);

        userRepositoryMock = new Mock<UserRepository>(
            () => new UserRepository(contextFactoryMock.Object),
            MockBehavior.Strict);
    }

    private static ApplicationDbContext CreateInMemoryContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName ?? Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private void SetupCreateContext(string dbName)
    {
        userRepositoryMock.Protected()
            .Setup<Task<ApplicationDbContext>>("CreateContextAsync", ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(CreateInMemoryContext(dbName))
            .Verifiable(Times.Once());
    }

    private async Task SeedUserAsync(string dbName, UserEntity entity)
    {
        await using var context = CreateInMemoryContext(dbName);
        await context.User.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    [TestMethod]
    public async Task UserSingleByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User
        {
            UserId = userId,
            Username = "johndoe",
            DisplayName = "John Doe",
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        userRepositoryMock
            .Setup(r => r.UserSingleByIdAsync(userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByIdAsync(userId, cancellationToken))
            .ReturnsAsync(expectedUser)
            .Verifiable(Times.Once());

        // Act
        var result = await userRepositoryMock.Object.UserSingleByIdAsync(userId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Username.Should().Be("johndoe");

        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UserSingleByIdAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;

        userRepositoryMock
            .Setup(r => r.UserSingleByIdAsync(userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByIdAsync(userId, cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => userRepositoryMock.Object.UserSingleByIdAsync(userId, cancellationToken));

        exception.Message.Should().Be($"User not found (UserId: {userId}).");

        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await SeedUserAsync(dbName, new UserEntity
        {
            UserId = 1,
            Username = "johndoe",
            DisplayName = "John Doe",
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByIdAsync(1, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await userRepositoryMock.Object.UserSingleOrDefaultByIdAsync(1, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(1);
        result.Username.Should().Be("johndoe");
        result.DisplayName.Should().Be("John Doe");

        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByIdAsync(999, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await userRepositoryMock.Object.UserSingleOrDefaultByIdAsync(999, cancellationToken);

        // Assert
        result.Should().BeNull();

        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByUsernameAsync_ShouldReturnUser_WhenUsernameExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await SeedUserAsync(dbName, new UserEntity
        {
            UserId = 1,
            Username = "janedoe",
            DisplayName = "Jane Doe",
            CreatedAt = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero)
        });

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByUsernameAsync("janedoe", cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await userRepositoryMock.Object.UserSingleOrDefaultByUsernameAsync("janedoe", cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("janedoe");
        result.DisplayName.Should().Be("Jane Doe");

        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByUsernameAsync_ShouldReturnNull_WhenUsernameDoesNotExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();

        userRepositoryMock
            .Setup(r => r.UserSingleOrDefaultByUsernameAsync("nonexistent", cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await userRepositoryMock.Object.UserSingleOrDefaultByUsernameAsync("nonexistent", cancellationToken);

        // Assert
        result.Should().BeNull();

        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UserAddAsync_ShouldReturnCreatedUser_WhenUserIsValid()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var user = new User
        {
            UserId = 0,
            Username = "newuser",
            DisplayName = "New User",
            CreatedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        userRepositoryMock
            .Setup(r => r.UserAddAsync(user, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await userRepositoryMock.Object.UserAddAsync(user, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("newuser");
        result.DisplayName.Should().Be("New User");

        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UserAddAsync_ShouldPersistUser_WhenUserIsAdded()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var user = new User
        {
            UserId = 0,
            Username = "persisteduser",
            DisplayName = "Persisted User",
            CreatedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        userRepositoryMock
            .Setup(r => r.UserAddAsync(user, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        await userRepositoryMock.Object.UserAddAsync(user, cancellationToken);

        // Assert — verify persisted via separate context
        await using var verifyContext = CreateInMemoryContext(dbName);
        var persisted = await verifyContext.User.FirstOrDefaultAsync(u => u.Username == "persisteduser");
        persisted.Should().NotBeNull();
        persisted!.DisplayName.Should().Be("Persisted User");

        userRepositoryMock.VerifyAll();
    }
}
