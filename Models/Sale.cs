
namespace InventorySystem.Models
{
    public class Sale
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public List<SaleItem> SaleItems { get; set; } = new();
    public decimal TotalAmount => SaleItems.Sum(i => i.Quantity * i.Price);
}
}