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
    
    public partial class HoursWorked
    {
        public decimal ID { get; set; }
        public Nullable<decimal> CountryID { get; set; }
        public decimal LocationID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Nullable<decimal> Hours { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUserName { get; set; }
        public Nullable<System.DateTime> CreatedByDate { get; set; }
        public Nullable<decimal> EntityID { get; set; }
        public string EditedByUserName { get; set; }
        public Nullable<System.DateTime> EditedByDate { get; set; }
        public string EditedByName { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual Location Location { get; set; }
        public virtual Entity Entity { get; set; }
    }
}
