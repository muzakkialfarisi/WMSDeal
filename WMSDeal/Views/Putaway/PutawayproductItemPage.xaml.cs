using WMSDeal.ViewModels.Putaway;

namespace WMSDeal.Views.Putaway;

public partial class PutawayproductItemPage : ContentPage
{
	public PutawayproductItemPage(PutawayProductItemViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
}