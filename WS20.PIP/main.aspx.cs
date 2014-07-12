using Plumtree.Remote.Portlet;
using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using Devart.Data.Oracle;

namespace PIP
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            common aCommon = new common(Application, Request.Form["selLang"], PortletContextFactory.CreatePortletContext(Request, Response));
            
            #region log

            if (aCommon.getLogging())
            {
                aCommon.logError("main.aspx.cs", "Page_Load",
                                    String.Format("username:{0}<br>lang:{1}<br>postback:{2}<br>selProjStatus:'{3}'"
                                   , aCommon.getUser()
                                   , aCommon.getLang()
                                   , Page.IsPostBack.ToString()
                                   , Request.Form["selProjStatus"]
                                   )
                                   , "", "LOG");
            }

            #endregion

            #region load text & dropdowns

            imgHeader.Src = "images/header" + aCommon.getLang() + ".jpg";  
            //search bar
            labelLang.InnerHtml = aCommon.getText("labelLang");
            labelSearchProjectTitle.InnerHtml = aCommon.getText("labelSearchProjectTitle");
            buttonSearch.Value = aCommon.getText("buttonSearch");
            //app msg
            appMsg.InnerHtml = aCommon.getText("appMsg");
            //titles
            labelLinksTitle.InnerHtml = aCommon.getText("labelLinksTitle");
            labelReportTitle.InnerHtml = aCommon.getText("labelReportTitle");
            labelAdminTitle.InnerHtml = aCommon.getText("labelAdminTitle");
            labelDocLinks.InnerHtml = aCommon.getText("labelDocLinks");
            labelSearchTitle.InnerHtml = aCommon.getText("labelMyProjectsBefore")+aCommon.getUserName()+" " + aCommon.getText("labelMyProjectsEnd"); 
            //links
            linkEnterUpdate.InnerHtml = aCommon.getText("linkEnterUpdate");
            linkEnterUpdate.HRef = ConfigurationManager.AppSettings["indexPage"];
            //linkAdminPage.InnerHtml = aCommon.getText("linkAdminPage");
            //linkAdminPage.HRef = ConfigurationManager.AppSettings["adminPage"];
            

            linkPlans.InnerHtml = aCommon.getText("linkPlans");
            linkPlans.HRef = ConfigurationManager.AppSettings["plansReportPage"] + ConfigurationManager.AppSettings["reportPW_" + aCommon.getLang()];
            linkGraph.InnerHtml = aCommon.getText("linkGraph");
            linkGraph.HRef = ConfigurationManager.AppSettings["graphReportPage"] + ConfigurationManager.AppSettings["reportPW_" + aCommon.getLang()];
            linkSearch.InnerHtml = aCommon.getText("linkSearch");
            linkSearch.HRef = ConfigurationManager.AppSettings["searchReportPage"] + ConfigurationManager.AppSettings["reportPW_" + aCommon.getLang()];
            //docs
            linkAccessingDoc.InnerHtml = aCommon.getText("linkAccessingDoc");
            linkAccessingDoc.HRef = ConfigurationManager.AppSettings["accessDoc"];
            linkEnterProjectDoc.InnerHtml = aCommon.getText("linkEnterProjectDoc");
            linkEnterProjectDoc.HRef = ConfigurationManager.AppSettings["projectDoc"];
            linkReportDoc.InnerHtml = aCommon.getText("linkReportDoc");
            linkReportDoc.HRef = ConfigurationManager.AppSettings["reportDoc"];

            //lang select
            selLang.DataSource = (DataView)Application[aCommon.getLang() + "LangSel"];
            selLang.DataTextField = "member_desc";
            selLang.DataValueField = "member_code";
            selLang.Text = aCommon.getLang();
            selLang.DataBind();

            //project select
            selSearchProjectTitleTD.InnerHtml = aCommon.getProjectsSel("");
            
            //project sel status
            selProjStatus.DataSource = new DataView((DataTable)Application[aCommon.getLang() + "StatusSel"], "id in ("+ConfigurationManager.AppSettings["projectSearchStatus"]+")", "", DataViewRowState.OriginalRows);
            selProjStatus.DataTextField = "member_desc";
            selProjStatus.DataValueField = "id";
            selProjStatus.DataBind();
            //need a value or onchange doesn't fire... weird
            selProjStatus.Items.Insert(0, new ListItem(aCommon.getText("selProjectsAll"), "ALL"));
            selProjStatus.Text = Request.Form["selProjStatus"] != null ? Request.Form["selProjStatus"] : "";
            

            #endregion

            #region search
            OracleDataReader aDR;
            //probably a really really bad idea to get currecny here
            //should get it from common so it is done the same way throughout the app
            //if someone less lazy then me reads this please udate it
            aDR = aCommon.GetReader(
                              @"SELECT h.project_id
                                      ,decode(curr.lang
                                             ,NULL
                                             ,def.project_title
                                             ,curr.project_title) AS project_title
                                      ,h.project_owner
                                      ,h.status_id
                                      ,b.bottomdesc branch
                                      ,TRIM(to_char(h.savings*nvl(c.rate,1),'999,999,999')) savings
                                      ,h.key_words
                                  FROM pip_header    h
                                      ,pip_detail    def
                                      ,pip_detail    curr
                                      ,mart.V_HFM_PIP_ENTITY@rpt.world b
                                      ,pip_currency c
                                 WHERE h.deleted = 'N'
                                   AND h.project_id = def.project_id
                                   AND h.defualt_lang = def.lang
                                   AND h.status_id in (" + ConfigurationManager.AppSettings["projectSearchStatus"]+@")
                                   AND h.project_owner = '" + aCommon.getUser() + @"'
                                   AND h.branch_id = b.bottomitem(+)
                                   AND h.project_id = curr.project_id(+)
                                   AND c.from_currency_id(+) = "+ConfigurationManager.AppSettings["defaultCurrency"]+ @"
                                   AND c.to_currency_id(+) = h.currency_id
                                   AND h.create_date between c.start_date and nvl(c.end_date,sysdate)
                                   AND '" + aCommon.getLang() + @"' = curr.lang(+)"+
                                  (Request.Form["selProjStatus"] != null && Request.Form["selProjStatus"] != "ALL" ? " AND h.status_id = " + Request.Form["selProjStatus"] : "") +
                                " ORDER BY " + ConfigurationManager.AppSettings["projectSearchStatusOrder"]);
            divSearchResults.InnerHtml = "";

            if (aDR != null)
            {
                while (aDR.Read())
                {
                    //<span style='float:left;'>{0} - pid</span><span style='float:right;'>{4}</span>
                      //<table cellspacing='0' cellpadding='0'>
                      //          <tr><td class='labelSearch'>{1}</td><td class='inputSearch'>{2}</td></tr>
                      //          <tr><td class='labelSearch'>{3}</td><td class='inputSearch'>{4}</td></tr>
                      //          <tr><td class='labelSearch'>{5}</td><td class='inputSearch'>{6}</td></tr>
                      //          <tr><td class='labelSearch'><br></td><td class='inputSearch'> </td></tr>
                      //          <tr><td  class='labelSearch' colspan='2' style='text-align:center;'><a href='{9}'>Update</a></td></tr>
                      //      </table>
                    divSearchResults.InnerHtml += string.Format(
                      @"<div class='searchResultsDiv'>
                        <div class='searchResultsHeaderDiv'><span class='searchResultsHeaderSpanLeft'><a href='{9}'>{0}</a> - {2}</span><span  class='searchResultsHeaderSpanRight'>{4}&nbsp;&nbsp;</span></div>
                        <div class='searchResultsMainLeftDiv'><br>{10}<br><br><span class='searchSavingsColor'>{6}</span></div>
                        <div class='searchResultsMainMiddleDiv'>{7}</div>
                        <div class='searchResultsMainRightDiv'><image alt='status' src='images/status{8}{11}.jpg'/></div>
                    </div>"
                        , aDR["project_title"].ToString()
                        , aCommon.getText("labelProjectID")
                        , aDR["project_id"].ToString()
                        , aCommon.getText("labelBranch")
                        , aDR["branch"].ToString()
                        , aCommon.getText("labelSavings") //5
                        //, Convert.ToString(Convert.ToDecimal(aDR["savings"]) * theCommon.getRate(ConfigurationManager.AppSettings["defaultCurrency"],aDR["currency_id"]))
                        , aDR["savings"].ToString()
                        , aDR["key_words"].ToString()
                        , aDR["status_id"].ToString()
                        , ConfigurationManager.AppSettings["indexPage"] + "?pid=" + aDR["project_id"].ToString()
                        , aCommon.getText("labelSavingAnnual") //10
                        , aCommon.getLang()
                        );
                }

                aDR.Close();
                aDR.Dispose();
            }

            aCommon.cleanUp();

            #endregion
        }
    }
}
