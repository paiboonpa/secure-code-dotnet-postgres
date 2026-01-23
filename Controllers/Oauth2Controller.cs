using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;

[Authorize] // บังคับว่าต้อง Login ผ่าน Google ก่อนถึงจะเข้า Method ในนี้ได้
[ApiController]
[Route("[controller]")]
public class Oauth2Controller : ControllerBase
{
    private readonly IConfiguration _config;

    public Oauth2Controller(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("read")]
    public async Task<IActionResult> ReadSheet()
    {
        // 1. ดึง Access Token ที่ถูกเก็บไว้ใน Cookie หลัง Login
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token not found. Please login again.");
        }

        try 
        {
            // 2. สร้าง Credentials จาก Token
            var credential = GoogleCredential.FromAccessToken(accessToken);

            // 3. สร้าง Sheets Service
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MyDotNetApp"
            });

            // 4. กำหนด ID ของ Spreadsheet (ดึงจาก config หรือใส่ตรงๆ)
            string spreadsheetId = "18SRMG5GfVJVCOUmfkdnh3GlIjymr8TBZCj4KSImaS3E";
            string range = "Sheet3!A1:B10"; 

            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            var values = response.Values;

            if (values == null || values.Count == 0)
            {
                return Ok(new { message = "No data found." });
            }

            // 5. จัดรูปแบบข้อมูล (Mapping)
            var resultList = values.Select(row => new {
                Column1 = row.Count > 0 ? row[0]?.ToString() : "",
                Column2 = row.Count > 1 ? row[1]?.ToString() : ""
            });

            return Ok(resultList);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error fetching data: {ex.Message}");
        }
    }
}