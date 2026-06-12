using KoridrawsPI.Data;
using KoridrawsPI.Models;
using KoridrawsPI.Models.Abstract;
using KoridrawsPI.Models.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static KoridrawsPI.Models.DTOs.Auth.AuthResponseDtos;
using static KoridrawsPI.Models.DTOs.Auth.PasswordResetDto;

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

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromForm] LoginDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            {
                return Unauthorized("Credenciais inválidas.");
            }

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.Role, usuario.Papel),
        new Claim("UsuarioId", usuario.Id.ToString())
    };

            var token = GerarToken(authClaims);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new LoginResponseDto
            {
                Token = tokenString,
                Expiracao = token.ValidTo,
                Usuario = new UsuarioResponseDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Papel = usuario.Papel
                }
            };

            return Ok(response);
        }

        [HttpPost("registro")]
        public async Task<ActionResult<RegistroResponseDto>> Registrar([FromForm] RegisterDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("E-mail já está em uso.");
            }

            var novoCliente = new Cliente
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Papel = dto.Papel
            };

            _context.Clientes.Add(novoCliente);
            await _context.SaveChangesAsync();

            var response = new RegistroResponseDto
            {
                Mensagem = "Conta criada com sucesso.",
                Usuario = new UsuarioResponseDto
                {
                    Id = novoCliente.Id,
                    Nome = novoCliente.Nome,
                    Email = novoCliente.Email,
                    Papel = novoCliente.Papel
                }
            };

            return CreatedAtAction(nameof(Login), new { email = novoCliente.Email }, response);
        }

        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha([FromForm] EsqueciSenhaDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Por segurança, retornamos Ok mesmo se o e-mail não existir, 
            // para evitar que hackers usem a API para descobrir e-mails cadastrados.
            if (usuario == null)
            {
                return Ok(new { Mensagem = "Se o e-mail estiver registado, um código de recuperação será enviado." });
            }

            // Gerar um código de 6 dígitos
            string token = new Random().Next(100000, 999999).ToString();

            usuario.ResetSenhaToken = token;
            usuario.ResetSenhaExpiracao = DateTime.UtcNow.AddMinutes(30); // O código expira em 30 minutos

            await _context.SaveChangesAsync();

            // Simulação do envio de E-mail (Aparecerá no terminal onde a API roda)
            Console.WriteLine("\n================ SIMULAÇÃO DE E-MAIL ================");
            Console.WriteLine($"Para: {usuario.Email}");
            Console.WriteLine($"Assunto: Recuperação de Senha");
            Console.WriteLine($"Seu código de verificação é: {token}");
            Console.WriteLine("=====================================================\n");

            return Ok(new { Mensagem = "Se o e-mail estiver registado, um código de recuperação será enviado." });
        }

        [HttpPost("resetar-senha")]
        public async Task<IActionResult> ResetarSenha([FromForm] ResetarSenhaDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null || usuario.ResetSenhaToken != dto.Token)
            {
                return BadRequest("Token inválido.");
            }

            if (usuario.ResetSenhaExpiracao < DateTime.UtcNow)
            {
                return BadRequest("O token expirou. Por favor, solicite um novo código.");
            }

            // Atualiza a senha (Substitua por sua lógica de hash se for diferente)
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);

            // Invalida o token para que não possa ser usado novamente
            usuario.ResetSenhaToken = null;
            usuario.ResetSenhaExpiracao = null;

            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Senha alterada com sucesso! Já pode fazer login com a nova senha." });
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
