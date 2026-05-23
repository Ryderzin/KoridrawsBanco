using KoridrawsPI.Data;
using KoridrawsPI.Models;
using KoridrawsPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoridrawsPI.Data;
using KoridrawsPI.Services;
using System.Security.Claims;

namespace KoridrawsPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private readonly Context _context;
        private readonly GoogleDriveService _driveService;

        public EventosController(Context context, GoogleDriveService driveService)
        {
            _context = context;
            _driveService = driveService;
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
        public async Task<ActionResult<Evento>> PostEvento([FromForm] EventoUploadDto eventoDto)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var gerenteIdClaim = claimsIdentity?.FindFirst("UsuarioId")?.Value;

            var novoEvento = new Evento
            {
                Nome = eventoDto.Nome,
                Descricao = eventoDto.Descricao,
                Data = eventoDto.Data,
                EnderecoId = eventoDto.EnderecoId,
                GerenteId = gerenteIdClaim != null ? int.Parse(gerenteIdClaim) : null,
                Imagens = new List<Imagem>()
            };

            if (eventoDto.Imagens != null && eventoDto.Imagens.Any())
            {
                foreach (var arquivo in eventoDto.Imagens)
                {
                    if (arquivo.Length > 0)
                    {
                        var (url, caminhoCloud) = await _driveService.UploadImagemAsync(arquivo);

                        novoEvento.Imagens.Add(new Imagem
                        {
                            Url = url,
                            CaminhoCloud = caminhoCloud
                        });
                    }
                }
            }

            _context.Eventos.Add(novoEvento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventos), new { id = novoEvento.Id }, novoEvento);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, [FromForm] EventoUpdateDto eventoDto)
        {
            var evento = await _context.Eventos
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null) return NotFound();

            evento.Nome = eventoDto.Nome;
            evento.Descricao = eventoDto.Descricao;
            evento.Data = eventoDto.Data;
            evento.EnderecoId = eventoDto.EnderecoId;

            if (eventoDto.ImagensParaRemover != null && eventoDto.ImagensParaRemover.Any())
            {
                var imagensRemover = evento.Imagens
                    .Where(img => eventoDto.ImagensParaRemover.Contains(img.Id))
                    .ToList();

                foreach (var img in imagensRemover)
                {
                    if (!string.IsNullOrEmpty(img.CaminhoCloud))
                    {
                        await _driveService.ExcluirImagemAsync(img.CaminhoCloud);
                    }
                    _context.Imagens.Remove(img);
                }
            }

            if (eventoDto.NovasImagens != null && eventoDto.NovasImagens.Any())
            {
                foreach (var arquivo in eventoDto.NovasImagens)
                {
                    if (arquivo.Length > 0)
                    {
                        var (url, caminhoCloud) = await _driveService.UploadImagemAsync(arquivo);

                        evento.Imagens.Add(new Imagem
                        {
                            Url = url,
                            CaminhoCloud = caminhoCloud
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Gerente")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var evento = await _context.Eventos
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null) return NotFound();

            foreach (var img in evento.Imagens)
            {
                if (!string.IsNullOrEmpty(img.CaminhoCloud))
                {
                    await _driveService.ExcluirImagemAsync(img.CaminhoCloud);
                }
            }

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}