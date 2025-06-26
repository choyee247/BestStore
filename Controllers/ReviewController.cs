using Example2.Data;
using Example2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Example2.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var reviews = _context.Reviews.Include(r => r.Product).ToList();
            return View(reviews);
        }

        public IActionResult Create(int ProductId)
        {
            return View(new Review { ProductId = ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Review review)
        {
            Console.WriteLine("product id.........................." + review.ProductId);
            if (!ModelState.IsValid)
            {
                _context.Reviews.Add(new Review()
                {
                    ReviewerName = review.ReviewerName,
                    ProductId = review.ProductId,
                    Product = _context.Products.FirstOrDefault(p => p.ProductId == review.ProductId),
                    Rating = review.Rating,
                    Comment = review.Comment,
                    ReviewDate = DateTime.Now
                });
                _context.SaveChanges();

                return RedirectToAction("Details", "Product", new { id = review.ProductId });
            }
            return View(review);
        }


        public IActionResult Edit(int id)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return NotFound();

            return View(review);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Review review)
        {
            //if (ModelState.IsValid)
            //{
            _context.Reviews.Update(review);
            _context.SaveChanges();
            return RedirectToAction("Index", new { productId = review.ProductId });
            //}

            //return View(review);
        }
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.Include(r => r.Product).FirstOrDefault(r => r.Id == id);
            if (review == null)
                return NotFound();

            return View(review);
        }
        public IActionResult TopRated()
        {
            var topRatedProducts = _context.Products
                .Where(p => p.Rating >= 4.5)
                .Include(p => p.Reviews) // Optional: Include related reviews
                .ToList();

            return View(topRatedProducts); // Create a view to display these products
        }
    }
}
