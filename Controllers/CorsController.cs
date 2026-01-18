// ในไฟล์ Controllers/CorsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // จำเป็นสำหรับ ISession และ Extension Methods เช่น GetInt32, SetInt32
using System; // จำเป็นสำหรับ TimeSpan (ถ้าไม่ได้ using ในไฟล์ Program.cs)

[Route("[controller]")] // กำหนด Base Route สำหรับ Controller นี้ เช่น /Session
[ApiController] // Attribute แนะนำสำหรับ Controller ที่เป็น API
public class CorsController : ControllerBase
{
    // ไม่ต้องมี Constructor เพื่อรับ HttpContext.Session โดยตรง
    // เพราะ HttpContext สามารถเข้าถึงได้ผ่าน ControllerBase อยู่แล้ว

    [HttpGet("demo")] // กำหนด Route สำหรับ Action นี้ เช่น /Session/demo สำหรับ GET Request
    [HttpPost("demo")] // กำหนด Route สำหรับ Action นี้ เช่น /Session/demo สำหรับ POST Request
    public IActionResult SessionDemo()
    {
        // เข้าถึง Session ของ Request ปัจจุบัน
        ISession session = HttpContext.Session;

        // ตัวแปรสำหรับเก็บค่า views
        int views = 0;

        // พยายามอ่านค่า 'views' จาก Session
        // Session ใน ASP.NET Core เก็บข้อมูลเป็น Byte Array
        // ต้องใช้ Extension Method เช่น GetInt32, SetInt32, GetString, SetString
        int? viewsFromSession = session.GetInt32("views"); // GetInt32 จะคืนค่า null ถ้าไม่มี หรือแปลงไม่ได้

        if (viewsFromSession.HasValue) // ตรวจสอบว่ามีค่า 'views' อยู่ใน Session หรือไม่
        {
            // ถ้ามีค่า ให้อ่านค่าปัจจุบัน
            views = viewsFromSession.Value;
            views++; // เพิ่มค่า

            // เก็บค่าใหม่กลับเข้าไปใน Session
            session.SetInt32("views", views);

            // ส่ง Response กลับเป็น HTML พร้อมค่า views ที่อัปเดตแล้ว
            return Content($"<p>views: {views}</p>", "text/html");
        }
        else
        {
            // ถ้ายังไม่มีค่า 'views' ใน Session
            views = 1; // กำหนดค่าเริ่มต้น

            // เก็บค่าเริ่มต้นเข้าไปใน Session
            session.SetInt32("views", views);

            // ส่ง Response กลับเป็นข้อความต้อนรับ
             return Content("welcome to the session demo. refresh!", "text/plain");
        }
    }
}