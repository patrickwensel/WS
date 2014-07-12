using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using WS.Framework.Objects;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;


namespace WS.Framework.ServicesInterfaceImplementation
{
    public class BusinessUnitService : IBusinessUnitService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public List<BusinessUnitShort> GetAllBusinessUnitShorts()
        {
            List<BusinessUnitShort> businessUnitShorts = new List<BusinessUnitShort>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    IEnumerable<BusinessUnit> businessUnits = GetAllBusinessUnits(context);

                    businessUnitShorts.AddRange(businessUnits.Select(businessUnit => new BusinessUnitShort
                        {
                            ID = businessUnit.ID, DescriptionCompressed = businessUnit.DescriptionCompressed, Company = businessUnit.Company, Description = businessUnit.Description, CategoryCodeBusinessUnit02 = businessUnit.CategoryCodeBusinessUnit02, BranchOffice = businessUnit.BranchOffice
                        }));

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return businessUnitShorts;

        }


        public IEnumerable<BusinessUnit> GetAllBusinessUnits(WSJDE context)
        {
            IQueryable<BusinessUnit> businessUnits = null;

            businessUnits = (from bu in context.BusinessUnits
                             where bu.CategoryCodeBusinessUnit09 == "Y"
                                   && bu.BusinessUnitType == "BR"
                                   orderby bu.Description
                             select bu);
            return businessUnits;
        }
    }
}

