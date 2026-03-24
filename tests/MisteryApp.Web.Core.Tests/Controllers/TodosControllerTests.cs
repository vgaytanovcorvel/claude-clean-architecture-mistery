using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Web.Core.Controllers;

namespace MisteryApp.Web.Core.Tests.Controllers;

[TestClass]
public class TodosControllerTests
{
    private Mock<ITodoService> todoServiceMock = new(MockBehavior.Strict);
    private TodosController controller = null!;

    private readonly CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        todoServiceMock = new Mock<ITodoService>(MockBehavior.Strict);
        controller = new TodosController(todoServiceMock.Object);
    }

    [TestMethod]
    public async Task GetTodos_ShouldReturnOkWithTodos_WhenTodosExist()
    {
        // Arrange
        var userId = 1;
        var todos = new List<TodoItem>
        {
            new() { Id = 1, Title = "Buy groceries", UserId = userId },
            new() { Id = 2, Title = "Read a book", UserId = userId },
        };

        todoServiceMock
            .Setup(s => s.GetTodosByUserAsync(userId, cancellationToken))
            .ReturnsAsync(todos)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.GetTodos(userId, cancellationToken);

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IReadOnlyList<TodoItem>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);

        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetTodoById_ShouldReturnOkWithTodo_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;
        var todo = new TodoItem { Id = todoId, Title = "Buy groceries", UserId = 1 };

        todoServiceMock
            .Setup(s => s.GetTodoByIdAsync(todoId, cancellationToken))
            .ReturnsAsync(todo)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.GetTodoById(todoId, cancellationToken);

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<TodoItem>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(todoId);

        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task CreateTodo_ShouldReturnCreatedWithTodo_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateTodoRequest("Buy groceries", 1);
        var createdTodo = new TodoItem { Id = 10, Title = request.Title, UserId = request.UserId };

        todoServiceMock
            .Setup(s => s.CreateTodoAsync(request, cancellationToken))
            .ReturnsAsync(createdTodo)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.CreateTodo(request, cancellationToken);

        // Assert
        var createdResult = actionResult.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(TodosController.GetTodoById));
        var response = createdResult.Value.Should().BeOfType<ApiResponse<TodoItem>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(10);

        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task UpdateTodo_ShouldReturnOkWithUpdatedTodo_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;
        var request = new UpdateTodoRequest("Updated title");
        var updatedTodo = new TodoItem { Id = todoId, Title = request.Title, UserId = 1 };

        todoServiceMock
            .Setup(s => s.UpdateTodoAsync(todoId, request, cancellationToken))
            .ReturnsAsync(updatedTodo)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.UpdateTodo(todoId, request, cancellationToken);

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<TodoItem>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Title.Should().Be("Updated title");

        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task ToggleTodo_ShouldReturnOkWithToggledTodo_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;
        var toggledTodo = new TodoItem { Id = todoId, Title = "Buy groceries", IsComplete = true, UserId = 1 };

        todoServiceMock
            .Setup(s => s.ToggleTodoCompleteAsync(todoId, cancellationToken))
            .ReturnsAsync(toggledTodo)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.ToggleTodo(todoId, cancellationToken);

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<TodoItem>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.IsComplete.Should().BeTrue();

        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task DeleteTodo_ShouldReturnNoContent_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;

        todoServiceMock
            .Setup(s => s.DeleteTodoAsync(todoId, cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once());

        // Act
        var actionResult = await controller.DeleteTodo(todoId, cancellationToken);

        // Assert
        actionResult.Should().BeOfType<NoContentResult>();

        todoServiceMock.VerifyAll();
    }
}
