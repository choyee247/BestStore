using Example2.Data;
using Example2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Example2.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public IActionResult Create()
        //{
        //    ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Brand brand)
        //{
        //    if (!ModelState.IsValid)
        //    {

        //        _context.Brands.Add(brand);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", brand.BrandandCatViews);

        //    return View(brand);
        //}
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return View(new Brand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                _context.Brands.Add(brand);
                _context.SaveChanges();

                foreach (var catId in brand.SelectedCategoryIds)
                {
                    _context.BrandandCatViews.Add(new BrandandCatView
                    {
                        BrandId = brand.Id,
                        CategoryId = catId
                    });
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return View(brand);
        }
        public IActionResult Edit(int id)
        {

            var brand = _context.Brands.Find(id);
            if (brand == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", brand.BrandandCatViews);
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                _context.Brands.Update(brand);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", brand.BrandandCatViews);
            return View(brand);
        }

        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.Find(id);

            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            _context.SaveChanges(true);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var totalBrands = await _context.Brands.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalBrands / pageSize);

            var brands = await _context.Brands
                .OrderBy(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(brands);
        }
    }

}
