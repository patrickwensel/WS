using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AS.App.Intranet.Objects.Video;
using AS.App.Intranet.ViewModels.Company;
using AS.App.Intranet.ViewModels.Hierarchy;
using AS.App.Intranet.ViewModels.Video;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using WS.Framework.ASJDE;
using WS.Framework.ServicesInterface;
using VideoArea = WS.Framework.ASJDE.VideoArea;
using VideoGroup = WS.Framework.ASJDE.VideoGroup;
using VideoLog = WS.Framework.ASJDE.VideoLog;


namespace AS.App.Intranet.Controllers
{
    public class VideoController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISecurityService _securityService;
        private readonly IHierarchyService _hierarchyService;

        public VideoController(ISecurityService securityService, IHierarchyService hierarchyService)
        {
            _securityService = securityService;
            _hierarchyService = hierarchyService;
        }

        public ActionResult Log()
        {
            return View();
        }

        #region AS
        
        public ActionResult AlgecoScotsman()
        {
            var hierarchy = new Hierarchy
            {
                //SBUViewModels = GetSBUViewModels()
            };

            return View(hierarchy);
        }

        private IEnumerable<StrategicBusinessUnitViewModel> GetSbuViewModels()
        {
            //List<StrategicBusinessUnitViewModel> sbuViewModels = new List<StrategicBusinessUnitViewModel>();

            //using (ASJDE context = new ASJDE())
            //{

            //    IQueryable<StrategicBusinessUnit> sbus = (from s in context.StrategicBusinessUnits
            //                                              where s.Entity.EntityStatus.ID == 1
            //                                              select s);

            //    foreach (var strategicBusinessUnit in sbus)
            //    {
            //        StrategicBusinessUnitViewModel sbuViewModel = new StrategicBusinessUnitViewModel();
            //        sbuViewModel.Name = strategicBusinessUnit.Entity.Name;
            //        sbuViewModel.OBUViewModels = new List<OperationalBusinessUnitViewModel>();

            //        StrategicBusinessUnit sbUnit = strategicBusinessUnit;

            //        IQueryable<OperationalBusinessUnit> obs = (from o in context.OperationalBusinessUnits
            //                                                   where o.Entity.EntityStatus.ID == 1
            //                                                   && o.StrategicBusinessUnitID == sbUnit.ID
            //                                                   select o);

            //        foreach (var operationalBusinessUnit in obs)
            //        {
            //            OperationalBusinessUnitViewModel obuViewModel = new OperationalBusinessUnitViewModel();
            //            obuViewModel.Name = operationalBusinessUnit.Entity.Name;
            //            obuViewModel.BUViewModels = new List<BusinessUnitViewModel>();

            //            OperationalBusinessUnit bUnit = operationalBusinessUnit;
            //            IQueryable<WS.Framework.ASJDE.BusinessUnit> bus = (from b in context.BusinessUnits
            //                                                               where b.Entity.EntityStatus.ID == 1
            //                                                  && b.OperationalBusinessUnitID == bUnit.ID
            //                                            select b);

            //            foreach (var bu in bus)
            //            {
            //                BusinessUnitViewModel buViewModel = new BusinessUnitViewModel();
            //                buViewModel.Name = bu.Entity.Name;
            //                buViewModel.BranchViewModels = new List<BranchViewModel>();

            //                WS.Framework.ASJDE.BusinessUnit bu1 = bu;
            //                IQueryable<Branch> branchs = (from br in context.Branches
            //                                              where br.Entity.EntityStatus.ID == 1
            //                                                    && br.BusinessUnitID == bu1.ID
            //                                              select br);

            //                foreach (var branch in branchs)
            //                {
            //                    BranchViewModel branchViewModel = new BranchViewModel();
            //                    branchViewModel.Name = branch.Entity.Name;
            //                    branchViewModel.DepotViewModels = new List<DepotViewModel>();

            //                    Branch branch1 = branch;
            //                    var depots = (from d in context.Depots
            //                                  where d.Entity.EntityStatus.ID == 1
            //                                        && d.BranchID == branch1.ID
            //                                  select d);
            //                    foreach (var depot in depots)
            //                    {
            //                        DepotViewModel depotViewModel = new DepotViewModel();
            //                        depotViewModel.Name = depot.Entity.Name;

            //                        branchViewModel.DepotViewModels.Add(depotViewModel);
            //                    }

            //                    buViewModel.BranchViewModels.Add(branchViewModel);
            //                }

            //                obuViewModel.BUViewModels.Add(buViewModel);
            //            }

            //            sbuViewModel.OBUViewModels.Add(obuViewModel);
            //        }

            //        sbuViewModels.Add(sbuViewModel);
            //    }
            //}
            //return sbuViewModels;

            return null;
        }

