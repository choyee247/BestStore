using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Example2.Models;
using Example2.Data; 
using Microsoft.EntityFrameworkCore; 
using System.Linq;

namespace Example2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context; 

    // Update constructor to inject DbContext
    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context; 
    }

    public IActionResult Index()
    {
        var maxRating = _context.Reviews.Max(r => r.Rating);

        var countMaxRating = _context.Reviews.Count(r => r.Rating == maxRating);


        // Create dashboard view model with database queries
        var model = new DashboardViewModel
        {
            ProductSummary = new ProductSummary
            {
                TotalProducts = _context.Products.Count(),
                TotalCategories = _context.Categories.Count(),
                TopRated = countMaxRating, //Count(p => p.Rating >= 2),
                NewArrivals = _context.Products.Count(p =>
                    p.CreatedAt >= DateTime.Now.AddDays(-30))
            },
            OrderSummary = new OrderSummary
            {
                TotalOrders = _context.Orders.Count(),
                Completed = _context.Orders.Count(o => o.Status == OrderStatus.Completed),
                Pending = _context.Orders.Count(o => o.Status == OrderStatus.Pending),
                Cancelled = _context.Orders.Count(o => o.Status == OrderStatus.Cancelled)
            },
        };
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
