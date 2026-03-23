using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User> GetOrCreateUserAsync(string username, string displayName, CancellationToken cancellationToken);
}
