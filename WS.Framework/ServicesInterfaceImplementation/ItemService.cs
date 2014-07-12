using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using NLog;
using WS.Framework.Objects;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class ItemService : IItemService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public List<ItemActivity> GetItemActivitiesByBusinessUnitID(string businessUnitID)
        {
            List<ItemActivity> itemActivities = new List<ItemActivity>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var query = (from i in context.Items
                                 join ib in context.ItemBranches on i.ID equals ib.ID
                                 join la in context.LaborActivities on i.SecondItemNumber equals la.ActivityKey
                                 where la.UserReservedCode == DbFunctions.AsNonUnicode("CC")
                                       && ib.BusinessUnitID.Trim() == DbFunctions.AsNonUnicode(businessUnitID)
                                       && ib.BusinessUnitID == la.Branch
                                 select
                                     new
                                         {
                                             i.ID,
                                             i.Description,
                                             la.BusinessUnit,
                                             i.SalesCatalogSection,
                                             i.SalesCategoryCode4,
                                             i.UnitofMeasurePrimary, 
                                             i.SecondItemNumber,
                                             i.ThirdItemNumber
                                         });

                    itemActivities.AddRange(query.Select(x => x.ID).Distinct().Select(id => new ItemActivity { ID = id }));

                    foreach (ItemActivity itemActivity in itemActivities)
                    {
                        int itemID = itemActivity.ID;

                        var i = (from q in query where q.ID == itemID select q).FirstOrDefault();

                        itemActivity.Description = i.Description;
                        itemActivity.SalesCategoryCode4 = i.SalesCategoryCode4;
                        itemActivity.SalesCatalogSection = i.SalesCatalogSection;
                        itemActivity.UnitofMeasurePrimary = i.UnitofMeasurePrimary;
                        itemActivity.SecondItemNumber = i.SecondItemNumber;
                        itemActivity.ThirdItemNumber = i.ThirdItemNumber;

                        string workTypes = string.Join(",", query.Where(x => x.ID == itemID)
                            .Select(y => y.BusinessUnit.Replace(businessUnitID, "").Trim()));

                        itemActivity.WorkTypes = workTypes;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return itemActivities;
        }

        public List<ItemPart> GetItemPartsByBusinessUnitID(string businessUnitID)
        {
            List<ItemPart> parts = new List<ItemPart>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var query = (from i in context.Items
                                 join ib in context.ItemBranches on i.ID equals ib.ID
                                 orderby ib.Region descending, i.Description
                                 where i.SalesCategoryCode5 == "PRT"
                                 && ib.BusinessUnitID.Trim() == DbFunctions.AsNonUnicode(businessUnitID)
                                 select new
                                 {
                                     i.ID,
                                     i.Description,
                                     i.SearchText,
                                     ib.Region,
                                     i.SalesCatalogSection,
                                     i.UnitofMeasurePrimary
                                 });

                    parts.AddRange(query.Select(variable => new ItemPart
                    {
                        ID = variable.ID,
                        Description = variable.Description.Trim(),
                        SearchText = variable.SearchText.Trim(),
                        Region = variable.Region.Trim(),
                        SalesCatalogSection= variable.SalesCatalogSection.Trim(),
                        UnitofMeasurePrimary = variable.UnitofMeasurePrimary
                    }).OrderBy(p => p.SalesCatalogSection));
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return parts;
        }

        public string GetCatagoryGLBySecondItemNumber(string secondItemNumber)
        {
            string catagoryGLBySecond = "";

            string secondItemNumberString = secondItemNumber;
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    catagoryGLBySecond = (from i in context.Items
                                          where i.SecondItemNumber == DbFunctions.AsNonUnicode(secondItemNumberString)
                                          select i.CategoryGL).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return catagoryGLBySecond;
        }

        public int GetItemIDBySecondItemNumber(string secondItemNumber)
        {
            int itemID = 0;
            string secondItemNumberString = secondItemNumber;

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    itemID = (from i in context.Items
                              where i.SecondItemNumber == DbFunctions.AsNonUnicode(secondItemNumberString)
                              select i.ID).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return itemID;
        }

        public string[] GetSecondAndThirdItemNumberByItemID(int itemID)
        {
            string[] itemNumbers = new string[2];

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var query = (from i in context.Items
                                 where i.ID == itemID
                                 select new
                                     {
                                         i.SecondItemNumber,
                                         i.ThirdItemNumber
                                     }).FirstOrDefault();

                    itemNumbers[0] = query.SecondItemNumber;
                    itemNumbers[1] = query.ThirdItemNumber;

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return itemNumbers;
        }
    }
}
