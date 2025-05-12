using Microsoft.AspNetCore.Mvc.RazorPages;
using InventorySystem.Models;
using InventorySystem.Data;
using Microsoft.EntityFrameworkCore; // ✅ Needed for .Include()
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace InventorySystem.Pages.Sales
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Sale> Sales { get; set; }

        public void OnGet()
        {
            Sales = _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .ToList();
        }
    }
}
