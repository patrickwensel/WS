using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AS.App.PassportAddOn.Objects;
using AS.App.PassportAddOn.ViewModels.EmployeeSearch;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using LinqToLdap;
using WS.Framework.ServicesInterface;
using User = WS.Framework.Objects.LDAP.User;

namespace AS.App.PassportAddOn.Controllers
{
    public class EmployeeSearchController : Controller
    {

        private readonly ILDAPService _ldapService;

        public EmployeeSearchController(ILDAPService ldapService)
        {
            _ldapService = ldapService;
        }

        public ActionResult Index()
        {
            PopulateCountries();

            SearchViewModel searchViewModel = new SearchViewModel();

            return View(searchViewModel);
        }


        public ActionResult Results(string id)
        {
            PopulateCountries();
            PopulateFunctionalArea();
            PopulateLocation();

            string[] partsOfID = id.Split(new[] { '|' });

            SearchViewModel searchViewModel = new SearchViewModel
                {
                    Keyword = partsOfID[0],
                    CountryCode = partsOfID[1],
                    Advanced = partsOfID[2]
                };

            return View(searchViewModel);
        }

        public void PopulateCountries()
        {
            IEnumerable<CountryViewModel> countries = AutoMapper.Mapper.Map<IEnumerable<CountryViewModel>>(_ldapService.GetAllCountries());

            List<SelectListItem> countriesList = countries.Select(countryViewModel => new SelectListItem { Value = countryViewModel.Code, Text = countryViewModel.Name }).OrderBy(t => t.Text).ToList();

            SelectList countriesSelectList = new SelectList(countriesList, "value", "text");

            ViewData["countries"] = countriesSelectList;
        }

        public void PopulateLocation()
        {
            IEnumerable<SiteViewModel> siteViewModels = AutoMapper.Mapper.Map<IEnumerable<SiteViewModel>>(_ldapService.GetAllSites());

            List<SelectListItem> siteViewModelList = siteViewModels.Select(siteViewModel => new SelectListItem { Value = siteViewModel.Name, Text = siteViewModel.Name }).OrderBy(t => t.Text).ToList();

            SelectList locationSelectList = new SelectList(siteViewModelList, "value", "text");

            ViewData["sites"] = locationSelectList;
        }

        public void PopulateFunctionalArea()
        {
            IEnumerable<ServiceViewModel> serviceViewModels = AutoMapper.Mapper.Map<IEnumerable<ServiceViewModel>>(_ldapService.GetAllServices());

            List<SelectListItem> serviceViewModelsList = serviceViewModels.Select(serviceViewModel => new SelectListItem { Value = serviceViewModel.Name, Text = serviceViewModel.Name }).OrderBy(t => t.Text).ToList();

            SelectList serviceViewModelsSelectList = new SelectList(serviceViewModelsList, "value", "text");

            ViewData["services"] = serviceViewModelsSelectList;
        }



        #region SearchResults

        public ActionResult SearchResultsRead([DataSourceRequest] DataSourceRequest request)
        {
            EmployeeSearchAdditionalData employeeSearchAdditionalData = new EmployeeSearchAdditionalData();

            TryUpdateModel(employeeSearchAdditionalData);
            int searchType = FindSearchType(employeeSearchAdditionalData);

            if (searchType == 0)
            {
                return null;
            }

            string domain = ConfigurationManager.AppSettings["LDAPDomain"];
            string serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
            string servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

            LdapDirectoryIdentifier ldapDirectoryIdentifier = new LdapDirectoryIdentifier(domain);
            NetworkCredential myCredentials = new NetworkCredential(serviceUser, servicePassword);

            LdapConnection connection = new LdapConnection(ldapDirectoryIdentifier, myCredentials, AuthType.Basic);
            connection.SessionOptions.ProtocolVersion = 3;
            DirectoryContext context = new DirectoryContext(connection);

            var orders = context.Query<User>();

            switch (searchType)
            {
                case 1:
                    orders = orders.Where(u => (u.FirstName.Contains(employeeSearchAdditionalData.Keyword)) || (u.LastName.Contains(employeeSearchAdditionalData.Keyword)));
                    break;
                case 2:
                    orders = orders.Where(u => (u.CountryCode == employeeSearchAdditionalData.CountryCode1));
                    break;
                case 3:
                    orders = orders.Where(u => ((u.FirstName.Contains(employeeSearchAdditionalData.Keyword)) || (u.LastName.Contains(employeeSearchAdditionalData.Keyword)))
                        && (u.CountryCode == employeeSearchAdditionalData.CountryCode1));
                    break;
                case 4:
                    if(!String.IsNullOrWhiteSpace(employeeSearchAdditionalData.FirstName))
                    {
                        orders = orders.Where(u => (u.FirstName.Contains(employeeSearchAdditionalData.FirstName)));
                    }
                    if (!String.IsNullOrWhiteSpace(employeeSearchAdditionalData.LastName))
                    {
                        orders = orders.Where(u => (u.LastName.Contains(employeeSearchAdditionalData.LastName)));
                    }
                    if (!String.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode2))
                    {
                        orders = orders.Where(u => (u.CountryCode == employeeSearchAdditionalData.CountryCode2));
                    }
                    if (!String.IsNullOrWhiteSpace(employeeSearchAdditionalData.Location))
                    {
                        orders = orders.Where(u => (u.Location == employeeSearchAdditionalData.Location));
                    }
                    if (!String.IsNullOrWhiteSpace(employeeSearchAdditionalData.PositionTitle))
                    {
                        orders = orders.Where(u => (u.PositionTitle.Contains(employeeSearchAdditionalData.PositionTitle)));
                    }
                    if (!String.IsNullOrWhiteSpace(employeeSearchAdditionalData.FunctionalArea))
                    {
                        orders = orders.Where(u => (u.FunctionalArea == employeeSearchAdditionalData.FunctionalArea));
                    }
                    break;

            }

