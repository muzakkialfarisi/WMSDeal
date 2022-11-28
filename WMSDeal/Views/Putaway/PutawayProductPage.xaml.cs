using WMSDeal.ViewModels.Putaway;

namespace WMSDeal.Views.Putaway;

public partial class PutawayProductPage : ContentPage
{
	public PutawayProductPage(PutawayProductViewModel model)
	{
		InitializeComponent();
		this.BindingContext=model;
	}
}