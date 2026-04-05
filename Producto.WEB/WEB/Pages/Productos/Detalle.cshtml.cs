using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages.Productos
{
    public class DetalleModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public ProductoResponse? Producto { get; set; }

        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public DetalleModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            if (id == Guid.Empty)
                return RedirectToPage("./Index");

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProducto");
            string url = string.Format(endpoint, id);

            using var cliente = new HttpClient();
            var respuesta = await cliente.GetAsync(url);

            if (!respuesta.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            var contenido = await respuesta.Content.ReadAsStringAsync();
            Producto = JsonSerializer.Deserialize<ProductoResponse>(contenido, _jsonOptions);

            if (Producto == null)
                return RedirectToPage("./Index");

            return Page();
        }
    }
}