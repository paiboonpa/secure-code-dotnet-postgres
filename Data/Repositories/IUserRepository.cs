using secure_code.Models;

namespace secure_code.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByIdVulnerableAsync(string id);
    Task<List<User>> GetHighEarnersAsync(decimal minSalary);
    Task<decimal> GetTotalMoneyAsync();
    Task<int> GetUserCountAsync();
}