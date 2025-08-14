using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAppFirst.Data;
using MyAppFirst.Models;

namespace MyAppFirst.Controllers
{
    public class PlatosController : Controller
    {
        private readonly MyAppContext _context;

        public PlatosController(MyAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Platos.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plato plato)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plato);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var plato = await _context.Platos.FindAsync(id);
            if (plato == null) return NotFound();
            return View(plato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Plato plato)
        {
            if (id != plato.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(plato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plato);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var plato = await _context.Platos.FindAsync(id);
            if (plato == null) return NotFound();
            return View(plato);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plato = await _context.Platos
                .Include(p => p.PedidoDetalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plato == null)
                return NotFound();

            if (plato.PedidoDetalles.Any())
            {
                TempData["Error"] = "No se puede eliminar un plato que ya ha sido pedido.";
                return RedirectToAction(nameof(Index));
            }

            _context.Platos.Remove(plato);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}