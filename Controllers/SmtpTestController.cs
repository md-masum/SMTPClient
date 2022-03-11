using Microsoft.AspNetCore.Mvc;
using SMTPClient.Dtos;
using SMTPClient.Service;

namespace SMTPClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmtpTestController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public SmtpTestController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost(Name = "SendEmail")]
        public async Task<IActionResult> SendEmail([FromForm]EmailDetailsDto emailDetails)
        {
            var message = new Message(emailDetails.To, emailDetails.Subject, emailDetails.HtmlContent, emailDetails.Attachments);
            await _emailSender.SendEmailAsync(message);

            return Ok();
        }
    }
}