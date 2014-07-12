using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WS.Framework.Objects.SalesForce;
using WS.Framework.ServicesInterface;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class SalesForceService : ISalesForceService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public async Task<bool> PostLead(SFLead sfLead)
        {
            JObject jObject = await GetSFOAuthObject();
            string oauthToken = (string)jObject["access_token"];
            string serviceUrl = (string)jObject["instance_url"];

            HttpClient leadClient = new HttpClient();

            string requestMessage = JsonConvert.SerializeObject(sfLead.Lead);

            HttpContent content2 = new StringContent(requestMessage, Encoding.UTF8, "application/json");
            string sfdcObjectPath = ConfigurationManager.AppSettings.Get("sfdcObjectPath");

            string uri = serviceUrl + sfdcObjectPath + "Lead";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

            request.Headers.Add("Authorization", "Bearer " + oauthToken);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            request.Content = content2;

            HttpResponseMessage response = await leadClient.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            XDocument xDocument = XDocument.Parse(result);

            var resultsElements = xDocument.Elements();

            foreach (var xElement in resultsElements)
            {
                var elementName = xElement.Name;

                if (elementName == "Errors")
                {
                    var x = xDocument.Root.Elements();

                    foreach (var resultsElement in x)
                    {
                        string y = resultsElement.Value;
                        logger.Error("Salesforce API Error: " + y);
                    }

                    return false;

                }
                if (elementName == "Result")
                {
                    XElement success = (from x in xDocument.Descendants("success")
                                       select x).FirstOrDefault();

                    if (success.Value == "true")
                    {
                        XElement leadID = (from x in xDocument.Descendants("id")
                                           select x).FirstOrDefault();

                        sfLead.LeadID = leadID.Value;

                        bool qaProcess = await PostQA(sfLead);

                        return true;

                    }
                    else
                    {
                        logger.Error("Salesforce API Error: Unknown");
                        return false;
                    }
                }
            }

            logger.Error("Salesforce API Error: Unknown");
            return false;
        }

        public async Task<bool> PostQA(SFLead sfLead)
        {
            JObject jObject = await GetSFOAuthObject();
            string oauthToken = (string)jObject["access_token"];
            string serviceUrl = (string)jObject["instance_url"];

            //Lead_Q_A__c lead_Q_A__c2 = new Lead_Q_A__c
            //{
            //    Answer__c = "abcdef",
            //    Question__c = "222222",
            //    Quote_Id__c = "12345678",
            //    Lead__c = sfLead.LeadID
            //};

            foreach (Lead_Q_A__c leadQAC in sfLead.QuestionAnswers)
            {
                leadQAC.Lead__c = sfLead.LeadID;

                HttpClient leadClient = new HttpClient();
                string requestMessage = JsonConvert.SerializeObject(leadQAC);

                HttpContent content2 = new StringContent(requestMessage, Encoding.UTF8, "application/json");
                string sfdcObjectPath = ConfigurationManager.AppSettings.Get("sfdcObjectPath");

                string uri = serviceUrl + sfdcObjectPath + "Lead_Q_A__c";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

                request.Headers.Add("Authorization", "Bearer " + oauthToken);

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                request.Content = content2;

                HttpResponseMessage response = await leadClient.SendAsync(request);

                string result = await response.Content.ReadAsStringAsync();
            }

            return true;

        }




        public async Task<JObject> GetSFOAuthObject()
        {

            HttpClient authClient = new HttpClient();

            string sfdcConsumerKey = ConfigurationManager.AppSettings.Get("sfdcConsumerKey");
            string sfdcConsumerSecret = ConfigurationManager.AppSettings.Get("sfdcConsumerSecret");
            string sfdcUserName = ConfigurationManager.AppSettings.Get("sfdcUserName");
            string sfdcPassword = ConfigurationManager.AppSettings.Get("sfdcPassword");
            string sfdcToken = ConfigurationManager.AppSettings.Get("sfdcToken");
            string sfdcOAuthSite = ConfigurationManager.AppSettings.Get("sfdcOAuthSite");

            string loginPassword = sfdcPassword + sfdcToken;

            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "password"},
                    {"client_id", sfdcConsumerKey},
                    {"client_secret", sfdcConsumerSecret},
                    {"username", sfdcUserName},
                    {"password", loginPassword}
                });

            HttpResponseMessage message = await authClient.PostAsync(sfdcOAuthSite, content);

            string responseString = await message.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(responseString);

            return jObject;

        }

        public async Task<bool> LeadDescribe()
        {
            HttpClient authClient = new HttpClient();

            string sfdcConsumerKey = ConfigurationManager.AppSettings.Get("sfdcConsumerKey");
            string sfdcConsumerSecret = ConfigurationManager.AppSettings.Get("sfdcConsumerSecret");
            string sfdcUserName = ConfigurationManager.AppSettings.Get("sfdcUserName");
            string sfdcPassword = ConfigurationManager.AppSettings.Get("sfdcPassword");
            string sfdcToken = ConfigurationManager.AppSettings.Get("sfdcToken");
            string sfdcOAuthSite = ConfigurationManager.AppSettings.Get("sfdcOAuthSite");

            string loginPassword = sfdcPassword + sfdcToken;

            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "password"},
                    {"client_id", sfdcConsumerKey},
                    {"client_secret", sfdcConsumerSecret},
                    {"username", sfdcUserName},
                    {"password", loginPassword}
                });

            HttpResponseMessage message = await authClient.PostAsync(sfdcOAuthSite, content);

            string responseString = await message.Content.ReadAsStringAsync();

            JObject obj = JObject.Parse(responseString);
            string oauthToken = (string)obj["access_token"];
            string serviceUrl = (string)obj["instance_url"];


            HttpClient createClient = new HttpClient();

            //string requestMessage = JsonConvert.SerializeObject(lead);

            //HttpContent content2 = new StringContent(requestMessage, Encoding.UTF8, "application/json");
            string sfdcObjectPath = ConfigurationManager.AppSettings.Get("sfdcObjectPath");

            string uri = serviceUrl + sfdcObjectPath + "Lead/describe";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Add("Authorization", "Bearer " + oauthToken);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            //request.Content = content2;

            HttpResponseMessage response = await createClient.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            XDocument xDocument = XDocument.Parse(result);

            xDocument.Save(@"c:\temp\leaddescribe.xml");

            return true;
        }
    }
}
