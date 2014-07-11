using System.Collections.Generic;

namespace AS.App.PassportAddOn.ViewModels.Video
{
    public class SBUVideoHierarchy
    {
        public decimal ID { get; set; }
        public string Name { get; set; }

        public List<OBUVideoHierarchy> OBUVideoHierarchies { get; set; }
    }
}