using System;
using System.Linq;
using NLog;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class HyperionService : IHyperionService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string RunGLExtract(string month, string year, string ledgerType)
        {
            string returnMessage = null;
            
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    decimal result =
                        context.Database.SqlQuery<decimal>("SELECT jde_pkg.fn_gl_extract_hyp(" + month + "," + year + ",'" + ledgerType + "',0) retVal FROM DUAL").FirstOrDefault();

                    if (result == 0)
                    {
                        returnMessage = "The file is being created.  You should receive an email when it is complete";
                    }
                    else
                    {
                        returnMessage = "There was an error creating the file, please contact tech support";
                        logger.Error("Error creating the Hyperion GL Extract");
                    }

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                returnMessage = "There was an error creating the file, please contact tech support";
            }

            return returnMessage;
        }
    }
}
