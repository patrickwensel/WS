using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AS.App.Intranet.ViewModels.Company
{
    public class CategoryItem
    {
        public string CategoryName { get; set; }
        public List<SubCategoryItem> SubCategories { get; set; }
    }
}