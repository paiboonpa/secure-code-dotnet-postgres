using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using secure_code.Data;
using secure_code.Models;
using System.Text.Json;

[Route("[controller]")]
[ApiController]
public class SqlInjectionController : Controller
{
    private readonly ApplicationDbContext _context;

    public SqlInjectionController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Helper method to convert User entities to Dictionary format for the view
    private List<Dictionary<string, object>> ConvertUsersToDictionary(List<User> users)
    {
        return users.Select(u => new Dictionary<string, object>
        {
            ["ID"] = u.Id,
            ["FIRSTNAME"] = u.FirstName,
            ["LASTNAME"] = u.LastName,
            ["PASSWORD"] = u.Password,
            ["SALARY"] = u.Salary,
            ["MONEY"] = u.Money,
            ["JSON_DATA"] = u.JsonData
        }).ToList();
    }

    // Vulnerable endpoint - Using FromSqlRaw with string interpolation (DEMONSTRATION PURPOSES ONLY)
    [HttpGet("injection")]
    public async Task<IActionResult> Injection([FromQuery] string? id)
    {
        // VULNERABLE: Direct string interpolation - SQL Injection possible
        string sql = $"SELECT * FROM users WHERE id = {id}";
        //string sql = "SELECT * FROM users WHERE id = @id";

        var users = new List<User>();
        try {
            users = await _context.Users
            .FromSqlRaw(sql)
            //.FromSqlRaw(sql, new NpgsqlParameter("@id", int.Parse(id)))
            .ToListAsync();
        } catch (Exception ex) {
            Console.WriteLine($"Error param in SQL: {ex.Message}");
        }
        

        ViewData["Results"] = ConvertUsersToDictionary(users);
        ViewData["Id"] = id;
        ViewData["Method"] = "Vulnerable (EF Core FromSqlRaw + String Interpolation)";
        return View("SqlInjection");
    }

    // Protected endpoint - Using FromSqlRaw with parameters
    [HttpGet("protected")]
    public async Task<IActionResult> Protected([FromQuery] int? id)
    {
        // PROTECTED: Using parameterized query with FromSqlRaw
        // PostgreSQL uses named parameters with @ prefix
        string sql = "SELECT * FROM users WHERE id = @id";

        // Try both parameter types - Varchar and Int32
        var users = await _context.Users
            .FromSqlRaw(sql, new NpgsqlParameter("@id", id))
            .ToListAsync();

        ViewData["Results"] = ConvertUsersToDictionary(users);
        ViewData["Id"] = id;
        ViewData["Method"] = "Protected (EF Core FromSqlRaw with Parameters)";
        return View("SqlInjection");
    }

    // Secure endpoint - Using LINQ with EF Core
    [HttpGet("secure")]
    public async Task<IActionResult> Secure([FromQuery] string? id)
    {
        // Input validation
        if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int userId) || userId <= 0)
        {
            return BadRequest("Invalid user ID. Must be a positive integer.");
        }

        // SECURE: Using LINQ with EF Core (recommended approach)
        var users = await _context.Users
            .Where(u => u.Id == userId)
            .ToListAsync();

        ViewData["Results"] = ConvertUsersToDictionary(users);
        ViewData["Id"] = id;
        ViewData["Method"] = "Secure (EF Core LINQ + Input Validation)";
        return View("SqlInjection");
    }

    // Vulnerable POST endpoint for updating password
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromForm] string? id, [FromForm] string? password)
    {
        Console.WriteLine($"=== UPDATE PASSWORD ENDPOINT ===");
        Console.WriteLine($"ID: {id}");
        Console.WriteLine($"New Password: {password}");

        // VULNERABLE: Direct string interpolation - SQL Injection possible
        string sql = $"UPDATE users SET password = '{password}' WHERE id = {id}";

        Console.WriteLine($"Executing SQL: {sql}");

        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql);

        Console.WriteLine($"Rows affected: {rowsAffected}");

        ViewData["Message"] = "Password changed!";
        ViewData["RowsAffected"] = rowsAffected;
        ViewData["Method"] = "Vulnerable (EF Core ExecuteSqlRaw + String Interpolation)";
        ViewData["Id"] = id;
        ViewData["Password"] = password;
        ViewData["Sql"] = sql;

        return View("SqlInjection");
    }

    // Protected UPDATE endpoint - Using parameters
    [HttpPost("update-password-protected")]
    public async Task<IActionResult> UpdatePasswordProtected([FromForm] string? id, [FromForm] string? password)
    {
        Console.WriteLine($"=== UPDATE PASSWORD PROTECTED ENDPOINT ===");
        Console.WriteLine($"ID: {id}");
        Console.WriteLine($"New Password: {password}");

        // PROTECTED: Using parameterized query
        string sql = "UPDATE users SET password = @password WHERE id = @id";

        NpgsqlParameter[] parameters = [];
        try {
            parameters = new[]
            {
                new NpgsqlParameter("@password", password ?? ""),
                new NpgsqlParameter("@id", Convert.ToInt32(id ?? "0"))
            };
        } catch (Exception ex) {
            Console.WriteLine($"Error converting id to int: {ex.Message}");
            parameters = new[]
            {
                new NpgsqlParameter("@password", ""),
                new NpgsqlParameter("@id", Convert.ToInt32("0"))
            };
        }
        

        Console.WriteLine($"Executing parameterized SQL: {sql}");
        Console.WriteLine($"Parameters: password={password}, id={id}");

        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

        Console.WriteLine($"Rows affected: {rowsAffected}");

        ViewData["Message"] = "Password changed securely!";
        ViewData["RowsAffected"] = rowsAffected;
        ViewData["Method"] = "Protected (EF Core ExecuteSqlRaw with Parameters)";
        ViewData["Id"] = id;
        ViewData["Password"] = password;
        ViewData["Sql"] = sql;

        return View("SqlInjection");
    }
}
