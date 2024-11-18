using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;

namespace MeowCompany.Pages
{
    [Authorize]
    public class SentmailModel : PageModel
    {
        public List<MailData> listEmails = new List<MailData>();
        public MailData selectedEmail = null;
        public string selectedSubject = "";

        public class MailData
        {
            public int ID { get; set; }
            public string date { get; set; }
            public string frommail { get; set; }
            public string tomail { get; set; }
            public string subject { get; set; }
            public string message { get; set; }
            public bool IsRead { get; set; }
        }

        public void OnGet(int id = 0)
        {
            string connectionString = "Server=tcp:meowgroup.database.windows.net,1433;Initial Catalog=MeowCompany;Persist Security Info=False;User ID=meow;Password=Meemee-12;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT date, frommail, tomail, subject, message, IsRead, ID FROM Emails WHERE frommail = @currentUserEmail";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@currentUserEmail", currentUserEmail);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listEmails.Add(new MailData
                            {
                                date = reader.GetDateTime(0).ToString("dd/MM/yyyy"),
                                frommail = reader.GetString(1),
                                tomail = reader.GetString(2),
                                subject = reader.GetString(3),
                                message = reader.GetString(4),
                                IsRead = !reader.IsDBNull(5) && reader.GetBoolean(5),
                                ID = reader.GetInt32(6)
                            });
                        }
                    }
                }

                //แสดงรายละเอียด email ที่เลือก
                if (id != 0)
                {
                    sql = "SELECT date, frommail, tomail, subject, message, IsRead FROM Emails WHERE ID = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                selectedEmail = new MailData
                                {
                                    date = reader.GetDateTime(0).ToString("dd/MM/yyyy"),
                                    frommail = reader.GetString(1),
                                    tomail = reader.GetString(2),
                                    subject = reader.GetString(3),
                                    message = reader.GetString(4),
                                    IsRead = reader.GetBoolean(5)
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}
