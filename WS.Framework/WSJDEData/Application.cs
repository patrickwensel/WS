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
    
    public partial class Application
    {
        public Application()
        {
            this.WebSecurity = new HashSet<Security>();
        }
    
        public decimal ID { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Status { get; set; }
    
        public virtual ICollection<Security> WebSecurity { get; set; }
    }
}
