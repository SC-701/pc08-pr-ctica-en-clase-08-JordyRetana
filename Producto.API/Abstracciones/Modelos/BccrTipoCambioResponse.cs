using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Servicios.Modelos
{
    public class BccrTipoCambioResponse
    {
        [JsonPropertyName("estado")]
        public bool Estado { get; set; }

        [JsonPropertyName("mensaje")]
        public string? Mensaje { get; set; }

        [JsonPropertyName("datos")]
        public List<BccrDato>? Datos { get; set; }
    }

    public class BccrDato
    {
        [JsonPropertyName("titulo")]
        public string? Titulo { get; set; }

        [JsonPropertyName("periodicidad")]
        public string? Periodicidad { get; set; }

        [JsonPropertyName("indicadores")]
        public List<BccrIndicador>? Indicadores { get; set; }
    }

    public class BccrIndicador
    {
        [JsonPropertyName("codigoIndicador")]
        public string? CodigoIndicador { get; set; }

        [JsonPropertyName("nombreIndicador")]
        public string? NombreIndicador { get; set; }

        [JsonPropertyName("series")]
        public List<BccrSerie>? Series { get; set; }
    }

    public class BccrSerie
    {
        [JsonPropertyName("fecha")]
        public string? Fecha { get; set; }

        [JsonPropertyName("valorDatoPorPeriodo")]
        public decimal ValorDatoPorPeriodo { get; set; }
    }
}