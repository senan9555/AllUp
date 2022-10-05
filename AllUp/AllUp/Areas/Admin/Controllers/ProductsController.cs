using AllUp.DAL;
using AllUp.Helpers;
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
            Category? firstMainCat = await _db.Categories.Include(x => x.Children).FirstOrDefaultAsync(x => x.IsMain);
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            ViewBag.ChildCategories = firstMainCat.Children;
            return View();
        }

        #endregion

        #region Create Post

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Product product, int? MainCatId, int? childCatId)
        {
            Category? firstMainCat = await _db.Categories.Include(x => x.Children).FirstOrDefaultAsync(x => x.IsMain);
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            ViewBag.ChildCategories = firstMainCat.Children;
            if (product.Photos == null)
            {
                ModelState.AddModelError("Photos", "add photo");
            };
            if (MainCatId == null)
            {
                return BadRequest();
            }
            List<ProductImage> productImages = new List<ProductImage>();
            foreach (IFormFile Photo in product.Photos)
            {
                if (!Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "please select a image type");
                    return View();
                }
                if (Photo.IsOlder1MB())
                {
                    ModelState.AddModelError("Photo", "Max 1 MB");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "images", "product");
                ProductImage productImage = new ProductImage
                {
                    Image = await Photo.SaveFileAsync(folder),
                };
                productImages.Add(productImage);
            }
            List<ProductCategory> productCategories = new List<ProductCategory>();
            ProductCategory mainProductCategory = new ProductCategory
            {
                CategoryId = (int)MainCatId,
            };

            ProductCategory childProductCategory = new ProductCategory
            {
                CategoryId = (int)childCatId,
            };

            productCategories.Add(mainProductCategory);
            productCategories.Add(childProductCategory);
            product.ProductCategories = productCategories;
            product.ProductImages = productImages;
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        #endregion

        #region LoadChildCategories
        public async Task<IActionResult> LoadChildCategories(int MainCatId)
        {
            List<Category> childCategories = await _db.Categories.Where(x => x.ParentId == MainCatId).ToListAsync();
            return PartialView("_LoadChildCategoriesPartial", childCategories);
        }
        #endregion

        #region DeleteProductImages

        public async Task<IActionResult> DeleteProductImages(int? proImgId)
        {
            ProductImage? productImage = await _db.ProductImages.Include(x=>x.Product).ThenInclude(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id == proImgId);
            int productImagesCount = productImage.Product.ProductImages.Count;
            _db.ProductImages.Remove(productImage);
            await _db.SaveChangesAsync();
            if (productImagesCount == 2)
            {
                return Content("stop");
            }
            return Content("ok");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int? id)
        {
            if(id== null)
            {
                return NotFound();
            }
            Product? dbProduct = await _db.Products.Include(x=>x.ProductCategories).ThenInclude(x=>x.Category).ThenInclude(x=>x.Children).Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id==id);
            if(dbProduct == null)
            {
                return BadRequest();
            }
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            return View(dbProduct);
        }

        #endregion


    }
}
