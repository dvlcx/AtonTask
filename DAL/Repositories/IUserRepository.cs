using AtonTask.Models.InputModels;
using AtonTask.Models.OutputModels;
using Models.Entities;

namespace AtonTask.DAL.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(UserCreateDto dto, string createdBy);
        Task<UserResponseDto?> GetUserDtoByLoginAsync(string login);
        Task<User> GetUserByLoginAsync(string login);
        Task<User?> GetUserByCredentialsAsync(string login, string password);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetUsersOlderThanAsync(int age);
        Task UpdateUserAsync(string login, string modifiedBy, UserUpdateDto dto);
        Task UpdatePasswordAsync(string login, string modifiedBy, string newPassword);
        Task UpdateLoginAsync(string login, string modifiedBy, string newLogin);
        Task SoftDeleteUserAsync(string login, string revokedBy);
        Task RestoreUserAsync(string login);
        Task HardDeleteUserAsync(string login);
    }
}