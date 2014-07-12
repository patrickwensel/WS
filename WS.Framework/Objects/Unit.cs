using System;
using System.Collections.Generic;
using WS.Framework.Objects.WorkOrder;

namespace WS.Framework.Objects
{
    public class Unit
    {
        public int AssetID { get; set; }
        public string MajorAccountingClass { get; set; }
        public string BusinessUnit { get; set; }
        public string UnitNumber { get; set; }
        public string ComplexUnitNumber { get; set; }
        public string SerialNumber { get; set; }
        public string EquipmentClass { get; set; }
        public string Manufacturer { get; set; }
        public string ModelYear { get; set; }
        public int? TotalLength { get; set; }
        public int? BoxWidth { get; set; }
        public int? BoxLength { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public DateTime DateAcquired { get; set; }
        public int? SquareFootage { get; set; }
        public int UnitReturnCode { get; set; }
        public string UnitReturnCodeMessage { get; set; }
        public string Kit2ndItemNumber { get; set; }

        public List<UnitAttribute> UnitAttributes { get; set; }
        public List<OMBOrder> OMBOrders { get; set; }
    }
}
