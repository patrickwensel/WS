//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WS.Framework.WSJDEData
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaperlessRequest
    {
        public PaperlessRequest()
        {
            this.PaperlessRequestOrder = new HashSet<PaperlessRequestOrder>();
        }
    
        public decimal ID { get; set; }
        public Nullable<decimal> AccountNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RequesterEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string SendToEmail { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual ICollection<PaperlessRequestOrder> PaperlessRequestOrder { get; set; }
    }
}
