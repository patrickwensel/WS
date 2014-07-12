using System.Collections.Generic;

namespace WS.SecureAPI.ViewModels.WSCRequest
{
    public class Request
    {
        public string Token { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public List<RequestData> Data { get; set; }
    }
    
}