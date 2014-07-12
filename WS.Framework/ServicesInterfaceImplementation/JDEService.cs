using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using NLog;
using WS.Framework.Objects;
using WS.Framework.ServicesInterface;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class JDEService : IJDEService
    {
        [DllImport("xmlinterop.dll",
            EntryPoint = "_jdeXMLRequest@20",
            CharSet = CharSet.Auto,
            ExactSpelling = false,
            CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]

        private static extern IntPtr jdeXMLRequest([MarshalAs(UnmanagedType.LPWStr)] string server, UInt16 port,
                                                   Int32 timeout, [MarshalAs(UnmanagedType.LPStr)] StringBuilder buf,
                                                   Int32 length);

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public JDEBusinessFunctionResponse CallJDEBusinessFunction(JDEBusinessFunction jdeBusinessFunction)
        {
            JDEConnectionVariables jdeConnectionVariables = GetJDEConnectionVariables();

            StringBuilder sbXML = new StringBuilder();

            sbXML.Append("<?xml version='1.0' encoding='utf-8'?>");
            sbXML.Append("<jdeRequest pwd='" + jdeConnectionVariables.Password + "' role='*ALL' type='callmethod' user='" + jdeConnectionVariables.UserName + "' session='' environment='" + jdeConnectionVariables.Environment + "' sessionidle=''> ");
            sbXML.Append("<callMethod name='" + jdeBusinessFunction.CallMethod + "' app='DotNepApplication' runOnError='no'> ");
            sbXML.Append("<params>");
            foreach (KeyValuePair<string, string> parameter in jdeBusinessFunction.Parameters)
            {
                sbXML.Append("<param name='" + parameter.Key + "'>" + parameter.Value +"</param>");
            }
            sbXML.Append("</params>");
            sbXML.Append("<onError abort='yes'></onError>");
            sbXML.Append("</callMethod></jdeRequest>");

            string serverName = jdeConnectionVariables.ServerName;
            UInt16 port = Convert.ToUInt16(jdeConnectionVariables.Port);
            Int32 timeout = Convert.ToInt32(jdeConnectionVariables.Timeout);

            string result = Marshal.PtrToStringAnsi(jdeXMLRequest(serverName, port, timeout, sbXML, sbXML.Length));

            XDocument xDocument = XDocument.Parse(result);

            JDEBusinessFunctionResponse jdeBusinessFunctionResponse = new JDEBusinessFunctionResponse
                {
                    ReturnCode = Convert.ToInt16((from item in xDocument.Descendants("returnCode")
                                                  select item.Attribute("code").Value).FirstOrDefault()),
                    Parameters = (from item in xDocument.Descendants("param")
                                  select item).ToDictionary(n => n.Attribute("name").Value, n => n.Value)
                };

            if (jdeBusinessFunctionResponse.ReturnCode != 0)
            {
                jdeBusinessFunctionResponse.ReturnCodeDescription =
                    GetReturnCodeDescription(jdeBusinessFunctionResponse.ReturnCode);

                logger.Error("JDEBusinessFunction Call threw an error, BOATransactionReturnCode={0}, ReturnCodeMessage={1}, Request={2}", jdeBusinessFunctionResponse.ReturnCode, jdeBusinessFunctionResponse.ReturnCodeDescription, sbXML.ToString());
            }


            return jdeBusinessFunctionResponse;
        }

        private JDEConnectionVariables GetJDEConnectionVariables()
        {
            JDEConnectionVariables jdeConnectionVariables = new JDEConnectionVariables
                {
                    UserName = ConfigurationManager.AppSettings.Get("UserName"),
                    Password = ConfigurationManager.AppSettings.Get("Password"),
                    Environment = ConfigurationManager.AppSettings.Get("JDEEnvironment"),
                    ServerName = ConfigurationManager.AppSettings.Get("ServerName"),
                    Port = ConfigurationManager.AppSettings.Get("Port"),
                    Timeout = ConfigurationManager.AppSettings.Get("Timeout")
                };

            return jdeConnectionVariables;
        }

        private string GetReturnCodeDescription(int returnCode)
        {
            string returnCodeDescription = null;

            switch (returnCode)
            {
                case 1:
                    returnCodeDescription = "Root XML element is not a jdeRequest or jdeResponse";
                    break;
                case 2:
                    returnCodeDescription = "The jdeRequest user identification is unknown. Check the user, password, and environment attributes or a callmethod request is missing the session attribute.";
                    break;
                case 3:
                    returnCodeDescription = "An XML parse error exists at line.";
                    break;
                case 4:
                    returnCodeDescription = "A fatal XML parse exists error at line.";
                    break;
                case 5:
                    returnCodeDescription = "An error occurred during parser initialization; the server is not configured correctly.";
                    break;
                case 6:
                    returnCodeDescription = "There is an unknown parse error.";
                    break;
                case 7:
                    returnCodeDescription = "The request session attribute is invalid.";
                    break;
                case 8:
                    returnCodeDescription = "The request type attribute is invalid.";
                    break;
                case 9:
                    returnCodeDescription = "The request type attribute is not given.";
                    break;
                case 10:
                    returnCodeDescription = "The request session attribute is invalid; the referenced process 'processid' no longer exists.";
                    break;
                case 11:
                    returnCodeDescription = "The jdeRequest child element is invalid or unknown.";
                    break;
                case 12:
                    returnCodeDescription = "The environment could not be initialized for user. Check user, password, and environment attribute values.";
                    break;
                case 13:
                    returnCodeDescription = "The jdeXMLRequest parameter is invalid.";
                    break;
                case 14:
                    returnCodeDescription = "The connection to JD Edwards EnterpriseOne failed.";
                    break;
                case 15:
                    returnCodeDescription = "The jdeXMLRequest send failed.";
                    break;
                case 16:
                    returnCodeDescription = "The jdeXMLResponse receive failed.";
                    break;
                case 17:
                    returnCodeDescription = "The jdeXMLResponse memory allocation failed.";
                    break;
                case 99:
                    returnCodeDescription = "An invalid BSFN name exists.";
                    break;
            }

            return returnCodeDescription;
        }
    }

    internal class JDEConnectionVariables
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Environment { get; set; }
        public string ServerName { get; set; }
        public string Port { get; set; }
        public string Timeout { get; set; }
    }
}



