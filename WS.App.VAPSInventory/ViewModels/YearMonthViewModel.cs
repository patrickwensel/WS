using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WS.App.VAPSInventory.ViewModels
{
    public class YearMonthViewModel
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }
    }
}