using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Models;
using InventorySystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace InventorySystem.Pages.Dashboard
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public ChartData? SalesChart { get; set; }
        public ChartData? TopProductsChart { get; set; }
        public int TotalSalesCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Range { get; set; } = "monthly";

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            
            DateTime startDate = Range switch
            {
                "weekly" => DateTime.Today.AddDays(-7),
                "daily" => DateTime.Today,
                _ => DateTime.Today.AddMonths(-1)
            };

            var sales = _context.Sales
                .Where(s => s.Timestamp >= startDate)
                .Include(s => s.SaleItems)
                .AsEnumerable()
                .GroupBy(s => s.Timestamp.Date)
                .Select(g => new { Date = g.Key.ToString("MM/dd"), Total = g.Sum(s => s.TotalAmount) })
                .OrderBy(s => s.Date)
                .ToList();

            var s = _context.Sales.Include(s => s.SaleItems).ToList();
            var t = s.GroupBy(k => k.Timestamp.Date);
            
            
            
            var topProducts = _context.SaleItems
                .Include(si => si.Sale)
                .Where(si => si.Sale.Timestamp >= startDate)
                .AsEnumerable()
                .GroupBy(si => si.ProductName)
                .Select(g => new { Product = g.Key, Quantity = g.Sum(si => si.Quantity) })
                .OrderByDescending(p => p.Quantity)
                .Take(5)
                .ToList();

            TotalSalesCount = _context.Sales.Count(s => s.Timestamp >= startDate);

            TotalRevenue = _context.Sales
                .Where(s => s.Timestamp >= startDate)
                .AsEnumerable()
                .Select(s => s.TotalAmount)
                .DefaultIfEmpty(0)
                .Sum();
            
            
            LowStockCount = _context.Products
                .Where(p => p.Quantity <= p.RestockThreshold && !p.IsDeleted)
                .Count();
            Console.WriteLine("Products in DB: " + _context.Products.Count());


            Console.WriteLine($"Low Stock Count: {LowStockCount}");

            
            SalesChart = new ChartData
            {
                Labels = [.. sales.Select(s => s.Date)],
                Data = [.. sales.Select(s => s.Total)]
            };

            
            TopProductsChart = new ChartData
            {
                Labels = topProducts.Select(p => p.Product).ToList(),
                Data = topProducts.Select(p => (decimal)p.Quantity).ToList()
            };
        }
    }

    public class ChartData
    {
        public List<string>? Labels { get; set; }
        public List<decimal>? Data { get; set; }
    }
}
