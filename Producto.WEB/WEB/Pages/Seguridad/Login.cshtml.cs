using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;

namespace Web.Pages.Seguridad
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public LoginModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        [BindProperty]
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Contrasenia { get; set; } = string.Empty;

        public string? MensajeError { get; set; }
        public string? MensajeExito { get; set; }

        public void OnGet()
        {
            if (TempData.ContainsKey("MensajeExito"))
                MensajeExito = TempData["MensajeExito"]?.ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var hash = Autenticacion.ObtenerHash(
                Autenticacion.GenerarHash((Contrasenia ?? string.Empty).Trim()));

            var login = new LoginBase
            {
                NombreUsuario = string.Empty,
                CorreoElectronico = (CorreoElectronico ?? string.Empty).Trim(),
                PasswordHash = hash.Trim()
            };

            using var cliente = new HttpClient();
            string endpoint = _configuracion.ObtenerMetodo("ApiSeguridad", "Login");

            var respuesta = await cliente.PostAsJsonAsync(endpoint, login);
            var contenido = await respuesta.Content.ReadAsStringAsync();

            if (!respuesta.IsSuccessStatusCode)
            {
                MensajeError = $"Error {(int)respuesta.StatusCode}: {contenido}";
                return Page();
            }

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var resultado = JsonSerializer.Deserialize<Token>(contenido, opciones);

            if (resultado == null || !resultado.ValidacionExitosa || string.IsNullOrWhiteSpace(resultado.AccessToken))
            {
                MensajeError = "Credenciales inválidas.";
                return Page();
            }

            JwtSecurityToken? jwtToken = Autenticacion.leerToken(resultado.AccessToken);
            List<Claim> claims = Autenticacion.GenerarClaims(jwtToken, resultado.AccessToken);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            var urlRedirigir = $"{HttpContext.Request.Query["ReturnUrl"]}";
            if (string.IsNullOrWhiteSpace(urlRedirigir))
                return RedirectToPage("/Productos/Index");

            return Redirect(urlRedirigir);
        }
    }
}