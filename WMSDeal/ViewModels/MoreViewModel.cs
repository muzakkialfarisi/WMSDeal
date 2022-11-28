using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WMSDeal.Views.More;

namespace WMSDeal.ViewModels
{
    public partial class MoreViewModel
    {
        public ICommand HandoverCommand => new Command(() =>
        {
            Shell.Current.GoToAsync("Handover");
        });
        public ICommand StockOpnameCommand => new Command(() =>
        {
            Shell.Current.GoToAsync("StockOpname");
        });

        public ICommand TransferStorageCommand => new Command(() =>
        {
            Shell.Current.GoToAsync("TransferStorage");
        });
        public ICommand DashboardCommand => new Command( () =>
        {
            Shell.Current.GoToAsync("StockOpname");
        });
    }
}
