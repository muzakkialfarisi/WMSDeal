using WMSDeal.ViewModels.Startup;

namespace WMSDeal.Views.Startup;

public partial class UserProfilePage : ContentPage
{
	public UserProfilePage(UserProfileViewModel model)
	{
		InitializeComponent();
		this.BindingContext= model;
	}
}