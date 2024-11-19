using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace MeowCompany.Pages
{
    public class RegisterModel : PageModel
    {
        public class UserData
        {
            public string role { get; set; }
            public string email { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string cfpassword { get; set; }
        }

        public UserData User = new UserData();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            User.role = Request.Form["role"];
            User.email = Request.Form["email"];
            User.username = Request.Form["username"];
            User.password = Request.Form["password"];
            User.cfpassword = Request.Form["cfpassword"];

            // ตรวจสอบการกรอกข้อมูล
            if (string.IsNullOrEmpty(User.role) ||
                string.IsNullOrEmpty(User.email) ||
                string.IsNullOrEmpty(User.username) ||
                string.IsNullOrEmpty(User.password) ||
                string.IsNullOrEmpty(User.cfpassword))
            {
                errorMessage = "Please fill all the fields.";
                return Page();
            }

            // ตรวจสอบว่ารหัสผ่านตรงกันหรือไม่
            if (User.password != User.cfpassword)
            {
                errorMessage = "Passwords do not match.";
                return Page();
            }

            if (!Regex.IsMatch(User.username, @"^[a-zA-Z0-9]+$"))
            {
                errorMessage = "Username must contain only letters and numbers, without spaces.";
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:meowgroup.database.windows.net,1433;Initial Catalog=MeowCompany;Persist Security Info=False;User ID=meow;Password=Meemee-12;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // ตรวจสอบว่า username ซ้ำหรือไม่
                    string checkUsernameSql = "SELECT COUNT(*) FROM Users WHERE username = @username";
                    using (SqlCommand checkCommand = new SqlCommand(checkUsernameSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@username", User.username);
                        int userExists = (int)checkCommand.ExecuteScalar();

                        if (userExists > 0)
                        {
                            errorMessage = "Username already exists.";
                            return Page();
                        }
                    }


                    // ตรวจสอบว่า email ซ้ำหรือไม่
                    string checkemailSql = "SELECT COUNT(*) FROM Users WHERE email = @email";
                    using (SqlCommand checkEmailCommand = new SqlCommand(checkemailSql, connection))
                    {
                        checkEmailCommand.Parameters.AddWithValue("@email", User.email);
                        int emailExists = (int)checkEmailCommand.ExecuteScalar();

                        if (emailExists > 0)
                        {
                            errorMessage = "Email already exists.";
                            return Page();
                        }
                    }




                    // บันทึกข้อมูลผู้ใช้ใหม่
                    string sql = "INSERT INTO Users (role, email, username, password) VALUES (@role, @email, @username, @password)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@role", User.role);
                        command.Parameters.AddWithValue("@email", User.email);
                        command.Parameters.AddWithValue("@username", User.username);
                        command.Parameters.AddWithValue("@password", User.password);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }

            // ล้างข้อมูลหลังจากบันทึกสำเร็จ
            User.role = "";
            User.email = "";
            User.username = "";
            User.password = "";
            User.cfpassword = "";
            successMessage = "Registered successfully!";

            return Page();
        }
    }
}