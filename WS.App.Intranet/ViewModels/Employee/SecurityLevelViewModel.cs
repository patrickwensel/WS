using System;

namespace WS.App.Intranet.ViewModels.Employee
{
    public class SecurityLevelViewModel
    {
        public decimal ApplicationID { get; set; }
        public decimal LevelID { get; set; }
        public string Description { get; set; }
        public decimal? SubApplicationID { get; set; }
    }
}