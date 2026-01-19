using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // สำหรับการเข้าถึง Session
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using secure_code.Data;
using secure_code.Models;
using System.IO;

// ตั้งชื่อ Controller ให้ตรงกับ URL ที่ต้องการ เช่น /api/BrokenAccessControl
[Route("[controller]")]
[ApiController]
public class BrokenAccessControlController : Controller
{
    private readonly ApplicationDbContext _context;

    public BrokenAccessControlController(ApplicationDbContext context)
    {
        _context = context;
    }
    // Index action สำหรับแสดงหน้า HTML
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    // Login action สำหรับตั้งค่า Session
    [HttpGet("Login")]
    public IActionResult Login()
    {
        // ตั้งค่า Session user_id = 1 (แปลงจาก $_SESSION['user_id'] = 1;)
        HttpContext.Session.SetInt32("user_id", 1);

        // Redirect ไปยังหน้า Index (แปลงจาก header("location: ..."))
        return RedirectToAction("Index");
    }

    // Logout action สำหรับลบ Session
    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        // ลบ Session (แปลงจาก session_destroy())
        HttpContext.Session.Clear();

        // Redirect ไปยังหน้า Index (แปลงจาก header("location: ..."))
        return RedirectToAction("Index");
    }

    // New: Add user profile endpoint using EF Core
    [HttpGet("Profile")]
    public async Task<IActionResult> Profile()
    {
        var userId = HttpContext.Session.GetInt32("user_id");

        if (userId.HasValue && userId.Value == 1)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user != null)
            {
                return Json(new {
                    user.FirstName,
                    user.LastName,
                    user.Salary,
                    user.Money
                });
            }
        }

        return Forbid(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    // ตั้งชื่อ Action ให้ตรงกับหน้าที่ เช่น /api/Image/GetImage
    [HttpGet("GetImage")]
    public async Task<IActionResult> GetImage([FromQuery] string id)
    {
        // 1. การดึงค่า Session (แปลงจาก $_SESSION['user_id'])
        // Session ID ใน .NET จะเป็น byte array, เราต้องแปลงเป็น string หรือ int ก่อน
        var userId = HttpContext.Session.GetInt32("user_id"); // ดึงค่า integer จาก Session

        // 2. การตรวจสอบสิทธิ์ (แปลงจาก if (!empty($_SESSION['user_id']) && $_SESSION['user_id'] == 1))
        // ตรวจสอบว่ามีค่าใน Session และค่าเป็น 1 หรือไม่
        //bool hasAccess = userId.HasValue && userId.Value == 1;
        bool hasAccess = true; // ลบการตรวจสอบสิทธิ์เพื่อสาธิต Broken Access Control
        if (hasAccess)
        {
            // 3. การสร้าง Path (แปลงจาก $url = "forbidden/identity-cardno-".$_GET['id'].".jpg";)
            // ใช้ Path.Combine เพื่อสร้าง path ที่ปลอดภัยและถูกต้องตาม OS
            // ต้องระบุ Root Path ของ Application
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"); // หรือที่ใดที่คุณเก็บไฟล์
            string filePath = Path.Combine(rootPath, "images", "forbidden", $"identity-cardno-{id}.jpg");

            // 4. การตรวจสอบไฟล์ (แปลงจาก if (file_exists($url)))
            if (System.IO.File.Exists(filePath))
            {
                // 5. การส่งไฟล์ (แปลงจาก header("Content-type: image/jpeg"); echo file_get_contents($url);)
                // ใช้ File() method ของ ControllerBase เพื่อส่งไฟล์กลับไป
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "image/jpeg");
            }
            else
            {
                // 6. กรณีไม่พบไฟล์ (แปลงจาก else echo "file not found";)
                return NotFound("file not found");
            }
        }
        else
        {
            // ผู้ใช้ไม่มีสิทธิ์ (User ID ไม่ใช่ 1 หรือไม่มี Session)
            return Forbid(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}