using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WMSDeal.ViewModels
{
    public partial class ScanViewModel
    {
        public ICommand BackCommand => new Command(() =>
        {
            Shell.Current.GoToAsync("..");
        });
    }
}
