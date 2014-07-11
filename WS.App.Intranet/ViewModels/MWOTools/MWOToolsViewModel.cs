using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WS.App.Intranet.ViewModels
{
    public class MWOToolsViewModel
    {
        [Required]
        [DisplayName("Work Order Number")]
        public int WorkOrderNumber { get; set; }
    }
}