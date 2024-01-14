using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Services.Interfaces;
using System.Net.Mail;
using System.Runtime.InteropServices.JavaScript;

namespace MISBack.Controllers
{
    [ApiController]
    [Route("api/mail")]
    public class MailController : ControllerBase
    {
        /*private readonly IEmailSender _mailsService;

        public MailController(IEmailSender mailsService)
        {
            _mailsService = mailsService;
        }*/

        [HttpPost]
        public async Task SendEmail([FromQuery] string email, string mSubject, string message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("brasscout@gmail.com");
                mail.To.Add(email);
                mail.Subject = mSubject;
                mail.Body = message;

                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("brasscout@gmail.com", "szup fpwp chel ehum");
                smtp.EnableSsl = true;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //await _mailsService.SendEmailAsync(email, subject, message);
        }
    }
}
