using NYC.MobileApp.Model;
using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class CategoryDetailView : ContentPage
{
    public CategoryDetailView(CategoryModel data)
    {
        InitializeComponent();
        BindingContext = new CategoryDetailViewModel(data);
    }
}