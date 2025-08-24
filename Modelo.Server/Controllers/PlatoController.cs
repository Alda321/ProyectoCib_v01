using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.Server.Data;
using Modelo.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Modelo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatoController : ControllerBase
    {
        private readonly MyAppContext _context;

        public PlatoController(MyAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plato>>> GetPlatos()
        {
            return await _context.Platos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Plato>> GetPlato(int id)
        {
            var plato = await _context.Platos.FindAsync(id);
            if (plato == null) return NotFound();
            return plato;
        }

        [HttpPost]
        public async Task<ActionResult<Plato>> PostPlato([FromBody] PlatoDTO platoDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var plato = new Plato
            {
                Nombre = platoDto.Nombre,
                Precio = platoDto.Precio,
                Activo = platoDto.Activo
            };

            _context.Platos.Add(plato);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlato), new { id = plato.Id }, plato);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlato(int id, [FromBody] PlatoDTO platoDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var plato = await _context.Platos.FindAsync(id);
            if (plato == null) return NotFound();

            plato.Nombre = platoDto.Nombre;
            plato.Precio = platoDto.Precio;
            plato.Activo = platoDto.Activo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Platos.Any(p => p.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlato(int id)
        {
            var plato = await _context.Platos
                .Include(p => p.PedidoDetalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plato == null) return NotFound();

            if (plato.PedidoDetalles != null && plato.PedidoDetalles.Any())
                return BadRequest("No se puede eliminar un plato que ya ha sido pedido.");

            _context.Platos.Remove(plato);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Plato/activar/5
        [HttpPatch("activar/{id}")]
        public async Task<IActionResult> ActivarPlato(int id)
        {
            var plato = await _context.Platos.FindAsync(id);
            if (plato == null) return NotFound();

            plato.Activo = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Plato/desactivar/5
        [HttpPatch("desactivar/{id}")]
        public async Task<IActionResult> DesactivarPlato(int id)
        {
            var plato = await _context.Platos.FindAsync(id);
            if (plato == null) return NotFound();

            plato.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
