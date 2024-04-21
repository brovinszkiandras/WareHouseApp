using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WH_APP_GUI
{
    internal class Email
    {
        private static SmtpClient client = new SmtpClient("smtp.gmail.com", 587); // Gmail SMTP server and port
        public static void send(string emailAddressTosend, string subject, string body)
        {
            client.EnableSsl = true; // Enable SSL/TLS
            client.Credentials = new NetworkCredential("horreumperfectum@gmail.com", "rfrj hsyw nplz voqo"); // Your Gmail and api key

            MailMessage message = new MailMessage();
            message.From = new MailAddress("horreumperfectum@gmail.com"); // Sender's email address
            message.To.Add(emailAddressTosend); // Recipient's email address
            message.Subject = subject;
            message.Body = body;

            // Send the email
            client.Send(message);
        }
    }
}
