using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoridrawsPI.Data;
using KoridrawsPI.Models;
using System.Security.Claims;
namespace KoridrawsPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly Context _context;

        public PedidosController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            if (User.IsInRole("Gerente"))
            {
                return await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Itens)
                    .ToListAsync();
            }

            var emailUsuario = User.Identity?.Name;
            return await _context.Pedidos
                .Include(p => p.Itens)
                .Where(p => p.Cliente!.Email == emailUsuario)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            if (!User.IsInRole("Gerente") && pedido.Cliente?.Email != User.Identity?.Name)
            {
                return Forbid();
            }

            return pedido;
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido([FromForm] Pedido pedido)
        {
            var emailUsuario = User.Identity?.Name;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == emailUsuario);

            if (cliente == null) return BadRequest("Perfil de cliente não encontrado.");

            pedido.ClienteId = cliente.Id;
            pedido.Cliente = null;
            pedido.DataEmissao = DateTime.UtcNow;
            pedido.Status = StatusPedido.Criado;

            if (pedido.Itens != null && pedido.Itens.Any())
            {
                var itemIds = pedido.Itens.Select(i => i.Id).ToList();
                var itensDoBanco = await _context.Itens.Where(i => itemIds.Contains(i.Id)).ToListAsync();
                pedido.Itens = itensDoBanco;
            }

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus([FromForm] int id, [FromForm] StatusPedido novoStatus)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Status = novoStatus;
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Status atualizado.", Status = pedido.Status.ToString() });
        }

        [Authorize(Roles = "Gerente")]
        [HttpDelete]
        public async Task<IActionResult> DeletePedido([FromForm] int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
