namespace WS.App.Intranet.ViewModels.APIAdmin
{
    public class RequestDataViewModel
    {
        public decimal ID { get; set; }
        public decimal APIRequestID { get; set; }
        public decimal? APIRequestDataKeyID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}