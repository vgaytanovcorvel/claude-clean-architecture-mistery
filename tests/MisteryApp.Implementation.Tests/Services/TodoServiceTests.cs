using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Implementation.Services;

namespace MisteryApp.Implementation.Tests.Services;

[TestClass]
public class TodoServiceTests
{
    private Mock<ITodoRepository> todoRepositoryMock = new(MockBehavior.Strict);
    private Mock<IUserRepository> userRepositoryMock = new(MockBehavior.Strict);
    private FakeTimeProvider timeProvider = null!;
    private Mock<TodoService> todoServiceMock = null!;

    private readonly CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        todoRepositoryMock = new Mock<ITodoRepository>(MockBehavior.Strict);
        userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero));

        todoServiceMock = new Mock<TodoService>(
            () => new TodoService(
                todoRepositoryMock.Object,
                userRepositoryMock.Object,
                timeProvider),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task GetTodosByUserAsync_ShouldReturnTodos_WhenUserIdIsProvided()
    {
        // Arrange
        var userId = 1;
        var expectedTodos = new List<TodoItem>
        {
            new() { Id = 1, Title = "Buy groceries", UserId = userId },
            new() { Id = 2, Title = "Read a book", UserId = userId },
        };

        todoServiceMock
            .Setup(s => s.GetTodosByUserAsync(userId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoFindByUserIdAsync(userId, cancellationToken))
            .ReturnsAsync(expectedTodos)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.GetTodosByUserAsync(userId, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedTodos);

        todoServiceMock.VerifyAll();
        todoRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetTodoByIdAsync_ShouldReturnTodo_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;
        var expectedTodo = new TodoItem { Id = todoId, Title = "Buy groceries", UserId = 1 };

        todoServiceMock
            .Setup(s => s.GetTodoByIdAsync(todoId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoSingleByIdAsync(todoId, cancellationToken))
            .ReturnsAsync(expectedTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.GetTodoByIdAsync(todoId, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedTodo);

        todoServiceMock.VerifyAll();
        todoRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetTodoByIdAsync_ShouldThrowNotFoundException_WhenTodoDoesNotExist()
    {
        // Arrange
        var todoId = 999;

        todoServiceMock
            .Setup(s => s.GetTodoByIdAsync(todoId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoSingleByIdAsync(todoId, cancellationToken))
            .ThrowsAsync(new NotFoundException($"Todo item not found (TodoItemId: {todoId})."))
            .Verifiable(Times.Once());

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => todoServiceMock.Object.GetTodoByIdAsync(todoId, cancellationToken));

        exception.Message.Should().Contain("999");

        todoServiceMock.VerifyAll();
        todoRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task CreateTodoAsync_ShouldReturnCreatedTodo_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateTodoRequest("Buy groceries", 1);
        var user = new User { Id = 1, Name = "Alice" };
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var savedTodo = new TodoItem
        {
            Id = 10,
            Title = request.Title,
            UserId = request.UserId,
            CreatedAtUtc = now,
        };

        todoServiceMock
            .Setup(s => s.CreateTodoAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleByIdAsync(request.UserId, cancellationToken))
            .ReturnsAsync(user)
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoAddAsync(
                It.Is<TodoItem>(t =>
                    t.Title == request.Title &&
                    t.UserId == request.UserId &&
                    t.CreatedAtUtc == now),
                cancellationToken))
            .ReturnsAsync(savedTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.CreateTodoAsync(request, cancellationToken);

        // Assert
        result.Id.Should().Be(10);
        result.Title.Should().Be(request.Title);
        result.UserId.Should().Be(request.UserId);
        result.CreatedAtUtc.Should().Be(now);

        todoServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
        todoRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task CreateTodoAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new CreateTodoRequest("Buy groceries", 999);

        todoServiceMock
            .Setup(s => s.CreateTodoAsync(request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        userRepositoryMock
            .Setup(r => r.UserSingleByIdAsync(999, cancellationToken))
            .ThrowsAsync(new NotFoundException("User not found (UserId: 999)."))
            .Verifiable(Times.Once());

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => todoServiceMock.Object.CreateTodoAsync(request, cancellationToken));

        exception.Message.Should().Contain("999");

        todoServiceMock.VerifyAll();
        userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateTodoAsync_ShouldReturnUpdatedTodo_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;
        var request = new UpdateTodoRequest("Updated title");
        var existing = new TodoItem
        {
            Id = todoId,
            Title = "Old title",
            IsComplete = false,
            UserId = 1,
            CreatedAtUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        var updated = new TodoItem
        {
            Id = todoId,
            Title = request.Title,
            IsComplete = false,
            UserId = 1,
            CreatedAtUtc = existing.CreatedAtUtc,
        };

        todoServiceMock
            .Setup(s => s.UpdateTodoAsync(todoId, request, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoSingleByIdAsync(todoId, cancellationToken))
            .ReturnsAsync(existing)
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoUpdateAsync(
                It.Is<TodoItem>(t =>
                    t.Id == todoId &&
                    t.Title == request.Title &&
                    t.IsComplete == existing.IsComplete &&
                    t.UserId == existing.UserId &&
                    t.CreatedAtUtc == existing.CreatedAtUtc),
                cancellationToken))
            .ReturnsAsync(updated)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.UpdateTodoAsync(todoId, request, cancellationToken);

        // Assert
        result.Title.Should().Be(request.Title);
        result.Id.Should().Be(todoId);

        todoServiceMock.VerifyAll();
        todoRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task ToggleTodoCompleteAsync_ShouldMarkComplete_WhenTodoIsIncomplete()
    {
        // Arrange
        var todoId = 1;
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var existing = new TodoItem
        {
            Id = todoId,
            Title = "Buy groceries",
            IsComplete = false,
            UserId = 1,
            CreatedAtUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        var toggled = new TodoItem
        {
            Id = todoId,
            Title = existing.Title,
            IsComplete = true,
            UserId = 1,
            CreatedAtUtc = existing.CreatedAtUtc,
            CompletedAtUtc = now,
        };

        todoServiceMock
            .Setup(s => s.ToggleTodoCompleteAsync(todoId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoSingleByIdAsync(todoId, cancellationToken))
            .ReturnsAsync(existing)
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoUpdateAsync(
                It.Is<TodoItem>(t =>
                    t.Id == todoId &&
                    t.IsComplete == true &&
                    t.CompletedAtUtc == now),
                cancellationToken))
            .ReturnsAsync(toggled)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.ToggleTodoCompleteAsync(todoId, cancellationToken);

        // Assert
        result.IsComplete.Should().BeTrue();
        result.CompletedAtUtc.Should().Be(now);

        todoServiceMock.VerifyAll();
        todoRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task ToggleTodoCompleteAsync_ShouldMarkIncomplete_WhenTodoIsComplete()
    {
        // Arrange
        var todoId = 1;
        var existing = new TodoItem
        {
            Id = todoId,
            Title = "Buy groceries",
            IsComplete = true,
            UserId = 1,
            CreatedAtUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CompletedAtUtc = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        var toggled = new TodoItem
        {
            Id = todoId,
            Title = existing.Title,
            IsComplete = false,
            UserId = 1,
            CreatedAtUtc = existing.CreatedAtUtc,
            CompletedAtUtc = null,
        };

        todoServiceMock
            .Setup(s => s.ToggleTodoCompleteAsync(todoId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoSingleByIdAsync(todoId, cancellationToken))
            .ReturnsAsync(existing)
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoUpdateAsync(
                It.Is<TodoItem>(t =>
                    t.Id == todoId &&
                    t.IsComplete == false &&
                    t.CompletedAtUtc == null),
                cancellationToken))
            .ReturnsAsync(toggled)
            .Verifiable(Times.Once());

        // Act
        var result = await todoServiceMock.Object.ToggleTodoCompleteAsync(todoId, cancellationToken);

        // Assert
        result.IsComplete.Should().BeFalse();
        result.CompletedAtUtc.Should().BeNull();

        todoServiceMock.VerifyAll();
        todoRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task DeleteTodoAsync_ShouldDeleteTodo_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;

        todoServiceMock
            .Setup(s => s.DeleteTodoAsync(todoId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        todoRepositoryMock
            .Setup(r => r.TodoDeleteAsync(todoId, cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once());

        // Act
        await todoServiceMock.Object.DeleteTodoAsync(todoId, cancellationToken);

        // Assert
        todoServiceMock.VerifyAll();
        todoRepositoryMock.VerifyAll();
    }
}
