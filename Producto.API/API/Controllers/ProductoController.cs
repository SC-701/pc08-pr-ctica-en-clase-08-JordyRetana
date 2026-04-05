using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Producto.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase, IProductoController
    {
        private readonly IProductoFlujo _productoFlujo;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(
            IProductoFlujo productoFlujo,
            ILogger<ProductoController> logger)
        {
            _productoFlujo = productoFlujo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            try
            {
                var data = await _productoFlujo.Obtener();

                if (!data.Any())
                    return NoContent();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GET /api/Producto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener([FromRoute] Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("El id del producto es inválido.");

                var item = await _productoFlujo.Obtener(id);

                if (item == null)
                    return NotFound("El producto no existe.");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GET /api/Producto/{id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] ProductoRequest producto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = await _productoFlujo.Agregar(producto);

                return CreatedAtAction(nameof(Obtener), new { id }, new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en POST /api/Producto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar([FromRoute] Guid id, [FromBody] ProductoRequest producto)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("El id del producto es inválido.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existe = await _productoFlujo.Obtener(id);
                if (existe == null)
                    return NotFound("El producto no existe.");

                var resultado = await _productoFlujo.Editar(id, producto);
                return Ok(new { id = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en PUT /api/Producto/{id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar([FromRoute] Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("El id del producto es inválido.");

                var existe = await _productoFlujo.Obtener(id);
                if (existe == null)
                    return NotFound("El producto no existe.");

                var resultado = await _productoFlujo.Eliminar(id);
                return Ok(new { id = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en DELETE /api/Producto/{id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}