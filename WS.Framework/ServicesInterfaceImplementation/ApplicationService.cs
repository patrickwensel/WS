using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Web;
using NLog;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class ApplicationService : IApplicationService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEmployeeService _employeeService;

        public ApplicationService(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public List<Application> GetActiveApplications()
        {
            List<Application> applications = new List<Application>();
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    string userName = HttpContext.Current.User.Identity.Name.Split('\\')[1];

                    string userEmployeeID = _employeeService.GetEmployeeIDByADUserName(userName);

                    List<int?> userSecurities = (from s in context.Securities
                                                 where s.EmployeeID == userEmployeeID
                                                       && s.ApplicationID == 8
                                                 select s.SecurityLevelLevelID).ToList();

                    if (userSecurities.Count != 0)
                    {

                        if (userSecurities.Contains(1))
                        {
                            applications = (from a in context.Applications
                                            where a.Status == 1
                                            select a).ToList();
                        }
                        else
                        {
                            var apps1 = (from s in context.Securities
                                         where s.EmployeeID == DbFunctions.AsNonUnicode(userEmployeeID)
                                               && s.ApplicationID == 8
                                         select s.SecurityLevelLevelID);

                            var apps2 = (from sl in context.SecurityLevels
                                         where sl.ApplicationID == 8
                                         && apps1.Contains(Convert.ToInt32(sl.LevelID))
                                         select sl.SubApplicationID);

                            applications = (from a in context.Applications
                                            where a.Status == 1
                                            && apps2.Contains(a.ID)
                                            select a).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
            return applications;

        }
    }
}
