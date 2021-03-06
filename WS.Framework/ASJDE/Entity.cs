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
    
    public partial class Entity
    {
        public Entity()
        {
            this.Branch = new HashSet<Branch>();
            this.BusinessUnit = new HashSet<BusinessUnit>();
            this.Company = new HashSet<Company>();
            this.Depot = new HashSet<Depot>();
            this.OperationalBusinessUnit = new HashSet<OperationalBusinessUnit>();
            this.StrategicBusinessUnit = new HashSet<StrategicBusinessUnit>();
            this.AS_HOURS_WORKED = new HashSet<HoursWorked>();
            this.AS_SAFETY_INCIDENT = new HashSet<SafetyIncident>();
        }
    
        public decimal ID { get; set; }
        public Nullable<decimal> LocationID { get; set; }
        public Nullable<decimal> StatusID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CostCenter { get; set; }
    
        public virtual EntityStatus EntityStatus { get; set; }
        public virtual ICollection<Branch> Branch { get; set; }
        public virtual ICollection<BusinessUnit> BusinessUnit { get; set; }
        public virtual ICollection<Company> Company { get; set; }
        public virtual ICollection<Depot> Depot { get; set; }
        public virtual ICollection<OperationalBusinessUnit> OperationalBusinessUnit { get; set; }
        public virtual ICollection<StrategicBusinessUnit> StrategicBusinessUnit { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<HoursWorked> AS_HOURS_WORKED { get; set; }
        public virtual ICollection<SafetyIncident> AS_SAFETY_INCIDENT { get; set; }
    }
}
