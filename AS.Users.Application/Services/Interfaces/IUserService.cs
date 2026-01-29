using AS.Users.Application.DTO;

namespace AS.Users.Application.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(string id);
    Task<UserDto> CreateUserAsync(CreateUserModel model);
    Task UpdateUserAsync(string id, UpdateUserModel model);
    Task DeleteUserAsync(string id);
}