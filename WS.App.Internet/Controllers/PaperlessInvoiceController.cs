using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Devart.Data.Oracle;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using WS.App.Internet.ViewModels.PaperlessInvoice;
using WS.Framework;
using WS.Framework.Objects.Emails;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;


namespace WS.App.Internet.Controllers
{
    public class PaperlessInvoiceController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEmailService _emailService;

        public PaperlessInvoiceController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public ActionResult Index()
        {
            Request request = new Request();

            return View(request);
        }

        [HttpPost]
        public ActionResult AddPaperlessRequest(Request request)
        {

            if (request.Orders.Count == 0)
            {
                return Json(new { success = false });
            }

            //request = UpdateRequestWithOrders(request);

            AddPaperlessRequestRecord(request);

            SendRequestEmail(request);

            return Json(new { success = true });

        }




        public ActionResult Confirmation(Confirmation confirmation)
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReadUnits([DataSourceRequest]DataSourceRequest request, int accountNumber)
        {
            if (accountNumber == -1)
            {
                return null;
                //return Json(EmptyGrid().ToDataSourceResult(request));
            }

            return Json(GetUnits(accountNumber).ToDataSourceResult(request));
        }

        private static IEnumerable<Unit> EmptyGrid()
        {
            List<Unit> units = new List<Unit>();
            Unit unit = new Unit();
            units.Add(unit);

            return units;
        }

        public Request UpdateRequestWithOrders(Request request)
        {
            List<Unit> units = new List<Unit>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    foreach (int order in request.Orders)
                    {
                        var unitNumber = (from ou in context.OMBUnits
                                        join a in context.Assets on ou.AssetID equals a.ID
                                        where ou.OMBOrderHeaderID == order
                                        select a.UnitNumber).FirstOrDefault();

                        Unit unit = new Unit
                            {
                                OrderNumber = order,
                                UnitNumber = unitNumber
                            };

                        units.Add(unit);
                    }
                }

                request.Units = units;
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return request;
        }

        public void SendRequestEmail(Request request)
        {
            PaperlessInvoiceEmail paperlessInvoiceEmail = new PaperlessInvoiceEmail
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    RequesterEmail = request.RequesterEmail,
                    PhoneNumer = request.PhoneNumer,
                    AccountNumber = request.AccountNumber,
                    Email = request.Email,
                    Units = string.Join(";\r\n", request.Orders)
                };

            _emailService.PaperlessRequest(paperlessInvoiceEmail);

        }

        public void AddPaperlessRequestRecord(Request request)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    PaperlessRequest paperlessRequest = new PaperlessRequest
                        {
                            AccountNumber = Convert.ToInt32(request.AccountNumber),
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            RequesterEmail = request.RequesterEmail,
                            PhoneNumber = request.PhoneNumer,
                            SendToEmail = request.Email,
                            CreateDate = DateTime.Now
                        };
                    context.PaperlessRequests.Add(paperlessRequest);
                    context.SaveChanges();

                    List<PaperlessRequestOrder> paperlessRequestOrders = new List<PaperlessRequestOrder>();

                    foreach (int order in request.Orders)
                    {
                        PaperlessRequestOrder paperlessRequestOrder = new PaperlessRequestOrder
                            {
                                PaperlessRequestID = paperlessRequest.ID,
                                OrderNumber = order.ToString()
                            };

                        paperlessRequestOrders.Add(paperlessRequestOrder);
                    }

                    paperlessRequestOrders.ForEach(c => context.PaperlessRequestOrders.Add(c));
                    context.SaveChanges();

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
        }

        private static IEnumerable<Unit> GetUnits(int accountNumber)
        {
            WSJDE context = new WSJDE();

            List<Unit> units = new List<Unit>();


            try
            {
                ObjectResult<Unit> x = (context as IObjectContextAdapter).ObjectContext.ExecuteStoreQuery<Unit>(@"
                    SELECT OMBOrderHeader.order_skey AS OrderNumber,
                       (SELECT wwmlnm
                          FROM f0111
                         WHERE     wwan8 = :accountNumber
                               AND wwtyc = 'N'
                               AND wwidln = OMBOrderHeader.attention_to_idln) AS Attention,
                       Asset.faapid AS UnitNumber,
                       AddressByDate.aladd1 as Address,
                       AddressByDate.alcty1 as City,
                       AddressByDate.aladds as State,
                       AddressByDate.aladdz as Zip
                    FROM omb_order_header OMBOrderHeader
                       JOIN f0101 AddressBookMaster
                          ON OMBOrderHeader.ship_an8 = AddressBookMaster.aban8
                       JOIN omb_unit OMBUnit
                          ON OMBOrderHeader.order_skey = OMBUnit.order_skey
                       JOIN f1201 Asset 
                          ON OMBUnit.fanumb = Asset.fanumb
                       JOIN f0116 AddressByDate
                          ON OMBOrderHeader.ship_an8 = AddressByDate.alan8
                    WHERE     
                       AddressBookMaster.aban85 = :accountNumber
                       AND 
                       OMBOrderHeader.q_status_id = 11",
                   new OracleParameter { ParameterName = "accountNumber", Value = accountNumber });
                
                    units = x.ToList();
                    return units;

            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return units;
        }
    }
}
