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

        [HttpGet]
        public async Task<ActionResult> GetProfile()
        {
            var claimId = User.FindFirst("UsuarioId");

            if (claimId == null || !int.TryParse(claimId.Value, out int usuarioId))
            {
                return Unauthorized();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == usuarioId);

            if (cliente == null)
            {
                return NotFound();
            }

            var enderecos = await _context.Enderecos
                .Include(e => e.Cidade).ThenInclude(e => e.Estado)
                .Where(e => e.ClienteId == usuarioId)
                .ToListAsync();

            var pedidos = await _context.Pedidos
                .Include(p => p.Endereco)
                .Include(p => p.ItensPedido)
                    .ThenInclude(ip => ip.Item)
                .Where(p => p.ClienteId == usuarioId)
                .OrderByDescending(p => p.DataEmissao)
                .ToListAsync();

            return Ok(new
            {
                Perfil = new
                {
                    cliente.Id,
                    cliente.Nome,
                    cliente.Email,
                    cliente.Papel
                },
                Enderecos = enderecos,
                Pedidos = pedidos
            });
        }
    }
}