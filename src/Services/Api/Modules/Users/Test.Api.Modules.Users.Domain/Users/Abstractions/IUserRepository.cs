namespace Test.Api.Modules.Users.Domain.Users.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(User user);
}