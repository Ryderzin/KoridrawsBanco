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
    public class PerfilController : ControllerBase
    {
        private readonly Context _context;

        public PerfilController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PerfilCompletoDto>> GetProfile()
        {
            var claimId = User.FindFirst("UsuarioId");

            if (claimId == null || !int.TryParse(claimId.Value, out int usuarioId))
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            var respostaDto = new PerfilCompletoDto
            {
                Perfil = new PerfilUsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Papel = usuario.Papel
                }
            };

            if (usuario is Cliente)
            {
                var enderecos = await _context.Enderecos
                    .Include(e => e.Cidade)
                    .Where(e => e.ClienteId == usuarioId)
                    .ToListAsync();

                respostaDto.Enderecos = enderecos.Select(e => new PerfilEnderecoDto
                {
                    Id = e.Id,
                    Rua = e.Rua,
                    Bairro = e.Bairro,
                    Cep = e.Cep,
                    Complemento = e.Complemento,
                    Numero = e.Numero,
                    Cidade = e.Cidade
                }).ToList();

                var pedidos = await _context.Pedidos
                    .Include(p => p.Endereco)
                    .Include(p => p.Frete)
                    .Include(p => p.ItensPedido)
                        .ThenInclude(ip => ip.Item)
                    .Where(p => p.ClienteId == usuarioId)
                    .OrderByDescending(p => p.DataEmissao)
                    .ToListAsync();

                respostaDto.Pedidos = pedidos.Select(p => new PerfilPedidoDto
                {
                    Id = p.Id,
                    DataEmissao = p.DataEmissao,
                    ValorTotal = p.ValorTotal,
                    Status = p.Status.ToString(),
                    Pagamento = p.Pagamento.ToString(),
                    EnderecoEntregaResumido = p.Endereco != null ? $"{p.Endereco.Rua}, {p.Endereco.Numero}" : "Endereço removido",
                    Itens = p.ItensPedido.Select(ip => new PerfilPedidoItemDto
                    {
                        NomeProduto = ip.Item != null ? ip.Item.Nome : "Produto indisponível",
                        Quantidade = ip.Quantidade,
                        PrecoUnitario = ip.PrecoUnitario
                    }).ToList(),
                    Frete = p.Frete != null ? new PerfilPedidoFreteDto
                    {
                        Servico = p.Frete.Servico,
                        Valor = p.Frete.Valor,
                        PrazoDias = p.Frete.PrazoDias
                    } : new PerfilPedidoFreteDto()
                }).ToList();
            }

            return Ok(respostaDto);
        }
    }
}