using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventorySystem.Models;
using InventorySystem.Data;
using Microsoft.EntityFrameworkCore;


namespace InventorySystem.Pages.Products
{
    public class EditProductModel : PageModel
{
    private readonly AppDbContext _context;
    [BindProperty] public Product Product { get; set; }
    public EditProductModel(AppDbContext context) => _context = context;

    public IActionResult OnGet(int id)
    {
        Product = _context.Products.Find(id);
        if (Product == null) return NotFound();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        _context.Attach(Product).State = EntityState.Modified;
        _context.SaveChanges();
        return RedirectToPage("/Products/Index");
    }
}
}