        #endregion

        #region Video Player

        public ActionResult VideoPlayer(int id)
        {
            var video = GetVideoById(id);

            var videoPlayerViewModel = new VideoPlayerViewModel
                {
                    Name = video.Name,
                    ReferenceID = video.ReferenceID,
                    VideoPlayer = BuildPlayerJavaScript(video.ReferenceID)
                };

            LogVideo(id);

            return View(videoPlayerViewModel);

        }

        public ActionResult VideoPlayerFrame(int id)
        {
            Video video = GetVideoById(id);

            var videoPlayerViewModel = new VideoPlayerViewModel
            {
                Name = video.Name,
                ReferenceID = video.ReferenceID,
                VideoPlayer = BuildPlayerJavaScript(video.ReferenceID)
            };

            LogVideo(id);

            return View(videoPlayerViewModel);

        }

        public void LogVideo(int videoId)
        {
            try
            {
                using (var context = new ASJDE())
                {
                    var videoLog = new VideoLog
                    {
                        VideoID = videoId,
                        User = _securityService.GetDomainAndUsername(),
                        TimeStamp = DateTime.Now
                    };

                    context.VideoLogs.Add(videoLog);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
            }
        }

        public string BuildPlayerJavaScript(decimal id)
        {
            var js = new StringBuilder();

            string bgColor = ConfigurationManager.AppSettings["VideoPlayerBGColor"];
            string width = ConfigurationManager.AppSettings["VideoPlayerWidth"];
            string height = ConfigurationManager.AppSettings["VideoPlayerHeight"];
            string playerID = ConfigurationManager.AppSettings["VideoPlayerPlayerID"];
            string playerKey = ConfigurationManager.AppSettings["VideoPlayerPlayerKey"];
            string isVid = ConfigurationManager.AppSettings["VideoPlayerIsVid"];
            string isUI = ConfigurationManager.AppSettings["VideoPlayerIsUi"];
            string dynamicStreaming = ConfigurationManager.AppSettings["VideoPlayerDynamicStreamingr"];

            js.Append("<script language='JavaScript' type='text/javascript' src='http://admin.brightcove.com/js/BrightcoveExperiences.js'></script>");
            js.Append("<object id='myExperience2676245806001' class='BrightcoveExperience'>");
            js.Append("<param name='bgcolor' value='" + bgColor + "' />");
            js.Append("<param name='width' value='" + width + "' />");
            js.Append("<param name='height' value='" + height + "' />");
            js.Append("<param name='playerID' value='" + playerID + "' />");
            js.Append("<param name='playerKey' value='" + playerKey + "' />");
            js.Append("<param name='isVid' value='" + isVid + "' />");
            js.Append("<param name='isUI' value='" + isUI + "' />");
            js.Append("<param name='dynamicStreaming' value='" + dynamicStreaming + "' />");
            js.Append("<param name='@videoPlayer' value='" + id + "' />");
            js.Append("<script type='text/javascript'>brightcove.createExperiences();</script>");


            //<script language="JavaScript" type="text/javascript" src="http://admin.brightcove.com/js/BrightcoveExperiences.js"></script>

            //<object id="myExperience2676245806001" class="BrightcoveExperience">
            //  <param name="bgcolor" value="#FFFFFF" />
            //  <param name='width' value='480' />
            //  <param name='height' value='270' />
            //  <param name='playerID' value='2669870126001' />
            //  <param name='playerKey' value='AQ~~,AAACbZTjlpE~,8zoTOFpOgAibDyMDm98HBsvL_78ClyJc' />
            //  <param name='isVid' value='true' />
            //  <param name='isUI' value='true' />
            //  <param name='dynamicStreaming' value='true' />

            //  <param name='@videoPlayer' value='2676245806001' />
            //</object>

            //<!-- 
            //This script tag will cause the Brightcove Players defined above it to be created as soon
            //as the line is read by the browser. If you wish to have the player instantiated only after
            //the rest of the HTML is processed and the page load is complete, remove the line.
            //-->
            //<script type='text/javascript'>brightcove.createExperiences();</script>

            //<!-- End of Brightcove Player -->

            return js.ToString();
        }

        public Video GetVideoById(long referenceId)
        {
            try
            {
                using (var context = new ASJDE())
                {
                    var video = context.Videos.FirstOrDefault(v => v.ID == referenceId);
                    return video;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
                return null;
            }
        }

        #endregion

        #region Area

        public ActionResult Area()
        {
            PopulateCompanies();
            PopulateSbUs();
            PopulateObUs();
            PopulateBUs();
            PopulateBranchs();
            PopulateDepots();
            return View();
        }

        public ActionResult AreaRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllAreas().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaCreate([DataSourceRequest] DataSourceRequest request, AreaViewModel areaViewModel)
        {
            try
            {
                if (areaViewModel != null && ModelState.IsValid)
                {
                    using (var context = new ASJDE())
                    {
                        var videoArea = new VideoArea
                            {
                                CompanyID = areaViewModel.CompanyID,
                                StrategicBusinessUnitID = areaViewModel.StrategicBusinessUnitID,
                                OperationalBusinessUnitID = areaViewModel.OperationalBusinessUnitID,
                                BusinessUnitID = areaViewModel.BusinessUnitID,
                                BranchID = areaViewModel.BranchID,
                                DepotID = areaViewModel.DepotID,
                                Name = areaViewModel.Name
                            };

                        context.VideoAreas.Add(videoArea);
                        context.SaveChanges();

                        areaViewModel.ID = videoArea.ID;
                    }
                }

                return Json(new[] { areaViewModel }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, AreaViewModel areaViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = new ASJDE())
                    {
                        var videoArea = context.VideoAreas.FirstOrDefault(va => va.ID == areaViewModel.ID);

                        if (videoArea != null)
                        {
                            videoArea.CompanyID = areaViewModel.CompanyID;
                            videoArea.StrategicBusinessUnitID = areaViewModel.StrategicBusinessUnitID;
                            videoArea.OperationalBusinessUnitID = areaViewModel.OperationalBusinessUnitID;
                            videoArea.BusinessUnitID = areaViewModel.BusinessUnitID;
                            videoArea.BranchID = areaViewModel.BranchID;
                            videoArea.DepotID = areaViewModel.DepotID;
                            videoArea.Name = areaViewModel.Name;
                        }

                        context.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
            }
            return Json(new[] { areaViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaDestroy([DataSourceRequest] DataSourceRequest request, AreaViewModel areaViewModel)
        {
            try
            {
                if (areaViewModel != null)
                {
                    using (var context = new ASJDE())
                    {
                        var videoArea = context.VideoAreas.FirstOrDefault(va => va.ID == areaViewModel.ID);
                        context.VideoAreas.Remove(videoArea);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        public void PopulateAreas()
        {
            IQueryable<AreaViewModel> areas = GetAllAreas();

            ViewData["areas"] = areas;
        }

        public JsonResult ReadAllAreas()
        {
            return Json(GetAllAreas(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreas(AreaFilter areaFilter)
        {
            return Json(AreaFiltered(areaFilter), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<AreaViewModel> GetAllAreas()
        {
            var context = new ASJDE();

            var areaViewModels = context.VideoAreas.Select(a => new AreaViewModel
                {
                    ID = a.ID,
                    CompanyID = a.CompanyID.Value,
                    StrategicBusinessUnitID = a.StrategicBusinessUnitID.Value,
                    OperationalBusinessUnitID = a.OperationalBusinessUnitID.Value,
                    BusinessUnitID = a.BusinessUnitID.Value,
                    BranchID = a.BranchID.Value,
                    DepotID = a.DepotID.Value,
                    Name = a.Name
                });

            return areaViewModels;
        }

        public static List<AreaViewModel> AreaFiltered(AreaFilter areaFilter)
        {
            var context = new ASJDE();

            return context.VideoAreas.Where(a => a.CompanyID == areaFilter.CompanyID &&
                    (areaFilter.StrategicBusinessUnitID != null ? a.StrategicBusinessUnitID == areaFilter.StrategicBusinessUnitID : a.StrategicBusinessUnitID == null) &&
                    (areaFilter.OperationalBusinessUnitID != null ? a.OperationalBusinessUnitID == areaFilter.OperationalBusinessUnitID : a.OperationalBusinessUnitID == null) &&
                    (areaFilter.BusinessUnitID != null ? a.BusinessUnitID == areaFilter.BusinessUnitID: a.BusinessUnitID == null) &&
                    (areaFilter.BranchID != null ? a.BranchID == areaFilter.BranchID : a.BranchID == null) &&
                    (areaFilter.DepotID != null ? a.DepotID == areaFilter.DepotID : a.DepotID == null))
                    .Select(a => new AreaViewModel
                        {
                            ID = a.ID,
                            CompanyID = a.CompanyID.Value,
                            StrategicBusinessUnitID = a.StrategicBusinessUnitID.Value,
                            OperationalBusinessUnitID = a.OperationalBusinessUnitID.Value,
                            BusinessUnitID = a.BusinessUnitID.Value,
                            BranchID = a.BranchID.Value,
                            DepotID = a.DepotID.Value,
                            Name = a.Name
                        }).ToList();
        }

        public VideoArea GetAreaById(int areaId)
        {
            var videoArea = new VideoArea();

            try
            {
                using (var context = new ASJDE())
                {
                    videoArea = context.VideoAreas.FirstOrDefault(va => va.ID == areaId);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
            }

            return videoArea;
        }

        #endregion

        #region Group

        public ActionResult Group()
        {
            PopulateCompanies();
            PopulateSbUs();
            PopulateObUs();
            PopulateBUs();
            PopulateBranchs();
            PopulateDepots();
            PopulateAreas();
            return View();
        }

        public ActionResult GroupRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllGroups().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GroupCreate([DataSourceRequest] DataSourceRequest request, GroupViewModel groupViewModel)
        {
            try
            {
                if (groupViewModel != null && ModelState.IsValid)
                {
                    using (var context = new ASJDE())
                    {
                        var videoGroup = new VideoGroup
                            {
                                VideoAreaID = groupViewModel.AreaID,
                                Name = groupViewModel.Name,
                            };

                        context.VideoGroups.Add(videoGroup);
                        context.SaveChanges();

                        groupViewModel.ID = videoGroup.ID;
                    }
                }

                return Json(new[] { groupViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GroupUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, GroupViewModel groupViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = new ASJDE())
                    {
                        var videoGroup = context.VideoGroups.FirstOrDefault(vg => vg.ID == groupViewModel.ID);

                        if (videoGroup != null)
                        {
                            videoGroup.Name = groupViewModel.Name;
                            videoGroup.VideoAreaID = groupViewModel.AreaID;
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
            }

            return Json(new[] { groupViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GroupDestroy([DataSourceRequest] DataSourceRequest request, GroupViewModel groupViewModel)
        {
            try
            {
                if (groupViewModel != null)
                {
                    using (var context = new ASJDE())
                    {
                        var videoGroup = context.VideoGroups.FirstOrDefault(vg => vg.ID == groupViewModel.ID);
                        context.VideoGroups.Remove(videoGroup);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        public void PopulateGroups()
        {
            IQueryable<GroupViewModel> groups = GetAllGroups();
            ViewData["groups"] = groups;
        }

        public IQueryable<GroupViewModel> GetAllGroups()
        {
            var context = new ASJDE();

            var groupViewModels = context.VideoGroups.Select(g => new GroupViewModel
                {
                    ID = g.ID,
                    AreaID = g.VideoAreaID,
                    Name = g.Name,
                    CompanyID = g.VideoArea.CompanyID,
                    StrategicBusinessUnitID = g.VideoArea.StrategicBusinessUnitID,
                    OperationalBusinessUnitID = g.VideoArea.OperationalBusinessUnitID,
                    BusinessUnitID = g.VideoArea.BusinessUnitID,
                    BranchID = g.VideoArea.BranchID,
                    DepotID = g.VideoArea.DepotID
                });

            return groupViewModels;
        }

        public JsonResult ReadAllGroupsByAreaId(int areaId)
        {
            var groups = GetAllGroups();

            return Json(groups.Where(g => g.AreaID == areaId), JsonRequestBehavior.AllowGet);
        }
        
        #endregion

        #region Video

        public ActionResult Index()
        {
            PopulateCompanies();
            PopulateSbUs();
            PopulateObUs();
            PopulateBUs();
            PopulateBranchs();
            PopulateDepots();
            PopulateAreas();
            PopulateGroups();
            return View();
        }

        public ActionResult VideoRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllVideos().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VideoCreate([DataSourceRequest] DataSourceRequest request, VideoViewModel videoViewModel)
        {
            try
            {
                if (videoViewModel != null && ModelState.IsValid)
                {
                    using (var context = new ASJDE())
                    {
                        var video = new Video
                            {
                                VideoGroupID = videoViewModel.GroupID,
                                Name = videoViewModel.Name,
                                ReferenceID = videoViewModel.ReferenceID,
                                CreatedDate = DateTime.Now,
                                CreatedByUser = ""
                            };

                        context.Videos.Add(video);
                        context.SaveChanges();

                        videoViewModel.ID = video.ID;
                    }
                }

                return Json(new[] { videoViewModel }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VideoUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, VideoViewModel videoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = new ASJDE())
                    {
                        var video = context.Videos.FirstOrDefault(v => v.ID == videoViewModel.ID);

                        if (video != null)
                        {
                            video.Name = videoViewModel.Name;
                            video.VideoGroupID = videoViewModel.GroupID;
                            video.ReferenceID = videoViewModel.ReferenceID;
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
            }

            return Json(new[] { videoViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VideoDestroy([DataSourceRequest] DataSourceRequest request, VideoViewModel videoViewModel)
        {
            try
            {
                if (videoViewModel != null)
                {
                    using (var context = new ASJDE())
                    {
                        var video = context.Videos.FirstOrDefault(v => v.ID == videoViewModel.ID);
                        context.Videos.Remove(video);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Exception", e);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        public IQueryable<VideoViewModel> GetAllVideos()
        {
            var context = new ASJDE();

            var videoViewModels = context.Videos.Select(v => new VideoViewModel
                {
                    CompanyID = v.VideoGroup.VideoArea.CompanyID,
                    StrategicBusinessUnitID = v.VideoGroup.VideoArea.StrategicBusinessUnitID,
                    OperationalBusinessUnitID = v.VideoGroup.VideoArea.OperationalBusinessUnitID,
                    BusinessUnitID = v.VideoGroup.VideoArea.BusinessUnitID,
                    BranchID = v.VideoGroup.VideoArea.BranchID,
                    DepotID = v.VideoGroup.VideoArea.DepotID,
                    ID = v.ID,
                    AreaID = v.VideoGroup.VideoAreaID,
                    GroupID = v.VideoGroupID,
                    Name = v.Name,
                    ReferenceID = v.ReferenceID
                });

            return videoViewModels;
        }

        public JsonResult GetGroups(decimal areaId)
        {
            return Json(GroupFiltered(areaId), JsonRequestBehavior.AllowGet);
        }

        public static List<GroupViewModel> GroupFiltered(decimal areaId)
        {
            using (var context = new ASJDE())
            {
                return context.VideoGroups.Where(a => a.VideoAreaID == areaId).Select(a => new GroupViewModel
                    {
                        ID = a.ID,
                        Name = a.Name
                    }).ToList();
            }
        }

        #endregion

        #region Log

        public ActionResult VideoLogRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetAllVideoLogs().ToDataSourceResult(request));
        }

        public IQueryable<LogViewModel> GetAllVideoLogs()
        {
            var context = new ASJDE();

            var logViewModels = context.VideoLogs.Select(vl => new LogViewModel
                {
                    ID = vl.ID,
                    UserName = vl.User,
                });

            return logViewModels;
        }

        #endregion

        #region List

        public ActionResult List(int? id)
        {
            if (id != null)
            {
                var listViewModel = new ListViewModel();

                var videoArea = GetAreaById(id.Value);

                listViewModel.Name = videoArea.Name;
                listViewModel.ID = id.Value;

                return View(listViewModel);
            }

            return null;
        }

        public JsonResult VideoList(int areaID, int? id)
        {
            var context = new ASJDE();

            IQueryable result;

            if (id.HasValue)
            {
                result = context.Videos.Where(v => v.VideoGroupID == id).Select(v => new
                    {
                        id = v.ID,
                        v.Name,
                        hasChildren = false,
                        type = "video",
                    });
            }
            else
            {
                result = context.VideoGroups.Where(vg => vg.VideoAreaID == areaID).Select(vg => new
                    {
                        id = vg.ID,
                        vg.Name,
                        hasChildren = (from v in context.Videos where v.VideoGroupID == vg.ID select v.ID).Any(),
                        type = "group",
                    });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        # region Hierarchy

        public void PopulateCompanies()
        {
            var hierarchyController = new HierarchyController();
            IEnumerable<CompanyViewModel> companyViewModels = hierarchyController.GetAllCompanies();

            ViewData["companies"] = companyViewModels;
        }

        public void PopulateSbUs()
        {
            var hierarchyController = new HierarchyController();
            var strategicBusinessUnitViewModels = hierarchyController.GetAllStrategicBusinessUnits();

            ViewData["strategicBusinessUnits"] = strategicBusinessUnitViewModels;
        }

        
        public void PopulateObUs()
        {
            var hierarchyController = new HierarchyController();
            IEnumerable<OperationalBusinessUnitViewModel> operataionalBusinessUnitViewModels = hierarchyController.GetAllOperationalBusinessUnits();

            ViewData["operationalBusinessUnits"] = operataionalBusinessUnitViewModels;
        }

        public void PopulateBUs()
        {
            var hierarchyController = new HierarchyController();
            IEnumerable<BusinessUnitViewModel> businessUnitViewModels = hierarchyController.GetAllBusinessUnits();

            ViewData["businessUnits"] = businessUnitViewModels;
        }

        public void PopulateBranchs()
        {
            var hierarchyController = new HierarchyController();
            IEnumerable<BranchViewModel> branchViewModels = hierarchyController.GetAllBranchs();

            ViewData["branchViewModels"] = branchViewModels;
        }
        
        public void PopulateDepots()
        {
            var hierarchyController = new HierarchyController();
            IEnumerable<DepotViewModel> depotViewModels = hierarchyController.GetAllDepots();

            ViewData["depotViewModels"] = depotViewModels;
        }

        #endregion

        #region Company

        public ActionResult Company()
        {
            var companyVideoHierarchy = new CompanyVideoHierarchy
            {
                SBUVideoHierarchies = GetSbuVideoHierarchy(),
                AreaViewModels = GetAreaViewModels()
            };

            return View(companyVideoHierarchy);
        }

        public IEnumerable<SBUVideoHierarchy> GetSbuVideoHierarchy()
        {
            var sbuVideoHierarchies = new List<SBUVideoHierarchy>();

            using (var context = new ASJDE())
            {
                var sbusWithVideos = context.VideoAreas.Where(a => a.CompanyID == 1 && a.StrategicBusinessUnitID != null).Select(a => a.StrategicBusinessUnitID).Distinct();

                sbuVideoHierarchies.AddRange(sbusWithVideos.Select(sbusWithVideo => context.StrategicBusinessUnits.FirstOrDefault(s => s.ID == sbusWithVideo)).Select(sbu => new SBUVideoHierarchy
                    {
                        ID = sbu.ID,
                        Name = sbu.Entity.Name, 
                        OBUVideoHierarchies = new List<OBUVideoHierarchy>()
                    }));
            }

            return sbuVideoHierarchies;
        }

        public IEnumerable<AreaViewModel> GetAreaViewModels()
        {
            using (var context = new ASJDE())
            {
                var videoAreas = context.VideoAreas.Where(a => a.CompanyID == 1
                    && a.StrategicBusinessUnitID == null
                    && a.OperationalBusinessUnitID == null
                    && a.BusinessUnitID == null
                    && a.BranchID == null
                    && a.DepotID == null);

                return GetAreaViewModelsByVideoAreas(context, videoAreas);
            }
        }

        public List<AreaViewModel> GetAreaViewModelsByVideoAreas(ASJDE context, IQueryable<VideoArea> videoAreas)
        {
            var areaViewModels = new List<AreaViewModel>();

            foreach (var videoArea in videoAreas)
            {
                var areaViewModel = new AreaViewModel
                {
                    ID = videoArea.ID,
                    Name = videoArea.Name,
                    GroupViewModels = new List<GroupViewModel>()
                };

                var area = videoArea;

                var groups = context.VideoGroups.Where(g => g.VideoAreaID == area.ID);

                foreach (var videoGroup in groups)
                {
                    var groupViewModel = new GroupViewModel
                    {
                        ID = videoGroup.ID,
                        Name = videoGroup.Name,
                        VideoViewModels = new List<VideoViewModel>()
                    };

                    var vGroup = videoGroup;
                    var videos = context.Videos.Where(v => v.VideoGroupID == vGroup.ID);

                    foreach (var videoViewModel in videos.Select(video => new VideoViewModel
                        {
                            ID = video.ID,
                            Name = video.Name,
                            ReferenceID = video.ReferenceID,
                            Type = "video",
                        }))
                    {
                        groupViewModel.VideoViewModels.Add(videoViewModel);
                    }

                    areaViewModel.GroupViewModels.Add(groupViewModel);
                }

                areaViewModels.Add(areaViewModel);
            }

            return areaViewModels;
        }

        #endregion
    }
}
