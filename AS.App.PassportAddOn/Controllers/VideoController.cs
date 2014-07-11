using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AS.App.PassportAddOn.Objects;
using AS.App.PassportAddOn.ViewModels.Video;
using NLog;
using Plumtree.Remote.Portlet;
using WS.Framework.ASJDE;

namespace AS.App.PassportAddOn.Controllers
{
    public class VideoController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Company()
        {
            ViewData["webService"] = ConfigurationManager.AppSettings["webService"];


            string environment = ConfigurationManager.AppSettings["environment"];
            if (environment != "Dev")
            {
                PortalUser portalUser = new PortalUser();

                HttpRequest httpRequest = GetHttpRequestFromHttpResponseBase(Request);
                HttpResponse httpResponse = GetHttpResponseFromHttpResponseBase(Response);

                portalUser = GetPortalUser(PortletContextFactory.CreatePortletContext(httpRequest, httpResponse));

                if (portalUser != null)
                {
                    ViewData["user"] = portalUser.UserName;
                }
            }

            CompanyVideoHierarchy companyVideoHierarchy = new CompanyVideoHierarchy
            {
                SBUVideoHierarchies = GetSBUVideoHierarchiy(),
                AreaViewModels = GetAreaViewModels()
            };

            return View(companyVideoHierarchy);
        }

        public IEnumerable<SBUVideoHierarchy> GetSBUVideoHierarchiy()
        {
            List<SBUVideoHierarchy> sbuVideoHierarchies = new List<SBUVideoHierarchy>();

            using (ASJDE context = new ASJDE())
            {
                IQueryable<decimal?> sbusWithVideos = (from a in context.VideoAreas
                                                       where a.CompanyID == 1
                                                             && a.StrategicBusinessUnitID != null
                                                       select a.StrategicBusinessUnitID).Distinct();

                foreach (decimal? sbusWithVideo in sbusWithVideos)
                {
                    StrategicBusinessUnit sbu = (from s in context.StrategicBusinessUnits
                                                 where s.ID == sbusWithVideo
                                                 select s).FirstOrDefault();

                    SBUVideoHierarchy sbuVideoHierarchy = new SBUVideoHierarchy
                    {
                        ID = sbu.ID,
                        Name = sbu.Entity.Name,
                        OBUVideoHierarchies = new List<OBUVideoHierarchy>()
                    };



                    sbuVideoHierarchies.Add(sbuVideoHierarchy);
                }

            }

            return sbuVideoHierarchies;
        }

        public IEnumerable<AreaViewModel> GetAreaViewModels()
        {
            List<AreaViewModel> areaViewModels = new List<AreaViewModel>();

            using (ASJDE context = new ASJDE())
            {

                IQueryable<VideoArea> videoAreas = (from a in context.VideoAreas
                                                    where a.CompanyID == 1
                                                    && a.StrategicBusinessUnitID == null
                                                    && a.OperationalBusinessUnitID == null
                                                    && a.BusinessUnitID == null
                                                    && a.BranchID == null
                                                    && a.DepotID == null
                                                    select a);

                areaViewModels = GetAreaViewModelsByVideoAreas(context, videoAreas);

            }

            return areaViewModels;
        }

        public List<AreaViewModel> GetAreaViewModelsByVideoAreas(ASJDE context, IQueryable<VideoArea> videoAreas)
        {
            List<AreaViewModel> areaViewModels = new List<AreaViewModel>();

            foreach (VideoArea videoArea in videoAreas)
            {
                AreaViewModel areaViewModel = new AreaViewModel
                {
                    ID = videoArea.ID,
                    Name = videoArea.Name,
                    GroupViewModels = new List<GroupViewModel>()
                };

                VideoArea area = videoArea;

                var groups = (from g in context.VideoGroups
                              where g.VideoAreaID == area.ID
                              select g);

                foreach (VideoGroup videoGroup in groups)
                {
                    GroupViewModel groupViewModel = new GroupViewModel
                    {
                        ID = videoGroup.ID,
                        Name = videoGroup.Name,
                        VideoViewModels = new List<VideoViewModel>()
                    };

                    VideoGroup vGroup = videoGroup;
                    var videos = (from v in context.Videos
                                  where v.VideoGroupID == vGroup.ID
                                  select v);

                    foreach (Video video in videos)
                    {
                        VideoViewModel videoViewModel = new VideoViewModel
                        {
                            ID = video.ID,
                            Name = video.Name,
                            ReferenceID = video.ReferenceID,
                            Type = "video",
                        };

                        groupViewModel.VideoViewModels.Add(videoViewModel);

                    }

                    areaViewModel.GroupViewModels.Add(groupViewModel);

                }

                areaViewModels.Add(areaViewModel);

            }

            return areaViewModels;
        }

