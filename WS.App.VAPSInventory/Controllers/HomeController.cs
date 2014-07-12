using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using RazorPDF;
using WS.App.VAPSInventory.Models;
using WS.App.VAPSInventory.Objects;
using WS.App.VAPSInventory.ViewModels;
using WS.Framework.ServicesInterface;
using System.Data.Entity;
using WS.Framework.WSJDEData;

namespace WS.App.VAPSInventory.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly IEmployeeService _employeeService;
        private readonly ISecurityService _securityService;
        //private readonly IInventoryService _inventoryService;

        public HomeController(IProductCategoryService productCategoryService, IProductService productService,
                              IEmployeeService employeeService, ISecurityService securityService)
                              //IInventoryService inventoryService)
        {
            _productCategoryService = productCategoryService;
            _productService = productService;
            _employeeService = employeeService;
            _securityService = securityService;
            //_inventoryService = inventoryService;
        }

        private static InventoryViewModel CurrentInventory { get; set; }

        public static InventoryViewModel GetNewInventory()
        {
            InventoryViewModel inventoryViewModel = new InventoryViewModel();

            return inventoryViewModel;
        }

        [Authorize(Roles = "VAPS Inventory User")]
        public ActionResult Index(AddMonthViewModel addMonthViewModel)
        {
            if (addMonthViewModel.LocationNumber == 0 || addMonthViewModel.LocationNumber == null)
            {
                InventoryViewModel inventoryViewModel = GetInitialInventoryViewModel();
                CurrentInventory = inventoryViewModel;
            }

            return View(CurrentInventory);

        }

        [Authorize(Roles = "VAPS Inventory User")]
        public ActionResult AddMonth(int? id)
        {
            AddMonthViewModel addMonthViewModel = new AddMonthViewModel
                {
                    LocationNumber = id
                };

            return View(addMonthViewModel);
        }



        [HttpPost]
        public ActionResult AddMonth(AddMonthViewModel addMonthViewModel)
        {
            if (ModelState.IsValid)
            {
                AddNewMonthByBranchYearMonth(addMonthViewModel);

                return Json(new { success = true });
            }
            return View(addMonthViewModel);
        }

        [HttpPost]
        public ActionResult GetReportPath(ReportMonthViewModel reportMonthViewModel)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string baseURL = ConfigurationManager.AppSettings.Get("BaseURL");
            string reportType = ConfigurationManager.AppSettings.Get("p_REPORT_TYPE");
            string camUsername = ConfigurationManager.AppSettings.Get("CAMUsername");
            string camPassword = ConfigurationManager.AppSettings.Get("CAMPassword");
            string camNamespace = ConfigurationManager.AppSettings.Get("CAMNamespace");

            stringBuilder.Append(baseURL);
            //stringBuilder.Append("p_pBranch=" + reportMonthViewModel.LocationNumber + "&");
            stringBuilder.Append("p_pBranch=&");
            stringBuilder.Append("p_pYear=" + reportMonthViewModel.Year + "&");
            stringBuilder.Append("p_pMonth=" + reportMonthViewModel.Month + "&");
            stringBuilder.Append("p_REPORT_TYPE=" + reportType + "&");
            stringBuilder.Append("CAMUsername=" + camUsername + "&");
            stringBuilder.Append("CAMPassword=" + camPassword + "&");
            stringBuilder.Append("CAMNamespace=" + camNamespace);

            string reportPath = stringBuilder.ToString();
            return Json(reportPath, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReadInventoryProducts([DataSourceRequest] DataSourceRequest request)
        {
            InventoryProductDataSourceRequest inventoryProductDataSourceRequest = new InventoryProductDataSourceRequest();
            TryUpdateModel(inventoryProductDataSourceRequest);

            return Json(GetInventoryProducts(inventoryProductDataSourceRequest).ToDataSourceResult(request));
        }

        public ActionResult InventorySheet()
        {
            InventorySheetViewModel inventorySheetViewModels = new InventorySheetViewModel
            {
                InventorySheetRowViewModels = GetActiveInventorySheetRowViewModel()
            };

            var pdf = new PdfResult(inventorySheetViewModels, "InventorySheet");

            return pdf;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateInventoryProducts([DataSourceRequest] DataSourceRequest dataSourceRequest, InventoryProductViewModel inventoryProductViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        InventoryProduct inventoryProduct = (from ip in context.InventoryProducts
                                                             where ip.ID == inventoryProductViewModel.ID
                                                             select ip).FirstOrDefault();
                        inventoryProduct.Usable = inventoryProductViewModel.Usable;
                        inventoryProduct.Repairable = inventoryProductViewModel.Repairable;
                        inventoryProduct.ModifiedTime = DateTime.Now;
                        inventoryProduct.ModifiedByUser = DataService.GetUserName();

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return Json(ModelState.ToDataSourceResult());
        }


        public JsonResult GetBranches()
        {
            return Json(DataService.Branches(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetYear()
        {
            return Json(DataService.Year(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAddMonthYear(int locationNumber)
        {
            return Json(DataService.YearFiltered(locationNumber), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMonth()
        {
            return Json(DataService.Month(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAddMonthMonth(int locationNumber, int year)
        {
            return Json(DataService.MonthFiltered(locationNumber, year), JsonRequestBehavior.AllowGet);
        }

        #region Methods

        public static List<InventorySheetRowViewModel> GetActiveInventorySheetRowViewModel()
        {
            List<InventorySheetRowViewModel> inventorySheetRowViewModels = new List<InventorySheetRowViewModel>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    inventorySheetRowViewModels = (from p in context.Products
                                                   join pc in context.ProductCategories on p.ProductCategoryID equals
                                                       pc.ID
                                                   where p.StatusID == 1 && pc.StatusID == 1
                                                   select new InventorySheetRowViewModel
                                                   {
                                                       ProductCategory = pc.Name,
                                                       Product = p.Name
                                                   }
                                                  ).OrderBy(x => x.ProductCategory).ThenBy(x => x.Product).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return inventorySheetRowViewModels;
        }

        private IQueryable<InventoryProductViewModel> GetInventoryProducts(InventoryProductDataSourceRequest inventoryProductDataSourceRequest)
        {

            WSJDE context = new WSJDE();

            decimal inventoryID = (from i in context.Inventories
                               where i.LocationNumber == inventoryProductDataSourceRequest.LocationNumber
                                     && i.Year == inventoryProductDataSourceRequest.Year
                                     && i.Month == inventoryProductDataSourceRequest.Month
                               select i.ID
                  ).FirstOrDefault();

            IQueryable<InventoryProductViewModel> inventoryProducts = (
                                                                          from ip in context.InventoryProducts
                                                                                            .Include(p => p.Product)
                                                                                            .Include(
                                                                                                pc =>
                                                                                                pc.Product
                                                                                                  .ProductCategory)
                                                                          .Where(i => i.InventoryID.Equals(inventoryID))
                                                                          .OrderBy(x=>x.Product.ProductCategory.Name).ThenBy(x=>x.Product.Name)
                                                                          select new InventoryProductViewModel
                                                                              {
                                                                                  ID = ip.ID,
                                                                                  Product = ip.Product.Name,
                                                                                  ProductCategory =
                                                                                      ip.Product.ProductCategory
                                                                                        .Name,
                                                                                  Usable = ip.Usable,
                                                                                  Repairable = ip.Repairable
                                                                              }
                                                                      );
            return inventoryProducts;

        }

        public InventoryViewModel GetInitialInventoryViewModel()
        {
            int locationNumber = 0;
            int year = 0;
            int month = 0;

            string userName = DataService.GetUserName();

            int branch = _employeeService.GetEmployeeBranchByADUserName(userName);

            if (branch != 0)
            {
                locationNumber = branch;

                Inventory inventory = GetMostRecentInventoryByBranch(branch);

                year = inventory.Year;
                month = inventory.Month;
            }

            InventoryViewModel inventoryViewModel = new InventoryViewModel
                {
                    LocationNumber = locationNumber,
                    Year = year,
                    Month = month
                };

            return inventoryViewModel;
        }

        public Inventory GetMostRecentInventoryByBranch(int branch)
        {
            Inventory inventory = new Inventory();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var years = (from i in context.Inventories
                            where i.LocationNumber == branch
                            select i.Year);

                    if (years.Any())
                    {
                        int year = years.Max();

                        int month = (from i in context.Inventories
                                     where i.LocationNumber == branch
                                           && i.Year == year
                                     select i.Month).Max();

                        inventory.Year = year;
                        inventory.Month = month;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return inventory;
        }

        public void AddNewMonthByBranchYearMonth(AddMonthViewModel addMonthViewModel)
        {
            string userName = DataService.GetUserName();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    Inventory inventory = new Inventory
                        {
                            LocationNumber = addMonthViewModel.LocationNumber.GetValueOrDefault(),
                            Month = addMonthViewModel.Month.GetValueOrDefault(),
                            Year = addMonthViewModel.Year.GetValueOrDefault(),
                            CreatedByUser = userName,
                            CreatedTime = DateTime.Now
                        };

                    context.Inventories.Add(inventory);
                    context.SaveChanges();

                    IQueryable<Product> products = (from p in context.Products
                                                    join ip in context.ProductCategories on p.ProductCategoryID equals ip.ID
                                                    where p.StatusID == 1 && ip.StatusID == 1
                                                    select p);

                    foreach (Product product in products)
                    {
                        InventoryProduct inventoryProduct = new InventoryProduct
                            {
                                InventoryID = inventory.ID,
                                ProductID = product.ID,
                                CreatedByUser = userName,
                                CreatedTime = DateTime.Now
                            };
                        context.InventoryProducts.Add(inventoryProduct);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

        }

        

        

        #endregion
              
    }

   
}
