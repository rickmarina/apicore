using com.rorisoft.utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace com.rorisoft.mail
{
    public interface IEmailSender
    {
        void SendEmailAsync();
    }

    public class EmailSettings
    {
        public string? server { get; set; }
        public string? user { get; set; }
        public string? password { get; set; }
        public string? port { get; set; }
        public bool? ssl { get; set; }
        public string? from { get; set; }
        public string? template { get; set; }
        public string? botonurl { get; set; }
        public string? botontexto { get; set; }
        public string? footerCompany { get; set; }
        public string? footerPowered { get; set; }
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public string? culture { get; set; }
        public string? asunto { get; set; }
        public string? texto { get; set; }
        public string? texto2 { get; set; }
        
        public MailMessage mensaje { get; set; }
        public string? sendTo { get; set; }
        public string? sendCC { get; set; }
        public string? sendCC2 { get; set; }
        
        public string fichero { get; set; }
        public ArrayList sendtoMultiple { get; set; }

        public string? plantillaHTML { get; set; }
        public string? error { get; set; }


        public EmailSender(EmailSettings settings) : this(Options.Create<EmailSettings>(settings))
        {
            
        }
        public EmailSender(IOptions<EmailSettings> settings)
        {
            _emailSettings = settings.Value; 

            fichero = String.Empty;
            mensaje = new MailMessage();
            sendtoMultiple = [];
        }

        public bool enviarEmail()
        {
            bool ok = true;

            ArgumentNullException.ThrowIfNull(_emailSettings.user, nameof(_emailSettings.user));
            ArgumentNullException.ThrowIfNull(_emailSettings.from, nameof(_emailSettings.from));

            mensaje.From = new MailAddress(_emailSettings.user, _emailSettings.from);
            if (sendtoMultiple.Count == 0) {
                ArgumentNullException.ThrowIfNullOrEmpty(sendTo, nameof(sendTo));
                mensaje.To.Add(new MailAddress(sendTo));
            }
            else
                foreach (String item in sendtoMultiple)
                {
                    mensaje.To.Add(new MailAddress(item));
                }

            if (!String.IsNullOrEmpty(sendCC2))
                mensaje.Bcc.Add(new MailAddress(sendCC2));
            mensaje.Subject = asunto;


            //Cargar plantilla si existe
            if (!string.IsNullOrEmpty(_emailSettings.template))
            {
                plantillaHTML = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), _emailSettings.template));

                plantillaHTML = plantillaHTML.Replace("[texto]", texto);
                plantillaHTML = plantillaHTML.Replace("[texto2]", texto2);
                plantillaHTML = plantillaHTML.Replace("[footer-company]", _emailSettings.footerCompany);
                plantillaHTML = plantillaHTML.Replace("[footer-powered]", _emailSettings.footerPowered);
                
                mensaje.Body = plantillaHTML;

            }
            else
            {
                mensaje.Body = texto;
            }

            //Insertar fichero
            if (!String.IsNullOrEmpty(fichero) && System.IO.File.Exists(fichero)) mensaje.Attachments.Add(new Attachment(fichero));

            mensaje.IsBodyHtml = true;
            mensaje.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient(_emailSettings.server);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(_emailSettings.user, _emailSettings.password);
            smtp.EnableSsl = _emailSettings?.ssl ?? false;
            smtp.Port = ParseUtils.parseInteger(_emailSettings?.port ?? "0");


            try
            {
                smtp.Send(mensaje);
            }
            catch (Exception ex)
            {
                ok = false;
                error = ex.Message;
                //LogUseCase.insertarLog(new LogVO() { tipo = LogVO.TIPO_LOG.ERROR_EMAIL, descripcion = error });
            }

            smtp.Dispose();
            return ok;
        }

        public async void SendEmailAsync()
        {

            ArgumentNullException.ThrowIfNull(_emailSettings.user, nameof(_emailSettings.user));
            ArgumentNullException.ThrowIfNull(_emailSettings.from, nameof(_emailSettings.from));

            //Crear mensaje
            mensaje.From = new MailAddress(_emailSettings.user, _emailSettings.from);
            if (sendtoMultiple.Count == 0) {
                ArgumentNullException.ThrowIfNullOrEmpty(sendTo, nameof(sendTo));
                mensaje.To.Add(new MailAddress(sendTo));
            }
            else
                foreach (String item in sendtoMultiple)
                {
                    mensaje.To.Add(new MailAddress(item));
                }

            if (!String.IsNullOrEmpty(sendCC2))
                mensaje.Bcc.Add(new MailAddress(sendCC2));
            mensaje.Subject = asunto;

            //Cargar plantilla si existe
            if (!string.IsNullOrEmpty(_emailSettings.template) && File.Exists(_emailSettings.template))
            {

                //Crear diccionario de reemplazos
                Dictionary<string, string> replaces = new Dictionary<string, string>()
                {
                    {"[texto]", texto ?? string.Empty },
                    {"[texto2]", texto2 ?? string.Empty },
                    {"[footer-company]", _emailSettings?.footerCompany ?? string.Empty},
                    {"[footer-powered]", _emailSettings ?.footerPowered ?? string.Empty},
                    {"[botonurl]", _emailSettings ?.botonurl ?? string.Empty },
                    {"[botontexto]", _emailSettings ?.botontexto ?? string.Empty }

                };

                plantillaHTML = File.ReadAllText(_emailSettings?.template ?? string.Empty);

                foreach (var item in replaces)
                {
                    plantillaHTML = plantillaHTML.Replace(item.Key.ToString(), item.Value?.ToString() ?? "");
                }

                mensaje.Body = plantillaHTML;
            }
            else
            {
                mensaje.Body = texto;
            }


            //Insertar fichero
            if (!String.IsNullOrEmpty(fichero) && System.IO.File.Exists(fichero)) mensaje.Attachments.Add(new Attachment(fichero));

            mensaje.IsBodyHtml = true;
            mensaje.Priority = MailPriority.Normal;

            using (var smtp = new SmtpClient(_emailSettings?.server))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(_emailSettings?.user, _emailSettings?.password);
                smtp.EnableSsl = _emailSettings?.ssl ?? false;
                smtp.Port = ParseUtils.parseInteger(_emailSettings?.port ?? "0");
                await smtp.SendMailAsync(mensaje);
            }


        }
    }
}
