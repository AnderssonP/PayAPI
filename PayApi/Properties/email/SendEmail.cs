namespace PayApi.Properties.email
{
    public class SendEmail
    {
        public static void sendEmailToCustomer(string email, string subject, string message)
        {
            try
            {                
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("pontus.andersson@gritacademy.se", "fveqtazaobvkoupx"),
                    EnableSsl = true,
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("pontus.andersson@gritacademy.se"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };
                mail.To.Add(email);

                
                smtpClient.Send(mail);

                Console.WriteLine("Email sent successfully!");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
        }
    }
}
