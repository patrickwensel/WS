using WS.Framework.ServicesInterface;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class ProductService : IProductService
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        //public List<Product> GetActiveProducts()
        //{
        //    List<Product> products = new List<Product>();

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            products = (from p in context.Products
        //                                 where p.Status == 1
        //                                 select p).ToList();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return products;

        //}

        //public IEnumerable<Product> GetActiveProductsByProductCategory(int productCategoryID)
        //{
        //    IEnumerable<Product> products = new List<Product>();

        //    try
        //    {
        //        using (WSJDE context = new WSJDE())
        //        {
        //            products = (from p in context.Products
        //                        where p.Status == 1 && p.ProductCategoryID == productCategoryID
        //                        select p).ToList();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.ErrorException("Exception", e);
        //    }

        //    return products;

        //}

    }
}
