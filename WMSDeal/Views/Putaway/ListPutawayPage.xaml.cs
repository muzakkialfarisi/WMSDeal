using WMSDeal.ViewModels.Putaway;

namespace WMSDeal.Views.Putaway;

public partial class ListPutawayPage : ContentPage
{
	public ListPutawayPage(PutawayViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
		if(BindingContext is PutawayViewModel vm)
		{
			vm.RefreshCommand.Execute(this);
		}
    }
}