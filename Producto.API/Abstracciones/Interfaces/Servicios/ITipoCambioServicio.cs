using System.Threading;
using System.Threading.Tasks;

namespace Servicios.Interfaces
{
    public interface ITipoCambioServicio
    {
        Task<decimal> ObtenerTipoCambioVentaHoyAsync(CancellationToken ct = default);
    }
}