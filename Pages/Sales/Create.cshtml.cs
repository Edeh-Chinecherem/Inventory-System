using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventorySystem.Models;
using InventorySystem.Data;
using System.Threading.Tasks;
using InventorySystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<InventoryHub> _hubContext;

        public CreateModel(AppDbContext context, IHubContext<InventoryHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [BindProperty] public List<SaleItem> Items { get; set; }

        // This will be passed to the Razor Page
        public List<Product> AvailableProducts { get; set; }

        public void OnGet()
        {
            // Get only products that are in stock
            AvailableProducts = _context.Products
                .Where(p => p.Quantity > 0)
                .ToList();
        }

        public IActionResult OnPost()
        {
            var sale = new Sale
            {
                Timestamp = DateTime.UtcNow
            };

            foreach (var item in Items)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                    return BadRequest("Insufficient stock");

                product.Quantity -= item.Quantity;

                if (product.Quantity < product.RestockThreshold)
                {
                    // TODO: Send notification logic (email or in-app)
                }
            }

            _context.Sales.Add(sale);
            _context.SaveChanges();

            foreach (var item in Items)
            {
                item.SaleId = sale.Id;
                _context.SaleItems.Add(item);
            }

            _context.SaveChanges();

            // Notify all connected clients that inventory has been updated
            _hubContext.Clients.All.SendAsync("InventoryUpdated");

            return RedirectToPage("/Sales/Receipt", new { id = sale.Id });
        }
    }
}
