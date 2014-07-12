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
    
    public partial class ItemCost
    {
        public decimal ItemNumberShort { get; set; }
        public string SecondItemNumber { get; set; }
        public string ThirdItemNumber { get; set; }
        public string BusinessUnit { get; set; }
        public string Location { get; set; }
        public string LotSerialNumber { get; set; }
        public string LotGrade { get; set; }
        public string CostMethod { get; set; }
        public Nullable<decimal> AmountUnitCost { get; set; }
        public string CostingSelectionPurchasing { get; set; }
        public string CostingSelectionInventory { get; set; }
        public string UserReservedCode { get; set; }
        public Nullable<int> UserReservedDate { get; set; }
        public Nullable<decimal> UserReservedAmount { get; set; }
        public Nullable<decimal> UserReservedNumber { get; set; }
        public string UserReservedReference { get; set; }
        public string UserID { get; set; }
        public string ProgramID { get; set; }
        public string WorkStationID { get; set; }
        public Nullable<int> DateUpdated { get; set; }
        public Nullable<decimal> TimeofDay { get; set; }
        public string CostChangedFlag { get; set; }
        public Nullable<decimal> CarryingCost { get; set; }
        public Nullable<decimal> OverstockCost { get; set; }
        public Nullable<decimal> StockoutCost { get; set; }
    }
}