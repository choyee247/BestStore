using Example2.Data;
using Example2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example2.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Pay(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound();

            var vm = new PaymentViewModel
            {
                OrderId = order.Id,
                Amount = order.OrderItems.Sum(i => i.Quantity * i.Product.Price)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var payment = new Payment
            {
                OrderId = model.OrderId,
                CardHolder = model.CardHolder,
                CardNumber = model.CardNumber,
                Expiry = model.Expiry,
                CVV = model.CVV
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { id = model.OrderId });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return View(order);
        }
    }
}
