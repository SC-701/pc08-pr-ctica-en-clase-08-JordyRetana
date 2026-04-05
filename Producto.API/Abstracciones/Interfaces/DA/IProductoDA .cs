using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IProductoDA
    {
        Task<IEnumerable<ProductoResponse>> Obtener();
        Task<ProductoResponse?> Obtener(Guid id);
        Task<Guid> Agregar(ProductoRequest producto);
        Task<Guid> Editar(Guid id, ProductoRequest producto);
        Task<Guid> Eliminar(Guid id);
    }
}