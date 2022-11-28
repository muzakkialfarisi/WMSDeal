using WMSDeal.ViewModels.Startup;

namespace WMSDeal.Views.Startup;

public partial class LoadingPage : ContentPage
{
	public LoadingPage(LoadingViewModel model)
	{
		InitializeComponent();
		this.BindingContext= model;
	}
}