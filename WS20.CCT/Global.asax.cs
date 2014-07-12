using System;
using WS20.CCT.App_Data;

namespace WS20.CCT
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
            //Common aCommon = new Common();
            //aCommon.loadApplication(this.Application);
		}

		protected void Application_End(object sender, EventArgs e)
		{

		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{
//            WSDBConnection aConn = new WSDBConnection();
//            aConn.Open("SISR", 0);
//            aConn.ExecuteCommand(string.Format(@"	INSERT INTO cct_session
//													  (id
//													  ,msg
//													  ,maint_user_id
//													  ,maint_date)
//													VALUES
//													  (CCT_SESSION_SEQ.nextval
//													  ,'End   - {0}'
//													  ,'{1}'
//													  ,SYSDATE)
//												"
//                                                , Session.SessionID
//                                                , Request.ServerVariables["LOGON_USER"].Replace("WS\\", "")
//                                                ));
//            aConn.Close();
		}

		protected void Application_Error(Object sender, EventArgs e)
		{

			Exception objErr = Server.GetLastError().GetBaseException();

			string err = "Error Caught in Application_Error event\n" +

				  "Error in: " + Request.Url.ToString() +

				  "\nError Message:" + objErr.Message.ToString() +

				  "\nStack Trace:" + objErr.StackTrace.ToString();

			Common mCommon = new Common();
			mCommon.logError("Global.aspx", "Application_Error", "", "", err, "ERROR");

			//Server.ClearError();
		}
	}
}