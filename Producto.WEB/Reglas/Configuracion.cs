using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Reglas
{
    public class Configuracion : IConfiguracion
    {
        private readonly IConfiguration _configuracion;

        public Configuracion(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public string ObtenerMetodo(string seccion, string nombre)
        {
            var apiConfig = _configuracion
                .GetSection(seccion)
                .Get<APIEndPoint>();

            var urlBase = apiConfig?.UrlBase ?? string.Empty;

            var metodo = apiConfig?.Metodos?
                .FirstOrDefault(m => m.Nombre == nombre)?
                .Valor ?? string.Empty;

            return $"{urlBase}{metodo}";
        }
    }
}