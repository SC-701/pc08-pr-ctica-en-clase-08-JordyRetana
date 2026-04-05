using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Reglas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flujo
{
    public class ProductoFlujo : IProductoFlujo
    {
        private readonly IProductoDA _productoDA;
        private readonly IProductoReglas _productoReglas;

        public ProductoFlujo(IProductoDA productoDA, IProductoReglas productoReglas)
        {
            _productoDA = productoDA;
            _productoReglas = productoReglas;
        }

        public async Task<IEnumerable<ProductoResponse>> Obtener()
        {
            var items = (await _productoDA.Obtener()).ToList();

            foreach (var p in items)
            {
                p.PrecioUSD = await _productoReglas.CalcularPrecioUsdAsync(p.Precio);
            }

            return items;
        }

        public async Task<ProductoResponse?> Obtener(Guid id)
        {
            var producto = await _productoDA.Obtener(id);

            if (producto == null)
                return null;

            producto.PrecioUSD = await _productoReglas.CalcularPrecioUsdAsync(producto.Precio);

            return producto;
        }

        public Task<Guid> Agregar(ProductoRequest producto)
        {
            return _productoDA.Agregar(producto);
        }

        public Task<Guid> Editar(Guid id, ProductoRequest producto)
        {
            return _productoDA.Editar(id, producto);
        }

        public Task<Guid> Eliminar(Guid id)
        {
            return _productoDA.Eliminar(id);
        }
    }
}