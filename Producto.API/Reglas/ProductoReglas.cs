using Reglas.Interfaces;
using Servicios.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reglas
{
    public class ProductoReglas : IProductoReglas
    {
        private readonly ITipoCambioServicio _tipoCambioServicio;

        public ProductoReglas(ITipoCambioServicio tipoCambioServicio)
        {
            _tipoCambioServicio = tipoCambioServicio;
        }

        public async Task<decimal> CalcularPrecioUsdAsync(decimal precioColones, CancellationToken ct = default)
        {
            if (precioColones < 0)
                throw new ArgumentOutOfRangeException(nameof(precioColones), "El precio no puede ser negativo.");

            var tipoCambioVenta = await _tipoCambioServicio.ObtenerTipoCambioVentaHoyAsync(ct);
            var usd = precioColones / tipoCambioVenta;

            return Math.Round(usd, 2, MidpointRounding.AwayFromZero);
        }

        public Task<decimal> ObtenerTipoCambioVentaHoyAsync(CancellationToken ct = default)
        {
            return _tipoCambioServicio.ObtenerTipoCambioVentaHoyAsync(ct);
        }
    }
}