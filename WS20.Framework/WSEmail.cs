using System;
using System.Net.Mail;
using System.Text;
using System.Configuration;

namespace WS20.Framework
{
    public class WSEmail
    {
        private MailMessage mMessage;
        string mAppName;
        string mCustMessage;


        /// <summary>
        /// Create a <c>WSEmail</c> object
        /// <remarks><code>Send()</code> must be called before the email is delivered</remarks>
        /// <param name="theAppName">Pass an Application name for error logging</param>
        /// </summary>
        public WSEmail(string theAppName)
        {
            mMessage = new MailMessage();
            mAppName = theAppName;
            mCustMessage = "";
        }

        /// <summary>
        /// Create a <c>WSEmail</c> object
        /// <remarks><code>Send()</code> must be called before the email is delivered</remarks>
        /// <param name="theAppName">Pass an Application name for error logging</param>
        /// <param name="theCustMessage">Pass a Custom Message to identify emails for an Application</param>
        /// </summary>
        public WSEmail(string theAppName, string theCustMessage)
        {
            mMessage = new MailMessage();
            mAppName = theAppName;
            mCustMessage = theCustMessage;
        }

        /// <summary>
        /// Create a <c>WSEmail</c> object
        /// <remarks><code>Send()</code> must be called before the email is delivered</remarks>
        /// </summary>
        /// <param name="theAppName">Pass an Application name for error logging</param>
        /// <param name="theCustMessage">Pass a Custom Message to identify emails for an Application</param>
        /// <param name="theSubject">The email subject</param>
        /// <param name="theSender">The email sender</param>
        /// <param name="theRecipient">The email recipient</param>
        /// <param name="theBody">The email body</param>
        public WSEmail(string theAppName, string theCustMessage, string theSubject, MailAddress theSender, MailAddress theRecipient, string theBody)
        {
            mAppName = theAppName;
            mCustMessage = theCustMessage;
            mMessage = new MailMessage();
            mMessage.Subject = theSubject;
            mMessage.From = theSender;
            mMessage.To.Add(theRecipient);
            mMessage.Body = theBody;
        }

        /// <summary>
        /// Create a <c>WSEmail</c> object
        /// <remarks><code>Send()</code> must be called before the email is delivered</remarks>
        /// </summary>
        /// <param name="theAppName">Pass an Application name for error logging</param>
        /// <param name="theCustMessage">Pass a Custom Message to identify emails for an Application</param>
        /// <param name="theSubject">The email subject</param>
        /// <param name="theSender">The email sender</param>
        /// <param name="theRecipient">The email recipient</param>
        /// <param name="theBody">The email body</param>
        public WSEmail(string theAppName, string theCustMessage, string theSubject, string theSender, string theRecipient, string theBody)
        {
            MailAddress aSender = new MailAddress(theSender);
            MailAddress aRecipient = new MailAddress(theRecipient);

            mAppName = theAppName;
            mCustMessage = theCustMessage;
            mMessage = new MailMessage();
            mMessage.Subject = theSubject;
            mMessage.From = aSender;
            mMessage.To.Add(aRecipient);
            mMessage.Body = theBody;
        }

        /// <summary>
        /// Create a <c>WSEmail</c> object
        /// <remarks><code>Send()</code> must be called before the email is delivered</remarks>
        /// </summary>
        /// <param name="theAppName">Pass an Application name for error logging</param>
        /// <param name="theCustMessage">Pass a Custom Message to identify emails for an Application</param>
        /// <param name="theSubject">The email subject</param>
        /// <param name="theSender">The email sender</param>
        /// <param name="theRecipient">The email recipient</param>
        /// <param name="theBody">The email body</param>
        public WSEmail(string theAppName, string theCustMessage, string theSubject, MailAddress theSender, MailAddress[] theRecipient, string theBody)
        {
            mAppName = theAppName;
            mCustMessage = theCustMessage;
            mMessage = new MailMessage();
            mMessage.Subject = theSubject;
            mMessage.From = theSender;
            for (int i = 0; i < theRecipient.Length; i++)
                mMessage.To.Add(theRecipient[i]);
            mMessage.Body = theBody;
        }

