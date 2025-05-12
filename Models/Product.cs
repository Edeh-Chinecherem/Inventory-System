
namespace InventorySystem.Models
{
    public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int RestockThreshold { get; set; }

    public bool IsDeleted { get; set; }

    public List<SaleItem> SaleItems { get; set; } = new();
}
}
