using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoridrawsPI.Data;

namespace KoridrawsPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly Context _context;

        public PerfilController(Context context)
        {
            _context = context;
        }

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetMeuPerfil()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            if (User.IsInRole("Cliente"))
            {
                var cliente = await _context.Clientes
                    .Include(c => c.ImagemPerfil)
                    .Include(c => c.Enderecos)
                        .ThenInclude(e => e.Cidade)
                            .ThenInclude(cid => cid.Estado)
                    .FirstOrDefaultAsync(c => c.Email == email);

                if (cliente == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    Tipo = "Cliente",
                    Email = email,
                    Dados = new
                    {
                        cliente.Id,
                        cliente.Nome,
                        Imagem = cliente.ImagemPerfil?.Url,
                        Enderecos = cliente.Enderecos.Select(e => new
                        {
                            e.Id,
                            e.Rua,
                            e.Numero,
                            e.Bairro,
                            e.Cep,
                            e.Complemento,
                            Cidade = e.Cidade?.Descricao,
                            Estado = e.Cidade?.Estado?.Sigla
                        })
                    }
                });
            }

            if (User.IsInRole("Gerente"))
            {
                var gerente = await _context.Gerentes.FirstOrDefaultAsync(g => g.Email == email);

                if (gerente == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    Tipo = "Gerente",
                    Email = email,
                    Dados = new
                    {
                        gerente.Id,
                        gerente.Nome,
                        gerente.Setor
                    }
                });
            }

            return BadRequest();
        }
    }
}
