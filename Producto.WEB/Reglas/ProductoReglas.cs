using Abstracciones.Modelos;
using System.Text.RegularExpressions;

namespace Reglas
{
    public class ProductoReglas
    {
        public bool CodigoBarrasEsValido(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            return Regex.IsMatch(codigo, @"^[A-Za-z0-9-]+$");
        }

        public bool PrecioEsValido(decimal precio)
        {
            return precio > 0;
        }

        public bool StockEsValido(int stock)
        {
            return stock >= 0;
        }

        public bool ProductoEsValido(ProductoRequest producto)
        {
            if (producto == null)
                return false;

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return false;

            if (string.IsNullOrWhiteSpace(producto.Descripcion))
                return false;

            if (!PrecioEsValido(producto.Precio))
                return false;

            if (!StockEsValido(producto.Stock))
                return false;

            if (!CodigoBarrasEsValido(producto.CodigoBarras))
                return false;

            if (producto.IdSubCategoria == Guid.Empty)
                return false;

            return true;
        }
    }
}