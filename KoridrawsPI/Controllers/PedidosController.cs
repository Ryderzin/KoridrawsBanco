using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoridrawsPI.Data;
using KoridrawsPI.Models;
using KoridrawsPI.Models.DTOs;

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
                    .Include(p => p.Endereco)
                    .Include(p => p.ItensPedido)
                        .ThenInclude(ip => ip.Item)
                    .ToListAsync();
            }

            var claimId = User.FindFirst("UsuarioId");
            if (claimId == null || !int.TryParse(claimId.Value, out int clienteId))
            {
                return Unauthorized();
            }

            return await _context.Pedidos
                .Include(p => p.Endereco)
                .Include(p => p.ItensPedido)
                    .ThenInclude(ip => ip.Item)
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Endereco)
                .Include(p => p.ItensPedido)
                    .ThenInclude(ip => ip.Item)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            var claimId = User.FindFirst("UsuarioId");
            if (claimId == null || !int.TryParse(claimId.Value, out int clienteId))
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Gerente") && pedido.ClienteId != clienteId)
            {
                return Forbid();
            }

            return pedido;
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido([FromBody] PedidoCriacaoDto dto)
        {
            var claimId = User.FindFirst("UsuarioId");
            if (claimId == null || !int.TryParse(claimId.Value, out int clienteId))
            {
                return Unauthorized();
            }

            var enderecoValido = await _context.Enderecos
                .AnyAsync(e => e.Id == dto.EnderecoId && e.ClienteId == clienteId);

            if (!enderecoValido) return BadRequest("Endereço inválido ou não pertence a este cliente.");

            if (dto.Itens == null || !dto.Itens.Any()) return BadRequest("O pedido deve conter itens.");

            if (dto.Frete == null) return BadRequest("É necessário selecionar uma opção de frete.");

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                EnderecoId = dto.EnderecoId,
                Status = StatusPedido.Criado,
                Pagamento = dto.Pagamento,
                DataEmissao = DateTime.UtcNow,
                ItensPedido = new List<PedidoItem>(),
                Frete = new Frete
                {
                    Servico = dto.Frete.Servico,
                    Valor = dto.Frete.Valor,
                    PrazoDias = dto.Frete.PrazoDias
                }
            };

            decimal valorTotalItens = 0;

            foreach (var itemDto in dto.Itens)
            {
                if (itemDto.Quantidade <= 0) return BadRequest("A quantidade deve ser maior que zero.");

                var itemBanco = await _context.Itens.FindAsync(itemDto.ItemId);
                if (itemBanco == null) return BadRequest($"Item com ID {itemDto.ItemId} não encontrado.");

                if (itemBanco.Estoque < itemDto.Quantidade)
                {
                    return BadRequest($"Estoque insuficiente para o item {itemBanco.Nome}. Disponível: {itemBanco.Estoque}.");
                }

                itemBanco.Estoque -= itemDto.Quantidade;

                var pedidoItem = new PedidoItem
                {
                    ItemId = itemBanco.Id,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = itemBanco.Preco
                };

                valorTotalItens += pedidoItem.Quantidade * pedidoItem.PrecoUnitario;
                pedido.ItensPedido.Add(pedidoItem);
            }

            pedido.ValorTotal = valorTotalItens + dto.Frete.Valor;

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPatch("status")]
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