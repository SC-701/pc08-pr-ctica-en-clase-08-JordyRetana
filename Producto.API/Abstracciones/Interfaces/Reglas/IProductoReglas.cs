using System.Threading;
using System.Threading.Tasks;

namespace Reglas.Interfaces
{
    public interface IProductoReglas
    {
        Task<decimal> CalcularPrecioUsdAsync(decimal precioColones, CancellationToken ct = default);
        Task<decimal> ObtenerTipoCambioVentaHoyAsync(CancellationToken ct = default);
    }
}