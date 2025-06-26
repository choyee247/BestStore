using Example2.Models;
using Example2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Linq;

namespace Example2.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder, int pageNumber = 1)
        {
            ViewBag.CurrentFilter = searchString;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.BrandSortParm = sortOrder == "Brand" ? "brand_desc" : "Brand";
            //ViewBag.Today = DateTime.Now.ToString("dd-MM-yyyy");

            var products = context.Products.Include(p => p.Category)
                .Include(p => p.Brand).Where(p => !p.IsDeleted)
                .AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.Contains(searchString));
            }
            //ViewBag.TotalProducts = context.Products.Count(); // or add any filters if needed

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "Brand":
                    products = products.OrderBy(p => p.Brand);
                    break;
                case "brand_desc":
                    products = products.OrderByDescending(p => p.Brand);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            int pageSize = 6;

            var PaginatedList = await PaginatedList<Product>.CreateAsync(products, pageNumber, pageSize);
            ViewBag.TotalProducts = await products.CountAsync(); // ← Total count here
            ViewBag.Today = DateTime.Now.ToString("dd-MM-yyyy");
            ViewBag.TopRated = context.Products
                .Where(p => p.Rating >= 4.5)
                .OrderByDescending(p => p.Rating)
                .Take(6)
                .ToList();

            ViewBag.NewArrivals = context.Products
                .Where(p => p.CreatedAt >= DateTime.Now.AddDays(-30))
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToList();
            return View(PaginatedList);
        }
        public IActionResult Create()
        {
            //ViewBag.Categories = new SelectList(context.Categories, "Id", "Name");
            //ViewBag.Brands = new SelectList(context.Brands, "Id", "Name");
            var newProduct = new ProductDto
            {
                CreatedAt = DateTime.Today,
                Categories = context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),

                Brands = context.Brands.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList()
            };

            return View(newProduct);
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
            else if (productDto.ImageFile.Length > 30720)
            {
                ModelState.AddModelError("ImageFile", "The image size must be 30 KB or less.");
            }
            if (productDto.BrandId == 0)
            {
                ModelState.AddModelError("BrandId", "Please select a brand.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(context.Categories, "Id", "Name");
                ViewBag.Brands = new SelectList(context.Brands, "Id", "Name");
                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                 Path.GetExtension(productDto.ImageFile.FileName);
            string imageFullPath = Path.Combine(environment.WebRootPath, "Products", newFileName);

            using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            long imageSizeKb = productDto.ImageFile.Length / 1024;

            int newProductId = context.Products.Any() ? context.Products.Max(t => t.ProductId) + 1 : 1;

            Product product = new Product()
            {
                ProductId = newProductId,
                ProductName = productDto.ProductName,
                BrandId = productDto.BrandId,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            Console.WriteLine($"Image saved: {imageFullPath} ({imageSizeKb} KB)");
            Console.WriteLine($"New Product Created: {product.ProductName}, Entry Date: {product.CreatedAt.ToShortDateString()}");

            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }

        public IActionResult Edit(int Id)
        {
            var product = context.Products.Find(Id);

            ViewBag.Categories = new SelectList(context.Categories.ToList(), "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(context.Brands.ToList(), "Id", "Name", product.BrandId);

            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            var productDto = new ProductDto()

            {
                ProductName = product.ProductName,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreateAt.ToString("dd/MM/yyyy");

            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int Id, ProductDto productDto)
        {
            var product = context.Products.Find(Id);

            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            if (productDto.ImageFile != null)
            {
                // Check file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(productDto.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, .png, and .gif formats are allowed.");
                }

                // Check file size: must be less than or equal to 500KB
                if (productDto.ImageFile.Length > 30 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Image size must not exceed 30KB.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(context.Categories.ToList(), "Id", "Name", productDto.CategoryId);
                ViewBag.Brands = new SelectList(context.Brands.ToList(), "Id", "Name", productDto.BrandId);

                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreateAt.ToString("dd/MM/yyyy");

                return View(productDto);
            }

            string newFileName = product.ImageFileName;

            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDto.ImageFile.FileName);

                string uploadsFolder = Path.Combine(environment.WebRootPath, "Products");
                string newFilePath = Path.Combine(uploadsFolder, newFileName);

                // Save new image
                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // Delete old image
                string oldImageFullPath = Path.Combine(uploadsFolder, product.ImageFileName);
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }

                // Update filename
                product.ImageFileName = newFileName;

                // Log path and size
                long fileSizeKB = productDto.ImageFile.Length / 1024;
                Console.WriteLine($"Image uploaded: /Products/{newFileName} | Size: {fileSizeKB} KB");
            }

            // Update product info
            product.ProductName = productDto.ProductName;
            product.BrandId = productDto.BrandId;
            product.CategoryId = productDto.CategoryId;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.UpdatedAt = DateTime.Now;

            context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }
   
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }

            product.IsDeleted = true;
            context.SaveChanges();

            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RecycleBin()
        {
            var deletedProducts = await context.Products
                                      .Where(p => p.IsDeleted)
                                      .Include(p => p.Brand)
                                      .Include(p => p.Category)
                                      .ToListAsync();

            return View(deletedProducts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
                return NotFound();

            product.IsDeleted = false;   // un-delete
            context.SaveChanges();

            TempData["Success"] = "Product restored successfully!";
            return RedirectToAction("RecycleBin");
        }
        public IActionResult Details(int id)
        {
            var product = context.Products
                .Include(p => p.Reviews)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null) return NotFound();

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                Review = new Review { Id = product.ProductId }
            };

            return View(viewModel);
        }

        public IActionResult TopRated()
        {
            var products = context.Products
                .Where(p => p.Rating >= 4.5)
                .ToList();

            return View(products);
        }
        //public IActionResult TopRated()
        //{
        //    var topRatedProducts = context.Products
        //        .Where(p => p.Rating >= 4.5)
        //        .Include(p => p.Reviews) // Optional: Include related reviews
        //        .ToList();

        //    return View(topRatedProducts); // Create a view to display these products
        //}

        public IActionResult NewArrivals()
        {
            var cutoffDate = DateTime.Now.AddDays(-30);

            var products = context.Products
                .Where(p => p.CreatedAt >= cutoffDate)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(products);
        }

    }
}
