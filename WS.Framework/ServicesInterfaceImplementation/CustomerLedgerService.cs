using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Objects;
using System.Linq;
using NLog;
using WS.Framework.Objects;
using WS.Framework.Objects.Enums;
using WS.Framework.ServicesInterface;
using WS.Framework.Extensions;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class CustomerLedgerService : ICustomerLedgerService
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        //private readonly IHelperService _helperService;
        //private readonly IApplicationLogCodeService _applicationLogCodeService;
        //private readonly IAddressBookService _addressBookService;

        //public CustomerLedgerService(IHelperService helperService, IApplicationLogCodeService applicationLogCodeService,
        //                             IAddressBookService addressBookService)
        //{
        //    _helperService = helperService;
        //    _applicationLogCodeService = applicationLogCodeService;
        //    _addressBookService = addressBookService;
        //}

        //public DateTime GetInvoiceDateByInvoiceNumber(int invoiceNumber)
        //{
        //    DateTime invoiceDate = DateTime.Now;

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            int? query = (from cl in context.CustomerLedgers
        //                          where cl.DocumentNumber == invoiceNumber
        //                          select cl.DateInvoiceJulian
        //                         ).FirstOrDefault();

        //            invoiceDate = query.JdeDateToDateTime();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return invoiceDate;
        //}

        //public CustomerLedgerInvoice GetCustomerLedgerInvoiceByInvoiceNumber(int invoiceNumber)
        //{
        //    CustomerLedgerInvoice customerLedgerInvoice = new CustomerLedgerInvoice {OutstandingBalance = false};

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            var query = (from cl in context.CustomerLedgers
        //                         where cl.DocumentNumber == invoiceNumber
        //                         group cl by new
        //                             {
        //                                 cl.AddressNumberParent,
        //                                 cl.SupplierInvoiceNumber,
        //                                 cl.DocumentType,
        //                                 cl.PayStatusCode,
        //                                 cl.DocumentCompany,
        //                             }
        //                         into g
        //                         select new
        //                             {
        //                                 g.Key.AddressNumberParent,
        //                                 g.Key.SupplierInvoiceNumber,
        //                                 g.Key.DocumentType,
        //                                 g.Key.PayStatusCode,
        //                                 g.Key.DocumentCompany,
        //                                 AmountGross = g.Sum(cl => cl.AmountGross),
        //                                 AmountTax = g.Sum(cl => cl.AmountTax),
        //                                 AmountOpen = g.Sum(cl => cl.AmountOpen)
        //                             }).FirstOrDefault();

        //            if (query != null)
        //            {
        //                customerLedgerInvoice.Number = invoiceNumber;
        //                if (query.AddressNumberParent != null)
        //                {
        //                    customerLedgerInvoice.AddressNumberParent = query.AddressNumberParent.ToString();
        //                }
        //                customerLedgerInvoice.SupplierInvoiceNumber = query.SupplierInvoiceNumber;
        //                customerLedgerInvoice.DocumentType = query.DocumentType;
        //                customerLedgerInvoice.PayStatusCode = query.PayStatusCode;
        //                customerLedgerInvoice.DocumentCompany = query.DocumentCompany;
        //                customerLedgerInvoice.AmountGross = Convert.ToInt32(query.AmountGross)/100;
        //                customerLedgerInvoice.AmountTax = Convert.ToInt32(query.AmountTax)/100;
        //                customerLedgerInvoice.AmountOpen = Convert.ToInt32(query.AmountOpen)/100;
        //                if (customerLedgerInvoice.AmountOpen != customerLedgerInvoice.AmountGross)
        //                {
        //                    customerLedgerInvoice.OutstandingBalance = true;
        //                }

        //                customerLedgerInvoice = SetDisplayTypeMessage(customerLedgerInvoice);
        //            }
        //            else
        //            {
        //                customerLedgerInvoice.DisplayType = 3;
        //                customerLedgerInvoice.DisplayMessage = _applicationLogCodeService.GetDisplayMessageAndAddLog(
        //                    100, LogType.Info, "");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }
        //    return customerLedgerInvoice;
        //}

        //public CustomerLedgerInvoice GetCustomerLedgerInvoiceByInvoiceNumberDate(int invoiceNumber, DateTime invoiceDate,
        //                                                                         int displayType, string displayMessage)
        //{
        //    CustomerLedgerInvoice customerLedgerInvoice = new CustomerLedgerInvoice
        //        {
        //            OutstandingBalance = false,
        //            DisplayType = displayType,
        //            DisplayMessage = displayMessage
        //        };

        //    string invoice = invoiceNumber.ToString().PadRight(25);
        //    int jdeInvoiceDate = invoiceDate.DateTimeToJdeDate();

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            List<CustomerLedger> customerLedgers = (from cl in context.CustomerLedgers
        //                                                    where cl.DocumentNumber == invoiceNumber
        //                                                          &&
        //                                                          (cl.DateInvoiceJulian == jdeInvoiceDate ||
        //                                                           cl.DateNetDue == jdeInvoiceDate)
        //                                                    select cl).ToList();

        //            if (!customerLedgers.Any())
        //            {
        //                //No invoices found

        //                customerLedgerInvoice.DisplayType = 3;
        //                customerLedgerInvoice.Number = invoiceNumber;
        //                customerLedgerInvoice.Date = invoiceDate;
        //                customerLedgerInvoice.DisplayMessage = _applicationLogCodeService.GetDisplayMessageAndAddLog(
        //                    100, LogType.Info, "");
        //                return customerLedgerInvoice;
        //            }

        //            bool groupBill = true;
        //            string supplierInvoiceNumber = null;

        //            foreach (CustomerLedger customerLedger in customerLedgers)
        //            {
        //                if (customerLedger.ARReportingCode8 != "GRP")
        //                {
        //                    groupBill = false;
        //                }
        //                else
        //                {
        //                    supplierInvoiceNumber = customerLedger.SupplierInvoiceNumber;
        //                }
        //            }

        //            if (groupBill)
        //            {
        //                //Is Group Bill

        //                var list1 = (from cl in context.CustomerLedgers
        //                             where
        //                                 cl.SupplierInvoiceNumber == DbFunctions.AsNonUnicode(supplierInvoiceNumber)
        //                                 && (cl.DateInvoiceJulian == jdeInvoiceDate || cl.DateNetDue == jdeInvoiceDate)
        //                             group cl by new
        //                                 {
        //                                     cl.AddressNumberParent,
        //                                     cl.SupplierInvoiceNumber,
        //                                     cl.DocumentType,
        //                                     cl.PayStatusCode,
        //                                     cl.DocumentCompany,
        //                                     cl.DateInvoiceJulian,
        //                                     cl.ARReportingCode8
        //                                 }
        //                             into g
        //                             select new
        //                                 {
        //                                     g.Key.AddressNumberParent,
        //                                     g.Key.SupplierInvoiceNumber,
        //                                     g.Key.DocumentType,
        //                                     g.Key.PayStatusCode,
        //                                     g.Key.DocumentCompany,
        //                                     g.Key.DateInvoiceJulian,
        //                                     AmountGross = g.Sum(cl => cl.AmountGross - cl.AmountTax),
        //                                     AmountTax = g.Sum(cl => cl.AmountTax),
        //                                     AmountOpen = g.Sum(cl => cl.AmountOpen),
        //                                     g.Key.ARReportingCode8
        //                                 }).ToList();

        //                //If there is only one
        //                if (list1.Count() == 1)
        //                {
        //                    customerLedgerInvoice.Number = Convert.ToInt32(list1[0].SupplierInvoiceNumber);
        //                    customerLedgerInvoice.Date = list1[0].DateInvoiceJulian.JdeDateToDateTime();
        //                    if (list1[0].AddressNumberParent != null)
        //                    {
        //                        customerLedgerInvoice.AddressNumberParent = list1[0].AddressNumberParent.ToString();
        //                    }
        //                    customerLedgerInvoice.SupplierInvoiceNumber = list1[0].SupplierInvoiceNumber;
        //                    customerLedgerInvoice.DocumentType = list1[0].DocumentType;
        //                    customerLedgerInvoice.DocumentCompany = list1[0].DocumentCompany;
        //                    customerLedgerInvoice.AmountGross =
        //                        Convert.ToDecimal(Convert.ToInt32(list1[0].AmountGross))/100;
        //                    customerLedgerInvoice.AmountTax = Convert.ToDecimal(Convert.ToInt32(list1[0].AmountTax))/100;
        //                    customerLedgerInvoice.AmountOpen = Convert.ToDecimal(Convert.ToInt32(list1[0].AmountOpen))/
        //                                                       100;

        //                    if (customerLedgerInvoice.AmountOpen !=
        //                        (customerLedgerInvoice.AmountGross + customerLedgerInvoice.AmountTax))
        //                    {
        //                        customerLedgerInvoice.OutstandingBalance = true;
        //                        customerLedgerInvoice.AmountGross = customerLedgerInvoice.AmountOpen;
        //                    }

        //                    customerLedgerInvoice = SetDisplayTypeMessage(customerLedgerInvoice);
        //                }
        //                else
        //                {
        //                    IEnumerable<string> payStatusCodes = list1.GroupBy(l => l.PayStatusCode).Select(g => g.Key);

        //                    foreach (string payStatusCode in payStatusCodes)
        //                    {
        //                        if (payStatusCode != "J" && payStatusCode != "P")
        //                        {
        //                            customerLedgerInvoice.PayStatusCode = payStatusCode;
        //                            break;
        //                        }
        //                        if (payStatusCode == "J")
        //                        {
        //                            customerLedgerInvoice.PayStatusCode = "J";
        //                        }
        //                        if (payStatusCode == "P")
        //                        {
        //                            customerLedgerInvoice.PayStatusCode = "P";
        //                        }
        //                    }

        //                    customerLedgerInvoice.Number = Convert.ToInt32(list1[0].SupplierInvoiceNumber);
        //                    customerLedgerInvoice.Date = list1[0].DateInvoiceJulian.JdeDateToDateTime();
        //                    if (list1[0].AddressNumberParent != null)
        //                    {
        //                        customerLedgerInvoice.AddressNumberParent = list1[0].AddressNumberParent.ToString();
        //                    }
        //                    customerLedgerInvoice.SupplierInvoiceNumber = list1[0].SupplierInvoiceNumber;
        //                    customerLedgerInvoice.DocumentType = list1[0].DocumentType;
        //                    customerLedgerInvoice.DocumentCompany = list1[0].DocumentCompany;

        //                    decimal amountGross = 0;
        //                    decimal amountTax = 0;
        //                    decimal amountOpen = 0;

        //                    foreach (var y in list1)
        //                    {
        //                        if (y.PayStatusCode != "J" && y.PayStatusCode != "P")
        //                        {
        //                            amountGross = amountGross + Convert.ToInt32(y.AmountGross);
        //                            amountTax = amountTax + Convert.ToInt32(y.AmountTax);
        //                            amountOpen = amountOpen + Convert.ToInt32(y.AmountOpen);
        //                        }
        //                    }

        //                    customerLedgerInvoice.AmountGross = amountGross/100;
        //                    customerLedgerInvoice.AmountTax = amountTax/100;
        //                    customerLedgerInvoice.AmountOpen = amountOpen/100;

        //                    if (customerLedgerInvoice.AmountOpen != customerLedgerInvoice.AmountGross)
        //                    {
        //                        customerLedgerInvoice.OutstandingBalance = true;
        //                        customerLedgerInvoice.AmountGross = customerLedgerInvoice.AmountOpen;
        //                    }

        //                    customerLedgerInvoice = SetDisplayTypeMessage(customerLedgerInvoice);
        //                }
        //            }
        //            else
        //            {
        //                var list2 = (from cl in context.CustomerLedgers
        //                             where cl.DocumentNumber == invoiceNumber
        //                                   &&
        //                                   (cl.DateInvoiceJulian == jdeInvoiceDate || cl.DateNetDue == jdeInvoiceDate)
        //                             group cl by new
        //                                 {
        //                                     cl.AddressNumberParent,
        //                                     cl.SupplierInvoiceNumber,
        //                                     cl.DocumentType,
        //                                     cl.PayStatusCode,
        //                                     cl.DocumentCompany,
        //                                     cl.DateInvoiceJulian,
        //                                     cl.ARReportingCode8
        //                                 }
        //                             into g
        //                             select new
        //                                 {
        //                                     g.Key.AddressNumberParent,
        //                                     g.Key.SupplierInvoiceNumber,
        //                                     g.Key.DocumentType,
        //                                     g.Key.PayStatusCode,
        //                                     g.Key.DocumentCompany,
        //                                     g.Key.DateInvoiceJulian,
        //                                     AmountGross = g.Sum(cl => cl.AmountGross - cl.AmountTax),
        //                                     AmountTax = g.Sum(cl => cl.AmountTax),
        //                                     AmountOpen = g.Sum(cl => cl.AmountOpen),
        //                                     g.Key.ARReportingCode8
        //                                 }).ToList();

        //                if (list2.Count() == 1)
        //                {
        //                    customerLedgerInvoice.Number = invoiceNumber;
        //                    customerLedgerInvoice.Date = list2[0].DateInvoiceJulian.JdeDateToDateTime();
        //                    if (list2[0].AddressNumberParent != null)
        //                    {
        //                        customerLedgerInvoice.AddressNumberParent = list2[0].AddressNumberParent.ToString();
        //                    }
        //                    customerLedgerInvoice.SupplierInvoiceNumber = list2[0].SupplierInvoiceNumber;
        //                    customerLedgerInvoice.DocumentType = list2[0].DocumentType;
        //                    customerLedgerInvoice.PayStatusCode = list2[0].PayStatusCode;
        //                    customerLedgerInvoice.DocumentCompany = list2[0].DocumentCompany;
        //                    customerLedgerInvoice.AmountGross =
        //                        Convert.ToDecimal(Convert.ToInt32(list2[0].AmountGross))/100;
        //                    customerLedgerInvoice.AmountTax = Convert.ToDecimal(Convert.ToInt32(list2[0].AmountTax))/100;
        //                    customerLedgerInvoice.AmountOpen = Convert.ToDecimal(Convert.ToInt32(list2[0].AmountOpen))/
        //                                                       100;
        //                    if (customerLedgerInvoice.AmountOpen !=
        //                        (customerLedgerInvoice.AmountGross + customerLedgerInvoice.AmountTax))
        //                    {
        //                        customerLedgerInvoice.OutstandingBalance = true;
        //                        customerLedgerInvoice.AmountGross = customerLedgerInvoice.AmountOpen;
        //                    }

        //                    customerLedgerInvoice = SetDisplayTypeMessage(customerLedgerInvoice);
        //                }
        //                else if (list2.Count() == 2)
        //                {
        //                    if (list2[0].PayStatusCode == "A" || list2[1].PayStatusCode == "A")
        //                    {
        //                        customerLedgerInvoice.PayStatusCode = "A";
        //                    }
        //                    else if (list2[0].PayStatusCode == "P" && list2[1].PayStatusCode == "P")
        //                    {
        //                        customerLedgerInvoice.PayStatusCode = "P";
        //                    }
        //                    else if (list2[0].PayStatusCode == "J" && list2[1].PayStatusCode == "J")
        //                    {
        //                        customerLedgerInvoice.PayStatusCode = "J";
        //                    }
        //                    else if (list2[0].PayStatusCode == "J" || list2[0].PayStatusCode == "P")
        //                    {
        //                        customerLedgerInvoice.PayStatusCode = list2[1].PayStatusCode;
        //                    }
        //                    else if (list2[1].PayStatusCode == "J" || list2[1].PayStatusCode == "P")
        //                    {
        //                        customerLedgerInvoice.PayStatusCode = list2[0].PayStatusCode;
        //                    }

        //                    customerLedgerInvoice.Number = invoiceNumber;
        //                    customerLedgerInvoice.Date = list2[0].DateInvoiceJulian.JdeDateToDateTime();
        //                    if (list2[0].AddressNumberParent != null)
        //                    {
        //                        customerLedgerInvoice.AddressNumberParent = list2[0].AddressNumberParent.ToString();
        //                    }
        //                    customerLedgerInvoice.SupplierInvoiceNumber = list2[0].SupplierInvoiceNumber;
        //                    customerLedgerInvoice.DocumentType = list2[0].DocumentType;
        //                    customerLedgerInvoice.DocumentCompany = list2[0].DocumentCompany;
        //                    customerLedgerInvoice.AmountGross =
        //                        Convert.ToDecimal(Convert.ToInt32(list2[0].AmountGross) +
        //                                          Convert.ToInt32(list2[1].AmountGross))/100;
        //                    customerLedgerInvoice.AmountTax =
        //                        Convert.ToDecimal(Convert.ToInt32(list2[0].AmountTax) +
        //                                          Convert.ToInt32(list2[1].AmountTax))/100;
        //                    customerLedgerInvoice.AmountOpen =
        //                        Convert.ToDecimal(Convert.ToInt32(list2[0].AmountOpen) +
        //                                          Convert.ToInt32(list2[1].AmountOpen))/100;

        //                    if (customerLedgerInvoice.AmountOpen != customerLedgerInvoice.AmountGross)
        //                    {
        //                        customerLedgerInvoice.OutstandingBalance = true;
        //                        customerLedgerInvoice.AmountGross = customerLedgerInvoice.AmountOpen;
        //                    }

        //                    customerLedgerInvoice = SetDisplayTypeMessage(customerLedgerInvoice);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }
        //    return customerLedgerInvoice;
        //}

        //private CustomerLedgerInvoice SetDisplayTypeMessage(CustomerLedgerInvoice customerLedgerInvoice)
        //{
        //    if (customerLedgerInvoice.DisplayType != 3)
        //    {

        //        int displayType = 0;
        //        string displayMessage = "";
        //        bool outstandingBalance = customerLedgerInvoice.OutstandingBalance;
        //        string payStatusCode = customerLedgerInvoice.PayStatusCode;
        //        decimal amountOpen = customerLedgerInvoice.AmountOpen;
        //        string documentCompany = customerLedgerInvoice.DocumentCompany;

        //        if (payStatusCode != "J" && payStatusCode != "P")
        //        {
        //            if (customerLedgerInvoice.AmountGross < 0 || customerLedgerInvoice.AmountTax < 0)
        //            {
        //                displayType = 3;
        //                displayMessage = _applicationLogCodeService.GetDisplayMessageAndAddLog(105, LogType.Info, "");
        //            }
        //            else
        //            {
        //                if (outstandingBalance == false && documentCompany == "00010" && amountOpen > 0)
        //                {
        //                    displayType = 1;
        //                }
        //                else if (outstandingBalance && documentCompany == "00010" && amountOpen > 0)
        //                {
        //                    displayType = 2;
        //                    customerLedgerInvoice.AmountTax = 0;
        //                }
        //                else if (documentCompany != "00010")
        //                {
        //                    displayType = 3;
        //                    displayMessage = _applicationLogCodeService.GetDisplayMessageAndAddLog(101, LogType.Info, "");
        //                }
        //            }
        //        }
        //        else if (payStatusCode == "J")
        //        {
        //            displayType = 3;
        //            displayMessage = _applicationLogCodeService.GetDisplayMessageAndAddLog(102, LogType.Info, "");
        //        }
        //        else if (payStatusCode == "P")
        //        {
        //            displayType = 3;
        //            displayMessage = _applicationLogCodeService.GetDisplayMessageAndAddLog(103, LogType.Info, "");
        //        }
        //        else
        //        {
        //            displayType = 3;
        //            displayMessage = _applicationLogCodeService.GetDisplayMessageAndAddLog(100, LogType.Info, "");
        //        }

        //        customerLedgerInvoice.DisplayType = displayType;
        //        customerLedgerInvoice.DisplayMessage = displayMessage;
        //        return customerLedgerInvoice;
        //    }
        //    return customerLedgerInvoice;
        //}

        //public CustomerLedgerInvoicePDF GetCustomerLedgerInvoicePDFByInvoiceNumber(int invoiceNumber)
        //{
        //    CustomerLedgerInvoicePDF customerLedgerInvoicePDF = new CustomerLedgerInvoicePDF();

        //    DateTime invoiceDate = Convert.ToDateTime("12/12/2011");

        //    int jdeInvoiceDate = _helperService.DateTimeToJDEDate(invoiceDate);

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            var query = (from cl in context.CustomerLedgers
        //                         join ab in context.AddressBooks on cl.AddressNumber equals ab.ID
        //                         where cl.DocumentNumber == invoiceNumber
        //                               && (cl.DateInvoiceJulian == jdeInvoiceDate || cl.DateNetDue == jdeInvoiceDate)
        //                         group cl by new
        //                             {
        //                                 cl.DocumentNumber,
        //                                 cl.AddressNumberParent,
        //                                 cl.SupplierInvoiceNumber,
        //                                 cl.DocumentType,
        //                                 cl.PayStatusCode,
        //                                 cl.DocumentCompany,
        //                                 ab.FactorSpecialPayee,
        //                             }
        //                         into g
        //                         select new
        //                             {
        //                                 g.Key.DocumentNumber,
        //                                 g.Key.AddressNumberParent,
        //                                 g.Key.SupplierInvoiceNumber,
        //                                 g.Key.DocumentType,
        //                                 g.Key.PayStatusCode,
        //                                 g.Key.DocumentCompany,
        //                                 g.Key.FactorSpecialPayee,
        //                                 AmountGross = g.Sum(cl => cl.AmountGross),
        //                                 AmountTax = g.Sum(cl => cl.AmountTax),
        //                                 AmountOpen = g.Sum(cl => cl.AmountOpen)
        //                             }).FirstOrDefault();

        //            if (query != null)
        //            {
        //                customerLedgerInvoicePDF.Number = invoiceNumber;
        //                customerLedgerInvoicePDF.Date = invoiceDate;
        //                if (query.AddressNumberParent != null)
        //                {
        //                    customerLedgerInvoicePDF.AddressNumberParent = query.AddressNumberParent.ToString();
        //                }
        //                customerLedgerInvoicePDF.SupplierInvoiceNumber = query.SupplierInvoiceNumber;
        //                customerLedgerInvoicePDF.DocumentType = query.DocumentType;
        //                customerLedgerInvoicePDF.PayStatusCode = query.PayStatusCode;
        //                customerLedgerInvoicePDF.DocumentCompany = query.DocumentCompany;
        //                customerLedgerInvoicePDF.FactorSpecialPayee = Convert.ToInt32(query.FactorSpecialPayee);
        //                customerLedgerInvoicePDF.AmountGross = Convert.ToDecimal(Convert.ToInt32(query.AmountGross))/100;
        //                customerLedgerInvoicePDF.AmountTax = Convert.ToDecimal(Convert.ToInt32(query.AmountTax))/100;
        //                customerLedgerInvoicePDF.AmountOpen = Convert.ToDecimal(Convert.ToInt32(query.AmountOpen))/100;

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }
        //    return customerLedgerInvoicePDF;
        //}

        //public AccountNumberLookUpResult IsUSorCA(AccountNumberLookUpResult accountNumberLookUpResult,
        //                                          List<CustomerLedger> customerLedgers)
        //{
        //    bool USorCA = true;
        //    foreach (var x in customerLedgers)
        //    {
        //        if (x.DocumentCompany != "00010")
        //        {
        //            if (x.DocumentCompany != "00020")
        //            {
        //                USorCA = false;
        //            }
        //        }
        //    }

        //    if (!USorCA)
        //    {
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.NonUSorCA;
        //    }

        //    return accountNumberLookUpResult;
        //}

        //private readonly List<string> documentTypes = new List<string> {"RI", "RM", "RJ", "RN"};

        //public AccountNumberLookUpResult GetAccountNumberLookUpResult(
        //    AccountNumberLookUpResult accountNumberLookUpResult, List<CustomerLedger> customerLedgers)
        //{
        //    int daysToGoBack = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DaysToGoBack"));
        //    DateTime today = DateTime.Today;
        //    int dateToCompare = today.AddDays(-daysToGoBack).DateTimeToJdeDate();

        //    //Check to see if there are any
        //    if (!customerLedgers.Any())
        //    {
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.ChildOfGroupBill;
        //        return accountNumberLookUpResult;
        //    }

        //    // Check for US or CA
        //    accountNumberLookUpResult = IsUSorCA(accountNumberLookUpResult, customerLedgers);

        //    if (accountNumberLookUpResult.ResultCode == AccountNumberLookUpResultCode.NonUSorCA)
        //    {
        //        return accountNumberLookUpResult;
        //    }

        //    //Check Dcoument Type
        //    IEnumerable<CustomerLedger> queryToCheckInvoiceType = (from cl in customerLedgers
        //                                                           where
        //                                                               documentTypes.Contains(
        //                                                                   cl.DocumentType)
        //                                                           select cl);

        //    if (!queryToCheckInvoiceType.Any())
        //    {
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.NotValidDocumentType;
        //        return accountNumberLookUpResult;
        //    }

        //    //Check For Write Off
        //    if (customerLedgers.Any(customerLedger => customerLedger.PayStatusCode == "4"))
        //    {
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.WriteOff;
        //        return accountNumberLookUpResult;
        //    }

        //    //Check for number of days
        //    var queryToCheckIfInvocesWithSetNumberOfDays = (from cl in customerLedgers
        //                                                    where cl.DateInvoiceJulian > dateToCompare
        //                                                    select cl);
        //    if (!queryToCheckIfInvocesWithSetNumberOfDays.Any())
        //    {
        //        accountNumberLookUpResult.ResultCode =
        //            AccountNumberLookUpResultCode.DidNotFindRecordsWithinSetDays;
        //        return accountNumberLookUpResult;
        //    }

        //    accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //    return accountNumberLookUpResult;
        //}

        //public AccountNumberLookUpResult AccountNumberLookByAccountNumberAndInvoiceNumber(int accountNumber,
        //                                                                                  int invoiceNumber)
        //{
        //    string invoice = invoiceNumber.ToString().PadRight(25);

        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult
        //        {
        //            AccountNumber = accountNumber,
        //            DefaultInvoiceNumber = invoiceNumber
        //        };

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {

        //            List<CustomerLedger> customerLedgers = (from cl in context.CustomerLedgers
        //                                                    where cl.AddressNumberParent == accountNumber
        //                                                          && cl.DocumentNumber == invoiceNumber
        //                                                    select cl).ToList();

        //            bool groupBill = customerLedgers.Any(customerLedger => customerLedger.ARReportingCode8 == "GRP");

        //            if (groupBill)
        //            {
        //                customerLedgers = (from cl in context.CustomerLedgers
        //                                   where cl.AddressNumberParent == accountNumber
        //                                         && cl.SupplierInvoiceNumber == DbFunctions.AsNonUnicode(invoice)
        //                                   select cl).ToList();
        //            }

        //            accountNumberLookUpResult = GetAccountNumberLookUpResult(accountNumberLookUpResult, customerLedgers);

        //            if (accountNumberLookUpResult.ResultCode == AccountNumberLookUpResultCode.FoundRecords)
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //                accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context, accountNumber);
        //                accountNumberLookUpResult.DefaultTab = GetDefaultTabByInvoiceNumber(context, invoiceNumber);
        //            }

        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }

        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByAccountNumberAndUnitNumber(int accountNumber,
        //                                                                               string unitNumber)
        //{
        //    int daysToGoBack = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DaysToGoBack"));
        //    DateTime today = DateTime.Today;
        //    int dateToCompare = today.AddDays(-daysToGoBack).DateTimeToJdeDate();

        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult
        //        {
        //            AccountNumber = accountNumber
        //        };

        //    unitNumber = _helperService.FormatUnitNumber(unitNumber).PadRight(25);

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            IQueryable<CustomerLedger> query1 = (from cl in context.CustomerLedgers
        //                                                 where
        //                                                     cl.AddressNumberParent == accountNumber &&
        //                                                     cl.Reference == DbFunctions.AsNonUnicode(unitNumber)
        //                                                 select cl);
        //            if (query1.Any())
        //            {
        //                IQueryable<CustomerLedger> iQueryable =
        //                    (
        //                        context.CustomerLedgers.Where(cl => cl.AddressNumberParent == accountNumber
        //                                                            && (cl.DateInvoiceJulian > dateToCompare)));


        //                if (iQueryable.Any())
        //                {
        //                    accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //                    accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                            accountNumber);
        //                    accountNumberLookUpResult.DefaultTab = GetDefaultTabByAccountNumber(context, accountNumber);
        //                }
        //                else
        //                {
        //                    accountNumberLookUpResult.ResultCode =
        //                        AccountNumberLookUpResultCode.DidNotFindRecordsWithinSetDays;

        //                }
        //            }
        //            else
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }

        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByAccountNumberAndPurchaseOrderNumber(int accountNumber,
        //                                                                                        string
        //                                                                                            purchaseOrderNumber)
        //{
        //    int daysToGoBack = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DaysToGoBack"));
        //    DateTime today = DateTime.Today;
        //    int dateToCompare = today.AddDays(-daysToGoBack).DateTimeToJdeDate();

        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult
        //        {
        //            AccountNumber = accountNumber
        //        };

        //    purchaseOrderNumber = purchaseOrderNumber.PadRight(8);

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            IQueryable<CustomerLedger> query1 = (from cl in context.CustomerLedgers
        //                                                 where
        //                                                     cl.AddressNumberParent == accountNumber &&
        //                                                     cl.PurchaseOrder ==
        //                                                     DbFunctions.AsNonUnicode(purchaseOrderNumber)
        //                                                 select cl);
        //            if (query1.Any())
        //            {
        //                IQueryable<CustomerLedger> iQueryable =
        //                    (
        //                        context.CustomerLedgers.Where(cl => cl.AddressNumberParent == accountNumber
        //                                                            && (cl.DateInvoiceJulian > dateToCompare)));


        //                if (iQueryable.Any())
        //                {
        //                    iQueryable =
        //                        iQueryable.Where(cl => cl.DocumentCompany == "00020" || cl.DocumentCompany == "00010");

        //                    if (iQueryable.Any())
        //                    {
        //                        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //                        accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                                accountNumber);
        //                        accountNumberLookUpResult.DefaultTab = GetDefaultTabByAccountNumber(context,
        //                                                                                            accountNumber);
        //                    }
        //                    else
        //                    {
        //                        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.NonUSorCA;
        //                    }

        //                }
        //                else
        //                {
        //                    accountNumberLookUpResult.ResultCode =
        //                        AccountNumberLookUpResultCode.DidNotFindRecordsWithinSetDays;

        //                }
        //            }
        //            else
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }

        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByAccountNumberAndZipCode(int accountNumber, string zipCode)
        //{
        //    int daysToGoBack = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DaysToGoBack"));
        //    DateTime today = DateTime.Today;
        //    int dateToCompare = today.AddDays(-daysToGoBack).DateTimeToJdeDate();

        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult
        //        {
        //            AccountNumber = accountNumber
        //        };

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            string postalCode = _addressBookService.GetPostalCodeByID(context, accountNumber);

        //            if (postalCode == zipCode)
        //            {

        //                IQueryable<CustomerLedger> query1 = (from cl in context.CustomerLedgers
        //                                                     where cl.AddressNumberParent == accountNumber
        //                                                     select cl);

        //                if (query1.Any())
        //                {
        //                    IQueryable<CustomerLedger> iQueryable =
        //                        (
        //                            context.CustomerLedgers.Where(cl => cl.AddressNumberParent == accountNumber
        //                                                                && (cl.DateInvoiceJulian > dateToCompare)));


        //                    if (iQueryable.Any())
        //                    {
        //                        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //                        accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                                accountNumber);
        //                        accountNumberLookUpResult.DefaultTab = GetDefaultTabByAccountNumber(context,
        //                                                                                            accountNumber);
        //                    }
        //                    else
        //                    {
        //                        accountNumberLookUpResult.ResultCode =
        //                            AccountNumberLookUpResultCode.DidNotFindRecordsWithinSetDays;

        //                    }
        //                }
        //                else
        //                {
        //                    accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //                }
        //            }
        //            else
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }

        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByInvoiceNumberAndUnitNumber(int invoiceNumber,
        //                                                                               string unitNumber)
        //{
        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult
        //        {
        //            DefaultInvoiceNumber = invoiceNumber
        //        };

        //    string invoice = invoiceNumber.ToString().PadRight(25);
        //    unitNumber = _helperService.FormatUnitNumber(unitNumber).PadRight(25);

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {

        //            List<CustomerLedger> customerLedgers = (from cl in context.CustomerLedgers
        //                                                    where cl.DocumentNumber == invoiceNumber
        //                                                          &&
        //                                                          cl.Reference ==
        //                                                          DbFunctions.AsNonUnicode(unitNumber)
        //                                                    select cl).ToList();

        //            bool groupBill = customerLedgers.Any(customerLedger => customerLedger.ARReportingCode8 == "GRP");

        //            if (groupBill)
        //            {
        //                customerLedgers = (from cl in context.CustomerLedgers
        //                                   where
        //                                       cl.SupplierInvoiceNumber == DbFunctions.AsNonUnicode(invoice) &&
        //                                       cl.Reference == DbFunctions.AsNonUnicode(unitNumber)
        //                                   select cl).ToList();
        //            }

        //            accountNumberLookUpResult = GetAccountNumberLookUpResult(accountNumberLookUpResult, customerLedgers);

        //            if (accountNumberLookUpResult.ResultCode == AccountNumberLookUpResultCode.FoundRecords)
        //            {

        //                decimal? accountNumber =
        //                    (from cl in customerLedgers select cl.AddressNumberParent).FirstOrDefault();

        //                accountNumberLookUpResult.AccountNumber = Convert.ToInt32(accountNumber);
        //                accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                        Convert.ToInt32(
        //                                                                                            accountNumber));
        //                accountNumberLookUpResult.DefaultTab = GetDefaultTabByInvoiceNumber(context, invoiceNumber);
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }

        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByInvoiceNumberAndPurchaseOrderNumber(int invoiceNumber,
        //                                                                                        string
        //                                                                                            purchaseOrderNumber)
        //{
        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult
        //        {
        //            DefaultInvoiceNumber = invoiceNumber
        //        };

        //    string invoice = invoiceNumber.ToString().PadRight(25);
        //    purchaseOrderNumber = purchaseOrderNumber.PadRight(8);

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            List<CustomerLedger> customerLedgers = (from cl in context.CustomerLedgers
        //                                                    where cl.DocumentNumber == invoiceNumber
        //                                                          &&
        //                                                          cl.PurchaseOrder ==
        //                                                          DbFunctions.AsNonUnicode(purchaseOrderNumber)
        //                                                    select cl).ToList();

        //            bool groupBill = customerLedgers.Any(customerLedger => customerLedger.ARReportingCode8 == "GRP");

        //            if (groupBill)
        //            {
        //                customerLedgers = (from cl in context.CustomerLedgers
        //                                   where
        //                                       cl.SupplierInvoiceNumber == DbFunctions.AsNonUnicode(invoice) &&
        //                                       cl.PurchaseOrder == DbFunctions.AsNonUnicode(purchaseOrderNumber)
        //                                   select cl).ToList();
        //            }

        //            accountNumberLookUpResult = GetAccountNumberLookUpResult(accountNumberLookUpResult, customerLedgers);

        //            if (accountNumberLookUpResult.ResultCode == AccountNumberLookUpResultCode.FoundRecords)
        //            {
        //                decimal? accountNumber =
        //                    (from cl in customerLedgers select cl.AddressNumberParent).FirstOrDefault();

        //                accountNumberLookUpResult.AccountNumber = Convert.ToInt32(accountNumber);
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //                accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                        Convert.ToInt32(
        //                                                                                            accountNumber));
        //                accountNumberLookUpResult.DefaultTab = GetDefaultTabByInvoiceNumber(context, invoiceNumber);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }

        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByInvoiceNumberAndZipCode(int invoiceNumber, string zipCode)
        //{
        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult
        //        {
        //            DefaultInvoiceNumber = invoiceNumber
        //        };

        //    string invoice = invoiceNumber.ToString().PadRight(25);

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            var addressNumberParent = (from cl in context.CustomerLedgers
        //                                       where
        //                                           cl.DocumentNumber == invoiceNumber
        //                                       select cl.AddressNumberParent);
        //            if (addressNumberParent.Any())
        //            {

        //                int accountNumber = Convert.ToInt32(addressNumberParent.FirstOrDefault());

        //                string postalCode = _addressBookService.GetPostalCodeByID(context, accountNumber);

        //                if (postalCode == zipCode)
        //                {
        //                    List<CustomerLedger> customerLedgers = (from cl in context.CustomerLedgers
        //                                                            where
        //                                                                cl.DocumentNumber == invoiceNumber
        //                                                            select cl).ToList();

        //                    bool groupBill =
        //                        customerLedgers.Any(customerLedger => customerLedger.ARReportingCode8 == "GRP");

        //                    if (groupBill)
        //                    {
        //                        customerLedgers = (from cl in context.CustomerLedgers
        //                                           join a in context.Addresses on cl.AddressNumberParent equals
        //                                               a.ID
        //                                           where
        //                                               cl.SupplierInvoiceNumber ==
        //                                               DbFunctions.AsNonUnicode(invoice) &&
        //                                               a.PostalCode == DbFunctions.AsNonUnicode(zipCode)
        //                                           select cl).ToList();
        //                    }
        //                    accountNumberLookUpResult = GetAccountNumberLookUpResult(accountNumberLookUpResult,
        //                                                                             customerLedgers);
        //                    if (accountNumberLookUpResult.ResultCode == AccountNumberLookUpResultCode.FoundRecords)
        //                    {
        //                        accountNumberLookUpResult.AccountNumber = Convert.ToInt32(accountNumber);
        //                        accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                                Convert
        //                                                                                                    .ToInt32
        //                                                                                                    (
        //                                                                                                        accountNumber));
        //                        accountNumberLookUpResult.DefaultTab = GetDefaultTabByInvoiceNumber(context,
        //                                                                                            invoiceNumber);
        //                    }

        //                }
        //                else
        //                {
        //                    accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //                }
        //            }
        //            else
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }
        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByUnitNumberAndPurchaseOrderNumber(string unitNumber,
        //                                                                                     string purchaseOrderNumber)
        //{
        //    int daysToGoBack = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DaysToGoBack"));
        //    DateTime today = DateTime.Today;
        //    int dateToCompare = today.AddDays(-daysToGoBack).DateTimeToJdeDate();

        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult();

        //    purchaseOrderNumber = purchaseOrderNumber.PadRight(8);
        //    unitNumber = _helperService.FormatUnitNumber(unitNumber).PadRight(25);

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            List<decimal?> accountNumbers = (from cl in context.CustomerLedgers
        //                                             where
        //                                                 cl.Reference == DbFunctions.AsNonUnicode(unitNumber) &&
        //                                                 cl.PurchaseOrder ==
        //                                                 DbFunctions.AsNonUnicode(purchaseOrderNumber)
        //                                             select cl.AddressNumberParent).Distinct().ToList();
        //            if (accountNumbers.Any())
        //            {
        //                if (accountNumbers.Count() == 1)
        //                {
        //                    int accountNumber = Convert.ToInt32(accountNumbers[0]);

        //                    IQueryable<CustomerLedger> iQueryable =
        //                        (
        //                            context.CustomerLedgers.Where(cl => cl.AddressNumberParent == accountNumber
        //                                                                &&
        //                                                                (cl.DateInvoiceJulian > dateToCompare)));

        //                    if (iQueryable.Any())
        //                    {
        //                        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //                        accountNumberLookUpResult.AccountNumber = accountNumber;
        //                        accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                                accountNumber);
        //                        accountNumberLookUpResult.DefaultTab = GetDefaultTabByAccountNumber(context,
        //                                                                                            accountNumber);
        //                    }
        //                    else
        //                    {
        //                        accountNumberLookUpResult.ResultCode =
        //                            AccountNumberLookUpResultCode.DidNotFindRecordsWithinSetDays;
        //                    }
        //                }
        //                else
        //                {
        //                    accountNumberLookUpResult.ResultCode =
        //                        AccountNumberLookUpResultCode.MoreThanOneAccountNumber;
        //                }
        //            }
        //            else
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }


        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByUnitNumberAndZipCode(string unitNumber, string zipCode)
        //{
        //    int daysToGoBack = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DaysToGoBack"));
        //    DateTime today = DateTime.Today;
        //    int dateToCompare = today.AddDays(-daysToGoBack).DateTimeToJdeDate();

        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult();
        //    unitNumber = _helperService.FormatUnitNumber(unitNumber).PadRight(25);
        //    //zipCode = zipCode.PadRight(12);

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {

        //            IQueryable<decimal?> accountNumberList = (from cl in context.CustomerLedgers
        //                                                      where
        //                                                          cl.Reference ==
        //                                                          DbFunctions.AsNonUnicode(unitNumber)
        //                                                      select cl.AddressNumberParent).Distinct();

        //            List<string> postalCodes = new List<string>();


        //            foreach (var accountNumber in accountNumberList)
        //            {
        //                string postalCode = _addressBookService.GetPostalCodeByID(context,
        //                                                                          Convert.ToInt32(accountNumber));
        //                postalCodes.Add(postalCode);
        //            }

        //            int countOfPostalCodes = postalCodes.Count(code => code == zipCode);

        //            if (countOfPostalCodes == 0)
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //                return accountNumberLookUpResult;
        //            }
        //            if (countOfPostalCodes > 1)
        //            {
        //                accountNumberLookUpResult.ResultCode =
        //                    AccountNumberLookUpResultCode.MoreThanOneAccountNumber;
        //                return accountNumberLookUpResult;
        //            }
        //            if (countOfPostalCodes == 1)
        //            {
        //                var accountNumber = (from cl in context.CustomerLedgers
        //                                     where cl.Reference == DbFunctions.AsNonUnicode(unitNumber)
        //                                     select cl.AddressNumberParent).FirstOrDefault();

        //                IQueryable<CustomerLedger> iQueryable =
        //                    (
        //                        context.CustomerLedgers.Where(cl => cl.AddressNumberParent == accountNumber
        //                                                            &&
        //                                                            (cl.DateInvoiceJulian > dateToCompare)));
        //                if (iQueryable.Any())
        //                {
        //                    accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //                    accountNumberLookUpResult.AccountNumber = Convert.ToInt32(accountNumber);
        //                    accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                            Convert.ToInt32(
        //                                                                                                accountNumber));
        //                    accountNumberLookUpResult.DefaultTab = GetDefaultTabByAccountNumber(context,
        //                                                                                        Convert.ToInt32(
        //                                                                                            accountNumber));
        //                }
        //                else
        //                {
        //                    accountNumberLookUpResult.ResultCode =
        //                        AccountNumberLookUpResultCode.DidNotFindRecordsWithinSetDays;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }

        //    return accountNumberLookUpResult;

        //}

        //public AccountNumberLookUpResult AccountNumberLookByPurchaseOrderNumberAndZipCode(string purchaseOrderNumber,
        //                                                                                  string zipCode)
        //{
        //    int daysToGoBack = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DaysToGoBack"));
        //    DateTime today = DateTime.Today;
        //    int dateToCompare = today.AddDays(-daysToGoBack).DateTimeToJdeDate();

        //    AccountNumberLookUpResult accountNumberLookUpResult = new AccountNumberLookUpResult();


        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            IQueryable<decimal?> accountNumberList = (from cl in context.CustomerLedgers
        //                                                      where
        //                                                          cl.PurchaseOrder ==
        //                                                          DbFunctions.AsNonUnicode(purchaseOrderNumber)
        //                                                      select cl.AddressNumberParent).Distinct();

        //            List<string> postalCodes = new List<string>();

        //            foreach (var accountNumber in accountNumberList)
        //            {
        //                string postalCode = _addressBookService.GetPostalCodeByID(context,
        //                                                                          Convert.ToInt32(accountNumber));
        //                postalCodes.Add(postalCode);
        //            }

        //            int countOfPostalCodes = postalCodes.Count(code => code == zipCode);

        //            if (countOfPostalCodes == 0)
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.DidNotFindRecords;
        //                return accountNumberLookUpResult;
        //            }

        //            if (countOfPostalCodes < 1)
        //            {
        //                accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.MoreThanOneAccountNumber;
        //                return accountNumberLookUpResult;
        //            }
        //            if (countOfPostalCodes == 1)
        //            {
        //                var accountNumber = (from cl in context.CustomerLedgers
        //                                     where cl.PurchaseOrder == DbFunctions.AsNonUnicode(purchaseOrderNumber)
        //                                     select cl.AddressNumberParent).FirstOrDefault();

        //                IQueryable<CustomerLedger> iQueryable =
        //                    (
        //                        context.CustomerLedgers.Where(cl => cl.AddressNumberParent == accountNumber
        //                                                            &&
        //                                                            (cl.DateInvoiceJulian > dateToCompare)));
        //                if (iQueryable.Any())
        //                {
        //                    accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.FoundRecords;
        //                    accountNumberLookUpResult.AccountNumber = Convert.ToInt32(accountNumber);
        //                    accountNumberLookUpResult.CompanyName = _addressBookService.GetNameByID(context,
        //                                                                                            Convert.ToInt32(
        //                                                                                                accountNumber));
        //                    accountNumberLookUpResult.DefaultTab = GetDefaultTabByAccountNumber(context,
        //                                                                                        Convert.ToInt32(
        //                                                                                            accountNumber));
        //                }
        //                else
        //                {
        //                    accountNumberLookUpResult.ResultCode =
        //                        AccountNumberLookUpResultCode.DidNotFindRecordsWithinSetDays;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //        accountNumberLookUpResult.ResultCode = AccountNumberLookUpResultCode.GeneralError;
        //    }

        //    return accountNumberLookUpResult;

        //}

        //public void SetInvoiceStatusToJ(int boaTransactionID)
        //{
        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {

        //            IQueryable<BOATransactionDetail> boaTransactionDetails = (from btd in context.BOATransactionDetails
        //                                                                      where
        //                                                                          btd.BOATransactionID ==
        //                                                                          boaTransactionID
        //                                                                          && btd.AmountToPayTotal > 0
        //                                                                      select btd);

        //            foreach (BOATransactionDetail boaTransactionDetail in boaTransactionDetails)
        //            {
        //                string invoice = boaTransactionDetail.InvoiceNumber.ToString().PadRight(25);

        //                IQueryable<CustomerLedger> customerLedgers = (from cl in context.CustomerLedgers
        //                                                              where
        //                                                                  cl.SupplierInvoiceNumber ==
        //                                                                  DbFunctions.AsNonUnicode(invoice)
        //                                                              select cl);

        //                if (customerLedgers.Any())
        //                {
        //                    foreach (CustomerLedger customerLedger in customerLedgers)
        //                    {
        //                        if (customerLedger.PayStatusCode != "J" && customerLedger.PayStatusCode != "P")
        //                        {
        //                            customerLedger.PayStatusCode = "J";
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    int invoiceInt = Convert.ToInt32(invoice);

        //                    IQueryable<CustomerLedger> customerLedgers2 = (from cl in context.CustomerLedgers
        //                                                                   where
        //                                                                       cl.DocumentNumber == invoiceInt
        //                                                                   select cl);

        //                    foreach (CustomerLedger customerLedger in customerLedgers2)
        //                    {
        //                        customerLedger.PayStatusCode = "J";
        //                    }

        //                }
        //            }

        //            context.SaveChanges();

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }
        //}

        //private int GetDefaultTabByInvoiceNumber(WSDBContext context, int invoiceNumber)
        //{
        //    int defaultTab = 0;

        //    var query = (from cl in context.CustomerLedgers
        //                 where cl.DocumentNumber == invoiceNumber
        //                 group cl by new
        //                     {
        //                         cl.AddressNumberParent,
        //                         cl.SupplierInvoiceNumber,
        //                         cl.DocumentType,
        //                         cl.PayStatusCode,
        //                         cl.DocumentCompany,
        //                     }
        //                 into g
        //                 select new
        //                     {
        //                         AmountOpen = g.Sum(cl => cl.AmountOpen)
        //                     }).FirstOrDefault();

        //    if (query != null)
        //    {
        //        var amountOpen = Convert.ToInt32(query.AmountOpen)/100;

        //        if (amountOpen == 0)
        //        {
        //            defaultTab = 1;
        //        }

        //    }


        //    return defaultTab;
        //}

        //private int GetDefaultTabByAccountNumber(WSDBContext context, int accountNumber)
        //{
        //    int defaultTab = 1;

        //    var query = (from cl in context.CustomerLedgers
        //                 where cl.AddressNumberParent == accountNumber
        //                 group cl by new
        //                     {
        //                         cl.AddressNumberParent,
        //                         cl.SupplierInvoiceNumber,
        //                         cl.DocumentType,
        //                         cl.PayStatusCode,
        //                         cl.DocumentCompany,
        //                     }
        //                 into g
        //                 select new
        //                     {
        //                         AmountOpen = g.Sum(cl => cl.AmountOpen)
        //                     });

        //    foreach (var x in query)
        //    {
        //        var amountOpen = Convert.ToInt32(x.AmountOpen)/100;

        //        if (amountOpen > 0)
        //        {
        //            defaultTab = 0;
        //            return defaultTab;
        //        }
        //    }

        //    return defaultTab;
        //}

        //public List<int> GetAddressNumberParentsByBOATransactionID(int id)
        //{
        //    List<int> addressNumberParents = new List<int>();

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            IQueryable<decimal?> invoiceNumbers = (from btd in context.BOATransactionDetails
        //                                                   where btd.BOATransactionID == id
        //                                                   select btd.InvoiceNumber);


        //            List<decimal?> numbers = (from cl in context.CustomerLedgers
        //                                      where invoiceNumbers.Contains(cl.DocumentNumber)
        //                                      select cl.AddressNumberParent).Distinct().ToList();

        //            addressNumberParents.AddRange(numbers.Select(number => Convert.ToInt32(number)));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return addressNumberParents;
        //}

        //public Dictionary<int, string> GetDocumentTypesByDocumentNumber(int[] idArray, WSDBContext context)
        //{
        //    Dictionary<int, string> dictionary = new Dictionary<int, string>();

        //    foreach (int id in idArray)
        //    {
        //        decimal documentNumber = Convert.ToDecimal(id);

        //        string documentType = (from cl in context.CustomerLedgers
        //                               where cl.DocumentNumber == documentNumber
        //                               select cl.DocumentType).FirstOrDefault();

        //        dictionary.Add(id, documentType);
        //    }
        //    return dictionary;
        //}

        //public Dictionary<int, string> GetDocumentTypesByDocumentNumber(int[] idArray)
        //{
        //    Dictionary<int, string> dictionary = new Dictionary<int, string>();

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            dictionary = GetDocumentTypesByDocumentNumber(idArray, context);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return dictionary;

        //}

        //public bool CheckToMakeSureAccountNumberMatchesInvoices(int accountNumber, int[] invoices)
        //{
        //    bool match = true;

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            foreach (int invoice in invoices)
        //            {
        //                decimal? addressNumberParent = (from cl in context.CustomerLedgers
        //                                       where cl.DocumentNumber == invoice
        //                                       select cl.AddressNumberParent).FirstOrDefault();
                        
        //                int invoiceAccountNumber = Convert.ToInt32(addressNumberParent);

        //                if (accountNumber != invoiceAccountNumber)
        //                {
        //                    match = false;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }
        //    return match;
        //}

    }
}

