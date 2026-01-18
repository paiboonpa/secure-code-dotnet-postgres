using Microsoft.EntityFrameworkCore;
using secure_code.Data;
using secure_code.Models;

namespace secure_code.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByIdVulnerableAsync(string id)
    {
        // VULNERABLE: For demonstration purposes only
        string sql = $"SELECT * FROM USERS WHERE ID = {id}";
        return await _context.Users
            .FromSqlRaw(sql)
            .FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetHighEarnersAsync(decimal minSalary)
    {
        return await _context.Users
            .Where(u => u.Salary > minSalary)
            .OrderByDescending(u => u.Salary)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalMoneyAsync()
    {
        return await _context.Users.SumAsync(u => u.Money);
    }

    public async Task<int> GetUserCountAsync()
    {
        return await _context.Users.CountAsync();
    }
}