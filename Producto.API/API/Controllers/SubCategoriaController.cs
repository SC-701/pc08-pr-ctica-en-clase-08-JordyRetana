using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Producto.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoriaController : ControllerBase, ISubCategoriaController
    {
        private readonly ISubCategoriaFlujo _subCategoriaFlujo;
        private readonly ILogger<SubCategoriaController> _logger;

        public SubCategoriaController(ISubCategoriaFlujo subCategoriaFlujo, ILogger<SubCategoriaController> logger)
        {
            _subCategoriaFlujo = subCategoriaFlujo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            try
            {
                var data = await _subCategoriaFlujo.Obtener();

                if (!data.Any())
                    return NoContent();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GET /api/SubCategoria");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}