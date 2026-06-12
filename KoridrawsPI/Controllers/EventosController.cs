using KoridrawsPI.Data;
using KoridrawsPI.Data;
using KoridrawsPI.Models;
using KoridrawsPI.Models.DTOs.Endereco;
using KoridrawsPI.Models.DTOs.Evento;
using KoridrawsPI.Models.DTOs.ImagemDtos;
using KoridrawsPI.Services;
using KoridrawsPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<GetEventosDto>>> GetEventos()
        {
            var eventos = await _context.Eventos
                .Include(e => e.Endereco)
                    .ThenInclude(ed => ed!.Cidade).ThenInclude(cid => cid.Estado)
                .Include(e => e.Imagens)
                .ToListAsync();

            var resultado = eventos.Select(e => new GetEventosDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Descricao = e.Descricao,
                Data = e.Data,
                EnderecoId = e.EnderecoId,
                Endereco = e.Endereco != null ? new EnderecoEventoDto
                {
                    Id = e.Endereco.Id,
                    Rua = e.Endereco.Rua,
                    Numero = e.Endereco.Numero,
                    Bairro = e.Endereco.Bairro,
                    Cep = e.Endereco.Cep,
                    Complemento = e.Endereco.Complemento,
                    CidadeNome = e.Endereco.Cidade != null ? e.Endereco.Cidade.Descricao + " | " + e.Endereco.Cidade.Estado.Sigla : string.Empty
                } : null,
                Imagem = e.Imagens.FirstOrDefault() != null ? new Models.DTOs.ImagemDtos.ImagemRelationDto 
                { CaminhoCloud = e.Imagens.FirstOrDefault().CaminhoCloud, Url = e.Imagens.FirstOrDefault().Url, Id = e.Imagens.FirstOrDefault().Id  } : null,
            }).ToList();

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetEventoDto>> GetEvento(int id)
        {
            var evento = await _context.Eventos
                .Include(e => e.Endereco)
                    .ThenInclude(ed => ed!.Cidade).ThenInclude(cid => cid.Estado)
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            var dto = new GetEventoDto
            {
                Id = evento.Id,
                Nome = evento.Nome,
                Descricao = evento.Descricao,
                Data = evento.Data,
                EnderecoId = evento.EnderecoId,
                Endereco = evento.Endereco != null ? new EnderecoEventoDto
                {
                    Id = evento.Endereco.Id,
                    Rua = evento.Endereco.Rua,
                    Numero = evento.Endereco.Numero,
                    Bairro = evento.Endereco.Bairro,
                    Cep = evento.Endereco.Cep,
                    Complemento = evento.Endereco.Complemento,
                    CidadeNome = evento.Endereco.Cidade != null ? evento.Endereco.Cidade.Descricao + " | " + evento.Endereco.Cidade.Estado.Sigla : string.Empty
                } : null,
                Imagens = evento.Imagens.Select(i => new ImagemRelationDto { CaminhoCloud = i.CaminhoCloud, Id = i.Id, Url = i.Url }).ToList(),
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPost("Post")]
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
        [HttpPut("Put/{id}")]
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
        [HttpDelete("Delete/{id}")]
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