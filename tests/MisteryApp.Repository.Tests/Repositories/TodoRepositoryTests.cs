using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Models;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Entities;
using MisteryApp.Repository.Repositories;
using Moq;

namespace MisteryApp.Repository.Tests.Repositories;

[TestClass]
public class TodoRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> dbContextOptions = null!;
    private Mock<IDbContextFactory<ApplicationDbContext>> contextFactoryMock = null!;
    private TodoRepository repository = null!;

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

        repository = new TodoRepository(contextFactoryMock.Object);
    }

    private async Task SeedDataAsync()
    {
        await using var dbContext = new ApplicationDbContext(dbContextOptions);
        dbContext.Users.Add(new UserEntity { Id = 1, Name = "Alice", AvatarUrl = "" });

        dbContext.TodoItems.AddRange(
            new TodoItemEntity
            {
                Id = 1,
                Title = "Buy groceries",
                IsComplete = false,
                UserId = 1,
                CreatedAtUtc = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc),
            },
            new TodoItemEntity
            {
                Id = 2,
                Title = "Read a book",
                IsComplete = true,
                UserId = 1,
                CreatedAtUtc = new DateTime(2025, 6, 14, 10, 0, 0, DateTimeKind.Utc),
                CompletedAtUtc = new DateTime(2025, 6, 15, 8, 0, 0, DateTimeKind.Utc),
            });
        await dbContext.SaveChangesAsync();
    }

    [TestMethod]
    public async Task TodoFindByUserIdAsync_ShouldReturnTodosOrderedByCreatedAtDesc_WhenTodosExist()
    {
        // Arrange
        await SeedDataAsync();

        // Act
        var result = await repository.TodoFindByUserIdAsync(1, cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Buy groceries");
        result[1].Title.Should().Be("Read a book");
    }

    [TestMethod]
    public async Task TodoFindByUserIdAsync_ShouldReturnEmptyList_WhenNoTodosExistForUser()
    {
        // Arrange
        await SeedDataAsync();

        // Act
        var result = await repository.TodoFindByUserIdAsync(999, cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task TodoSingleByIdAsync_ShouldReturnTodo_WhenTodoExists()
    {
        // Arrange
        await SeedDataAsync();

        // Act
        var result = await repository.TodoSingleByIdAsync(1, cancellationToken);

        // Assert
        result.Id.Should().Be(1);
        result.Title.Should().Be("Buy groceries");
        result.IsComplete.Should().BeFalse();
        result.UserId.Should().Be(1);
    }

    [TestMethod]
    public async Task TodoSingleByIdAsync_ShouldThrowNotFoundException_WhenTodoDoesNotExist()
    {
        // Arrange
        await SeedDataAsync();

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repository.TodoSingleByIdAsync(999, cancellationToken));

        exception.Message.Should().Contain("999");
    }

    [TestMethod]
    public async Task TodoAddAsync_ShouldReturnSavedTodo_WhenTodoIsValid()
    {
        // Arrange
        await SeedDataAsync();
        var todoItem = new TodoItem
        {
            Title = "New todo",
            UserId = 1,
            CreatedAtUtc = new DateTime(2025, 6, 16, 10, 0, 0, DateTimeKind.Utc),
        };

        // Act
        var result = await repository.TodoAddAsync(todoItem, cancellationToken);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New todo");
        result.UserId.Should().Be(1);
    }

    [TestMethod]
    public async Task TodoUpdateAsync_ShouldReturnUpdatedTodo_WhenTodoExists()
    {
        // Arrange
        await SeedDataAsync();
        var updated = new TodoItem
        {
            Id = 1,
            Title = "Updated groceries",
            IsComplete = true,
            UserId = 1,
            CreatedAtUtc = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc),
            CompletedAtUtc = new DateTime(2025, 6, 16, 10, 0, 0, DateTimeKind.Utc),
        };

        // Act
        var result = await repository.TodoUpdateAsync(updated, cancellationToken);

        // Assert
        result.Title.Should().Be("Updated groceries");
        result.IsComplete.Should().BeTrue();
        result.CompletedAtUtc.Should().NotBeNull();
    }

    [TestMethod]
    public async Task TodoDeleteAsync_ShouldRemoveTodo_WhenTodoExists()
    {
        // Arrange
        await SeedDataAsync();

        // Act
        await repository.TodoDeleteAsync(1, cancellationToken);

        // Assert
        await using var dbContext = new ApplicationDbContext(dbContextOptions);
        var remainingTodos = await dbContext.TodoItems.ToListAsync();
        remainingTodos.Should().HaveCount(1);
        remainingTodos[0].Id.Should().Be(2);
    }

    [TestMethod]
    public async Task TodoDeleteAsync_ShouldThrowNotFoundException_WhenTodoDoesNotExist()
    {
        // Arrange
        await SeedDataAsync();

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repository.TodoDeleteAsync(999, cancellationToken));

        exception.Message.Should().Contain("999");
    }

    [TestMethod]
    public async Task TodoFindByUserIdAsync_ShouldMapAllProperties_WhenTodoIsComplete()
    {
        // Arrange
        await SeedDataAsync();

        // Act
        var result = await repository.TodoFindByUserIdAsync(1, cancellationToken);
        var completeTodo = result.First(t => t.IsComplete);

        // Assert
        completeTodo.Id.Should().Be(2);
        completeTodo.Title.Should().Be("Read a book");
        completeTodo.IsComplete.Should().BeTrue();
        completeTodo.UserId.Should().Be(1);
        completeTodo.CompletedAtUtc.Should().NotBeNull();
    }
}
