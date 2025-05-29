using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Model
{
    public class ProductListModel: BaseViewModel
    {
        public string ItemType { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; }
        public string? BrandName { get; set; }
        public string? Price { get; set; }
        public string ImageUrl { get; set; }
        public string Details { get; set; }
        
        public string SingleQty { get; set; }
        public string MixSizeQty { get; set; }

        private bool _IsAvailable;
        public bool IsAvailable
        {
            get => _IsAvailable;
            set
            {
                if (_IsAvailable != value)
                {
                    _IsAvailable = value;
                    OnPropertyChanged(nameof(IsAvailable));
                    OnPropertyChanged(nameof(AvailableColor));
                }
            }
        }
        public Color AvailableColor
        {
            get
            {
                if (IsAvailable)
                {
                    return Color.FromArgb("#00C569");
                }
                return Color.FromArgb("#FFB900");
            }
        }

        public string AvailableText
        {
            get
            {
                if (IsAvailable)
                {
                    return "In Stock";
                }
                return "Out of Stock";
            }
        }

    }
}
