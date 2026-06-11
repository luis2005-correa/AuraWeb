using Microsoft.EntityFrameworkCore;
using Aura.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Aura.Services;

var builder = WebApplication.CreateBuilder(args); 

if (FirebaseApp.DefaultInstance == null)
{
#pragma warning disable CS0618
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile("aura-1ee36-firebase-adminsdk-fbsvc-d5a8a8ade8.json")
    });
#pragma warning restore CS0618
}

// 🔐 CONFIGURACIÓN DE BASE DE DATOS FUERTE Y LIMPIA PARA POSTGRESQL (SUPABASE)
var connectionString = builder.Configuration.GetConnectionString("con");

builder.Services.AddDbContext<TiendaRopaDbContext>(options =>
{
    // 1. Le obligamos a usar únicamente PostgreSQL con reintentos para fallos transitorios
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });

    // 2. Desactivamos el proveedor interno para que ignore el UseSqlServer viejo de una vez por todas
    options.UseInternalServiceProvider(null);
});

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. CONFIGURAR AUTENTICACIÓN JWT DE FIREBASE
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];
var authorityUrl = $"https://securetoken.google.com/{firebaseProjectId}";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authorityUrl;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authorityUrl,
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true,
            // Mapea el Custom Claim "role" de Firebase al rol de .NET
            RoleClaimType = "role"
        };

        // Este evento permite leer el token incluso si viene guardado en una cookie de sesión
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Si guardas el token en la sesión en el Login, el sistema lo leerá automáticamente aquí
                var token = context.HttpContext.Session.GetString("FirebaseToken");
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

// 3. AGREGAR EL SERVICIO DE AUTORIZACIÓN
builder.Services.AddAuthorization();

// Registrar el HttpClient junto a nuestro servicio personalizado de Firebase
builder.Services.AddHttpClient<FirebaseService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ¡EL ORDEN AQUÍ ES CRUCIAL!
app.UseSession();        // 1. Primero habilitamos la sesión para poder extraer el token

app.UseAuthentication(); // 2. Autenticamos al usuario usando el token extraído de la sesión
app.UseAuthorization();  // 3. Validamos si tiene el rol permitido para la ruta

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();