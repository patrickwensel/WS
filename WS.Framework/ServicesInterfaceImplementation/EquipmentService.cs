using System;
using System.Linq;
using NLog;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class EquipmentService : IEquipmentService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string GetProductFamilyByAssetID(int assetID)
        {
            string productFamily = "";
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    productFamily = (from e in context.Equipments
                                     where e.ID == assetID
                                     select e.ProductFamily).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return productFamily;

        }
    }
}
