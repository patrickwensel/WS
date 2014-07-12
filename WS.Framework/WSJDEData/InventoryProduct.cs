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
    
    public partial class InventoryProduct
    {
        public int ID { get; set; }
        public int InventoryID { get; set; }
        public int ProductID { get; set; }
        public Nullable<int> Usable { get; set; }
        public Nullable<int> Repairable { get; set; }
        public string CreatedByUser { get; set; }
        public string ModifiedByUser { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
    
        public virtual Inventory Inventory { get; set; }
        public virtual Product Product { get; set; }
    }
}
