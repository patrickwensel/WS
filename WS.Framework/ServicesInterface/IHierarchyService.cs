using System.Collections.Generic;
using System.Linq;
using WS.Framework.ASJDE;

namespace WS.Framework.ServicesInterface
{
    public interface IHierarchyService
    {
        IQueryable<Company> GetAllCompanies();

        IQueryable<StrategicBusinessUnit> GetAllStrategicBusinessUnits();
        List<StrategicBusinessUnit> GetStrategicBusinessUnitsByCompanyID(decimal companyID);

        IQueryable<OperationalBusinessUnit> GetAllOperationalBusinessUnits();
        List<OperationalBusinessUnit> GetOperationalBusinessUnitsByStrategicBusinessUnitID(decimal strategicBusinessUnitID);

        IQueryable<BusinessUnit> GetAllBusinessUnits();
        List<BusinessUnit> GetBusinessUnitsByOperationalBusinessUnitID(decimal operationalBusinessUnitID);

        IQueryable<Branch> GetAllBranchs();
        IQueryable<Depot> GetAllDepots();

    }
}
