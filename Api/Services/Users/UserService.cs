using Microsoft.EntityFrameworkCore;
using SignalChat.Api.Data;
using SignalChat.Api.Models;

namespace SignalChat.Api.Services.Users;

public class UserService : IUserService
{
    private readonly SignalChatDbContext _context;

    public UserService(SignalChatDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ICollection<UserModel>> GetAllUsers()
    {
        var users = await _context.Users.AsNoTracking().ToListAsync();

        return users;
    }

    public async ValueTask<UserModel?> GetUserByEmail(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(user => user.Email.Equals(email));

        return user;
    }

    public async ValueTask<UserModel?> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);

        return user;
    }

    public async ValueTask<UserModel> RegisterUser(UserModel user)
    {
        var result = _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return result.Entity;
    }
}
