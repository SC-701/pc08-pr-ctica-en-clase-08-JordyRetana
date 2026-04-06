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

            if (apiConfig == null || string.IsNullOrWhiteSpace(apiConfig.UrlBase))
                throw new Exception($"No se encontró configuración para la sección '{seccion}'.");

            var metodo = apiConfig.Metodos?
                .FirstOrDefault(m => m.Nombre == nombre)?
                .Valor;

            if (string.IsNullOrWhiteSpace(metodo))
                throw new Exception($"No se encontró el método '{nombre}' en la sección '{seccion}'.");

            return $"{apiConfig.UrlBase}{metodo}";
        }
    }
}