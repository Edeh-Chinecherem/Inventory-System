using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventorySystem.Models;
using InventorySystem.Data;
using System.Threading.Tasks;
using InventorySystem.Hubs;
using Microsoft.AspNetCore.SignalR;


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

    public IActionResult OnPost()
    {
        var sale = new Sale();
        foreach (var item in Items)
        {
            var product = _context.Products.Find(item.ProductId);
            if (product == null || product.Quantity < item.Quantity)
                return BadRequest("Insufficient stock");

            product.Quantity -= item.Quantity;

            if (product.Quantity < product.RestockThreshold)
            {
                // Add notification logic (email/in-app)
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

        // Notify clients of inventory update
        _hubContext.Clients.All.SendAsync("InventoryUpdated");

        return RedirectToPage("/Sales/Receipt", new { id = sale.Id });
    }
}
        }
    

