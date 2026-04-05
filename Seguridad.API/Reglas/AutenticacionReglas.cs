using Abstracciones.DA;
using Abstracciones.Modelos;
using Abstracciones.Reglas;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Reglas
{
    public class AutenticacionReglas : IAutenticacionBC
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioDA _usuarioDA;
        private Usuario? _usuario;

        public AutenticacionReglas(IConfiguration configuration, IUsuarioDA usuarioDA)
        {
            _configuration = configuration;
            _usuarioDA = usuarioDA;
        }

        public async Task<Token> LoginAync(LoginBase login)
        {
            Token respuestaToken = new Token
            {
                AccessToken = string.Empty,
                ValidacionExitosa = false
            };

            _usuario = await _usuarioDA.ObtenerUsuario(new UsuarioBase
            {
                CorreoElectronico = login.CorreoElectronico,
                NombreUsuario = login.NombreUsuario ?? string.Empty,
                PasswordHash = string.Empty
            });

            if (_usuario == null)
            {
                throw new Exception("No se encontró el usuario.");
            }

            if (login.PasswordHash != _usuario.PasswordHash)
            {
                throw new Exception("Credenciales inválidas.");
            }

            TokenConfiguracion? tokenConfiguracion = _configuration.GetSection("Token").Get<TokenConfiguracion>();

            if (tokenConfiguracion == null)
            {
                throw new Exception("No se encontró la configuración Token.");
            }

            JwtSecurityToken token = await GenerarTokenJWT(tokenConfiguracion);
            respuestaToken.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            respuestaToken.ValidacionExitosa = true;

            return respuestaToken;
        }

        private Task<JwtSecurityToken> GenerarTokenJWT(TokenConfiguracion tokenConfiguracion)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguracion.key));
            List<Claim> claims = GenerarClaims();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: tokenConfiguracion.Issuer,
                audience: tokenConfiguracion.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(tokenConfiguracion.ExpireMinutes),
                signingCredentials: credentials
            );

            return Task.FromResult(token);
        }

        private List<Claim> GenerarClaims()
        {
            List<Claim> claims = new();

            if (_usuario != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, _usuario.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, _usuario.NombreUsuario ?? string.Empty));
                claims.Add(new Claim(ClaimTypes.Email, _usuario.CorreoElectronico ?? string.Empty));
            }

            return claims;
        }
    }
}