using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models.ViewModels.ApiViewModel.Maui
{
    public class APIResponseViewModel
    {
        public int statusCode { get; set; } = StatusCodes.Status500InternalServerError;
        public string message { get; set; } = "Can't reach server!";
    }

    public class HomeDashboard
    {
        public string Title { get; set; }
        public int Total { get; set; }
        public int Done { get; set; }
        public int Outstanding { get; set; }
    }
}
