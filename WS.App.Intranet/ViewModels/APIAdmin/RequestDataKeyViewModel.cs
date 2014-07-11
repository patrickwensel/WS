namespace WS.App.Intranet.ViewModels.APIAdmin
{
    public class RequestDataKeyViewModel
    {
        public decimal ID { get; set; }
        public decimal? APIRequestDataKeyClassID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ValueRequired { get; set; }
    }
}