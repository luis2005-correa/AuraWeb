using System.Text;
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
    }
}