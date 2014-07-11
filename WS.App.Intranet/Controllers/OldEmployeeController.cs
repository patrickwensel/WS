//using System;
//using System.Collections.Generic;
//using System.Data.Objects;
//using System.Linq;
//using System.Threading;
//using System.Web.Mvc;
//using MvcContrib.UI.Grid;
//using NLog;
//using WS.App.Intranet.Models;
//using WS.App.Intranet.ViewModels;
//using WS.App.Tools.Common;
//using WS.App.Tools.Models;
//using WS.Framework.Data;
//using WS.Framework.Objects.Security;
//using WS.Framework.ServicesInterface;
//using WS.Framework.ServicesInterfaceImplementation;

//namespace WS.App.Intranet.Controllers
//{
//    public class OldEmployeeController : Controller
//    {
//        private static Logger logger = LogManager.GetCurrentClassLogger();

//        private readonly IEmployeeService _employeeService;
//        private readonly ISecurityService _securityService;
//        private readonly IDemoService _demoService;

//        public EmployeeController(IEmployeeService employeeService, ISecurityService securityService, IDemoService demoService)
//        {
//            _employeeService = employeeService;
//            _securityService = securityService;
//            _demoService = demoService;
//        }

//        public ActionResult Index(string searchWord, string employeeStatusID, GridSortOptions gridSortOptions, int? page)
//        {
//            if (employeeStatusID == null)
//            {
//                employeeStatusID = "A";
//            }

//            PagedViewModel<Employee> pagedViewModel = SingleWordSearch(searchWord, employeeStatusID, gridSortOptions, page);

//            return View(pagedViewModel);

//        }

//        public ActionResult Details(string id)
//        {
//            SecurityWeb securityWeb = _securityService.GetSecurityWebByEmployeeID(id);

//            SecurityWebViewModel securityWebViewModel = DTOSecurityWebViewModel(securityWeb);

//            securityWebViewModel = AddSecurityLevelsSelectList(securityWebViewModel);

//            ViewBag.RouteDicForList = Request.QueryString.ToRouteDic();
//            return View(securityWebViewModel);
//        }

//        [HttpPost]
//        public ActionResult Details(SecurityWebViewModel securityWebViewModel)
//        {
//            TempData["securityWebViewModel"] = securityWebViewModel;

//            SecurityWeb securityWeb = DTOSecurityWeb(securityWebViewModel);

//            _securityService.UpdateSecurityWeb(securityWeb);

//            return RedirectToAction("Details");
//        }

//        public SecurityWebViewModel AddSecurityLevelsSelectList(SecurityWebViewModel securityWebViewModel)
//        {
//            foreach (SecurityApplicationViewModel securityApplicationViewModel in securityWebViewModel.SecurityApplicationViewModels)
//            {
//                List<SelectListItem> selectListItems = new List<SelectListItem>();

//                foreach (SecurityLevel securityLevel in securityApplicationViewModel.SecurityLevels)
//                {
//                    selectListItems.Add(new SelectListItem {Value = securityLevel.LevelID.ToString(), Text = securityLevel.Description});
//                }

//                securityApplicationViewModel.SecurityLevelsSelectList = selectListItems;
//            }

//            return securityWebViewModel;
//        }

//        public PagedViewModel<Employee> SingleWordSearch(string searchWord, string employeeStatusID, GridSortOptions gridSortOptions, int? page)
//        {
//            if (searchWord != null)
//            {
//                searchWord = CheckIfStringIsCapitalized(searchWord);
//            }

//            PagedViewModel<Employee> pagedViewModel = new PagedViewModel<Employee>
//            {
//                ViewData = ViewData,
//                Query = _employeeService.GetEmployee(),
//                GridSortOptions = gridSortOptions,
//                DefaultSortColumn = "LastName",
//                Page = page,
//                PageSize = 10,
//            }
//            .AddFilter("searchWord", searchWord, a => a.FirstName.Contains(searchWord) || a.LastName.Contains(searchWord))
//            .AddFilter("employeeStatusID", employeeStatusID, a => a.EmployeeStatusID == employeeStatusID, _employeeService.GetEmployeeStatus(), "Status")
//            .Setup();

//            return pagedViewModel;
//        }

//        public string CheckIfStringIsCapitalized(string stringToCapitalize)
//        {
//            if (!char.IsUpper(stringToCapitalize[0]))
//            {
//                stringToCapitalize = char.ToUpper(stringToCapitalize[0]) + stringToCapitalize.Substring(1);
//            }
//            return stringToCapitalize;
//        }

//        public SecurityWebViewModel DTOSecurityWebViewModel(SecurityWeb securityWeb)
//        {
//            SecurityWebViewModel securityWebViewModel =
//                AutoMapper.Mapper.Map<SecurityWeb, SecurityWebViewModel>(securityWeb);
//            securityWebViewModel.EmployeeViewModel =
//                AutoMapper.Mapper.Map<Employee, EmployeeViewModel>(securityWeb.Employee);
//            securityWebViewModel.SecurityApplicationViewModels = new List<SecurityApplicationViewModel>();

//            foreach (SecurityApplication securityApplication in securityWeb.SecurityApplications)
//            {
//                SecurityApplicationViewModel securityApplicationViewModel =
//                    AutoMapper.Mapper.Map<SecurityApplication, SecurityApplicationViewModel>(securityApplication);

//                securityWebViewModel.SecurityApplicationViewModels.Add(securityApplicationViewModel);
//            }

//            return securityWebViewModel;
//        }

//        public SecurityWeb DTOSecurityWeb(SecurityWebViewModel securityWebViewModel)
//        {
//            SecurityWeb securityWeb = AutoMapper.Mapper.Map<SecurityWebViewModel, SecurityWeb>(securityWebViewModel);
//            securityWeb.Employee =
//                AutoMapper.Mapper.Map<EmployeeViewModel, Employee>(securityWebViewModel.EmployeeViewModel);
//            securityWeb.SecurityApplications = new List<SecurityApplication>();

//            foreach (SecurityApplicationViewModel securityApplicationViewModel in securityWebViewModel.SecurityApplicationViewModels)
//            {
//                SecurityApplication securityApplication =
//                    AutoMapper.Mapper.Map<SecurityApplicationViewModel, SecurityApplication>(
//                        securityApplicationViewModel);
//                securityWeb.SecurityApplications.Add(securityApplication);
//            }
            
//            return securityWeb;
//        }

//        [HttpPost]
//        public JsonResult StudentList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
//        {
//            try
//            {
//                var studentCount = 200;
//                var students = _demoService.GetStudents(jtStartIndex, jtPageSize, jtSorting);
//                return Json(new { Result = "OK", Records = students, TotalRecordCount = studentCount });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { Result = "ERROR", Message = ex.Message });
//            }
//        }

//        [HttpPost]
//        public JsonResult GetCityOptions()
//        {
//            try
//            {
//                var cities = _demoService.GetAllCities().Select(c => new { DisplayText = c.CityName, Value = c.CityId });
//                return Json(new { Result = "OK", Options = cities });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { Result = "ERROR", Message = ex.Message });
//            }
//        }
//    }
//}
