﻿namespace SMTPClient.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
