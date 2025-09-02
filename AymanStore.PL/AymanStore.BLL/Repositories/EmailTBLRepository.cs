﻿using AymanStore.BLL.Interfaces;
using AymanStore.DAL.Contexts;
using AymanStore.DAL.Entities;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Net;
using System.Net.Mail;

namespace AymanStore.BLL.Repositories
{
    public class EmailTBLRepository : GenericRepository<EmailTBL>, IEmailTBLRepository
    {
        private readonly AymanStoreDbContext _context;

        public EmailTBLRepository(AymanStoreDbContext context) :base(context)
        {
            _context = context;
        }

        public async Task SendEmail(EmailTBL email)
        {
            var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);

            client.EnableSsl = true;

            client.Credentials = new NetworkCredential(email.From, "you code");

            client.Send(email.From, email.To, email.Subject, email.Body);

            client.Dispose();
        }

        public async Task SendEmailAsync(EmailTBL emails)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("yourcompany@example.com"));
            email.To.Add(MailboxAddress.Parse(emails.To));
            email.Subject = emails.Subject;

            // HTML body
            email.Body = new TextPart(TextFormat.Html) { Text = emails.Body };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emails.From, "you code");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmailAsync(EmailTBL emails,int SecondType = 0, List<string>? ccEmails = null)
        {
            var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com") // or your SMTP server
            {
                Port = 587,
                Credentials = new NetworkCredential(emails.From, "scpaxajogqlcagxy"),
                EnableSsl = true,
            };

            var mail = new MailMessage
            {
                From = new MailAddress(emails.From, "LegendShop"),
                Subject = emails.Subject,
                Body = emails.Body,
                IsBodyHtml = true
            };

            // To
            mail.To.Add(emails.To);

            // CC (optional)
            if (ccEmails != null)
            {
                foreach (var cc in ccEmails)
                {
                    mail.CC.Add(cc);
                }
            }

            await smtp.SendMailAsync(mail);
        }
    }
}
