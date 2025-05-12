using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Models;
using InventorySystem.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace InventorySystem.Pages.Sales
{
    [Authorize]
    public class ReceiptModel : PageModel
{
    private readonly AppDbContext _context;
    public Sale Sale { get; set; }

    public ReceiptModel(AppDbContext context) => _context = context;

    public IActionResult OnGet(int id)
    {
        Sale = _context.Sales.Include(s => s.SaleItems).FirstOrDefault(s => s.Id == id);
        if (Sale == null) return NotFound();
        return Page();
    }
}
}