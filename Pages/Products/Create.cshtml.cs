using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventorySystem.Models;
using InventorySystem.Data;
using System.Threading.Tasks;

namespace InventorySystem.Pages.Products
{
    public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    [BindProperty] public Product Product { get; set; }
    public CreateModel(AppDbContext context) => _context = context;

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        _context.Products.Add(Product);
        _context.SaveChanges();
        return RedirectToPage("/Products/Index");
    }
}
}
