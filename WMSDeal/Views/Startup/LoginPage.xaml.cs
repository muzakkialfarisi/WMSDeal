using WMSDeal.ViewModels.Startup;

namespace WMSDeal.Views.Startup;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel model)
	{
		InitializeComponent();
		this.BindingContext= model;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		txUserName.Text = "";
		txPassword.Text = "";
    }
}