using AllUp.DAL;
using AllUp.Helpers;
using AllUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllUp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

       
        public CategoryController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
       

        #region Index
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _db.Categories.Include(x => x.Children).Include(x => x.Parent).ToListAsync();
            return View(categories);
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            return View();
        }
        #endregion

        #region Create Post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, int? MainCatId)
        {
            ViewBag.Categories = await _db.Categories.Where(x => x.IsMain).ToListAsync();

            
            if (category.IsMain)
            {
                bool isExist = await _db.Categories.AnyAsync(x => x.Parent.Name == category.Name);
                if (isExist)
                {
                    ModelState.AddModelError("Title", "This service already is exist");
                    return View();
                }

                if (category.Photo == null)
                {
                    ModelState.AddModelError("Photo", "please select a photo");
                    return View();
                }
                if (!category.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "please select a image type");
                    return View();
                }
                if (category.Photo.IsOlder1MB())
                {
                    ModelState.AddModelError("Photo", "Max 1 MB");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "images");
                category.Image = await category.Photo.SaveFileAsync(folder);
            }
            else
            {
                if (MainCatId == null)
                {
                    return NotFound();
                }
                category.ParentId = MainCatId;
            }


            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
           
            return View();

        }
        #endregion
    }
}
