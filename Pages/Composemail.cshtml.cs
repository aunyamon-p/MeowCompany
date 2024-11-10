using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace MeowCompany.Pages
{
    public class ComposemailModel : PageModel
    {
        public class MailData
        {
            public string date { get; set; }
            public string frommail { get; set; }
            public string tomail { get; set; }
            public string subject { get; set; }
            public string message { get; set; }
        }

        public MailData Mail = new MailData();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Mail.date = Request.Form["date"];
            Mail.frommail = Request.Form["frommail"];
            Mail.tomail = Request.Form["tomail"];
            Mail.subject = Request.Form["subject"];
            Mail.message = Request.Form["message"];

            try
            {
                string connectionString = "Server = tcp:meowgroup.database.windows.net,1433; Initial Catalog = MeowCompany; Persist Security Info = False; User ID = meow; Password =Meemee-12; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Emails (date, frommail, tomail, subject, message) VALUES (@date, @frommail, @tomail, @subject, @message);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@date", Mail.date);
                        command.Parameters.AddWithValue("@frommail", Mail.frommail);
                        command.Parameters.AddWithValue("@tomail", Mail.tomail);
                        command.Parameters.AddWithValue("@subject", Mail.subject);
                        command.Parameters.AddWithValue("@message", Mail.message);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }

            Mail.date = "";
            Mail.frommail = "";
            Mail.tomail = "";
            Mail.subject = "";
            Mail.message = "";
            successMessage = "Your message is already sent.";

            return Page();
        }
    }
}
