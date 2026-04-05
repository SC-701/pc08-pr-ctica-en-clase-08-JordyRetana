namespace Servicios.Configuracion
{
    public class BancoCentralCrOptions
    {
        public const string SectionName = "BancoCentralCR";

        public string UrlBase { get; set; } = string.Empty;
        public string BearerToken { get; set; } = string.Empty;
    }
}