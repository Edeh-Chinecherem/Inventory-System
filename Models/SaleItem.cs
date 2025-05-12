
namespace InventorySystem.Models
{
    public class SaleItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int SaleId { get; set; }
    public Sale Sale { get; set; }

    public Product Product { get; set; } // Navigation property to Product
}
}