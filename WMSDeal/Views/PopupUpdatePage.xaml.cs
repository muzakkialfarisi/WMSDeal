using CommunityToolkit.Maui.Views;

namespace WMSDeal.Views;

public partial class PopupUpdatePage : Popup
{
    string link = App.LinkUpdate;
    public PopupUpdatePage()
    {
        InitializeComponent();
        linkUpdateText.Text = link;
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        Uri uri = new Uri(link);
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    }
}