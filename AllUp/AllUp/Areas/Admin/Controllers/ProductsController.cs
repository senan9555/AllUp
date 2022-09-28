using AllUp.DAL;
using AllUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllUp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;


        public ProductsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _db.Products.Include(x => x.ProductDetail).Include(x => x.ProductImages).Include(x => x.ProductCategories).ThenInclude(x => x.Category).ToListAsync();
            return View(products);
        }
        #endregion

        #region Create

        public async Task<IActionResult> Create()
        {
            Category? firstMainCat= await _db.Categories.Include(x=>x.Children).FirstOrDefaultAsync(x=>x.IsMain);
            ViewBag.MainCategories = await _db.Categories.Where(x=>x.IsMain).ToListAsync();
            ViewBag.ChildCategories = firstMainCat.Children;
            return View();
        }

        #endregion

    }
}
