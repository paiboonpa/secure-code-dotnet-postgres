using Microsoft.AspNetCore.Mvc;
using static BCrypt.Net.BCrypt;

[Route("[controller]")]
[ApiController]
public class HashController : ControllerBase
{
    [HttpGet("bcrypt")]
    public IActionResult Hash([FromQuery] string? password)
    {
        string databasePassword = "password";
        string hashedDatabasePassword = HashPassword(databasePassword);
        string result = $"Database password: {hashedDatabasePassword}<br>";

        if (string.IsNullOrEmpty(password))
        {
            result += "Please provide a 'password' query parameter.";
            return Content(result, "text/html");
        }

        string hashedInputPassword = HashPassword(password);
        result += $"New Input password: {hashedInputPassword}";

        // DON'T COMPARE PASSWORD DIRECTLY WHEN USE BCRYPT!!
        if (hashedDatabasePassword == hashedInputPassword)
            result += "<br>Password Matched!!";
        else
            result += "<br>Password NOT MATCH!!";

        if (Verify(password, hashedDatabasePassword))
            result += "<br>Password Matched!!";
        else
            result += "<br>Password NOT MATCH!!";

        return Content(result, "text/html");
    }

    [HttpGet("stupidhash")]
    public IActionResult StupidHash([FromQuery] string? password)
    {
        string databasePassword = "password";
        string hashedDatabasePassword = StupidHashFunc(databasePassword);
        string result = $"Database password: {hashedDatabasePassword}<br>";

        if (string.IsNullOrEmpty(password))
        {
            result += "Please provide a 'password' query parameter.";
            return Content(result, "text/html");
        }

        string hashedInputPassword = StupidHashFunc(password);
        result += $"New Input password: {hashedInputPassword}";

        if (hashedDatabasePassword == hashedInputPassword)
            result += "<br>Password Matched!!";
        else
            result += "<br>Password NOT MATCH!!";

        return Content(result, "text/html");
    }

    public static string StupidHashFunc(string pwd)
    {
        // ค่า "123" ในที่นี้ทำหน้าที่เหมือน "Fixed Salt" หรือ "Secret"
        return pwd + "123";
    }
}
