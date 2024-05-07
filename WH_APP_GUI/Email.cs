using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WH_APP_GUI
{
    internal class Email
    {
        private static SmtpClient client = new SmtpClient("smtp.gmail.com", 587); // Gmail SMTP server and port
        private static string EmailAddress = "horreumperfectum@gmail.com";
        private static string ApiKey = "rfrj hsyw nplz voqo";
        public static void send(string emailAddressTosend, string subject, string body)
        {
            if (TherIsExistingEmailTxt())
            {
                string[] datas = File.ReadAllLines("email.txt");
                EmailAddress = datas[0];
                ApiKey = datas[1];
            }

            client.EnableSsl = true; // Enable SSL/TLS
            client.Credentials = new NetworkCredential(EmailAddress, ApiKey); // Your Gmail and api key

            MailMessage message = new MailMessage();
            message.From = new MailAddress(EmailAddress); // Sender's email address
            message.To.Add(emailAddressTosend); // Recipient's email address
            message.Subject = subject;
            message.Body = body;

            // Send the email
            client.Send(message);
        }

        private static bool TherIsExistingEmailTxt()
        {
            if (File.Exists("email.txt"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
