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
using AS.App.Intranet.ViewModels.HoursWorked;
using AS.App.Intranet.ViewModels.SafetyIncident;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using WS.Framework.ASJDE;
using WS.Framework.Objects.ActiveDirectory;
using WS.Framework.ServicesInterface;

namespace AS.App.Intranet.Controllers
{
    public class HoursWorkedController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IHierarchyService _hierarchyService;
        private readonly ILDAPService _ldapService;

        public HoursWorkedController(ILDAPService ldapService, IHierarchyService hierarchyService)
        {
            _hierarchyService = hierarchyService;
            _ldapService = ldapService;
        }

        [Authorize(Roles = "AS SG Hours Worked User")]
        public ActionResult Index()
        {
            User user = new User();

            user = (User)Session["user"];

            HoursViewModel hoursViewModel = new HoursViewModel
                {
                    UserName = user.LastName + ", " + user.FirstName
                };

            HierarchyController hierarchyController = new HierarchyController();

            LocationController locationController = new LocationController();

            IQueryable<CountryViewModel> countryViewModels = hierarchyController.GetAllCountries();
            ViewData["countries"] = countryViewModels;

            IQueryable<LocationViewModel> locationViewModels = locationController.GetAllLocations();
            ViewData["locations"] = locationViewModels;

            IQueryable<EntityViewModel> entityViewModels = hierarchyController.GetAllEntitiesWithDetails();
            ViewData["entityViewModels"] = entityViewModels;

            return View(hoursViewModel);
        }

        public ActionResult AddMonthLocation()
        {
            AddMonthLocationViewModel addMonthLocationViewModel = new AddMonthLocationViewModel();

            return View(addMonthLocationViewModel);
        }

        [HttpPost]
        public ActionResult AddMonthLocation(AddMonthLocationViewModel addMonthLocationViewModel)
        {
            if (ModelState.IsValid)
            {
                AddNewMonth(addMonthLocationViewModel);

                return Json(new { success = true });
            }

            return View(addMonthLocationViewModel);
        }

        public ActionResult AddMonth()
        {
            AddMonthViewModel addMonthViewModel = new AddMonthViewModel();

            return View(addMonthViewModel);
        }

        [HttpPost]
        public ActionResult AddMonth(AddMonthViewModel addMonthViewModel)
        {
            if (ModelState.IsValid)
            {
                AddNewMonth(addMonthViewModel);

                return Json(new { success = true });
            }

            return View(addMonthViewModel);
        }


