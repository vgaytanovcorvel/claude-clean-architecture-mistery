using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;
using MisteryApp.Abstractions.Models.Requests;
using System.Net;

namespace MisteryApp.Web.Core.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<User>>> Login(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetOrCreateUserAsync(request.Username, request.DisplayName, cancellationToken);
        return Ok(ApiResponse<User>.Ok(user));
    }

    [HttpGet("{username}")]
    [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<User>>> GetByUsername(
        string username,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByUsernameAsync(username, cancellationToken);

        if (user is null)
            return NotFound(ApiResponse<User>.Fail($"User not found (Username: {username}).", HttpStatusCode.NotFound));

        return Ok(ApiResponse<User>.Ok(user));
    }
}
