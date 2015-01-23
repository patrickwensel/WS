using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AS.App.Intranet.ViewModels.Company;
using AS.App.Intranet.ViewModels.Hierarchy;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using WS.Framework.ASJDE;

namespace AS.App.Intranet.Controllers
{
    public class CompanyController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //[Authorize(Roles = "Corporate Hierarchy Admin")]
        public ActionResult Admin()
        {

            AdminViewModel adminViewModel = new AdminViewModel();
            try
            {
                Hierarchy hierarchy = new Hierarchy
                {
                    CompanyViewModels = GetCompanyViewModels()
                };

                adminViewModel.Hierarchy = hierarchy;
                PopulateEntityStatuss();
                PopulateLocations();
                PopulateLocationStatuss();
                PopulateEntities();
                PopulateCompanies();
                PopulateSBUs();

            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
                return null;
            }

            return View(adminViewModel);
        }

        public ActionResult _Company()
        {

            
            return View();
        }

        public ActionResult _Hierarchy()
        {
            Hierarchy hierarchy = new Hierarchy
                {
                    //CompanyViewModels = GetCompanyViewModels(),

                };

            return View(hierarchy);
        }

        public ActionResult _Location()
        {
            return View();
        }

        public ActionResult _Entity()
        {
            PopulateLocations();
            PopulateEntityStatuss();
            return View();
        }

        #region Tree View

