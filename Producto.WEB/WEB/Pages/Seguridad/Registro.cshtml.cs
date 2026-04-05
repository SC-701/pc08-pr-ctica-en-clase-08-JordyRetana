using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;

namespace WEB.Pages.Seguridad
{
    public class RegistroModel : PageModel
    {
        [BindProperty]
        public UsuarioBase Usuario { get; set; } = new();

        private readonly IConfiguracion _configuracion;

        public RegistroModel(IConfiguracion configuracion)
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

            var hash = Autenticacion.GenerarHash(Usuario.Password ?? string.Empty);
            Usuario.PasswordHash = Autenticacion.ObtenerHash(hash);

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPointsSeguridad", "Registro");

            using var client = new HttpClient();
            var respuesta = await client.PostAsJsonAsync(endpoint, Usuario);
            respuesta.EnsureSuccessStatusCode();

            return RedirectToPage("/Seguridad/Login");
        }
    }
}