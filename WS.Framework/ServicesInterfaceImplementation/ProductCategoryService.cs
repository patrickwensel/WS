using System;
using System.Collections.Generic;
using NLog;
using WS.Framework.ServicesInterface;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class ProductCategoryService : IProductCategoryService
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        //public List<ProductCategory> GetActiveProductCategories()
        //{
        //    List<ProductCategory> productCatagories = new List<ProductCategory>();

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            productCatagories = (from pc in context.ProductCategories
        //                                 where pc.Status == 1
        //                                 select pc).ToList();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return productCatagories;
        //}
    }
}
