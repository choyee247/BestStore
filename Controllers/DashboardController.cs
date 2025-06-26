using Example2.Data;
using Example2.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Example2.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var totalProducts = _context.Products.Count();
            var totalCategories = _context.Products.Select(p => p.Category).Distinct().Count();
            var averagePrice = _context.Products.Average(p => p.Price);

            var products = new DashboardViewModel
            {
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                AveragePrice = averagePrice
            };
            
            return View(products);
        }

    }
}
