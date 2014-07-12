using System;
using System.Collections.Generic;
using Attribute = WS.Framework.Objects.WorkOrder.Attribute;

namespace WS.Framework.Objects.MobileWorkOrder
{
    public class MobileWorkOrder
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string VersionNumber { get; set; }
        public int IsUnitAttributeChanged { get; set; }
        public string WorkOrderID { get; set; }
        public string EmployeeNumber { get; set; }
        public string ReportLocationNumber { get; set; }
        public string TabletID { get; set; }
        public string AdditionalWork { get; set; }
        public string Company { get; set; }
        public string UserDefinedCodeID { get; set; }
        public int AssetID { get; set; }
        public string MajorAccountingClass { get; set; }
        public string BusinessUnit { get; set; }
        public string UnitNumber { get; set; }
        public string ComplexUnitNumber { get; set; }
        public string SerialNumber { get; set; }
        public string EquipmentClass { get; set; }
        public string Manufacturer { get; set; }
        public string ModelYear { get; set; }
        public int TotalLength { get; set; }
        public int BoxWidth { get; set; }
        public int BoxLength { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public DateTime DateAcquired { get; set; }
        public int SquareFootage { get; set; }
        public int UnitReturnCode { get; set; }
        public object UnitReturnCodeMessage { get; set; }
        public string Kit2ndItemNumber { get; set; }
        public string RevenueBranch { get; set; }
        public string OMBOrderInbound { get; set; }
        public string UnitAttributeChanges { get; set; }

        public List<MobileWorkOrderPart> MobileWorkOrderParts { get; set; }
        public List<MobileWorkOrderActivity> MobileWorkOrderActivities { get; set; }
        public List<MobileWorkOrderImage> MobileWorkOrderImages { get; set; }
        public List<UnitAttribute> UnitAttributes { get; set; }
    }
}
