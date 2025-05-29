using NYC.MobileApp.ViewModel;
using static NYC.MobileApp.Model.TrackOrderModel;

namespace NYC.MobileApp.Views;

public partial class TrackOrderView : ContentPage
{
    public TrackOrderView(Track data)
    {
        InitializeComponent();
        BindingContext = new TrackOrderViewModel(data);
    }
}