using WMSDeal.ViewModels.Deliveryorder;

namespace WMSDeal.Views.Deliveryorder;

public partial class ArrivalProductPage : ContentPage
{
	public ArrivalProductPage(ArrivalProductViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
}