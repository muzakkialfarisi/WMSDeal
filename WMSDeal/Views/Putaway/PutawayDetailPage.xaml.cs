using WMSDeal.ViewModels.Putaway;

namespace WMSDeal.Views.Putaway;

public partial class PutawayDetailPage : ContentPage
{
	public PutawayDetailPage(PutawayDetailViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		if(BindingContext is PutawayDetailViewModel vm)
		{
			vm.Waiting();
		}
    }
}