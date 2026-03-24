using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Entities;
using MisteryApp.Repository.Repositories;
using Moq;

namespace MisteryApp.Repository.Tests.Repositories;

[TestClass]
public class UserRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> dbContextOptions = null!;
    private Mock<IDbContextFactory<ApplicationDbContext>> contextFactoryMock = null!;
    private UserRepository repository = null!;

    private readonly CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        contextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>(MockBehavior.Strict);
        contextFactoryMock
            .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ApplicationDbContext(dbContextOptions));

        repository = new UserRepository(contextFactoryMock.Object);
    }

    private async Task SeedUsersAsync()
    {
        await using var dbContext = new ApplicationDbContext(dbContextOptions);
        dbContext.Users.AddRange(
            new UserEntity { Id = 1, Name = "Alice", AvatarUrl = "https://example.com/alice.png" },
            new UserEntity { Id = 2, Name = "Bob", AvatarUrl = "https://example.com/bob.png" },
            new UserEntity { Id = 3, Name = "Charlie", AvatarUrl = "https://example.com/charlie.png" });
        await dbContext.SaveChangesAsync();
    }

    [TestMethod]
    public async Task UserGetAllAsync_ShouldReturnAllUsersOrderedByName_WhenUsersExist()
    {
        // Arrange
        await SeedUsersAsync();

        // Act
        var result = await repository.UserGetAllAsync(cancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
        result[2].Name.Should().Be("Charlie");
    }

    [TestMethod]
    public async Task UserGetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Act
        var result = await repository.UserGetAllAsync(cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task UserSingleByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        await SeedUsersAsync();

        // Act
        var result = await repository.UserSingleByIdAsync(1, cancellationToken);

        // Assert
        result.Id.Should().Be(1);
        result.Name.Should().Be("Alice");
        result.AvatarUrl.Should().Be("https://example.com/alice.png");
    }

    [TestMethod]
    public async Task UserSingleByIdAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        await SeedUsersAsync();

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repository.UserSingleByIdAsync(999, cancellationToken));

        exception.Message.Should().Contain("999");
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        await SeedUsersAsync();

        // Act
        var result = await repository.UserSingleOrDefaultByIdAsync(2, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(2);
        result.Name.Should().Be("Bob");
    }

    [TestMethod]
    public async Task UserSingleOrDefaultByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        await SeedUsersAsync();

        // Act
        var result = await repository.UserSingleOrDefaultByIdAsync(999, cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task UserGetAllAsync_ShouldReturnDomainModels_WhenMappingFromEntities()
    {
        // Arrange
        await SeedUsersAsync();

        // Act
        var result = await repository.UserGetAllAsync(cancellationToken);

        // Assert
        result.Should().AllSatisfy(u =>
        {
            u.Id.Should().BeGreaterThan(0);
            u.Name.Should().NotBeNullOrEmpty();
        });
    }
}
