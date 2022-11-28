using WMSDeal.ViewModels.Deliveryorder;

namespace WMSDeal.Views.Deliveryorder;

public partial class ListArrivalOrderPage : ContentPage
{
	public ListArrivalOrderPage(ArrivalDeliveryOrderViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
		Shell.Current.DisplayAlert("Deal", "appeared", "OK");
    }
}