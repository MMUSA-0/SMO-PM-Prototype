// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmtpEmailService.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.Notifications
{
    using Microsoft.Extensions.Logging;
    using System;
    #region usings

    using System.Net.Mail;

    #endregion

    /// <summary>
    ///     The smtp email service.
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        //private readonly IEmailService emailService;
        private readonly ILogger<SmtpEmailService> _logger;
        public SmtpEmailService(ILogger<SmtpEmailService> logger)
        {
            //this.emailService = emailService;
            _logger = logger;
        }
        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="emailMessage">
        /// The email message.
        /// </param>
        /// <param name="notificationSettings">
        /// todo: describe notificationSettings parameter on SendEmail
        /// </param>
        
        public async Task<bool> SendEmail(EmailMessage emailMessage, NotificationSettings notificationSettings)
        {
            using (SmtpClient smtp = new SmtpClient(notificationSettings.SmtpServer, notificationSettings.SmtpPort))
            {
                smtp.EnableSsl = notificationSettings.SmtpEnableSSL;
                smtp.UseDefaultCredentials = notificationSettings.IsSmtpAuthenticated;
                smtp.Credentials = new System.Net.NetworkCredential(notificationSettings.SmtpUserName,
                    notificationSettings.SmtpPassword);
                var mail = emailMessage.ToMailMessage();

                try
                {
                    smtp.Send(mail);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return false;
                }
            }
        }
    }
}