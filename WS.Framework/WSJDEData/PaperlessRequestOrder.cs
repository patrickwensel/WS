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
    
    public partial class PaperlessRequestOrder
    {
        public decimal ID { get; set; }
        public Nullable<decimal> PaperlessRequestID { get; set; }
        public string OrderNumber { get; set; }
        public string Attention { get; set; }
        public string UnitNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    
        public virtual PaperlessRequest PaperlessRequest { get; set; }
    }
}
