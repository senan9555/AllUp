using AllUp.DAL;
using AllUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllUp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _db.Categories.Include(x=>x.Children).Include(x=>x.Parent).ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
