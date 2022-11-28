using WMSDeal.ViewModels.Pickorder;

namespace WMSDeal.Views.Pickorder;

public partial class PickOrderDetailPage : ContentPage
{
	public PickOrderDetailPage(PickOrderDetailViewModel model)
	{
		InitializeComponent();
		this.BindingContext= model;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}