using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAppFirst.Data;
using MyAppFirst.Models;

namespace MyAppFirst.Controllers
{
    public class ItemController : Controller
    {
        private readonly MyAppContext _appContext;

        public ItemController(MyAppContext appContext) { 
            _appContext = appContext;
        }

        public async Task<IActionResult> Indice() { 
            var item = await _appContext.Items.ToListAsync();
            return View(item);
        }

        public IActionResult Create() { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Description, Price")] Item item) { 
            if(ModelState.IsValid)
            {
                _appContext.Items.Add(item);
                await _appContext.SaveChangesAsync();
                return RedirectToAction("Indice");
            }
            return View(item);
        }
    }
}
