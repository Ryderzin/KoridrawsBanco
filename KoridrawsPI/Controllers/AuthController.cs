using KoridrawsPI.Data;
using KoridrawsPI.Models;
using KoridrawsPI.Models.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static KoridrawsPI.Models.DTOs.AuthDtos;

namespace KoridrawsPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Context _context;
        private readonly IConfiguration _configuration;

        public AuthController(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto modelo)
        {
            // 1. Verifica se o e-mail já existe na tabela base (Usuarios)
            if (await _context.Usuarios.AnyAsync(u => u.Email == modelo.Email))
            {
                return BadRequest(new { Mensagem = "E-mail já está em uso." });
            }

            // 2. Criptografa a senha usando BCrypt
            var senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(modelo.Senha);

            // 3. Instancia a classe correta dependendo do perfil
            Usuario novoUsuario;

            if (modelo.Perfil.Equals("Gerente", StringComparison.OrdinalIgnoreCase))
            {
                novoUsuario = new Gerente
                {
                    Nome = modelo.Nome,
                    Email = modelo.Email,
                    SenhaHash = senhaCriptografada,
                    Papel = "Gerente",
                    Setor = "Geral" // Ou pegar do DTO
                };
            }
            else // Por padrão, cria Cliente
            {
                novoUsuario = new Cliente
                {
                    Nome = modelo.Nome,
                    Email = modelo.Email,
                    SenhaHash = senhaCriptografada,
                    Papel = "Cliente"
                };
            }

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Usuário registrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto modelo)
        {
            // 1. Busca qualquer usuário (Cliente ou Gerente) pelo e-mail
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == modelo.Email);

            // 2. Verifica se encontrou e se a senha bate com o Hash
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(modelo.Senha, usuario.SenhaHash))
            {
                return Unauthorized(new { Mensagem = "Email ou senha incorretos." });
            }

            // 3. Monta as Claims para o Token
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Papel), // É aqui que o [Authorize(Roles="...")] funciona!
                new Claim("UsuarioId", usuario.Id.ToString()), // Dica: Guardar o ID facilita buscas no banco depois
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = GerarToken(authClaims);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracao = token.ValidTo,
                Papel = usuario.Papel
            });
        }

        private JwtSecurityToken GerarToken(List<Claim> authClaims)
        {
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}
