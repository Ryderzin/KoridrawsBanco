using KoridrawsPI.Data;
using KoridrawsPI.Models;
using KoridrawsPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static KoridrawsPI.Models.DTOs.Ibge;

namespace SuaLojaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnderecosController : ControllerBase
    {
        private readonly Context _context;

        public EnderecosController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Endereco>>> GetEnderecos()
        {
            return await _context.Enderecos
                .Include(e => e.Cidade)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Endereco>> GetEndereco(int id)
        {
            var endereco = await _context.Enderecos
                .Include(e => e.Cidade)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (endereco == null) return NotFound();

            return endereco;
        }

        [Authorize]
        [HttpPost("Post")]
        public async Task<ActionResult<Endereco>> PostEndereco([FromForm] EnderecoDto enderecoDto)
        {
            var claimId = User.FindFirst("UsuarioId");

            if (claimId == null)
            {
                var claimsDisponiveis = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                return StatusCode(401, new { Erro = "ID não encontrado no token.", Claims = claimsDisponiveis });
            }

            if (!int.TryParse(claimId.Value, out int clienteId))
            {
                return BadRequest(new { Erro = "ID inválido no token.", Valor = claimId.Value });
            }

            var novoEndereco = new Endereco
            {
                Rua = enderecoDto.Rua,
                Numero = enderecoDto.Numero,
                Bairro = enderecoDto.Bairro,
                Cep = enderecoDto.Cep,
                Complemento = enderecoDto.Complemento,
                CidadeId = enderecoDto.CidadeId,
                ClienteId = clienteId
            };

            _context.Enderecos.Add(novoEndereco);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEndereco), new { id = novoEndereco.Id }, novoEndereco);
        }

        [Authorize]
        [HttpPut("Put/{id}")]
        public async Task<IActionResult> PutEndereco(int id, [FromForm] EnderecoDto enderecoDto)
        {
            var endereco = await _context.Enderecos.FindAsync(id);

            if (endereco == null) return NotFound();

            endereco.Rua = enderecoDto.Rua;
            endereco.Numero = enderecoDto.Numero;
            endereco.Bairro = enderecoDto.Bairro;
            endereco.Cep = enderecoDto.Cep;
            endereco.Complemento = enderecoDto.Complemento;
            endereco.CidadeId = enderecoDto.CidadeId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            var endereco = await _context.Enderecos.FindAsync(id);

            if (endereco == null) return NotFound();

            _context.Enderecos.Remove(endereco);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("estados")]
        public async Task<ActionResult<IEnumerable<EstadoDropdownDto>>> GetEstados()
        {
            return await _context.Estados
                .OrderBy(e => e.Descricao)
                .Select(e => new EstadoDropdownDto
                {
                    Id = e.Id,
                    Nome = e.Descricao,
                    Sigla = e.Sigla
                })
                .ToListAsync();
        }

        [HttpGet("estados/{estadoId}/cidades")]
        public async Task<ActionResult<IEnumerable<CidadeDropdownDto>>> GetCidadesPorEstado(int estadoId)
        {
            var cidades = await _context.Cidades
                .Where(c => c.EstadoId == estadoId)
                .OrderBy(c => c.Descricao)
                .Select(c => new CidadeDropdownDto
                {
                    Id = c.Id,
                    Nome = c.Descricao
                })
                .ToListAsync();

            return cidades;
        }
    }
}