            orders = orders.Where(c => (c.Status == "Actif"));
            
            var total = orders.Count();

            orders = orders.ApplyOrdersSorting(request.Groups, request.Sorts);

            var x = orders.ApplyOrdersPaging(request.Page, request.PageSize, total);

            var result = new DataSourceResult()
            {
                Data = x,
                Total = total
            };

            return Json(result);

        }

        public int FindSearchType(EmployeeSearchAdditionalData employeeSearchAdditionalData)
        {
            //Blank
            if (
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Keyword) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode1) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FirstName) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.LastName) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode2) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Location) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.PositionTitle) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FunctionalArea)
                )
                return 0;
            //Keyword
            if (
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Keyword) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode1) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FirstName) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.LastName) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode2) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Location) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.PositionTitle) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FunctionalArea)
                )
                return 1;
            //Country
            if (
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Keyword) &&
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode1) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FirstName) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.LastName) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode2) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Location) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.PositionTitle) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FunctionalArea)
                )
                return 2;
            //Keyword and Country
            if (
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Keyword) &&
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode1) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FirstName) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.LastName) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode2) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Location) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.PositionTitle) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FunctionalArea)
                )
                return 3;
            //First
            if (
                (string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Keyword) &&
                string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode1))  &&
                (
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FirstName) ||
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.LastName) ||
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.CountryCode2) ||
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.Location) ||
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.PositionTitle) ||
                !string.IsNullOrWhiteSpace(employeeSearchAdditionalData.FunctionalArea))
                )
                return 4;

            return 0;
        }

        #endregion

    }


    public static class AjaxCustomBindingExtensions
    {
        public static List<User> ApplyOrdersPaging(this IQueryable<User> data, int page, int pageSize, int total)
        {
            List<User> users = new List<User>();

            if (pageSize > 0 && page > 0)
            {
                users = data.Take(page * pageSize).ToList();
                if (page > 1)
                {
                    users.RemoveRange(0, ((page - 1) * pageSize));
                }
            }

            return users;
        }


        public static IQueryable<User> ApplyOrdersSorting(this IQueryable<User> data,
                    IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
        {
            if (groupDescriptors != null && groupDescriptors.Any())
            {
                foreach (var groupDescriptor in groupDescriptors.Reverse())
                {
                    data = AddSortExpression(data, groupDescriptor.SortDirection, groupDescriptor.Member);
                }
            }

            if (sortDescriptors != null && sortDescriptors.Any())
            {
                foreach (SortDescriptor sortDescriptor in sortDescriptors)
                {
                    data = AddSortExpression(data, sortDescriptor.SortDirection, sortDescriptor.Member);
                }
            }

            return data;
        }

        private static IQueryable<User> AddSortExpression(IQueryable<User> data, ListSortDirection
                    sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "FirstName":
                        data = data.OrderBy(order => order.FirstName);
                        break;
                    case "LastName":
                        data = data.OrderBy(order => order.LastName);
                        break;
                    case "CountryCode":
                        data = data.OrderBy(order => order.CountryCode);
                        break;
                    case "Location":
                        data = data.OrderBy(order => order.Location);
                        break;
                    case "PositionTitle":
                        data = data.OrderBy(order => order.PositionTitle);
                        break;
                    case "Function":
                        data = data.OrderBy(order => order.FunctionalArea);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "FirstName":
                        data = data.OrderByDescending(order => order.FirstName);
                        break;
                    case "LastName":
                        data = data.OrderByDescending(order => order.LastName);
                        break;
                    case "CountryCode":
                        data = data.OrderByDescending(order => order.CountryCode);
                        break;
                    case "Location":
                        data = data.OrderByDescending(order => order.Location);
                        break;
                    case "PositionTitle":
                        data = data.OrderByDescending(order => order.PositionTitle);
                        break;
                    case "Function":
                        data = data.OrderByDescending(order => order.FunctionalArea);
                        break;
                }
            }
            return data;
        }

        public static IQueryable<User> ApplyOrdersFiltering(this IQueryable<User> data,
           IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<User>(filterDescriptors, false));
            }
            return data;
        }
    }
}
