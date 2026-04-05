using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DA
{
    public class ProductoDA : IProductoDA
    {
        private readonly IRepositorioDapper _repositorioDapper;

        public ProductoDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
        }

        public async Task<IEnumerable<ProductoResponse>> Obtener()
        {
            const string sql = @"
            SELECT 
                p.Id,
                p.Nombre,
                p.Descripcion,
                p.Precio,
                p.Stock,
                p.CodigoBarras,
                p.IdSubCategoria,
                sc.Nombre AS SubCategoria,
                c.Nombre AS Categoria
            FROM dbo.Producto p
            INNER JOIN dbo.SubCategorias sc ON p.IdSubCategoria = sc.Id
            INNER JOIN dbo.Categorias c ON sc.IdCategoria = c.Id";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorio();
            return await sqlConnection.QueryAsync<ProductoResponse>(sql);
        }

        public async Task<ProductoResponse?> Obtener(Guid id)
        {
            const string sql = @"
            SELECT 
                p.Id,
                p.Nombre,
                p.Descripcion,
                p.Precio,
                p.Stock,
                p.CodigoBarras,
                p.IdSubCategoria,
                sc.Nombre AS SubCategoria,
                c.Nombre AS Categoria
            FROM dbo.Producto p
            INNER JOIN dbo.SubCategorias sc ON p.IdSubCategoria = sc.Id
            INNER JOIN dbo.Categorias c ON sc.IdCategoria = c.Id
            WHERE p.Id = @Id";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorio();
            return await sqlConnection.QueryFirstOrDefaultAsync<ProductoResponse>(sql, new { Id = id });
        }

        public async Task<Guid> Agregar(ProductoRequest producto)
        {
            Guid id = Guid.NewGuid();

            const string sql = @"
            INSERT INTO dbo.Producto
            (
                Id,
                Nombre,
                Descripcion,
                Precio,
                Stock,
                CodigoBarras,
                IdSubCategoria
            )
            VALUES
            (
                @Id,
                @Nombre,
                @Descripcion,
                @Precio,
                @Stock,
                @CodigoBarras,
                @IdSubCategoria
            )";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorio();
            await sqlConnection.ExecuteAsync(sql, new
            {
                Id = id,
                producto.Nombre,
                producto.Descripcion,
                producto.Precio,
                producto.Stock,
                producto.CodigoBarras,
                producto.IdSubCategoria
            });

            return id;
        }

        public async Task<Guid> Editar(Guid id, ProductoRequest producto)
        {
            const string sql = @"
            UPDATE dbo.Producto
            SET
                Nombre = @Nombre,
                Descripcion = @Descripcion,
                Precio = @Precio,
                Stock = @Stock,
                CodigoBarras = @CodigoBarras,
                IdSubCategoria = @IdSubCategoria
            WHERE Id = @Id";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorio();
            await sqlConnection.ExecuteAsync(sql, new
            {
                Id = id,
                producto.Nombre,
                producto.Descripcion,
                producto.Precio,
                producto.Stock,
                producto.CodigoBarras,
                producto.IdSubCategoria
            });

            return id;
        }

        public async Task<Guid> Eliminar(Guid id)
        {
            const string sql = "DELETE FROM dbo.Producto WHERE Id = @Id";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorio();
            await sqlConnection.ExecuteAsync(sql, new { Id = id });

            return id;
        }
    }
}