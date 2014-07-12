using System;
using System.Linq;
using NLog;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class AddressBookService : IAddressBookService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string GetNameByID(WSJDE context, int id)
        {
            string name = (from ad in context.AddressBooks
                           where ad.ID == id
                           select ad.NameAlpha).FirstOrDefault();

            return name;

        }

        public string GetNameByID(int id)
        {
            string name = "";
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    name = GetNameByID(context, id);
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return name;
        }

        public string GetBusinessUnitByID(int id)
        {
            string businessUnit = "";
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var businessUnitUnpadded = (from ab1 in context.AddressBooks
                                                join ab2 in context.AddressBooks on ab1.AddressNumber5th equals ab2.ID
                                                where ab1.ID == id
                                                select ab2.BusinessUnit).FirstOrDefault();
                    businessUnit = businessUnitUnpadded.Trim().PadLeft(5, Convert.ToChar("0"));
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return businessUnit;
        }


        //public string GetPostalCodeByID(WSJDE context, int addressNumberParent)
        //{
        //    string postalCode = "";

        //    var query = (from ab in context.AddressBooks
        //                 join a in context.Addresses on ab.ID equals a.ID
        //                 where ab.ID == addressNumberParent
        //                 select new
        //                 {
        //                     ab.BusinessUnit,
        //                     a.PostalCode
        //                 });

        //    if (query.Any())
        //    {
        //        if (query.Count() == 1)
        //        {
        //            var x = query.FirstOrDefault();

        //            postalCode = x.BusinessUnit.Trim() == "10" ? x.PostalCode.Substring(0, 5) : x.PostalCode.Trim();
        //        }
        //    }

        //    return postalCode;
        //}

        //public string GetPostalCodeByID(int addressNumberParent)
        //{
        //    string name = "";
        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            GetPostalCodeByID(context, addressNumberParent);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return name;
        //}
    }
}