        private IEnumerable<CompanyViewModel> GetCompanyViewModels()
        {
            //List<StrategicBusinessUnitViewModel> sbuViewModels = new List<StrategicBusinessUnitViewModel>();

            List<CompanyViewModel> companyViewModels = new List<CompanyViewModel>();
            try
            {
                using (ASJDE context = new ASJDE())
                {

                    IQueryable<Company> companies = (from c in context.Companies select c);

                    foreach (Company company in companies)
                    {
                        CompanyViewModel companyViewModel = new CompanyViewModel();
                        companyViewModel.Name = company.Entity.Name;
                        companyViewModel.StrategicBusinessUnitViews = new List<StrategicBusinessUnitViewModel>();

                        Company co = company;
                        IQueryable<StrategicBusinessUnit> sbus = (from s in context.StrategicBusinessUnits
                                                                  where s.Entity.EntityStatus.ID == 1
                                                                  && s.CompanyID == co.ID
                                                                  select s);

                        foreach (var strategicBusinessUnit in sbus)
                        {
                            StrategicBusinessUnitViewModel sbuViewModel = new StrategicBusinessUnitViewModel();
                            sbuViewModel.Name = strategicBusinessUnit.Entity.Name;
                            sbuViewModel.OBUViewModels = new List<OperationalBusinessUnitViewModel>();

                            StrategicBusinessUnit sbUnit = strategicBusinessUnit;

                            IQueryable<OperationalBusinessUnit> obs = (from o in context.OperationalBusinessUnits
                                                                       where o.Entity.EntityStatus.ID == 1
                                                                             && o.StrategicBusinessUnitID == sbUnit.ID
                                                                       select o);

                            foreach (var operationalBusinessUnit in obs)
                            {
                                OperationalBusinessUnitViewModel obuViewModel = new OperationalBusinessUnitViewModel();
                                obuViewModel.Name = operationalBusinessUnit.Entity.Name;
                                obuViewModel.BUViewModels = new List<BusinessUnitViewModel>();

                                OperationalBusinessUnit bUnit = operationalBusinessUnit;
                                IQueryable<BusinessUnit> bus = (from b in context.BusinessUnits
                                                                where b.Entity.EntityStatus.ID == 1
                                                                      && b.OperationalBusinessUnitID == bUnit.ID
                                                                select b);

                                foreach (var bu in bus)
                                {
                                    BusinessUnitViewModel buViewModel = new BusinessUnitViewModel();
                                    buViewModel.Name = bu.Entity.Name;
                                    buViewModel.BranchViewModels = new List<BranchViewModel>();

                                    BusinessUnit bu1 = bu;
                                    IQueryable<Branch> branchs = (from br in context.Branches
                                                                  where br.Entity.EntityStatus.ID == 1
                                                                        && br.BusinessUnitID == bu1.ID
                                                                  select br);

                                    foreach (var branch in branchs)
                                    {
                                        BranchViewModel branchViewModel = new BranchViewModel();
                                        branchViewModel.Name = branch.Entity.Name;
                                        branchViewModel.DepotViewModels = new List<DepotViewModel>();

                                        Branch branch1 = branch;
                                        var depots = (from d in context.Depots
                                                      where d.Entity.EntityStatus.ID == 1
                                                            && d.BranchID == branch1.ID
                                                      select d);
                                        foreach (var depot in depots)
                                        {
                                            DepotViewModel depotViewModel = new DepotViewModel();
                                            depotViewModel.Name = depot.Entity.Name;

                                            branchViewModel.DepotViewModels.Add(depotViewModel);
                                        }

                                        buViewModel.BranchViewModels.Add(branchViewModel);
                                    }

                                    obuViewModel.BUViewModels.Add(buViewModel);
                                }

                                sbuViewModel.OBUViewModels.Add(obuViewModel);
                            }

                            companyViewModel.StrategicBusinessUnitViews.Add(sbuViewModel);
                        }

                        companyViewModels.Add(companyViewModel);
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
                return null;
            }
            return companyViewModels;
        }

        #endregion

        #region Company

        public void PopulateCompanies()
        {
            HierarchyController hierarchyController = new HierarchyController();
            IQueryable<CompanyViewModel> companyViewModels = hierarchyController.GetAllCompanies();

            ViewData["companyViewModels"] = companyViewModels;
        }

        public ActionResult CompanyRead([DataSourceRequest] DataSourceRequest request)
        {
            HierarchyController hierarchyController = new HierarchyController();
            return Json(hierarchyController.GetAllCompanies().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyCreate([DataSourceRequest] DataSourceRequest request, CompanyViewModel companyViewModel)
        {
            try
            {
                if (companyViewModel != null && ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {

                        Company company = new Company
                            {
                                ID = companyViewModel.ID,
                                EntityID = companyViewModel.EntityID
                            };

                        context.Companies.Add(company);
                        context.SaveChanges();

                        companyViewModel.ID = company.ID;

                    }
                }

                return Json(new[] { companyViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, EntityViewModel entityViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        //Video video = (from v in context.Videos
                        //               where v.ID == locationViewModel.ID
                        //               select v).FirstOrDefault();

                        //if (video != null)
                        //{
                        //    video.Name = locationViewModel.Name;
                        //    video.VideoGroupID = locationViewModel.GroupID;
                        //    video.ReferenceID = locationViewModel.ReferenceID;

                        //    context.SaveChanges();
                        //}
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
            return Json(new[] { entityViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyDestroy([DataSourceRequest] DataSourceRequest request, EntityViewModel entityViewModel)
        {
            try
            {
                if (entityViewModel != null)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        //Video video = (from v in context.Videos
                        //               where v.ID == locationViewModel.ID
                        //               select v).FirstOrDefault();

                        //context.Videos.Remove(video);
                        //context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        #endregion

        #region SBU

        public void PopulateSBUs()
        {
            IQueryable<StrategicBusinessUnitViewModel> strategicBusinessUnitViewModels = GetAllStrategicBusinessUnits();

            ViewData["strategicBusinessUnits"] = strategicBusinessUnitViewModels;
        }

        public IQueryable<StrategicBusinessUnitViewModel> GetAllStrategicBusinessUnits()
        {

            ASJDE context = new ASJDE();

            IQueryable<StrategicBusinessUnitViewModel> sbus = (from a in context.StrategicBusinessUnits
                                                               select new StrategicBusinessUnitViewModel
                                                               {
                                                                   ID = a.ID,
                                                                   EntityID = a.EntityID,
                                                                   CompanyID = a.CompanyID,
                                                                   Name = a.Entity.Name
                                                                   //Entity = a.Entity

                                                               }
                                                       );

            var x = sbus.ToList();

            return sbus;

        }

        public ActionResult SBURead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllStrategicBusinessUnits().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SBUCreate([DataSourceRequest] DataSourceRequest request, StrategicBusinessUnitViewModel strategicBusinessUnitViewModel)
        {
            try
            {
                if (strategicBusinessUnitViewModel != null && ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {

                        StrategicBusinessUnit strategicBusinessUnit = new StrategicBusinessUnit
                            {
                                CompanyID = strategicBusinessUnitViewModel.CompanyID,
                                EntityID = strategicBusinessUnitViewModel.EntityID
                            };

                        context.StrategicBusinessUnits.Add(strategicBusinessUnit);
                        context.SaveChanges();
                        strategicBusinessUnitViewModel.ID = strategicBusinessUnit.ID;

                    }
                }

                return Json(new[] { strategicBusinessUnitViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SBUUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, StrategicBusinessUnitViewModel strategicBusinessUnitViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        StrategicBusinessUnit strategicBusinessUnit = (from s in context.StrategicBusinessUnits
                                                                       where s.ID == strategicBusinessUnitViewModel.ID
                                                                       select s).FirstOrDefault();

                        if (strategicBusinessUnit != null)
                        {
                            strategicBusinessUnit.EntityID = strategicBusinessUnitViewModel.EntityID;
                            strategicBusinessUnit.CompanyID = strategicBusinessUnitViewModel.CompanyID;

                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
            return Json(new[] { strategicBusinessUnitViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SBUDestroy([DataSourceRequest] DataSourceRequest request, StrategicBusinessUnitViewModel strategicBusinessUnitViewModel)
        {
            try
            {
                if (strategicBusinessUnitViewModel != null)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        StrategicBusinessUnit strategicBusinessUnit = (from s in context.StrategicBusinessUnits
                                                                       where s.ID == strategicBusinessUnitViewModel.ID
                                                                       select s).FirstOrDefault();
                        context.StrategicBusinessUnits.Remove(strategicBusinessUnit);
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

        #endregion

        #region OBU

        public ActionResult OBURead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllOperationalBusinessUnits().ToDataSourceResult(request));
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OBUCreate([DataSourceRequest] DataSourceRequest request, OperationalBusinessUnitViewModel operationalBusinessUnitViewModel)
        {
            try
            {
                if (operationalBusinessUnitViewModel != null && ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {

                        OperationalBusinessUnit operationalBusinessUnit = new OperationalBusinessUnit
                            {
                                StrategicBusinessUnitID = operationalBusinessUnitViewModel.StrategicBusinessUnitID,
                                EntityID = operationalBusinessUnitViewModel.EntityID
                            };

                        context.OperationalBusinessUnits.Add(operationalBusinessUnit);
                        context.SaveChanges();
                        operationalBusinessUnitViewModel.ID = operationalBusinessUnit.ID;

                    }
                }

                return Json(new[] { operationalBusinessUnitViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OBUUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, OperationalBusinessUnitViewModel operationalBusinessUnitViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        OperationalBusinessUnit operationalBusinessUnit = (from s in context.OperationalBusinessUnits
                                                                           where
                                                                               s.ID ==
                                                                               operationalBusinessUnitViewModel.ID
                                                                           select s).FirstOrDefault();


                        if (operationalBusinessUnit != null)
                        {
                            operationalBusinessUnit.EntityID = operationalBusinessUnitViewModel.EntityID;
                            operationalBusinessUnit.StrategicBusinessUnitID = operationalBusinessUnitViewModel.StrategicBusinessUnitID;

                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
            return Json(new[] { operationalBusinessUnitViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OBUDestroy([DataSourceRequest] DataSourceRequest request, OperationalBusinessUnitViewModel operationalBusinessUnitViewModel)
        {
            try
            {
                if (operationalBusinessUnitViewModel != null)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        OperationalBusinessUnit operationalBusinessUnit = (from s in context.OperationalBusinessUnits
                                                                           where
                                                                               s.ID ==
                                                                               operationalBusinessUnitViewModel.ID
                                                                           select s).FirstOrDefault();
                        context.OperationalBusinessUnits.Remove(operationalBusinessUnit);
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

        #endregion

        #region Location

        public void PopulateLocations()
        {
            HierarchyController hierarchyController = new HierarchyController();
            IQueryable<LocationViewModel> statuss = hierarchyController.GetAllLocations();

            ViewData["locations"] = statuss;
        }

        public void PopulateLocationStatuss()
        {
            HierarchyController hierarchyController = new HierarchyController();
            IQueryable<LocationStatusViewModel> statuss = hierarchyController.GetAllLocationStatuss();

            ViewData["locationStatuss"] = statuss;
        }

        public ActionResult LocationRead([DataSourceRequest] DataSourceRequest request)
        {
            HierarchyController hierarchyController = new HierarchyController();
            return Json(hierarchyController.GetAllLocations().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationCreate([DataSourceRequest] DataSourceRequest request, LocationViewModel locationViewModel)
        {
            try
            {
                if (locationViewModel != null && ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {

                        //Video video = new Video
                        //{
                        //    VideoGroupID = locationViewModel.GroupID,
                        //    Name = locationViewModel.Name,
                        //    ReferenceID = locationViewModel.ReferenceID,
                        //    CreatedDate = DateTime.Now,
                        //    CreatedByUser = ""
                        //};

                        //context.Videos.Add(video);
                        //context.SaveChanges();

                        //locationViewModel.ID = video.ID;

                    }
                }

                return Json(new[] { locationViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, LocationViewModel locationViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        //Video video = (from v in context.Videos
                        //               where v.ID == locationViewModel.ID
                        //               select v).FirstOrDefault();

                        //if (video != null)
                        //{
                        //    video.Name = locationViewModel.Name;
                        //    video.VideoGroupID = locationViewModel.GroupID;
                        //    video.ReferenceID = locationViewModel.ReferenceID;

                        //    context.SaveChanges();
                        //}
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
            return Json(new[] { locationViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationDestroy([DataSourceRequest] DataSourceRequest request, LocationViewModel locationViewModel)
        {
            try
            {
                if (locationViewModel != null)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        //Video video = (from v in context.Videos
                        //               where v.ID == locationViewModel.ID
                        //               select v).FirstOrDefault();

                        //context.Videos.Remove(video);
                        //context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }

            return Json(ModelState.ToDataSourceResult());
        }

       
        #endregion

        #region Entity

        public void PopulateEntities()
        {
            IQueryable<EntityViewModel> entityViewModels = GetAllEntities();

            ViewData["entityViewModels"] = entityViewModels;
        }

        public ActionResult EntityRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllEntities().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EntityCreate([DataSourceRequest] DataSourceRequest request, EntityViewModel entityViewModel)
        {
            try
            {
                if (entityViewModel != null && ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {

                        Entity entity = new Entity
                            {
                                LocationID = entityViewModel.LocationID,
                                StatusID = entityViewModel.StatusID,
                                Name = entityViewModel.Name,
                                Code = entityViewModel.Code,
                                CostCenter = entityViewModel.CostCenter

                            };

                        context.Entities.Add(entity);
                        context.SaveChanges();
                        entityViewModel.ID = entity.ID;
                    }
                }

                return Json(new[] { entityViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EntityUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, EntityViewModel entityViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        Entity entity = (from v in context.Entities
                                         where v.ID == entityViewModel.ID
                                       select v).FirstOrDefault();

                        if (entity != null)
                        {
                            entity.LocationID = entityViewModel.LocationID;
                            entity.StatusID = entityViewModel.StatusID;
                            entity.Name = entityViewModel.Name;
                            entity.Code = entityViewModel.Code;
                            entity.CostCenter = entityViewModel.CostCenter;

                            context.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }
            return Json(new[] { entityViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EntityDestroy([DataSourceRequest] DataSourceRequest request, EntityViewModel entityViewModel)
        {
            try
            {
                if (entityViewModel != null)
                {
                    using (ASJDE context = new ASJDE())
                    {
                        Entity entity = (from v in context.Entities
                                         where v.ID == entityViewModel.ID
                                         select v).FirstOrDefault();

                        context.Entities.Remove(entity);
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

        public IQueryable<EntityViewModel> GetAllEntities()
        {
            ASJDE context = new ASJDE();

            IQueryable<EntityViewModel> entityViewModels = (from l in context.Entities
                                    select new EntityViewModel
                                        {
                                            ID = l.ID,
                                            Name = l.Name,
                                            LocationID = l.LocationID.Value,
                                            StatusID = l.StatusID.Value,
                                            Code = l.Code,
                                            CostCenter = l.CostCenter
                                        });

            return entityViewModels;
        }
        #endregion

        #region Entity Status

        public void PopulateEntityStatuss()
        {
            IQueryable<EntityStatusViewModel> statuss = GetAllEntityStatuss();

            ViewData["entityStatuss"] = statuss;
        }

        public JsonResult ReadAllEntityStatuss()
        {
            return Json(GetAllEntityStatuss(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<EntityStatusViewModel> GetAllEntityStatuss()
        {
            ASJDE context = new ASJDE();

            IQueryable<EntityStatusViewModel> statusViewModels = (from a in context.EntityStatus
                                                                  select new EntityStatusViewModel
                                                                      {
                                                                          ID = a.ID,
                                                                          Name = a.Name
                                                                      }
                                                                 );

            return statusViewModels;
        }

        #endregion
    }
}
