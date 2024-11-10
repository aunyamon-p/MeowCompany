using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace MeowCompany.Pages
{
    [Authorize]
    public class InboxModel : PageModel
    {
        public List<MailData> listEmails = new List<MailData>();
        public MailData selectedEmail = null;
        public string selectedSubject = "";

        public class MailData
        {
            public string date { get; set; }
            public string frommail { get; set; }
            public string tomail { get; set; }
            public string subject { get; set; }
            public string message { get; set; }
        }

        public void OnGet(string subject = "")
        {
            string connectionString = "Server=tcp:meowgroup.database.windows.net,1433;Initial Catalog=MeowCompany;Persist Security Info=False;User ID=meow;Password=Meemee-12;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ดึงข้อมูลทั้งหมดมาแสดงในรายการ
                string sql = "SELECT date, frommail, tomail, subject, message FROM Emails";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
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
                                message = reader.GetString(4)
                            });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(subject))
                {
                    selectedSubject = subject;
                    sql = "SELECT date, frommail, tomail, subject, message FROM Emails WHERE subject = @subject";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@subject", subject);
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
                                    message = reader.GetString(4)
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}
