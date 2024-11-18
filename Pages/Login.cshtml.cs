using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace MeowCompany.Pages
{
    public class LoginModel : PageModel
    {
        public class LoginData
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public LoginData User = new LoginData();
        public string errorMessage = "";

        public void OnGet()
        {
        }

        // เมธอดสำหรับการเข้าสู่ระบบผู้ใช้
        public async Task<IActionResult> OnPost()
        {
            User.username = Request.Form["username"];
            User.password = Request.Form["password"];

            if (string.IsNullOrEmpty(User.username) || string.IsNullOrEmpty(User.password))
            {
                errorMessage = "Please fill in all required fields.";
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:meowgroup.database.windows.net,1433;Initial Catalog=MeowCompany;Persist Security Info=False;User ID=meow;Password=Meemee-12;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // เปลี่ยนจาก COUNT เป็นการดึงข้อมูลทั้งหมด
                    string sql = "SELECT email, role FROM Users WHERE username = @username AND password = @password";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", User.username);
                        command.Parameters.AddWithValue("@password", User.password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // ดึงอีเมลและบทบาทจากฐานข้อมูล
                                string email = reader.GetString(0);
                                string role = reader.GetString(1);

                                // สร้าง Claim เพื่อเก็บข้อมูล
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, User.username),
                                    new Claim(ClaimTypes.Email, email),
                                    new Claim(ClaimTypes.Role, role)
                                };

                                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var principal = new ClaimsPrincipal(identity);

                                // บันทึกการเข้าสู่ระบบ
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                                return RedirectToPage("/Index");
                            }
                            else
                            {
                                errorMessage = "Invalid username or password.";
                                return Page();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }
        }

        // เมธอดสำหรับการออกจากระบบผู้ใช้
        public async Task<IActionResult> OnPostSignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }
    }
}
