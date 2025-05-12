using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventorySystem.Models;
using InventorySystem.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace InventorySystem.Pages.Products
{
    [Authorize]
    public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public List<Product> Products { get; set; }

    public IndexModel(AppDbContext context) => _context = context;

    public void OnGet() => Products = _context.Products.ToList();
}
}
