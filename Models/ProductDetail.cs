using CommunityToolkit.Mvvm.ComponentModel;
using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Model
{
    public partial class ProductDetail : BaseViewModel
    {
        [ObservableProperty]
        private string imageUrl;

        [ObservableProperty]
        private string itemType;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string singlePrice;

        [ObservableProperty]
        private string mixPrice;

        [ObservableProperty]
        private List<ColorGroupModel> colorGroups;
    }
    
    public class ColorGroupModel
    {
        public string Color { get; set; }
        public List<ItemModel> Items { get; set; }
    }

    public partial class ItemModel : BaseViewModel
    {
        public string ItemCode { get; set; }
        public string Size { get; set; }
        
        public ItemUomModel ItemUom { get; set; }
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalPrice))]
        private int quantity;
        
        public decimal TotalPrice => Quantity * ItemUom.Price;
    }

    public class ItemUomModel
    {
        public string UOM { get; set; }
        public decimal Price { get; set; }
    }
}