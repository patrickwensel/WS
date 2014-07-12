using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using NLog;
using WS.Framework.Objects.Emails;
using WS.Framework.ServicesInterface;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class EmailService : IEmailService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void AttributeChanges(UpdateAttributesEmail updateAttributesEmail)
        {
            updateAttributesEmail = CheckEmailOverride(updateAttributesEmail);

            string note = string.IsNullOrEmpty(updateAttributesEmail.Note) ? "" : updateAttributesEmail.Note;

            string smtpServer = ConfigurationManager.AppSettings.Get("SMTPServer");

            MailDefinition md = new MailDefinition
                {
                    From = "DoNotReply@as.willscot.com",
                    IsBodyHtml = true,
                    Subject = "Unit Attribute Changes for: " + updateAttributesEmail.UnitNumber
                };

            ListDictionary replacements = new ListDictionary
                {
                    {"<%BranchName%>", updateAttributesEmail.BranchName},
                    {"<%UnitNumber%>", updateAttributesEmail.UnitNumber},
                    {"<%Name%>", updateAttributesEmail.Name},
                    {"<%AttributChanges%>", updateAttributesEmail.AttributeChanges},
                    {"<%Note%>", note}
                };

            var emailPath = HostingEnvironment.MapPath("~/Content/Emails/");
            var imagePath = HostingEnvironment.MapPath("~/Content/Images/");

            string body = File.ReadAllText(emailPath + "AttributeChanges.html");

            EmbeddedMailObject emo1 = new EmbeddedMailObject { Path = imagePath + "MobileWorkHeader.jpg", Name = "header" };
            md.EmbeddedObjects.Add(emo1);

            EmbeddedMailObject emo2 = new EmbeddedMailObject { Path = imagePath + "EmailLogo.jpg", Name = "logo" };
            md.EmbeddedObjects.Add(emo2);

            MailMessage msg = md.CreateMailMessage(updateAttributesEmail.To, replacements, body, new System.Web.UI.Control());
           

            SmtpClient smtpClient = new SmtpClient(smtpServer);
            smtpClient.Send(msg);
        }

        public bool BOATransaction(BOATransactionEmail boaTransactionEmail)
        {
            bool success = true;

            try
            {
                string smtpServer = ConfigurationManager.AppSettings.Get("SMTPServer");

                MailDefinition md = new MailDefinition
                    {
                        From = "DoNotReply@as.willscot.com",
                        IsBodyHtml = true,
                        Subject = "Williams Scotsman Payment Confirmation"
                    };

                ListDictionary replacements = new ListDictionary
                    {
                        {"<%ConfirmationNumber%>", boaTransactionEmail.ConfirmationNumber},
                        {"<%AmountCharged%>", boaTransactionEmail.AmountCharged}
                    };

                var emailPath = HostingEnvironment.MapPath("~/Content/Emails/");
                var imagePath = HostingEnvironment.MapPath("~/Content/Images/");

                string body = File.ReadAllText(emailPath + "BOATransactionConfirmation.html");

                EmbeddedMailObject emo1 = new EmbeddedMailObject {Path = imagePath + "EmailHeader.jpg", Name = "header"};
                md.EmbeddedObjects.Add(emo1);

                EmbeddedMailObject emo2 = new EmbeddedMailObject {Path = imagePath + "EmailLogo.jpg", Name = "logo"};
                md.EmbeddedObjects.Add(emo2);

                MailMessage msg = md.CreateMailMessage(boaTransactionEmail.To, replacements, body,
                                                       new System.Web.UI.Control());

                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Send(msg);
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);

                success = false;
            }

            return success;
        }

        public void HoursIT(HoursITEmail hoursItEmail)
        {
            try
            {
                string smtpServer = ConfigurationManager.AppSettings.Get("SMTPServer");
                string HREmail = ConfigurationManager.AppSettings.Get("HREmail");

                string overrideEmail = ConfigurationManager.AppSettings.Get("OverrideEmail");
                if (overrideEmail == "Yes")
                {
                    hoursItEmail.Note = "In Production this email would go to: " + HREmail;
                    HREmail = ConfigurationManager.AppSettings.Get("OverrideEmailAddress");
                }

                string note = string.IsNullOrEmpty(hoursItEmail.Note) ? "" : hoursItEmail.Note;

                MailDefinition md = new MailDefinition
                {
                    From = "DoNotReply@as.willscot.com",
                    IsBodyHtml = true,
                    Subject = "IT Time Tracking Exception Hours for UTA"
                };

                ListDictionary replacements = new ListDictionary
                    {
                        {"<%Company%>",hoursItEmail.Company},
                        {"<%CompanyCode%>",hoursItEmail.CompanyCode},
                        {"<%UTACompanyCode%>", hoursItEmail.UTACompanyCode},
                        {"<%PayrollPeriodEnding%>",hoursItEmail.PayrollPeriod},
                        {"<%Note%>", note}
                    };

                string emailPath = ConfigurationManager.AppSettings.Get("EmailTemplatePath");
                string imagePath = ConfigurationManager.AppSettings.Get("ImagePath");

                string body = File.ReadAllText(emailPath + "IT_Hours_Email.html");

                EmbeddedMailObject emo1 = new EmbeddedMailObject { Path = imagePath + "EmailHeader.jpg", Name = "header" };
                md.EmbeddedObjects.Add(emo1);

                EmbeddedMailObject emo2 = new EmbeddedMailObject { Path = imagePath + "EmailLogo.jpg", Name = "logo" };
                md.EmbeddedObjects.Add(emo2);

                MailMessage msg = md.CreateMailMessage(HREmail, replacements, body, new System.Web.UI.Control());
                msg.Attachments.Add(new Attachment(hoursItEmail.Attachment));

                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Send(msg);
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
            
        }

        public void PaperlessRequest(PaperlessInvoiceEmail paperlessInvoiceEmail)
        {
            try
            {
                string smtpServer = ConfigurationManager.AppSettings.Get("SMTPServer");
                string paperlessInvoiceRequestEmail = ConfigurationManager.AppSettings.Get("PaperlessInvoiceRequestEmail");

                MailDefinition md = new MailDefinition
                {
                    From = "DoNotReply@as.willscot.com",
                    IsBodyHtml = true,
                    Subject = "Paperless Invoicing Request"
                };

                ListDictionary replacements = new ListDictionary
                    {
                        {"<%FirstName%>", paperlessInvoiceEmail.FirstName},
                        {"<%LastName%>", paperlessInvoiceEmail.LastName},
                        {"<%RequesterEmail%>", paperlessInvoiceEmail.RequesterEmail},
                        {"<%PhoneNumer%>", paperlessInvoiceEmail.PhoneNumer},
                        {"<%AccountNumber%>", paperlessInvoiceEmail.AccountNumber},
                        {"<%Email%>", paperlessInvoiceEmail.Email},
                        {"<%Units%>", paperlessInvoiceEmail.Units}
                    };

                var emailPath = HostingEnvironment.MapPath("~/Content/Emails/");
                var imagePath = HostingEnvironment.MapPath("~/Content/Images/");

                string body = File.ReadAllText(emailPath + "PaperlessInvoiceRequest.html");

                EmbeddedMailObject emo1 = new EmbeddedMailObject { Path = imagePath + "EmailHeader.jpg", Name = "header" };
                md.EmbeddedObjects.Add(emo1);

                EmbeddedMailObject emo2 = new EmbeddedMailObject { Path = imagePath + "EmailLogo.jpg", Name = "logo" };
                md.EmbeddedObjects.Add(emo2);

                MailMessage msg = md.CreateMailMessage(paperlessInvoiceRequestEmail, replacements, body,
                                                       new System.Web.UI.Control());

                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Send(msg);
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
        }

        public void SafetyIncident(SafetyIncidentEmail safetyIncidentEmail)
        {
            try
            {
                string smtpServer = ConfigurationManager.AppSettings.Get("SMTPServer");

                MailDefinition md = new MailDefinition
                {
                    From = "DoNotReply@algecoscotsman.com",
                    IsBodyHtml = true,
                    Subject = "Safety Incident Submitted/Updated"
                };

                string recoradable;

                switch (safetyIncidentEmail.Recordable)
                {
                    case "1":
                        recoradable = "Yes";
                        break;
                    case "0":
                        recoradable = "No";
                        break;
                    default:
                        recoradable = "";
                        break;
                }

                string workRelated;

                switch (safetyIncidentEmail.WorkRelated)
                {
                    case "1":
                        workRelated = "Yes";
                        break;
                    case "0":
                        workRelated = "No";
                        break;
                    default:
                        workRelated = "";
                        break;
                }

                ListDictionary replacements = new ListDictionary
                    {
                        {"<%CreatedBy%>", safetyIncidentEmail.CreatedBy},
                        {"<%DateOfIncident%>", safetyIncidentEmail.DateOfIncident != null ? safetyIncidentEmail.DateOfIncident.Value.ToString("D") : "N/A"},
                        {"<%Country%>", safetyIncidentEmail.Country ?? ""},
                        {"<%Location%>", safetyIncidentEmail.Location ?? ""},
                        {"<%EmployeeType%>", safetyIncidentEmail.EmployeeType ?? ""},
                        {"<%WorkRelated%>", workRelated ?? ""},
                        {"<%DescriptionOfIncident%>", safetyIncidentEmail.DescriptionOfIncident ?? ""},
                        {"<%Outcome%>", safetyIncidentEmail.Outcome ?? ""},
                        {"<%UnsafeActCondition%>", safetyIncidentEmail.UnsafeActCondition ?? ""},
                        {"<%RootCase%>", safetyIncidentEmail.RootCase ?? ""},
                        {"<%Recordable%>", recoradable ?? ""},
                        {"<%ContactMessage%>", safetyIncidentEmail.ContactMessage ?? ""},
                        {"<%Status%>", safetyIncidentEmail.Status ?? ""},
                        {"<%ID%>", safetyIncidentEmail.ID.ToString() ?? ""},
                        {"<%EmailMessage%>", safetyIncidentEmail.EmailMessage ?? ""},
                    };

                var emailPath = HostingEnvironment.MapPath("~/Content/Email/");
                var imagePath = HostingEnvironment.MapPath("~/Content/Images/");

                string body = File.ReadAllText(emailPath + "SafetyIncident.html");

                EmbeddedMailObject emo1 = new EmbeddedMailObject { Path = imagePath + "as_email_header.jpg", Name = "header" };
                md.EmbeddedObjects.Add(emo1);

                EmbeddedMailObject emo2 = new EmbeddedMailObject { Path = imagePath + "as_email_icon.png", Name = "logo" };
                md.EmbeddedObjects.Add(emo2);

                MailMessage msg = md.CreateMailMessage(safetyIncidentEmail.ToAddress, replacements, body,
                                                       new System.Web.UI.Control());

                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Send(msg);
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }
        }

     
        private UpdateAttributesEmail CheckEmailOverride(UpdateAttributesEmail updateAttributesEmail)
        {
            string overrideEmail = ConfigurationManager.AppSettings.Get("OverrideEmail");

            if (overrideEmail == "Yes")
            {
                updateAttributesEmail.Note = "In Production this email would go to: " + updateAttributesEmail.To;
                updateAttributesEmail.To = ConfigurationManager.AppSettings.Get("OverrideEmailAddress");
            }
            return updateAttributesEmail;
        }
    }
}
