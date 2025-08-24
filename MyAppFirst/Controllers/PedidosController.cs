using System.Reflection.Metadata;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAppFirst.Data;
using MyAppFirst.Models;
using iTextSharp;
using Document = iTextSharp.text.Document;
using System.Net;


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
            ViewBag.Platos = _context.Platos.Where(p => p.Activo).ToList();

            var mozoId = HttpContext.Session.GetInt32("UserId");
            if (mozoId.HasValue){
                var mozo = _context.Usuarios.FirstOrDefault(m => m.Id == mozoId.Value);
                ViewBag.Username = mozo?.Nombre ?? "Desconocido";
            }
            else{
                ViewBag.Username = "Desconocido";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pedido pedido, int[] platoIds, int[] cantidades)
        {
            var mozoId = HttpContext.Session.GetInt32("UserId");
            if (mozoId.HasValue)
            {
                pedido.MozoId = mozoId.Value;

                var mozo = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == mozoId.Value);
                ViewBag.Username = mozo?.Nombre ?? "Desconocido";
            }
            else
            {
                ViewBag.Username = "Desconocido";
            }

            pedido.Detalles ??= new List<PedidoDetalle>();

            //Validacion Cantidad - Filtro
            if (ModelState.IsValid && platoIds.Length == cantidades.Length)
            {
                for (int i = 0; i < platoIds.Length; i++)
                {
                    if (cantidades[i] > 0)
                    {
                        var plato = await _context.Platos
                            .FirstOrDefaultAsync(p => p.Id == platoIds[i] && p.Activo);

                        if (plato != null)
                        {
                            pedido.Detalles.Add(new PedidoDetalle
                            {
                                PlatoId = plato.Id,
                                Cantidad = cantidades[i]
                            });
                        }
                    }
                }

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Platos = await _context.Platos.Where(p => p.Activo).ToListAsync();

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

        //Metodo Para exportar
        [HttpGet]
        public IActionResult ExportarReporteVentas()
        {
            

            var pedidos = _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Plato)
                .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 20, 20, 40, 40);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();


                // Imagen
                string imageUrl = "https://mir-s3-cdn-cf.behance.net/project_modules/2800_opt_1/50c751119128597.60972227ed8fd.png";
                using (WebClient webClient = new WebClient())
                {
                    byte[] imageBytes = webClient.DownloadData(imageUrl);
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imageBytes);

                    // Configurar tamaño y posición
                    img.ScaleToFit(80f, 80f);
                    img.SetAbsolutePosition(document.PageSize.Width - 100f, document.PageSize.Height - 120f);
                    document.Add(img);

                    document.Add(new Paragraph("\n\n"));
                }

                // Titulo
                Paragraph titulo = new Paragraph("Reporte de Ventas",
                    new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.BLUE));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                document.Add(new Paragraph("Generado el: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    new Font(Font.FontFamily.HELVETICA, 10, Font.ITALIC, BaseColor.DARK_GRAY)));
                document.Add(new Paragraph(" ")); 

                // Tabla
                PdfPTable table = new PdfPTable(6); // columnas
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 10f;

                // Ajuste dinámico de proporciones
                // ID (1f), Fecha (2f), Mesa (1f), Plato (3f), Cantidad (1f), Total (2f)
                table.SetWidths(new float[] { 1f, 2f, 1f, 3f, 2f, 2f });

                // Encabezados
                string[] encabezados = { "ID Pedido", "Fecha", "Mesa", "Plato", "Cantidad", "Total (S/.)" };
                foreach (var texto in encabezados)
                {
                    PdfPCell header = new PdfPCell(new Phrase(texto,
                        new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE)));
                    header.BackgroundColor = new BaseColor(52, 152, 219);
                    header.HorizontalAlignment = Element.ALIGN_CENTER;
                    header.Padding = 5;
                    table.AddCell(header);
                }

                // Filas
                decimal totalVentas = 0;
                foreach (var pedido in pedidos)
                {
                    foreach (var detalle in pedido.Detalles.Where(d => d.Cantidad >= 1))
                    {
                        var totalPlato = detalle.Cantidad * detalle.Plato.Precio;
                        totalVentas += totalPlato;

                        table.AddCell(new PdfPCell(new Phrase(pedido.Id.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(pedido.Fecha.ToString("dd/MM/yyyy"))));
                        table.AddCell(new PdfPCell(new Phrase(pedido.NumeroMesa.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(detalle.Plato.Nombre)));
                        table.AddCell(new PdfPCell(new Phrase(detalle.Cantidad.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(totalPlato.ToString("N2"))));
                    }
                }

                // Total
                PdfPCell celdaTotal = new PdfPCell(new Phrase("TOTAL GENERAL:",
                    new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                celdaTotal.Colspan = 5;
                celdaTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
                celdaTotal.BackgroundColor = new BaseColor(230, 230, 230);
                celdaTotal.Padding = 5;
                table.AddCell(celdaTotal);

                PdfPCell celdaTotalValor = new PdfPCell(new Phrase(totalVentas.ToString("N2"),
                    new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.RED)));
                celdaTotalValor.BackgroundColor = new BaseColor(230, 230, 230);
                celdaTotalValor.Padding = 5;
                table.AddCell(celdaTotalValor);

                // Agregar tabla
                document.Add(table);

                // Finnnnnnnnnnnnnnnnnnnn Xd
                Paragraph footer = new Paragraph("Reporte generado automáticamente - Sistema Ventas Loloyta",
                    new Font(Font.FontFamily.HELVETICA, 9, Font.ITALIC, BaseColor.GRAY));
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();

                byte[] bytes = ms.ToArray();
                return File(bytes, "application/pdf", "ReporteVentas.pdf");
            }
        }


    }
}
