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
            ViewBag.Categories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            if(id == null)
            {
                return NotFound();
            }
            Category dbcategory =await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(dbcategory == null)
            {
                return BadRequest();
            }
            return View(dbcategory);

        }
        #endregion

        #region Update Post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category,int CatId)
        {
            ViewBag.Categories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            if (id == null)
            {
                return NotFound();
            }
            Category dbcategory = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (dbcategory == null)
            {
                return BadRequest();
            }
            if (dbcategory.IsMain)
            {
                bool isExist = await _db.Categories.AnyAsync(x => x.Parent.Name == category.Name);
                if (isExist)
                {
                    ModelState.AddModelError("Title", "This category already is exist");
                    return View(dbcategory);
                }
                if(category.Photo != null)
                {
                    if (!category.Photo.IsImage())
                    {
                        ModelState.AddModelError("Photo", "please select a photo");
                        return View(dbcategory);
                    }
                    if (category.Photo.IsOlder1MB())
                    {
                        ModelState.AddModelError("Photo", "Max 1MB");
                        return View(dbcategory);
                    }
                    string folder = Path.Combine(_env.WebRootPath, "assets", "images");
                    dbcategory.Image = await category.Photo.SaveFileAsync(folder);
                }

            }
            else
            {
                if(CatId == null)
                {
                    return NotFound();
                }
                dbcategory.ParentId = CatId;
            }
            dbcategory.Name=category.Name;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            Category? category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category == null)
            {
                return BadRequest();
            }
            return View(category);
        }

        #endregion
    }
}
