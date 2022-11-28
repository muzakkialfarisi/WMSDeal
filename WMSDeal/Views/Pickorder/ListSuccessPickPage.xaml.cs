using WMSDeal.ViewModels.Pickorder;

namespace WMSDeal.Views.Pickorder;

public partial class ListSuccessPickPage : ContentPage
{
	public ListSuccessPickPage(SuccessPickViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
}