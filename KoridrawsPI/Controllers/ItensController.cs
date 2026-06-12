using KoridrawsPI.Data;
using KoridrawsPI.Models;
using KoridrawsPI.Models.DTOs.Item;
using KoridrawsPI.Services;
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
        private readonly GoogleDriveService _driveService;

        public ItensController(Context context, GoogleDriveService driveService)
        {
            _context = context;
            _driveService = driveService;
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
        [Authorize(Roles = "Gerente")]
        [HttpPost("Post")]
        public async Task<ActionResult<Item>> PostItem([FromForm] ItemUploadDto itemDto)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var gerenteIdClaim = claimsIdentity?.FindFirst("UsuarioId")?.Value;

            var novoItem = new Item
            {
                Nome = itemDto.Nome,
                Preco = itemDto.Preco,
                Imagens = new List<Imagem>(),
                Estoque = itemDto.Estoque
            };

            if (itemDto.Imagens != null && itemDto.Imagens.Any())
            {
                foreach (var arquivo in itemDto.Imagens)
                {
                    if (arquivo.Length > 0)
                    {
                        var (url, caminhoCloud) = await _driveService.UploadImagemAsync(arquivo);

                        novoItem.Imagens.Add(new Imagem
                        {
                            Url = url,
                            CaminhoCloud = caminhoCloud
                        });
                    }
                }
            }

            _context.Itens.Add(novoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = novoItem.Id }, novoItem);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Itens
                .Include(i => i.Imagens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [Authorize(Roles = "Gerente")]
        [HttpPut("Put/{id}")]
        public async Task<IActionResult> PutItem(int id, [FromForm] ItemUpdateDto itemDto)
        {
            var item = await _context.Itens
                .Include(i => i.Imagens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            item.Nome = itemDto.Nome;
            item.Preco = itemDto.Preco;

            if (itemDto.ImagensParaRemover != null && itemDto.ImagensParaRemover.Any())
            {
                var imagensRemover = item.Imagens
                    .Where(img => itemDto.ImagensParaRemover.Contains(img.Id))
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

            if (itemDto.NovasImagens != null && itemDto.NovasImagens.Any())
            {
                foreach (var arquivo in itemDto.NovasImagens)
                {
                    if (arquivo.Length > 0)
                    {
                        var (url, caminhoCloud) = await _driveService.UploadImagemAsync(arquivo);

                        item.Imagens.Add(new Imagem
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
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Itens
                .Include(i => i.Imagens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            foreach (var img in item.Imagens)
            {
                if (!string.IsNullOrEmpty(img.CaminhoCloud))
                {
                    await _driveService.ExcluirImagemAsync(img.CaminhoCloud);
                }
            }

            _context.Itens.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Gerente")]
        [HttpPatch("{id}/estoque")]
        public async Task<IActionResult> AtualizarEstoque(int id, [FromForm] int novoEstoque)
        {
            if (novoEstoque < 0)
            {
                return BadRequest(new { Mensagem = "O estoque não pode ser negativo." });
            }

            var item = await _context.Itens.FindAsync(id);

            if (item == null)
            {
                return NotFound(new { Mensagem = "Item não encontrado." });
            }

            item.Estoque = novoEstoque;
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Estoque atualizado com sucesso.", NovoEstoque = item.Estoque });
        }

    }
}
