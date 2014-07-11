using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using WS.App.Intranet.ViewModels.Employee;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.App.Intranet.Controllers
{
    public class EmployeeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Authorize(Roles = "Application User Management")]
        public ActionResult Index()
        {
            PopulateApplications();
            PopulateSecurityLevels();

            return View();
        }

        public void PopulateApplications()
        {
            List<ApplicationViewModel> applicationViewModels = GetAllApplications();

            ViewData["applicationViewModels"] = applicationViewModels;
        }

        public List<ApplicationViewModel> GetAllApplications()
        {

            List<ApplicationViewModel> applicationViewModels = new List<ApplicationViewModel>();

            try
            {
                WSJDE context = new WSJDE();

                applicationViewModels = (from a in context.Applications
                                         where a.Status == 1
                                         select new ApplicationViewModel
                                             {
                                                 ID = a.ID,
                                                 Name = a.Name

                                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                return null;
            }

            return applicationViewModels;
        }

        public void PopulateSecurityLevels()
        {
            List<SecurityLevelViewModel> securityLevelViewModels = GetAllSecurityLevels();
            ViewData["securityLevelViewModels"] = securityLevelViewModels;
        }

        public List<SecurityLevelViewModel> GetAllSecurityLevels()
        {

            List<SecurityLevelViewModel> securityLevelViewModels = new List<SecurityLevelViewModel>();

            try
            {
                WSJDE context = new WSJDE();

                securityLevelViewModels = (from a in context.SecurityLevels
                                           select new SecurityLevelViewModel
                                         {
                                             ApplicationID = a.ApplicationID,
                                             LevelID = a.LevelID,
                                             Description = a.Description

                                         }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                return null;
            }

            return securityLevelViewModels;
        }

        #region Employee

        public ActionResult EmployeeRead([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            return Json(GetAllEmployees().ToDataSourceResult(dataSourceRequest));
        }

        public JsonResult ReadAllEmployees()
        {
            return Json(GetAllEmployees(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<EmployeeViewModel> GetAllEmployees()
        {
            try
            {
                WSJDE context = new WSJDE();

                //IQueryable<EmployeeViewModel> requestSourceViewModels = AutoMapper.Mapper.Map<IQueryable<EmployeeViewModel>>(from e in context.Employees select e);

                IQueryable<EmployeeViewModel> requestSourceViewModels = (from e in context.Employees
                                                                         select new EmployeeViewModel
                                                                             {
                                                                                 ID = e.ID,
                                                                                 EmployeeNumber = e.EmployeeNumber,
                                                                                 FirstName = e.FirstName,
                                                                                 LastName = e.LastName,
                                                                                 Title = e.Title,
                                                                                 EmployeeStatusID = e.EmployeeStatusID
                                                                             });


                return requestSourceViewModels;

            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                return null;
            }


        }
        
        #endregion

        #region Security

        public ActionResult GetSecurityDataByEmployeeID(string employeeID, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            return Json(GetAllRequestDatas()
                .Where(request => request.EmployeeID == employeeID)
                .ToDataSourceResult(dataSourceRequest));
        }

        public JsonResult GetSecurityLevelsFilteredByApplicationID(decimal applicationID)
        {
            return Json(GetSecurityLevelsByApplicationID(applicationID), JsonRequestBehavior.AllowGet);
        }

        public static List<SecurityLevelViewModel> GetSecurityLevelsByApplicationID(decimal applicationID)
        {
            WSJDE context = new WSJDE();

            List<SecurityLevelViewModel> securityLevelViewModels = (from a in context.SecurityLevels
                                                                    where a.ApplicationID == applicationID
                                                                    select new SecurityLevelViewModel
                                                                  {
                                                                      Description = a.Description,
                                                                      LevelID = a.LevelID
                                                                  }
                                                     ).OrderBy(x => x.Description).ToList();

            return securityLevelViewModels;
        }

        public IQueryable<SecurityViewModel> GetAllRequestDatas()
        {
            WSJDE context = new WSJDE();

            IQueryable<SecurityViewModel> requestDataViewModel = (from a in context.Securities
                                                                  select new SecurityViewModel
                                                                     {
                                                                         EmployeeID = a.EmployeeID,
                                                                         ID = a.ID,
                                                                         SecurityLevel = a.SecurityLevel,
                                                                         ApplicationID = a.ApplicationID,
                                                                         Password = a.Password
                                                                     }
                                                                    );

            return requestDataViewModel;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecurityCreate([DataSourceRequest] DataSourceRequest request, EmployeeViewModel requestSourceViewModel)
        {
            try
            {
                if (requestSourceViewModel != null && ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        //APIEmployee apiEmployee = new APIEmployee
                        //{
                        //    ID = requestSourceViewModel.ID,
                        //    Name = requestSourceViewModel.Name
                        //};

                        //context.APIEmployees.Add(apiEmployee);
                        //context.SaveChanges();

                    }
                }

                return Json(new[] { requestSourceViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecurityUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, EmployeeViewModel requestSourceViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {

                        //APIEmployee apiEmployee = (from s in context.APIEmployees
                        //                                     where s.ID == requestSourceViewModel.ID
                        //                                     select s).FirstOrDefault();

                        //if (apiEmployee != null)
                        //{
                        //    apiEmployee.Name = requestSourceViewModel.Name;

                        //    context.SaveChanges();
                        //}

                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
            return Json(new[] { requestSourceViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecurityDestroy([DataSourceRequest] DataSourceRequest request, EmployeeViewModel requestSourceViewModel)
        {
            try
            {
                if (requestSourceViewModel != null)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        //APIEmployee source = (from s in context.APIEmployees
                        //                           where s.ID == requestSourceViewModel.ID
                        //                           select s).FirstOrDefault();
                        //context.APIEmployees.Remove(source);
                        //context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return Json(ModelState.ToDataSourceResult());
        }


        #endregion

    }
}
