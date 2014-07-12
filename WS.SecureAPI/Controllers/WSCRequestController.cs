using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;
using Newtonsoft.Json;
using WS.Framework.Objects.SalesForce;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;
using WS.SecureAPI.ViewModels.WSCRequest;

namespace WS.SecureAPI.Controllers
{
    public class WSCRequestController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ISalesForceService _salesForceService;

        public WSCRequestController(ISalesForceService salesForceService)
        {
            _salesForceService = salesForceService;


        }

        [HttpPost]
        public async Task<object> Submit()
        {
            try
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
                    var x = Task.Factory.StartNew(delegate
                        {
                            ProcessRequest(request, apiSource, jsonRequest);
                        }
                        );

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                try
                {
                    logger.Error("Raw Request: " + Request.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    logger.Error("Could not Read request");
                    logger.ErrorException("Exception", ex);
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        private async void ProcessRequest(Request request, APISource apiSource, string jsonRequest)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    APIRequest apiRequest = new APIRequest
                        {
                            APISourceID = apiSource.ID,
                            APIRequestSourceID = Convert.ToDecimal(request.Type),
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            Company = request.Company,
                            Address1 = request.Address1,
                            City = request.City,
                            State = request.State,
                            Zip = request.Zip,
                            Phone = request.Phone,
                            Email = request.Email,
                            CreateDate = DateTime.Now
                        };

                    context.APIRequests.Add(apiRequest);
                    context.SaveChanges();

                    foreach (RequestData data in request.Data)
                    {
                        APIRequestData apiRequestData = new APIRequestData
                            {
                                APIRequestID = apiRequest.ID,
                                APIRequestDataKeyID = Convert.ToDecimal(data.Key),
                                Name = data.Name,
                                Value = data.Value
                            };
                        context.APIRequestDatas.Add(apiRequestData);
                    }

                    context.SaveChanges();

                    if (request.Type == "1")
                    {
                        SFLead sfLead = BuiltLeadObject(request, context);

                        var x = await _salesForceService.PostLead(sfLead);
                    }

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                logger.Error("Raw Request: " + jsonRequest);
            }
        }

        private SFLead BuiltLeadObject(Request request, WSJDE context)
        {
            SFLead sfLead = new SFLead();

            string webSource = (from s in context.APISources
                                where s.Token == request.Token
                                select s.Code).FirstOrDefault();

            Lead lead = new Lead
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Company = request.Company,
                Phone = request.Phone,
                Street = request.Address1 + " " + request.Address2,
                City = request.City,
                State = request.State,
                PostalCode = request.Zip,
                Email = request.Email,
                Web_Source__c = webSource

            };

            sfLead.Lead = lead;

            List<Lead_Q_A__c> questionAnswers = new List<Lead_Q_A__c>();

            foreach (RequestData requestData in request.Data)
            {
                if (requestData.Key != null)
                {
                    RequestData data = requestData;
                    int id = Convert.ToInt32(data.Key);
                    APIRequestDataKey dataKey = (from dk in context.APIRequestDataKeys
                                                 where dk.ID == id
                                                 select dk).FirstOrDefault();

                    Lead_Q_A__c leadQAC = new Lead_Q_A__c();
                    leadQAC.Question__c = dataKey.APIRequestDataKeyClass.Name;

                    if (dataKey.ValueRequired)
                    {
                        leadQAC.Answer__c = requestData.Value;
                    }
                    else
                    {
                        leadQAC.Answer__c = dataKey.Name;

                    }

                    questionAnswers.Add(leadQAC);

                }
                else
                {
                    Lead_Q_A__c leadQAC = new Lead_Q_A__c
                        {
                            Question__c = requestData.Name,
                            Answer__c = requestData.Value
                        };

                    questionAnswers.Add(leadQAC);

                }
            }


            sfLead.QuestionAnswers = questionAnswers;


            return sfLead;

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
