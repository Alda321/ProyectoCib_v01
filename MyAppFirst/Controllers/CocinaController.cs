using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAppFirst.Data;
using MyAppFirst.Models;

namespace MyAppFirst.Controllers
{
    public class CocinaController : Controller
    {
        private readonly MyAppContext _context;

        public CocinaController(MyAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Plato)
                .Where(p => p.Estado != EstadoPedido.Servido && p.Estado != EstadoPedido.Cancelado)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> CambiarEstado(int id, EstadoPedido nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Estado = nuevoEstado;
            _context.Update(pedido);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
