using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using NLog;
using WS.Framework.WSJDEData;


namespace WS.App.VAPSInventory.Models
{
    public class DataService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string GetUserName()
        {
            string userName = "";

            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            string userNameWithDomain = principal.Identity.Name;

            if (userNameWithDomain != null)
            {
                userName = userNameWithDomain.Split('\\')[1];
            }

            return userName;
        }

        public static List<Branch> Branches()
        {
            List<Branch> branchs = new List<Branch>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var query = (from bu in context.BusinessUnits
                                 where bu.CategoryCodeBusinessUnit09 == "Y"
                                       && bu.BusinessUnitType == "BR"
                                 orderby bu.Description
                                 select new
                                 {
                                     id = bu.ID,
                                     desc = bu.Description
                                 }).ToList();

                    foreach (var x in query)
                    {
                        Branch branch = new Branch { ID = Convert.ToInt32(x.id.Trim()), Name = x.desc.Trim() };
                        branchs.Add(branch);
                    }

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return branchs;
        }

        public static List<Year> Year()
        {
            List<Year> years = new List<Year>();

            DateTime now = DateTime.Now;

            int thisYear = now.Year;

            for (int i = -5; i < 2; i++)
            {
                if ((thisYear + i) > 2012)
                {
                    Year year = new Year { ID = thisYear + i, Name = thisYear + i };

                    years.Add(year);
                }
            }

            years.OrderBy(y => y.ID);

            return years;
        }

        public static List<Year> YearFiltered(int locationNumber)
        {
            List<Year> allYears = Year();
            List<Year> years = new List<Year>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    foreach (Year allYear in allYears)
                    {
                        List<int> monthsInThisYear = (from i in context.Inventories
                                                      where i.Year == allYear.ID && i.LocationNumber == locationNumber
                                                      select i.Month).ToList();

                        List<int> allMonths = new List<int>();

                        for (int i = 1; i < 13; i++)
                        {
                            allMonths.Add(i);
                        }

                        IEnumerable<int> x = allMonths.Except(monthsInThisYear);

                        if (x.Any())
                        {
                            years.Add(allYear);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return years;
        }

        public static List<Month> Month()
        {
            List<Month> months = new List<Month>
                {
                    new Month {ID = 1, Name = "January"},
                    new Month {ID = 2, Name = "February"},
                    new Month {ID = 3, Name = "March"},
                    new Month {ID = 4, Name = "April"},
                    new Month {ID = 5, Name = "May"},
                    new Month {ID = 6, Name = "June"},
                    new Month {ID = 7, Name = "July"},
                    new Month {ID = 8, Name = "August"},
                    new Month {ID = 9, Name = "September"},
                    new Month {ID = 10, Name = "October"},
                    new Month {ID = 11, Name = "November"},
                    new Month {ID = 12, Name = "December"}
                };

            return months;
        }

        public static List<Month> MonthFiltered(int locationNumber, int year)
        {
            List<Month> allMonths = Month();

            List<Month> months = new List<Month>();

            try
            {
                using (WSJDE context = new WSJDE())
                {

                    List<int> monthsInThisYear = (from i in context.Inventories
                                                  where i.LocationNumber == locationNumber && i.Year == year
                                                  select i.Month).ToList();

                    months = (from al in allMonths
                              where !monthsInThisYear.Contains(al.ID)
                              select al).ToList();


                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }


            return months;
        }
    }
    public class Year
    {
        public int ID { get; set; }
        public int Name { get; set; }
    }

    public class Month
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Branch
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}