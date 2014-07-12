using System;
using System.Collections.Generic;
using System.Text;

namespace WS20.SafetyAudit
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
			
			mLink.Attributes.Add("href", "\\include\\style.css"); 
			//WriteAppMenuHeader(Request.ServerVariables["HTTP_USER_AGENT"].ToString());
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

	    public void AddMenuItem(Dictionary<string, string> links)
	    {
	        StringBuilder str = new StringBuilder();

	        foreach (KeyValuePair<string, string> kvp in links)
	        {
                str.Append(String.Format(@"<a href=""{0}"" onClick=""{2}"">{1}</a> |", kvp.Value, kvp.Key,null));
	        }
	        menuCenterContainer.InnerHtml = str.ToString();

	    }

	    public void AddMenuItem(string theMenuItem, string theURL)
		{
			AddMenuItem(theMenuItem, theURL, "");
		}

		public void AddMenuItem(string theMenuItem, string theURL, string theOnClick)
		{
		    menuCenterContainer.InnerHtml = "";

			mHasAppMenu = true;
			mAppMenuText = String.Format(@"{0}<li><a href=""{1}"" onClick=""{3}"">{2}</a></li>", mAppMenuText , theURL, theMenuItem, theOnClick);
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
