using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Implementation.Services;

public class UserService(
    IUserRepository userRepository,
    TimeProvider timeProvider) : IUserService
{
    public virtual async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await userRepository.UserSingleOrDefaultByUsernameAsync(username, cancellationToken);
    }

    public virtual async Task<User> GetOrCreateUserAsync(string username, string displayName, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.UserSingleOrDefaultByUsernameAsync(username, cancellationToken);
        if (existingUser is not null)
            return existingUser;

        var newUser = new User
        {
            Username = username,
            DisplayName = displayName,
            CreatedAt = timeProvider.GetUtcNow()
        };

        return await userRepository.UserAddAsync(newUser, cancellationToken);
    }
}
