using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Interfaces;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> UserGetAllAsync(CancellationToken cancellationToken);
    Task<User> UserSingleByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> UserSingleOrDefaultByIdAsync(int id, CancellationToken cancellationToken);
}
