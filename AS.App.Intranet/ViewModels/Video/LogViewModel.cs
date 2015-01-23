using System.ComponentModel;

namespace AS.App.Intranet.ViewModels.Video
{
    public class LogViewModel
    {
        [DisplayName("ID")]
        public decimal ID { get; set; }

        [DisplayName("User Name")]
        public string UserName { get; set; }

        [DisplayName("View Date")]
        public string ViewDate { get; set; }

        [DisplayName("Video Name")]
        public string VideoName { get; set; }
    }
}