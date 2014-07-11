using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;
using Newtonsoft.Json;
using WS.API.Infrastructure;
using WS.Framework.Objects;
using WS.Framework.Objects.MobileWorkOrder;
using WS.Framework.Objects.Security;
using WS.Framework.Objects.WorkOrder;
using WS.Framework.ServicesInterface;
using Attribute = WS.Framework.Objects.WorkOrder.Attribute;

namespace WS.API.Controllers
{
    public class WorkOrdersController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ISecurityService _securityService;
        private readonly IUnitService _unitService;
        private readonly IBusinessUnitService _businessUnitService;
        private readonly IItemService _itemService;
        private readonly IWorkOrderService _workOrderService;
        private readonly IUserDefinedCodeService _userDefinedCodeService;

        public WorkOrdersController(ISecurityService securityService, IUnitService unitService, IBusinessUnitService businessUnitService, IItemService itemService, IWorkOrderService workOrderService, IUserDefinedCodeService userDefinedCodeService)
        {
            _securityService = securityService;
            _unitService = unitService;
            _businessUnitService = businessUnitService;
            _itemService = itemService;
            _workOrderService = workOrderService;
            _userDefinedCodeService = userDefinedCodeService;
        }

        [HttpGet]
        public object Unit()
        {
            IEnumerable<KeyValuePair<string, string>> qs = Request.GetQueryNameValuePairs();

            var unitNumber = qs.Where(x => x.Key == "unitNumber").Select(x => x.Value).FirstOrDefault();
            var versionNumber = qs.Where(x => x.Key == "version").Select(x => x.Value).FirstOrDefault();

            if (String.IsNullOrEmpty(unitNumber))
                throw new HttpResponseException(HttpStatusCode.BadRequest);


            Unit unit = new Unit();

            if (IsThisCurrentVersion(versionNumber))
            {
                unit = _unitService.GetUnitByUnitNumber(unitNumber);
            }

            else
            {
                logger.Info("Attempted to Login but was running an old version.");
                unit.UnitReturnCode = 2;
                unit.UnitReturnCodeMessage = ConfigurationManager.AppSettings.Get("AuthenticationCode5Message");
            }

            return unit;
        }

        [HttpGet]
        public object OMBOrders()
        {
            IEnumerable<KeyValuePair<string, string>> qs = Request.GetQueryNameValuePairs();

            var unitNumber = qs.Where(x => x.Key == "unitNumber").Select(x => x.Value).FirstOrDefault();
            var versionNumber = qs.Where(x => x.Key == "version").Select(x => x.Value).FirstOrDefault();

            if (String.IsNullOrEmpty(unitNumber))
                throw new HttpResponseException(HttpStatusCode.BadRequest);


            List<OMBOrder> ombOrders = new List<OMBOrder>();

            if (IsThisCurrentVersion(versionNumber))
            {
                ombOrders = _unitService.GetLast2OMBOrdersByUnitNumber(unitNumber);
            }

            else
            {
                logger.Info("Attempted to Login but was running an old version.");
            }

            return ombOrders;
        }

        [HttpPost]
        public async Task<object> Login()
        {
            var form = await Request.Content.ReadAsFormDataAsync();

            string employeeNumber = form["employeeNumber"];
            var password = form["password"];
            string versionNumber = form["versionNumber"];

            SecurityTablet securityTablet;

            if (IsThisCurrentVersion(versionNumber))
            {
                securityTablet = _securityService.GetSecurityTabletByEmployeeNumber(employeeNumber, password);
            }

            else
            {
                securityTablet = new SecurityTablet
                    {
                        AuthenticationCode = 2,
                        ReportLocationNumber = null,
                        AuthenticationCodeMessage = ConfigurationManager.AppSettings.Get("AuthenticationCode5Message")
                    };

                logger.Info("Attempted to Login but was running an old version. Employee #: " + employeeNumber);
            }

            return securityTablet;
        }

        [HttpGet]
        public object BusinessUnits()
        {
            List<BusinessUnitShort> businessUnitShorts = _businessUnitService.GetAllBusinessUnitShorts();

            return businessUnitShorts;


        }

        [HttpGet]
        public object Activities()
        {
            IEnumerable<KeyValuePair<string, string>> qs = Request.GetQueryNameValuePairs();

            KeyValuePair<string, string> businessUnitID = qs.FirstOrDefault(x => x.Key == "businessUnitID");

            List<ItemActivity> activities = _itemService.GetItemActivitiesByBusinessUnitID(businessUnitID.Value);

            return activities;

        }

        [HttpGet]
        public object Parts()
        {
            IEnumerable<KeyValuePair<string, string>> qs = Request.GetQueryNameValuePairs();

            KeyValuePair<string, string> businessUnitID = qs.FirstOrDefault(x => x.Key == "businessUnitID");

