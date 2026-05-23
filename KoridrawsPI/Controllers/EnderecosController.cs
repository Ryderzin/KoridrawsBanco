using KoridrawsPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoridrawsPI.Data;
using KoridrawsPI.Models;

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

        [Authorize(Roles = "Gerente")]
        [HttpPost]
        public async Task<ActionResult<Endereco>> PostEndereco([FromForm] EnderecoDto enderecoDto)
        {
            var novoEndereco = new Endereco
            {
                Rua = enderecoDto.Rua,
                Numero = enderecoDto.Numero,
                Bairro = enderecoDto.Bairro,
                Cep = enderecoDto.Cep,
                Complemento = enderecoDto.Complemento,
                CidadeId = enderecoDto.CidadeId
            };

            _context.Enderecos.Add(novoEndereco);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEndereco), new { id = novoEndereco.Id }, novoEndereco);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPut("{id}")]
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

        [Authorize(Roles = "Gerente")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            var endereco = await _context.Enderecos.FindAsync(id);

            if (endereco == null) return NotFound();

            _context.Enderecos.Remove(endereco);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}