using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using NLog;
using WS.Framework.Objects;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;
using Attribute = WS.Framework.Objects.WorkOrder.Attribute;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class UserDefinedCodeService : IUserDefinedCodeService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public List<UserDefinedCodeWorkOrder> GetUserDefinedCodeWorkOrder()
        {
            List<UserDefinedCodeWorkOrder> userDefinedCodeWorkOrders = new List<UserDefinedCodeWorkOrder>();
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    IEnumerable<UserDefinedCode> userDefinedCodes = (from udc in context.UserDefinedCodes
                                                                     orderby udc.Description1
                                                                     where udc.Description2 == DbFunctions.AsNonUnicode("AS")
                                                                           && udc.UserDefinedCodeTypeID == DbFunctions.AsNonUnicode("TY")
                                                                           && udc.ProductCode == DbFunctions.AsNonUnicode("00")
                                                                     select udc);

                    userDefinedCodeWorkOrders.AddRange(userDefinedCodes.Select(userDefinedCode => new UserDefinedCodeWorkOrder
                        {
                            UserDefinedCodeID = userDefinedCode.UserDefinedCodeID.Trim(), Description1 = userDefinedCode.Description1
                        }));
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return userDefinedCodeWorkOrders;
        }

        public List<Attribute> GetAllAttibutes()
        {
            List<Attribute> attributes = new List<Attribute>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    attributes = (from udc in context.UserDefinedCodes
                                  join udct in context.UserDefinedCodeTypes
                                      on new {udc.ProductCode, udc.UserDefinedCodeTypeID} equals
                                      new {udct.ProductCode, udct.UserDefinedCodeTypeID}
                                  join ofc in context.OMBFleetCategories
                                      on
                                      new
                                          {
                                              ProdcutCode = udct.ProductCode,
                                              udct.UserDefinedCodeTypeID
                                          } equals
                                      new {ProdcutCode = ofc.SystemCode, UserDefinedCodeTypeID = ofc.ID}
                                  select new Attribute()
                                      {
                                          ID = udc.ProductCode.Trim() + "|" + udc.UserDefinedCodeTypeID.Trim() + "|" + udc.UserDefinedCodeID.Trim(),
                                          ProductCode = udc.ProductCode.Trim(),
                                          UserDefinedCodeTypeID = udc.UserDefinedCodeTypeID.Trim(),
                                          UserDefinedCodeID = udc.UserDefinedCodeID.Trim(),
                                          Description = udct.Description.Trim(),
                                          Description1 = udc.Description1.Trim(),
                                          SortSequence = ofc.SortSequence.Value
                                      }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return attributes;

        }
    }
    
}
