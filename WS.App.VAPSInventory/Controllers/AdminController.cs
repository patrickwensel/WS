using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using WS.App.VAPSInventory.Models;
using WS.App.VAPSInventory.Objects;
using WS.App.VAPSInventory.ViewModels;
using WS.Framework.WSJDEData;


namespace WS.App.VAPSInventory.Controllers
{
    public class AdminController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //[Authorize(Roles = "VAPS Inventory Admin")]
        public ActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "VAPS Inventory Admin")]
        public ActionResult Category()
        {
            PopulateProductCategoryStatuses();
            CategoryAdminViewModel categoryAdminViewModel = new CategoryAdminViewModel();
            return View(categoryAdminViewModel);
        }

        //[Authorize(Roles = "VAPS Inventory Admin")]
        public ActionResult Product()
        {
            PopulateProductStatuses();
            PopulateProductCategories();
            ProductAdminViewModel productAdminViewModel = new ProductAdminViewModel();
            return View(productAdminViewModel);
        }

        //[Authorize(Roles = "VAPS Inventory Admin")]
        public ActionResult AddMonthToAllBranches()
        {
            return View();
        }

        //[Authorize(Roles = "VAPS Inventory Admin")]
        public ActionResult AddActiveProductToAllBranches()
        {
            return View();
        }

        public JsonResult GetYear()
        {
            return Json(DataService.Year(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMonth()
        {
            return Json(DataService.Month(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadProductCategory([DataSourceRequest] DataSourceRequest request)
        {
            WSJDE context = new WSJDE();

            IQueryable<ProductCategoryViewModel> productCategoryViewModels = (from pc in context.ProductCategories
                                                                              select new ProductCategoryViewModel
                                                                              {
                                                                                  ID = pc.ID,
                                                                                  Status = pc.StatusID,
                                                                                  Name = pc.Name,
                                                                                  Description = pc.Description
                                                                              }
                                             );

            return Json(productCategoryViewModels.ToDataSourceResult(request));
        }

        public ActionResult ReadProduct([DataSourceRequest] DataSourceRequest request)
        {
            WSJDE context = new WSJDE();

            IQueryable<ProductViewModel> productViewModels = (from p in context.Products
                                                              select new ProductViewModel
                                                                  {
                                                                      ID = p.ID,
                                                                      ProductCategoryID = p.ProductCategoryID,
                                                                      Status = p.StatusID,
                                                                      Name = p.Name,
                                                                      Description = p.Description
                                                                  }
                                                             );

            return Json(productViewModels.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateProductCategory([DataSourceRequest] DataSourceRequest dataSourceRequest, ProductCategoryViewModel productCategoryViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool duplicate = CheckForDuplicateProductCategory(productCategoryViewModel);

                    if (duplicate)
                    {
                        ModelState.AddModelError(string.Empty, "This is a duplicate Category");
                    }
                    else
                    {
                        using (WSJDE context = new WSJDE())
                        {
                            ProductCategory productCategory = new ProductCategory
                                {
                                    Name = productCategoryViewModel.Name,
                                    Description = productCategoryViewModel.Description,
                                    StatusID = productCategoryViewModel.Status,
                                    CreatedByUser = GetUserName(),
                                    CreatedTime = DateTime.Now
                                };

                            context.ProductCategories.Add(productCategory);
                            context.SaveChanges();

                            productCategoryViewModel.ID = productCategory.ID;

                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return Json(new[] { productCategoryViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateProductCategory([DataSourceRequest] DataSourceRequest dataSourceRequest, ProductCategoryViewModel productCategoryViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        var x = (from pc in context.ProductCategories
                                 where pc.ID != productCategoryViewModel.ID
                                       && pc.Name == productCategoryViewModel.Name
                                 select pc);
                        if (x.Any())
                        {
                            ModelState.AddModelError(string.Empty, "This is a duplicate Category");
                        }
                        else
                        {
                            ProductCategory productCategory = (from pc in context.ProductCategories
                                                               where pc.ID == productCategoryViewModel.ID
                                                               select pc).FirstOrDefault();

                            productCategory.Name = productCategoryViewModel.Name;
                            productCategory.StatusID = productCategoryViewModel.Status;
                            productCategory.Description = productCategoryViewModel.Description;
                            productCategory.ModifiedByUser = GetUserName();
                            productCategory.ModifiedTime = DateTime.Now;

                            context.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
            return Json(new[] { productCategoryViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateProduct([DataSourceRequest] DataSourceRequest dataSourceRequest, ProductViewModel productViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        bool duplicate = CheckForDuplicateProduct(productViewModel);

                        if (duplicate)
                        {
                            ModelState.AddModelError(string.Empty, "This is a duplicate Product");
                        }
                        else
                        {
                            Product product = new Product
                                {
                                    Name = productViewModel.Name,
                                    Description = productViewModel.Description,
                                    ProductCategoryID = productViewModel.ProductCategoryID,
                                    StatusID = productViewModel.Status,
                                    CreatedByUser = GetUserName(),
                                    CreatedTime = DateTime.Now
                                };

                            context.Products.Add(product);
                            context.SaveChanges();

                            productViewModel.ID = product.ID;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return Json(new[] { productViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateProduct([DataSourceRequest] DataSourceRequest dataSourceRequest, ProductViewModel productViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        var x = (from p in context.Products
                                 where p.ID != productViewModel.ID
                                       && p.Name == productViewModel.Name
                                       && p.ID == productViewModel.ID
                                 select p);
                        if (x.Any())
                        {
                            ModelState.AddModelError(string.Empty, "This is a duplicate Product");
                        }
                        else
                        {
                            Product product = (from p in context.Products
                                               where p.ID == productViewModel.ID
                                               select p).FirstOrDefault();

                            product.Name = productViewModel.Name;
                            product.Description = productViewModel.Description;
                            product.ProductCategoryID = productViewModel.ProductCategoryID;
                            product.StatusID = productViewModel.Status;
                            product.ModifiedByUser = GetUserName();
                            product.ModifiedTime = DateTime.Now;

                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return Json(new[] { productViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        private void PopulateProductCategoryStatuses()
        {
            IQueryable<ProductCategoryStatusViewModel> productCategoryStatuses = GetProductCategoryStatuses();

            ViewData["productCategoryStatuses"] = productCategoryStatuses;

        }

        public IQueryable<ProductCategoryStatusViewModel> GetProductCategoryStatuses()
        {
            WSJDE context = new WSJDE();

            IQueryable<ProductCategoryStatusViewModel> productCategoryStatuses = (from pcs in context.ProductCategoryStatuses
                                                                                  select new ProductCategoryStatusViewModel
                                                                                  {
                                                                                      ID = pcs.ID,
                                                                                      Status = pcs.Status
                                                                                  });
            return productCategoryStatuses;
        }

        private void PopulateProductStatuses()
        {
            WSJDE context = new WSJDE();

            IQueryable<ProductStatusViewModel> productStatuses = (from ps in context.ProductStatuses
                                                                                  select new ProductStatusViewModel
                                                                                  {
                                                                                      ID = ps.ID,
                                                                                      Status = ps.Status
                                                                                  });

            ViewData["productStatuses"] = productStatuses;

        }

        private void PopulateProductCategories()
        {
            IQueryable<ProductCategoryViewModel> productCategories = GetProductCategories();

            ViewData["productCategories"] = productCategories;

        }

        public JsonResult ReadProductCategories()
        {
            return Json(GetProductCategories(), JsonRequestBehavior.AllowGet);
        }

        private IQueryable<ProductCategoryViewModel> GetProductCategories()
        {
            WSJDE context = new WSJDE();

            IQueryable<ProductCategoryViewModel> productCategories = (from pc in context.ProductCategories
                                                                      //where pc.Status == 1
                                                                      select new ProductCategoryViewModel
                                                                      {
                                                                          ID = pc.ID,
                                                                          Name = pc.Name
                                                                      }).OrderBy(x=>x.Name);

            ViewData["productCategories"] = productCategories;

            return productCategories;
        }

        public JsonResult ReadActiveProductCategories()
        {
            return Json(GetActiveProductCategories(), JsonRequestBehavior.AllowGet);
        }

        private IQueryable<ProductCategoryViewModel> GetActiveProductCategories()
        {
            WSJDE context = new WSJDE();

            IQueryable<ProductCategoryViewModel> productCategories = (from pc in context.ProductCategories
                                                                      where pc.StatusID == 1
                                                                      select new ProductCategoryViewModel
                                                                      {
                                                                          ID = pc.ID,
                                                                          Name = pc.Name
                                                                      }).OrderBy(x => x.Name);


            return productCategories;
        }

        public string GetUserName()
        {
            string userName = "";

            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            String userNameWithDomain = principal.Identity.Name;

            if (userNameWithDomain != null)
            {
                userName = userNameWithDomain.Split('\\')[1];
            }

            return userName;
        }

        public bool CheckForDuplicateProductCategory(ProductCategoryViewModel productCategoryViewModel)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var x = (from pc in context.ProductCategories
                             where pc.Name == productCategoryViewModel.Name
                             select pc);

                    if(x.Any())
                    {
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
            
            return false;
        }

        public bool CheckForDuplicateProduct(ProductViewModel productViewModel)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var x = (from p in context.Products
                             where p.Name == productViewModel.Name  && p.ProductCategoryID == productViewModel.ProductCategoryID
                             select p);

                    if (x.Any())
                    {
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return false;
        }

        public ActionResult AddNewMonthToAllBranchsByYearMonth(YearMonthViewModel yearMonthViewModel)
        {
            string userName = DataService.GetUserName();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    List<Branch> branchs = DataService.Branches();
                    foreach (Branch branch in branchs)
                    {
                        var x = (from i in context.Inventories
                                 where i.LocationNumber == branch.ID && i.Month == yearMonthViewModel.Month
                                 select i);

                        if (!x.Any())
                        {
                            Inventory inventory = new Inventory
                                {
                                    LocationNumber = branch.ID,
                                    Month = yearMonthViewModel.Month,
                                    Year = yearMonthViewModel.Year,
                                    CreatedByUser = userName,
                                    CreatedTime = DateTime.Now
                                };

                            context.Inventories.Add(inventory);
                            context.SaveChanges();

                            IQueryable<Product> products = (from p in context.Products
                                                            where p.StatusID == 1
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
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);

                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddActiveProductToAllBranches(YearMonthViewModel yearMonthViewModel)
        {
            string userName = DataService.GetUserName();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    List<Branch> branchs = DataService.Branches();
                    foreach (Branch branch in branchs)
                    {
                        IQueryable<Inventory> inventories = (from i in context.Inventories
                                  where i.LocationNumber == branch.ID
                                  select i);

                        List<int> inventoriesToAdd = new List<int>();

                        foreach (Inventory inventory in inventories)
                        {
                            if (inventory.Year == yearMonthViewModel.Year)
                            {
                                if (inventory.Month >= yearMonthViewModel.Month)
                                {
                                    inventoriesToAdd.Add(inventory.ID);
                                }
                            }
                            if (inventory.Year > yearMonthViewModel.Year)
                            {
                                inventoriesToAdd.Add(inventory.ID);
                            }
                        }

                        if (inventoriesToAdd.Count != 0)
                        {
                            foreach (int inv in inventoriesToAdd)
                            {
                                IQueryable<int> inventoryProducts = (from ip in context.InventoryProducts
                                                                     where ip.InventoryID == inv
                                                                     select ip.ProductID);

                                IQueryable<int> activeProducts = (from p in context.Products
                                                                  where p.StatusID == 1
                                                                  select p.ID);

                                IQueryable<int> difference = activeProducts.Except(inventoryProducts);

                                foreach (int i in difference)
                                {
                                    InventoryProduct inventoryProduct = new InventoryProduct
                                        {
                                            InventoryID = inv,
                                            ProductID = i,
                                            CreatedByUser = userName,
                                            CreatedTime = DateTime.Now
                                        };

                                    context.InventoryProducts.Add(inventoryProduct);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);

                return Json(new { success = false });
            }

            return Json(new { success = true });
        }
    }
}
