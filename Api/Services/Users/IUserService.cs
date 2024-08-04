using SignalChat.Api.Models;

namespace SignalChat.Api.Services.Users;

public interface IUserService
{
    ValueTask<UserModel> RegisterUser(UserModel user);
    ValueTask<UserModel?> GetUserById(int id);
    ValueTask<UserModel?> GetUserByEmail(string email);
    ValueTask<ICollection<UserModel>> GetAllUsers();
}
