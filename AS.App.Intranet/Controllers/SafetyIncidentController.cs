using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using AS.App.Intranet.ViewModels;
using AS.App.Intranet.ViewModels.Company;
using AS.App.Intranet.ViewModels.Hierarchy;
using AS.App.Intranet.ViewModels.SafetyIncident;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using WS.Framework.ASJDE;
using WS.Framework.Objects.ActiveDirectory;
using WS.Framework.Objects.Emails;
using WS.Framework.ServicesInterface;

namespace AS.App.Intranet.Controllers
{
    public class SafetyIncidentController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ILDAPService _ldapService;
        private readonly IEmailService _emailService;
        private readonly IHierarchyService _hierarchyService;

        public SafetyIncidentController(ILDAPService ldapService, IEmailService emailService, IHierarchyService hierarchyService)
        {
            _ldapService = ldapService;
            _emailService = emailService;
            _hierarchyService = hierarchyService;
        }

        #region Incident


        [Authorize(Roles = "AS SG Safety Incident User")]
        public ActionResult Index()
        {
            User user = new User();
            
            user = (User) Session["user"];

            IncidentViewModel incidentViewModel = new IncidentViewModel
                {
                    UserName = user.LastName + ", " + user.FirstName
                };

            string safetyUsers = ConfigurationManager.AppSettings.Get("SafetyUsers");

            string[] safetyUserList = safetyUsers.Split(Convert.ToChar("|"));

            foreach (string safetyUser in safetyUserList.Where(safetyUser => user.UserName == safetyUser))
            {
                incidentViewModel.AccessToSafetyIncidentApplication = true;
            }

            HierarchyController hierarchyController = new HierarchyController();
            LocationController locationController = new LocationController();

            IQueryable<CountryViewModel> countryViewModels = hierarchyController.GetAllCountries();
            ViewData["countries"] = countryViewModels;

            IQueryable<EntityViewModel> entityViewModels = hierarchyController.GetAllEntitiesWithDetails();
            ViewData["entityViewModels"] = entityViewModels;

            IQueryable<LocationViewModel> locationViewModels = locationController.GetAllLocationsWithStatus();
            ViewData["locations"] = locationViewModels;

            PopulateStatuss();
            PopulateSiteCategories();
            PopulateUnsafeActs();
            PopulateCauseCategories();
            PopulateCauses();
            PopulateTypes();
            PopulateOutcomes();
            PopulateEmployeeTypes();

            return View(incidentViewModel);

            
        }