        public ActionResult HoursWorkedRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllHoursWorked().ToDataSourceResult(request));
        }

        public ActionResult FilterMenuCustomization_Years()
        {
            return Json(FilteredYears(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterMenuCustomization_Months()
        {
            return Json(FilteredMonths(), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HoursWorkedUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, HoursWorkedViewModel hoursWorkedViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        var user = Session["user"] as User;

                        HoursWorked hoursWorked = (from hw in context.HoursWorkeds
                                                   where hw.ID == hoursWorkedViewModel.ID
                                                   select hw).FirstOrDefault();

                        hoursWorked.Hours = hoursWorkedViewModel.Hours;
                        hoursWorked.EntityID = hoursWorkedViewModel.EntityID;
                        hoursWorked.EditedByUserName = user.UserName;
                        hoursWorked.EditedByDate = DateTime.Now;
                        hoursWorked.EditedByName = user.LastName + ", " + user.FirstName;

                        context.SaveChanges();

                        hoursWorkedViewModel.EditedBy = user.LastName + ", " + user.FirstName;

                    }
                }
            }
            catch (DbEntityValidationException)
            {
                return null;
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
            return Json(new[] { hoursWorkedViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HoursWorkedDestroy([DataSourceRequest] DataSourceRequest request,
                                               HoursWorkedViewModel hoursWorkedViewModel)
        {
            try
            {
                if (hoursWorkedViewModel != null)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        HoursWorked hoursWorked = (from hw in context.HoursWorkeds
                                                   where hw.ID == hoursWorkedViewModel.ID
                                                   select hw).FirstOrDefault();

                        context.HoursWorkeds.Remove(hoursWorked);
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

        public IQueryable<HoursWorkedViewModel> GetAllHoursWorked()
        {

            ASJDE context = new ASJDE();

            IQueryable<HoursWorkedViewModel> hoursWorkedViewModels = (from hw in context.HoursWorkeds
                                                                      select new HoursWorkedViewModel
                                                                          {
                                                                              ID = hw.ID,
                                                                              LocationID = hw.LocationID,
                                                                              CountryID = hw.CountryID,
                                                                              Year = hw.Year,
                                                                              Month = hw.Month,
                                                                              Hours = hw.Hours,
                                                                              CreatedBy = hw.CreatedBy,
                                                                              EntityID = hw.EntityID,
                                                                              EditedBy = hw.EditedByName
                                                                          });

            return hoursWorkedViewModels;

        }

        public JsonResult GetAddMonthYear(int countryID)
        {
            return Json(YearFiltered(countryID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAddMonthMonth(int countryID, int year)
        {
            return Json(MonthFiltered(countryID, year), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAddMonthYearLocation(int countryID, int locationID)
        {
            return Json(Year(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAddMonthMonthLocation(int countryID, int locationID, int year)
        {
            return Json(Month(), JsonRequestBehavior.AllowGet);
        }

        public void AddNewMonth(AddMonthViewModel addMonthViewModel)
        {
            try
            {
                using (ASJDE context = new ASJDE())
                {
                    var user = Session["user"] as User;

                    List<decimal> locations = (from l in context.Locations
                                               where l.CountryID == addMonthViewModel.CountryID
                                               && l.LocationStatusID == 1
                                               select l.ID).ToList();

                    List<decimal> existingLocations = (from h in context.HoursWorkeds
                                                       where
                                                           h.CountryID == addMonthViewModel.CountryID &&
                                                           h.Month == addMonthViewModel.Month &&
                                                           h.Year == addMonthViewModel.Year
                                                       select h.LocationID).ToList();

                    var locationsThatNeedAdded = locations.Except(existingLocations);


                    foreach (var location in locationsThatNeedAdded)
                    {
                        HoursWorked hoursWorked = new HoursWorked
                            {
                                CountryID = addMonthViewModel.CountryID,
                                LocationID = location,
                                Year = addMonthViewModel.Year,
                                Month = addMonthViewModel.Month,
                                CreatedByDate = DateTime.Now,
                                CreatedBy = user.LastName + ", " + user.FirstName,
                                CreatedByUserName = user.UserName
                            };
                        context.HoursWorkeds.Add(hoursWorked);
                    }
                    context.SaveChanges();

                }
            }
            catch (DbEntityValidationException dbEx)
            {
                logger.Error("Exception", dbEx);
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
        }

        public void AddNewMonth(AddMonthLocationViewModel addMonthLocationViewModel)
        {
            try
            {
                using (ASJDE context = new ASJDE())
                {
                    var user = Session["user"] as User;


                    HoursWorked hoursWorked = new HoursWorked
                        {
                            CountryID = addMonthLocationViewModel.CountryID,
                            LocationID = addMonthLocationViewModel.LocationID,
                            Year = addMonthLocationViewModel.Year,
                            Month = addMonthLocationViewModel.Month,
                            CreatedByDate = DateTime.Now,
                            CreatedBy = user.LastName + ", " + user.FirstName,
                            CreatedByUserName = user.UserName
                        };
                    context.HoursWorkeds.Add(hoursWorked);

                    context.SaveChanges();

                }
            }
            catch (DbEntityValidationException dbEx)
            {
                logger.Error("Exception", dbEx);
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
        }

        public static List<Year> YearFiltered(int countryID, int locationID)
        {
            List<Year> allYears = Year();
            List<Year> years = new List<Year>();

            try
            {
                using (ASJDE context = new ASJDE())
                {
                    foreach (Year allYear in allYears)
                    {
                        List<int> monthsInThisYear = (from i in context.HoursWorkeds
                                                      where i.Year == allYear.ID && i.CountryID == countryID && i.LocationID == locationID
                                                      select i.Month).Distinct().ToList();

                        List<int> allMonths = new List<int>();

                        for (int i = 1; i < 13; i++)
                        {
                            allMonths.Add(i);
                        }

                        IEnumerable<int> x = allMonths.Except(monthsInThisYear);

                        if (x.Any())
                        {
                            years.Add(allYear);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }

            return years;
        }

        public static List<Year> YearFiltered(int countryID)
        {
            List<Year> allYears = Year();
            List<Year> years = new List<Year>();

            try
            {
                using (ASJDE context = new ASJDE())
                {
                    foreach (Year allYear in allYears)
                    {


                        int countOfLocationsInCountry = (from l in context.Locations
                                                         where l.CountryID == countryID && l.LocationStatusID == 1
                                                         select l).Count();

                        var y = (from h in context.HoursWorkeds
                                 where h.CountryID == countryID
                                       && h.Year == allYear.ID
                                 group h by h.Month
                                     into grp
                                     select new
                                     {
                                         Month = grp.Key,
                                         Locations = grp.Select(l => l.LocationID)
                                     }).ToList();

                        List<int> monthsInThisYear = new List<int>();

                        foreach (var z in y)
                        {
                            int countOfDistinct = z.Locations.Distinct().Count();
                            if (countOfDistinct == countOfLocationsInCountry)
                            {
                                monthsInThisYear.Add(z.Month);
                            }
                        }

                        List<int> allMonths = new List<int>();

                        for (int i = 1; i < 13; i++)
                        {
                            allMonths.Add(i);
                        }

                        IEnumerable<int> x = allMonths.Except(monthsInThisYear);

                        if (x.Any())
                        {
                            years.Add(allYear);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }

            return years;
        }

        public static List<Month> MonthFiltered(int countryID, int year)
        {
            List<Month> allMonths = Month();

            List<Month> months = new List<Month>();

            try
            {
                using (ASJDE context = new ASJDE())
                {
                    int countOfLocationsInCountry = (from l in context.Locations
                                                     where l.CountryID == countryID && l.LocationStatusID == 1
                                                     select l).Count();

                    var y = (from h in context.HoursWorkeds
                             where h.CountryID == countryID
                                   && h.Year == year
                             group h by h.Month
                                 into grp
                                 select new
                                 {
                                     Month = grp.Key,
                                     Locations = grp.Select(l => l.LocationID)
                                 }).ToList();

                    //List<int> monthsInThisYear = (from i in context.HoursWorkeds
                    //                                  where i.CountryID == countryID && i.Year == year
                    //                                  select i.Month).ToList();

                    List<int> monthsInThisYear = new List<int>();

                    foreach (var z in y)
                    {
                        int countOfDistinct = z.Locations.Distinct().Count();
                        if (countOfDistinct == countOfLocationsInCountry)
                        {
                            monthsInThisYear.Add(z.Month);
                        }
                    }

                    months = (from al in allMonths
                              where !monthsInThisYear.Contains(al.ID)
                              select al).ToList();

                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }


            return months;
        }

        public static List<Month> MonthFiltered(int countryID, int locationID, int year)
        {
            List<Month> allMonths = Month();

            List<Month> months = new List<Month>();

            try
            {
                using (ASJDE context = new ASJDE())
                {
                    List<int> monthsInThisYear = (from i in context.HoursWorkeds
                                                  where i.CountryID == countryID && i.Year == year && i.LocationID == locationID
                                                  select i.Month).ToList();

                    months = (from al in allMonths
                              where !monthsInThisYear.Contains(al.ID)
                              select al).ToList();


                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }


            return months;
        }

        public List<int> FilteredYears()
        {
            List<int> years = new List<int>();

            using (ASJDE context = new ASJDE())
            {
                IQueryable<int> x = (from hw in context.HoursWorkeds
                                     select hw.Year).Distinct();

                foreach (var i in x)
                {
                    years.Add(i);
                }
            }

            return years;

        }

        public List<int> FilteredMonths()
        {
            List<int> months = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            return months;

        }

        public static List<Year> Year()
        {
            List<Year> years = new List<Year>();

            DateTime now = DateTime.Now;

            int thisYear = now.Year;

            for (int i = -5; i < 2; i++)
            {
                if ((thisYear + i) > 2012)
                {
                    Year year = new Year { ID = thisYear + i, Name = thisYear + i };

                    years.Add(year);
                }
            }

            years.OrderBy(y => y.ID);

            return years;
        }

        public static List<Month> Month()
        {
            List<Month> months = new List<Month>
                {
                    new Month {ID = 1, Name = "January"},
                    new Month {ID = 2, Name = "February"},
                    new Month {ID = 3, Name = "March"},
                    new Month {ID = 4, Name = "April"},
                    new Month {ID = 5, Name = "May"},
                    new Month {ID = 6, Name = "June"},
                    new Month {ID = 7, Name = "July"},
                    new Month {ID = 8, Name = "August"},
                    new Month {ID = 9, Name = "September"},
                    new Month {ID = 10, Name = "October"},
                    new Month {ID = 11, Name = "November"},
                    new Month {ID = 12, Name = "December"}
                };

            return months;
        }
    }
    public class Year
    {
        public int ID { get; set; }
        public int Name { get; set; }
    }

    public class Month
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
