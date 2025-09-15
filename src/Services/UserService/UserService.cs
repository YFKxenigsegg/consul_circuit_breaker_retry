using Microsoft.EntityFrameworkCore;

namespace CCBR.Services.UserService;

public interface IUserService
{
    Task<User> GetUserAsync(int id);
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<IEnumerable<User>> GetUsersAsync();
    Task<bool> DeleteUserAsync(int id);
}

public class UserService(
    UserDbContext context,
    ILogger<UserService> logger)
    : IUserService
{
    private readonly UserDbContext _context = context;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<User> GetUserAsync(int id)
    {
        _logger.LogInformation($"Getting user with id: {id}");
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        _logger.LogInformation($"Creating user with email: {request.Email}");

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        _logger.LogInformation("Getting all users");
        return await _context.Users.Where(u => u.IsActive).ToListAsync();
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        _logger.LogInformation($"Deleting user with id: {id}");
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;
        user.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
