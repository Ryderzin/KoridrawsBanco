using KoridrawsPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoridrawsPI.Data;
using KoridrawsPI.Models;
using System.Security.Claims;

namespace KoridrawsPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private readonly Context _context;

        public EventosController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventos()
        {
            return await _context.Eventos
                .Include(e => e.Endereco)
                    .ThenInclude(ed => ed!.Cidade)
                .Include(e => e.Imagens)
                .ToListAsync();
        }

        [Authorize(Roles = "Gerente")]
        [HttpPost]
        public async Task<ActionResult<Evento>> PostEvento(Evento evento)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var gerenteId = claimsIdentity?.FindFirst("UsuarioId")?.Value;

            if (gerenteId != null)
            {
                evento.GerenteId = int.Parse(gerenteId);
            }

            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventos), new { id = evento.Id }, evento);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, Evento evento)
        {
            if (id != evento.Id) return BadRequest();

            _context.Entry(evento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Gerente")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null) return NotFound();

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
