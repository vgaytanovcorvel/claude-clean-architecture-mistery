using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Abstractions.Models.Requests;
using MisteryApp.Web.Core.Controllers;
using Moq;

namespace MisteryApp.Web.Core.Tests.Controllers;

[TestClass]
public class TodosControllerTests
{
    private Mock<ITodoService> todoServiceMock = new(MockBehavior.Strict);
    private Mock<IUserService> userServiceMock = new(MockBehavior.Strict);
    private TodosController controller = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    private readonly User testUser = new()
    {
        UserId = 1,
        Username = "johndoe",
        DisplayName = "John Doe",
        CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
    };

    [TestInitialize]
    public void Setup()
    {
        todoServiceMock = new(MockBehavior.Strict);
        userServiceMock = new(MockBehavior.Strict);
        controller = new TodosController(todoServiceMock.Object, userServiceMock.Object);
    }

    [TestMethod]
    public async Task GetAll_ShouldReturnOkWithTodos_WhenUserExists()
    {
        // Arrange
        var todos = new List<TodoItem>
        {
            new()
            {
                TodoItemId = 1,
                UserId = 1,
                Title = "Todo 1",
                IsCompleted = false,
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
            }
        };

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("johndoe", cancellationToken))
            .ReturnsAsync(testUser)
            .Verifiable(Times.Once());

        todoServiceMock
            .Setup(s => s.GetTodosByUserIdAsync(1, cancellationToken))
            .ReturnsAsync(todos)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.GetAll("johndoe", cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as ApiResponse<IReadOnlyList<TodoItem>>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().HaveCount(1);

        userServiceMock.VerifyAll();
        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetAll_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("nonexistent", cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.GetAll("nonexistent", cancellationToken);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        var response = notFoundResult!.Value as ApiResponse<IReadOnlyList<TodoItem>>;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Create_ShouldReturnCreatedWithTodo_WhenUserExists()
    {
        // Arrange
        var request = new CreateTodoRequest("New Todo", "Description");
        var createdTodo = new TodoItem
        {
            TodoItemId = 1,
            UserId = 1,
            Title = "New Todo",
            Description = "Description",
            IsCompleted = false,
            CreatedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("johndoe", cancellationToken))
            .ReturnsAsync(testUser)
            .Verifiable(Times.Once());

        todoServiceMock
            .Setup(s => s.CreateTodoAsync(1, request, cancellationToken))
            .ReturnsAsync(createdTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Create("johndoe", request, cancellationToken);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        var response = createdResult!.Value as ApiResponse<TodoItem>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data!.Title.Should().Be("New Todo");

        userServiceMock.VerifyAll();
        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Create_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new CreateTodoRequest("New Todo", "Description");

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("nonexistent", cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Create("nonexistent", request, cancellationToken);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Update_ShouldReturnOkWithTodo_WhenUserExists()
    {
        // Arrange
        var request = new UpdateTodoRequest("Updated Title", "Updated Desc", true);
        var updatedTodo = new TodoItem
        {
            TodoItemId = 1,
            UserId = 1,
            Title = "Updated Title",
            Description = "Updated Desc",
            IsCompleted = true,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            CompletedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("johndoe", cancellationToken))
            .ReturnsAsync(testUser)
            .Verifiable(Times.Once());

        todoServiceMock
            .Setup(s => s.UpdateTodoAsync(1, 1, request, cancellationToken))
            .ReturnsAsync(updatedTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Update(1, "johndoe", request, cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as ApiResponse<TodoItem>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data!.Title.Should().Be("Updated Title");

        userServiceMock.VerifyAll();
        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Update_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new UpdateTodoRequest("Title", "Desc", false);

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("nonexistent", cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Update(1, "nonexistent", request, cancellationToken);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Delete_ShouldReturnNoContent_WhenUserExists()
    {
        // Arrange
        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("johndoe", cancellationToken))
            .ReturnsAsync(testUser)
            .Verifiable(Times.Once());

        todoServiceMock
            .Setup(s => s.DeleteTodoAsync(1, 1, cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Delete(1, "johndoe", cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        userServiceMock.VerifyAll();
        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("nonexistent", cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Delete(1, "nonexistent", cancellationToken);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Toggle_ShouldReturnOkWithTodo_WhenUserExists()
    {
        // Arrange
        var toggledTodo = new TodoItem
        {
            TodoItemId = 1,
            UserId = 1,
            Title = "Toggled Todo",
            IsCompleted = true,
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            CompletedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("johndoe", cancellationToken))
            .ReturnsAsync(testUser)
            .Verifiable(Times.Once());

        todoServiceMock
            .Setup(s => s.ToggleTodoAsync(1, 1, cancellationToken))
            .ReturnsAsync(toggledTodo)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Toggle(1, "johndoe", cancellationToken);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as ApiResponse<TodoItem>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data!.IsCompleted.Should().BeTrue();

        userServiceMock.VerifyAll();
        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task Toggle_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("nonexistent", cancellationToken))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var result = await controller.Toggle(1, "nonexistent", cancellationToken);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();

        userServiceMock.VerifyAll();
    }
}
