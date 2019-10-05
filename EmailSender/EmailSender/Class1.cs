using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EmailSender
{
    public static class Email
    {
        /// <summary>
        /// Отправить сообщение по электронной почте.
        /// </summary>
        /// <param name="message">Текстовое сообщение.</param>
        /// <param name="email">E-Mail.</param>
        /// <param name="smtp">Секция SMTP.</param>
        /// <param name="subject">Тема письма.</param>    
        public static void SendEmail(string smtp, string email, string subject, string message, Collection<Attachment> attachmentList)
        {

                var props = smtp.Split(new[] { '|' });
                string host = props[1];
                int port = int.Parse(props[2], CultureInfo.InvariantCulture);
                string from = props[0];
                string login = props[3];
                string password = props[4];

                bool useSsl = false;

                if (props.Length == 6)
                    useSsl = bool.Parse(props[5]);

            if (string.IsNullOrWhiteSpace(login)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(from))
                throw new Exception($"Неккоректно задан набор значений: Имя пользователя = {login}, Пароль = {password}, Адрес отправителя = {from}. Сообщение не было отправлено.");

                using (var client = new SmtpClient(host, port))
                {
                    client.Credentials = new NetworkCredential(login, password);
                    client.EnableSsl = useSsl;
                    using (var mailMessage = new MailMessage())
                    {
                        //Формирование сообщения
                        foreach (string currentEmail in email.Split(';'))
                            mailMessage.To.Add(new MailAddress(currentEmail));

                        mailMessage.From = new MailAddress(from);
                        mailMessage.Subject = subject;

                        if (attachmentList != null)
                            foreach (Attachment currentAttach in attachmentList)
                                mailMessage.Attachments.Add(currentAttach);

                        mailMessage.Body = message;
                        mailMessage.Priority = MailPriority.High;
                        mailMessage.BodyEncoding = Encoding.UTF8;

                        //непосредственно отправка сообщения
                        client.Send(mailMessage);
                    }
                }
        }
    }
}
