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
    
    public partial class APIRequestData
    {
        public decimal ID { get; set; }
        public decimal APIRequestID { get; set; }
        public Nullable<decimal> APIRequestDataKeyID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    
        public virtual APIRequestDataKey APIRequestDataKey { get; set; }
        public virtual APIRequest APIRequest { get; set; }
    }
}