        /// <summary>
        /// Create a <c>WSEmail</c> object
        /// <remarks><code>Send()</code> must be called before the email is delivered</remarks>
        /// </summary>
        /// <param name="theAppName">Pass an Application name for error logging</param>
        /// <param name="theCustMessage">Pass a Custom Message to identify emails for an Application</param>
        /// <param name="theSubject">The email subject</param>
        /// <param name="theSender">The email sender</param>
        /// <param name="theRecipient">The email recipient</param>
        /// <param name="theBody">The email body</param>
        public WSEmail(string theAppName, string theCustMessage, string theSubject, string theSender, string[] theRecipient, string theBody)
        {
            MailAddress aSender = new MailAddress(theSender);

            mAppName = theAppName;
            mCustMessage = theCustMessage;
            mMessage = new MailMessage();
            mMessage.Subject = theSubject;
            mMessage.From = aSender;
            for (int i = 0; i < theRecipient.Length; i++)
            {
                MailAddress aRecipient = new MailAddress(theRecipient[i]);
                mMessage.To.Add(theRecipient[i]);
            }
            mMessage.Body = theBody;
        }

        /// <summary>
        /// Add an attachment
        /// </summary>
        /// <param name="thePath">The web path to the attachment</param>
        public void AddAttachment(string thePath)
        {
            Attachment anAttach = new Attachment(thePath);
            mMessage.Attachments.Add(anAttach);
        }

        /// <summary>
        /// Set the email subject
        /// </summary>
        /// <param name="theSubject">The email subject</param>
        public void SetSubject(string theSubject)
        {
            mMessage.Subject = theSubject;
        }

        /// <summary>
        /// Set the email sender
        /// </summary>
        /// <param name="theSender">The email sender</param>
        public void SetSender(MailAddress theSender)
        {
            mMessage.From = theSender;
        }

        /// <summary>
        /// Set the email sender
        /// </summary>
        /// <param name="theSender">The email sender</param>
        public void SetSender(string theSender)
        {
            MailAddress aSender = new MailAddress(theSender);
            mMessage.From = aSender;
        }

        /// <summary>
        /// Add a recipient to the email
        /// </summary>
        /// <param name="theRecipient">The recipient to be added</param>
        public void AddRecipient(MailAddress theRecipient)
        {
            mMessage.To.Add(theRecipient);
        }

        /// <summary>
        /// Add a recipient to the email
        /// </summary>
        /// <param name="theRecipient">The recipient to be added</param>
        public void AddRecipient(string theRecipient)
        {
            MailAddress aRecipient = new MailAddress(theRecipient);
            mMessage.To.Add(aRecipient);
        }

        /// <summary>
        /// Set the email's recipient
        /// <remarks>
        /// This clears the current list of recipients before replacing it with <c>theRecipient</c>
        /// </remarks>
        /// </summary>
        /// <param name="theRecipient">The recipient to be set</param>
        public void SetRecipient(MailAddress theRecipient)
        {
            ClearRecipients();
            AddRecipient(theRecipient);
        }

        /// <summary>
        /// Set the email's recipient
        /// <remarks>
        /// This clears the current list of recipients before replacing it with <c>theRecipient</c>
        /// </remarks>
        /// </summary>
        /// <param name="theRecipient">The recipient to be set</param>
        public void SetRecipient(string theRecipient)
        {
            MailAddress aRecipient = new MailAddress(theRecipient);
            ClearRecipients();
            AddRecipient(aRecipient);
        }

        /// <summary>
        /// Clears the email's recipients
        /// </summary>
        public void ClearRecipients()
        {
            mMessage.To.Clear();
        }

        /// <summary>
        /// Adds to the email's body text
        /// </summary>
        /// <param name="theBodyAdd">The text to be added to the body</param>
        public void AddToBody(string theBodyAdd)
        {
            mMessage.Body += theBodyAdd;
        }

        /// <summary>
        /// Sets the email's body text
        /// <remarks>
        /// This clears the email's current body before replacing it with <c>theBody</c>
        /// </remarks>
        /// </summary>
        /// <param name="theBody">The body text to be set</param>
        public void SetBody(string theBody)
        {
            mMessage.Body = theBody;
        }

