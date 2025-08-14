using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAppFirst.Data;
using MyAppFirst.Models;

namespace MyAppFirst.Controllers
{
    public class PedidosController : Controller
    {
        private readonly MyAppContext _context;

        public PedidosController(MyAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Plato)
                .ToListAsync();
            return View(pedidos);
        }

        public IActionResult Create()
        {
            ViewBag.Platos = _context.Platos.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pedido pedido, int[] platoIds, int[] cantidades)
        {
            var mozoId = HttpContext.Session.GetInt32("UserId");
            if (mozoId.HasValue) pedido.MozoId = mozoId.Value;

            if (ModelState.IsValid && platoIds.Length == cantidades.Length)
            {
                for (int i = 0; i < platoIds.Length; i++)
                {
                    if (cantidades[i] > 0)
                    {
                        pedido.Detalles.Add(new PedidoDetalle { PlatoId = platoIds[i], Cantidad = cantidades[i] });
                    }
                }
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Platos = _context.Platos.ToList();
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View(pedido);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Plato)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null || pedido.Estado != EstadoPedido.Pendiente)
                return NotFound();

            ViewBag.Platos = _context.Platos.ToList();
            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string NumeroMesa, int[] platoIds, int[] cantidades)
        {
            var pedido = await _context.Pedidos
                                .Include(p => p.Detalles)
                                .FirstOrDefaultAsync(p => p.Id == id);
            if (pedido == null) return NotFound();
            pedido.NumeroMesa = NumeroMesa;

            for (int i = 0; i < platoIds.Length; i++)
            {
                var detalle = pedido.Detalles.FirstOrDefault(d => d.PlatoId == platoIds[i]);
                if (detalle != null)
                {
                    if (cantidades[i] > 0)
                        detalle.Cantidad = cantidades[i];
                    else
                        _context.PedidoDetalles.Remove(detalle); 
                }
                else if (cantidades[i] > 0)
                {
                    pedido.Detalles.Add(new PedidoDetalle
                    {
                        PlatoId = platoIds[i],
                        Cantidad = cantidades[i]
                    });
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //--------------------
        public async Task<IActionResult> ConfirmarPago(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Pagado = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ReporteVentas()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Plato)
                .Where(p => p.Estado == EstadoPedido.Servido && p.Pagado)
                .ToListAsync();

            return View(pedidos);
        }



    }
}
