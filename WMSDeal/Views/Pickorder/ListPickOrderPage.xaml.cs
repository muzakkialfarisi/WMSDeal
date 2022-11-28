using WMSDeal.ViewModels.Pickorder;

namespace WMSDeal.Views.Pickorder;

public partial class ListPickOrderPage : ContentPage
{
	public ListPickOrderPage(PickOrderViewModel model)
	{
		InitializeComponent();
		this.BindingContext = model;
	}
  //  protected override void OnAppearing()
  //  {
  //      base.OnAppearing();
		//if(BindingContext is PickOrderViewModel vm)
		//{
		//	vm.RefreshCommand.Execute(this);
		//}
  //  }
}