using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.Server.Data;
using Modelo.Server.Models;

namespace Modelo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoDetallesController : ControllerBase
    {
        private readonly MyAppContext _context;

        public PedidoDetallesController(MyAppContext context)
        {
            _context = context;
        }

        // GET: api/PedidoDetalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDetalle>>> GetPedidoDetalles()
        {
            return await _context.PedidoDetalles
                                 .Include(pd => pd.Pedido)
                                 .Include(pd => pd.Plato)
                                 .ToListAsync();
        }

        // GET: api/PedidoDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDetalle>> GetPedidoDetalle(int id)
        {
            var pedidoDetalle = await _context.PedidoDetalles
                                              .Include(pd => pd.Pedido)
                                              .Include(pd => pd.Plato)
                                              .FirstOrDefaultAsync(pd => pd.Id == id);

            if (pedidoDetalle == null)
            {
                return NotFound();
            }

            return pedidoDetalle;
        }

        // POST: api/PedidoDetalles
        [HttpPost]
        public async Task<ActionResult<PedidoDetalle>> PostPedidoDetalle(PedidoDetalle pedidoDetalle)
        {
            _context.PedidoDetalles.Add(pedidoDetalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedidoDetalle), new { id = pedidoDetalle.Id }, pedidoDetalle);
        }

        // PUT: api/PedidoDetalles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoDetalle(int id, PedidoDetalle pedidoDetalle)
        {
            if (id != pedidoDetalle.Id)
            {
                return BadRequest();
            }

            _context.Entry(pedidoDetalle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PedidoDetalles.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/PedidoDetalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedidoDetalle(int id)
        {
            var pedidoDetalle = await _context.PedidoDetalles.FindAsync(id);
            if (pedidoDetalle == null)
            {
                return NotFound();
            }

            _context.PedidoDetalles.Remove(pedidoDetalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
