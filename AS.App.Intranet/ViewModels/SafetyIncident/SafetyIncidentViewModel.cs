using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AS.App.Intranet.Infrastructure;


namespace AS.App.Intranet.ViewModels.SafetyIncident
{
    public class SafetyIncidentViewModel
    {
        public decimal ID { get; set; }

        [UIHint("SIStatusID")]
        [DisplayName("Status")]
        [Required(ErrorMessage = "Please enter a Status")]
        public decimal? StatusID { get; set; }

        [DisplayName("Employee Type")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Employee Type is Required")]
        public decimal? EmployeeTypeID { get; set; }

        [DisplayName("Site Category")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Site Category is Required")]
        public decimal? SiteCategoryID { get; set; }

        [DisplayName("Type")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Type is Required")]
        public decimal? TypeID { get; set; }

        [DisplayName("Outcome")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "3", ErrorMessage = "Outcome is required")]
        public decimal? OutcomeID { get; set; }

        [DisplayName("Unsafe Act/Condition")]
        public decimal? UnsafeActID { get; set; }

        [DisplayName("Cause Category")]
        public decimal? CauseCategoryID { get; set; }

        [DisplayName("Cause")]
        public decimal? CauseID { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Date is Required")]
        public DateTime? DateOfIncident { get; set; }

        [DisplayName("Date Returned to Work")]
        [Validators.GreaterDateAttribute(EarlierDateField = "DateOfIncident",
            ErrorMessage = "Return to work date should not be before the Incident date")]
        public DateTime? DateReturnedToWork { get; set; }

        [DisplayName("Date Returned to Work Restricted")]
        [Validators.GreaterDateAttribute(EarlierDateField = "DateOfIncident",
            ErrorMessage = "Return to work date should  not be before the Incident date")]
        public DateTime? DateReturnedToWorkRestricted { get; set; }

        [DisplayName("Employee Name")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Name is Required")]
        public string EmployeeName { get; set; }

        [DisplayName("Title/Position")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Title/Position is Required")]
        public string Title { get; set; }

        [DisplayName("Was injury work related?")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Work Related is Required")]
        public string WorkRelated { get; set; }

        [DisplayName("Location of Site")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Location of Site is Required")]
        public string LocationSite { get; set; }

        [DisplayName("Description")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Description is Required")]
        public string Description { get; set; }

        [DisplayName("Days Away from Work")]
        [Range(0, 10000, ErrorMessage = "Please enter a value greater than 0")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "2,3", ErrorMessage = "Days Away is required")]
        public decimal? DaysAwayFromWork { get; set; }

        [DisplayName("Days Transferred or Restricted")]
        [Range(0, 10000, ErrorMessage = "Please enter a value greater than 0")]
        public decimal? DaysTransferredRestricted { get; set; }

        [DisplayName("Return to Work?")]
        [Validators.ConditionalRequired(FieldName = "StatusID", DesiredFieldValue = "3", ErrorMessage = "Return to Work is Required")]
        public string ReturnToWork { get; set; }

        [DisplayName("Root Cause")]
        public string RootCause { get; set; }

        [DisplayName("Corrective Action taken to prevent recurrence")]
        public string CorrectiveAction { get; set; }

        [DisplayName("Corrective Action Responsible Person")]
        public string ResponsiblePerson { get; set; }

        [DisplayName("Corrective Action Deadline")]
        [Validators.GreaterDateAttribute(EarlierDateField = "DateOfIncident", ErrorMessage = "Corrective Action Deadline should  not be before the Incident date")]
        public DateTime? Deadline { get; set; }

        [DisplayName("Corrective Action Complete?")]
        public string Complete { get; set; }

        [DisplayName("Recordable")]
        public string Recordable { get; set; }

        [DisplayName("Created By")]
        public string CreatedByUser { get; set; }

        [DisplayName("Created Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "Please enter a Country")]
        public decimal CountryID { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Please enter a Location")]
        public decimal LocationID { get; set; }

        [DisplayName("Entity")]
        [Required(ErrorMessage = "Please enter an Entity")]
        public decimal EntityID { get; set; }

        public string EmailMessage { get; set; }

    }
   
}
