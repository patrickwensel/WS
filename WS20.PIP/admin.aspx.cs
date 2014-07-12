using Plumtree.Remote.Portlet;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PIP
{
    public partial class admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common aCommon = new common(Application, Request.Form["selLang"], PortletContextFactory.CreatePortletContext(Request, Response));
            

            //team members sel
            selOneTeamMembers.DataSource = (DataTable)Application["PeopleSel"];
            selOneTeamMembers.DataTextField = "full_name";
            selOneTeamMembers.DataValueField = "objectid";
            selOneTeamMembers.DataBind();
            //inputProjectOwner.Text = Request.Form["inputProjectOwner"] == "" ? aCommon.getUserName() : Request.Form["inputProjectOwner"];
            selOneTeamMembers.Items.Insert(0, new ListItem("", ""));
            selOneTeamMembers.SelectedValue = Request.Form["selTeamMembers"] != null ? Request.Form["selTeamMembers"] : "";

            buttonSaveAccess.Text = aCommon.getText("saveButton");

            aCommon.cleanUp();
        }

        protected void buttonSaveAccess_click(object sender, EventArgs e)
        {
            common aCommon = new common(Application, Request.Form["selLang"], PortletContextFactory.CreatePortletContext(Request, Response));
            
            //WEB_SECURITY_ID_SEQ
            //probably should just do this in page load but feeling lazy
            //common aCommon = new common(Application, "ENG", PortletContextFactory.CreatePortletContext(Request, Response));
            //aCommon.loadApp();
            //aCommon.cleanUp();


//            if (mLogging)
//            {
//                logError("common.cs", "setLang1", theLang, "", "LOG");
//            }


//            OracleDataReader aDR = GetReader("SELECT last_lang, pip_admin FROM pip_user_info WHERE user_id = '" + mUser + "'");
//            if (aDR != null && aDR.Read())
//            {
//                mLang = (theLang == null || theLang == "") ? aDR["last_lang"].ToString() : theLang;
//                mAdmin = aDR["pip_admin"].ToString() == "Y" ? true : false;
//                aDR.Close();
//            }
//            else
//            {
//                mLang = ConfigurationManager.AppSettings["defaultLang"];
//                mAdmin = false;
//            }


//            ArrayList aAL = new ArrayList(1);
//            aAL.Add(@"MERGE INTO pip_user_info i
//                            USING (SELECT '" + mUser + @"' user_id FROM dual) d
//                            ON (i.user_id = d.user_id)
//                            WHEN MATCHED THEN
//                              UPDATE
//                                 SET last_lang    = '" + theLang + @"'
//                                    ,last_login = SYSDATE
//                            WHEN NOT MATCHED THEN
//                              INSERT
//                                (user_id
//                                ,last_lang
//                                ,last_login)
//                              VALUES
//                                (d.user_id
//                                ,'" + theLang + @"'
//                                ,SYSDATE)");
//            ExecuteCommand(aAL);


//            if (mLogging)
//            {
//                logError("common.cs", "setLang2", theLang, "", "LOG");
//            }

//            return theLang;
            aCommon.cleanUp();
        }

        /// <summary>
        /// Logs error 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void HandleError(object sender, AsyncPostBackErrorEventArgs e)
        {
            //long mCurrTicks =  DateTime.Now.Ticks;

            //common aCommon = new common(Application, "ENG", null);
           // aCommon.logError("admin.aspx.cs", "HandleError", "", e.Exception.ToString(), "ERROR");
           // aCommon.cleanUp();

        }
    }
}
