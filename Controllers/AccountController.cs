using Aura.Services;
using Aura.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;

namespace Aura.Controllers
{
    public class AccountController : Controller
    {
        private readonly FirebaseService _firebaseService;
        private readonly TiendaRopaDbContext _context;

        public AccountController(FirebaseService firebaseService, TiendaRopaDbContext context)
        {
            _firebaseService = firebaseService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("FirebaseToken")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Por favor, llena todos los campos.";
                return View();
            }

            var token = await _firebaseService.LoginAsync(email, password);

            if (token != null)
            {
                HttpContext.Session.SetString("FirebaseToken", token);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("FirebaseToken")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario, string password)
        {
            // Removemos las propiedades internas del modelo para evitar fallos de validación arbitrarios
            ModelState.Remove(nameof(usuario.PasswordHash));
            ModelState.Remove(nameof(usuario.Rol));
            ModelState.Remove(nameof(usuario.RolId));

            if (string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "El correo electrónico y la contraseña son obligatorios.";
                return View(usuario);
            }

            if (password.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres.";
                return View(usuario);
            }

            // 1. Intentar registrar las credenciales en Firebase Auth
            var firebaseUid = await _firebaseService.RegistrarUsuarioAuthAsync(usuario.Email, password);

            if (string.IsNullOrEmpty(firebaseUid))
            {
                ViewBag.Error = "Error al crear la cuenta. El correo electrónico podría estar en uso.";
                return View(usuario);
            }

            try
            {

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                usuario.RolId = 2; // 2 = Cliente
                usuario.FechaRegistro = DateTime.UtcNow;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();


                var token = await _firebaseService.LoginAsync(usuario.Email, password);

                if (token != null)
                { 
                    HttpContext.Session.SetString("FirebaseToken", token);

                    TempData["SuccessMessage"] = "¡Cuenta creada e inicio de sesión correcto!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    TempData["SuccessMessage"] = "Cuenta creada. Por favor, inicia sesión de forma manual.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                var errorReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ViewBag.Error = $"Error en Supabase: {errorReal}";
                return View(usuario);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("FirebaseToken");
            return RedirectToAction("Login"); 
        }

        [HttpGet("/account/inicializar-admin")]
        public async Task<IActionResult> InicializarAdmin()
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
#pragma warning disable CS0618
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("aura-1ee36-firebase-adminsdk-fbsvc-d5a8a8ade8.json")
                    });
#pragma warning restore CS0618
                }
                    
                string uid = "rpmZy4Q3wlPiOlEWmVWLu3EzqsA2";


                var claims = new Dictionary<string, object>()
                {
                    { "role", "Admin" }
                };

                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);

                return Ok("¡Rol de Admin asignado correctamente en Firebase Auth!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}