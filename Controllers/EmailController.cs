using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PersonalWebsite.Models;

namespace PersonalWebsite.Controllers
{
    [Route("api/Email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailSettings config;
        public EmailController(IOptions<EmailSettings> config)
        {
            this.config = config.Value;
        }

        [HttpPost]
        public async Task<dynamic> SendEmail([FromBody] EmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var message = new MailMessage();
            message.From = new MailAddress(config.From);
            message.To.Add(new MailAddress(config.To));
            message.Subject = "Contact from website form";
            message.Body = string.Format(body, model.Name, model.Email, model.Message);
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            using (var smtp = new SmtpClient())
            {
                if (config.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                {
                    // testing mode
                    smtp.DeliveryMethod = config.DeliveryMethod;
                    smtp.PickupDirectoryLocation = config.PickupDirectoryLocation;
                }
                else
                {
                    var credential = new NetworkCredential
                    {
                        UserName = config.From,
                        Password = config.Password
                    };
                    smtp.Credentials = credential;
                    smtp.UseDefaultCredentials = false;
                    smtp.Host = config.Host;
                    smtp.Port = config.Port;
                    smtp.EnableSsl = false;
                }

                await smtp.SendMailAsync(message);
            }
            return true;
        }
    }
}
