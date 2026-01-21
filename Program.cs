// ส่วนหัว (มักจะมีอยู่แล้วในเทมเพลตโปรเจกต์)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using secure_code.Data;
using secure_code.Data.Repositories;
using System.Text;

// using System.Net.Http; // มักจะไม่ต้องใช้ Using เพราะ AddHttpClient จัดการให้
// using System; // สำหรับ Uri, Exception

var builder = WebApplication.CreateBuilder(args);

// เพิ่มบริการสำหรับ Session
builder.Services.AddDistributedMemoryCache(); // ใช้ Memory Cache สำหรับเก็บ Session (ง่ายที่สุดสำหรับการทดสอบ)
// สำหรับ production ควรใช้ IDistributedCache ตัวอื่น เช่น Redis, SQL Server
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // กำหนด Session Timeout
    options.Cookie.HttpOnly = true; // กำหนด Cookie เป็น HttpOnly เพื่อความปลอดภัย
    options.Cookie.IsEssential = true; // ทำให้ Session Cookie จำเป็นต่อการทำงาน
});

// 1. กำหนดค่า Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // กำหนดชื่อ Scheme สำหรับการอ้างอิง
        // CookieAuthenticationDefaults.AuthenticationScheme คือ "Cookies"
        // options.LoginPath = "/Account/Login"; // ระบุหน้า Login (ถ้ามี)
    });

// 2. เพิ่ม Authorization Service พร้อม Policy
builder.Services.AddAuthorization();


// เพิ่มบริการสำหรับ CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( // ตั้งชื่อ Policy เป็น Default
        policy =>
        {
            // policy.WithOrigins("*") // อนุญาตเฉพาะ Origin นี้
            //       .AllowAnyMethod() // อนุญาต HTTP Method ใดก็ได้ (GET, POST, etc.)
            //       .AllowAnyHeader(); // อนุญาต Header ใดก็ได้
        });
});

// Add EF Core DbContext with enhanced logging
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlDb"))
           .EnableSensitiveDataLogging(true)  // Show parameter values
           .LogTo(Console.WriteLine, LogLevel.Information)  // Console logging
           .EnableDetailedErrors(true);  // Detailed error information
});

// Add Repository Pattern
builder.Services.AddScoped<IUserRepository, UserRepository>();

// เพิ่มบริการสำหรับ Controller (ถ้ายังไม่มี)
builder.Services.AddControllers();

// เพิ่มบริการ HttpClientFactory - เป็นวิธีที่ดีที่สุดในการสร้าง HttpClient ใน .NET Core
builder.Services.AddHttpClient();
// เพิ่มบริการ MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// เพิ่ม Middleware ใน Pipeline (ลำดับสำคัญ!)
// UseCors ต้องมาก่อน UseSession
app.UseCors(); // ใช้ CORS Middleware (จะใช้ Policy ที่เป็น Default)

app.UseSession(); // ใช้ Session Middleware

// เปิดใช้งาน static files (ถ้ามี)
app.UseStaticFiles();

// เปิดใช้งาน routing
app.UseRouting();

// กำหนดให้แอปพลิเคชันรันที่พอร์ต 8080
app.Urls.Add("http://localhost:8080");

// Top-level route registration สำหรับ MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();