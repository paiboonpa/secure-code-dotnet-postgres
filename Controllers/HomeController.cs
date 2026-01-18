using Microsoft.AspNetCore.Mvc;

namespace secure_code.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask; // Placeholder for async operation
            return Content("Welcome to Secure Code C# Application! 🚀<br><br>" +
                          "Available Endpoints:<br>" +
                          "• /Hash - Password hashing<br>" +
                          "• /SqlInjection - SQL Injection examples<br>" +
                          "• /SSRF - Server-Side Request Forgery<br>" +
                          "• /Cors - CORS configuration test<br><br>" +
                          "Application is running successfully!", "text/html");
        }
    }
}