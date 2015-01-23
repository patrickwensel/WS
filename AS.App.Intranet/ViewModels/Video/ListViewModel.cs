using System.ComponentModel;

namespace AS.App.Intranet.ViewModels.Video
{
    public class ListViewModel
    {
        public decimal ID { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

    }
}