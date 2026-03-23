using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Interfaces;

public interface IUserRepository
{
    Task<User> UserSingleByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> UserSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> UserSingleOrDefaultByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User> UserAddAsync(User user, CancellationToken cancellationToken);
}
