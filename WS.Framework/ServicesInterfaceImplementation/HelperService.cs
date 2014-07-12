using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using System.Net.Mail;
using System.Web.UI.WebControls;
using NLog;
using WS.Framework.ServicesInterface;
using System.Web.Mvc;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class HelperService : IHelperService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DateTime JDEDateToDateTime(int jdeDate)
        {
            DateTime convertedDate;

            short yearValue = (short)(jdeDate / 1000d + 1900d);
            short dayValue = (short)((jdeDate % 1000) - 1);
            convertedDate = new DateTime(yearValue, 1, 1).AddDays(dayValue);

            return convertedDate;
        }

        public int DateTimeToJDEDate(DateTime dateTime)
        {
            int jdeDate;

            int dayInYear = dateTime.DayOfYear;
            int theYear = dateTime.Year - 1900;
            jdeDate = (theYear * 1000) + dayInYear;

            return jdeDate;
        }

        public int DateTimeToJDETime(DateTime dateTime)
        {
            int hour = dateTime.Hour;
            int minute = dateTime.Minute;
            int second = dateTime.Second;

            int jdeTime = hour*10000 + minute*100 + second;

            return jdeTime;
        }

        public void LogDbEntityValidationException(DbEntityValidationException dbEx)
        {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    logger.ErrorException("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage, dbEx);
                }
            }
        }

        public List<SelectListItem> GetSelectListStates()
        {

            List<SelectListItem> selectListItems = new List<SelectListItem>()
                {
                    new SelectListItem {Text = "Please select"},
                    new SelectListItem {Value = "AL", Text = "Alabama"},
                    new SelectListItem {Value = "AK", Text = "Alaska"},
                    //new SelectListItem { Value = "AS", Text="American Samoa" },
                    new SelectListItem {Value = "AZ", Text = "Arizona"},
                    new SelectListItem {Value = "AR", Text = "Arkansas"},
                    new SelectListItem {Value = "CA", Text = "California"},
                    new SelectListItem {Value = "CO", Text = "Colorado"},
                    new SelectListItem {Value = "CT", Text = "Connecticut"},
                    new SelectListItem {Value = "DE", Text = "Delaware"},
                    new SelectListItem {Value = "DC", Text = "District of Columbia"},
                    //new SelectListItem { Value = "FM", Text="Federated States of Micronesia" },
                    new SelectListItem {Value = "FL", Text = "Florida"},
                    new SelectListItem {Value = "GA", Text = "Georgia"},
                    new SelectListItem {Value = "GU", Text = "Guam"},
                    new SelectListItem {Value = "HI", Text = "Hawaii"},
                    new SelectListItem {Value = "ID", Text = "Idaho"},
                    new SelectListItem {Value = "IL", Text = "Illinois"},
                    new SelectListItem {Value = "IN", Text = "Indiana"},
                    new SelectListItem {Value = "IA", Text = "Iowa"},
                    new SelectListItem {Value = "KS", Text = "Kansas"},
                    new SelectListItem {Value = "KY", Text = "Kentucky"},
                    new SelectListItem {Value = "LA", Text = "Louisiana"},
                    new SelectListItem {Value = "ME", Text = "Maine"},
                    //new SelectListItem { Value = "MH", Text="Marshall Islands" },
                    new SelectListItem {Value = "MD", Text = "Maryland"},
                    new SelectListItem {Value = "MA", Text = "Massachusetts"},
                    new SelectListItem {Value = "MI", Text = "Michigan"},
                    new SelectListItem {Value = "MN", Text = "Minnesota"},
                    new SelectListItem {Value = "MS", Text = "Mississippi"},
                    new SelectListItem {Value = "MO", Text = "Missouri"},
                    new SelectListItem {Value = "MT", Text = "Montana"},
                    new SelectListItem {Value = "NE", Text = "Nebraska"},
                    new SelectListItem {Value = "NV", Text = "Nevada"},
                    new SelectListItem {Value = "NH", Text = "New Hampshire"},
                    new SelectListItem {Value = "NJ", Text = "New Jersey"},
                    new SelectListItem {Value = "NM", Text = "New Mexico"},
                    new SelectListItem {Value = "NY", Text = "New York"},
                    new SelectListItem {Value = "NC", Text = "North Carolina"},
                    new SelectListItem {Value = "ND", Text = "North Dakota"},
                    //new SelectListItem { Value = "MP", Text="Northern Mariana Islands" },
                    new SelectListItem {Value = "OH", Text = "Ohio"},
                    new SelectListItem {Value = "OK", Text = "Oklahoma"},
                    new SelectListItem {Value = "OR", Text = "Oregon"},
                    //new SelectListItem { Value = "PW", Text="Palau" },
                    new SelectListItem {Value = "PA", Text = "Pennsylvania"},
                    new SelectListItem {Value = "PR", Text = "Puerto Rico"},
                    new SelectListItem {Value = "RI", Text = "Rhode Island"},
                    new SelectListItem {Value = "SC", Text = "South Carolina"},
                    new SelectListItem {Value = "SD", Text = "South Dakota"},
                    new SelectListItem {Value = "TN", Text = "Tennessee"},
                    new SelectListItem {Value = "TX", Text = "Texas"},
                    new SelectListItem {Value = "UT", Text = "Utah"},
                    new SelectListItem {Value = "VT", Text = "Vermont"},
                    //new SelectListItem { Value = "VI", Text="Virgin Islands" },
                    new SelectListItem {Value = "VA", Text = "Virginia"},
                    new SelectListItem {Value = "WA", Text = "Washington"},
                    new SelectListItem {Value = "WV", Text = "West Virginia"},
                    new SelectListItem {Value = "WI", Text = "Wisonsin"},
                    new SelectListItem {Value = "WY", Text = "Wyoming"}
                };

            return selectListItems;

        }

        public void SendEmail()
        {
            MailDefinition md = new MailDefinition();
            md.From = "test@domain.com";
            md.IsBodyHtml = true;
            md.Subject = "Test of MailDefinition";

            ListDictionary replacements = new ListDictionary();
            replacements.Add("<%Name%>", "Martin");
            replacements.Add("<%Country%>", "Denmark");

            string body = "Hello <%Name%> You're from <%Country%>.";

            MailMessage msg = md.CreateMailMessage("patrick.wensel@as.willscot.com", replacements, body, new System.Web.UI.Control());

            SmtpClient smtp = new SmtpClient();
            smtp.Send(msg);

        }

        public string FormatUnitNumber(string unitNumber)
        {
            string[] unitNumberParts = unitNumber.Split(Convert.ToChar("-"));

            string firstPart = unitNumberParts[0].PadRight(3, Convert.ToChar(" ")).ToUpper();

            unitNumber = firstPart + "-" + unitNumberParts[1];

            return unitNumber;
        }
    }
}
