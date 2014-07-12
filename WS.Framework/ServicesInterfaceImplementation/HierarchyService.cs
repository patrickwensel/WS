using System.Collections.Generic;
using System.Linq;
using WS.Framework.ASJDE;
using WS.Framework.ServicesInterface;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class HierarchyService : IHierarchyService
    {
        #region Company

        public IQueryable<Company> GetAllCompanies()
        {
            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<Company> companies = (from a in context.Companies
                                             select a);

            return companies;

        }

        #endregion

        #region StrategicBusienssUnit

        public IQueryable<StrategicBusinessUnit> GetAllStrategicBusinessUnits()
        {

            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<StrategicBusinessUnit> strategicBusinessUnits = (from a in context.StrategicBusinessUnits
                                                                        select a
                                                                       );

            return strategicBusinessUnits;

        }

        public List<StrategicBusinessUnit> GetStrategicBusinessUnitsByCompanyID(decimal companyID)
        {

            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<StrategicBusinessUnit> strategicBusinessUnits = (from a in context.StrategicBusinessUnits
                                                                        where a.CompanyID == companyID
                                                                        select a
                                                                       );

            return strategicBusinessUnits.ToList();

        }

        #endregion


        #region OperationalBusinessUnit

        public IQueryable<OperationalBusinessUnit> GetAllOperationalBusinessUnits()
        {

            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<OperationalBusinessUnit> operationalBusinessUnits = (from a in context.OperationalBusinessUnits
                                                                            select a
                                                                           );

            return operationalBusinessUnits;

        }

        public List<OperationalBusinessUnit> GetOperationalBusinessUnitsByStrategicBusinessUnitID(decimal strategicBusinessUnitID)
        {

            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<OperationalBusinessUnit> operationalBusinessUnits = (from a in context.OperationalBusinessUnits
                                                                            where a.StrategicBusinessUnitID == strategicBusinessUnitID
                                                                            select a
                                                                           );

            return operationalBusinessUnits.ToList();

        }

        #endregion

        #region Business Unit

        public IQueryable<BusinessUnit> GetAllBusinessUnits()
        {

            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<BusinessUnit> businessUnits = (from a in context.BusinessUnits
                                                     select a
                                                    );

            return businessUnits;

        }

        public List<BusinessUnit> GetBusinessUnitsByOperationalBusinessUnitID(decimal operationalBusinessUnitID)
        {

            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<BusinessUnit> businessUnits = (from a in context.BusinessUnits
                                                      where a.OperationalBusinessUnitID == operationalBusinessUnitID
                                                      select a
                                                    );

            return businessUnits.ToList();

        }

        #endregion

        public IQueryable<Branch> GetAllBranchs()
        {

            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<Branch> branchs = (from a in context.Branches
                                                   select a
                                                  );

            return branchs;

        }

        


        public IQueryable<Depot> GetAllDepots()
        {

            ASJDE.ASJDE context = new ASJDE.ASJDE();

            IQueryable<Depot> depots = (from a in context.Depots
                                                          select a
                                                   );

            return depots;

        }
    }
}
