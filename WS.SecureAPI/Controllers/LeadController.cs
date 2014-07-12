using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;
using Newtonsoft.Json;
using WS.Framework.WSJDEData;
using WS.SecureAPI.ViewModels.WSCRequest;

namespace WS.SecureAPI.Controllers
{
    public class LeadController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        public async Task<object> Submit()
        {
            string jsonRequest = await Request.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonRequest))
            {
                return Request.CreateResponse(HttpStatusCode.LengthRequired, "JSON Object is empty");
            }

            Request request = new Request();

            try
            {
                request = JsonConvert.DeserializeObject<Request>(jsonRequest);

            }
            catch (Exception e)
            {
                logger.WarnException("Exception", e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, "JSON Data was malformed");
            }

            APISource apiSource = GetSource(request.Token);

            if (apiSource != null)
            {
                bool didRequestProcessOk = ProcessRequest(request, apiSource);

                if (didRequestProcessOk)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private bool ProcessRequest(Request request, APISource apiSource)
        {
            bool didRequestProcessOk = false;

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    
                    didRequestProcessOk = true;
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return didRequestProcessOk;
        }

        private APISource GetSource(string token)
        {
            APISource source = new APISource();
            try
            {
                using (WSJDE context = new WSJDE())
                {

                    source = (from s in context.APISources
                              where s.Token == token
                              select s).FirstOrDefault();

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return source;
        }
    }
}
