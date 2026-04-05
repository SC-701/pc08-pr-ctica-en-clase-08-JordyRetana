using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public List<ProductoResponse> Productos { get; set; } = new();

        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public IndexModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task OnGet()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProductos");

            using var cliente = new HttpClient();
            var respuesta = await cliente.GetAsync(endpoint);

            if (!respuesta.IsSuccessStatusCode)
            {
                Productos = new List<ProductoResponse>();
                return;
            }

            if (respuesta.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Productos = new List<ProductoResponse>();
                return;
            }

            var contenido = await respuesta.Content.ReadAsStringAsync();
            Productos = JsonSerializer.Deserialize<List<ProductoResponse>>(contenido, _jsonOptions) ?? new List<ProductoResponse>();
        }
    }
}