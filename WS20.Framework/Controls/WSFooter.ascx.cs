using System;
using System.Configuration;
using System.Web;
using System.Text;
using System.IO;

namespace WS20.Framework.Controls
{
	/// <summary>
	///		Summary description for WSFooter.
	/// </summary>
	public partial class WSFooter : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			WriteAppMenuFooter(Request.ServerVariables["HTTP_USER_AGENT"].ToString());
			mAppFooter.InnerHtml = string.Format("© {0} Williams Scotsman Inc.", DateTime.Now.Year);
			//show debug info (for devl)			
			if(Session["showdebug"] != null)
			{
				if(Session["showdebug"].ToString() == "Y")
				{
					//populate debug info			
					mAppDebugInfo.InnerHtml = String.Format("<br><br><hr>{0}<br><br>{1}<br><br>{3}<br><br>{2}",
						WriteFormVarsDump(Request), WriteUrlVarsDump(Request), WriteServerVariables(Request), WriteSessionVars());				
					ShowDebugInfo();
				}
			}

			//ShowDebugInfo();
		}

		public string AppFooterText
		{
			get
			{
				return mAppFooter.InnerHtml;
			}
			set
			{
				mAppFooter.InnerHtml = value;
			}
		}

		private void WriteAppMenuFooter(string aUserAgent)
		{
			try
			{
				string aMenuHeader = "";
				string aFileName = "";

				if (aUserAgent.IndexOf("MSIE 5") >= 0)
				{
					aFileName = ConfigurationSettings.AppSettings["gFooterFileMenu55"];
				}
				else
				{
					aFileName = ConfigurationSettings.AppSettings["gFooterFileMenu"];
				}

				StreamReader aReader = new StreamReader(String.Format(@"{0}\{1}", ConfigurationSettings.AppSettings["gHeaderFileLoc"], aFileName));

				aMenuHeader = aReader.ReadToEnd();

				aReader.Close();

				mAppMenuFooter.InnerHtml = aMenuHeader;

			}
			catch
			{
				mAppMenuFooter.InnerHtml = "Error loading footer, check files and try again.";
			}
		}

		public void ShowDebugInfo()
		{
			mAppDebugInfo.Attributes.Remove("style");
		}

		public void SetFooterHtml(string theHTML)
		{
			mAppFooter.InnerHtml = theHTML;
		}

		public string WriteFormVarsDump(HttpRequest theRequest)
		{
			StringBuilder aDumpTable = new StringBuilder();
			aDumpTable.Append("<table border='1'><tr><td style='background:#666666;color:white;' colspan='2'><b>Form Variables</b></td></tr>");
			int aFormVarsCount = theRequest.Form.Count;
			for(int i=1;i<aFormVarsCount;i++) //start with 1 to avoid stupid viewstate
			{
				aDumpTable.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", theRequest.Form.Keys[i].ToString(), theRequest.Form[i].ToString());
			}

			aDumpTable.Append("</table>");

			return aDumpTable.ToString();
		}

		public string WriteUrlVarsDump(HttpRequest theRequest)
		{
			StringBuilder aDumpTable = new StringBuilder();
			aDumpTable.Append("<table border='1'><tr><td style='background:#666666;color:white;' colspan='2'><b>URL Variables</b></td></tr>");
			int aVarsCount = theRequest.QueryString.Count;
			for(int i=0;i<aVarsCount;i++)
			{
				try 
				{
					aDumpTable.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", theRequest.QueryString.Keys[i].ToString(), theRequest.QueryString[i].ToString());
				}
				catch 
				{
					aDumpTable.Append("<tr><td><b>-unknown variable</b></td><td>null</td></tr>");
				}
			}

			aDumpTable.Append("</table>");

			return aDumpTable.ToString();
		}

		public string WriteServerVariables(HttpRequest theRequest)
		{	
			StringBuilder aDumpTable = new StringBuilder();
			aDumpTable.Append("<table border='1'><tr><td style='background:#666666;color:white;' colspan='2'><b>Server Variables</b></td></tr>");
			int aVarsCount = Request.ServerVariables.Count;
			for(int i=2;i<aVarsCount;i++) //start at 2 to skip the ALL_ ones
			{
				aDumpTable.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", theRequest.ServerVariables.Keys[i].ToString(), theRequest.ServerVariables[i].ToString());
			}

			aDumpTable.Append("</table>");

			return aDumpTable.ToString();
		}

		public string WriteSessionVars()
		{	

			StringBuilder aDumpTable = new StringBuilder();
			aDumpTable.Append("<table border='1'><tr><td style='background:#666666;color:white;' colspan='2'><b>Session Variables</b></td></tr>");
			int aVarsCount = Session.Count;
			for(int i=0;i<aVarsCount;i++) 
			{
				try
				{
					aDumpTable.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", Session.Keys[i].ToString(), Session[i].ToString());
				}
				catch
				{
					//dont care
				}				
			}

			aDumpTable.Append("</table>");

			return aDumpTable.ToString();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
