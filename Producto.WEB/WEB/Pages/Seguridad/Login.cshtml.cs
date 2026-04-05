using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Abstracciones.Modelos.Seguridad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace WEB.Pages.Seguridad
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login loginInfo { get; set; } = new();

        [BindProperty]
        public Token token { get; set; } = new();

        private readonly IConfiguracion _configuracion;

        public LoginModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var hash = Autenticacion.GenerarHash(loginInfo.Contrasena);
            var passwordHash = Autenticacion.ObtenerHash(hash);

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPointsSeguridad", "Login");

            using var client = new HttpClient();
            var respuesta = await client.PostAsJsonAsync(endpoint, new LoginBase
            {
                NombreUsuario = loginInfo.Correo.Split("@")[0],
                CorreoElectronico = loginInfo.Correo,
                PasswordHash = passwordHash
            });

            respuesta.EnsureSuccessStatusCode();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            token = JsonSerializer.Deserialize<Token>(
                await respuesta.Content.ReadAsStringAsync(), opciones) ?? new Token();

            if (token.ValidacionExitosa && !string.IsNullOrWhiteSpace(token.AccessToken))
            {
                JwtSecurityToken? jwtToken = Autenticacion.leerToken(token.AccessToken);
                var claims = Autenticacion.GenerarClaims(jwtToken, token.AccessToken);
                await EstablecerAutenticacion(claims);

                var returnUrl = $"{HttpContext.Request.Query["ReturnUrl"]}";
                if (string.IsNullOrWhiteSpace(returnUrl))
                    return RedirectToPage("/Index");

                return Redirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            return Page();
        }

        private async Task EstablecerAutenticacion(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}