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
    
    public partial class WorkOrder
    {
        public string OrderType { get; set; }
        public decimal ID { get; set; }
        public string OrderSuffix { get; set; }
        public string RelatedPOSOWOOrderType { get; set; }
        public string RelatedPOSOWONumber { get; set; }
        public Nullable<decimal> LineNumber { get; set; }
        public Nullable<decimal> PegtoWorkOrder { get; set; }
        public string ParentWONumber { get; set; }
        public string TypeWO { get; set; }
        public string PriorityWO { get; set; }
        public string AdditionalWork { get; set; }
        public string StatusComment { get; set; }
        public string Company { get; set; }
        public string BusinessUnit { get; set; }
        public string Branch { get; set; }
        public string Location { get; set; }
        public string Aisle { get; set; }
        public string Bin { get; set; }
        public string StatusCodeWO { get; set; }
        public Nullable<int> DateStatusChanged { get; set; }
        public string Subsidiary { get; set; }
        public Nullable<decimal> AddressNumber { get; set; }
        public Nullable<decimal> AddressNumberOriginator { get; set; }
        public Nullable<decimal> AddressNumberManager { get; set; }
        public Nullable<decimal> Supervisor { get; set; }
        public Nullable<decimal> AddressNumberAssignedTo { get; set; }
        public Nullable<int> DateWOPlannedCompleted { get; set; }
        public Nullable<decimal> AddressNumberInspector { get; set; }
        public Nullable<decimal> NextAddressNumber { get; set; }
        public Nullable<int> DateOrderTransaction { get; set; }
        public Nullable<int> DateStart { get; set; }
        public Nullable<int> DateRequested { get; set; }
        public Nullable<int> DateCompletion { get; set; }
        public Nullable<int> DateAssignedto { get; set; }
        public Nullable<int> DateAssignedtoInspector { get; set; }
        public Nullable<int> DatePaperPrintedDate { get; set; }
        public string CategoriesWorkOrder01 { get; set; }
        public string CategoriesWorkOrder02 { get; set; }
        public string CategoriesWorkOrder03 { get; set; }
        public string CategoriesWorkOrder04 { get; set; }
        public string CategoriesWorkOrder05 { get; set; }
        public string EnterpriseOneConsultingStatus { get; set; }
        public string EnterpriseOneConsultingServiceType { get; set; }
        public string EnterpriseOneConsultingSkillType { get; set; }
        public string E1ConsultingExperienceLevel { get; set; }
        public string CategoriesWorkOrder10 { get; set; }
        public string Reference { get; set; }
        public string Reference2 { get; set; }
        public Nullable<decimal> AmountEstimated { get; set; }
        public Nullable<decimal> CrewSize { get; set; }
        public Nullable<decimal> RevenueRateMarkupOverride { get; set; }
        public string PayDeductionBenefitType { get; set; }
        public Nullable<decimal> AmountChangetoOriginalDollars { get; set; }
        public Nullable<decimal> HoursEstimated { get; set; }
        public Nullable<decimal> HoursChangetoOriginalHours { get; set; }
        public Nullable<decimal> AmountActual { get; set; }
        public Nullable<decimal> HoursActual { get; set; }
        public Nullable<decimal> ItemNumberShort { get; set; }
        public string ThirdItemNumber { get; set; }
        public string SecondItemNumber { get; set; }
        public Nullable<decimal> AssetItemNumber { get; set; }
        public string UnitorTagNumber { get; set; }
        public Nullable<decimal> UnitsOrderTransactionQuantity { get; set; }
        public Nullable<decimal> UnitsQtyBackorderedHeld { get; set; }
        public Nullable<decimal> UnitsQuantityCanceledScrapped { get; set; }
        public Nullable<decimal> QuantityShipped { get; set; }
        public Nullable<decimal> UnitsShippedtoDate { get; set; }
        public string UnitofMeasureasInput { get; set; }
        public string MessageNumber { get; set; }
        public Nullable<decimal> TimeBeginning { get; set; }
        public string TypeBillofMaterial { get; set; }
        public string TypeofRouting { get; set; }
        public string WOPickListPrinted { get; set; }
        public string PostingEdit { get; set; }
        public string VarianceFlag { get; set; }
        public string BillofMaterialN { get; set; }
        public string RouteSheetN { get; set; }
        public string WorkOrderFlashMessage { get; set; }
        public string WorkOrderFreezeCode { get; set; }
        public string IndentedCode { get; set; }
        public Nullable<decimal> ResequenceCode { get; set; }
        public Nullable<decimal> AmountMilesorHours { get; set; }
        public Nullable<int> DateScheduledTickler { get; set; }
        public Nullable<decimal> AmountMemoBudgetChanges { get; set; }
        public Nullable<decimal> PercentComplete { get; set; }
        public Nullable<decimal> LeadtimeLevel { get; set; }
        public Nullable<decimal> LeadtimeCumulative { get; set; }
        public Nullable<decimal> HoursUnaccountedDirectLabor { get; set; }
        public string LotSerialNumber { get; set; }
        public Nullable<decimal> LotPotency { get; set; }
        public string LotGrade { get; set; }
        public Nullable<decimal> RatioCriticalRatioPriority1 { get; set; }
        public Nullable<decimal> RatioCriticalRatioPriority2 { get; set; }
        public string DocumentType { get; set; }
        public string SubledgerInactiveCode { get; set; }
        public string CompanyKeyRelatedOrder { get; set; }
        public string BillRevisionLevel { get; set; }
        public string RoutingRevisionLevel { get; set; }
        public string DrawingChange { get; set; }
        public string RoutingChange { get; set; }
        public string NewPartNumber { get; set; }
        public string ReasonforECO { get; set; }
        public string PhaseIn { get; set; }
        public string ExistingDisposition { get; set; }
        public string BOMChange { get; set; }
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
        public Nullable<decimal> ParentNumber { get; set; }
        public string NextStatusWO { get; set; }
        public Nullable<decimal> CriticalityWorkOrder { get; set; }
        public Nullable<decimal> EstimatedDowntimeHours { get; set; }
        public Nullable<decimal> ActualDowntimeHours { get; set; }
        public Nullable<decimal> ServiceAddressNumber { get; set; }
        public Nullable<decimal> MeterPosition { get; set; }
        public string ApprovalType { get; set; }
        public Nullable<decimal> AmountEstimatedLabor { get; set; }
        public Nullable<decimal> AmountEstimatedMaterial { get; set; }
        public Nullable<decimal> AmountEstimatedOther { get; set; }
        public Nullable<decimal> AmountActualLabor { get; set; }
        public Nullable<decimal> AmountActualMaterial { get; set; }
    }
}
