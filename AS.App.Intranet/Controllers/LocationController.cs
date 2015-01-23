using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AS.App.Intranet.ViewModels.Company;
using AS.App.Intranet.ViewModels.SafetyIncident;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using WS.Framework.ASJDE;

namespace AS.App.Intranet.Controllers
{
    public class LocationController : Controller
    {
        public ActionResult LocationRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllLocations().ToDataSourceResult(request));
        }

        public void PopulateLocations()
        {
            IQueryable<LocationViewModel> locations = GetAllLocations();

            ViewData["locations"] = locations;
        }

        public JsonResult ReadAllLocations()
        {
            return Json(GetAllLocations(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<LocationViewModel> GetAllLocations()
        {
            ASJDE context = new ASJDE();

            IQueryable<LocationViewModel> locationViewModels = (from a in context.Locations
                                                                select new LocationViewModel
                                                                {
                                                                    ID = a.ID,
                                                                    LocationStatusID = a.LocationStatusID,
                                                                    CurrencyID = a.CountryID,
                                                                    CountryID = a.CountryID,
                                                                    Name = a.Name,
                                                                    Address1 = a.Address1,
                                                                    Address2 = a.Address2,
                                                                    City = a.City,
                                                                    State = a.State,
                                                                    PostalCode = a.PostalCode,
                                                                    Description = a.Description,
                                                                    Status = a.LocationStatus.Name
                                                                }
                                                             ).OrderBy(x => x.Name);

            return locationViewModels;
        }

        public IQueryable<LocationViewModel> GetAllLocationsWithStatus()
        {
            ASJDE context = new ASJDE();

            IQueryable<LocationViewModel> locationViewModels = (from a in context.Locations
                                                                select new LocationViewModel
                                                                {
                                                                    ID = a.ID,
                                                                    LocationStatusID = a.LocationStatusID,
                                                                    CurrencyID = a.CountryID,
                                                                    CountryID = a.CountryID,
                                                                    Name = a.Name + (a.LocationStatusID == 2 ? " : " + a.LocationStatus.Name : ""),
                                                                    Address1 = a.Address1,
                                                                    Address2 = a.Address2,
                                                                    City = a.City,
                                                                    State = a.State,
                                                                    PostalCode = a.PostalCode,
                                                                    Description = a.Description,
                                                                    Status = a.LocationStatus.Name
                                                                }
                                                             ).OrderBy(x => x.Name);

            return locationViewModels;
        }

        public JsonResult GetLocationsFilteredByCountryID(decimal countryID)
        {
            var locations = GetLocationsByCountryID(countryID);

            return Json(locations.Select(c => new { Name = String.Format("{0}{1}", c.Name, (c.LocationStatusID == 2 ? " : " + c.Status : "")), ID = c.ID }), JsonRequestBehavior.AllowGet);
        }

        public static List<LocationViewModel> GetLocationsByCountryID(decimal countryID)
        {
            ASJDE context = new ASJDE();

            List<LocationViewModel> locationViewModels = (from a in context.Locations
                                                          where a.CountryID == countryID
                                                          select new LocationViewModel
                                                          {
                                                              ID = a.ID,
                                                              LocationStatusID = a.LocationStatusID,
                                                              CurrencyID = a.CountryID,
                                                              CountryID = a.CountryID,
                                                              Name = a.Name,
                                                              Address1 = a.Address1,
                                                              Address2 = a.Address2,
                                                              City = a.City,
                                                              State = a.State,
                                                              PostalCode = a.PostalCode,
                                                              Description = a.Description,
                                                              Status = a.LocationStatus.Name
                                                          }
                                                         ).OrderBy(x => x.Name).ToList();

            return locationViewModels;

        }

    }
}