        public ActionResult IncidentRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllIncidents().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IncidentCreate([DataSourceRequest] DataSourceRequest request,
                                           SafetyIncidentViewModel safetyIncidentViewModel)
        {
            try
            {
                if (safetyIncidentViewModel != null && ModelState.IsValid)
                {

                    using (ASJDE context = new ASJDE())
                    {

                        var user = Session["user"] as User;

                        SafetyIncident safetyIncident = new SafetyIncident
                                {
                                    StatusID = safetyIncidentViewModel.StatusID,
                                    EmployeeTypeID = safetyIncidentViewModel.EmployeeTypeID,
                                    SiteCategoryID = safetyIncidentViewModel.SiteCategoryID,
                                    TypeID = safetyIncidentViewModel.TypeID,
                                    OutcomeID = safetyIncidentViewModel.OutcomeID,
                                    UnsafeActID = safetyIncidentViewModel.UnsafeActID,
                                    CauseCategoryID = safetyIncidentViewModel.CauseCategoryID,
                                    CauseID = safetyIncidentViewModel.CauseID,
                                    DateOfIncident = safetyIncidentViewModel.DateOfIncident,
                                    EmployeeName = safetyIncidentViewModel.EmployeeName,
                                    Title = safetyIncidentViewModel.Title,
                                    WorkRelated = safetyIncidentViewModel.WorkRelated,
                                    LocationSite = safetyIncidentViewModel.LocationSite,
                                    Description = safetyIncidentViewModel.Description,
                                    DaysAwayFromWork = safetyIncidentViewModel.DaysAwayFromWork,
                                    DaysTransferredRestricted = safetyIncidentViewModel.DaysTransferredRestricted,
                                    ReturnToWork = safetyIncidentViewModel.ReturnToWork,
                                    DateReturnedToWork = safetyIncidentViewModel.DateReturnedToWork,
                                    RootCause = safetyIncidentViewModel.RootCause,
                                    CorrectiveAction = safetyIncidentViewModel.CorrectiveAction,
                                    ResponsiblePerson = safetyIncidentViewModel.ResponsiblePerson,
                                    Deadline = safetyIncidentViewModel.Deadline,
                                    Complete = safetyIncidentViewModel.Complete,
                                    Recordable = safetyIncidentViewModel.Recordable,
                                    CreatedByUser = user.LastName + ", " + user.FirstName,
                                    CreatedByUserName = user.UserName,
                                    CreatedDate = Convert.ToDateTime(DateTime.Today.ToShortDateString()),
                                    LocationID = safetyIncidentViewModel.LocationID,
                                    CountryID = safetyIncidentViewModel.CountryID,
                                    EntityID = safetyIncidentViewModel.EntityID,
                                    DateReturnedToWorkRestricted = safetyIncidentViewModel.DateReturnedToWorkRestricted

                                };

                        context.SafetyIncidents.Add(safetyIncident);
                        context.SaveChanges();

                        safetyIncidentViewModel.ID = safetyIncident.ID;
                        safetyIncidentViewModel.CreatedByUser = safetyIncident.CreatedByUser;
                        safetyIncidentViewModel.CreatedDate = safetyIncident.CreatedDate;

                    }

                }

                SendEmail(safetyIncidentViewModel);


                return Json(new[] { safetyIncidentViewModel }.ToDataSourceResult(request, ModelState));
            }
            catch (DbEntityValidationException)
            {
                return null;
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IncidentUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, SafetyIncidentViewModel safetyIncidentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        var existingIncident = (from si in context.SafetyIncidents
                                                where si.ID == safetyIncidentViewModel.ID
                                                select si).FirstOrDefault();

                        existingIncident.StatusID = safetyIncidentViewModel.StatusID;
                        existingIncident.EmployeeTypeID = safetyIncidentViewModel.EmployeeTypeID;
                        existingIncident.SiteCategoryID = safetyIncidentViewModel.SiteCategoryID;
                        existingIncident.TypeID = safetyIncidentViewModel.TypeID;
                        existingIncident.OutcomeID = safetyIncidentViewModel.OutcomeID;
                        existingIncident.UnsafeActID = safetyIncidentViewModel.UnsafeActID;
                        existingIncident.CauseCategoryID = safetyIncidentViewModel.CauseCategoryID;
                        existingIncident.CauseID = safetyIncidentViewModel.CauseID;
                        existingIncident.DateOfIncident = safetyIncidentViewModel.DateOfIncident;
                        existingIncident.EmployeeName = safetyIncidentViewModel.EmployeeName;
                        existingIncident.Title = safetyIncidentViewModel.Title;
                        existingIncident.WorkRelated = safetyIncidentViewModel.WorkRelated;
                        existingIncident.LocationSite = safetyIncidentViewModel.LocationSite;
                        existingIncident.Description = safetyIncidentViewModel.Description;
                        existingIncident.DaysAwayFromWork = safetyIncidentViewModel.DaysAwayFromWork;
                        existingIncident.DaysTransferredRestricted = safetyIncidentViewModel.DaysTransferredRestricted;
                        existingIncident.ReturnToWork = safetyIncidentViewModel.ReturnToWork;
                        existingIncident.DateReturnedToWork = safetyIncidentViewModel.DateReturnedToWork;
                        existingIncident.RootCause = safetyIncidentViewModel.RootCause;
                        existingIncident.CorrectiveAction = safetyIncidentViewModel.CorrectiveAction;
                        existingIncident.ResponsiblePerson = safetyIncidentViewModel.ResponsiblePerson;
                        existingIncident.Deadline = safetyIncidentViewModel.Deadline;
                        existingIncident.Complete = safetyIncidentViewModel.Complete;
                        existingIncident.Recordable = safetyIncidentViewModel.Recordable;
                        existingIncident.LocationID = safetyIncidentViewModel.LocationID;
                        existingIncident.CountryID = safetyIncidentViewModel.CountryID;
                        existingIncident.EntityID = safetyIncidentViewModel.EntityID;
                        existingIncident.DateReturnedToWorkRestricted =
                            safetyIncidentViewModel.DateReturnedToWorkRestricted;

                        context.SaveChanges();
                    }

                }

