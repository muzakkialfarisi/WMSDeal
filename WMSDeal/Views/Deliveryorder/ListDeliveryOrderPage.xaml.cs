using WMSDeal.ViewModels.Deliveryorder;

namespace WMSDeal.Views.Deliveryorder;

public partial class ListDeliveryOrderPage : ContentPage
{
	public ListDeliveryOrderPage(DeliveryOrderViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
		if(BindingContext is DeliveryOrderViewModel vm)
		{
			vm.RefreshCommand.Execute(this);
		}
    }
}