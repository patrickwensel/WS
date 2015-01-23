using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using AS.App.Intranet.ViewModels;
using AS.App.Intranet.ViewModels.Company;
using AS.App.Intranet.ViewModels.Hierarchy;
using AS.App.Intranet.ViewModels.SafetyIncident;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using WS.Framework.ASJDE;
using WS.Framework.ServicesInterface;

namespace AS.App.Intranet.Controllers
{
    public class HierarchyController : Controller
    {
        #region Company

        public void PopulateCompanies()
        {
            IEnumerable<CompanyViewModel> companyViewModels = GetAllCompanies();

            ViewData["companies"] = companyViewModels;
        }

        public JsonResult ReadAllCompanies()
        {
            return Json(GetAllCompanies(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<CompanyViewModel> GetAllCompanies()
        {
            ASJDE context = new ASJDE();

            IQueryable<CompanyViewModel> companyViewModels = (from a in context.Companies
                                                              select new CompanyViewModel
                                                              {
                                                                  ID = a.ID,
                                                                  EntityID = a.EntityID.Value,
                                                                  Name = a.Entity.Name
                                                              }
                                                             ).OrderBy(x => x.EntityID);

            return companyViewModels;

        }

        #endregion

        #region Strategic Business Unit

        //public void PopulateStrategicBusinessUnits()
        //{
        //    List<StrategicBusinessUnitViewModel> strategicBusinessUnitViewModels = GetAllStrategicBusinessUnits();

        //    ViewData["strategicBusinessUnits"] = strategicBusinessUnitViewModels;
        //}

        //public JsonResult ReadAllStrategicBusinessUnits()
        //{
        //    return Json(GetAllStrategicBusinessUnits(), JsonRequestBehavior.AllowGet);
        //}

        public static List<StrategicBusinessUnitViewModel> GetStrategicBusinessUnitsByCompanyID(decimal companyID)
        {
            ASJDE context = new ASJDE();

            List<StrategicBusinessUnitViewModel> sbuViewModels = (from a in context.StrategicBusinessUnits
                                                                  where a.CompanyID == companyID
                                                                  select new StrategicBusinessUnitViewModel
                                                                  {
                                                                      ID = a.ID,
                                                                      Name = a.Entity.Name
                                                                  }
                                                     ).OrderBy(x => x.Name).ToList();

            return sbuViewModels;
        }

        public JsonResult GetStrategicBusinessUnitsFilteredByCompanyID(decimal companyID)
        {
            return Json(GetStrategicBusinessUnitsByCompanyID(companyID), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<StrategicBusinessUnitViewModel> GetAllStrategicBusinessUnits()
        {

            ASJDE context = new ASJDE();

            IQueryable<StrategicBusinessUnitViewModel> sbus = (from a in context.StrategicBusinessUnits
                                                         select new StrategicBusinessUnitViewModel
                                                             {
                                                                 ID = a.ID,
                                                                 Name = a.Entity.Name,
                                                                 EntityID = a.EntityID,
                                                                 CompanyID = a.CompanyID,
                                                                 //Entity = a.Entity

                                                             }
                                                       );//.OrderBy(x => x.EntityViewModel.Name);

            return sbus;

        }


        #endregion

        #region Operational Business Unit

        public void PopulateOperationalBusinessUnits()
        {
            IEnumerable<OperationalBusinessUnitViewModel> operataionalBusinessUnitViewModels =
                GetAllOperationalBusinessUnits();

            ViewData["operationalBusinessUnits"] = operataionalBusinessUnitViewModels;
        }

        public JsonResult ReadAllOperationalBusinessUnits()
        {
            return Json(GetAllOperationalBusinessUnits(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOperationalBusinessUnitsFilteredByStrategicBusinessUnitID(decimal strategicBusinessUnitID)
        {
            return Json(GetOperationalBusinessUnitsByStrategicBusinessUnitID(strategicBusinessUnitID), JsonRequestBehavior.AllowGet);
        }

        public List<OperationalBusinessUnitViewModel> GetAllOperationalBusinessUnits()
        {

            ASJDE context = new ASJDE();

            List<OperationalBusinessUnitViewModel> obuViewModels = (from a in context.OperationalBusinessUnits
                                                                          select new OperationalBusinessUnitViewModel
                                                                          {
                                                                              ID = a.ID,
                                                                              CompanyID = a.StrategicBusinessUnit.CompanyID,
                                                                              EntityID = a.EntityID,
                                                                              StrategicBusinessUnitID = a.StrategicBusinessUnitID,
                                                                              Name = a.Entity.Name
                                                                          }
                                                     ).OrderBy(x => x.Name).ToList();

            return obuViewModels;

        }

        public static List<OperationalBusinessUnitViewModel> GetOperationalBusinessUnitsByStrategicBusinessUnitID(decimal strategicBusinessUnitID)
        {
            ASJDE context = new ASJDE();

            List<OperationalBusinessUnitViewModel> sbuViewModels = (from a in context.OperationalBusinessUnits
                                                                    where a.StrategicBusinessUnitID == strategicBusinessUnitID
                                                                    select new OperationalBusinessUnitViewModel
                                                                    {
                                                                        ID = a.ID,
                                                                        Name = a.Entity.Name
                                                                    }
                                               ).OrderBy(x => x.Name).ToList();

            return sbuViewModels;
        }

        #endregion

        #region Business Unit

        public void PopulateBusinessUnits()
        {
            IEnumerable<BusinessUnitViewModel> businessUnitViewModels = GetAllBusinessUnits();

            ViewData["businessUnits"] = businessUnitViewModels;
        }

        public JsonResult ReadAllBusinessUnits()
        {
            return Json(GetAllBusinessUnits(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBusinessUnitsFilteredByOperationalBusinessUnitID(decimal operationalBusinessUnitID)
        {
            return Json(GetBusinessUnitsByOperationalBusinessUnitID(operationalBusinessUnitID), JsonRequestBehavior.AllowGet);
        }

        public static List<BusinessUnitViewModel> GetBusinessUnitsByOperationalBusinessUnitID(decimal OperationalBusinessUnitID)
        {
            ASJDE context = new ASJDE();

            List<BusinessUnitViewModel> buViewModels = (from a in context.BusinessUnits
                                                        where a.OperationalBusinessUnitID == OperationalBusinessUnitID
                                                select new BusinessUnitViewModel
                                                {
                                                    ID = a.ID,
                                                    Name = a.Entity.Name
                                                }
                                               ).OrderBy(x => x.Name).ToList();

            return buViewModels;
        }

        public List<BusinessUnitViewModel> GetAllBusinessUnits()
        {

            ASJDE context = new ASJDE();

            List<BusinessUnitViewModel> buViewModels = (from a in context.BusinessUnits
                                                                          select new BusinessUnitViewModel
                                                                          {
                                                                              ID = a.ID,
                                                                              Name = a.Entity.Name
                                                                          }
                                                     ).OrderBy(x => x.Name).ToList();

            return buViewModels;

        }

        #endregion

        #region Branch

        public void PopulateBranchs()
        {
            IEnumerable<BranchViewModel> branchViewModels = GetAllBranchs();

            ViewData["branchViewModels"] = branchViewModels;
        }

        public JsonResult ReadAllBranchs()
        {
            return Json(GetAllBranchs(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranchsFilteredByBusinessUnitID(decimal businessUnitID)
        {
            return Json(GetBranchsByBusinessUnitID(businessUnitID), JsonRequestBehavior.AllowGet);
        }

        public static List<BranchViewModel> GetBranchsByBusinessUnitID(decimal businessUnitID)
        {
            ASJDE context = new ASJDE();

            List<BranchViewModel> branchViewModels = (from a in context.Branches
                                                      where a.BusinessUnitID == businessUnitID
                                                      select new BranchViewModel
                                                      {
                                                          ID = a.ID,
                                                          Name = a.Entity.Name
                                                      }
                                                     ).OrderBy(x => x.Name).ToList();

            return branchViewModels;
        }


        //public static List<BranchViewModel> GetBranchsByBusinessUnitID(decimal businessUnitID)
        //{
        //    ASJDE context = new ASJDE();

        //    List<BranchViewModel> branchViewModels = (from a in context.Branches
        //                                              where a.BusinessUnitID == businessUnitID
        //                                              select new BranchViewModel
        //                                              {
        //                                                  ID = a.ID,
        //                                                  Name = a.Location.Name
        //                                              }
        //                                             ).ToList();

        //    return branchViewModels;
        //}

        public IQueryable<BranchViewModel> GetAllBranchs()
        {

            ASJDE context = new ASJDE();

            IQueryable<BranchViewModel> branchViewModels = (from a in context.Branches
                                                            select new BranchViewModel
                                                              {
                                                                  ID = a.ID,
                                                                  Name = a.Entity.Name
                                                              }
                                                     ).OrderBy(x => x.Name);

            return branchViewModels;

        }

        #endregion

        #region Depot

        public void PopulateDepots()
        {
            IEnumerable<DepotViewModel> depotViewModels = GetAllDepots();

            ViewData["depotViewModels"] = depotViewModels;
        }

        public JsonResult ReadAllDepots()
        {
            return Json(GetAllDepots(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepotsFilteredByBranchID(decimal branchID)
        {
            return Json(GetDepotsByBranchID(branchID), JsonRequestBehavior.AllowGet);
        }

        public static List<DepotViewModel> GetDepotsByBranchID(decimal branchID)
        {
            ASJDE context = new ASJDE();

            List<DepotViewModel> depotViewModels = (from a in context.Depots
                                                    where a.BranchID == branchID
                                                    select new DepotViewModel
                                                    {
                                                        ID = a.ID,
                                                        Name = a.Entity.Name
                                                    }
                                                   ).OrderBy(x => x.Name).ToList();

            return depotViewModels;
        }

        public IQueryable<DepotViewModel> GetAllDepots()
        {

            ASJDE context = new ASJDE();

            IQueryable<DepotViewModel> depotViewModels = (from a in context.Depots
                                                          select new DepotViewModel
                                                            {
                                                                ID = a.ID,
                                                                Name = a.Entity.Name
                                                            }
                                                     ).OrderBy(x => x.Name);

            return depotViewModels;

        }

        #endregion
        
        #region Country

        public void PopulateCountries()
        {
            IQueryable<CountryViewModel> countries = GetAllCountries();

            ViewData["countries"] = countries;
        }

        public JsonResult ReadAllCountries()
        {
            return Json(GetAllCountries(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReadAllCountriesExceptUS()
        {
            return Json(GetAllCountriesExceptUS(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<CountryViewModel> GetAllCountriesExceptUS()
        {
            ASJDE context = new ASJDE();

            IQueryable<CountryViewModel> countryViewModels = (from a in context.Countries
                                                              where a.ID != 4
                                                              select new CountryViewModel
                                                              {
                                                                  ID = a.ID,
                                                                  Name = a.Name
                                                              }
                                                              ).OrderBy(x => x.Name);

            return countryViewModels;
        }

        public IQueryable<CountryViewModel> GetAllCountries()
        {
            ASJDE context = new ASJDE();

            IQueryable<CountryViewModel> countryViewModels = (from a in context.Countries
                                                              select new CountryViewModel
                                                              {
                                                                  ID = a.ID,
                                                                  Name = a.Name
                                                              }
                                                             ).OrderBy(x => x.Name);

            return countryViewModels;
        }

        #endregion

        #region Entity

        public void PopulateEntities()
        {
            List<EntityViewModel> entityViewModels = GetAllEntities();

            ViewData["entityViewModels"] = entityViewModels;
        }

        public JsonResult ReadAllEntities()
        {
            return Json(GetAllEntities(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReadAllEntityStatuss()
        {
            return Json(GetAllEntityStatuss(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEntitiesFilteredByLocationIDHours(decimal locationID)
        {
            var entityViewModels = GetEntitiesByLocationIDHours(locationID);

            return
                Json(
                    entityViewModels.Select(
                    c => new { Name = String.Format("{0} - {1}{2}{3}", c.Name, c.Code, (c.CostCenter != null ? ": " + c.CostCenter:null) , (c.StatusID == 2 ? " - " + c.Status : "")), ID = c.ID }),
                    JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEntitiesFilteredByLocationIDSafety(decimal locationID)
        {
            var entityViewModels = GetEntitiesByLocationIDSafety(locationID);

            return
                Json(
                    entityViewModels.Select(
                    c => new { Name = String.Format("{0} - {1}{2}{3}", c.Name, c.Code, (c.CostCenter != null ? ": " + c.CostCenter : null), (c.StatusID == 2 ? " - " + c.Status : "")), ID = c.ID }),
                    JsonRequestBehavior.AllowGet);
        }

        public IQueryable<EntityViewModel> GetAllEntitiesWithDetails()
        {
            ASJDE context = new ASJDE();

            IQueryable<EntityViewModel> entityViewModels = (from a in context.Entities
                                                            select new EntityViewModel
                                                            {
                                                                ID = a.ID,
                                                                Name = a.Name + "- " + a.Code + (a.CostCenter != null ? ": " + a.CostCenter : null) + (a.StatusID == 2 ? " - " + a.EntityStatus.Name : ""),
                                                                LocationID = a.LocationID,
                                                                StatusID = a.StatusID,
                                                                Code = a.Code,
                                                                CostCenter = a.CostCenter ?? "",
                                                                Status = a.EntityStatus.Name
                                                            }
                                                           ).OrderBy(x => x.Name);
            return entityViewModels;
        }

        public static List<EntityViewModel> GetEntitiesByLocationIDHours(decimal locationID)
        {
            ASJDE context = new ASJDE();

            string hoursAllowInactiveEntities = ConfigurationManager.AppSettings.Get("HoursAllowInactiveEntities");

            IQueryable<EntityViewModel> entityViewModels = (from a in context.Entities
                                                      where a.LocationID == locationID
                                                      select new EntityViewModel
                                                          {
                                                              ID = a.ID,
                                                              Name = a.Name,
                                                              LocationID = a.LocationID,
                                                              StatusID = a.StatusID,
                                                              Code = a.Code,
                                                              CostCenter = a.CostCenter ?? "",
                                                              Status = a.EntityStatus.Name
                                                          }
                                                     ).OrderBy(x => x.Name);


                if (hoursAllowInactiveEntities == "No")
                {
                    entityViewModels = entityViewModels.Where(x => x.StatusID != 2);
                }

            return entityViewModels.ToList();
        }

        public static List<EntityViewModel> GetEntitiesByLocationIDSafety(decimal locationID)
        {
            ASJDE context = new ASJDE();

            string safetyAllowInactiveEntities = ConfigurationManager.AppSettings.Get("SafetyAllowInactiveEntities");

            IQueryable<EntityViewModel> entityViewModels = (from a in context.Entities
                                                            where a.LocationID == locationID
                                                            select new EntityViewModel
                                                            {
                                                                ID = a.ID,
                                                                Name = a.Name,
                                                                LocationID = a.LocationID,
                                                                StatusID = a.StatusID,
                                                                Code = a.Code,
                                                                CostCenter = a.CostCenter ?? "",
                                                                Status = a.EntityStatus.Name
                                                            }
                                                     ).OrderBy(x => x.Name);


            if (safetyAllowInactiveEntities == "No")
            {
                entityViewModels = entityViewModels.Where(x => x.StatusID != 2);
            }

            return entityViewModels.ToList();
        }

        public List<EntityViewModel> GetAllEntities()
        {

            ASJDE context = new ASJDE();

            List<EntityViewModel> entityViewModels = (from a in context.Entities
                                                            select new EntityViewModel
                                                                {
                                                                    ID = a.ID,
                                                                    Name = a.Name,
                                                                    LocationID = a.LocationID,
                                                                    StatusID = a.StatusID,
                                                                    Code = a.Code,
                                                                    CostCenter = a.CostCenter ?? "",
                                                                    Status = a.EntityStatus.Name
                                                                }
                                                           ).OrderBy(x => x.Name).ToList();

            return entityViewModels;

        }

        public List<EntityStatusViewModel> GetAllEntityStatuss()
        {

            ASJDE context = new ASJDE();

            List<EntityStatusViewModel> entityStatusViewModels = (from a in context.EntityStatus
                                                      select new EntityStatusViewModel
                                                      {
                                                          ID = a.ID,
                                                          Name = a.Name
                                                      }
                                                           ).OrderBy(x => x.Name).ToList();

            return entityStatusViewModels;

        }

        #endregion

        #region Locations

        public void PopulateLocations()
        {
            IQueryable<LocationViewModel> locationViewModels = GetAllLocations();

            ViewData["locationViewModels"] = locationViewModels;
        }

        public ActionResult ReadAllLocations([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllLocations(), JsonRequestBehavior.AllowGet);
        }
        
        public IQueryable<LocationViewModel> GetAllLocations()
        {
            ASJDE context = new ASJDE();

            var locationViewModels = (from l in context.Locations
                                      select new LocationViewModel
                                      {
                                          ID = l.ID,
                                          LocationStatusID = l.LocationStatusID,
                                          CurrencyID = l.CurrencyID,
                                          CountryID = l.CountryID,
                                          Name = l.Name,
                                          Address1 = l.Address1,
                                          Address2 = l.Address2,
                                          City = l.City,
                                          State = l.State,
                                          PostalCode = l.PostalCode,
                                          Description = l.Description,
                                      });

            return locationViewModels;
        }

        public IQueryable<LocationStatusViewModel> GetAllLocationStatuss()
        {
            ASJDE context = new ASJDE();

            IQueryable<LocationStatusViewModel> locationStatusViewModels = (from a in context.LocationStatus
                                                                            select new LocationStatusViewModel
                                                                            {
                                                                                ID = a.ID,
                                                                                Name = a.Name
                                                                            }
                                                                 );

            return locationStatusViewModels;
        }

        #endregion


    }
}
