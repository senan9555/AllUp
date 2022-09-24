using AllUp.DAL;
using AllUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AllUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> IndexAsync()
        {
            List<Category> categories = await _db.Categories.Where(x=>x.IsMain).ToListAsync();
            return View(categories);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}