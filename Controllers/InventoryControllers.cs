using Microsoft.AspNetCore.SignalR;
using InventorySystem.Hubs;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Data;
using InventorySystem.Models;

public class InventoryController : Controller
{
    private readonly IHubContext<InventoryHub> _hubContext;
    private readonly AppDbContext _context;

    public InventoryController(IHubContext<InventoryHub> hubContext, AppDbContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    // Example method for updating inventory
    public async Task<IActionResult> UpdateProduct(Product updatedProduct)
    {
        // Update the product in the database
        _context.Products.Update(updatedProduct);
        await _context.SaveChangesAsync();

        // Notify all clients about the inventory update
        await _hubContext.Clients.All.SendAsync("InventoryUpdated");

        return RedirectToAction("Index");
    }

    // Example method for adding a product
    public async Task<IActionResult> AddProduct(Product newProduct)
    {
        // Add new product to the database
        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();

        // Notify all clients about the inventory update
        await _hubContext.Clients.All.SendAsync("InventoryUpdated");

        return RedirectToAction("Index");
    }
}