        /// <summary>
        /// Clears the email's body text
        /// </summary>
        public void ClearBody()
        {
            mMessage.Body = "";
        }

        /// <summary>
        /// Sets the email's blind carbon copy recipient
        /// <remarks>
        /// This clears the email's current BCC list before replacing it with <c>theBCC</c>
        /// </remarks>
        /// </summary>
        /// <param name="theBCC">The BCC recipient to be set</param>
        public void SetBCC(MailAddress theBCC)
        {
            ClearBCC();
            AddBCC(theBCC);
        }

        /// <summary>
        /// Sets the email's blind carbon copy recipient
        /// <remarks>
        /// This clears the email's current BCC list before replacing it with <c>theBCC</c>
        /// </remarks>
        /// </summary>
        /// <param name="theBCC">The BCC recipient to be set</param>
        public void SetBCC(string theBCC)
        {
            MailAddress aBCC = new MailAddress(theBCC);
            ClearBCC();
            AddBCC(aBCC);
        }

        /// <summary>
        /// Adds a blind carbon copy recipient
        /// </summary>
        /// <param name="theBCCAdd">The BCC recipient to be added</param>
        public void AddBCC(MailAddress theBCCAdd)
        {
            mMessage.Bcc.Add(theBCCAdd);
        }

        /// <summary>
        /// Adds a blind carbon copy recipient
        /// </summary>
        /// <param name="theBCCAdd">The BCC recipient to be added</param>
        public void AddBCC(string theBCCAdd)
        {
            MailAddress aBCCAdd = new MailAddress(theBCCAdd);
            mMessage.Bcc.Add(aBCCAdd);
        }

        /// <summary>
        /// Clears an email's blind carbon copy list
        /// </summary>
        public void ClearBCC()
        {
            mMessage.Bcc.Clear();
        }

        /// <summary>
        /// Sets the email's  carbon copy recipient
        /// <remarks>
        /// This clears the email's current CC list before replacing it with <c>theCC</c>
        /// </remarks>
        /// </summary>
        /// <param name="theCC">The CC recipient to be set</param>
        public void SetCC(MailAddress theCC)
        {
            ClearCC();
            AddCC(theCC);
        }

        /// <summary>
        /// Sets the email's  carbon copy recipient
        /// <remarks>
        /// This clears the email's current CC list before replacing it with <c>theCC</c>
        /// </remarks>
        /// </summary>
        /// <param name="theCC">The CC recipient to be set</param>
        public void SetCC(string theCC)
        {
            MailAddress aCC = new MailAddress(theCC);
            ClearCC();
            AddCC(aCC);
        }

        /// <summary>
        /// Adds a carbon copy recipient
        /// </summary>
        /// <param name="theCCAdd">The CC recipient to be added</param>
        public void AddCC(MailAddress theCCAdd)
        {
            mMessage.CC.Add(theCCAdd);
        }

        /// <summary>
        /// Adds a carbon copy recipient
        /// </summary>
        /// <param name="theCCAdd">The CC recipient to be added</param>
        public void AddCC(string theCCAdd)
        {
            MailAddress aCCAdd = new MailAddress(theCCAdd);
            mMessage.CC.Add(aCCAdd);
        }

        /// <summary>
        /// Clears an email's carbon copy list
        /// </summary>
        public void ClearCC()
        {
            mMessage.CC.Clear();
        }

        /// <summary>
        /// Add an email header
        /// </summary>
        /// <param name="theHeaderName">The email header name</param>
        /// <param name="theHeaderValue">The email header value</param>
        public void AddHeader(string theHeaderName, string theHeaderValue)
        {
            mMessage.Headers.Add(theHeaderName, theHeaderValue);
        }

        /// <summary>
        /// Clears the email's headers
        /// </summary>
        public void ClearHeaders()
        {
            mMessage.Headers.Clear();
        }

