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

        public async Task<IActionResult> OnPost()
        {
            User.username = Request.Form["username"];
            User.password = Request.Form["password"];

            if (string.IsNullOrEmpty(User.username) || string.IsNullOrEmpty(User.password))
            {
                errorMessage = "Please fill all the fields.";
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:meowgroup.database.windows.net,1433; Initial Catalog=MeowCompany; Persist Security Info=False; User ID=meow; Password=Meemee-12; MultipleActiveResultSets=False; Encrypt=True; TrustServerCertificate=False; Connection Timeout=30";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT COUNT(*) FROM Users WHERE username = @username AND password = @password";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", User.username);
                        command.Parameters.AddWithValue("@password", User.password);

                        int userExists = (int)command.ExecuteScalar();

                        if (userExists > 0)
                        {
                            // สร้าง ClaimsPrincipal สำหรับการตรวจสอบสิทธิ์
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, User.username)
                            };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

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
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }
        }
    }
}


