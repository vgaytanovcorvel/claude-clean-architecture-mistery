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
public class TodoItemRepositoryTests
{
    private Mock<IDbContextFactory<ApplicationDbContext>> contextFactoryMock = new(MockBehavior.Strict);
    private Mock<TodoItemRepository> todoItemRepositoryMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        contextFactoryMock = new(MockBehavior.Strict);

        todoItemRepositoryMock = new Mock<TodoItemRepository>(
            () => new TodoItemRepository(contextFactoryMock.Object),
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
        todoItemRepositoryMock.Protected()
            .Setup<Task<ApplicationDbContext>>("CreateContextAsync", ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(CreateInMemoryContext(dbName))
            .Verifiable(Times.Once());
    }

    private async Task SeedTodoItemsAsync(string dbName, params TodoItemEntity[] entities)
    {
        await using var context = CreateInMemoryContext(dbName);
        await context.TodoItem.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    [TestMethod]
    public async Task TodoItemSingleByIdAsync_ShouldReturnTodoItem_WhenTodoItemExists()
    {
        // Arrange
        var todoItemId = 1;
        var expectedTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = 1,
            Title = "Test Todo",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleOrDefaultByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync(expectedTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await todoItemRepositoryMock.Object.TodoItemSingleByIdAsync(todoItemId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.TodoItemId.Should().Be(todoItemId);
        result.Title.Should().Be("Test Todo");

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemSingleByIdAsync_ShouldThrowNotFoundException_WhenTodoItemDoesNotExist()
    {
        // Arrange
        var todoItemId = 999;

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleOrDefaultByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync((TodoItem?)null)
            .Verifiable(Times.Once());

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => todoItemRepositoryMock.Object.TodoItemSingleByIdAsync(todoItemId, cancellationToken));

        exception.Message.Should().Be($"TodoItem not found (TodoItemId: {todoItemId}).");

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemSingleOrDefaultByIdAsync_ShouldReturnTodoItem_WhenTodoItemExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await SeedTodoItemsAsync(dbName, new TodoItemEntity
        {
            TodoItemId = 1,
            UserId = 1,
            Title = "Seeded Todo",
            Description = "A description",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleOrDefaultByIdAsync(1, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await todoItemRepositoryMock.Object.TodoItemSingleOrDefaultByIdAsync(1, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.TodoItemId.Should().Be(1);
        result.Title.Should().Be("Seeded Todo");
        result.Description.Should().Be("A description");

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemSingleOrDefaultByIdAsync_ShouldReturnNull_WhenTodoItemDoesNotExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleOrDefaultByIdAsync(999, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await todoItemRepositoryMock.Object.TodoItemSingleOrDefaultByIdAsync(999, cancellationToken);

        // Assert
        result.Should().BeNull();

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemGetAllByUserIdAsync_ShouldReturnTodoItems_WhenTodoItemsExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await SeedTodoItemsAsync(dbName,
            new TodoItemEntity
            {
                TodoItemId = 1,
                UserId = 1,
                Title = "Todo 1",
                IsCompleted = false,
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new TodoItemEntity
            {
                TodoItemId = 2,
                UserId = 1,
                Title = "Todo 2",
                IsCompleted = true,
                CreatedAt = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero)
            });

        todoItemRepositoryMock
            .Setup(r => r.TodoItemGetAllByUserIdAsync(1, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await todoItemRepositoryMock.Object.TodoItemGetAllByUserIdAsync(1, cancellationToken);

        // Assert
        result.Should().HaveCount(2);

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemGetAllByUserIdAsync_ShouldReturnEmptyList_WhenNoTodoItemsExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();

        todoItemRepositoryMock
            .Setup(r => r.TodoItemGetAllByUserIdAsync(1, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await todoItemRepositoryMock.Object.TodoItemGetAllByUserIdAsync(1, cancellationToken);

        // Assert
        result.Should().BeEmpty();

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemGetAllByUserIdAsync_ShouldReturnItemsOrderedByCreatedAtDescending_WhenMultipleItemsExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await SeedTodoItemsAsync(dbName,
            new TodoItemEntity
            {
                TodoItemId = 1,
                UserId = 1,
                Title = "Oldest",
                IsCompleted = false,
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new TodoItemEntity
            {
                TodoItemId = 2,
                UserId = 1,
                Title = "Middle",
                IsCompleted = false,
                CreatedAt = new DateTimeOffset(2025, 1, 5, 0, 0, 0, TimeSpan.Zero)
            },
            new TodoItemEntity
            {
                TodoItemId = 3,
                UserId = 1,
                Title = "Newest",
                IsCompleted = false,
                CreatedAt = new DateTimeOffset(2025, 1, 10, 0, 0, 0, TimeSpan.Zero)
            });

        todoItemRepositoryMock
            .Setup(r => r.TodoItemGetAllByUserIdAsync(1, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await todoItemRepositoryMock.Object.TodoItemGetAllByUserIdAsync(1, cancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result[0].Title.Should().Be("Newest");
        result[1].Title.Should().Be("Middle");
        result[2].Title.Should().Be("Oldest");

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemAddAsync_ShouldReturnCreatedTodoItem_WhenTodoItemIsValid()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var todoItem = new TodoItem
        {
            TodoItemId = 0,
            UserId = 1,
            Title = "New Todo",
            Description = "New Description",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        todoItemRepositoryMock
            .Setup(r => r.TodoItemAddAsync(todoItem, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await todoItemRepositoryMock.Object.TodoItemAddAsync(todoItem, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Todo");
        result.Description.Should().Be("New Description");
        result.IsCompleted.Should().BeFalse();

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemUpdateAsync_ShouldReturnUpdatedTodoItem_WhenTodoItemIsValid()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await SeedTodoItemsAsync(dbName, new TodoItemEntity
        {
            TodoItemId = 1,
            UserId = 1,
            Title = "Original Title",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });

        var updatedTodoItem = new TodoItem
        {
            TodoItemId = 1,
            UserId = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = true,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            CompletedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        todoItemRepositoryMock
            .Setup(r => r.TodoItemUpdateAsync(updatedTodoItem, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act
        var result = await todoItemRepositoryMock.Object.TodoItemUpdateAsync(updatedTodoItem, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.IsCompleted.Should().BeTrue();

        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task TodoItemDeleteAsync_ShouldThrowNotFoundException_WhenTodoItemDoesNotExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();

        todoItemRepositoryMock
            .Setup(r => r.TodoItemDeleteAsync(999, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        SetupCreateContext(dbName);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => todoItemRepositoryMock.Object.TodoItemDeleteAsync(999, cancellationToken));

        exception.Message.Should().Be("TodoItem not found (TodoItemId: 999).");

        todoItemRepositoryMock.VerifyAll();
    }
}
