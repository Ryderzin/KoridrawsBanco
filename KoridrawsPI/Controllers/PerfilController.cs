using KoridrawsPI.Data;
using KoridrawsPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            // Buscamos na tabela base 'Usuarios' para pegar qualquer perfil (Cliente ou Gerente)
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            // Se for Cliente, buscamos os pedidos e endereços
            if (usuario is Cliente)
            {
                var pedidos = await _context.Pedidos
                    .Include(p => p.Endereco)
                    .Include(p => p.ItensPedido)
                        .ThenInclude(ip => ip.Item)
                    .Where(p => p.ClienteId == usuarioId)
                    .OrderByDescending(p => p.DataEmissao)
                    .ToListAsync();

                var enderecos = await _context.Enderecos
                    .Include(e => e.Cidade)
                    .Where(e => e.ClienteId == usuarioId)
                    .ToListAsync();

                return Ok(new { Perfil = usuario, Enderecos = enderecos, Pedidos = pedidos });
            }

            // Se for apenas Gerente, retornamos os dados do perfil
            return Ok(new { Perfil = usuario });
        }
    }
}