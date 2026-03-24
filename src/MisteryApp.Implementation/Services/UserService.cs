using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Implementation.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public virtual async Task<IReadOnlyList<User>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await userRepository.UserGetAllAsync(cancellationToken);
    }

    public virtual async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await userRepository.UserSingleOrDefaultByIdAsync(id, cancellationToken);
    }
}