        /// <summary>
        /// Add a read receipt to the email
        /// </summary>
        /// <param name="theReceiptRecipient">The email address of the read receipt's recipient</param>
        public void AddReadReceipt(string theReceiptRecipient)
        {
            AddHeader("Read-Receipt-To", theReceiptRecipient);
            AddHeader("Disposition-Notification-To", theReceiptRecipient);
        }

        /// <summary>
        /// Sets whether or not the email's body contains HTML markup
        /// </summary>
        /// <param name="hasHTML">Pass true if the message contains HTML</param>
        public void SetHTML(bool hasHTML)
        {
            mMessage.IsBodyHtml = hasHTML;
        }

        /// <summary>
        /// Adds an Alternate View
        /// </summary>
        /// <param name="theItem">The AlternateView</param>
        public void AddAlternateView(AlternateView theItem)
        {
            mMessage.AlternateViews.Add(theItem);
        }

        /// <summary>
        /// Clears the AlternateViews
        /// </summary>
        public void ClearAlternateView(AlternateView theItem)
        {
            mMessage.AlternateViews.Clear();
        }

        /// <summary>
        /// Sends the email
        /// </summary>
        /// <remarks>Inserts a row into **** indicating success or failure</remarks>
        /// <returns>Returns true upon success, else false</returns>
        public bool Send()
        {
            if (mMessage.To.Count == 0)
                return false;

            WSDBConnection aConn = new WSDBConnection();
            aConn.Open("JDE_WEB");
            bool retVal = false;
            try
            {
                mMessage.BodyEncoding = Encoding.GetEncoding("utf-8");

                //SmtpClient aSmtpClient = new SmtpClient("WSEmailPackage");
                //SmtpClient aSmtpClient = new SmtpClient("WSEmail");
                //SmtpClient aSmtpClient = new SmtpClient("wsmail");
                
                //JLawrence - get the SMTP settings from the config file
                SmtpClient aSmtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);

                aSmtpClient.Send(mMessage);

                // Insert success
                aConn.ExecuteCommand(
                        string.Format(@"INSERT INTO web_email
								  (id
								  ,application
								  ,email_from
								  ,email_to
								  ,email_cc
								  ,email_bcc
								  ,email_subject
								  ,status
								  ,custom
								  ,sent_date)
								VALUES
								  (web_email_seq.nextval
									,'{0}'
									,'{1}'
									,'{2}'
									,'{3}'
									,'{4}'
									,'{5}'
									,'Message Sent'
									,'{6}'
									,sysdate
									)"
                                        , mAppName
                                        , mMessage.From.ToString()
                                        , mMessage.To.ToString()
                                        , mMessage.CC.ToString()
                                        , mMessage.Bcc.ToString()
                                        , mMessage.Subject.ToString()
                                        , mCustMessage));
                retVal = true;
            }
            catch (Exception e)
            {
                // Insert failure
                //WSStringManip wsStringManip = new WSStringManip();

                aConn.ExecuteCommand(
                    string.Format(@"INSERT INTO web_email
								  (id
								  ,application
								  ,email_from
								  ,email_to
								  ,email_cc
								  ,email_bcc
								  ,email_subject
								  ,status
								  ,custom
								  ,sent_date)
								VALUES
								  (web_email_seq.nextval
									,'{0}'
									,'{1}'
									,'{2}'
									,'{3}'
									,'{4}'
									,'{5}'
									,'{6}'
									,'{7}'
									,sysdate
									)"
                                    , mAppName
                                    , mMessage.From.ToString()
                                    , mMessage.To.ToString()
                                    , mMessage.CC.ToString()
                                    , mMessage.Bcc.ToString()
                                    , mMessage.Subject.ToString().Replace("'","''")
//                    //,WSStringManip.WSString.FilterSQL(e.Message)
//                    //,WSStringManip.WSString.FilterSQL(e.ToString())
                                    , e.Message.ToString().Replace("'", "''")
                                    , e.ToString().Replace("'", "''")
                                    ));
            }
            finally
            {
                if (aConn != null) { aConn.Close(); }
                mMessage.Attachments.Dispose();
                mMessage.Dispose();
            }

            return retVal;
        }
    }
}
