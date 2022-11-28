using WMSDeal.ViewModels.Deliveryorder;

namespace WMSDeal.Views.Deliveryorder;

public partial class DeliveryOrderDetailPage : ContentPage
{
	public DeliveryOrderDetailPage(DeliveryOrderDetailViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
}