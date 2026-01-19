// ในไฟล์ Controllers/SSRFController.cs
using Microsoft.AspNetCore.Mvc; // สำหรับ ControllerBase, Attributes เช่น [Route], [HttpGet], [FromQuery]
using System.Net; // สำหรับ WebClient
using System; // สำหรับ Uri, TimeSpan, Exception
using System.Threading.Tasks; // สำหรับ async/await

// กำหนด Route หลักสำหรับ Controller นี้
// [controller] จะถูกแทนที่ด้วยชื่อ Controller โดยตัดคำว่า "Controller" ออก
// ดังนั้น Endpoint หลักจะเป็น /SSRF
[Route("[controller]")]
[ApiController] // แนะนำให้ใช้ Attribute นี้สำหรับ Controller ที่เป็น API โดยเฉพาะ
public class SSRFController : ControllerBase // ใช้ ControllerBase เพราะเราไม่ต้องใช้ View
{

    // Action Method ที่จะรับ GET Request
    [HttpGet("fetch")] // หมายความว่า Method นี้จะทำงานเมื่อมีการเรียก GET ไปที่ Path /SSRF/fetch
    public async Task<IActionResult> Fetch([FromQuery] string? url) // รับค่า 'url' จาก Query String
    {
        // *** ตรวจสอบค่า url เหมือนเดิม ***
        if (string.IsNullOrEmpty(url))
        {
            // ใช้ Method BadRequest() ที่มีอยู่ใน ControllerBase แทน Results.BadRequest()
            return BadRequest("Please provide a 'url' query parameter.");
        }

        // *** Optional: ตรวจสอบรูปแบบ URL เบื้องต้น เหมือนเดิม ***
        // if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) ||
        //     (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        // {
        //      return BadRequest("Please provide a valid HTTP or HTTPS URL.");
        // }

        // *** ดึงเนื้อหาจาก URL ด้วย WebClient ***
        try
        {
            using (var webClient = new WebClient())
            {
                // WebClient ไม่มี property Timeout โดยตรง
                // ใช้ DownloadStringTaskAsync ที่มี CancellationToken แทน
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                // ดึงเนื้อหาจาก URL (รองรับทั้ง http, https, และ file://)
                string output = await webClient.DownloadStringTaskAsync(url);

                // *** แสดงผลเนื้อหา ***
                // ใช้ Method Content() ที่มีใน ControllerBase แทน Results.Content() หรือ Results.Text()
                // Content(string content, string contentType) จะส่ง Response เป็น 200 OK พร้อมเนื้อหา
                return Content(output, "text/html");
            }
        }
        catch (WebException e)
        {
            // *** จัดการข้อผิดพลาด ***
            // ใช้ Method StatusCode(statusCode, body) หรือ Content(body, contentType, statusCode)
            // StatusCode(500, message) จะส่ง Response เป็น 500 Internal Server Error พร้อม Body ที่เป็นข้อความ
            return StatusCode(500, $"Error fetching content from {url}: {e.Message}");
        }
        catch (Exception ex)
        {
            // จัดการข้อผิดพลาดอื่นๆ
             return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
}