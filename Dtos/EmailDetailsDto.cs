namespace SMTPClient.Dtos
{
    public class EmailDetailsDto
    {
        public string[] To { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
        public IFormFileCollection Attachments { get; set; }
    }
}
