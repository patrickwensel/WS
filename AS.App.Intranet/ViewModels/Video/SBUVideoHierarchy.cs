using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AS.App.Intranet.ViewModels.Video
{
    public class SBUVideoHierarchy
    {
        public decimal ID { get; set; }
        public string Name { get; set; }

        public List<OBUVideoHierarchy> OBUVideoHierarchies { get; set; }
    }
}