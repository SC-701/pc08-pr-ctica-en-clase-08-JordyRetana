using Abstracciones.DA;
using Abstracciones.Modelos;
using Dapper;
using System.Data.SqlClient;

namespace DA
{
    public class UsuarioDA : IUsuarioDA
    {
        private readonly IRepositorioDapper _repositorioDapper;

        public UsuarioDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
        }

        public async Task<Guid> CrearUsuario(UsuarioBase usuario)
        {
            Guid id = Guid.NewGuid();

            const string sql = @"
            INSERT INTO dbo.Usuarios
            (
                Id,
                NombreUsuario,
                PasswordHash,
                CorreoElectronico
            )
            VALUES
            (
                @Id,
                @NombreUsuario,
                @PasswordHash,
                @CorreoElectronico
            )";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorioDapper();

            await sqlConnection.ExecuteAsync(sql, new
            {
                Id = id,
                usuario.NombreUsuario,
                usuario.PasswordHash,
                usuario.CorreoElectronico
            });

            return id;
        }

        public async Task<Usuario?> ObtenerUsuario(UsuarioBase usuario)
        {
            const string sql = @"
            SELECT TOP 1
                Id,
                NombreUsuario,
                PasswordHash,
                CorreoElectronico
            FROM dbo.Usuarios
            WHERE CorreoElectronico = @CorreoElectronico";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorioDapper();

            return await sqlConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new
            {
                usuario.CorreoElectronico
            });
        }

        public async Task<IEnumerable<Perfil>> ObtenerPerfilesxUsuario(UsuarioBase usuario)
        {
            const string sql = @"
            SELECT
                p.Id,
                p.Nombre
            FROM dbo.Usuarios u
            INNER JOIN dbo.UsuariosPerfiles up ON u.Id = up.UsuarioId
            INNER JOIN dbo.Perfiles p ON up.PerfilId = p.Id
            WHERE u.CorreoElectronico = @CorreoElectronico";

            using SqlConnection sqlConnection = _repositorioDapper.ObtenerRepositorioDapper();

            return await sqlConnection.QueryAsync<Perfil>(sql, new
            {
                usuario.CorreoElectronico
            });
        }
    }
}