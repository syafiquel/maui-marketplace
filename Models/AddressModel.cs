
using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Model
{
    public class AddressModel: BaseViewModel
    {
        public string AddressType { get; set; }
        public string FullAddress { get; set; }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get => _IsSelected;
            set => SetProperty(ref _IsSelected, value);
        }
    }
}
