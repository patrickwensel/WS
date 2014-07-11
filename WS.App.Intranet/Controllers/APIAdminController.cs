using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using WS.App.Intranet.ViewModels.APIAdmin;
using WS.Framework.WSJDEData;

namespace WS.App.Intranet.Controllers
{
    public class APIAdminController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Authorize(Roles = "Secure API Administrator")]
        public ActionResult Index()
        {
            PopulateSources();
            PopulateRequestSources();
            PopulateRequestDataKeys();
            PopulateRequestDataKeyClasses();

            return View();
        }

        #region Request

        public ActionResult RequestRead([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            return Json(GetAllRequests().ToDataSourceResult(dataSourceRequest));
        }

        public void PopulateRequests()
        {
            IQueryable<RequestViewModel> requests = GetAllRequests();

            ViewData["requests"] = requests;
        }

        public JsonResult ReadAllRequests()
        {
            return Json(GetAllRequests(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<RequestViewModel> GetAllRequests()
        {
            WSJDE context = new WSJDE();

            IQueryable<RequestViewModel> requestViewModels = (from a in context.APIRequests
                                                              select new RequestViewModel
                                                              {
                                                                  ID = a.ID,
                                                                  APISourceID = a.APISourceID,
                                                                  APIRequestSourceID = a.APIRequestSourceID,
                                                                  FirstName = a.FirstName,
                                                                  LastName = a.LastName,
                                                                  Company = a.Company,
                                                                  Address1 = a.Address1,
                                                                  Address2 = a.Address2,
                                                                  City = a.City,
                                                                  State = a.State,
                                                                  Zip = a.Zip,
                                                                  Phone = a.Phone,
                                                                  Email = a.Email,
                                                                  CreateDate = a.CreateDate
                                                              }
                                                             );

            return requestViewModels;
        }

        #endregion

        #region RequestData

        public ActionResult GetRequestDataByRequestID(int requestID, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            return Json(GetAllRequestDatas()
                .Where(request => request.APIRequestID == requestID)
                .ToDataSourceResult(dataSourceRequest));
        }

        public IQueryable<RequestDataViewModel> GetAllRequestDatas()
        {
            WSJDE context = new WSJDE();

            IQueryable<RequestDataViewModel> requestDataViewModel = (from a in context.APIRequestDatas
                                                                     select new RequestDataViewModel
                                                                         {
                                                                             ID = a.ID,
                                                                             APIRequestID = a.APIRequestID,
                                                                             APIRequestDataKeyID = a.APIRequestDataKeyID,
                                                                             Name = a.Name ?? a.APIRequestDataKey.APIRequestDataKeyClass.Name + ": " + a.APIRequestDataKey.Name,
                                                                             Value = a.Value

                                                                         }
                                                                    );

            return requestDataViewModel;
        }

        #endregion

        #region Source

        public ActionResult SourceRead([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            return Json(GetAllSources().ToDataSourceResult(dataSourceRequest));
        }

        public void PopulateSources()
        {
            IQueryable<SourceViewModel> sources = GetAllSources();

            ViewData["sources"] = sources;
        }

        public JsonResult ReadAllSources()
        {
            return Json(GetAllSources(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<SourceViewModel> GetAllSources()
        {
            WSJDE context = new WSJDE();

            IQueryable<SourceViewModel> sourceViewModels = (from a in context.APISources
                                                            select new SourceViewModel
                                                            {
                                                                ID = a.ID,
                                                                Name = a.Name,
                                                                Token = a.Token
                                                            }
                                                             );

            return sourceViewModels;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SourceCreate([DataSourceRequest] DataSourceRequest request, SourceViewModel sourceViewModel)
        {
            try
            {
                if (sourceViewModel != null && ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {

                        Guid guid = Guid.NewGuid();

                        APISource apiSource = new APISource
                        {
                            Token = guid.ToString(),
                            Name = sourceViewModel.Name
                        };

                        context.APISources.Add(apiSource);
                        context.SaveChanges();
                        sourceViewModel.ID = apiSource.ID;
                        sourceViewModel.Token = apiSource.Token;

                    }
                }

                return Json(new[] { sourceViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SourceUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, SourceViewModel sourceViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {

                        APISource source = (from s in context.APISources
                                            where s.ID == sourceViewModel.ID
                                            select s).FirstOrDefault();

                        if (source != null)
                        {
                            source.Name = sourceViewModel.Name;

                            context.SaveChanges();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
            return Json(new[] { sourceViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SourceDestroy([DataSourceRequest] DataSourceRequest request, SourceViewModel sourceViewModel)
        {
            try
            {
                if (sourceViewModel != null)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        APISource source = (from s in context.APISources
                                            where s.ID == sourceViewModel.ID
                                            select s).FirstOrDefault();
                        context.APISources.Remove(source);
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

        #endregion

        #region RequestSource 
        // 1= Quote; 2= Service; 3 = General
        public ActionResult RequestSourceRead([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            return Json(GetAllRequestSources().ToDataSourceResult(dataSourceRequest));
        }

        public void PopulateRequestSources()
        {
            IQueryable<RequestSourceViewModel> requestSources = GetAllRequestSources();

            ViewData["requestSources"] = requestSources;
        }

        public JsonResult ReadAllRequestSources()
        {
            return Json(GetAllRequestSources(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<RequestSourceViewModel> GetAllRequestSources()
        {
            WSJDE context = new WSJDE();

            IQueryable<RequestSourceViewModel> requestSourceViewModels = (from a in context.APIRequestSources
                                                                          select new RequestSourceViewModel
                                                                              {
                                                                                  ID = a.ID,
                                                                                  Name = a.Name
                                                                              }
                                                                         );

            return requestSourceViewModels;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestSourceCreate([DataSourceRequest] DataSourceRequest request, RequestSourceViewModel requestSourceViewModel)
        {
            try
            {
                if (requestSourceViewModel != null && ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        APIRequestSource apiRequestSource = new APIRequestSource
                        {
                            ID = requestSourceViewModel.ID,
                            Name = requestSourceViewModel.Name
                        };

                        context.APIRequestSources.Add(apiRequestSource);
                        context.SaveChanges();

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
        public ActionResult RequestSourceUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, RequestSourceViewModel requestSourceViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {

                        APIRequestSource apiRequestSource = (from s in context.APIRequestSources
                                            where s.ID == requestSourceViewModel.ID
                                            select s).FirstOrDefault();

                        if (apiRequestSource != null)
                        {
                            apiRequestSource.Name = requestSourceViewModel.Name;

                            context.SaveChanges();
                        }

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
        public ActionResult RequestSourceDestroy([DataSourceRequest] DataSourceRequest request, RequestSourceViewModel requestSourceViewModel)
        {
            try
            {
                if (requestSourceViewModel != null)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        APIRequestSource source = (from s in context.APIRequestSources
                                                   where s.ID == requestSourceViewModel.ID
                                            select s).FirstOrDefault();
                        context.APIRequestSources.Remove(source);
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

        #endregion
        
        #region RequestDataKey

        public ActionResult RequestDataKeyRead([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            return Json(GetAllRequestDataKeys().ToDataSourceResult(dataSourceRequest));
        }

        public void PopulateRequestDataKeys()
        {
            IQueryable<RequestDataKeyViewModel> requestDataKeys = GetAllRequestDataKeys();

            ViewData["requestDataKeys"] = requestDataKeys;
        }

        public JsonResult ReadAllRequestDataKeys()
        {
            return Json(GetAllSources(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<RequestDataKeyViewModel> GetAllRequestDataKeys()
        {
            WSJDE context = new WSJDE();

            IQueryable<RequestDataKeyViewModel> requestDataKeyViewModels = (from a in context.APIRequestDataKeys
                                                                           select new RequestDataKeyViewModel
                                                                          {
                                                                              ID = a.ID,
                                                                              APIRequestDataKeyClassID = a.APIRequestDataKeyClassID,
                                                                              Name = a.Name,
                                                                              Description = a.Description,
                                                                              ValueRequired = a.ValueRequired
                                                                          }
                                                                         );

            return requestDataKeyViewModels;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestDataKeyCreate([DataSourceRequest] DataSourceRequest request, RequestDataKeyViewModel requestDataKeyViewModel)
        {
            try
            {
                if (requestDataKeyViewModel != null && ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        APIRequestDataKey apiRequestDataKey = new APIRequestDataKey
                        {
                            ID = requestDataKeyViewModel.ID,
                            APIRequestDataKeyClassID = requestDataKeyViewModel.APIRequestDataKeyClassID,
                            Name = requestDataKeyViewModel.Name,
                            ValueRequired = requestDataKeyViewModel.ValueRequired
                        };

                        context.APIRequestDataKeys.Add(apiRequestDataKey);
                        context.SaveChanges();

                    }
                }

                return Json(new[] { requestDataKeyViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestDataKeyUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, RequestDataKeyViewModel requestDataKeyViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {

                        APIRequestDataKey apiRequestDataKey = (from s in context.APIRequestDataKeys
                                                             where s.ID == requestDataKeyViewModel.ID
                                                             select s).FirstOrDefault();

                        if (apiRequestDataKey != null)
                        {
                            apiRequestDataKey.ID = requestDataKeyViewModel.ID;
                            apiRequestDataKey.Name = requestDataKeyViewModel.Name;
                            apiRequestDataKey.APIRequestDataKeyClassID =
                                requestDataKeyViewModel.APIRequestDataKeyClassID;
                            apiRequestDataKey.ValueRequired = requestDataKeyViewModel.ValueRequired;


                            context.SaveChanges();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
            return Json(new[] { requestDataKeyViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestDataKeyDestroy([DataSourceRequest] DataSourceRequest request, RequestDataKeyViewModel requestDataKeyViewModel)
        {
            try
            {
                if (requestDataKeyViewModel != null)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        APIRequestDataKey apiRequestDataKey = (from s in context.APIRequestDataKeys
                                                               where s.ID == requestDataKeyViewModel.ID
                                                               select s).FirstOrDefault();

                        context.APIRequestDataKeys.Remove(apiRequestDataKey);
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

        #endregion
        
        #region RequestDataKeyClass

        public ActionResult RequestDataKeyClassRead([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            return Json(GetAllRequestDataKeyClasses().ToDataSourceResult(dataSourceRequest));
        }

        public void PopulateRequestDataKeyClasses()
        {
            IQueryable<RequestDataKeyClassViewModel> requestDataKeyClasses = GetAllRequestDataKeyClasses();

            ViewData["requestDataKeyClasses"] = requestDataKeyClasses;
        }

        public JsonResult ReadAllRequestDataKeyClasses()
        {
            return Json(GetAllRequestDataKeyClasses(), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<RequestDataKeyClassViewModel> GetAllRequestDataKeyClasses()
        {
            WSJDE context = new WSJDE();

            IQueryable<RequestDataKeyClassViewModel> requestDataKeyClassViewModels =
                (from a in context.APIRequestDataKeyClasses
                 select new RequestDataKeyClassViewModel
                     {
                         ID = a.ID,
                         Name = a.Name
                     }
                );

            return requestDataKeyClassViewModels;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestDataKeyClassCreate([DataSourceRequest] DataSourceRequest request, RequestDataKeyClassViewModel requestDataKeyClassViewModel)
        {
            try
            {
                if (requestDataKeyClassViewModel != null && ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        APIRequestDataKeyClass apiRequestDataKeyClass = new APIRequestDataKeyClass
                        {
                            ID = requestDataKeyClassViewModel.ID,
                            Name = requestDataKeyClassViewModel.Name
                        };

                        context.APIRequestDataKeyClasses.Add(apiRequestDataKeyClass);
                        context.SaveChanges();

                    }
                }

                return Json(new[] { requestDataKeyClassViewModel }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestDataKeyClassUpdate([DataSourceRequest] DataSourceRequest dataSourceRequest, RequestDataKeyClassViewModel requestDataKeyClassViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WSJDE context = new WSJDE())
                    {

                        APIRequestDataKeyClass apiRequestDataKeyClass = (from s in context.APIRequestDataKeyClasses
                                                                         where s.ID == requestDataKeyClassViewModel.ID
                                                                         select s).FirstOrDefault();

                        if (apiRequestDataKeyClass != null)
                        {
                            apiRequestDataKeyClass.ID = requestDataKeyClassViewModel.ID;
                            apiRequestDataKeyClass.Name = requestDataKeyClassViewModel.Name;

                            context.SaveChanges();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
            return Json(new[] { requestDataKeyClassViewModel }.ToDataSourceResult(dataSourceRequest, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestDataKeyClassDestroy([DataSourceRequest] DataSourceRequest request, RequestDataKeyClassViewModel requestDataKeyClassViewModel)
        {
            try
            {
                if (requestDataKeyClassViewModel != null)
                {
                    using (WSJDE context = new WSJDE())
                    {
                        APIRequestDataKeyClass apiRequestDataKeyClass = (from s in context.APIRequestDataKeyClasses
                                                                         where s.ID == requestDataKeyClassViewModel.ID
                                                               select s).FirstOrDefault();

                        context.APIRequestDataKeyClasses.Remove(apiRequestDataKeyClass);
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

        #endregion

    }
}