using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AS.App.Intranet.ViewModels.Video
{
    public class OBUVideoHierarchy
    {

        public decimal ID { get; set; }
        public string Name { get; set; }

        public List<BUVideoHierarchy> BUVideoHierarchies { get; set; }
    }
}