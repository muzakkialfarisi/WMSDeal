using WMSDeal.ViewModels.Pickorder;

namespace WMSDeal.Views.Pickorder;

public partial class ListCurrentPickPage : ContentPage
{
	 public ListCurrentPickPage(PickViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}

}