using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WS.App.Intranet.ViewModels.MWOTools
{
    public class StuckImagesViewModel
    {
        [Required]
        [DisplayName("Folder Name")]
        public string FolderName { get; set; }

    }
}