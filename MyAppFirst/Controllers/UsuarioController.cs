using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAppFirst.Data;
using MyAppFirst.Models;

namespace MyAppFirst.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly MyAppContext _context;
        public UsuarioController(MyAppContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string usuario, string password)
        {
            var user = _context.Usuarios
                .FirstOrDefault(u => u.UsuarioLogin == usuario && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }

            // Guardar usuario en sesión
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRol", user.Rol.ToString());
            HttpContext.Session.SetString("UserNombre", user.Nombre);

            // Redirigir según rol
            return user.Rol switch
            {
                RolUsuario.Mozo => RedirectToAction("Index", "Pedidos"),
                RolUsuario.Cocina => RedirectToAction("Index", "Cocina"),
                RolUsuario.Admin => RedirectToAction("ReporteVentas", "Pedidos"),
                _ => View()
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        //============== 
        
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }

        // Vista para crear usuario
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(usuario);
        }

    }
}
