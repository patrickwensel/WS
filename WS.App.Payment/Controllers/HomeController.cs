using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using NLog;
using WS.App.Payment.ViewModels;
using WS.Framework.Data;
using WS.Framework.Extensions;
using WS.Framework.Objects;
using WS.Framework.Objects.Emails;
using WS.Framework.Objects.Enums;
using WS.Framework.ServicesInterface;

namespace WS.App.Payment.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ICustomerLedgerService _customerLedgerService;
        private readonly IBOAService _boaService;
        private readonly IEmailService _emailService;
        private readonly IApplicationLogCodeService _applicationLogCodeService;
        private readonly ISecurityService _securityService;
        private readonly IOracleHelperService _oracleHelperService;
        private readonly IAddressBookService _addressBookService;
        private readonly ICustomerActivityLogService _customerActivityLogService;
        private readonly IMediaObjectStorageService _mediaObjectStorageService;

        public HomeController(ICustomerLedgerService customerLedgerService, IBOAService boaService, IEmailService emailService, IApplicationLogCodeService applicationLogCodeService, ISecurityService securityService, IOracleHelperService oracleHelperService, IAddressBookService addressBookService, ICustomerActivityLogService customerActivityLogService, IMediaObjectStorageService mediaObjectStorageService)
        {
            _customerLedgerService = customerLedgerService;
            _boaService = boaService;
            _emailService = emailService;
            _applicationLogCodeService = applicationLogCodeService;
            _securityService = securityService;
            _oracleHelperService = oracleHelperService;
            _addressBookService = addressBookService;
            _customerActivityLogService = customerActivityLogService;
            _mediaObjectStorageService = mediaObjectStorageService;
        }

        private static PaymentViewModel _currentPayment;

        private static PaymentViewModel CurrentPayment
        {
            get { return _currentPayment ?? (_currentPayment = GetNewPayment()); }
            set { _currentPayment = value; }
        }

        public ActionResult Index()
        {
            return View(CurrentPayment);
        }



        [HttpPost]
        public ActionResult Index(PaymentViewModel paymentViewModel, string submitButton)
        {
            switch (submitButton)
            {
                case "Next":
                    if (ModelState.IsValid)
                    {
                        TempData["paymentViewModel"] = paymentViewModel;
                        return RedirectToAction("PaymentInfo");
                    }

                    return Index();

                case "Add Another Invoice":

                    TempData["paymentViewModel"] = paymentViewModel;
                    InvoiceViewModel invoiceViewModel = new InvoiceViewModel();
                    paymentViewModel.InvoiceViewModels.Add(invoiceViewModel);

                    _currentPayment = paymentViewModel;

                    return Index();

            }
            return View(paymentViewModel);
        }
        
        [HttpPost]
        public ActionResult FromInvoicePrint(string invoices)
        {
            PaymentViewModel paymentViewModel = new PaymentViewModel {InvoiceViewModels = new List<InvoiceViewModel>()};

            int[] invoiceIDs;
            invoiceIDs = invoices.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToArray();

            foreach (int invoiceId in invoiceIDs)
            {
                InvoiceViewModel invoiceViewModel = new InvoiceViewModel();

                invoiceViewModel.Number = invoiceId;
                invoiceViewModel.Date = _customerLedgerService.GetInvoiceDateByInvoiceNumber(invoiceId);

                paymentViewModel.InvoiceViewModels.Add(invoiceViewModel);
            }

            TempData["paymentViewModel"] = paymentViewModel;

            return RedirectToAction("PaymentInfo", paymentViewModel);

        }

        public ActionResult PaymentInfo()
        {
            PaymentViewModel paymentViewModel = TempData["paymentViewModel"] as PaymentViewModel;
            if (paymentViewModel != null)
            {
                List<InvoiceViewModel> invoiceViewModels = paymentViewModel.InvoiceViewModels;

                int invoiceCount = invoiceViewModels.Count;

                foreach (InvoiceViewModel invoiceViewModel in invoiceViewModels)
                {
                    if (invoiceViewModel.DisplayType != 3)
                    {
                        int invoiceRowNumber = invoiceViewModels.IndexOf(invoiceViewModel);
                        int top = invoiceCount - 1;

                        for (int i = invoiceRowNumber; i < top; i++)
                        {
                            int? thisInvoiceNumber = invoiceViewModels[i + 1].Number;
                            DateTime? thisInvoiceDate = invoiceViewModels[i + 1].Date;

                            int? invoiceToCheck = invoiceViewModels[invoiceRowNumber].Number;
                            DateTime? invoiceDateToCheck = invoiceViewModels[invoiceRowNumber].Date;

                            if (thisInvoiceNumber == invoiceToCheck && thisInvoiceDate == invoiceDateToCheck)
                            {
                                invoiceViewModels[i + 1].DisplayType = 3;
                                invoiceViewModels[i + 1].DisplayMessage = _applicationLogCodeService.GetDisplayMessageAndAddLog(106, LogType.Info, "");
                            }
                        }
                    }
                }
                
                for (int i = invoiceCount - 1; i > -1; i--)
                {
                    int invoiceNumber = Convert.ToInt32(invoiceViewModels[i].Number);

                    if (invoiceNumber != 0)
                    {
                        InvoiceViewModel invoiceViewModel = invoiceViewModels[i];

                        DateTime invoiceDate = Convert.ToDateTime(invoiceViewModel.Date);

                        CustomerLedgerInvoice customerLedgerInvoice =
                            _customerLedgerService.GetCustomerLedgerInvoiceByInvoiceNumberDate(invoiceNumber,
                                                                                               invoiceDate, invoiceViewModel.DisplayType, invoiceViewModel.DisplayMessage);

                        invoiceViewModels[i] = CustomerLedgerInvoiceToInvoiceViewModel(customerLedgerInvoice,
                                                                                       invoiceViewModel);

                    }
                    else
                    {
                        invoiceViewModels.RemoveAt(i);
                    }
                }



            }

            TempData["paymentViewModel"] = paymentViewModel;

            return View(paymentViewModel);
        }

        public ActionResult PaymentInfoTemp(string invoices)
        {
            PaymentViewModel paymentViewModel = new PaymentViewModel { InvoiceViewModels = new List<InvoiceViewModel>() };

            if (!string.IsNullOrEmpty(invoices))
            {
                int[] invoiceIDs;
                invoiceIDs = invoices.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToArray();

                foreach (int invoiceId in invoiceIDs)
                {
                    InvoiceViewModel invoiceViewModel = new InvoiceViewModel();

                    CustomerLedgerInvoice customerLedgerInvoice =
                        _customerLedgerService.GetCustomerLedgerInvoiceByInvoiceNumber(invoiceId);

                    invoiceViewModel = CustomerLedgerInvoiceToInvoiceViewModel(customerLedgerInvoice, invoiceViewModel);

                    paymentViewModel.InvoiceViewModels.Add(invoiceViewModel);
                }

                return View("PaymentInfo", paymentViewModel);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult PaymentInfo(PaymentViewModel paymentViewModel, string submitButton)
        {
            TempData["paymentViewModel"] = paymentViewModel;

            switch (submitButton)
            {
                case "Next":

                    AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                    WindowsPrincipal principal = (WindowsPrincipal) Thread.CurrentPrincipal;
                    String userName = principal.Identity.Name;


                    if (ModelState.IsValid)
                    {
                        string ipAddress = Server.HtmlEncode(Request.UserHostAddress);

                        BOATransaction boaTransaction = new BOATransaction
                            {
                                PaymentAccountType = paymentViewModel.PaymentMethod == 1 ? "CC" : "ACH",
                                FirstInvoiceNumber = paymentViewModel.InvoiceViewModels[0].Number,
                                AmountPaidGross = paymentViewModel.SubTotal*100,
                                AmountPaidTax = paymentViewModel.TaxTotal*100,
                                EmailAddress = paymentViewModel.ConformationEmailAddress,
                                AccountNumber = Convert.ToDecimal(paymentViewModel.InvoiceViewModels[0].AddressNumberParent),
                                AmountPaidTotal = paymentViewModel.PaymentTotal * 100,
                                TypeID = 1, 
                                StatusID = 5,
                                CreateUserID = userName.Split('\\')[1],
                                IPAddress = ipAddress,
                                MaintenanceUserID = userName.Split('\\')[1],
                                MaintenanceDate = DateTime.Now

                            };

                        boaTransaction = _boaService.AddBOATransaction(boaTransaction);

                        List<BOATransactionDetail> boaTransactionDetails = new List<BOATransactionDetail>();

                        foreach (InvoiceViewModel invoiceViewModel in paymentViewModel.InvoiceViewModels)
                        {
                            if (invoiceViewModel.DisplayType != 3)
                            {
                                invoiceViewModel.AmountToPayTotal = invoiceViewModel.AmountToPayGross +
                                                                    invoiceViewModel.AmountToPayTax;

                            invoiceViewModel.AmountTotal = invoiceViewModel.AmountGross + invoiceViewModel.AmountTax;

                            BOATransactionDetail boaTransactionDetail = new BOATransactionDetail
                                {
                                    BOATransactionID = boaTransaction.ID,
                                    InvoiceNumber = invoiceViewModel.Number,
                                    AmountGross = invoiceViewModel.AmountGross*100,
                                    AmountTax = invoiceViewModel.AmountTax*100,
                                    AmountTotal = invoiceViewModel.AmountTotal*100,
                                    AmountToPayGross = invoiceViewModel.AmountToPayGross*100,
                                    AmountToPayTax = invoiceViewModel.AmountToPayTax*100,
                                    AmountToPayTotal = invoiceViewModel.AmountToPayTotal*100,
                                    OpenAmount = invoiceViewModel.AmountTotal*100

                                };

                                boaTransactionDetails.Add(boaTransactionDetail);
                            }
                        }

                        _boaService.AddBOATransactionDetails(boaTransactionDetails);

                        BOATransactionResult boaTransactionResult = _boaService.GetOneTimeSession(boaTransaction);

                        string url = boaTransactionResult.PaymentUrl;

                        return Redirect(url);
                    }
                    return View(paymentViewModel);

                case "Back":

                    int invoiceCount = paymentViewModel.InvoiceViewModels.Count;

                    if (invoiceCount < 7)
                    {
                        for (int i = invoiceCount; i < 7; i++)
                        {
                            InvoiceViewModel invoiceViewModel = new InvoiceViewModel();
                            paymentViewModel.InvoiceViewModels.Add(invoiceViewModel);
                        }
                    }

                    CurrentPayment = paymentViewModel;

                    return RedirectToAction("Index");

                case "Cancel":
                    return RedirectToAction("Index");
                    ;
            }
            return View(paymentViewModel);
        }

        public ActionResult Confirmation(int? id, string ntuserid, string apm, string email, string ret_status = null,
                                         decimal? cnfrm_amount = null, string cnfrm_cc_approval = null,
                                         int? cnfrm_number = null, int pid = 0)
        {
            ReturnResult returnResult = new ReturnResult
                {
                    ID = id,
                    NTUserID = ntuserid,
                    APM = apm,
                    Email = email,
                    ReturnStatus = Convert.ToInt32(ret_status),
                    ConfirmationAmount = cnfrm_amount,
                    ConfirmationCCApproval = cnfrm_cc_approval,
                    ConfirmationNumber = cnfrm_number,
                    PID = pid
                };

            int boaTransactionID = returnResult.PID;

            BOATransaction boaTransaction = new BOATransaction();

            if (boaTransactionID != 0)
            {
                boaTransaction = _boaService.GetBOATransactionByID(boaTransactionID);
            }

            ConfirmationViewModel confirmationViewModel = new ConfirmationViewModel();


            switch (returnResult.ReturnStatus)
            {
                case 0:

                    if (boaTransaction.ReceivedConfirmation == 0)
                    {
                        boaTransaction.ReceivedConfirmation = 1;
                        confirmationViewModel = ProcessReturnStatus0(returnResult, boaTransaction);

                        return View(confirmationViewModel);
                    }
                    confirmationViewModel.Message = _applicationLogCodeService.GetDisplayMessageAndAddLog(118,
                                                                                                          LogType
                                                                                                              .Error,
                                                                                                          "");
                    return View(confirmationViewModel);

                case 5:

                    confirmationViewModel.Message = _applicationLogCodeService.GetDisplayMessageAndAddLog(110,
                                                                                                          LogType
                                                                                                              .Error,
                                                                                                          "");
                    return View(confirmationViewModel);

                case 8:

                    confirmationViewModel.Message = _applicationLogCodeService.GetDisplayMessageAndAddLog(111,
                                                                                                          LogType
                                                                                                              .Warn,
                                                                                                          "");
                    return View(confirmationViewModel);

                case 11:

                    confirmationViewModel.Message = _applicationLogCodeService.GetDisplayMessageAndAddLog(112,
                                                                                                          LogType
                                                                                                              .Error,
                                                                                                          "");
                    return View(confirmationViewModel);

                case 41:

                    confirmationViewModel.Message = _applicationLogCodeService.GetDisplayMessageAndAddLog(113,
                                                                                                          LogType
                                                                                                              .Error,
                                                                                                          "");
                    return View(confirmationViewModel);

                case 55:

                    confirmationViewModel.Message = _applicationLogCodeService.GetDisplayMessageAndAddLog(114,
                                                                                                          LogType
                                                                                                              .Error,
                                                                                                          "");
                    return View(confirmationViewModel);

                case 66:

                    confirmationViewModel.Message = _applicationLogCodeService.GetDisplayMessageAndAddLog(115,
                                                                                                          LogType
                                                                                                              .Error,
                                                                                                          "");
                    return View(confirmationViewModel);

                case 99:

                    if (boaTransaction.ReceivedConfirmation == 0)
                    {
                        boaTransaction.ReceivedConfirmation = 1;

                        ProcessReturnStatus99(returnResult, boaTransaction);

                        _applicationLogCodeService.GetDisplayMessageAndAddLog(116, LogType.Info, "");

                        return RedirectToAction("Index");
                    }

                    confirmationViewModel.Message = _applicationLogCodeService.GetDisplayMessageAndAddLog(118,
                                                                                                          LogType
                                                                                                              .Error,
                                                                                                          "");
                    return View(confirmationViewModel);

                default:

                    confirmationViewModel.Message =
                        "There was an error processing the payment.  Please try again or contact the Williams Scotsman Customer Assistance hotline at 1-888-378-9084. ";

                    _applicationLogCodeService.GetDisplayMessageAndAddLog(117, LogType.Error, "");

                    return View(confirmationViewModel);

            }
        }

        private ConfirmationViewModel ProcessReturnStatus0(ReturnResult returnResult, BOATransaction boaTransaction)
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            String userName = principal.Identity.Name;

            boaTransaction.ReturnStatus = returnResult.ReturnStatus;
            boaTransaction.Confirmation = returnResult.ConfirmationNumber;
            boaTransaction.ConfirmationCCApproval = returnResult.ConfirmationCCApproval;
            boaTransaction.ChargeUserID = userName.Split('\\')[1];
            boaTransaction.ChargeDate = DateTime.Now;
            boaTransaction.StatusID = 6;

            _boaService.UpdateBOATransactionFromReturnURL(boaTransaction);
            
            _customerLedgerService.SetInvoiceStatusToJ(returnResult.PID);

            UpdateCustomerActivityLogAndMediaObjectStorage(returnResult, boaTransaction);

            BOATransactionEmail boaTransactionEmail = new BOATransactionEmail
                {
                    To = boaTransaction.EmailAddress,
                    ConfirmationNumber = returnResult.ConfirmationNumber.ToString(),
                    AmountCharged = String.Format("{0:N}", returnResult.ConfirmationAmount)
                };

            bool success = _emailService.BOATransaction(boaTransactionEmail);
            ConfirmationViewModel confirmationViewModel = new ConfirmationViewModel();
            
            if (success)
            {
                confirmationViewModel.Message =
                    "Thank you for submitting your payment to Williams Scotsman in the amount of $" +
                    String.Format("{0:N}", returnResult.ConfirmationAmount) +
                    ". Your confirmation number for this transaction is " +
                    returnResult.ConfirmationNumber.ToString() + ".  " +
                    "An email confirmation will be sent to " + boaTransaction.EmailAddress + ".  " +
                    "If you have any questions, concerns or require clarification regarding this payment, please contact the Williams Scotsman Customer Assistance hotline at 1-888-378-9084.";
            }
            else
            {
                confirmationViewModel.Message =
                    "Thank you for submitting your payment to Williams Scotsman in the amount of $" +
                    String.Format("{0:N}", returnResult.ConfirmationAmount) +
                    ". Your confirmation number for this transaction is " +
                    returnResult.ConfirmationNumber.ToString() + ".  " +
                    "It appears we had trouble sending the email confirmation to the address provided.  Please contact the Williams Scotsman Customer Assistance hotline at 1-888-378-9084 if you need additional documentation";
            }

            return confirmationViewModel;
        }

        private void UpdateCustomerActivityLogAndMediaObjectStorage(ReturnResult returnResult, BOATransaction boaTransaction)
        {
            List<int> addressNumberParents =
                _customerLedgerService.GetAddressNumberParentsByBOATransactionID(Convert.ToInt32(boaTransaction.ID));

            foreach (var addressNumberParent in addressNumberParents)
            {
                int activityID = _oracleHelperService.GetNextSequenceValue(SequenceNumber.Activity);

            

            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            String userName = principal.Identity.Name;

            CustomerActivityLog customerActivityLog = new CustomerActivityLog
                {
                    ActivityID = activityID,
                    ActivityType = "OP",
                    AddressNumber = addressNumberParent,
                    Amount1 = boaTransaction.AmountPaidTotal,
                    Amount2 = 0,
                    Company = _addressBookService.GetBusinessUnitByID(Convert.ToInt32(boaTransaction.AccountNumber)),
                    DateTickler = DateTime.Now.DateTimeToJdeDate(),
                    DateUpdated = DateTime.Now.DateTimeToJdeDate(),
                    NameRemark = boaTransaction.ID.ToString(),
                    PriorityActivity = "5",
                    ProgramID = "P03B31",
                    TimeLastUpdated = DateTime.Now.DateTimeToJDETime(),
                    UniqueKeyIDInternal = 0,
                    UserID = userName.Split('\\')[1],
                    WorkStationID = "INV_BOA"
                };

            _customerActivityLogService.AddCustomerActivityLog(customerActivityLog);

                List<BOATransactionDetail> boaTransactionDetails =
                    _boaService.GetBOATransactionDetailsByBOATransactionIDAndAddressNumberParent(Convert.ToInt32(boaTransaction.ID), addressNumberParent);

                StringBuilder sb = new StringBuilder();

                sb.Append(
                    @"{\rtf1\ansi\deff0\deftab720{\fonttbl{\f0\fswiss MS Sans Serif;}{\f1\froman\fcharset2 Symbol;}{\f2\fswiss\fprq2 Arial;}}");
                sb.Append("\r\n");
                sb.Append(@"{\colortbl\red0\green0\blue0;}");
                sb.Append("\r\n");
                sb.Append(@"\deflang1033\pard\plain\f2\fs24\cf0 ");
                sb.Append("BOA Transaction\\par\n");
                sb.Append("Type of Transaction: Onetime Payment \\par\n");
                sb.Append("Transaction ID: " + boaTransaction.ID + "\\par\n");
                sb.Append("Total Charge Amt: $" + boaTransaction.AmountPaidTotal/100 + "\\par\n");
                foreach (BOATransactionDetail boaTransactionDetail in boaTransactionDetails)
                {
                    sb.Append("\tInvoice: " + boaTransactionDetail.InvoiceNumber + " \tCharge: $" +
                              boaTransactionDetail.AmountToPayTotal/100 + "\\par\n");
                }
                sb.Append("Portal ID: \\par\n");
                sb.Append("ACCTID: \\par\n");
                sb.Append("Enrollment ID:\\par\n");
                sb.Append("Return Status: Success \\par\n");
                sb.Append("Return Message: \\par\n");
                sb.Append("Cnfrm Number: " + returnResult.ConfirmationNumber + "\\par\n");
                sb.Append("Cnfrm Amount: $" + returnResult.ConfirmationAmount + "\\par\n");
                sb.Append("Cnfrm CC Aproval: " + returnResult.ConfirmationCCApproval + "\\par\n");
                sb.Append("APM: " + boaTransaction.PaymentAccountType + "\\par\n");
                sb.Append(@"\par }");
                sb.Append("\r\n");
                sb.Append("^");

            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());

            MediaObjectStorage mediaObjectStorage = new MediaObjectStorage
                {
                    DateUpdated = DateTime.Now.DateTimeToJdeDate(),
                    GenericTextBLOBBuffer = bytes,
                    GenericTextFileName = " ",
                    GenericTextFutureUseMath1 = 0,
                    GenericTextFutureUseMath2 = 0,
                    GenericTextFutureUserString3 = " ",
                    GenericTextFutureUseString1 = " ",
                    GenericTextFutureUseString2 = " ",
                    GenericTextFutureUseString4 = " ",
                    GenericTextItemName = " ",
                    GenericTextKey = activityID.ToString(),
                    GenericTextMediaObjectType = 0,
                    Language = " ",
                    MediaObjectSequenceNumber = 0,
                    ObjectName = "GT03B31",
                    QueueName = " ",
                    TimeofDay = DateTime.Now.DateTimeToJDETime(),
                    UserID = " ",

                };

                _mediaObjectStorageService.AddMediaObjectStorage(mediaObjectStorage);
            }
        }

        private void ProcessReturnStatus99(ReturnResult returnResult, BOATransaction boaTransaction)
        {

            boaTransaction.ReturnStatus = returnResult.ReturnStatus;
            boaTransaction.StatusID = 6;
            boaTransaction.ReturnMessage = "user canceled out.";

            _boaService.UpdateBOATransactionFromReturnURL(boaTransaction);

        }
        
        public static PaymentViewModel GetNewPayment()
        {
            PaymentViewModel paymentViewModel = new PaymentViewModel();

            List<InvoiceViewModel> invoiceViewModels = new List<InvoiceViewModel>
                {
                    new InvoiceViewModel{},
                    new InvoiceViewModel{},
                    new InvoiceViewModel{},
                    new InvoiceViewModel{},
                    new InvoiceViewModel{},
                    new InvoiceViewModel{},
                    new InvoiceViewModel{}
                };
            paymentViewModel.InvoiceViewModels = invoiceViewModels;

            return paymentViewModel;

        }

        private InvoiceViewModel CustomerLedgerInvoiceToInvoiceViewModel(CustomerLedgerInvoice customerLedgerInvoice,
                                                                         InvoiceViewModel invoiceViewModel)
        {
            invoiceViewModel.Number = customerLedgerInvoice.Number;
            invoiceViewModel.AddressNumberParent = customerLedgerInvoice.AddressNumberParent;
            invoiceViewModel.SupplierInvoiceNumber = customerLedgerInvoice.SupplierInvoiceNumber;
            invoiceViewModel.DocumentType = customerLedgerInvoice.DocumentType;
            invoiceViewModel.PayStatusCode = customerLedgerInvoice.PayStatusCode;
            invoiceViewModel.OutstandingBalance = customerLedgerInvoice.OutstandingBalance;
            invoiceViewModel.Payable = customerLedgerInvoice.Payable;
            invoiceViewModel.DocumentCompany = customerLedgerInvoice.DocumentCompany;
            invoiceViewModel.AmountGross = customerLedgerInvoice.AmountGross;
            invoiceViewModel.AmountTax = customerLedgerInvoice.AmountTax;
            invoiceViewModel.AmountTotal = customerLedgerInvoice.AmountGross + customerLedgerInvoice.AmountTax;
            invoiceViewModel.AmountToPayGross = customerLedgerInvoice.AmountGross;
            invoiceViewModel.AmountToPayTax = customerLedgerInvoice.AmountTax;
            invoiceViewModel.AmountToPayTotal = customerLedgerInvoice.AmountGross + customerLedgerInvoice.AmountTax;
            invoiceViewModel.DisplayType = customerLedgerInvoice.DisplayType;
            invoiceViewModel.DisplayMessage = customerLedgerInvoice.DisplayMessage;

            return invoiceViewModel;
        }
    }

    internal class ReturnResult
    {
        public int? ID { get; set; }
        public string NTUserID { get; set; }
        public string APM { get; set; }
        public string Email { get; set; }
        public int ReturnStatus { get; set; }
        public decimal? ConfirmationAmount { get; set; }
        public string ConfirmationCCApproval { get; set; }
        public int? ConfirmationNumber { get; set; }
        public int PID { get; set; }

    }
}
