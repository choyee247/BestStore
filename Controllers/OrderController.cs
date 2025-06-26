using Example2.Data;
using Example2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Example2.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            var totalOrders = await _context.Orders.CountAsync();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(orders);
        }
        private bool CheckProductAvailability(List<ProductSelection> items)
        {
            foreach (var item in items)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    return false;
                }
            }
            return true;
        }
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    var productList = _context.Products.ToList();
        //    ViewBag.Products = new SelectList(productList, "Id", "ProductName");
        //    return View();
        //}
        [HttpGet]
        public IActionResult Create()
        {
            var products = _context.Products.ToList();
            ViewBag.ProductsList = products;

            // Initialize with one empty item
            var model = new OrderFormViewModel
            {
                Items = { new ProductSelection() }
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderFormViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    var products = _context.Products.ToList();
            //    ViewBag.ProductsList = products;
            //    return View(new OrderFormViewModel
            //    {
            //        Items = { new ProductSelection() }
            //    });
            //}
            // Check product availability
            if (!CheckProductAvailability(model.Items.Where(i => i.Quantity > 0).ToList()))
            {
                ModelState.AddModelError("", "One or more products are not available in the requested quantity.");
                var products = _context.Products.ToList();
                ViewBag.ProductsList = products;
                return View(new OrderFormViewModel
                {
                    Items = { new ProductSelection() }
                });
            }
            var order = new Order
            {
                Id=model.Id,
                CustomerName = model.CustomerName,
                Email = model.Email,
                Address = model.Address,
                ShippingMethod = model.ShippingMethod,
                ShippingCost = model.ShippingCost,
                OrderItems = model.Items
                    .Where(i => i.Quantity > 0)
                    .Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
            };
            // Update product quantities
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                product.StockQuantity -= item.Quantity;
                _context.Products.Update(product);
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = order.Id });
        }
        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }
        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {// Return quantities to stock
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        _context.Products.Update(product);
                    }
                }
                _context.OrderItems.RemoveRange(order.OrderItems); // remove children first
                _context.Orders.Remove(order);                     // then parent
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var products = _context.Products.ToList();
            ViewBag.ProductsList = products;

            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            var model = new OrderFormViewModel
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Email = order.Email,
                Address = order.Address,
                ShippingMethod = order.ShippingMethod,
                ShippingCost = order.ShippingCost,
                OrderItems = order.OrderItems,
                Items = order.OrderItems.Select(oi => new ProductSelection
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList()

            };

            var allProducts = _context.Products.ToList();

            //ViewBag.Products = new SelectList(allProducts, "Id", "ProductName");
            return View(model);  // Pass the correct view model here!
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderFormViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Products = new SelectList(_context.Products.ToList(), "Id", "ProductName");
            //    var products = _context.Products.ToList();
            //    ViewBag.ProductsList = products;
            //    return View(model);
            //}
            // Check product availability
            if (!CheckProductAvailability(model.Items.Where(i => i.Quantity > 0).ToList()))
            {
                ModelState.AddModelError("", "One or more products are not available in the requested quantity.");
                //ViewBag.Products = new SelectList(_context.Products.ToList(), "Id", "ProductName");
                var products = _context.Products.ToList();
                ViewBag.ProductsList = products;
                return View(new OrderFormViewModel
                {
                    Items = { new ProductSelection() }
                });
            }
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == model.Id);

            if (order == null)
                return NotFound();
            // First, return old quantities to stock
            foreach (var oldItem in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(oldItem.ProductId);
                product.StockQuantity += oldItem.Quantity;
                _context.Products.Update(product);
            }
            // Update basic fields
            order.CustomerName = model.CustomerName;
            order.Email = model.Email;
            order.Address = model.Address;
            order.ShippingMethod = model.ShippingMethod;
            order.ShippingCost = model.ShippingCost;

            // Clear existing items and add new ones
            _context.OrderItems.RemoveRange(order.OrderItems);

            order.OrderItems = model.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                OrderId = model.Id
            }).ToList();
            // Deduct new quantities from stock
            foreach (var newItem in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(newItem.ProductId);
                product.StockQuantity -= newItem.Quantity;
                _context.Products.Update(product);
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit");
        }
    }
}
