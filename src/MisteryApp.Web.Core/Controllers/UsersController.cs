using System.Net;
using Microsoft.AspNetCore.Mvc;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Web.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<User>>>> GetUsers(
        CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<User>>.Ok(users));
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<ApiResponse<User>>> GetUser(
        int id,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(id, cancellationToken);

        if (user is null)
            return NotFound(ApiResponse<User>.Fail($"User not found (UserId: {id}).", HttpStatusCode.NotFound));

        return Ok(ApiResponse<User>.Ok(user));
    }
}
