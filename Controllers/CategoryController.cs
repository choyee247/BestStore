using Example2.Data;
using Example2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Example2.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public IActionResult Edit()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();

                
                return RedirectToAction("Index");
            }

          
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges(true);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var totalCategories = await _context.Categories.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCategories / pageSize);

            var categories = await _context.Categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(categories);
        }
    
        [HttpGet]
        public JsonResult GetBrandsByCategory(int categoryId)
        {
            var brands = _context.Brands
                .Join(_context.BrandandCatViews,
                    b => b.Id,
                    bc => bc.BrandId,
                    (b, bc) => new { b.Id, b.Name, bc.CategoryId })
                .Join(_context.Categories,
                    bbc => bbc.CategoryId,
                    c => c.Id,
                    (bbc, c) => new
                    {
                        BrandId = bbc.Id,
                        BrandName = bbc.Name,
                        CategoryId = c.Id,
                        CategoryName = c.Name
                    })
                .Where(b => b.CategoryId == categoryId)
                .Select(b => new { id = b.BrandId, name = b.BrandName })
                .ToList();

            return Json(brands);
        }
    }

}
