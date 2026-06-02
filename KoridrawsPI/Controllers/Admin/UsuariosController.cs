using KoridrawsPI.Data;
using KoridrawsPI.Filters;
using KoridrawsPI.Models.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoridrawsPI.Filters;
using KoridrawsPI.Models;

namespace KoridrawsPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AdminKey]
    public class UsuariosController : ControllerBase
    {
        private readonly Context _context;

        public UsuariosController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioConsultaDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuarioConsultaDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Senha = u.SenhaHash
                })
                .ToListAsync();

            return Ok(usuarios);
        }
    }
}