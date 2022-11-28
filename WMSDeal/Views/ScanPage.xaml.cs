using CommunityToolkit.Mvvm.Messaging;
using Plugin.Maui.Audio;
using WMSDeal.Messages;
using WMSDeal.ViewModels;
using ZXing.Net.Maui;
using ZXing.QrCode.Internal;

namespace WMSDeal.Views;

public partial class ScanPage : ContentPage
{
    private readonly IAudioManager audioManager;
    bool isDetected = false;
    public ScanPage(ScanViewModel model, IAudioManager audioManager)
    {
        InitializeComponent();
        this.BindingContext = model;
        this.audioManager = audioManager;

        barcodeView.Options = new BarcodeReaderOptions()
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = false,
        };
    }

    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (!isDetected)
        {

            foreach (var barcode in e.Results)
            {
                //Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

                if (barcode.Value != null && lbBarcodeText.Text == "SCAN BARCODE")
                {
                    //Device.InvokeOnMainThreadAsync(async() =>
                    Dispatcher.DispatchAsync(async () =>
                    {
                        //var result = $"{e.Results[0].Value}{e.Results[0].Format}";

                        //var result = $"{e.Results[0].Value}";
                        //var format = $"{e.Results[0].Format}";

                        WeakReferenceMessenger.Default.Send(new ScanMessage(barcode.Value.ToString()));
                        //WeakReferenceMessenger.Default.Send(new ScanMessage(result.ToString()));

                        lbBarcodeText.Text = barcode.Value.ToString().ToUpper();

                        //lbBarcodeText.Text = result.ToString().ToUpper();


                        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("beep-sound.mp3"));
                        player.Play();

                        //await Shell.Current.DisplayAlert("", result.ToString(), "OK");

                        await Shell.Current.GoToAsync("..", true);

                    });
                }
            }
            isDetected = true;
        }
        if (barcodeView.IsTorchOn)
        {
            barcodeView.IsTorchOn = !barcodeView.IsTorchOn;
        }
    }

    void SwitchCameraButton_Clicked(object sender, EventArgs e)
    {
        barcodeView.CameraLocation = barcodeView.CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
    }

    void TorchButton_Clicked(object sender, EventArgs e)
    {
        barcodeView.IsTorchOn = !barcodeView.IsTorchOn;
    }
}