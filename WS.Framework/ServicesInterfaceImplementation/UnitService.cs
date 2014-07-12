using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using NLog;
using WS.Framework.Objects;
using WS.Framework.Objects.WorkOrder;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class UnitService : IUnitService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Unit GetUnitByUnitNumber(string unitNumber)
        {

            unitNumber = PadUnitNumber(unitNumber);

            Unit unit = new Unit();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var u = (from a in context.Assets
                             join ap in context.Assets on a.ParentNumber equals ap.ID
                             join e in context.Equipments on a.ID equals e.ID
                             join ca in context.CustomAssets on a.ID equals ca.ID
                             join ab in context.AddressBooks on a.AddressNumberLessorRentorMortgagor equals ab.ID
                             where a.UnitNumber.Trim() == DbFunctions.AsNonUnicode(unitNumber)
                             select new
                                 {
                                     AssetID = ca.ID,
                                     a.MajorAccountingClass,
                                     a.BusinessUnit,
                                     a.UnitNumber,
                                     ComplexUnitNumber = ap.UnitNumber,
                                     a.SerialNumber,
                                     a.EquipmentClass,
                                     a.Manufacturer,
                                     a.ModelYear,
                                     a.TotalLength,
                                     a.BoxWidth,
                                     a.BoxLength,
                                     a.Description2,
                                     a.Description3,
                                     a.DateAcquired,
                                     e.SquareFootage,
                                     a.Kit2ndItemNumber

                                 }).FirstOrDefault();

                    if (u != null)
                    {
                        if (u.MajorAccountingClass.Trim() == "CPX")
                        {
                            unit.UnitReturnCode = 2;
                            unit.UnitReturnCodeMessage = "Please enter separate Work Orders for each child unit of the complex";

                        }
                        else
                        {
                            IHelperService helperService = new HelperService();

                            unit.AssetID = Convert.ToInt32(u.AssetID);
                            unit.MajorAccountingClass = u.MajorAccountingClass.Trim();
                            unit.BusinessUnit = u.BusinessUnit.Trim();
                            unit.UnitNumber = u.UnitNumber.Trim();
                            unit.ComplexUnitNumber = u.ComplexUnitNumber.Trim();
                            unit.SerialNumber = u.SerialNumber.Trim();
                            unit.EquipmentClass = u.EquipmentClass.Trim();
                            unit.Manufacturer = u.Manufacturer.Trim();
                            unit.ModelYear = u.ModelYear.Trim();
                            unit.TotalLength = Convert.ToInt32(u.TotalLength);
                            unit.BoxWidth = Convert.ToInt32(u.BoxWidth);
                            unit.BoxLength = Convert.ToInt32(u.BoxLength);
                            if (u.Description2 != null)
                            {
                                unit.Description1 = u.Description2.Trim();
                            }
                            else
                            {
                                unit.Description1 = "";

                            }
                            if (u.Description3 != null)
                            {
                                unit.Description2 = u.Description3.Trim();
                            }
                            else
                            {
                                unit.Description2 = "";
                            }
                            unit.DateAcquired = helperService.JDEDateToDateTime((Convert.ToInt32(u.DateAcquired)));
                            unit.SquareFootage = Convert.ToInt32(u.SquareFootage);
                            unit.Kit2ndItemNumber = u.Kit2ndItemNumber;

                            unit.UnitAttributes = GetUnitAttributesByAssetID(context, unit.AssetID);

                            unit.UnitReturnCode = 1;
                            
                        }

                        unit.OMBOrders = GetLast2OMBOrdersByUnitNumber(unitNumber);
                    }
                    else
                    {
                        unit.UnitReturnCode = 2;
                        unit.UnitReturnCodeMessage = "The unit number you entered cannot be found in the JD Edwards system";
                    }

                   
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                unit.UnitReturnCode = 3;
                unit.UnitReturnCodeMessage = "A general error occured when trying to find the Unit Number";
            }
            return unit;
        }

        public List<OMBOrder> GetLast2OMBOrdersByUnitNumber(string unitNumber)
        {
            List<OMBOrder> ombOrders = new List<OMBOrder>();

            using (WSJDE context = new WSJDE())
            {
                ombOrders = (from f in context.Assets
                             join p in context.Assets on f.ParentNumber equals p.ID
                             join u in context.OMBUnits on p.UnitNumber.Trim() equals u.UnitNumber.Trim()
                             join h in context.OMBOrderHeaders on u.OMBOrderHeaderID equals h.ID
                             join a in context.AddressBooks on h.AddressBookID equals a.ID
                             join lt in context.OMBLookups on h.OrderTypeID equals lt.ID
                             where f.UnitNumber == DbFunctions.AsNonUnicode(unitNumber) &&
                             h.QStatusID != 15 &&
                             h.OrderTypeID != 8 &&
                             h.OrderTypeID != 10
                             select new OMBOrder
                             {
                                 SKey = h.ID,
                                 Name = a.NameAlpha,
                                 QStatus = h.QStatusID,
                                 Type = lt.MemberDescription,
                                 OMBOrderHeaderID = u.OMBOrderHeaderID,
                                 OrderTypeID = u.OMBOrderHeaderID
                             }
                            ).ToList();

                ombOrders = ombOrders.Where(o => o.OrderTypeID != 8 && o.OrderTypeID != 10).OrderByDescending(o => o.OMBOrderHeaderID).Take(2).ToList();

            }
            return ombOrders;
        }

        public List<UnitAttribute> GetUnitAttributesByAssetID(WSJDE context, int assetID)
        {
            List<UnitAttribute> unitAttributes = new List<UnitAttribute>();
            try
            {

                    unitAttributes = (from a in context.Assets
                                      join fa in context.FleetAttributes on a.ID equals fa.ID
                                      join udct in context.UserDefinedCodeTypes on
                                          new {ProductCode = fa.SystemKey, UserDefinedCodeTypeID = fa.RT} equals
                                          new {udct.ProductCode, udct.UserDefinedCodeTypeID}
                                      join udc in context.UserDefinedCodes on
                                          new
                                              {
                                                  ProductCode = fa.SystemKey,
                                                  UserDefinedCodeTypeID = fa.RT,
                                                  UserDefinedCodeID = fa.LookupKey
                                              } equals
                                          new {udc.ProductCode, udc.UserDefinedCodeTypeID, udc.UserDefinedCodeID}
                                      join fc in context.OMBFleetCategories on
                                          new {udct.ProductCode, udct.UserDefinedCodeTypeID} equals
                                          new {ProductCode = fc.SystemCode, UserDefinedCodeTypeID = fc.ID}
                                      orderby fc.SortSequence, udct.Description, udc.Description1, fa.Description
                                      where a.ID == assetID
                                      select new UnitAttribute
                                          {
                                              ID =
                                                  udc.ProductCode.Trim() + "|" + udc.UserDefinedCodeTypeID.Trim() + "|" +
                                                   udc.UserDefinedCodeID.Trim(),
                                              ProductCode = udc.ProductCode.Trim(),
                                              UserDefinedCodeTypeID = udc.UserDefinedCodeTypeID.Trim(),
                                              UserDefinedCodeID = udc.UserDefinedCodeID.Trim(),
                                              UserDefinedCodeTypeDescription = udct.Description.Trim(),
                                              UserDefinedCodeDescription = udc.Description1.Trim(),
                                              FleetAttributeDescription = fa.Description.Trim()
                                          }).ToList();
                
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return unitAttributes;
        }

        public string PadUnitNumber(string unitNumber)
        {
            string[] unitNumberParts = unitNumber.Split('-');

            string firstPart = unitNumberParts[0];

            unitNumber = firstPart.PadRight(3) + "-" + unitNumberParts[1];

            return unitNumber;
        }
    }
}
