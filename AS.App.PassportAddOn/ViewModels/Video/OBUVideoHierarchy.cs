using System.Collections.Generic;

namespace AS.App.PassportAddOn.ViewModels.Video
{
    public class OBUVideoHierarchy
    {

        public decimal ID { get; set; }
        public string Name { get; set; }

        public List<BUVideoHierarchy> BUVideoHierarchies { get; set; }
    }
}