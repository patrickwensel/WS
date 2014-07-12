using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class SecurityLevelService : ISecurityLevelService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public List<SecurityLevel> GetSecurityLevelsByApplicationID(int applicationID)
        {
            List<SecurityLevel> securityLevels = new List<SecurityLevel>();
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    securityLevels = (from sl in context.SecurityLevels
                                    where sl.ApplicationID == applicationID
                                    select sl).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return securityLevels;

        }
    }
}
