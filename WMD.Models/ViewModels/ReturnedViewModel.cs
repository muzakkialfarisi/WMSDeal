using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WMS.Models.ViewModels
{
    public class ReturnedViewModel
    {
        [ValidateNever]
        public InvReturn invReturn { get; set; }

        [ValidateNever]
        public OutSalesOrder outSalesOrder { get; set; }
    }
}
