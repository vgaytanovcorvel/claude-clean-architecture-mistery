using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Abstractions.Models.Requests;
using Moq;

namespace MisteryApp.Web.Api.Tests;

[TestClass]
public class GlobalExceptionHandlerTests
{
    private WebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private Mock<IUserService> userServiceMock = new(MockBehavior.Strict);
    private Mock<ITodoService> todoServiceMock = new(MockBehavior.Strict);

    [TestInitialize]
    public void Setup()
    {
        userServiceMock = new(MockBehavior.Strict);
        todoServiceMock = new(MockBehavior.Strict);

        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var userDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IUserService));
                    if (userDescriptor is not null)
                        services.Remove(userDescriptor);

                    var todoDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(ITodoService));
                    if (todoDescriptor is not null)
                        services.Remove(todoDescriptor);

                    services.AddSingleton(userServiceMock.Object);
                    services.AddSingleton(todoServiceMock.Object);
                });
            });

        client = factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        client.Dispose();
        factory.Dispose();
    }

    [TestMethod]
    public async Task Login_ShouldReturn200WithApiResponse_WhenRequestIsValid()
    {
        // Arrange
        var expectedUser = new User
        {
            UserId = 1,
            Username = "johndoe",
            DisplayName = "John Doe",
            CreatedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetOrCreateUserAsync("johndoe", "John Doe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser)
            .Verifiable(Times.Once());

        // Act
        var response = await client.PostAsJsonAsync("/api/users/login",
            new CreateUserRequest("johndoe", "John Doe"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<User>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Username.Should().Be("johndoe");

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetByUsername_ShouldReturn404WithApiResponse_WhenUserNotFound()
    {
        // Arrange
        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("ghost", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null)
            .Verifiable(Times.Once());

        // Act
        var response = await client.GetAsync("/api/users/ghost");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<User>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Contain("ghost");

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetByUsername_ShouldReturn404WithApiResponse_WhenNotFoundExceptionThrown()
    {
        // Arrange
        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("throwuser", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found (Username: throwuser)."))
            .Verifiable(Times.Once());

        // Act
        var response = await client.GetAsync("/api/users/throwuser");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Contain("throwuser");

        userServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetTodos_ShouldReturn401WithApiResponse_WhenUnauthorizedAccessExceptionThrown()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "johndoe",
            DisplayName = "John Doe",
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("johndoe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user)
            .Verifiable(Times.Once());

        todoServiceMock
            .Setup(s => s.GetTodosByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You do not have permission."))
            .Verifiable(Times.Once());

        // Act
        var response = await client.GetAsync("/api/todos?username=johndoe");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Contain("permission");

        userServiceMock.VerifyAll();
        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetTodos_ShouldReturn500WithGenericMessage_WhenUnhandledExceptionThrown()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "johndoe",
            DisplayName = "John Doe",
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("johndoe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user)
            .Verifiable(Times.Once());

        todoServiceMock
            .Setup(s => s.GetTodosByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Something broke internally"))
            .Verifiable(Times.Once());

        // Act
        var response = await client.GetAsync("/api/todos?username=johndoe");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Error.Should().Be("Internal server error");

        userServiceMock.VerifyAll();
        todoServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task GetTodos_ShouldReturn200WithTodoList_WhenUserAndTodosExist()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "johndoe",
            DisplayName = "John Doe",
            CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        var todos = new List<TodoItem>
        {
            new()
            {
                TodoItemId = 1,
                UserId = 1,
                Title = "Buy groceries",
                IsCompleted = false,
                CreatedAt = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero)
            }
        };

        userServiceMock
            .Setup(s => s.GetUserByUsernameAsync("johndoe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user)
            .Verifiable(Times.Once());

        todoServiceMock
            .Setup(s => s.GetTodosByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todos)
            .Verifiable(Times.Once());

        // Act
        var response = await client.GetAsync("/api/todos?username=johndoe");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<TodoItem>>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().HaveCount(1);
        apiResponse.Data![0].Title.Should().Be("Buy groceries");

        userServiceMock.VerifyAll();
        todoServiceMock.VerifyAll();
    }
}
