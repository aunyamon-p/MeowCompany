using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


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

            //??????? password ?????? confrim password ????
            if (User.password != User.cfpassword)
            {
                errorMessage = "Password don't match";
                return Page();
            }

            try
            {   
                string connectionString = "Server = tcp:meowgroup.database.windows.net,1433; Initial Catalog = MeowCompany; Persist Security Info = False; User ID = meow; Password =Meemee-12; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //??????? username ???????
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

                    //??????????????????? Users
                    string sql = "INSERT INTO Users (role, email, username, password) VALUES (@role, @email, @username, @password);";
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

            User.role = "";
            User.email = "";
            User.username = "";
            User.password = "";
            User.cfpassword = "";
            successMessage = "Add user successfully";

            return Page();
        }
    }
   
}



      