using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DA
{
    public class SubCategoriaDA : ISubCategoriaDA
    {
        private readonly IRepositorioDapper _repositorioDapper;

        public SubCategoriaDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
        }

        public async Task<IEnumerable<SubCategoria>> Obtener()
        {
            const string sql = @"
                SELECT Id, Nombre
                FROM dbo.SubCategorias
                ORDER BY Nombre";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorio();
            return await sqlConnection.QueryAsync<SubCategoria>(sql);
        }
    }
}