            List<ItemPart> parts = _itemService.GetItemPartsByBusinessUnitID(businessUnitID.Value);

            return parts;
        }

        [HttpPost]
        public async Task<object> AddWorkOrder()
        {
            string jsonWorkOrder = await Request.Content.ReadAsStringAsync();

            logger.Debug(jsonWorkOrder);

            MobileWorkOrder mobileWorkOrder = JsonConvert.DeserializeObject<MobileWorkOrder>(jsonWorkOrder);

            WorkOrderResponse workOrderResponse = new WorkOrderResponse();

            if (IsThisCurrentVersion(mobileWorkOrder.VersionNumber))
            {
                workOrderResponse = _workOrderService.AddUpdateWorkOrder(mobileWorkOrder);
            }

            else
            {
                workOrderResponse.WorkOrderID = 0;
                workOrderResponse.Success = false;
                workOrderResponse.ReturnCode = 2;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = ConfigurationManager.AppSettings.Get("AuthenticationCode5Message") });

                logger.Info("Attempted to Login but was running an old version. Employee #: " + mobileWorkOrder.EmployeeNumber);
            }

            return workOrderResponse;
        }

        [HttpPost]
        public Task<object> AddWorkOrderImages()
        {
            logger.Debug("Start: Work Order Image Service");
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var qs = Request.GetQueryNameValuePairs();

            var guid = qs.Where(x => x.Key == "guid").Select(x => x.Value).FirstOrDefault();
            if (String.IsNullOrEmpty(guid))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var imageFileSavePath = ConfigurationManager.AppSettings.Get("FeithImageStagingFileLocation");
            var rootPath = Path.Combine(imageFileSavePath, guid);

            var provider = new MultipartContentDispositionAwareFileStreamProvider(rootPath);

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            return Request.Content
                .ReadAsMultipartAsync(provider)
                .ContinueWith<object>(t =>
                {
                    if (t.IsFaulted)
                    {
                        foreach (var exc in t.Exception.InnerExceptions)
                        {
                            logger.LogException(LogLevel.Warn, exc.Message, exc);
                        }

                        Directory.Delete(rootPath);
                    }
                    else
                    {
                        //_workOrderService.ProcessMWOImages(rootPath);
                        logger.Debug("Done: Work Order Image Service");
                    }

                    return new { Success = !(t.IsFaulted || t.IsCanceled) };
                });

        }

        [HttpGet]
        public object ImageByteCount()
        {
            ImageByteResponse imageByteResponse = new ImageByteResponse();
            IEnumerable<KeyValuePair<string, string>> qs = Request.GetQueryNameValuePairs();

            KeyValuePair<string, string> imageByteCount = qs.FirstOrDefault(x => x.Key == "imageByteCount");
            KeyValuePair<string, string> guid = qs.FirstOrDefault(x => x.Key == "guid");
            KeyValuePair<string, string> id = qs.FirstOrDefault(x => x.Key == "id");

            var imageFileSavePath = ConfigurationManager.AppSettings.Get("FeithImageStagingFileLocation");
            var rootPath = Path.Combine(imageFileSavePath, guid.Value);
            var imagePath = Path.Combine(rootPath, id.Value + ".jpg");

            if (File.Exists(imagePath))
            {
                FileInfo file = new FileInfo(imagePath);
                var fileImageByteCount = file.Length;

                if (Convert.ToInt32(imageByteCount.Value) == fileImageByteCount)
                {
                    imageByteResponse.Complete = true;
                    _workOrderService.ProcessMWOImages(rootPath);
                }
                else
                {
                    imageByteResponse.Complete = false;
                }
            }
            else
            {
                imageByteResponse.Complete = false;
            }

            return imageByteResponse;
        }


        [HttpGet]
        public object WorkOrderTypes()
        {
            List<UserDefinedCodeWorkOrder> userDefinedCodeWorkOrders = _userDefinedCodeService.GetUserDefinedCodeWorkOrder();

            return userDefinedCodeWorkOrders;

        }
        

        [HttpGet]
        public object GetAttributes()
        {
            List<Attribute> attributes = _userDefinedCodeService.GetAllAttibutes();

            return attributes;

        }

        private bool IsThisCurrentVersion(string versionNumber)
        {
            bool isCurrentVersion = true;

            if (string.IsNullOrWhiteSpace(versionNumber))
            {
                isCurrentVersion = false;
            }

            else
            {
                isCurrentVersion = _securityService.IsThisTheCurrentVersion(versionNumber);
            }

            return isCurrentVersion;

        }
    }

    public class Test
    {
        public string Message { get; set; }
    }
}



//using (StreamWriter file = new StreamWriter(@"C:\Temp\ByteCount.txt"))
//{
//    file.WriteLine(imageByteCount);
//    file.WriteLine(guid);
//    file.WriteLine(id);
//}