                SendEmail(safetyIncidentViewModel);
            }
            catch (DbEntityValidationException)
            {
                return null;
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
            return Json(new[] { safetyIncidentViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IncidentDestroy([DataSourceRequest] DataSourceRequest request, SafetyIncidentViewModel safetyIncidentViewModel)
        {
            safetyIncidentViewModel.EmailMessage = "This incident has been deleted";

            SendEmail(safetyIncidentViewModel);

            try
            {
                if (safetyIncidentViewModel != null)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        SafetyIncident safetyIncident = (from si in context.SafetyIncidents
                                                         where si.ID == safetyIncidentViewModel.ID
                                                         select si).FirstOrDefault();

                        context.SafetyIncidents.Remove(safetyIncident);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        public IQueryable<SafetyIncidentViewModel> GetAllIncidents()
        {

            ASJDE context = new ASJDE();

            IEnumerable<SafetyIncidentViewModel> safetyIncidentViewModels = (from si in context.SafetyIncidents
                                                                             //where si.StrategicBusinessUnitID == 44
                                                                             select new SafetyIncidentViewModel
                                                                                 {
                                                                                     ID = si.ID,
                                                                                     StatusID = si.StatusID,
                                                                                     EmployeeTypeID = si.EmployeeTypeID,
                                                                                     SiteCategoryID = si.SiteCategoryID,
                                                                                     TypeID = si.TypeID,
                                                                                     OutcomeID = si.OutcomeID,
                                                                                     UnsafeActID = si.UnsafeActID,
                                                                                     CauseCategoryID = si.CauseCategoryID,
                                                                                     CauseID = si.CauseID,
                                                                                     DateOfIncident = si.DateOfIncident,
                                                                                     EmployeeName = si.EmployeeName,
                                                                                     Title = si.Title,
                                                                                     WorkRelated = si.WorkRelated,
                                                                                     LocationSite = si.LocationSite,
                                                                                     Description = si.Description,
                                                                                     DaysAwayFromWork = si.DaysAwayFromWork,
                                                                                     DaysTransferredRestricted = si.DaysTransferredRestricted,
                                                                                     ReturnToWork = si.ReturnToWork,
                                                                                     DateReturnedToWork = si.DateReturnedToWork,
                                                                                     RootCause = si.RootCause,
                                                                                     CorrectiveAction = si.CorrectiveAction,
                                                                                     ResponsiblePerson = si.ResponsiblePerson,
                                                                                     Deadline = si.Deadline,
                                                                                     Complete = si.Complete,
                                                                                     Recordable = si.Recordable,
                                                                                     CreatedByUser = si.CreatedByUser,
                                                                                     CreatedDate = si.CreatedDate,
                                                                                     LocationID = si.LocationID,
                                                                                     CountryID = si.CountryID,
                                                                                     EntityID = si.EntityID.Value,
                                                                                     DateReturnedToWorkRestricted = si.DateReturnedToWorkRestricted
                                                                                 }
                                                                            );

            //safetyIncidentViewModels.Where(s => s.StrategicBusinessUnitID == 44);
            //return safetyIncidentViewModels.Where(s => s.StrategicBusinessUnitID == 44).AsQueryable();

            return safetyIncidentViewModels.AsQueryable();
        }

        #endregion

        #region Status

        public void PopulateStatuss()
        {
            IQueryable<StatusViewModel> statuss = GetAllStatuss();

            ViewData["statuss"] = statuss;
        }

        public JsonResult ReadAllStatuss()
        {
            return Json(GetAllStatuss(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<StatusViewModel> GetAllStatuss()
        {
            ASJDE context = new ASJDE();

            IQueryable<StatusViewModel> statusViewModels = (from a in context.SafetyIncidentStatus
                                                            select new StatusViewModel
                                                              {
                                                                  ID = a.ID,
                                                                  Name = a.Name
                                                              }
                                                             );

            return statusViewModels;
        }

        #endregion

        #region SiteCategory

        public void PopulateSiteCategories()
        {
            IQueryable<SiteCategoryViewModel> siteCategoryViewModels = GetAllSiteCategories();

            ViewData["siteCategoryViewModels"] = siteCategoryViewModels;
        }

        public JsonResult ReadAllSiteCategories()
        {
            return Json(GetAllSiteCategories(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<SiteCategoryViewModel> GetAllSiteCategories()
        {
            ASJDE context = new ASJDE();

            IQueryable<SiteCategoryViewModel> siteCategoryViewModels = (from a in context.SafetyIncidentSiteCategories
                                                                        select new SiteCategoryViewModel
                                                                            {
                                                                                ID = a.ID,
                                                                                Name = a.Name
                                                                            }
                                                                       );

            return siteCategoryViewModels;
        }

        #endregion

        #region UnsafeAct

        public void PopulateUnsafeActs()
        {
            IQueryable<UnsafeActViewModel> unsafeActViewModels = GetAllUnsafeActs();

            ViewData["unsafeActViewModels"] = unsafeActViewModels;
        }

        public JsonResult ReadAllUnsafeActs()
        {
            return Json(GetAllUnsafeActs(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<UnsafeActViewModel> GetAllUnsafeActs()
        {
            ASJDE context = new ASJDE();

            IQueryable<UnsafeActViewModel> unsafeActViewModels = (from a in context.SafetyIncidentUnsafeActs
                                                                  select new UnsafeActViewModel
                                                                  {
                                                                      ID = a.ID,
                                                                      Name = a.Name
                                                                  }
                                                                       );

            return unsafeActViewModels;
        }

        #endregion

        #region CauseCategory

        public void PopulateCauseCategories()
        {
            IQueryable<CauseCategoryViewModel> causeCategoryViewModels = GetAllCauseCategories();

            ViewData["causeCategoryViewModels"] = causeCategoryViewModels;
        }

        public JsonResult ReadAllCauseCategories()
        {
            return Json(GetAllCauseCategories(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<CauseCategoryViewModel> GetAllCauseCategories()
        {
            ASJDE context = new ASJDE();

            IQueryable<CauseCategoryViewModel> causeCategoryViewModels = (from a in context.SafetyIncidentCauseCategories
                                                                          select new CauseCategoryViewModel
                                                                         {
                                                                             ID = a.ID,
                                                                             Name = a.Name
                                                                         }
                                                                       );

            return causeCategoryViewModels;
        }

        public JsonResult GetCausesFilteredByCauseCategoryID(decimal causeCategoryID)
        {
            return Json(GetCauseByCauseCategoryID(causeCategoryID), JsonRequestBehavior.AllowGet);
        }

        public static List<CauseViewModel> GetCauseByCauseCategoryID(decimal causeCategoryID)
        {
            ASJDE context = new ASJDE();

            List<CauseViewModel> causeViewModels = (from a in context.SafetyIncidentCauses
                                                    where a.CauseCategoryID == causeCategoryID
                                                    select new CauseViewModel
                                                     {
                                                         ID = a.ID,
                                                         Name = a.Name
                                                     }
                                                     ).ToList();

            return causeViewModels;
        }

        #endregion

        #region Cause

        public void PopulateCauses()
        {
            IQueryable<CauseViewModel> causeViewModels = GetAllCauses();

            ViewData["causeViewModels"] = causeViewModels;
        }

        public JsonResult ReadAllCauses()
        {
            return Json(GetAllCauses(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<CauseViewModel> GetAllCauses()
        {
            ASJDE context = new ASJDE();

            IQueryable<CauseViewModel> causeViewModels = (from a in context.SafetyIncidentCauses
                                                          select new CauseViewModel
                                                                  {
                                                                      ID = a.ID,
                                                                      Name = a.Name
                                                                  }
                                                                       );

            return causeViewModels;
        }

        #endregion

        #region Type

        public void PopulateTypes()
        {
            IQueryable<TypeViewModel> typeViewModels = GetAllTypes();

            ViewData["typeViewModels"] = typeViewModels;
        }

        public JsonResult ReadAllTypes()
        {
            return Json(GetAllTypes(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<TypeViewModel> GetAllTypes()
        {
            ASJDE context = new ASJDE();

            IQueryable<TypeViewModel> typeViewModels = (from a in context.SafetyIncidentTypes
                                                        select new TypeViewModel
                                                            {
                                                                ID = a.ID,
                                                                Name = a.Name
                                                            }
                                                       );

            return typeViewModels;
        }

        #endregion

        #region Outcome

        public void PopulateOutcomes()
        {
            IQueryable<OutcomeViewModel> outcomeViewModels = GetAllOutcomes();

            ViewData["outcomeViewModels"] = outcomeViewModels;
        }

        public JsonResult ReadAllOutcomes()
        {
            return Json(GetAllOutcomes(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<OutcomeViewModel> GetAllOutcomes()
        {
            ASJDE context = new ASJDE();

            IQueryable<OutcomeViewModel> outcomeViewModels = (from a in context.SafetyIncidentOutcomes
                                                              select new OutcomeViewModel
                                                                  {
                                                                      ID = a.ID,
                                                                      Name = a.Name
                                                                  }
                                                             );

            return outcomeViewModels;
        }

        #endregion

        #region Employee Type

        public void PopulateEmployeeTypes()
        {
            IQueryable<EmployeeTypeViewModel> employeeTypeViewModels = GetAllEmployeeTypes();

            ViewData["employeeTypeViewModels"] = employeeTypeViewModels;
        }

        public JsonResult ReadAllEmployeTypes()
        {
            return Json(GetAllEmployeeTypes(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<EmployeeTypeViewModel> GetAllEmployeeTypes()
        {
            ASJDE context = new ASJDE();

            IQueryable<EmployeeTypeViewModel> employeeTypeViewModels = (from a in context.SafetyIncidentEmployeeTypes
                                                                        select new EmployeeTypeViewModel
                                                                   {
                                                                       ID = a.ID,
                                                                       Name = a.Name
                                                                   }
                                                             );

            return employeeTypeViewModels;
        }

        #endregion


        public void SendEmail(SafetyIncidentViewModel safetyIncidentViewModel)
        {
            try
            {


                User user = Session["user"] as User;

                string contactMessage = ConfigurationManager.AppSettings.Get("SafetyContactMessage");

                ASJDE context = new ASJDE();

                SafetyIncidentEmail safetyIncidentEmail = (from si in context.SafetyIncidents
                                                           where si.ID == safetyIncidentViewModel.ID
                                                           select new SafetyIncidentEmail
                                                               {
                                                                   ToAddress = user.Email,
                                                                   CreatedBy = si.CreatedByUser,
                                                                   DateOfIncident = si.DateOfIncident,
                                                                   Country = si.Country.Name,
                                                                   Location = si.Location.Name,
                                                                   EmployeeType = si.SafetyIncidentEmployeeType.Name,
                                                                   WorkRelated = si.WorkRelated,
                                                                   DescriptionOfIncident = si.Description,
                                                                   Outcome = si.SafetyIncidentOutcome.Name,
                                                                   UnsafeActCondition = si.SafetyIncidentUnsafeAct.Name,
                                                                   RootCase = si.RootCause,
                                                                   Recordable = si.Recordable,
                                                                   ContactMessage = contactMessage,
                                                                   Status = si.SafetyIncidentStatus.Name,
                                                                   ID = si.ID,
                                                                   EmailMessage = safetyIncidentViewModel.EmailMessage
                                                               }).FirstOrDefault();

                _emailService.SafetyIncident(safetyIncidentEmail);

            }
            catch (Exception e)
            {
                logger.Error("Exception", e);

            }

        }
    }
}

