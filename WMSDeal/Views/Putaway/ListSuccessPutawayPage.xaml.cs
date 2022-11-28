using WMSDeal.ViewModels.Putaway;

namespace WMSDeal.Views.Putaway;

public partial class ListSuccessPutawayPage : ContentPage
{
	public ListSuccessPutawayPage(SuccessPutawayViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
}