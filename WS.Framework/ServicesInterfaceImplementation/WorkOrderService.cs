using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using WS.Framework.Objects;
using WS.Framework.Objects.Enums;
using WS.Framework.Objects.MobileWorkOrder;
using WS.Framework.Objects.WorkOrder;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class WorkOrderService : IWorkOrderService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IHelperService _helperService;
        private readonly IJDEService _jdeService;
        private readonly IItemService _itemService;
        private readonly IEquipmentService _equipmentService;
        private readonly IOracleHelperService _oracleHelperService;
        private readonly ISecurityService _securityService;
        private readonly IUnitService _unitService;

        public WorkOrderService(IHelperService helperService, IJDEService jdeService, IItemService itemService, IEquipmentService equipmentService, IOracleHelperService oracleHelperService, ISecurityService securityService, IUnitService unitService)
        {
            _helperService = helperService;
            _jdeService = jdeService;
            _itemService = itemService;
            _equipmentService = equipmentService;
            _oracleHelperService = oracleHelperService;
            _securityService = securityService;
            _unitService = unitService;
        }

        public string RunJDEBusinessFunctionCallWOFunctionByWorkOrderID(string workOrderID)
        {
            string response = "Success";
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    decimal woID = Convert.ToInt32(workOrderID);

                    var query = (from wo in context.WorkOrders
                                 where wo.ID == woID
                                 select new
                                     {
                                         wo.AssetItemNumber,
                                         wo.TypeWO
                                     }
                                ).FirstOrDefault();

                    if (query != null)
                    {
                        string assetItemNumber = Convert.ToString(query.AssetItemNumber);
                        string typeWO = Convert.ToString(query.TypeWO);

                        JDEBusinessFunction jdeBusinessFunction = new JDEBusinessFunction
                            {
                                CallMethod = "CallWOFunction",
                                Parameters = new Dictionary<string, string>
                                    {
                                        {"mnDocumentOrderInvoiceE", workOrderID},
                                        {"cCalledFromaTablet", "Y"},
                                        {"cReturnCode", ""},
                                        {"mnErrorIndicator", ""},
                                        {"szPcErrorMessage", ""},
                                        {"mnAssetItemNumber", assetItemNumber},
                                        {"mnAmountOriginalDollars", ""},
                                        {"cTypeWo", typeWO},
                                    }
                            };

                        JDEBusinessFunctionResponse jdeBusinessFunctionResponse =
                            _jdeService.CallJDEBusinessFunction(jdeBusinessFunction);

                        if (jdeBusinessFunctionResponse.ReturnCode != 0)
                        {
                            response = "There was an error connecting to JDE";
                            return response;
                        }

                        if (jdeBusinessFunctionResponse.Parameters["cReturnCode"] == "1")
                        {
                            response = jdeBusinessFunctionResponse.Parameters["szPcErrorMessage"];
                            return response;
                        }
                    }
                    else
                    {
                        response = "Could not find the Work Order Number";
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return response;
        }

        public void ProcessMWOImages(string folderPath)
        {
            if (AreAllImagesUploaded(folderPath))
            {
                ProcessImagesWithoutCheck(folderPath);
            }
        }

        public void ProcessImagesWithoutCheck(string folderPath)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    string imageFileSavePath = ConfigurationManager.AppSettings.Get("FeithImageFileLocation");

                    IEnumerable<string> files = Directory.EnumerateFiles(folderPath, "*.jpg");

                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);

                        string[] fileParts = fileName.Split(Convert.ToChar("."));

                        MWOFeithImage mwoFeithImage = GetMWOFeithImageByMWOImageID(fileParts[0], context);

                        if (mwoFeithImage != null)
                        {
                            File.Move(folderPath + @"\" + fileName, imageFileSavePath + @"\" + fileName);
                            CreateWOIFile(mwoFeithImage);
                        }
                        else
                        {
                            logger.Info("The image is not in the database and cannot be moved into Fieth and will deleted: " + folderPath + @"\" + fileName);
                        }

                    }
                }

                Directory.Delete(folderPath, true);
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
        }

        private bool AreAllImagesUploaded(string folderPath)
        {
            int countOfImagesFolder = Directory.GetFiles(folderPath, "*.jpg").Length;

            string guid = Path.GetFileName(folderPath);

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    int totalImages = (from mfi in context.MWOFeithImages
                                       where mfi.WorkOrderGUID == guid
                                       select mfi).Count();

                    if (countOfImagesFolder == totalImages)
                        return true;

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return false;
        }

        private void CreateWOIFile(MWOFeithImage mwoFeithImage)
        {
            string woiFileSavePath = ConfigurationManager.AppSettings.Get("FeithWOIFileLocation");
            string feithImageFileLocalPath = ConfigurationManager.AppSettings.Get("FeithImageFileLocalPath");
            string imageType;
            string docType = "";
            string adUserName = _securityService.GetADUserNameByEmployeeNumber(mwoFeithImage.EmployeeNumber);

            string submittedBy = string.IsNullOrEmpty(adUserName) ? mwoFeithImage.EmployeeNumber : adUserName;

            if (mwoFeithImage.Damaged == "True")
            {
                imageType = "DAMAGES";
            }
            else
            {
                switch (mwoFeithImage.Location)
                {
                    case "I":
                        imageType = "INTERIOR";
                        break;
                    case "E":
                        imageType = "EXTERIOR";
                        break;
                    default:
                        imageType = "";
                        break;
                }
            }

            switch (Convert.ToInt32(mwoFeithImage.ImageTypeID))
            {
                case 1:
                    docType = "INBOUND";
                    break;
                case 2:
                    docType = "OUTBOUND";
                    break;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Unit Image Processing");
            sb.AppendLine("Process: Unit Images into Unit Imaging FC");
            sb.AppendLine("SERIAL NUM: " + mwoFeithImage.UnitNumber);
            sb.AppendLine("PARENT NUM: " + mwoFeithImage.ComplexUnitNumber);
            sb.AppendLine("ORDER NUM: " + mwoFeithImage.OMBOrderInbound);
            sb.AppendLine("DOC TYPE: " + docType);
            sb.AppendLine("STATUS: ACTIVE");
            sb.AppendLine("IMAGE TYPE: " + imageType);
            sb.AppendLine("SUBMITTED BY: " + submittedBy);
            sb.AppendLine("DATE TIME SUBMITTED: " + DateTime.Now);
            sb.AppendLine("REF ID: " + mwoFeithImage.WorkOrderID);
            sb.AppendLine("FILE: " + feithImageFileLocalPath + @"\" + mwoFeithImage.MWOImageID + ".jpg");

            using (StreamWriter outfile = new StreamWriter(woiFileSavePath + @"\" + mwoFeithImage.MWOImageID + ".woi"))
            {
                outfile.Write(sb.ToString());
            }
        }

        public WorkOrder GetWorkOrderByID(int workOrderID, WSJDE context)
        {
            WorkOrder workOrder = (from wo in context.WorkOrders
                                   where wo.ID == workOrderID
                                   select wo).FirstOrDefault();

            return workOrder;
        }

        //public WorkOrder GetWorkOrderByID(int workOrderID)
        //{
        //    WorkOrder workOrder = new WorkOrder();

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            workOrder = GetWorkOrderByID(workOrderID, context);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return workOrder;
        //}


        private MWOFeithImage GetMWOFeithImageByMWOImageID(string mwoImageID, WSJDE context)
        {
            MWOFeithImage mwoFeithImage = (from mfi in context.MWOFeithImages
                                           where mfi.MWOImageID == mwoImageID
                                           select mfi).FirstOrDefault();

            return mwoFeithImage;
        }

        public WorkOrderResponse AddUpdateWorkOrder(MobileWorkOrder mobileWorkOrder)
        {
            WorkOrderResponse workOrderResponse = new WorkOrderResponse();

            switch (mobileWorkOrder.Type)
            {
                case "Inbound":
                    workOrderResponse = InboundWorkOrder(mobileWorkOrder);
                    break;
                case "Outbound":
                    workOrderResponse = OutboundWorkorder(mobileWorkOrder);
                    break;
            }

            return workOrderResponse;
        }

        public WorkOrderResponse OutboundWorkorder(MobileWorkOrder mobileWorkOrder)
        {
            WorkOrderResponse workOrderResponse = new WorkOrderResponse { ReturnCode = 1, Success = true, WorkOrderMessages = new List<WorkOrderMessage>() };
            WorkOrderCommonObject workOrderCommonObject = GetWorkOrderCommonObject(mobileWorkOrder);
            mobileWorkOrder.ReportLocationNumber = mobileWorkOrder.ReportLocationNumber;
            mobileWorkOrder.UnitNumber = CheckUnitNumberFormatting(mobileWorkOrder.UnitNumber);

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    workOrderResponse = UpdateAttributes(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                    if (!workOrderResponse.Success)
                    {
                        logger.Info("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                     ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                        workOrderResponse.WorkOrderID = 0;
                        return workOrderResponse;
                    }

                    workOrderResponse = Step3B(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                    if (!workOrderResponse.Success)
                    {
                        logger.Info("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                     ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                        workOrderResponse.WorkOrderID = 0;
                        return workOrderResponse;
                    }

                    workOrderResponse = Step9(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                    if (!workOrderResponse.Success)
                    {
                        logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                     ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                        workOrderResponse.WorkOrderID = 0;
                        return workOrderResponse;
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                _helperService.LogDbEntityValidationException(dbEx);
                workOrderResponse.ReturnCode = 3;
                workOrderResponse.Success = false;
                workOrderResponse.WorkOrderID = 0;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = "Database Error" });
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                workOrderResponse.ReturnCode = 3;
                workOrderResponse.Success = false;
                workOrderResponse.WorkOrderID = 0;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage
                {
                    Type = 2,
                    Message = "Unknown Error has occured"
                });
            }

            return workOrderResponse;
        }

        public WorkOrderResponse InboundWorkOrder(MobileWorkOrder mobileWorkOrder)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Add Work Order for Unit: " + mobileWorkOrder.UnitNumber);

            mobileWorkOrder.ReportLocationNumber = mobileWorkOrder.ReportLocationNumber;
            mobileWorkOrder.UnitNumber = CheckUnitNumberFormatting(mobileWorkOrder.UnitNumber);

            WorkOrderResponse workOrderResponse = new WorkOrderResponse { ReturnCode = 1, Success = true, WorkOrderMessages = new List<WorkOrderMessage>() };

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    WorkOrderCommonObject workOrderCommonObject = GetWorkOrderCommonObject(mobileWorkOrder);

                    if (Convert.ToInt32(mobileWorkOrder.WorkOrderID) == 0 ||
                        String.IsNullOrEmpty(mobileWorkOrder.WorkOrderID))
                    {
                        workOrderResponse.WorkOrderType = WorkOrderType.Create;
                    }
                    else
                    {
                        workOrderResponse.WorkOrderType = WorkOrderType.BusinessFuntionOnly;
                        workOrderCommonObject.ID = Convert.ToInt32(mobileWorkOrder.WorkOrderID);
                        workOrderResponse.WorkOrderID = Convert.ToInt32(mobileWorkOrder.WorkOrderID);
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = UpdateAttributes(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Info("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step1(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Info("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step2(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step3A(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step3B(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step4(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step5(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step6(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step7(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }
                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step8(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }

                    if (workOrderResponse.WorkOrderType != WorkOrderType.BusinessFuntionOnly)
                    {
                        workOrderResponse = Step9(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                        if (!workOrderResponse.Success)
                        {
                            logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                         ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                            workOrderResponse.WorkOrderID = 0;
                            return workOrderResponse;
                        }
                    }


                    workOrderResponse = Step10(workOrderResponse, workOrderCommonObject, mobileWorkOrder, context);

                    if (!workOrderResponse.Success)
                    {
                        logger.Error("Adding a new work order failed, ID={0}, Message={1}", workOrderCommonObject.ID,
                                     ConvertMessageListToString(workOrderResponse.WorkOrderMessages));

                        return workOrderResponse;
                    }

                }
            }
            catch (DbEntityValidationException dbEx)
            {
                _helperService.LogDbEntityValidationException(dbEx);
                workOrderResponse.ReturnCode = 3;
                workOrderResponse.Success = false;
                workOrderResponse.WorkOrderID = 0;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = "Database Error" });
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                workOrderResponse.ReturnCode = 3;
                workOrderResponse.Success = false;
                workOrderResponse.WorkOrderID = 0;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage
                    {
                        Type = 2,
                        Message = "Unknown Error has occured"
                    });
            }

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Add Work Order for Unit: " + mobileWorkOrder.UnitNumber);

            return workOrderResponse;
        }

        private WorkOrderCommonObject GetWorkOrderCommonObject(MobileWorkOrder mobileWorkOrder)
        {
            WorkOrderCommonObject workOrderCommonObject = new WorkOrderCommonObject();

            DateTime dateTime = DateTime.Now;

            workOrderCommonObject.JDEDate = _helperService.DateTimeToJDEDate(dateTime);
            workOrderCommonObject.JDETime = _helperService.DateTimeToJDETime(dateTime);
            workOrderCommonObject.TabletID10 = mobileWorkOrder.TabletID.Substring(0, 10);
            workOrderCommonObject.CurrencyCode = GetCurrencyCodeByCompany(mobileWorkOrder.Company);
            workOrderCommonObject.CatagoryGL = _itemService.GetCatagoryGLBySecondItemNumber(mobileWorkOrder.Kit2ndItemNumber);
            workOrderCommonObject.ProductFamily = _equipmentService.GetProductFamilyByAssetID(mobileWorkOrder.AssetID);
            workOrderCommonObject.ParentshortItemNumber =
                _itemService.GetItemIDBySecondItemNumber(mobileWorkOrder.Kit2ndItemNumber);

            return workOrderCommonObject;
        }

        private string ConvertMessageListToString(IEnumerable<WorkOrderMessage> workOrderMessages)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (WorkOrderMessage message in workOrderMessages)
            {
                stringBuilder.Append(message.Type + ": ");
                stringBuilder.Append(message.Message + "; ");
            }

            return stringBuilder.ToString();
        }

        private WorkOrderResponse Step1(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject, MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Server Side Validation");
            logger.Debug("Work Order Type: " + mobileWorkOrder.UserDefinedCodeID);

            if (mobileWorkOrder.UserDefinedCodeID == "N")
            {
                int countOfMajorRefurbWorkorders = GetCountOfMajorRefurbWorkorders(mobileWorkOrder.UnitNumber, context);

                if (countOfMajorRefurbWorkorders >= 1)
                {
                    workOrderResponse.Success = false;
                    workOrderResponse.ReturnCode = 2;
                    workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = "A Major Refurb work order already exists for this unit" });
                }
            }

            if (mobileWorkOrder.UserDefinedCodeID == "G")
            {
                int countOfGetReadyWorkOrders = GetCountOfGetReadyWorkorders(mobileWorkOrder.UnitNumber, context);
                
                if (countOfGetReadyWorkOrders >= 1 )
                {
                    workOrderResponse.Success = false;
                    workOrderResponse.ReturnCode = 2;
                    workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = "A Get Ready work order already exists for this unit" });
                }
            }

            string unitConditionCode = GetUnitConditionCode(mobileWorkOrder.UnitNumber, context);

            if (unitConditionCode == "F" && mobileWorkOrder.UserDefinedCodeID != "3")
            {
                workOrderResponse.Success = false;
                workOrderResponse.ReturnCode = 2;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = "Cannot create the work order because the unit has a condition code of F" });
            }

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Server Side Validation");
            return workOrderResponse;
        }



        private WorkOrderResponse Step2(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject, MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Add Work Order");

            WorkOrder workOrder = new WorkOrder();

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Getting WO Number from JDE");
            int id = _oracleHelperService.GetNextSequenceValue(SequenceNumber.WorkOrder);
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Getting WO Number from JDE");

            if (id == 0)
            {
                workOrderResponse.Success = false;
                workOrderResponse.ReturnCode = 2;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage
                    {
                        Type = 2,
                        Message = "Could not create a new Work Order ID on Oracle table F4801.WADOCO"
                    });

                return workOrderResponse;
            }

            workOrderResponse.WorkOrderID = id;

            workOrder.ID = id;
            workOrder = AddDefaultsToWorkOrder(workOrder, workOrderCommonObject);

            workOrder.AdditionalWork = string.IsNullOrEmpty(mobileWorkOrder.AdditionalWork) ? "WS Work Order" : "Additional Work";
            workOrder.AddressNumber = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
            workOrder.AddressNumberOriginator = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
            workOrder.AssetItemNumber = mobileWorkOrder.AssetID;
            workOrder.Branch = mobileWorkOrder.ReportLocationNumber.PadLeft(12);
            workOrder.BusinessUnit = mobileWorkOrder.RevenueBranch.PadLeft(12);
            workOrder.Company = mobileWorkOrder.Company;
            workOrder.ItemNumberShort = GetItemIDByKit2ndItemNumber(mobileWorkOrder.Kit2ndItemNumber, context);
            workOrder.NextAddressNumber = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
            workOrder.Reference = mobileWorkOrder.ComplexUnitNumber;
            workOrder.ParentWONumber = Convert.ToString(id).PadLeft(8, Convert.ToChar("0"));
            workOrder.SecondItemNumber = mobileWorkOrder.Kit2ndItemNumber;
            workOrder.ServiceAddressNumber = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
            workOrder.Supervisor = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
            workOrder.ThirdItemNumber = mobileWorkOrder.Kit2ndItemNumber;
            workOrder.TypeWO = mobileWorkOrder.UserDefinedCodeID;
            workOrder.UnitorTagNumber = mobileWorkOrder.UnitNumber;
            workOrder.UserID = mobileWorkOrder.EmployeeNumber;
            workOrder.WorkStationID = workOrderCommonObject.TabletID10;

            context.WorkOrders.Add(workOrder);

            logger.Debug("Start - Context Save Changes");
            context.SaveChanges();
            logger.Debug("Start - Context Save Changes");

            workOrderCommonObject.ID = id;

            workOrderResponse.Success = true;

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Add Work Order");

            return workOrderResponse;
        }

        private WorkOrderResponse Step3A(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject, MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Add OMB Work Order");

            OMBWorkOrder ombWorkOrder = new OMBWorkOrder
                {
                    WorkOrderID = workOrderCommonObject.ID,
                    OrderType = "WZ",
                    JobNumber = workOrderCommonObject.TabletID10,
                    OMBOrderInbound = mobileWorkOrder.OMBOrderInbound,
                    Ukurab = 0,
                    UpdateDate = workOrderCommonObject.JDEDate,
                    UpdateTime = workOrderCommonObject.JDETime,
                    UpdateUser = mobileWorkOrder.EmployeeNumber,
                    Urat = 0,
                    Urdt = 0,
                    AdditionalWork = mobileWorkOrder.AdditionalWork
                };

            context.OMBWorkOrders.Add(ombWorkOrder);

            logger.Debug("Start - Context Save Changes");
            context.SaveChanges();
            logger.Debug("End - Context Save Changes");

            workOrderResponse.Success = true;

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Add OMB Work Order");

            return workOrderResponse;
        }

        private WorkOrderResponse Step3B(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject, MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            var assetDescriptions = (from a in context.Assets
                                     where
                                         a.UnitNumber.Trim() == DbFunctions.AsNonUnicode(mobileWorkOrder.UnitNumber)
                                     select a).FirstOrDefault();

            assetDescriptions.Description2 = mobileWorkOrder.Description1;
            assetDescriptions.Description3 = mobileWorkOrder.Description2;

            context.SaveChanges();

            workOrderResponse.Success = true;

            return workOrderResponse;
        }

        private WorkOrderResponse Step4(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject, MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Add Work Order Part");
            int i = 1;

            foreach (MobileWorkOrderPart mobileWorkOrderPart in mobileWorkOrder.MobileWorkOrderParts)
            {
                logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Add Work Order Part: " + mobileWorkOrderPart.PartID + " - " + mobileWorkOrderPart.Part);

                WorkOrderPart workOrderPart = new WorkOrderPart();

                string[] secondAndThirdItemNumbers =
                    _itemService.GetSecondAndThirdItemNumberByItemID(mobileWorkOrderPart.PartID);

                logger.Debug("Start - Get Work Order Part ID - GetNextSequenceValue");
                int id = _oracleHelperService.GetNextSequenceValue(SequenceNumber.WorkOrderPart);
                logger.Debug("Done - Get Work Order Part ID - GetNextSequenceValue");

                if (id == 0)
                {
                    workOrderResponse.Success = false;
                    workOrderResponse.ReturnCode = 2;
                    workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage
                        {
                            Type = 2,
                            Message = "Could not create a new Work Order Part ID on Oracle Table F3111.WMUKID"
                        });

                    return workOrderResponse;
                }

                logger.Debug("Start - Get Unique Key Pricing ID - GetNextSequenceValue");
                int uniqueKeyPricingID = _oracleHelperService.GetNextSequenceValue(SequenceNumber.UniqueKeyPricing);
                logger.Debug("Done - Get Unique Key Pricing ID - GetNextSequenceValue");

                if (uniqueKeyPricingID == 0)
                {
                    workOrderResponse.Success = false;
                    workOrderResponse.ReturnCode = 2;
                    workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage
                    {
                        Type = 2,
                        Message = "Could not create a new Unique Key Pricing ID on Oracle table F31171.WSUKIDP"
                    });

                    return workOrderResponse;
                }

                workOrderPart.ID = id;

                workOrderPart = AddDefaultsToWorkOrderPart(workOrderPart, workOrderCommonObject);

                workOrderPart.AddressNumber = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
                workOrderPart.BusinessUnit = mobileWorkOrder.ReportLocationNumber.PadLeft(12);
                workOrderPart.Component2ndItemNumber = secondAndThirdItemNumbers[0];
                workOrderPart.Component3rdItemNumber = secondAndThirdItemNumbers[1];
                workOrderPart.ComponentBranch = mobileWorkOrder.ReportLocationNumber.PadLeft(12);
                workOrderPart.ComponentItemNumberShort = mobileWorkOrderPart.PartID;
                workOrderPart.ComponentLineNumberBOM = i * 100;
                workOrderPart.Description = mobileWorkOrderPart.Description;
                workOrderPart.NumberParentWONumber = workOrderCommonObject.ID.ToString().PadLeft(8, Convert.ToChar("0"));
                workOrderPart.UnitsOrderTransactionQuantity = mobileWorkOrderPart.Qty;
                workOrderPart.UnitofMeasure = mobileWorkOrderPart.UnitofMeasurePrimary;
                workOrderPart.WorkOrderID = workOrderCommonObject.ID;
                workOrderPart.WorkStationID = workOrderCommonObject.TabletID10;
                workOrderPart.UserID = mobileWorkOrder.EmployeeNumber;

                context.WorkOrderParts.Add(workOrderPart);

                PartExtension partExtension = new PartExtension();

                partExtension = AddDefaultsToPartExtension(partExtension, workOrderCommonObject);

                partExtension.WorkOrderPartID = id;
                partExtension.BillableCurrency = workOrderCommonObject.CurrencyCode;
                partExtension.CurrencyCodeTo = workOrderCommonObject.CurrencyCode;
                partExtension.WorkStationID = workOrderCommonObject.TabletID10;
                partExtension.PercentCovered = mobileWorkOrderPart.DamageBillingPercentage * 100;
                partExtension.ProgramID = "MWO";
                partExtension.ServiceAddressNumber = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
                partExtension.TimeofDay = workOrderCommonObject.JDETime;
                partExtension.UniqueKeyPricing = uniqueKeyPricingID;
                partExtension.DateUpdated = workOrderCommonObject.JDEDate;
                partExtension.UserID = mobileWorkOrder.EmployeeNumber;

                context.PartExtensions.Add(partExtension);

                i++;

                logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Add Work Order Part: " + mobileWorkOrderPart.PartID + " - " + mobileWorkOrderPart.Part);
            }

            logger.Debug("Start - Context Save Changes");
            context.SaveChanges();
            logger.Debug("Done - Context Save Changes");
            workOrderResponse.Success = true;
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Add Work Order Part");
            return workOrderResponse;
        }

        private WorkOrderResponse Step5(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject, MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Add Work Order Activity");

            int i = 1;

            foreach (MobileWorkOrderActivity mobileWorkOrderActivity in mobileWorkOrder.MobileWorkOrderActivities)
            {
                logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Add Work Order Activity: " + mobileWorkOrderActivity.ActivityID + " - " + mobileWorkOrderActivity.Activity);

                logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Getting Unigue Key Pricing ID from JDE");
                int uniqueKeyPricingID = _oracleHelperService.GetNextSequenceValue(SequenceNumber.UniqueKeyPricing);
                logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Getting Unigue Key Pricing ID from JDE");

                if (uniqueKeyPricingID == 0)
                {
                    workOrderResponse.Success = false;
                    workOrderResponse.ReturnCode = 2;
                    workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage
                    {
                        Type = 2,
                        Message = "Could not create a new Unique Key Pricing ID on Oracle table F31171.WSUKIDP"
                    });

                    return workOrderResponse;
                }

                WorkOrderActivity workOrderActivity = new WorkOrderActivity();

                workOrderActivity = AddDefaultsToWorkOrderActivity(workOrderActivity,
                                                                   workOrderCommonObject);

                workOrderActivity.AddressNumber = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
                workOrderActivity.Branch = mobileWorkOrder.ReportLocationNumber.PadLeft(12);
                workOrderActivity.BusinessUnit = (mobileWorkOrderActivity.WorkType + mobileWorkOrder.ReportLocationNumber).PadLeft(12);
                workOrderActivity.CostComponent = mobileWorkOrderActivity.SalesCategoryCode4;
                workOrderActivity.Description = mobileWorkOrderActivity.Description;
                workOrderActivity.JobTypeCraftCode = mobileWorkOrderActivity.SecondItemNumber.Trim();
                workOrderActivity.Quantity = mobileWorkOrderActivity.Qty * 10;
                workOrderActivity.SequenceNumberOperations = i * 100;
                workOrderActivity.TypeOperationCode = mobileWorkOrderActivity.SalesCatalogSection.Trim();
                workOrderActivity.UnitofMeasureasInput = mobileWorkOrderActivity.UnitofMeasurePrimary;
                workOrderActivity.WorkcenterBranch = mobileWorkOrder.ReportLocationNumber.PadLeft(12);
                workOrderActivity.WorkOrderID = workOrderCommonObject.ID;
                workOrderActivity.WorkStationID = workOrderCommonObject.TabletID10;
                workOrderActivity.ParentshortItemNumber = workOrderCommonObject.ParentshortItemNumber;
                workOrderActivity.Kit2ndItemNumber = mobileWorkOrder.Kit2ndItemNumber;
                workOrderActivity.ThirdItemNumberKit = mobileWorkOrder.Kit2ndItemNumber;
                workOrderActivity.UserID = mobileWorkOrder.EmployeeNumber;
                workOrderActivity.UnitsQuantityatOperation = i == 1 ? 1 : 0;

                context.WorkOrderActivities.Add(workOrderActivity);

                RoutingExtension routingExtension = new RoutingExtension();

                routingExtension = AddDefaultsToRoutingExtension(routingExtension, workOrderCommonObject);

                routingExtension.WorkOrderID = workOrderCommonObject.ID;
                routingExtension.BusinessUnit = (mobileWorkOrderActivity.WorkType + mobileWorkOrder.ReportLocationNumber).PadLeft(12);
                routingExtension.TypeOperationCode = mobileWorkOrderActivity.SalesCatalogSection.Trim();
                routingExtension.SequenceNumberOperation = i * 100;
                routingExtension.BillableCurrency = workOrderCommonObject.CurrencyCode;
                routingExtension.CostComponent = mobileWorkOrderActivity.SalesCategoryCode4;
                routingExtension.CurrencyCodeTo = workOrderCommonObject.CurrencyCode;
                routingExtension.CategoryGLNonCovered = workOrderCommonObject.CatagoryGL;
                routingExtension.WorkStationID = workOrderCommonObject.TabletID10;
                routingExtension.PercentCovered = mobileWorkOrderActivity.DamageBillingPercentage * 100;
                routingExtension.ProgramID = "MWO";
                routingExtension.ProductFamily = workOrderCommonObject.ProductFamily;
                routingExtension.ProductModel = mobileWorkOrder.EquipmentClass;
                routingExtension.ServiceAddressNumber = Convert.ToInt32(mobileWorkOrder.ReportLocationNumber);
                routingExtension.TimeofDay = workOrderCommonObject.JDETime;
                routingExtension.UniqueKeyPricing = uniqueKeyPricingID;
                routingExtension.DateUpdated = workOrderCommonObject.JDEDate;
                routingExtension.UserID = mobileWorkOrder.EmployeeNumber;

                context.RoutingExtensions.Add(routingExtension);

                i++;
                logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Add Work Order Activity: " + mobileWorkOrderActivity.ActivityID + " - " + mobileWorkOrderActivity.Activity);
            }

            logger.Debug("Start - Context Save Changes");
            context.SaveChanges();
            logger.Debug("Done - Context Save Changes");
            workOrderResponse.Success = true;
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Add Work Order Activity");
            return workOrderResponse;
        }

        private WorkOrderResponse Step6(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject,
                                        MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Adding Work Order Master Tag");

            WorkOrderMasterTag workOrderMasterTag = new WorkOrderMasterTag();

            workOrderMasterTag = AddDefaultsToWorkOrderMasterTag(workOrderMasterTag, workOrderCommonObject);

            workOrderMasterTag.CategoryGLNonCovered = workOrderCommonObject.CatagoryGL;
            workOrderMasterTag.CreatedByUser = mobileWorkOrder.EmployeeNumber;
            workOrderMasterTag.CurrencyCodeFrom = workOrderCommonObject.CurrencyCode;
            workOrderMasterTag.CurrencyCodeTo = workOrderCommonObject.CurrencyCode;
            workOrderMasterTag.ProductFamily = workOrderCommonObject.ProductFamily;
            workOrderMasterTag.ProductModel = mobileWorkOrder.EquipmentClass;
            workOrderMasterTag.SerialNumber = mobileWorkOrder.SerialNumber;
            workOrderMasterTag.WorkOrderID = workOrderCommonObject.ID;

            context.WorkOrderMasterTags.Add(workOrderMasterTag);
            context.SaveChanges();

            workOrderResponse.Success = true;
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Adding Work Order Master Tag");
            return workOrderResponse;
        }

        private WorkOrderResponse Step7(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject,
                                        MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Adding Work Order Instruction");

            WorkOrderInstruction workOrderInstruction = new WorkOrderInstruction();

            workOrderInstruction.AssociatedWOItem1 = " ";
            workOrderInstruction.AssociatedWOItem2 = " ";
            workOrderInstruction.AssociatedWOItem3 = " ";
            workOrderInstruction.DateAssociatedSar = workOrderCommonObject.JDEDate;
            workOrderInstruction.DescriptionWorkOrder = string.IsNullOrEmpty(mobileWorkOrder.AdditionalWork) ? "WS Work Order" : "Additional Work";
            workOrderInstruction.OrderSuffix = " ";
            workOrderInstruction.OrderType = "WZ";
            workOrderInstruction.RecordType = "A";
            workOrderInstruction.WorkOrderID = workOrderCommonObject.ID;
            workOrderInstruction.WorkOrderLineNumber = 100;

            context.WorkOrderInstructions.Add(workOrderInstruction);
            context.SaveChanges();

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Adding Work Order Instruction");

            return workOrderResponse;
        }

        private WorkOrderResponse Step8(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject,
               MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Adding Work Order Status JDE Function");

            DateTime now = DateTime.Now;

            string dateValue = now.Year + "/" + now.Month + "/" + now.Day;


            JDEBusinessFunction jdeBusinessFunction = new JDEBusinessFunction
            {
                CallMethod = "F1307UpdateWorkOrderStatus",
                Parameters = new Dictionary<string, string>
                        {
                            {"cRecordType", "1"},
                            {"mnWorkOrderNumber", workOrderCommonObject.ID.ToString()},
                            {"szNewStatusOfWorkOrder", "06"},
                            {"jdBeginDateOfStatus",dateValue},
                            {"mnBeginTimeOfStatus", workOrderCommonObject.JDETime.ToString()},
                            {"mnAssetNumber", mobileWorkOrder.AssetID.ToString()},
                            {"szProgramId", mobileWorkOrder.ReportLocationNumber},
                            {"szRemark", ""},
                            {"cProcessingMode", "3"},
                            {"cErrorCode", ""}
                        }
            };

            JDEBusinessFunctionResponse jdeBusinessFunctionResponse = _jdeService.CallJDEBusinessFunction(jdeBusinessFunction);

            if (jdeBusinessFunctionResponse.ReturnCode != 0)
            {
                workOrderResponse.ReturnCode = 2;
                workOrderResponse.Success = false;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = "There was an error writing history.  Please contact Test Support" });
                return workOrderResponse;
            }

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Adding Work Order Status JDE Function");

            return workOrderResponse;
        }


        private WorkOrderResponse Step9(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject,
                               MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Adding MWO Feith Images to Database");

            int imageTypeID = 0;
            decimal workOrderID = 0;

            switch (mobileWorkOrder.Type)
            {
                case "Inbound":
                    imageTypeID = 1;
                    workOrderID = workOrderCommonObject.ID;
                    break;
                case "Outbound":
                    imageTypeID = 2;
                    workOrderID = GetTemporaryWorkOrderNumber(mobileWorkOrder, context);
                    break;
            }



            foreach (MobileWorkOrderImage mobileWorkOrderImage in mobileWorkOrder.MobileWorkOrderImages)
            {
                MWOFeithImage mwoFeithImage = new MWOFeithImage
                    {
                        WorkOrderGUID = mobileWorkOrder.ID,
                        ImageTypeID = imageTypeID,
                        WorkOrderID = workOrderID,
                        MWOImageID = mobileWorkOrderImage.ID,
                        EmployeeNumber = mobileWorkOrder.EmployeeNumber,
                        UnitNumber = mobileWorkOrder.UnitNumber,
                        ComplexUnitNumber = mobileWorkOrder.ComplexUnitNumber,
                        OMBOrderInbound = mobileWorkOrder.OMBOrderInbound,
                        Location = mobileWorkOrderImage.Location,
                        Damaged = mobileWorkOrderImage.Damaged,
                        DateAdded = DateTime.Now
                    };

                context.MWOFeithImages.Add(mwoFeithImage);
            }

            context.SaveChanges();

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Done - Adding MWO Feith Images to Database");

            return workOrderResponse;
        }

        private WorkOrderResponse Step10(WorkOrderResponse workOrderResponse, WorkOrderCommonObject workOrderCommonObject,
                               MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Running JDE Business Function");

            JDEBusinessFunction jdeBusinessFunction = new JDEBusinessFunction
            {
                CallMethod = "CallWOFunction",
                Parameters = new Dictionary<string, string>
                        {
                            {"mnDocumentOrderInvoiceE", workOrderCommonObject.ID.ToString()},
                            {"cCalledFromaTablet", "Y"},
                            {"cReturnCode", ""},
                            {"mnErrorIndicator", ""},
                            {"szPcErrorMessage", ""},
                            {"mnAssetItemNumber", mobileWorkOrder.AssetID.ToString()},
                            {"mnAmountOriginalDollars", ""},
                            {"cTypeWo", mobileWorkOrder.UserDefinedCodeID},
                        }
            };

            JDEBusinessFunctionResponse jdeBusinessFunctionResponse = _jdeService.CallJDEBusinessFunction(jdeBusinessFunction);

            if (jdeBusinessFunctionResponse.ReturnCode != 0)
            {
                workOrderResponse.ReturnCode = 4;
                workOrderResponse.Success = false;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = "There was an error calling the JDE Business Function.  Please contact Test Support" });
                return workOrderResponse;
            }

            if (jdeBusinessFunctionResponse.Parameters["cReturnCode"] == "1")
            {
                workOrderResponse.ReturnCode = 4;
                workOrderResponse.Success = false;
                workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = jdeBusinessFunctionResponse.Parameters["szPcErrorMessage"] });
                return workOrderResponse;
            }

            logger.Debug("Unit Number: " + mobileWorkOrder.UnitNumber + " Start - Running JDE Business Function");

            return workOrderResponse;
        }



        private decimal GetTemporaryWorkOrderNumber(MobileWorkOrder mobileWorkOrder, WSJDE context)
        {

            decimal temporaryWorkOrderNumberID = 0;

                TemporaryWorkOrderNumber temporaryWorkOrderNumber = new TemporaryWorkOrderNumber
                    {
                        WorkOrderGUID = mobileWorkOrder.ID,
                        TimeStamp = DateTime.Now
                    };

                context.TemporaryWorkOrderNumbers.Add(temporaryWorkOrderNumber);
                context.SaveChanges();

                temporaryWorkOrderNumberID = temporaryWorkOrderNumber.ID;

            return temporaryWorkOrderNumberID;
        }

        private WorkOrderResponse UpdateAttributes(WorkOrderResponse workOrderResponse,
                                                   WorkOrderCommonObject workOrderCommonObject,
                                                   MobileWorkOrder mobileWorkOrder, WSJDE context)
        {
            if (mobileWorkOrder.IsUnitAttributeChanged == 1)
            {

                var existingFleetAttributes = (from fa in context.FleetAttributes
                                               where fa.ID == mobileWorkOrder.AssetID && fa.LookupKey != "   " 
                                               select fa);
                foreach (var existingFleetAttribute in existingFleetAttributes)
                {

                    context.FleetAttributes.Remove(existingFleetAttribute);
                    context.SaveChanges();
                }

                foreach (var attribute in mobileWorkOrder.UnitAttributes)
                {

                    UserDefinedCode userDefinedCode = (from udc in context.UserDefinedCodes
                                                       where udc.ProductCode == attribute.ProductCode
                                                 && udc.UserDefinedCodeTypeID == attribute.UserDefinedCodeTypeID
                                                 && udc.UserDefinedCodeID.Trim() == attribute.UserDefinedCodeID
                                           select udc).FirstOrDefault();
                    if (userDefinedCode != null)
                    {

                        FleetAttribute fleetAttribute = new FleetAttribute
                            {
                                ID = mobileWorkOrder.AssetID,
                                SystemKey = userDefinedCode.ProductCode,
                                RT = userDefinedCode.UserDefinedCodeTypeID,
                                LookupKey = userDefinedCode.UserDefinedCodeID,
                                Description = attribute.FleetAttributeDescription,
                                UpdateTime = workOrderCommonObject.JDETime,
                                UpdateDate = workOrderCommonObject.JDEDate,
                                UpdateUser = mobileWorkOrder.EmployeeNumber,
                                UpdatePID = mobileWorkOrder.EmployeeNumber
                            };

                        context.FleetAttributes.Add(fleetAttribute);
                    }
                    else
                    {
                        workOrderResponse.ReturnCode = 2;
                        workOrderResponse.Success = false;
                        workOrderResponse.WorkOrderMessages.Add(new WorkOrderMessage { Type = 2, Message = "Unable to find a matching Attribute in the User Defined Code table" });
                        return workOrderResponse;
                        
                    }
                }

                context.SaveChanges();
            }

            return workOrderResponse;
        }

        private string CheckUnitNumberFormatting(string unitNumber)
        {
            string[] unitNumberParts = unitNumber.Split(Convert.ToChar("-"));

            string firstPart = unitNumberParts[0].PadRight(3, Convert.ToChar(" "));

            unitNumber = firstPart + "-" + unitNumberParts[1];

            return unitNumber;
        }

        private int GetItemIDByKit2ndItemNumber(string kit2ndItemNumber, WSJDE context)
        {
            int itemID = (from i in context.Items
                          where i.SecondItemNumber == kit2ndItemNumber
                          select i.ID).FirstOrDefault();
            return itemID;

        }

        private string GetCurrencyCodeByCompany(string company)
        {
            if (company == null)
            {
                return "";
            }
            else
            {
                switch (company.Trim())
                {
                    case "00010":
                        return "USD";
                    case "00020":
                        return "CAD";
                    default:
                        return "";
                }
            }
        }

        public int GetCountOfGetReadyWorkorders(string unitNumber, WSJDE context)
        {

            List<string> woStatuss = new List<string> { "05", "06", "08", "10", "45" };

            logger.Debug("Start - countOfGetReadyWorkorders");
            int countOfGetReadyWorkorders = (from wo in context.WorkOrders
                                             where wo.TypeWO.Trim() == "G"
                                                   && woStatuss.Contains(wo.StatusCodeWO.Trim())
                                                   && wo.UnitorTagNumber.Trim() == DbFunctions.AsNonUnicode(unitNumber)
                                             select wo).Count();
            logger.Debug("End - countOfGetReadyWorkorders");
            return countOfGetReadyWorkorders;

        }

        public int GetCountOfMajorRefurbWorkorders(string unitNumber, WSJDE context)
        {
            List<string> woStatuss = new List<string> { "05", "06", "08", "10", "45" };

            logger.Debug("Start - countOfGetReadyWorkorders");
            int countOfGetReadyWorkorders = (from wo in context.WorkOrders
                                             where wo.TypeWO.Trim() == "N"
                                                   && woStatuss.Contains(wo.StatusCodeWO.Trim())
                                                   && wo.UnitorTagNumber.Trim() == DbFunctions.AsNonUnicode(unitNumber)
                                             select wo).Count();
            logger.Debug("End - countOfGetReadyWorkorders");
            return countOfGetReadyWorkorders;
        }

        public string GetUnitConditionCode(string unitNumber, WSJDE context)
        {
            logger.Debug("Start - unitConditionCode");
            string unitConditionCode = (from a in context.Assets
                                        where a.UnitNumber.Trim() == DbFunctions.AsNonUnicode(unitNumber)
                                        select a.CategoryCodeFA11.Trim()).FirstOrDefault();
            logger.Debug("End - unitConditionCode");
            return unitConditionCode;
        }

        private WorkOrder AddDefaultsToWorkOrder(WorkOrder workOrder, WorkOrderCommonObject workOrderCommonObject)
        {
            workOrder.ActualDowntimeHours = 0;
            workOrder.AddressNumberAssignedTo = 0;
            workOrder.AddressNumberInspector = 0;
            workOrder.AddressNumberManager = 0;
            workOrder.Aisle = " ";
            workOrder.AmountActual = 0;
            workOrder.AmountActualLabor = 0;
            workOrder.AmountActualMaterial = 0;
            workOrder.AmountChangetoOriginalDollars = 0;
            workOrder.AmountEstimated = 0;
            workOrder.AmountEstimatedLabor = 0;
            workOrder.AmountEstimatedMaterial = 0;
            workOrder.AmountEstimatedOther = 0;
            workOrder.AmountMemoBudgetChanges = 0;
            workOrder.AmountMilesorHours = 0;
            workOrder.ApprovalType = " ";
            workOrder.BillofMaterialN = " ";
            workOrder.BillRevisionLevel = " ";
            workOrder.Bin = " ";
            workOrder.BOMChange = " ";
            workOrder.CategoriesWorkOrder01 = " ";
            workOrder.CategoriesWorkOrder02 = " ";
            workOrder.CategoriesWorkOrder03 = " ";
            workOrder.CategoriesWorkOrder04 = " ";
            workOrder.CategoriesWorkOrder05 = " ";
            workOrder.CategoriesWorkOrder10 = " ";
            workOrder.CompanyKeyRelatedOrder = " ";
            workOrder.CrewSize = 0;
            workOrder.CriticalityWorkOrder = 0;
            workOrder.DateAssignedto = 0;
            workOrder.DateAssignedtoInspector = 0;
            workOrder.DateCompletion = 0;
            workOrder.DateOrderTransaction = workOrderCommonObject.JDEDate;
            workOrder.DatePaperPrintedDate = 0;
            workOrder.DateRequested = workOrderCommonObject.JDEDate;
            workOrder.DateScheduledTickler = 0;
            workOrder.DateStart = workOrderCommonObject.JDEDate;
            workOrder.DateStatusChanged = 0;
            workOrder.DateUpdated = workOrderCommonObject.JDEDate;
            workOrder.DateWOPlannedCompleted = 0;
            workOrder.DocumentType = " ";
            workOrder.DrawingChange = " ";
            workOrder.E1ConsultingExperienceLevel = " ";
            workOrder.EnterpriseOneConsultingServiceType = " ";
            workOrder.EnterpriseOneConsultingSkillType = " ";
            workOrder.EnterpriseOneConsultingStatus = " ";
            workOrder.EstimatedDowntimeHours = 0;
            workOrder.ExistingDisposition = " ";
            workOrder.HoursActual = 0;
            workOrder.HoursChangetoOriginalHours = 0;
            workOrder.HoursEstimated = 0;
            workOrder.HoursUnaccountedDirectLabor = 0;
            workOrder.IndentedCode = " ";
            workOrder.LeadtimeCumulative = 0;
            workOrder.LeadtimeLevel = 0;
            workOrder.LineNumber = 0;
            workOrder.Location = " ";
            workOrder.LotGrade = " ";
            workOrder.LotPotency = 0;
            workOrder.LotSerialNumber = " ";
            workOrder.MessageNumber = " ";
            workOrder.MeterPosition = 0;
            workOrder.NewPartNumber = " ";
            workOrder.NextStatusWO = "45";
            workOrder.OrderSuffix = " ";
            workOrder.OrderType = "WZ";
            workOrder.ParentNumber = 0;
            workOrder.PayDeductionBenefitType = " ";
            workOrder.PegtoWorkOrder = 0;
            workOrder.PercentComplete = 100;
            workOrder.PhaseIn = " ";
            workOrder.PostingEdit = " ";
            workOrder.PriorityWO = " ";
            workOrder.ProgramID = "MWO";
            workOrder.QuantityShipped = 0;
            workOrder.RatioCriticalRatioPriority1 = 0;
            workOrder.RatioCriticalRatioPriority2 = 0;
            workOrder.ReasonforECO = " ";
            workOrder.Reference2 = " ";
            workOrder.RelatedPOSOWONumber = " ";
            workOrder.RelatedPOSOWOOrderType = " ";
            workOrder.ResequenceCode = 0;
            workOrder.RevenueRateMarkupOverride = 0;
            workOrder.RouteSheetN = " ";
            workOrder.RoutingChange = " ";
            workOrder.RoutingRevisionLevel = " ";
            workOrder.StatusCodeWO = "06";
            workOrder.StatusComment = " ";
            workOrder.SubledgerInactiveCode = " ";
            workOrder.Subsidiary = " ";
            workOrder.TimeBeginning = 0;
            workOrder.TimeofDay = workOrderCommonObject.JDETime;
            workOrder.TypeBillofMaterial = "M";
            workOrder.TypeofRouting = "M";
            workOrder.UnitofMeasureasInput = "EA";
            workOrder.UnitsOrderTransactionQuantity = 1;
            workOrder.UnitsQtyBackorderedHeld = 0;
            workOrder.UnitsQuantityCanceledScrapped = 0;
            workOrder.UnitsShippedtoDate = 0;
            workOrder.UserReservedAmount = 0;
            workOrder.UserReservedCode = " ";
            workOrder.UserReservedDate = 0;
            workOrder.UserReservedNumber = 0;
            workOrder.UserReservedReference = " ";
            workOrder.VarianceFlag = " ";
            workOrder.WOPickListPrinted = " ";
            workOrder.WorkOrderFlashMessage = " ";
            workOrder.WorkOrderFreezeCode = "Y";

            return workOrder;
        }

        private WorkOrderPart AddDefaultsToWorkOrderPart(WorkOrderPart workOrderPart,
                                                         WorkOrderCommonObject workOrderCommonObject)
        {
            workOrderPart.ActiveIngredientFlag = "0";
            workOrderPart.AmountEstimatedCost = 0;
            workOrderPart.AmountUnaccountedDirectLabor = 0;
            workOrderPart.AmountUnaccountedScrap = 0;
            workOrderPart.BubbleSequenceAlphaNumeric = " ";
            workOrderPart.CommittedHS = "N";
            workOrderPart.CompanyKeyRelatedOrder = " ";
            workOrderPart.ComponentLineNumber = 0;
            workOrderPart.ComponentRevisionLevel = " ";
            workOrderPart.ComponentType = " ";
            workOrderPart.ConstraintsFlag = " ";
            workOrderPart.CoProductsByProductsIntermediate = " ";
            workOrderPart.CostComponent = " ";
            workOrderPart.CriticalHoldPropagation = " ";
            workOrderPart.DateCompletionJulian = 0;
            workOrderPart.DateOrderTransaction = workOrderCommonObject.JDEDate;
            workOrderPart.DateRequested = workOrderCommonObject.JDEDate;
            workOrderPart.DateUpdated = workOrderCommonObject.JDEDate;
            workOrderPart.DescriptionLine2 = " ";
            workOrderPart.DocumentType = " ";
            workOrderPart.FixedorVariableQuantity = "V";
            workOrderPart.FromGrade = " ";
            workOrderPart.FromPotency = 0;
            workOrderPart.GLDate = 0;
            workOrderPart.HoursUnaccountedDirectLabor = 0;
            workOrderPart.IssueandReceipt = " ";
            workOrderPart.IssueTypeCode = "I";
            workOrderPart.LeadtimeOffsetDays = 0;
            workOrderPart.LineType = "BW";
            workOrderPart.Location = " ";
            workOrderPart.LotEffectivityDate = 0;
            workOrderPart.LotSerialNumber = " ";
            workOrderPart.MaterialStatusCodeWO = " ";
            workOrderPart.MessageNumber = " ";
            workOrderPart.ObjectAccount = " ";
            workOrderPart.OperationScrapPercent = 0;
            workOrderPart.OrderSuffix = " ";
            workOrderPart.OrderType = "WZ";
            workOrderPart.PartsListSubstituteFlag = " ";
            workOrderPart.PercentAsis = 0;
            workOrderPart.PercentCumulativePlannedYield = 0;
            workOrderPart.PercentPercentofScrap = 0;
            workOrderPart.PercentRework = 0;
            workOrderPart.PickSlipNumber = 0;
            workOrderPart.PrimaryLastSupplierNumber = 0;
            workOrderPart.ProgramID = "MWO";
            workOrderPart.ProjectBusinessUnit = " ";
            workOrderPart.QuantityAvailable = 0;
            workOrderPart.RelatedPOSOLineNumber = 0;
            workOrderPart.RelatedPOSOWONumber = " ";
            workOrderPart.RelatedPOSOWOOrderType = " ";
            workOrderPart.ResourcePercent = 0;
            workOrderPart.RouteSheetN = " ";
            workOrderPart.SecondaryQuantityIssuedCompleted = 0;
            workOrderPart.SequenceBubbleSequence = 0;
            workOrderPart.SequenceNumberOperations = 100;
            workOrderPart.SerialNumberLot = " ";
            workOrderPart.Subsidiary = " ";
            workOrderPart.ThruGrade = " ";
            workOrderPart.ThruPotency = 0;
            workOrderPart.TimeofDay = workOrderCommonObject.JDETime;
            workOrderPart.TypeBillofMaterial = "M";
            workOrderPart.UnitofMeasureSecondary = " ";
            workOrderPart.UnitsQtyBackorderedHeld = 0;
            workOrderPart.UnitsQuantityCanceledScrapped = 0;
            workOrderPart.UnitsQuantityCommited = 0;
            workOrderPart.UnitsUnaccountedScrap = 0;
            workOrderPart.UserReservedAmount = 0;
            workOrderPart.UserReservedCode = " ";
            workOrderPart.UserReservedDate = 0;
            workOrderPart.UserReservedNumber = 0;
            workOrderPart.UserReservedReference = " ";

            return workOrderPart;
        }

        private PartExtension AddDefaultsToPartExtension(PartExtension partExtension,
                                                         WorkOrderCommonObject workOrderCommonObject)
        {
            partExtension.ActualUnitBillable = 0;
            partExtension.ActualUnitCost = 0;
            partExtension.ActualUnitPayable = 0;
            partExtension.AmountActual = 0;
            partExtension.AmountEstimated = 0;
            partExtension.PriceandAdjustmentScheduleService = " ";
            partExtension.PriceandAdjustmentScheduleFour = " ";
            partExtension.BillableExchangeRate = 0;
            partExtension.BillableCurrencyMode = "D";
            partExtension.BillableYN = "1";
            partExtension.ClaimAmount = 0;
            partExtension.CostComponent = " ";
            partExtension.CoverageGroup = " ";
            partExtension.CurrencyCodeTo = " ";
            partExtension.CausalPart = 0;
            partExtension.DateBilled = 0;
            partExtension.DatePaid = 0;
            partExtension.DateSubmitted = 0;
            partExtension.EstimatedAmount = 0;
            partExtension.EntitlementCheck = "0";
            partExtension.EstimatedPaymentAmount = 0;
            partExtension.EstimatedUnitBillable = 0;
            partExtension.EstimatedUnitCost = 0;
            partExtension.EstimatedUnitPayable = 0;
            partExtension.ForeignActualBillableUnitRate = 0;
            partExtension.ForeignActualPayableUnitRate = 0;
            partExtension.DefectCode = " ";
            partExtension.ForeignEstimatedBillableAmount = 0;
            partExtension.ForeignEstimatedPayableAmount = 0;
            partExtension.ForeignEstimatedBillableUnitRate = 0;
            partExtension.ForeignEstimatedPayableUnitRate = 0;
            partExtension.ForeignActualPayableAmount = 0;
            partExtension.ForeignActualBillableAmount = 0;
            partExtension.CategoryGLCovered = " ";
            partExtension.CategoryGLNonCovered = " ";
            partExtension.WorkStationID = " ";
            partExtension.CausalPartBusinessUnit = " ";
            partExtension.MethodofPricing = "T";
            partExtension.PaymentAmount = 0;
            partExtension.PayableYN = "0";
            partExtension.PercentCovered = 0;
            partExtension.PayableCurrency = " ";
            partExtension.PayableExchangeRate = 0;
            partExtension.PayableCurrencyMode = "F";
            partExtension.ProgramID = " ";
            partExtension.PayPricingMethod = " ";
            partExtension.ProductFamily = " ";
            partExtension.ProductModel = " ";
            partExtension.PayServiceProviderforParts = " ";
            partExtension.ReturnPolicy = " ";
            partExtension.SupplierLotNumber = " ";
            partExtension.ServiceAddressNumber = 0;
            partExtension.SupplierRecoveryVendorNumber = 0;
            partExtension.SalesTaxableYN = " ";
            partExtension.TimeofDay = 0;
            partExtension.TimeSubmitted = 0;
            partExtension.TotalBilled = 0;
            partExtension.UniqueKeyPricing = 0;
            partExtension.DateUpdated = 0;
            partExtension.UserID = " ";
            partExtension.PrimaryLastSupplierNumber = 0;
            partExtension.ComponentCodeSystem = " ";
            partExtension.ComponentCodeAssembly = " ";
            partExtension.ComponentCodePart = " ";
            partExtension.Reference2 = " ";

            return partExtension;
        }

        private WorkOrderActivity AddDefaultsToWorkOrderActivity(WorkOrderActivity workOrderActivity,
                                                                 WorkOrderCommonObject workOrderCommonObject)
        {
            workOrderActivity.ActivityBasedCostingActivityCode = " ";
            workOrderActivity.ActivityHours = 0;
            workOrderActivity.AddressNumberAssignedTo = 0;
            workOrderActivity.AddressNumberManager = 0;
            workOrderActivity.AmountUnaccountedDirectLabor = 0;
            workOrderActivity.AmountUnaccountedMachine = 0;
            workOrderActivity.AmountUnaccountedScrap = 0;
            workOrderActivity.AmountUnaccountedSetupLabor = 0;
            workOrderActivity.AssetItemNumber = 0;
            workOrderActivity.AutoLoadDescription = " ";
            workOrderActivity.CapacityUnitofmeasure = "HR";
            workOrderActivity.CompanyKeyRelatedOrder = " ";
            workOrderActivity.CompetencyCode = " ";
            workOrderActivity.CompetencyLevelFrom = 0;
            workOrderActivity.CompetencyLevelTo = 0;
            workOrderActivity.CompetencyType = " ";
            workOrderActivity.ConstraintsFlag = " ";
            workOrderActivity.CriticalRatio = 0;
            workOrderActivity.DateActualStart = 0;
            workOrderActivity.DateCompletionJulian = 0;
            workOrderActivity.DateOrderTransaction = workOrderCommonObject.JDEDate;
            workOrderActivity.DateRequested = 0;
            workOrderActivity.DateStartJulian = 0;
            workOrderActivity.DateUpdated = workOrderCommonObject.JDEDate;
            workOrderActivity.DocumentType = " ";
            workOrderActivity.FactorOperationShrinkageFactor = 0;
            workOrderActivity.HoursSetupLaborHoursActual = 0;
            workOrderActivity.HoursUnaccountedDirectLabor = 0;
            workOrderActivity.HoursUnaccountedMachineHours = 0;
            workOrderActivity.HoursUnaccountedSetupLabor = 0;
            workOrderActivity.InspectionCode = " ";
            workOrderActivity.LabororMachine = " ";
            workOrderActivity.LineCellIdentifier = " ";
            workOrderActivity.Location = " ";
            workOrderActivity.MaintenanceScheduleFlag = " ";
            workOrderActivity.MessageNumber = " ";
            workOrderActivity.MoveHoursStandard = 0;
            workOrderActivity.ObjectAccount = " ";
            workOrderActivity.OperationStatusCodeWO = " ";
            workOrderActivity.OrderSuffix = " ";
            workOrderActivity.OrderType = "WZ";
            workOrderActivity.PayPointCode = "0";
            workOrderActivity.PayPointStatus = " ";
            workOrderActivity.PercentCumulativePlannedYield = 10000;
            workOrderActivity.PercentLeadtimeOverlap = 0;
            workOrderActivity.PercentOperationalPlannedYield = 10000;
            workOrderActivity.PercentOverlap = 0;
            workOrderActivity.PrimaryLastSupplierNumber = 0;
            workOrderActivity.ProgramID = "MWO";
            workOrderActivity.ProjectBusinessUnit = " ";
            workOrderActivity.QuantityShipped = 0;
            workOrderActivity.QueueHoursStandard = 0;
            workOrderActivity.RatePiecework = 0;
            workOrderActivity.RelatedPOSOLineNumber = 0;
            workOrderActivity.RelatedPOSOWONumber = " ";
            workOrderActivity.RelatedPOSOWOOrderType = " ";
            workOrderActivity.ResourcesAssigned = " ";
            workOrderActivity.ResourceUnitsConsumed = 0;
            workOrderActivity.RuleMatchFlag = "1";
            workOrderActivity.RunLaborActual = 0;
            workOrderActivity.RunLaborCurrent = 0;
            workOrderActivity.RunLaborRate = 0;
            workOrderActivity.RunMachineActual = 0;
            workOrderActivity.RunMachineCurrent = 0;
            workOrderActivity.RunMachineRate = 0;
            workOrderActivity.RunMachineStandard = 0;
            workOrderActivity.SequenceNumberNextOperation = 0;
            workOrderActivity.SetupLaborCurrent = 0;
            workOrderActivity.SetupLaborRate = 0;
            workOrderActivity.SetupLaborStandard = 0;
            workOrderActivity.ShiftCodeCompleted = " ";
            workOrderActivity.ShiftCodeRequested = " ";
            workOrderActivity.ShiftCodeStart = " ";
            workOrderActivity.SlackTimeRatio = 0;
            workOrderActivity.Subsidiary = " ";
            workOrderActivity.Supervisor = 0;
            workOrderActivity.TimeBasisCode = "U";
            workOrderActivity.TimeCompleted = 0;
            workOrderActivity.TimeofDay = workOrderCommonObject.JDETime;
            workOrderActivity.TimeScheduledEndHHMMSS = 0;
            workOrderActivity.TimeScheduledStartHHMMSS = 0;
            workOrderActivity.TimeStartHHMMSS = 0;
            workOrderActivity.TypeofRouting = "M";
            workOrderActivity.UnitorTagNumber = " ";
            workOrderActivity.UnitsOrderTransactionQuantity = 1;
            workOrderActivity.UnitsQuantityCanceledScrapped = 0;
            workOrderActivity.UnitsUnaccountedScrap = 0;
            workOrderActivity.UserReservedAmount = 0;
            workOrderActivity.UserReservedCode = " ";
            workOrderActivity.UserReservedDate = 0;
            workOrderActivity.UserReservedNumber = 0;
            workOrderActivity.UserReservedReference = " ";


            return workOrderActivity;
        }

        private RoutingExtension AddDefaultsToRoutingExtension(RoutingExtension routingExtension,
                                                               WorkOrderCommonObject workOrderCommonObject)
        {
            routingExtension.ActualBillableRate = 0;
            routingExtension.ActualPayableRate = 0;
            routingExtension.AmountActual = 0;
            routingExtension.AmountEstimated = 0;
            routingExtension.PriceandAdjustmentScheduleService = " ";
            routingExtension.PriceandAdjustmentScheduleFour = " ";
            routingExtension.BillableExchangeRate = 0;
            routingExtension.BillableCurrencyMode = "D";
            routingExtension.BillableYN = "1";
            routingExtension.ClaimAmount = 0;
            routingExtension.CostComponent = " ";
            routingExtension.CoverageGroup = " ";
            routingExtension.CurrencyCodeTo = " ";
            routingExtension.CausalPart = 0;
            routingExtension.DateActualStart = 0;
            routingExtension.DateBilled = 0;
            routingExtension.DatePaid = 0;
            routingExtension.DateSubmitted = 0;
            routingExtension.EstimatedAmount = 0;
            routingExtension.EntitlementCheck = "0";
            routingExtension.EstimatedPaymentAmount = 0;
            routingExtension.EstimateTimeEndingHHMMSS = 0;
            routingExtension.EstimatedBillableRate = 0;
            routingExtension.EstimatedPayableRate = 0;
            routingExtension.ForeignActualBillableUnitRate = 0;
            routingExtension.ForeignActualPayableUnitRate = 0;
            routingExtension.DefectCode = " ";
            routingExtension.ForeignEstimatedBillableAmount = 0;
            routingExtension.ForeignEstimatedPayableAmount = 0;
            routingExtension.ForeignEstimatedBillableUnitRate = 0;
            routingExtension.ForeignEstimatedPayableUnitRate = 0;
            routingExtension.ForeignActualPayableAmount = 0;
            routingExtension.ForeignActualBillableAmount = 0;
            routingExtension.CategoryGLCovered = " ";
            routingExtension.InvoiceReceived = " ";
            routingExtension.InvoiceRequired = " ";
            routingExtension.RateLaborPerHour = 0;
            routingExtension.SourceforLaborLine = " ";
            routingExtension.CausalPartBusinessUnit = " ";
            routingExtension.MethodofPricing = "T";
            routingExtension.PaymentAmount = 0;
            routingExtension.PayableYN = "0";
            routingExtension.PayableCurrency = " ";
            routingExtension.PayableExchangeRate = 0;
            routingExtension.PayableCurrencyMode = " ";
            routingExtension.PayPricingMethod = " ";
            routingExtension.PayServiceProviderforParts = " ";
            routingExtension.SupplierLotNumber = " ";
            routingExtension.SupplierRecoveryVendorNumber = 0;
            routingExtension.TimeStartHHMMSS = 0;
            routingExtension.TimeSubmitted = 0;
            routingExtension.TotalBilled = 0;
            routingExtension.ComponentCodeSystem = " ";
            routingExtension.ComponentCodeAssembly = " ";
            routingExtension.ComponentCodePart = " ";

            return routingExtension;
        }

        private WorkOrderMasterTag AddDefaultsToWorkOrderMasterTag(WorkOrderMasterTag workOrderMasterTag,
                                                                   WorkOrderCommonObject workOrderCommonObject)
        {
            workOrderMasterTag.AccountID = " ";
            workOrderMasterTag.ActualSpecialAmount = 0;
            workOrderMasterTag.ActualSpecialUnits = 0;
            workOrderMasterTag.AlertSensitiveTask = " ";
            workOrderMasterTag.AmountActualOther = 0;
            workOrderMasterTag.AmountCommittedOriginal = 0;
            workOrderMasterTag.AmountCommittedRemaining = 0;
            workOrderMasterTag.AmountUnaccountedDirectLabor = 0;
            workOrderMasterTag.AmountUnaccountedScrap = 0;
            workOrderMasterTag.AssessorNumber = 0;
            workOrderMasterTag.BudgetedCost = 0;
            workOrderMasterTag.CallBacks = " ";
            workOrderMasterTag.CategoriesWorkOrder11 = " ";
            workOrderMasterTag.CategoriesWorkOrder12 = " ";
            workOrderMasterTag.CategoriesWorkOrder13 = " ";
            workOrderMasterTag.CategoriesWorkOrder14 = " ";
            workOrderMasterTag.CategoriesWorkOrder15 = " ";
            workOrderMasterTag.CategoriesWorkOrder16 = " ";
            workOrderMasterTag.CategoriesWorkOrder17 = " ";
            workOrderMasterTag.CategoriesWorkOrder18 = " ";
            workOrderMasterTag.CategoriesWorkOrder19 = " ";
            workOrderMasterTag.CategoriesWorkOrder20 = " ";
            workOrderMasterTag.CategoryGLCovered = " ";
            workOrderMasterTag.CausalPart = 0;
            workOrderMasterTag.CausalPartBusinessUnit = " ";
            workOrderMasterTag.ChangestoPartsAndRoutingsAllowed = " ";
            workOrderMasterTag.ChargeCosttoProjectAccount = " ";
            workOrderMasterTag.ComponentCodeAssembly = " ";
            workOrderMasterTag.ComponentCodePart = " ";
            workOrderMasterTag.ComponentCodeSystem = " ";
            workOrderMasterTag.ContactName2 = "WILLIAMS SCOTSMAN";
            workOrderMasterTag.ContractChangeNumber = " ";
            workOrderMasterTag.CostMethod = " ";
            workOrderMasterTag.Country = " ";
            workOrderMasterTag.CoverageGroup = " ";
            workOrderMasterTag.CurrencyCodeOrigin = " ";
            workOrderMasterTag.CurrencyCodeSource = " ";
            workOrderMasterTag.CurrencyConversionRateSpotRate = 0;
            workOrderMasterTag.CurrencyModeForeignorDomesticEntry = "D";
            workOrderMasterTag.CurrentBalanceofMeter1 = 0;
            workOrderMasterTag.CurrentBalanceofMeter2 = 0;
            workOrderMasterTag.CurrentBalanceofMeter3 = 0;
            workOrderMasterTag.DateActualStartJulian = 0;
            workOrderMasterTag.DateOther5 = 0;
            workOrderMasterTag.DateOther6 = 0;
            workOrderMasterTag.DaylightSavingsRuleName = " ";
            workOrderMasterTag.DefectCode = " ";
            workOrderMasterTag.DirectProjectRelationship = " ";
            workOrderMasterTag.DocumentCompany = " ";
            workOrderMasterTag.DocumentNumber = 0;
            workOrderMasterTag.DocumentType = " ";
            workOrderMasterTag.DrawingNumber = " ";
            workOrderMasterTag.DualUnitOfMeasureItem = " ";
            workOrderMasterTag.EligibleSupplierRecovery = " ";
            workOrderMasterTag.EntitlementCheck = "0";
            workOrderMasterTag.EntitlementCheckSupplier = " ";
            workOrderMasterTag.EstimatedSpecialAmount = 0;
            workOrderMasterTag.EstimatedSpecialUnits = 0;
            workOrderMasterTag.ExternalEventFlag = " ";
            workOrderMasterTag.FailureDate = 0;
            workOrderMasterTag.FailureTime = 0;
            workOrderMasterTag.FirmWorkOrder = " ";
            workOrderMasterTag.GeographicRegionCode = " ";
            workOrderMasterTag.HeldPartsListFlag = " ";
            workOrderMasterTag.IssueNumber = 0;
            workOrderMasterTag.JobTypeCraftCode = " ";
            workOrderMasterTag.Language = " ";
            workOrderMasterTag.LeadCraft = " ";
            workOrderMasterTag.LineCellIdentifier = " ";
            workOrderMasterTag.LineNumber = 0;
            workOrderMasterTag.MaintenanceScheduleFlag = " ";
            workOrderMasterTag.MethodofPricing = "F";
            workOrderMasterTag.MonthWeekDayShift = " ";
            workOrderMasterTag.ObjectAccount = " ";
            workOrderMasterTag.PaymentInstrument = " ";
            workOrderMasterTag.PhoneNumber = "999-999-9999";
            workOrderMasterTag.PhonePrefix = " ";
            workOrderMasterTag.PlannedLabor = 0;
            workOrderMasterTag.PlannedMaterial = 0;
            workOrderMasterTag.PlannedOther = 0;
            workOrderMasterTag.PlannedSpecialAmount = 0;
            workOrderMasterTag.PlannedSpecialUnits = 0;
            workOrderMasterTag.PriceandAdjustmentScheduleFour = " ";
            workOrderMasterTag.PriceandAdjustmentScheduleService = " ";
            workOrderMasterTag.PrimaryLastSupplierNumber = 0;
            workOrderMasterTag.ProblemDescription = "WS WORK ORDER";
            workOrderMasterTag.ProductionCostsExist = " ";
            workOrderMasterTag.ProjectNumber = 0;
            workOrderMasterTag.PromotionID = " ";
            workOrderMasterTag.ReimbursementMethod = " ";
            workOrderMasterTag.RepairDate = 0;
            workOrderMasterTag.RepairTime = 0;
            workOrderMasterTag.ResourcesAssigned = "0";
            workOrderMasterTag.ScheduleSpread = 0;
            workOrderMasterTag.SecondaryQuantityCompleted = 0;
            workOrderMasterTag.SequenceBubbleSequence = 0;
            workOrderMasterTag.SequenceNumber = 0;
            workOrderMasterTag.Serializedcomponents = " ";
            workOrderMasterTag.ShiftCode = " ";
            workOrderMasterTag.ShipThisPhase = " ";
            workOrderMasterTag.ShrinkFactor = 0;
            workOrderMasterTag.ShrinkFactorMethod = " ";
            workOrderMasterTag.SupplierLotNumber = " ";
            workOrderMasterTag.SupplierRecoveryManagerNumber = 0;
            workOrderMasterTag.TaxExplCode1 = " ";
            workOrderMasterTag.TaxRateArea = " ";
            workOrderMasterTag.TimeCompleted = 0;
            workOrderMasterTag.TimeGuaranteedResponse = 0;
            workOrderMasterTag.TimeScheduledEndHHMMSS = 0;
            workOrderMasterTag.TimeScheduledStartHHMMSS = 0;
            workOrderMasterTag.TimeZoneList = " ";
            workOrderMasterTag.TotalEstimated = 0;
            workOrderMasterTag.TotalPlanned = 0;
            workOrderMasterTag.UnitofMeasureSecondary = " ";
            workOrderMasterTag.UnitsSecondaryQuantityOrdered = 0;
            workOrderMasterTag.UnitsUnaccountedScrap = 0;


            return workOrderMasterTag;
        }
    }

}
