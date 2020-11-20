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
        private readonly IOptions<EmailSettings> config;
        public EmailController(IOptions<EmailSettings> config)
        {
            this.config = config;
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
            message.From = new MailAddress("worker@trajkov.dev");
            message.To.Add(new MailAddress("admin@trajkov.dev")); //replace with valid value
            message.Subject = "Contact from website form";
            message.Body = string.Format(body, model.Name, model.Email, model.Message);
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                if (config.Value.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                {
                    smtp.DeliveryMethod = config.Value.DeliveryMethod;
                    smtp.PickupDirectoryLocation = config.Value.PickupDirectoryLocation;
                }
                else
                {
                    var credential = new NetworkCredential
                    {
                        UserName = config.Value.From,  // replace with valid value
                        Password = config.Value.Password  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.UseDefaultCredentials = false;
                    smtp.Host = config.Value.Host;
                    smtp.Port = config.Value.Port;
                    smtp.EnableSsl = false;
                }

                await smtp.SendMailAsync(message);
            }
            return true;
        }
    }
}
