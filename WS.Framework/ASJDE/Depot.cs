//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WS.Framework.ASJDE
{
    using System;
    using System.Collections.Generic;
    
    public partial class Depot
    {
        public Depot()
        {
            this.VideoArea = new HashSet<VideoArea>();
        }
    
        public decimal ID { get; set; }
        public Nullable<decimal> BranchID { get; set; }
        public Nullable<decimal> EntityID { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual ICollection<VideoArea> VideoArea { get; set; }
    }
}