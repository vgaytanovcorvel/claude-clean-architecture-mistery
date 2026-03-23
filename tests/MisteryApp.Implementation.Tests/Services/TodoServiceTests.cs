using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Abstractions.Models.Requests;
using MisteryApp.Implementation.Services;
using Moq;

namespace MisteryApp.Implementation.Tests.Services;

[TestClass]
public class TodoServiceTests
{
    private Mock<ITodoItemRepository> todoItemRepositoryMock = new(MockBehavior.Strict);
    private FakeTimeProvider timeProvider = null!;
    private Mock<TodoService> todoServiceMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        todoItemRepositoryMock = new(MockBehavior.Strict);
        timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero));

        todoServiceMock = new Mock<TodoService>(
            () => new TodoService(todoItemRepositoryMock.Object, timeProvider),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task GetTodosByUserIdAsync_ShouldReturnTodos_WhenTodosExist()
    {
        // Arrange
        var userId = 1;
        var expectedTodos = new List<TodoItem>
        {
            new()
            {
                TodoItemId = 1,
                UserId = userId,
                Title = "First Todo",
                Description = "Description 1",
                IsCompleted = false,
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new()
            {
                TodoItemId = 2,
                UserId = userId,
                Title = "Second Todo",
                IsCompleted = true,
                CreatedAt = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero),
                CompletedAt = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero)
            }
        };

        todoServiceMock
            .Setup(s => s.GetTodosByUserIdAsync(userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemGetAllByUserIdAsync(userId, cancellationToken))
            .ReturnsAsync(expectedTodos)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.GetTodosByUserIdAsync(userId, cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("First Todo");
        result[1].Title.Should().Be("Second Todo");

        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task CreateTodoAsync_ShouldReturnCreatedTodo_WhenRequestIsValid()
    {
        // Arrange
        var userId = 1;
        var request = new CreateTodoRequest("Buy groceries", "Milk, eggs, bread");
        var expectedCreatedAt = timeProvider.GetUtcNow();
        var createdTodo = new TodoItem
        {
            TodoItemId = 1,
            UserId = userId,
            Title = "Buy groceries",
            Description = "Milk, eggs, bread",
            IsCompleted = false,
            CreatedAt = expectedCreatedAt
        };

        todoServiceMock
            .Setup(s => s.CreateTodoAsync(userId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemAddAsync(
                It.Is<TodoItem>(t =>
                    t.UserId == userId &&
                    t.Title == "Buy groceries" &&
                    t.Description == "Milk, eggs, bread" &&
                    t.IsCompleted == false &&
                    t.CreatedAt == expectedCreatedAt),
                cancellationToken))
            .ReturnsAsync(createdTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.CreateTodoAsync(userId, request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.TodoItemId.Should().Be(1);
        result.UserId.Should().Be(userId);
        result.Title.Should().Be("Buy groceries");
        result.Description.Should().Be("Milk, eggs, bread");
        result.IsCompleted.Should().BeFalse();
        result.CreatedAt.Should().Be(expectedCreatedAt);

        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateTodoAsync_ShouldReturnUpdatedTodo_WhenOwnershipIsValid()
    {
        // Arrange
        var todoItemId = 1;
        var userId = 1;
        var request = new UpdateTodoRequest("Updated Title", "Updated Description", false);
        var existingTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Original Title",
            Description = "Original Description",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var updatedTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = false,
            CreatedAt = existingTodo.CreatedAt
        };

        todoServiceMock
            .Setup(s => s.UpdateTodoAsync(todoItemId, userId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync(existingTodo)
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemUpdateAsync(
                It.Is<TodoItem>(t =>
                    t.TodoItemId == todoItemId &&
                    t.UserId == userId &&
                    t.Title == "Updated Title" &&
                    t.Description == "Updated Description" &&
                    t.IsCompleted == false &&
                    t.CompletedAt == null),
                cancellationToken))
            .ReturnsAsync(updatedTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.UpdateTodoAsync(todoItemId, userId, request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.IsCompleted.Should().BeFalse();

        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateTodoAsync_ShouldThrowUnauthorizedAccessException_WhenUserDoesNotOwnTodo()
    {
        // Arrange
        var todoItemId = 1;
        var userId = 2;
        var request = new UpdateTodoRequest("Updated Title", "Updated Description", false);
        var existingTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = 1,
            Title = "Original Title",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        todoServiceMock
            .Setup(s => s.UpdateTodoAsync(todoItemId, userId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync(existingTodo)
            .Verifiable(Times.Once());

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            () => todoServiceMock.Object.UpdateTodoAsync(todoItemId, userId, request, cancellationToken));

        exception.Message.Should().Be("You do not have permission to modify this todo.");

        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateTodoAsync_ShouldSetCompletedAt_WhenTodoIsNewlyCompleted()
    {
        // Arrange
        var todoItemId = 1;
        var userId = 1;
        var request = new UpdateTodoRequest("Title", "Description", true);
        var expectedCompletedAt = timeProvider.GetUtcNow();
        var existingTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Title",
            Description = "Description",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var updatedTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Title",
            Description = "Description",
            IsCompleted = true,
            CreatedAt = existingTodo.CreatedAt,
            CompletedAt = expectedCompletedAt
        };

        todoServiceMock
            .Setup(s => s.UpdateTodoAsync(todoItemId, userId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync(existingTodo)
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemUpdateAsync(
                It.Is<TodoItem>(t =>
                    t.TodoItemId == todoItemId &&
                    t.IsCompleted == true &&
                    t.CompletedAt == expectedCompletedAt),
                cancellationToken))
            .ReturnsAsync(updatedTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.UpdateTodoAsync(todoItemId, userId, request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsCompleted.Should().BeTrue();
        result.CompletedAt.Should().Be(expectedCompletedAt);

        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task DeleteTodoAsync_ShouldDeleteTodo_WhenOwnershipIsValid()
    {
        // Arrange
        var todoItemId = 1;
        var userId = 1;
        var existingTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Todo to delete",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        todoServiceMock
            .Setup(s => s.DeleteTodoAsync(todoItemId, userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync(existingTodo)
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemDeleteAsync(todoItemId, cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once());

        // Act
        await todoServiceMock.Object.DeleteTodoAsync(todoItemId, userId, cancellationToken);

        // Assert
        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task DeleteTodoAsync_ShouldThrowUnauthorizedAccessException_WhenUserDoesNotOwnTodo()
    {
        // Arrange
        var todoItemId = 1;
        var userId = 2;
        var existingTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = 1,
            Title = "Someone else's todo",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        todoServiceMock
            .Setup(s => s.DeleteTodoAsync(todoItemId, userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync(existingTodo)
            .Verifiable(Times.Once());

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            () => todoServiceMock.Object.DeleteTodoAsync(todoItemId, userId, cancellationToken));

        exception.Message.Should().Be("You do not have permission to modify this todo.");

        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task ToggleTodoAsync_ShouldToggleToCompleted_WhenTodoIsActive()
    {
        // Arrange
        var todoItemId = 1;
        var userId = 1;
        var expectedCompletedAt = timeProvider.GetUtcNow();
        var existingTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Active Todo",
            Description = "A description",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var toggledTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Active Todo",
            Description = "A description",
            IsCompleted = true,
            CreatedAt = existingTodo.CreatedAt,
            CompletedAt = expectedCompletedAt
        };

        todoServiceMock
            .Setup(s => s.ToggleTodoAsync(todoItemId, userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync(existingTodo)
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemUpdateAsync(
                It.Is<TodoItem>(t =>
                    t.TodoItemId == todoItemId &&
                    t.UserId == userId &&
                    t.Title == "Active Todo" &&
                    t.Description == "A description" &&
                    t.IsCompleted == true &&
                    t.CompletedAt == expectedCompletedAt &&
                    t.CreatedAt == existingTodo.CreatedAt),
                cancellationToken))
            .ReturnsAsync(toggledTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.ToggleTodoAsync(todoItemId, userId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsCompleted.Should().BeTrue();
        result.CompletedAt.Should().Be(expectedCompletedAt);

        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task ToggleTodoAsync_ShouldToggleToActive_WhenTodoIsCompleted()
    {
        // Arrange
        var todoItemId = 1;
        var userId = 1;
        var existingTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Completed Todo",
            Description = "A description",
            IsCompleted = true,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            CompletedAt = new DateTimeOffset(2025, 1, 5, 0, 0, 0, TimeSpan.Zero)
        };
        var toggledTodo = new TodoItem
        {
            TodoItemId = todoItemId,
            UserId = userId,
            Title = "Completed Todo",
            Description = "A description",
            IsCompleted = false,
            CreatedAt = existingTodo.CreatedAt,
            CompletedAt = null
        };

        todoServiceMock
            .Setup(s => s.ToggleTodoAsync(todoItemId, userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemSingleByIdAsync(todoItemId, cancellationToken))
            .ReturnsAsync(existingTodo)
            .Verifiable(Times.Once());

        todoItemRepositoryMock
            .Setup(r => r.TodoItemUpdateAsync(
                It.Is<TodoItem>(t =>
                    t.TodoItemId == todoItemId &&
                    t.UserId == userId &&
                    t.Title == "Completed Todo" &&
                    t.Description == "A description" &&
                    t.IsCompleted == false &&
                    t.CompletedAt == null &&
                    t.CreatedAt == existingTodo.CreatedAt),
                cancellationToken))
            .ReturnsAsync(toggledTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.ToggleTodoAsync(todoItemId, userId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsCompleted.Should().BeFalse();
        result.CompletedAt.Should().BeNull();

        todoServiceMock.VerifyAll();
        todoItemRepositoryMock.VerifyAll();
    }
}
