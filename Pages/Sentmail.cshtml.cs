using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

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
            public string date { get; set; }
            public string frommail { get; set; }
            public string tomail { get; set; }
            public string subject { get; set; }
            public string message { get; set; }
            public bool IsRead { get; set; }
        }

        public void OnGet(string subject = "")
        {
            string connectionString = "Server=tcp:meowgroup.database.windows.net,1433;Initial Catalog=MeowCompany;Persist Security Info=False;User ID=meow;Password=Meemee-12;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            // ดึง email ของผู้ใช้ที่ล็อกอิน
            string currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            // ตรวจสอบว่าได้ค่า currentUserEmail หรือไม่
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ดึงข้อมูลเฉพาะอีเมลที่ผู้ใช้ที่ล็อกอินเป็นคนส่ง
                string sql = "SELECT date, frommail, tomail, subject, message, IsRead FROM Emails WHERE frommail = @currentUserEmail";
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
                                IsRead = !reader.IsDBNull(5) && reader.GetBoolean(5)
                            });
                        }
                    }
                }

                // แสดงรายละเอียดเมื่อเลือกหัวข้อ
                if (!string.IsNullOrEmpty(subject))
                {
                    selectedSubject = subject;
                    sql = "SELECT date, frommail, tomail, subject, message, IsRead FROM Emails WHERE subject = @subject AND frommail = @currentUserEmail";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@subject", subject);
                        command.Parameters.AddWithValue("@currentUserEmail", currentUserEmail);
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
