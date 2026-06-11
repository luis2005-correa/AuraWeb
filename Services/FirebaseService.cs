using System.Text;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text.Json;

namespace Aura.Services
{
    public class FirebaseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _firebaseApiKey;

        public FirebaseService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _firebaseApiKey = configuration["Firebase:ApiKey"] ?? throw new InvalidOperationException("Firebase ApiKey is missing from configuration.");
        }

        // Método para iniciar sesión con Correo y Contraseña
        public async Task<string?> LoginAsync(string email, string password)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}";

            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);
                return doc.RootElement.GetProperty("idToken").GetString();
            }

            return null;
        }

        public async Task<string?> RegistrarUsuarioAuthAsync(string email, string password)
        {

            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={_firebaseApiKey}";

            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);

                return doc.RootElement.GetProperty("localId").GetString();
            }

            return null;
        }

        // Método para enviar correo de recuperación de contraseña
        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null) return false;

                // Aquí genera el link
                string resetLink = await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email);

                // Aquí envía el correo
                await EnviarCorreoPersonalizadoAsync(email, resetLink);

                return true;
            }
            catch (Exception ex)
            {
                // Al poner un punto de interrupción (Breakpoint) aquí, 
                // podrás revisar la variable 'ex' y ver el mensaje de error real.
                throw;
            }
        }
        private async Task EnviarCorreoPersonalizadoAsync(string emailDestino, string linkRecuperacion) 
        {
            var mensaje = new MimeMessage();
            // Recuerda cambiar "tu-correo-real@gmail.com" por tu cuenta configurada
            mensaje.From.Add(new MailboxAddress("Aura Soporte", "luiscorreaacosta@gmail.com"));
            mensaje.To.Add(new MailboxAddress("", emailDestino));
            mensaje.Subject = "Restablece tu contraseña - Aura";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
    <div style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; background-color: #09090b; padding: 50px 20px; color: #f4f4f5; margin: 0;'>
        <div style='max-width: 460px; margin: 0 auto; background-color: #121214; padding: 40px 32px; border: 1px solid #27272a; border-radius: 16px; box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.5);'>
            
            <div style='text-align: center; margin-bottom: 32px;'>
                <h2 style='font-size: 28px; font-weight: 800; color: #ffffff; letter-spacing: -0.05em; margin: 0;'>Aura</h2>
                <p style='font-size: 13px; color: #a1a1aa; margin: 6px 0 0 0; letter-spacing: 0.05em; text-transform: uppercase;'>Control de acceso e identidad digital</p>
            </div>
            
            <div style='border-top: 1px solid #27272a; margin-bottom: 32px;'></div>

            <p style='font-size: 15px; line-height: 1.6; color: #e4e4e7; margin: 0 0 16px 0;'>Hola,</p>
            <p style='font-size: 15px; line-height: 1.6; color: #a1a1aa; margin: 0 0 32px 0;'>Recibimos una solicitud para restablecer la contraseña de acceso a tu cuenta en <strong>Aura</strong>. Si no realizaste esta acción, puedes ignorar este mensaje con total tranquilidad.</p>
            
            <div style='text-align: center; margin: 35px 0;'>
                <a href='{linkRecuperacion}' style='background-color: #ffffff; color: #09090b; padding: 14px 32px; font-size: 14px; font-weight: 600; text-decoration: none; border-radius: 10px; display: inline-block; letter-spacing: -0.01em; box-shadow: 0 4px 12px rgba(255, 255, 255, 0.1);'>
                    Restablecer Contraseña
                </a>
            </div>

            <p style='font-size: 12px; line-height: 1.5; color: #71717a; text-align: center; margin: 32px 0 0 0;'>
                Este enlace de seguridad es temporal y expirará pronto.
            </p>
            
            <div style='border-top: 1px solid #27272a; margin-top: 32px; margin-bottom: 24px;'></div>
            
            <p style='font-size: 11px; text-align: center; color: #52525b; margin: 0; letter-spacing: 0.02em;'>
                &copy; {DateTime.Now.Year} Aura. Todos los derechos reservados.
            </p>
        </div>
    </div>";

            mensaje.Body = bodyBuilder.ToMessageBody();

            // 4. Conexión segura e inicio de envío (Asegúrate de cambiar tu-contraseña-de-aplicacion)
            using (var clienteSmtp = new SmtpClient())
            {
                await clienteSmtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await clienteSmtp.AuthenticateAsync("luiscorreaacosta@gmail.com", "momi yrnx fjgv rpya");
                await clienteSmtp.SendAsync(mensaje);
                await clienteSmtp.DisconnectAsync(true);
            }
        }
    }
}