        public ActionResult VideoPlayer(string id)
        {
            string[] partsOfID = id.Split(new[] { '|' });

            string environment = ConfigurationManager.AppSettings["environment"];

            Video video = GetVideoByID(Convert.ToInt64(partsOfID[0]));

            VideoPlayerViewModel videoPlayerViewModel = new VideoPlayerViewModel
            {
                Name = video.Name,
                ReferenceID = video.ReferenceID,
                VideoPlayer = BuildPlayerJavaScript(video.ReferenceID)
            };

            if (environment != "Dev")
            {
                using (ASJDE context = new ASJDE())
                {
                    VideoLog videoLog = new VideoLog
                        {
                            VideoID = Convert.ToInt64(partsOfID[0]),
                            User = partsOfID[1],
                            TimeStamp = DateTime.Now
                        };

                    context.VideoLogs.Add(videoLog);
                    context.SaveChanges();
                }

            }

            return View(videoPlayerViewModel);

        }

        public string BuildPlayerJavaScript(decimal id)
        {
            StringBuilder js = new StringBuilder();

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

            return js.ToString();
        }
        public Video GetVideoByID(long referenceID)
        {
            try
            {
                using (ASJDE context = new ASJDE())
                {
                    Video video = (from v in context.Videos
                                   where v.ID == referenceID
                                   select v).FirstOrDefault();

                    return video;
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                return null;
            }
        }

        public void LogUser(Request request)
        {
            PortalUser portalUser = new PortalUser();

            HttpRequest httpRequest = GetHttpRequestFromHttpResponseBase(Request);
            HttpResponse httpResponse = GetHttpResponseFromHttpResponseBase(Response);

            portalUser = GetPortalUser(PortletContextFactory.CreatePortletContext(httpRequest, httpResponse));

            if (portalUser != null)
            {
                using (ASJDE context = new ASJDE())
                {
                    VideoLog videoLog = new VideoLog
                        {
                            VideoID = request.VideoID,
                            User = request.User,
                            TimeStamp = DateTime.Now
                        };

                    context.VideoLogs.Add(videoLog);
                    context.SaveChanges();
                }
            }
        }



       public HttpResponse GetHttpResponseFromHttpResponseBase(HttpResponseBase request)
       {
           FieldInfo fieldInfo = request.GetType().GetField("_httpResponse", BindingFlags.NonPublic | BindingFlags.Instance);
           if (null != fieldInfo)
           {
               return fieldInfo.GetValue(request) as HttpResponse;
           }
           return null;
       }

       public HttpRequest GetHttpRequestFromHttpResponseBase(HttpRequestBase request)
       {
           FieldInfo fieldInfo = request.GetType().GetField("_httpRequest", BindingFlags.NonPublic | BindingFlags.Instance);
           if (null != fieldInfo)
           {
               return fieldInfo.GetValue(request) as HttpRequest;
           }
           return null;
       }

       public PortalUser GetPortalUser(IPortletContext thePortletContext)
       {
           PortalUser portalUser = new PortalUser();

           if (thePortletContext != null)
           {
               try
               {
                   IPortletRequest aPortletRequest = thePortletContext.GetRequest();

                   portalUser.User = thePortletContext.GetUser().GetUserID().ToString();
                   portalUser.Email = aPortletRequest.GetSettingValue(SettingType.UserInfo, "Email");
                   portalUser.UserName = aPortletRequest.GetSettingValue(SettingType.UserInfo, "FullName");
                   portalUser.Region = aPortletRequest.GetSettingValue(SettingType.UserInfo, "Region") != null ? aPortletRequest.GetSettingValue(SettingType.UserInfo, "Region").ToUpper() : "";
               }
               catch (Exception ex)
               {
                   portalUser.User = ConfigurationManager.AppSettings["adminPortalID"];
                   portalUser.Email = ConfigurationManager.AppSettings["adminEmail"];
                   portalUser.UserName = ConfigurationManager.AppSettings["adminName"];
                   portalUser.Region = ConfigurationManager.AppSettings["adminRegion"];
               }
           }

           return portalUser;
       }
    }

    public class Request
    {
        public long VideoID { get; set; }
        public string User { get; set; }
    }
}
