using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;
using Newtonsoft.Json;
using WS.Framework.Objects.LDAP;
using WS.Framework.ServicesInterface;

namespace AS.API.Controllers
{
    public class LDAPController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ILDAPService _ldapService;

        public LDAPController(ILDAPService ldapService)
        {
            _ldapService = ldapService;
        }

        [HttpGet]
        public object GetUserByUserName(string userName)
        {
            //logger.Debug("Start");
            User user = new User();

            try
            {
                if (String.IsNullOrEmpty(userName))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                else
                {
                    user = _ldapService.GetUserByUserName(userName);
                }

            }
            catch (Exception e)
            {

                logger.ErrorException("Exception", e);
            }

            return user;
        }

        [HttpGet]
        public object GetAllCountries()
        {
            logger.Debug("Start");
            List<Country> countries = new List<Country>();

            try
            {
                countries = _ldapService.GetAllCountries();

            }
            catch (Exception e)
            {

                logger.ErrorException("Exception", e);
            }

            return countries;
        }


        [HttpGet]
        public object GetUsersByNameCountrySearch(string name, string countryCode)
        {
            //IEnumerable<KeyValuePair<string, string>> qs = Request.GetQueryNameValuePairs();

            //var name = qs.Where(x => x.Key == "name").Select(x => x.Value).FirstOrDefault();
            //var countryCode = qs.Where(x => x.Key == "countryCode").Select(x => x.Value).FirstOrDefault();

            if (String.IsNullOrEmpty(name))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            NameCountrySearch nameCountrySearch = new NameCountrySearch
                {
                    Name = name,
                    CountryCode = countryCode
                };

            try
            {
                List<User> users = new List<User>();//_ldapService.GetUsersByNameCountrySearch(nameCountrySearch);
                return users;

            }
            catch (Exception e)
            {
                logger.WarnException("Exception", e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, "JSON Data was malformed");
            }


            return Request.CreateResponse(HttpStatusCode.OK);

        }

        [HttpGet]
        public object WSTest()
        {
            //logger.Debug("Start");
            User user = new User();

            try
            {

                user = _ldapService.WSTest();

            }
            catch (Exception e)
            {

                logger.ErrorException("Exception", e);
            }

            return user;
        }



    }
}
