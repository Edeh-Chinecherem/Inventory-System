using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Models;
using InventorySystem.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace InventorySystem.Pages.Dashboard
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public ChartData SalesChart { get; set; }
        public ChartData TopProductsChart { get; set; }
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
            DateTime startDate = Range == "weekly" ? DateTime.Today.AddDays(-7) : DateTime.Today.AddMonths(-1);

            // Fix: Use AsEnumerable() to calculate in-memory for SalesChart
            var sales = _context.Sales
                .Where(s => s.Timestamp >= startDate)
                .AsEnumerable() // Moves calculation to memory to avoid LINQ to SQL issues
                .GroupBy(s => s.Timestamp.Date)
                .Select(g => new { Date = g.Key.ToString("MM/dd"), Total = g.Sum(s => s.TotalAmount) })
                .OrderBy(s => s.Date)
                .ToList();

            // Fix: Use AsEnumerable() for SaleItems grouping
            var topProducts = _context.SaleItems
                .Where(si => si.Sale.Timestamp >= startDate)
                .AsEnumerable() // Move to in-memory calculation
                .GroupBy(si => si.ProductName)
                .Select(g => new { Product = g.Key, Quantity = g.Sum(si => si.Quantity) })
                .OrderByDescending(p => p.Quantity)
                .Take(5)
                .ToList();

            TotalSalesCount = _context.Sales.Count(s => s.Timestamp >= startDate);
            
            // Fix: Use AsEnumerable() for total revenue calculation to avoid translation issue
            TotalRevenue = _context.Sales
                .Where(s => s.Timestamp >= startDate)
                .AsEnumerable() // Move to in-memory calculation
                .Sum(s => s.TotalAmount);
            
            LowStockCount = _context.Products.Count(p => p.Quantity < p.RestockThreshold);

            // Set up chart data
            SalesChart = new ChartData
            {
                labels = sales.Select(s => s.Date).ToList(),
                data = sales.Select(s => s.Total).ToList()
            };

            TopProductsChart = new ChartData
            {
                labels = topProducts.Select(p => p.Product).ToList(),
                data = topProducts.Select(p => (decimal)p.Quantity).ToList()
            };
        }
    }

    public class ChartData
    {
        public List<string> labels { get; set; }
        public List<decimal> data { get; set; }
    }
}
