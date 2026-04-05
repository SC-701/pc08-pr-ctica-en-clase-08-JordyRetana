using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.API
{
    public interface ISubCategoriaController
    {
        Task<IActionResult> Obtener();
    }
}