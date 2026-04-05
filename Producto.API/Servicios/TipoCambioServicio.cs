using Microsoft.Extensions.Configuration;
using Servicios.Interfaces;
using Servicios.Modelos;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios
{

    public class TipoCambioServicio : ITipoCambioServicio
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TipoCambioServicio(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<decimal> ObtenerTipoCambioVentaHoyAsync(CancellationToken ct = default)
        {
            var urlBase = _configuration["BancoCentralCR:UrlBase"];
            var token = _configuration["BancoCentralCR:BearerToken"];

            if (string.IsNullOrWhiteSpace(urlBase))
                throw new InvalidOperationException("BancoCentralCR:UrlBase no está configurado.");

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("BancoCentralCR:BearerToken no está configurado.");

            var hoy = DateTime.Today;
            var fecha = hoy.ToString("yyyy/MM/dd");

            var endPoint = $"{urlBase}?fechaInicio={fecha}&fechaFin={fecha}&idioma=ES";

            var clienteBccr = _httpClientFactory.CreateClient("ServicioBccr");

            using var request = new HttpRequestMessage(HttpMethod.Get, endPoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var respuesta = await clienteBccr.SendAsync(request, ct);
            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync(ct);

            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<BccrTipoCambioResponse>(resultado, opciones);

            if (data is null)
                throw new InvalidOperationException("No se pudo deserializar la respuesta del BCCR.");

            if (!data.Estado)
                throw new InvalidOperationException($"BCCR respondió estado=false. Mensaje: {data.Mensaje}");

            var tipoCambio = data.Datos?
                .FirstOrDefault()?
                .Indicadores?.FirstOrDefault()?
                .Series?.FirstOrDefault()?
                .ValorDatoPorPeriodo;

            if (tipoCambio is null || tipoCambio <= 0)
                throw new InvalidOperationException("No se pudo extraer el tipo de cambio de la respuesta del BCCR.");

            return tipoCambio.Value;
        }
    }
}
