using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Abstractions.Models.Requests;
using System.Net;

namespace MisteryApp.Web.Core.Controllers;

[ApiController]
[Route("api/todos")]
public class TodosController(
    ITodoService todoService,
    IUserService userService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TodoItem>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TodoItem>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TodoItem>>>> GetAll(
        [FromQuery] string username,
        CancellationToken cancellationToken)
    {
        var user = await ResolveUserAsync(username, cancellationToken);
        if (user is null)
            return NotFound(ApiResponse<IReadOnlyList<TodoItem>>.Fail($"User not found (Username: {username}).", HttpStatusCode.NotFound));

        var todos = await todoService.GetTodosByUserIdAsync(user.UserId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TodoItem>>.Ok(todos));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TodoItem>>> Create(
        [FromQuery] string username,
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var user = await ResolveUserAsync(username, cancellationToken);
        if (user is null)
            return NotFound(ApiResponse<TodoItem>.Fail($"User not found (Username: {username}).", HttpStatusCode.NotFound));

        var todo = await todoService.CreateTodoAsync(user.UserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { username }, ApiResponse<TodoItem>.Ok(todo));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TodoItem>>> Update(
        int id,
        [FromQuery] string username,
        [FromBody] UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var user = await ResolveUserAsync(username, cancellationToken);
        if (user is null)
            return NotFound(ApiResponse<TodoItem>.Fail($"User not found (Username: {username}).", HttpStatusCode.NotFound));

        var todo = await todoService.UpdateTodoAsync(id, user.UserId, request, cancellationToken);
        return Ok(ApiResponse<TodoItem>.Ok(todo));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(
        int id,
        [FromQuery] string username,
        CancellationToken cancellationToken)
    {
        var user = await ResolveUserAsync(username, cancellationToken);
        if (user is null)
            return NotFound(ApiResponse<object>.Fail($"User not found (Username: {username}).", HttpStatusCode.NotFound));

        await todoService.DeleteTodoAsync(id, user.UserId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}/toggle")]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TodoItem>>> Toggle(
        int id,
        [FromQuery] string username,
        CancellationToken cancellationToken)
    {
        var user = await ResolveUserAsync(username, cancellationToken);
        if (user is null)
            return NotFound(ApiResponse<TodoItem>.Fail($"User not found (Username: {username}).", HttpStatusCode.NotFound));

        var todo = await todoService.ToggleTodoAsync(id, user.UserId, cancellationToken);
        return Ok(ApiResponse<TodoItem>.Ok(todo));
    }

    private async Task<User?> ResolveUserAsync(string username, CancellationToken cancellationToken)
    {
        return await userService.GetUserByUsernameAsync(username, cancellationToken);
    }
}
