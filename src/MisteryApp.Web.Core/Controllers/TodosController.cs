using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Web.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController(ITodoService todoService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TodoItem>>), StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<TodoItem>>>> GetTodos(
        [FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        var todos = await todoService.GetTodosByUserAsync(userId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TodoItem>>.Ok(todos));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<ApiResponse<TodoItem>>> CreateTodo(
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.CreateTodoAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetTodoById),
            new { id = todo.Id },
            ApiResponse<TodoItem>.Ok(todo));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<TodoItem>>> GetTodoById(
        int id,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.GetTodoByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<TodoItem>.Ok(todo));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<TodoItem>>> UpdateTodo(
        int id,
        [FromBody] UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.UpdateTodoAsync(id, request, cancellationToken);
        return Ok(ApiResponse<TodoItem>.Ok(todo));
    }

    [HttpPut("{id}/toggle")]
    [ProducesResponseType(typeof(ApiResponse<TodoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<TodoItem>>> ToggleTodo(
        int id,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.ToggleTodoCompleteAsync(id, cancellationToken);
        return Ok(ApiResponse<TodoItem>.Ok(todo));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult> DeleteTodo(
        int id,
        CancellationToken cancellationToken)
    {
        await todoService.DeleteTodoAsync(id, cancellationToken);
        return NoContent();
    }
}
