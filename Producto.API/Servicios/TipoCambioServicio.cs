using Abstracciones.Interfaces.Flujo;
using Microsoft.Extensions.Configuration;
using Servicios.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace Servicios
{
    public class TipoCambioServicio : ITipoCambioServicio
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TipoCambioServicio(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<decimal> ObtenerTipoCambioVentaHoyAsync(CancellationToken ct = default)
        {
            try
            {
                var url = _configuration["BancoCentralCR:UrlBase"];
                var bearerToken = _configuration["BancoCentralCR:BearerToken"];

                if (string.IsNullOrWhiteSpace(url))
                    return 0m;

                using var request = new HttpRequestMessage(HttpMethod.Get, url);

                if (!string.IsNullOrWhiteSpace(bearerToken))
                {
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                }

                using var response = await _httpClient.SendAsync(request, ct);

                if (!response.IsSuccessStatusCode)
                    return 0m;

                var content = await response.Content.ReadAsStringAsync(ct);

                if (string.IsNullOrWhiteSpace(content))
                    return 0m;

                using JsonDocument doc = JsonDocument.Parse(content);

                if (TryFindDecimal(doc.RootElement, out decimal tipoCambio))
                    return tipoCambio;

                return 0m;
            }
            catch
            {
                return 0m;
            }
        }

        private static bool TryFindDecimal(JsonElement element, out decimal value)
        {
            value = 0m;

            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetDecimal(out value))
                        return true;
                    break;

                case JsonValueKind.String:
                    var text = element.GetString();
                    if (!string.IsNullOrWhiteSpace(text) &&
                        decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        return true;
                    if (!string.IsNullOrWhiteSpace(text) &&
                        decimal.TryParse(text, NumberStyles.Any, new CultureInfo("es-CR"), out value))
                        return true;
                    break;

                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        if (TryFindDecimal(prop.Value, out value))
                            return true;
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        if (TryFindDecimal(item, out value))
                            return true;
                    }
                    break;
            }

            return false;
        }
    }
}