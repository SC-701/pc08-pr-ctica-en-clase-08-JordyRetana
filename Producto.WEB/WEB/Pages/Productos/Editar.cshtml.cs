using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reglas;
using System.Text.Json;

namespace Web.Pages.Productos
{
    public class EditarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;
        private readonly ProductoReglas _productoReglas;

        [BindProperty]
        public ProductoRequest Producto { get; set; } = new();

        [BindProperty]
        public Guid Id { get; set; }

        [BindProperty]
        public List<SelectListItem> SubCategorias { get; set; } = new();

        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public EditarModel(IConfiguracion configuracion, ProductoReglas productoReglas)
        {
            _configuracion = configuracion;
            _productoReglas = productoReglas;
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            if (id == Guid.Empty)
                return RedirectToPage("./Index");

            await CargarSubCategorias();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProducto");
            string url = string.Format(endpoint, id);

            using var cliente = new HttpClient();
            var respuesta = await cliente.GetAsync(url);

            if (!respuesta.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            var contenido = await respuesta.Content.ReadAsStringAsync();
            var productoResponse = JsonSerializer.Deserialize<ProductoResponse>(contenido, _jsonOptions);

            if (productoResponse == null)
                return RedirectToPage("./Index");

            Id = productoResponse.Id;
            Producto = new ProductoRequest
            {
                Nombre = productoResponse.Nombre,
                Descripcion = productoResponse.Descripcion,
                Precio = productoResponse.Precio,
                Stock = productoResponse.Stock,
                CodigoBarras = productoResponse.CodigoBarras,
                IdSubCategoria = productoResponse.IdSubCategoria
            };

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await CargarSubCategorias();

            if (Id == Guid.Empty)
                return RedirectToPage("./Index");

            if (Producto.IdSubCategoria == Guid.Empty)
                ModelState.AddModelError("Producto.IdSubCategoria", "Debe seleccionar una subcategoría.");

            if (!ModelState.IsValid)
                return Page();

            if (!_productoReglas.ProductoEsValido(Producto))
            {
                ModelState.AddModelError(string.Empty, "Los datos del producto no son válidos.");
                return Page();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EditarProducto");
            string url = string.Format(endpoint, Id);

            using var cliente = new HttpClient();
            var respuesta = await cliente.PutAsJsonAsync(url, Producto);

            if (!respuesta.IsSuccessStatusCode)
            {
                var detalle = await respuesta.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error API: {detalle}");
                return Page();
            }

            TempData["MensajeExito"] = "El producto se actualizó correctamente.";
            return RedirectToPage("./Index");
        }

        private async Task CargarSubCategorias()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerSubCategorias");

            using var cliente = new HttpClient();
            var respuesta = await cliente.GetAsync(endpoint);

            if (!respuesta.IsSuccessStatusCode || respuesta.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                SubCategorias = new List<SelectListItem>();
                return;
            }

            var contenido = await respuesta.Content.ReadAsStringAsync();
            var lista = JsonSerializer.Deserialize<List<SubCategoria>>(contenido, _jsonOptions) ?? new List<SubCategoria>();

            SubCategorias = lista.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Nombre
            }).ToList();
        }
    }
}