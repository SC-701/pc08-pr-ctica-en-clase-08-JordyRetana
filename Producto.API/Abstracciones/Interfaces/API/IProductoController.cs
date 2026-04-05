using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.API
{
    public interface IProductoController
    {
        Task<IActionResult> Obtener();
        Task<IActionResult> Obtener(Guid id);
        Task<IActionResult> Agregar(ProductoRequest producto);
        Task<IActionResult> Editar(Guid id, ProductoRequest producto);
        Task<IActionResult> Eliminar(Guid id);
    }
}