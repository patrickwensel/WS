using System;
using System.Configuration;
using System.Text;
using System.IO;

namespace WS20.Framework.Controls
{
	/// <summary>
	///	Creates an application header
	/// </summary>
	public partial class WSHeader : System.Web.UI.UserControl
	{
		private string	mPageName;
		private bool	mHasAppMenu = false;
		private string	mAppMenuText;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			mLink.Attributes.Add("href", ConfigurationSettings.AppSettings["gStyleSheet"].ToString()); 
			WriteAppMenuHeader(Request.ServerVariables["HTTP_USER_AGENT"].ToString());
		}

		public string AppLogoSrc
		{
			get
			{
				return mAppLogo.Src;
			}
			set
			{
				mAppLogo.Src = value;
			}
		}

		public int AppLogoHeight
		{
			get
			{
				return mAppLogo.Height;
			}
			set
			{
				mAppLogo.Height = value;
			}
		}

		public int AppLogoWidth
		{
			get
			{
				return mAppLogo.Width;
			}
			set
			{
				mAppLogo.Width = value;
			}
		}

		public string PageName
		{
			get
			{
				return	mPageName;
			}
			set
			{
				mPageName = value;
				mAppPageName.InnerHtml = mPageName;
			}
		}

		public string AppMenuHtml
		{
			get
			{
				return mAppMenuText;
			}
		}
		public void AddMenuItem(string theMenuItem, string theURL)
		{
			AddMenuItem(theMenuItem, theURL, "");
		}

		public void AddMenuItem(string theMenuItem, string theURL, string theOnClick)
		{
			mHasAppMenu = true;
			mAppMenuText = String.Format(@"{0}
				<li><a href=""{1}"" onClick=""{3}"">{2}</a></li>"
				, mAppMenuText
				, theURL
				, theMenuItem
				, theOnClick);
		}

		private void WriteAppMenuHeader(string aUserAgent)
		{
			StringBuilder aMenuHeader = new StringBuilder();
			string aFileName = "";
			
			try 
			{
				if (aUserAgent.IndexOf("MSIE") >= 0)
				{
					if (aUserAgent.Substring(aUserAgent.IndexOf("MSIE") + 5, 1) == "6")
					{
						aFileName = ConfigurationSettings.AppSettings["gHeaderFileIE6"];
					}
					else
					{
						aFileName = ConfigurationSettings.AppSettings["gHeaderFileIE55"];
					}
				}
				else if (aUserAgent.IndexOf("Mozilla") >= 0)
				{
					aFileName = ConfigurationSettings.AppSettings["gHeaderFileMozilla"];
				}

				StreamReader aReader = new StreamReader(String.Format(@"{0}\{1}", ConfigurationSettings.AppSettings["gHeaderFileLoc"], aFileName));

				while (!aReader.EndOfStream)
				{
					string aTmp = aReader.ReadLine();
					if (mHasAppMenu && aTmp.Trim() == "<!--[[APPMENUHERE]]-->")
					{
						aMenuHeader.Append(mAppMenuText);
					}
					else
					{
						aMenuHeader.Append(aTmp);
					}
				}

				aReader.Close();
				mAppMenuBar.InnerHtml = aMenuHeader.ToString();
				
			}
			catch 
			{
				mAppMenuBar.InnerHtml = "Error Creating Header, check file paths and try again.";
			}
		}

		public void SetAppTitleHtml(string theHTML)
		{
			mAppTitle.InnerHtml = theHTML;
		}

		public void SetSearchBoxHtml(string theHTML)
		{
			mAppSearchBox.InnerHtml = theHTML;
		}

		public void SetPageName(string thePageName)
		{
			mAppPageName.InnerHtml = thePageName;
		}

		public void SetHeaderImageSrc(string theSrc)
		{
			mAppLogo.Src = theSrc;
		}

		public void AddAppTitleStyle(string theNewStyle)
		{
			mAppTitle.Attributes.Add("style",theNewStyle);
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
