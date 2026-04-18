using KoridrawsPI.Data;
using KoridrawsPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KoridrawsPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensController : ControllerBase
    {
        private readonly Context _context;

        public ItensController(Context context)
        {
            _context = context;
        }

        // GET: api/Itens
        // Rota PÚBLICA: Não tem [Authorize], então qualquer um acessa
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItens()
        {
            // Opcional: Incluir as imagens do item na listagem
            return await _context.Itens.Include(i => i.Imagens).ToListAsync();
        }

        // GET: api/Itens/5
        // Rota PÚBLICA
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Itens
                .Include(i => i.Imagens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            return item;
        }

        // POST: api/Itens
        // PROTEGIDO: Exige um Token JWT válido ONDE a Role seja "Gerente"
        [Authorize(Roles = "Gerente")]
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem([FromForm]Item item)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var gerenteIdClaim = claimsIdentity?.FindFirst("UsuarioId")?.Value;

            if (gerenteIdClaim != null)
            {
                item.GerenteId = int.Parse(gerenteIdClaim);
            }

            _context.Itens.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        // PUT: api/Itens/5
        // PROTEGIDO: Apenas Gerentes
        [Authorize(Roles = "Gerente")]
        [HttpPut]
        public async Task<IActionResult> PutItem([FromForm]int id, [FromForm] Item item)
        {
            if (id != item.Id) return BadRequest();

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Itens/5
        // PROTEGIDO: Apenas Gerentes
        [Authorize(Roles = "Gerente")]
        [HttpDelete]
        public async Task<IActionResult> DeleteItem([FromForm]int id)
        {
            var item = await _context.Itens.FindAsync(id);
            if (item == null) return NotFound();

            _context.Itens.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(int id)
        {
            return _context.Itens.Any(e => e.Id == id);
        }
    }
}
