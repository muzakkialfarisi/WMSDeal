using WMSDeal.ViewModels;

namespace WMSDeal.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel model)
	{
		InitializeComponent();
		this.BindingContext= model;

		if(App.UserInfo != null)
		{
			lblHomeFullName.Text = App.UserInfo.FirstName + " " +App.UserInfo.LastName;
			lblHomeJobPosName.Text = App.UserInfo.JobPosName;
			lblHomeWilayah.Text = App.UserInfo.Warehouse;
		}
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
		if(BindingContext is HomeViewModel vm)
		{
			vm.RefreshCommand.Execute(vm);
		}
    }
}