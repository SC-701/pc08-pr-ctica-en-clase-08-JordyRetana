using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages.Productos
{
    public class EliminarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        [BindProperty]
        public ProductoResponse? Producto { get; set; }

        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public EliminarModel(IConfiguracion configuracion)
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

        public async Task<IActionResult> OnPost(Guid id)
        {
            if (id == Guid.Empty)
                return RedirectToPage("./Index");

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EliminarProducto");
            string url = string.Format(endpoint, id);

            using var cliente = new HttpClient();
            var respuesta = await cliente.DeleteAsync(url);

            if (!respuesta.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el producto.");
                return Page();
            }

            TempData["MensajeExito"] = "El producto se eliminó correctamente.";
            return RedirectToPage("./Index");
        }
    }
}