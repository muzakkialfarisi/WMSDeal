using WMSDeal.ViewModels;

namespace WMSDeal.Views;

public partial class MorePage : ContentPage
{
	public MorePage(MoreViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
}