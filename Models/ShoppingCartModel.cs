namespace NYC.MobileApp.Model;

public class ShoppingCartModel
{
    public string UserId { get; set; }  // optional, if tied to a user
    public string ItemType { get; set; }
    public List<ShoppingCartItemModel> Items { get; set; } = new();

    public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
}

public class ShoppingCartItemModel
{
    public string ItemCode { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
    public string UOM { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public decimal TotalPrice => Quantity * Price;
}