using AjaxControlToolkit;
using Plumtree.Remote.Portlet;
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Devart.Data.Oracle;
using WS20.Framework;

namespace PIP
{
    public partial class _Default : System.Web.UI.Page
    {

        /*
         * steps to add new lang
         * 
         * 1) add row to web_lang_codes table
         * 
         * 2) Add all rows for app in web_lang table and pip_lookup
         * 
         * 3) double check all rows in web lang and add missing :)
         * 
         *  SELECT *
  FROM web_lang e
      ,web_lang s
 WHERE e.lang = 'ENG'
   AND e.name = s.name(+)
   AND s.lang(+) = 'GER'

         * 
         * 4) update pip_lookup with tranulation for new lang in existing langs
         * select t.*,rowid from pip_lookup t where category_id = 3
         * 
         * insert into pip_lookup values (46,3,'ENG','GER','German',5);
insert into pip_lookup values (46,3,'ESP','GER','German',5);
insert into pip_lookup values (46,3,'POR','GER','German',5);
insert into pip_lookup values (46,3,'CZE','GER','German',5);
insert into pip_lookup values (46,3,'GER','GER','German',5);
         * 
         * 6)
         * need to update images for project title and status with new lang, make sure to name like others with new code
         * 
         * 7) app needs to refres cache to get new changes
         * */

        protected void Page_Init(object sender, EventArgs e)
        {
            //auto selects seleced fields here, but can't get which lang
            //--works on local host but not on server??
            //so if select dependant on lang put in page load

            //doing this here won't overwrite whats already there
            //just defaut it if its blank
            //inputStartDate.Text = DateTime.Now.ToShortDateString();

            //branch select
            selBranch.DataSource = (DataView)Application["BranchSel"];
            selBranch.DataTextField = "entity_desc";
            selBranch.DataValueField = "itemid";
            //selBranch.SelectedValue = Request.Form["selBranch"] != null ? Request.Form["selBranch"] : "";
            selBranch.DataBind();

            //pHiu3noxH5VTNu6epDSiPg==
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            common aCommon = new common(Application, Request.Form["selLang"], PortletContextFactory.CreatePortletContext(Request, Response));
            string aStartScript = "{}";
            string aSelProject = Request.Form["hiddenSearchProjectTitle"] != null ? Request.Form["hiddenSearchProjectTitle"] : "";
              
            #region log

            if (aCommon.getLogging())
            {
                aCommon.logError("index.aspx.cs", "Page_Load",
                                    String.Format("username:{0}<br>lang:{1}<br>pid:{2}<br>action:{3}<br>postback:{4}<br>impPlanRow:{5}<br>qstring:{6}"
                                   , aCommon.getUser()
                                   , aCommon.getLang()
                                   , Request.Form["hiddenSearchProjectTitle"]
                                   , Request.Form["hiddenEntryAction"]
                                   , Page.IsPostBack.ToString()
                                   , hiddenImpPlanRowCT.Value
                                   , Request.QueryString["pid"]
                                   )
                                   , "", "LOG");
            }

            #endregion
              
            #region load text
            labelMSG.InnerHtml = aCommon.getUser();// ""; 
            imgHeader.Src = "images/header"+aCommon.getLang()+".jpg"; 
            //titles
           // labelTitle.InnerHtml = aCommon.getText("labelTitle");
           // labelSearchTitle.InnerHtml = aCommon.getText("labelSearchTitle");
            //search page
            labelSearchProjectTitle.InnerHtml = aCommon.getText("labelSearchProjectTitle");
            buttonSearch.Value = aCommon.getText("buttonSearch");
            //links
            mainMenuLink.InnerHtml = aCommon.getText("mainMenuLink");
            mainMenuLink.HRef = ConfigurationManager.AppSettings["mainPage"];
            printLink.InnerHtml = aCommon.getText("printLink");
            //app msg
            appMsg.InnerHtml = aCommon.getText("appMsg"); 
            //entry page  
            labelProjectTitle.InnerHtml = aCommon.getText("labelProjectTitle");
            projectTitleEnglishTR.Style.Add(HtmlTextWriterStyle.Display, aCommon.getLang() == "ENG" ? "none" : "");
            labelProjectTitleEnglish.InnerHtml = aCommon.getText("labelProjectTitleEnglish");
            labelLang.InnerHtml = aCommon.getText("labelLang");
            labelProjectOwner.InnerHtml = aCommon.getText("labelProjectOwner");
            //  inputProjectOwner.Text = Request.Form["inputProjectOwner"] == "" ? aCommon.getUserName() : Request.Form["inputProjectOwner"];
            labelProjectID.InnerHtml = aCommon.getText("labelProjectID");
            labelBranch.InnerHtml = aCommon.getText("labelBranch");
            labelProduct.InnerHtml = aCommon.getText("labelProduct");
            labelProcess.InnerHtml = aCommon.getText("labelProcess");
            labelTeamMembers.InnerHtml = aCommon.getText("labelTeamMembers");
            buttonTeamMembersAdd.Value = aCommon.getText("buttonTeamMembersAdd");
            buttonTeamMembersRemove.Value = aCommon.getText("buttonTeamMembersRemove");
            labelStatus.InnerHtml = aCommon.getText("labelStatus");
            labelCurrency.InnerHtml = aCommon.getText("labelCurrency");
            labelSavings.InnerHtml = aCommon.getText("labelSavings");
            labelCapx.InnerHtml = aCommon.getText("labelCapx");
            labelExpense.InnerHtml = aCommon.getText("labelExpense");
            labelPayback.InnerHtml = aCommon.getText("labelPayback");
            labelKeyWords.InnerHtml = aCommon.getText("labelKeyWords");
            labelDesc.InnerHtml = aCommon.getText("labelDesc");
            labelCurrState.InnerHtml = aCommon.getText("labelCurrState");
            labelFutureState.InnerHtml = aCommon.getText("labelFutureState");
            labelSavingsCalc.InnerHtml = aCommon.getText("labelSavingsCalc");
            labelSavingsCategory.InnerHtml = aCommon.getText("labelSavingsCategory");
            labelStartDate.InnerHtml = aCommon.getText("labelStartDate");
            labelCurrYearSaving.InnerHtml = aCommon.getText("labelCurrYearSaving");
            labelMonthlySaving.InnerHtml = aCommon.getText("labelMonthlySaving");
            labelImplementationPlan.InnerHtml = aCommon.getText("labelImplementationPlan");
            //??date text.InnerHtml = aCommon.getText("labelProjectTitle");
            buttonInsMSRow.Value = aCommon.getText("buttonInsRow");
            labelAction.InnerHtml = aCommon.getText("labelAction");
            labelResponse.InnerHtml = aCommon.getText("labelResponse");
            labelTargetDate.InnerHtml = aCommon.getText("labelTargetDate");
            labelActionStatus.InnerHtml = aCommon.getText("labelActionStatus");
            buttonInsRow.Value = aCommon.getText("buttonInsRow");
            labelMSG.Style.Add(HtmlTextWriterStyle.Display, "none");
            labelFiles.InnerHtml = aCommon.getText("labelFiles");
            buttonSaveProject.Value = aCommon.getText("buttonSaveProject");
            buttonCopyProject.Value = aCommon.getText("buttonCopyProject");
            buttonDeleteProject.Value = aCommon.getText("buttonDeleteProject");
            labelAddFiles.InnerHtml = aCommon.getText("labelAddFiles");
            //tips
            labelLang.Attributes.Add("title", aCommon.getText("tipLang"));
            selLang.ToolTip = aCommon.getText("tipLang");
            labelSearchProjectTitle.Attributes.Add("title", aCommon.getText("tipProject"));
            //selSearchProject Title tip in common
            labelProjectTitle.Attributes.Add("title", aCommon.getText("tipProjectTitle"));
            inputProjectTitle.Attributes.Add("title", aCommon.getText("tipProjectTitle"));
            labelProjectTitleEnglish.Attributes.Add("title", aCommon.getText("tipProjectTitleEng"));
            inputProjectTitleEnglish.Attributes.Add("title", aCommon.getText("tipProjectTitleEng"));
            labelProjectOwner.Attributes.Add("title", aCommon.getText("tipProjectOwner"));
            //inputProjectOwner.ToolTip = aCommon.getText("tipProjectOwner");
            selProjectOwner.ToolTip = aCommon.getText("tipProjectOwner");
            labelProjectID.Attributes.Add("title", aCommon.getText("tipProjectID"));
            inputProjectID.Attributes.Add("title", aCommon.getText("tipProjectID"));
            labelBranch.Attributes.Add("title", aCommon.getText("tipBranch"));
            selBranch.ToolTip = aCommon.getText("tipBranch");
            labelTeamMembers.Attributes.Add("title", aCommon.getText("tipKeyTeamMembers"));
            
            //inputTeamMembers.ToolTip = aCommon.getText("tipKeyTeamMembers");
            selOneTeamMembers.Attributes.Add("title", aCommon.getText("tipKeyTeamMembers"));
            //selOneTeamMembers.ToolTip = aCommon.getText("tipKeyTeamMembers");
            labelProduct.Attributes.Add("title", aCommon.getText("tipProduct"));
            selProduct.ToolTip = aCommon.getText("tipProduct");
            labelCurrency.Attributes.Add("title", aCommon.getText("tipCurrency"));
            selCurrency.ToolTip = aCommon.getText("tipCurrency");
            //set in js
            //labelSavings.Attributes.Add("title", aCommon.getText("tipSavings"));
            //inputSavings.Attributes.Add("title", aCommon.getText("tipSavings"));
            labelCapx.Attributes.Add("title", aCommon.getText("tipCapex"));
            inputCapx.Attributes.Add("title", aCommon.getText("tipCapex"));
            labelExpense.Attributes.Add("title", aCommon.getText("tipExpense"));
            inputExpense.Attributes.Add("title", aCommon.getText("tipExpense"));
            //set in js
            //labelPayback.Attributes.Add("title", aCommon.getText("tipPayback"));
            //inputPayback.Attributes.Add("title", aCommon.getText("tipPayback"));
            labelKeyWords.Attributes.Add("title", aCommon.getText("tipShortDesc"));
            textareaKeyWords.Attributes.Add("title", aCommon.getText("tipShortDesc"));
            labelDesc.Attributes.Add("title", aCommon.getText("tipDetailedDesc"));
            textareaDesc.Attributes.Add("title", aCommon.getText("tipDetailedDesc"));
            labelCurrState.Attributes.Add("title", aCommon.getText("tipCurrentState"));
            textareaCurrState.Attributes.Add("title", aCommon.getText("tipCurrentState"));
            labelFutureState.Attributes.Add("title", aCommon.getText("tipFutureState"));
            textareaFutureState.Attributes.Add("title", aCommon.getText("tipFutureState"));
            labelSavingsCalc.Attributes.Add("title", aCommon.getText("tipSavingsCalc"));
            textareaSavingsCalc.Attributes.Add("title", aCommon.getText("tipSavingsCalc"));
            labelStartDate.Attributes.Add("title", aCommon.getText("tipStartDate"));
            inputStartDate.ToolTip = aCommon.getText("tipStartDate");
            labelMonthlySaving.Attributes.Add("title", aCommon.getText("tipMonthlySavings"));
            // put on all months? selProduct.ToolTip = aCommon.getText("tipMonthlySavings");
            labelSavingsCategory.Attributes.Add("title", aCommon.getText("tipCategory"));
            //selects in ms table create
            labelImplementationPlan.Attributes.Add("title", aCommon.getText("tipImpPlan"));
            labelAction.Attributes.Add("title", aCommon.getText("tipAction"));
            labelResponse.Attributes.Add("title", aCommon.getText("tipResponsible"));
            labelTargetDate.Attributes.Add("title", aCommon.getText("tipTargetDate"));
            labelFiles.Attributes.Add("title", aCommon.getText("tipFiles"));
            fileInput.ToolTip = aCommon.getText("tipFiles");
            buttonSaveProject.Attributes.Add("title", aCommon.getText("tipSave"));
            buttonCopyProject.Attributes.Add("title", aCommon.getText("tipCopy"));
            //buttonCopyProject.Style.Add(HtmlTextWriterStyle.Display, "none");
            buttonDeleteProject.Attributes.Add("title", aCommon.getText("tipDelete"));
            //buttonDeleteProject.Style.Add(HtmlTextWriterStyle.Display, "none");
             
            ////remove stars
            //also do same thing is js.  would like to only do in js but fing viewstate requires both places
            //if not then once they are set to red they don't change back
            labelMSG.Attributes.Remove("class");
            labelProjectTitleReq.Attributes.Add("class", "reqField");
            labelProjectTitle.Attributes.Add("class", "label");
            labelProjectTitleInput.Attributes.Add("class", "input");
            labelProjectTitleEnglishReq.Attributes.Add("class", "reqField");
            labelProjectTitleEnglish.Attributes.Add("class", "label");
            labelProjectTitleEnglishInput.Attributes.Add("class", "input");
            labelBranchReq.InnerHtml = "";
            labelBranchReq.Attributes.Add("class", "reqField");
            labelBranch.Attributes.Add("class", "label");
            labelBranchInput.Attributes.Add("class", "input");
            labelProductReq.InnerHtml = "";
            labelProductReq.Attributes.Add("class", "reqField");
            labelProduct.Attributes.Add("class", "label");
            labelProductInput.Attributes.Add("class", "inputR");
            labelProcessReq.InnerHtml = "";
            labelProcessReq.Attributes.Add("class", "reqField");
            labelProcess.Attributes.Add("class", "label");
            labelProcessInput.Attributes.Add("class", "inputR");
            labelSavingsReq.InnerHtml = "";
            labelSavingsReq.Attributes.Add("class", "reqField");
            labelSavings.Attributes.Add("class", "label");
            labelSavingsInput.Attributes.Remove("class");
            labelKeyWordsReq.InnerHtml = "";
            labelKeyWordsReq.Attributes.Add("class", "reqField");
            labelKeyWords.Attributes.Add("class", "label");
            labelDescReq.InnerHtml = "";
            labelDescReq.Attributes.Add("class", "reqField");
            labelDesc.Attributes.Add("class", "label");
            labelSavingsCalcReq.InnerHtml = "";
            labelSavingsCalcReq.Attributes.Add("class", "reqField");
            labelSavingsCalc.Attributes.Add("class", "label");
            
            //should never need to be readonly since if you have access you can update it
            labelTemplate.InnerHtml = aCommon.getText("labelTemplate");
            if (aCommon.isAdmin())
            {
                labelTemplate.Style.Add("display", "");
                inputTemplate.Style.Add("display", "");
            }
            else
            {
                labelTemplate.Style.Add("display", "none");
                inputTemplate.Style.Add("display", "none");
            }
            //autoExtenderProjectOwner.ServicePath = ConfigurationManager.AppSettings["peopleServicePath"];
           // autoExtenderTeamMembers.ServicePath = "portalUsers.asmx";
             
            #endregion

            # region load dropdowns

            //lang select
            selLang.DataSource = (DataView)Application[aCommon.getLang() + "LangSel"];
            selLang.DataTextField = "member_desc";
            selLang.DataValueField = "member_code";
            selLang.SelectedValue = aCommon.getLang();
            selLang.DataBind();

            //project owner sel
            selProjectOwner.DataSource = (DataTable)Application["PeopleSel"];
            selProjectOwner.DataTextField = "full_name";
            selProjectOwner.DataValueField = "objectid";
            selProjectOwner.SelectedValue = Request.Form["selProjectOwner"] != null && Request.Form["selProjectOwner"] != "" ? Request.Form["selProjectOwner"] : aCommon.getUser();
            //Response.Write(Request.Form["selProjectOwner"] + " --- " + aCommon.getUser() + " --- " + aCommon.getUserName());
            //return;
            selProjectOwner.DataBind();
            //  inputProjectOwner.Text = Request.Form["inputProjectOwner"] == "" ? aCommon.getUserName() : Request.Form["inputProjectOwner"];

            //team members sel
            //selOneTeamMembers.DataSource = (DataTable)Application["PeopleSel"];
            //selOneTeamMembers.DataTextField = "full_name";
            //selOneTeamMembers.DataValueField = "objectid";
            //selOneTeamMembers.DataBind();
            //  inputProjectOwner.Text = Request.Form["inputProjectOwner"] == "" ? aCommon.getUserName() : Request.Form["inputProjectOwner"];
            //selOneTeamMembers.Items.Insert(0,new ListItem("",""));
            //selOneTeamMembers.SelectedValue = Request.Form["selTeamMembers"] != null ? Request.Form["selTeamMembers"] : "";

            //product sel
            selProduct.DataSource = (DataView)Application[aCommon.getLang() + "ProductSel"];
            selProduct.DataTextField = "member_desc";
            selProduct.DataValueField = "id";
            selProduct.SelectedValue = Request.Form["selProduct"] != null ? Request.Form["selProduct"] : ConfigurationManager.AppSettings["defaultProduct"];
            selProduct.DataBind();

            //status sel
            selStatus.DataSource = (DataTable)Application[aCommon.getLang() + "StatusSel"];
            selStatus.DataTextField = "member_desc";
            selStatus.DataValueField = "id";
            selStatus.SelectedValue = Request.Form["selStatus"] != null ? Request.Form["selStatus"] : ConfigurationManager.AppSettings["statusDraft"];
            selStatus.DataBind();

            //process sel
            selProcess.DataSource = (DataView)Application[aCommon.getLang() + "ProcessSel"];
            selProcess.DataTextField = "member_desc";
            selProcess.DataValueField = "id";
            selProcess.SelectedValue = Request.Form["selProcess"] != null ? Request.Form["selProcess"] : ConfigurationManager.AppSettings["defaultProcess"];
            selProcess.DataBind();

            //process sel
            selCurrency.DataSource = (DataView)Application[aCommon.getLang() + "CurrencySel"];
            selCurrency.DataTextField = "member_desc";
            selCurrency.DataValueField = "id";
            selCurrency.SelectedValue = Request.Form["selCurrency"] != null ? Request.Form["selCurrency"] : ConfigurationManager.AppSettings["defaultCurrency"];
            selCurrency.DataBind();
            //load team members
            loadTeamMemebers(aCommon, Request.Form["hiddenTeamMembers"]);

            #endregion

            #region actions

            switch (Request.Form["hiddenEntryAction"])
            {
                case "addMSRow":
                    writeMSTable(aCommon, Convert.ToInt32(hiddenMSRowCT.Value), true, false, 1);
                    writeImpPlanTable(aCommon, Convert.ToInt32(hiddenImpPlanRowCT.Value), false, false);
                    writeFiles(aCommon, Request.Form["inputProjectID"], false);
                    aStartScript = "{ document.getElementById('buttonInsMSRow').focus(); }";
                    break;
                case "addImpPlanRow":
                    writeMSTable(aCommon, Convert.ToInt32(hiddenMSRowCT.Value), false, false, 1);
                    writeImpPlanTable(aCommon, Convert.ToInt32(hiddenImpPlanRowCT.Value), true, false);
                    writeFiles(aCommon, Request.Form["inputProjectID"], false);
                    aStartScript = "{ document.getElementById('buttonInsRow').focus(); }";
                    break;
                case "updateEntryForm":
                    bool aReqField = false;
                        
                    #region checkReqFields
                    if (Request.Form["inputProjectTitle"] == "")
                    {
                        labelProjectTitleReq.Attributes.Add("class", "reqField reqError");
                        labelProjectTitle.Attributes.Add("class", "label labelError");
                        labelProjectTitleInput.Attributes.Add("class", "input inputError");
                        aReqField = true;
                    }
                    if (Request.Form["inputProjectTitleEnglish"] == "" && aCommon.getLang() != ConfigurationManager.AppSettings["defaultLang"])
                    {
                        labelProjectTitleEnglishReq.Attributes.Add("class", "reqField reqError");
                        labelProjectTitleEnglish.Attributes.Add("class", "label labelError");
                        labelProjectTitleEnglishInput.Attributes.Add("class", "input inputError");
                        aReqField = true;
                    }
                    if (Request.Form["selStatus"].ToString() != ConfigurationManager.AppSettings["statusDraft"])
                    {
                        if (Request.Form["selBranch"] == "")
                        {
                            labelBranchReq.InnerHtml = "*";
                            labelBranchReq.Attributes.Add("class", "reqField reqError");
                            labelBranch.Attributes.Add("class", "label labelError");
                            labelBranchInput.Attributes.Add("class", "input inputError");
                            aReqField = true;
                        }
                        //if (Request.Form["selProduct"] == "")
                        //{
                        //    labelProductReq.InnerHtml = "*";
                        //    labelProductReq.Attributes.Add("class", "reqField reqError");
                        //    labelProduct.Attributes.Add("class", "label labelError");
                        //    labelProductInput.Attributes.Add("class", "inputR inputError");
                        //    aReqField = true;
                        //}
                        if (Request.Form["selProcess"] == "")
                        {
                            labelProcessReq.InnerHtml = "*";
                            labelProcessReq.Attributes.Add("class", "reqField reqError");
                            labelProcess.Attributes.Add("class", "label labelError");
                            labelProcessInput.Attributes.Add("class", "inputR inputError");
                            aReqField = true;
                        }
                        if (Request.Form["inputSavings"] == "0")
                        {
                            labelSavingsReq.InnerHtml = "*";
                            labelSavingsReq.Attributes.Add("class", "reqField reqError");
                            labelSavings.Attributes.Add("class", "label labelError");
                            labelSavingsInput.Attributes.Add("class", "inputError");
                            //labelMSG.InnerHtml = "asdf"+"<br>";
                            aReqField = true;
                        }
                        if (Request.Form["textareaKeyWords"] == "")
                        {
                            labelKeyWordsReq.InnerHtml = "*";
                            labelKeyWordsReq.Attributes.Add("class", "reqField reqError");
                            //labelKeyWords.Attributes.Add("class", "label labelError");
                            labelKeyWords.Attributes.Add("class", "label inputError");
                            aReqField = true;
                        }
                        if (Request.Form["textareaDesc"] == "")
                        {
                            labelDescReq.InnerHtml = "*";
                            labelDescReq.Attributes.Add("class", "reqField reqError");
                            //labelKeyWords.Attributes.Add("class", "label labelError");
                            labelDesc.Attributes.Add("class", "label inputError");
                            aReqField = true;
                        }
                        if (Request.Form["textareaSavingsCalc"] == "")
                        {
                            labelSavingsCalcReq.InnerHtml = "*";
                            labelSavingsCalcReq.Attributes.Add("class", "reqField reqError");
                            //labelKeyWords.Attributes.Add("class", "label labelError");
                            labelSavingsCalc.Attributes.Add("class", "label inputError");
                            aReqField = true;
                        }

                    }

                    #endregion

                    if (aReqField)
                    {
                        labelMSG.Style.Add(HtmlTextWriterStyle.Display, "");
                        labelMSG.InnerHtml += aCommon.getText("labelMSGReq");
                        labelMSG.Attributes.Add("class", "msgDivError");
                        int aMSRowCT = Convert.ToInt32(hiddenMSRowCT.Value);
                        int aImpPlanRowCT = Convert.ToInt32(hiddenImpPlanRowCT.Value);
                        int aFilesCT = Convert.ToInt32(hiddenFilesCT.Value);

                        writeMSTable(aCommon, Convert.ToInt32(hiddenMSRowCT.Value), false, false, 1);
                        writeImpPlanTable(aCommon, Convert.ToInt32(hiddenImpPlanRowCT.Value), false, false);
                        writeFiles(aCommon, Request.Form["inputProjectID"], false);
                    }
                    else
                    {
                        aSelProject = updateData(aCommon, false, "N");
                    }
                    break;
                case "copyEntryForm":
                    aSelProject = updateData(aCommon, true, "N");
                    break;
                case "deleteEntryForm":
                    aSelProject = updateData(aCommon, false, "Y");
                    break;
                case "changeLang":
                    writeMSTable(aCommon, Convert.ToInt32(hiddenMSRowCT.Value), false, hiddenReadonly.Value == "true", 1);
                    writeImpPlanTable(aCommon, Convert.ToInt32(hiddenImpPlanRowCT.Value), false, hiddenReadonly.Value == "true");
                    writeFiles(aCommon, Request.Form["inputProjectID"], hiddenReadonly.Value == "true");
                    break;
                case "changeCurrency":
                    float aRate = aCommon.getRate(hiddenCurrCurrency.Value, selCurrency.SelectedValue, Request.Form["inputProjectID"]);
                    //Response.Write(aRate.ToString());
                    inputCapx.Value = Convert.ToString(Convert.ToSingle(inputCapx.Value) * aRate);
                    inputExpense.Value = Convert.ToString(Convert.ToSingle(inputExpense.Value) * aRate);

                    writeMSTable(aCommon, Convert.ToInt32(hiddenMSRowCT.Value), false, false, aRate);
                    writeImpPlanTable(aCommon, Convert.ToInt32(hiddenImpPlanRowCT.Value), false, false);
                    writeFiles(aCommon, Request.Form["inputProjectID"], false);
                    break;
                case "search":
                    //aSelProject = Request.Form["hiddenSearchProjectTitle"];
                    loadData(aCommon, Request.Form["hiddenSearchProjectTitle"]);
                    break;
                default:
                    if (Request.QueryString["pid"] != null)
                    {
                        aSelProject = Request.QueryString["pid"];
                        loadData(aCommon, Request.QueryString["pid"]);
                    }
                    else
                    {
                        loadData(aCommon, "");
                        //writeMSTable(aCommon, Convert.ToInt32(hiddenMSRowCT.Value), true, false, 1);
                        //writeImpPlanTable(aCommon, Convert.ToInt32(hiddenImpPlanRowCT.Value), true, false);
                    }
                    break;
            }

            ClientScript.RegisterStartupScript(this.Page.GetType(), "focus", "<script>var prevAction = '" + Request.Form["hiddenEntryAction"] + @"'; 
                                                                                      var statusDraft = '" +ConfigurationManager.AppSettings["statusDraft"]+@"'; 
                                                                                      var tipSavings = '"+aCommon.getText("tipSavings")+@"';
                                                                                      var tipPayback = '" + aCommon.getText("tipPayback") + @"';
                                                                                      var dates = ['" + aCommon.getText("labelJan") + "','" + aCommon.getText("labelFeb") + "','" + aCommon.getText("labelMar") + "','" + aCommon.getText("labelApr") + "','" + aCommon.getText("labelMay") + "','" + aCommon.getText("labelJun") + "','" + aCommon.getText("labelJul") + "','" + aCommon.getText("labelAug") + "','" + aCommon.getText("labelSep") + "','" + aCommon.getText("labelOct") + "','" + aCommon.getText("labelNov") + "','" + aCommon.getText("labelDec") + @"']

                                                                                      function focusFun() " + aStartScript + 
                                                                              "</script>");
            //            ClientScript.RegisterStartupScript(this.Page.GetType(), "focus", string.Format(
            //                                    @"<script>
            //                                        var statusDraft = '{0}';
            //                                        var statusProposal = '{1}';
            //                                        var statusOnHold = '{2}';
            //                                        var statusOnPlan = '{3}';
            //                                        var statusLate = '{4}';
            //                                        var statusComplete = '{5}';
            //                                        var statusCanceled = '{6}';
            //                                        function focusFun() {7} 
            //                                      </script>"
            //                                    , ConfigurationManager.AppSettings["statusDraft"]
            //                                    , ConfigurationManager.AppSettings["statusProposal"]
            //                                    , ConfigurationManager.AppSettings["statusOnHold"]
            //                                    , ConfigurationManager.AppSettings["statusOnPlan"]
            //                                    , ConfigurationManager.AppSettings["statusLate"]
            //                                    , ConfigurationManager.AppSettings["statusComplete"]
            //                                    , ConfigurationManager.AppSettings["statusCanceled"]
            //                                    , aStartScript
            //                                    ));

            hiddenCurrCurrency.Value = Request.Form["selCurrency"] != null ? Request.Form["selCurrency"] : ConfigurationManager.AppSettings["defaultCurrency"];

            #endregion

            //do it here to catch any new ones just added
            //selSearchProjectTitle.DataSource 
            selSearchProjectTitleTD.InnerHtml = aCommon.getProjectsSel(aSelProject);

            //DataGrid aDG = new DataGrid();
            //aDG.DataSource = (DataTable)Application["CurrencyRateSel"];
            //aDG.DataBind();
            //Page.Controls.Add(aDG);
             
            aCommon.cleanUp();
        }

        protected void loadData(common theCommon, string theProject)
        {
            #region log

            if (theCommon.getLogging())
            {
                theCommon.logError("index.aspx.cs", "loadData", "theProject:" + theProject, "", "LOG");
            }

            #endregion

            OracleDataReader aDR;
            string aLang = "";  // hold which lang loaded (default if nothing saved for aCommon.getLang())
            bool isReadonly = true;
            hiddenReadonly.Value = "true";
            string aRate = "1";
            //string aToCurrency = "";

            #region load header / detail
            
            aDR = theCommon.GetReader(
                      @"SELECT (SELECT decode(COUNT(1) over(PARTITION BY NAME)
                                             ,1
                                             ,NAME
                                             ,NAME || ' ' || loginname)
                                  FROM ali.ptusers
                                 WHERE objectid = h.project_owner
                                   AND rownum < 2) AS project_owner
                              ,project_owner as project_owner_id
                              ,create_user_id
                              ,branch_id
                              ,status_id
                              ,product_id
                              ,nvl(capx*c.rate,0) capx
                              ,nvl(expense*c.rate,0) expense
                              ,nvl(payback,0) payback
                              ,to_char(h.start_date,'mm/dd/yyyy') start_date
                              ,team_members
                              ,project_title_eng
                              ,key_words
                              ,decode(cur.project_id
                                     ,NULL
                                     ,def.project_title
                                     ,cur.project_title) AS project_title
                              ,decode(cur.project_id
                                     ,NULL
                                     ,def.detailed_description
                                     ,cur.detailed_description) AS detailed_description
                              ,decode(cur.project_id
                                     ,NULL
                                     ,def.current_state
                                     ,cur.current_state) AS current_state
                              ,decode(cur.project_id
                                     ,NULL
                                     ,def.future_state
                                     ,cur.future_state) AS future_state
                              ,decode(cur.project_id
                                     ,NULL
                                     ,def.savings_calc
                                     ,cur.savings_calc) AS savings_calc
                              ,decode(cur.project_id
                                     ,NULL
                                     ,def.lang
                                     ,cur.lang) AS lang
                              ,h.process_id
                              ,c.rate
                              ,h.currency_id
                              ,h.template
                          FROM pip_header h
                              ,pip_detail def
                              ,pip_detail cur
                              ,pip_currency c
                         WHERE h.project_id = def.project_id
                           AND h.defualt_lang = def.lang
                           AND h.project_id = cur.project_id(+)
                           AND '" + theCommon.getLang() + @"' = cur.lang(+)
                           AND h.project_id = '" + theProject + @"'
                           AND c.from_currency_id = " + ConfigurationManager.AppSettings["defaultCurrency"] + @"
                           AND c.to_currency_id = h.currency_id
                           AND h.create_date between c.start_date and nvl(c.end_date,sysdate)
                     UNION ALL 
                     SELECT null," + theCommon.getUser()+@",null,null,"+ConfigurationManager.AppSettings["statusDraft"]+ @",null,0,0,null,to_char(sysdate,'mm/dd/yyyy'),null,null,null,null,null,null,null,
                            null,null,null,1,"+ConfigurationManager.AppSettings["defaultCurrency"]+@",null 
                     FROM dual ");
 
            if (aDR != null && aDR.Read())
            {
                aRate = aDR["rate"].ToString();
                //aRate = theCommon.getRate(ConfigurationManager.AppSettings["defaultCurrency"], aDR["currency_id"].ToString());
                //set security
                if (theCommon.getUser() == aDR["project_owner_id"].ToString() || theCommon.getUser() == aDR["create_user_id"].ToString() || theCommon.getUser() == ConfigurationManager.AppSettings["adminPortalID"] || theCommon.isAdmin())
                {
                    isReadonly = false;
                    hiddenReadonly.Value = "";
                }
                //header 
                inputProjectID.Value = theProject;
                //inputProjectOwner.Text = aDR["project_owner"].ToString() == "" ?  theCommon.getUserName() : aDR["project_owner"].ToString();
                selProjectOwner.SelectedValue = aDR["project_owner"].ToString() == "" ? theCommon.getUser() : aDR["project_owner_id"].ToString();
                selBranch.SelectedValue = aDR["branch_id"].ToString();
                selStatus.SelectedValue = aDR["status_id"].ToString();
                selProduct.SelectedValue = aDR["product_id"].ToString();
                //inputSavings.Value = Convert.ToString(Convert.ToSingle(aDR["savings"].ToString(); //calcs on form
                inputCapx.Value = aDR["capx"].ToString();
                inputExpense.Value = aDR["expense"].ToString();
                //inputPayback.Value = aDR["payback"].ToString();//calcs on form
                //inputCurrYearSaving.Value = Convert.ToString(Convert.ToSingle(aDR["current_year_savings"].ToString();//calcs on form
                inputStartDate.Text = aDR["start_date"].ToString();
                loadTeamMemebers(theCommon, aDR["team_members"].ToString());
                inputProjectTitleEnglish.Value = aDR["project_title_eng"].ToString();
                textareaKeyWords.Value = aDR["key_words"].ToString();
                //detail
                inputProjectTitle.Value = aDR["project_title"].ToString();
                textareaDesc.Value = aDR["detailed_description"].ToString();
                textareaCurrState.Value = aDR["current_state"].ToString();
                textareaFutureState.Value = aDR["future_state"].ToString();
                textareaSavingsCalc.Value = aDR["savings_calc"].ToString();
                selProcess.SelectedValue = aDR["process_id"].ToString();
                selCurrency.SelectedValue = aDR["currency_id"].ToString();
                hiddenCurrCurrency.Value = aDR["currency_id"].ToString();
                aLang = aDR["lang"].ToString() == "" ? theCommon.getLang() : aDR["lang"].ToString();
                templateCK.Checked = (aDR["template"].ToString() == "Y") ? true : false;
                aDR.Close();
            }

            #endregion
                        
            #region load ms row
             
            //order?
            aDR = theCommon.GetReader(@"SELECT savings_category_id
                                              ,nvl(monthly_savings_1*" + aRate + @",0) monthly_savings_1
                                              ,nvl(monthly_savings_2*" + aRate + @",0) monthly_savings_2
                                              ,nvl(monthly_savings_3*" + aRate + @",0) monthly_savings_3
                                              ,nvl(monthly_savings_4*" + aRate + @",0) monthly_savings_4
                                              ,nvl(monthly_savings_5*" + aRate + @",0) monthly_savings_5
                                              ,nvl(monthly_savings_6*" + aRate + @",0) monthly_savings_6
                                              ,nvl(monthly_savings_7*" + aRate + @",0) monthly_savings_7
                                              ,nvl(monthly_savings_8*" + aRate + @",0) monthly_savings_8
                                              ,nvl(monthly_savings_9*" + aRate + @",0) monthly_savings_9
                                              ,nvl(monthly_savings_10*" + aRate + @",0) monthly_savings_10
                                              ,nvl(monthly_savings_11*" + aRate + @",0) monthly_savings_11
                                              ,nvl(monthly_savings_12*" + aRate + @",0) monthly_savings_12
                                          FROM pip_ms_header h
                                         WHERE h.project_id = '" + theProject + "'"); 
            int aMSRowCT = 0;
            if (aDR != null)
            { 
                while (aDR.Read())
                {
                    writeMSRow(theCommon, aMSRowCT.ToString(), aDR["savings_category_id"].ToString(), aDR["monthly_savings_1"].ToString(), aDR["monthly_savings_2"].ToString(), aDR["monthly_savings_3"].ToString(), aDR["monthly_savings_4"].ToString(), aDR["monthly_savings_5"].ToString(), aDR["monthly_savings_6"].ToString(), aDR["monthly_savings_7"].ToString(), aDR["monthly_savings_8"].ToString(), aDR["monthly_savings_9"].ToString(), aDR["monthly_savings_10"].ToString(), aDR["monthly_savings_11"].ToString(), aDR["monthly_savings_12"].ToString(), isReadonly);
                    aMSRowCT++;
                }
                aDR.Close(); 
                aDR.Dispose();
            }

            if (aMSRowCT == 0)
            {
                writeMSTable(theCommon, 0, true, isReadonly,1);
            }
            else
            {
                aMSRowCT++;
                hiddenMSRowCT.Value = aMSRowCT.ToString();
            }

            #endregion
                                      
            #region load imp plan row

            aDR = theCommon.GetReader(@"SELECT h.row_id
                                              ,responsible
                                              ,to_char(target_date,'mm/dd/yyyy') target_date
                                              ,action
                                              ,status_id
                                          FROM pip_imp_plan_header h
                                              ,pip_imp_plan_detail d
                                         WHERE h.project_id = d.project_id
                                           AND h.row_id = d.row_id
                                           AND h.project_id = '" + theProject +
                                        "' AND d.lang = '" + aLang + @"'
                               ORDER BY h.target_date");
            int aImpPlanRowCT = 0;
            if (aDR != null)
            {
                while (aDR.Read())
                {
                    //don't use row_id here since sort order might change
                    writeImpPlanRow(theCommon, aImpPlanRowCT.ToString(), aDR["action"].ToString(), aDR["responsible"].ToString(), aDR["target_date"].ToString(), aDR["status_id"].ToString(), isReadonly);
                    aImpPlanRowCT++;
                }
                aDR.Close();
                aDR.Dispose();
                //hiddenImpPlanRowCT.Value = aImpPlanRowCT++.ToString();
            }
            //else
            //{
            //    writeImpPlanTable(0, true);
            //}

            if (aImpPlanRowCT == 0)
            {
                writeImpPlanTable(theCommon, 0, true, isReadonly);
            }
            else
            {
                aImpPlanRowCT++;
                hiddenImpPlanRowCT.Value = aImpPlanRowCT.ToString();
            }

            #endregion

            writeFiles(theCommon, theProject,isReadonly);
            
            setSecurity(isReadonly);

            if (theProject == "")
            {
                buttonCopyProject.Style.Add(HtmlTextWriterStyle.Display, "none");
                buttonDeleteProject.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                buttonCopyProject.Style.Add(HtmlTextWriterStyle.Display, "");
                buttonDeleteProject.Style.Add(HtmlTextWriterStyle.Display, "");
            }
        }

        protected void setSecurity(bool isReadonly)
        {
            //shouldn't need the else, but .net is being a little too helpful
            if (isReadonly)
            {
                #region security

                //header
                inputProjectTitle.Attributes.Add("readonly", "readonly");
                inputProjectTitle.Attributes.Add("class", "readonly inputWide");
                inputProjectTitleEnglish.Attributes.Add("readonly", "readonly");
                inputProjectTitleEnglish.Attributes.Add("class", "readonly inputWide");
                //inputProjectOwner.Attributes.Add("readonly", "readonly");
                //inputProjectOwner.Attributes.Add("class", "readonly");
                selProjectOwner.Attributes.Add("disabled", "true");
                selProjectOwner.Attributes.Add("class", "readonly");
                selBranch.Attributes.Add("disabled", "true");
                selBranch.Attributes.Add("class", "readonly");
                selProduct.Attributes.Add("disabled", "true");
                selProduct.Attributes.Add("class", "readonly");
                selProcess.Attributes.Add("disabled", "true");
                selProcess.Attributes.Add("class", "readonly");
                //inputTeamMembers.Attributes.Add("readonly", "readonly");
                //inputTeamMembers.Attributes.Add("class", "readonly");
                selOneTeamMembers.Attributes.Add("disabled", "true");
                selOneTeamMembers.Attributes.Add("class", "readonly");
                buttonTeamMembersAdd.Attributes.Add("disabled", "true");
                selTeamMembers.Attributes.Add("disabled", "true");
                selTeamMembers.Attributes.Add("class", "readonly");
                buttonTeamMembersRemove.Attributes.Add("disabled", "true");
                selStatus.Attributes.Add("disabled", "true");
                selStatus.Attributes.Add("class", "readonly");
                selCurrency.Attributes.Add("disabled", "true");
                selCurrency.Attributes.Add("class", "readonly");
                inputCapx.Attributes.Add("readonly", "readonly");
                inputCapx.Attributes.Add("class", "readonly");
                inputExpense.Attributes.Add("readonly", "readonly");
                inputExpense.Attributes.Add("class", "readonly");
                inputStartDate.Attributes.Add("readonly", "readonly");
                inputStartDate.Attributes.Add("class", "readonly");
                CalendarExtenderStartDate.Enabled = false;
                textareaKeyWords.Attributes.Add("readonly", "readonly");
                textareaKeyWords.Attributes.Add("class", "readonly inputWide");
                textareaDesc.Attributes.Add("readonly", "readonly");
                textareaDesc.Attributes.Add("class", "readonly inputWide");
                textareaCurrState.Attributes.Add("readonly", "readonly");
                textareaCurrState.Attributes.Add("class", "readonly inputWide");
                textareaFutureState.Attributes.Add("readonly", "readonly");
                textareaFutureState.Attributes.Add("class", "readonly inputWide");
                textareaSavingsCalc.Attributes.Add("readonly", "readonly");
                textareaSavingsCalc.Attributes.Add("class", "readonly inputWide");
                //ms row
                buttonInsMSRow.Attributes.Add("disabled", "true");
                //imp plan row
                buttonInsRow.Attributes.Add("disabled", "true");
                //file
                fileInput.Attributes.Add("readonly", "readonly");
                fileInput.Attributes.Add("class", "readonly");
                //main buttons
                buttonSaveProject.Attributes.Add("disabled", "true");
                //buttonCopyProject.Attributes.Add("disabled", "true");
                buttonDeleteProject.Attributes.Add("disabled", "true");

                #endregion
            }
            else
            {
                #region security
                //header
                inputProjectTitle.Attributes.Remove("readonly");
                inputProjectTitle.Attributes.Remove("class");
                inputProjectTitle.Attributes.Add("class", "inputWide");
                inputProjectTitleEnglish.Attributes.Remove("readonly");
                inputProjectTitleEnglish.Attributes.Remove("class");
                inputProjectTitleEnglish.Attributes.Add("class", "inputWide");
                //inputProjectOwner.Attributes.Remove("readonly");
                //inputProjectOwner.Attributes.Remove("class");
                //inputProjectOwner.Attributes.Add("class", "inputWide");
                selProjectOwner.Attributes.Remove("disabled");
                selProjectOwner.Attributes.Remove("class");
                selBranch.Attributes.Remove("disabled");
                selBranch.Attributes.Remove("class");
                selProduct.Attributes.Remove("disabled");
                selProduct.Attributes.Remove("class");
                selProcess.Attributes.Remove("disabled");
                selProcess.Attributes.Remove("class");
                selOneTeamMembers.Attributes.Remove("disabled");
                selOneTeamMembers.Attributes.Remove("class");
                //inputTeamMembers.Attributes.Remove("readonly");
                //inputTeamMembers.Attributes.Remove("class");
                buttonTeamMembersAdd.Attributes.Remove("disabled");
                selTeamMembers.Attributes.Remove("disabled");
                selTeamMembers.Attributes.Remove("class");
                buttonTeamMembersRemove.Attributes.Remove("disabled");
                selStatus.Attributes.Remove("disabled");
                selStatus.Attributes.Remove("class");
                selCurrency.Attributes.Remove("disabled");
                selCurrency.Attributes.Remove("class");
                inputCapx.Attributes.Remove("readonly");
                inputCapx.Attributes.Remove("class");
                inputExpense.Attributes.Remove("readonly");
                inputExpense.Attributes.Remove("class");
                inputStartDate.Attributes.Remove("readonly");
                inputStartDate.Attributes.Remove("class");
                CalendarExtenderStartDate.Enabled = true;
                textareaKeyWords.Attributes.Remove("readonly");
                textareaKeyWords.Attributes.Add("class", "inputWide");
                //detail
                textareaDesc.Attributes.Remove("readonly");
                textareaDesc.Attributes.Add("class", "inputWide");
                textareaCurrState.Attributes.Remove("readonly");
                textareaCurrState.Attributes.Add("class", "inputWide");
                textareaFutureState.Attributes.Remove("readonly");
                textareaFutureState.Attributes.Add("class", "inputWide");
                textareaSavingsCalc.Attributes.Remove("readonly");
                textareaSavingsCalc.Attributes.Add("class", "inputWide");
                //ms row
                buttonInsMSRow.Attributes.Remove("disabled");
                //imp plan row
                buttonInsRow.Attributes.Remove("disabled");
                //bottom buttons
                buttonSaveProject.Attributes.Remove("disabled");
                //file
                fileInput.Attributes.Remove("readonly");
                fileInput.Attributes.Remove("class");
                #endregion
            }
        }

        protected void loadTeamMemebers(common theCommon, string theTeamMembers)
        {
            #region log

            if (theCommon.getLogging())
            {
                theCommon.logError("index.aspx.cs", "loadTeamMemebers", "theTeamMembers:" + theTeamMembers, "", "LOG");
            }

            #endregion

            if (theTeamMembers != null)
            {
                selTeamMembers.Items.Clear();
                string[] aTeamMembers = theTeamMembers.Split(new string[1] { "','" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < aTeamMembers.Length; i++)
                {
                    if (aTeamMembers.Length == 1)
                    {
                        selTeamMembers.Items.Add(aTeamMembers[i].Substring(1, aTeamMembers[i].Length - 2));
                    }
                    else if (i == 0)
                    {
                        selTeamMembers.Items.Add(aTeamMembers[i].Substring(1, aTeamMembers[i].Length - 1));
                    }
                    else if (i == aTeamMembers.Length - 1)
                    {
                        selTeamMembers.Items.Add(aTeamMembers[i].Substring(0, aTeamMembers[i].Length - 1));
                    }
                    else
                    {
                        selTeamMembers.Items.Add(aTeamMembers[i]);
                    }
                }
            }
        }

        protected string updateData(common theCommon, bool theCopy, string theDelete)
        {
            #region log

            if (theCommon.getLogging())
            {
                theCommon.logError("index.aspx.cs", "updateData", "theCopy:" + theCopy.ToString() + "  theDelete:" + theDelete + "  status:" + Request.Form["selStatus"] + "  ?:" + Convert.ToString(Request.Form["selStatus"] != ConfigurationManager.AppSettings["statusDraft"]), "", "LOG");
            }

            #endregion

            OracleDataReader aDR;
            string aProjectID;
            string aBranchID;
            string aTemplate;
            labelMSG.InnerHtml = theCommon.getText("labelMSGUpdateSuccess");
            labelMSG.Style.Add(HtmlTextWriterStyle.Display, "");
            labelMSG.Attributes.Add("class", "msgDivInfo");
            ArrayList aQuery = new ArrayList();
            int aMSRowCT = Convert.ToInt32(hiddenMSRowCT.Value);
            int aImpPlanRowCT = Convert.ToInt32(hiddenImpPlanRowCT.Value);
            int aFilesCT = Convert.ToInt32(hiddenFilesCT.Value);
            string aSC = "";
            int aRowID = 0;
            
            //DataView aPeopleDV;
            string aProjectOwnerID;
            float aRate;

            string aResponsible = "";
            string aTargetDate = "";
            string aActionStatus = "";


                #region get project id & rate

                
                if (theCopy)
                {
                    aProjectID = "";
                    aProjectOwnerID = theCommon.getUser();
                    aBranchID = "";
                    aTemplate = "";
                    //action fields reset below
                }
                else
                {
                    aProjectID = Request.Form["inputProjectID"];
                    //not sure if i should alert error if can't find the owern
                    
                    //aPeopleDV = new DataView((DataTable)Application["PeopleSel"], " full_name = '" + Request.Form["inputProjectOwner"] + "'", "", DataViewRowState.CurrentRows);
                    aProjectOwnerID = Request.Form["selProjectOwner"] == null ? theCommon.getUser() : Request.Form["selProjectOwner"];
                    aBranchID = Request.Form["selBranch"];
                    aTemplate = Request.Form["templateCK"];
                }

                if (aProjectID == "")
                {
                    aDR = theCommon.GetReader("select pip_header_seq.nextval pip_seq from dual");
                    if (aDR.Read())
                    {
                        aProjectID = aDR["pip_seq"].ToString();
                    }
                    aDR.Close();
                    aDR.Dispose();
                }
                aRate = theCommon.getRate(Request.Form["selCurrency"], ConfigurationManager.AppSettings["defaultCurrency"], aProjectID);


                #endregion

                #region merge header/detail

            string aInputSaving ="";
            string aInputCapx = "";
            string aInputExpense = "";
            string aCurrYearSavings = "";
                try
                {

                    aInputCapx = Convert.ToString(Convert.ToSingle(Request.Form["inputCapx"].Replace(",", "")) * aRate);
                    aInputExpense = Convert.ToString(Convert.ToSingle(Request.Form["inputExpense"].Replace(",", "")) * aRate);
                    aInputSaving = Convert.ToString(Convert.ToSingle(Request.Form["inputSavings"].Replace(",", "")) * aRate);
                    aCurrYearSavings = Convert.ToString(Convert.ToSingle(Request.Form["inputCurrYearSaving"].Replace(",", "")) * aRate);
                              
                }
                catch(Exception ex){
                    theCommon.logError("index.aspx.cs", "updateData", "numbers", "Project:"+aProjectID+"  Savings:" + Request.Form["inputSavings"] + "  Capx:" + Request.Form["inputCapx"] + "  Expense:" + Request.Form["inputExpense"]+"  "+ex.ToString(), "ERROR");
                }

                //insert header
                aQuery.Add(string.Format(
                  @"MERGE INTO pip_header h
                        USING (SELECT " + aProjectID + @" project_id from dual) d
                           ON (h.project_id = d.project_id)
                           WHEN MATCHED THEN
                            UPDATE SET 
                             project_owner       = '{0}'
                            ,branch_id           = '{1}'
                            ,status_id           = '{2}'
                            ,product_id          = '{3}'
                            ,start_date          = to_date('{4}','mm/dd/yyyy')
                            ,maint_date          = sysdate
                            ,maint_user_id       = '{5}'
                            ,savings             = '{6}'
                            ,capx                = '{7}'
                            ,expense             = '{8}'
                            ,payback             = '{9}'
                            ,current_year_savings= '{10}'
                            ,TEAM_MEMBERS        = '{11}'
                             /*just want to save projecttitle if in eng, otherwise save the eng version*/
                            ,PROJECT_TITLE_ENG   = decode('{16}','ENG','{13}',nvl('{12}','{13}'))
                            ,key_words           = '{14}'
                            ,deleted             = '{15}'
                            ,process_id          = '{18}'
                            ,currency_id         = '{19}'
                            ,template            = '{20}'
                       WHEN NOT MATCHED THEN
                           INSERT
                          (project_id, project_owner, branch_id, status_id, product_id, start_date, create_date, create_user_id, maint_date, maint_user_id, savings, capx, expense, payback, current_year_savings, 
                           TEAM_MEMBERS,PROJECT_TITLE_ENG,key_words,deleted,DEFUALT_LANG,process_id,currency_id,template)
                        values
                          ({17}, '{0}','{1}','{2}','{3}', to_date('{4}','mm/dd/yyyy') , sysdate, '{5}', sysdate, '{5}',
                           '{6}','{7}','{8}','{9}','{10}','{11}',nvl('{12}','{13}'),'{14}','{15}','{16}','{18}','{19}','{20}')"
                           , aProjectOwnerID
                           , aBranchID
                           , Request.Form["selStatus"]
                           , Request.Form["selProduct"]
                           , Request.Form["inputStartDate"]
                           , theCommon.getUser()  //{5}
                           , aInputSaving//Convert.ToString(Convert.ToSingle(Request.Form["inputSavings"].Replace(",", "")) * aRate)
                           , aInputCapx//Convert.ToString(Convert.ToSingle(Request.Form["inputCapx"].Replace(",", "")) * aRate)
                           , aInputExpense//Convert.ToString(Convert.ToSingle(Request.Form["inputExpense"].Replace(",", "")) * aRate)
                           , Request.Form["inputPayback"].Replace(",", "")
                           , aCurrYearSavings //{10}   
                           , WSString.FilterSQL(Request.Form["hiddenTeamMembers"])
                           , WSString.FilterSQL(Request.Form["inputProjectTitleEnglish"])
                           , WSString.FilterSQL(Request.Form["inputProjectTitle"])
                           , WSString.FilterSQL(Request.Form["textareaKeyWords"])
                           , theDelete //15
                           , theCommon.getLang()
                           , aProjectID 
                           , Request.Form["selProcess"]
                           , Request.Form["selCurrency"]
                           , aTemplate //20 
                           ));

                //update detail 
                aQuery.Add(@"MERGE INTO pip_detail h
                                 USING (SELECT  " + aProjectID + @" project_id
                                              ,'" + theCommon.getLang() + @"' lang
                                              ,'" + WSString.FilterSQL(Request.Form["inputProjectTitle"]) + @"' as project_title 
                                           FROM dual) d
                                        ON (h.project_id = d.project_id AND h.lang = d.lang)
                                        WHEN MATCHED THEN
                                          UPDATE
                                             SET h.project_title = d.project_title
                                        WHEN NOT MATCHED THEN
                                          INSERT
                                            (h.project_id
                                            ,h.lang
                                            ,h.project_title)
                                          VALUES
                                            (d.project_id
                                            ,d.lang
                                            ,d.project_title)");
                

                #endregion

                #region merge ms rows

                //update ms rows
                for (int i = 0; i < aMSRowCT; i++)
                {
                    if (Request.Form["selSavingsCategory" + i.ToString()] != null)
                    {
                        //merge ms header
                        aQuery.Add(string.Format(
                                    @"MERGE INTO pip_ms_header h
                                         USING (SELECT {0} AS project_id
                                                      ,{13} AS SAVINGS_CATEGORY_ID
                                                 FROM dual) d
                                        ON (h.project_id = d.project_id AND h.SAVINGS_CATEGORY_ID = d.SAVINGS_CATEGORY_ID)
                                        WHEN MATCHED THEN
                                          UPDATE
                                             SET monthly_savings_1   = '{1}'
                                                ,monthly_savings_2   = '{2}'
                                                ,monthly_savings_3   = '{3}'
                                                ,monthly_savings_4   = '{4}'
                                                ,monthly_savings_5   = '{5}'
                                                ,monthly_savings_6   = '{6}'
                                                ,monthly_savings_7   = '{7}'
                                                ,monthly_savings_8   = '{8}'
                                                ,monthly_savings_9   = '{9}'
                                                ,monthly_savings_10  = '{10}'
                                                ,monthly_savings_11  = '{11}'
                                                ,monthly_savings_12  = '{12}'
                                        WHEN NOT MATCHED THEN
                                          INSERT
                                            (h.project_id
                                            ,h.SAVINGS_CATEGORY_ID
                                            ,h.monthly_savings_1
                                            ,h.monthly_savings_2
                                            ,h.monthly_savings_3
                                            ,h.monthly_savings_4
                                            ,h.monthly_savings_5
                                            ,h.monthly_savings_6
                                            ,h.monthly_savings_7
                                            ,h.monthly_savings_8
                                            ,h.monthly_savings_9
                                            ,h.monthly_savings_10
                                            ,h.monthly_savings_11
                                            ,h.monthly_savings_12)
                                          VALUES
                                            (d.project_id
                                            ,d.SAVINGS_CATEGORY_ID
                                            ,'{1}'
                                            ,'{2}'
                                            ,'{3}'
                                            ,'{4}'
                                            ,'{5}'
                                            ,'{6}'
                                            ,'{7}'
                                            ,'{8}'
                                            ,'{9}'
                                            ,'{10}'
                                            ,'{11}'
                                            ,'{12}')"
                                      , aProjectID        
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS1_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS2_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS3_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS4_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS5_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS6_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS7_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS8_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS9_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS10_"+ i.ToString()].Replace(",", "")) * aRate) //10
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS11_"+ i.ToString()].Replace(",", "")) * aRate)
                                      , Convert.ToString(Convert.ToSingle(Request.Form["inputMS12_" + i.ToString()].Replace(",", "")) * aRate)
                                      , Request.Form["selSavingsCategory"+ i.ToString()] 
                        ));

                        aSC += (aSC == "" ? "" : ",") + Request.Form["selSavingsCategory" + i.ToString()];
                    }
                }
                if (aSC != "")
                {
                    //remove current ms rows
                    aQuery.Add(" DELETE from pip_ms_header WHERE project_id = " + aProjectID +
                                                " AND SAVINGS_CATEGORY_ID not in (" + aSC + ")");
                }
                #endregion             

                #region merge imp plan rows

                //update imp plan rows
                for (int i = 0; i < aImpPlanRowCT; i++)
                {
                    if (Request.Form["inputAction" + i.ToString()] != null)
                    {
                        if (theCopy)
                        {
                            aResponsible = "";
                            aTargetDate = "";
                            aActionStatus = ConfigurationManager.AppSettings["defaultActionStatus"];
                        }
                        else
                        {
                            //aResponsible = Request.Form["inputResponsible" + i.ToString()];
                            aResponsible = WSString.FilterSQL(Request.Form["selResponsible" + i.ToString()]);
                            aTargetDate = Request.Form["inputTargetDate" + i.ToString()];
                            aActionStatus = Request.Form["selActionStatus" + i.ToString()];
                        }

                        //insert imp plan header
                        aQuery.Add(@"MERGE INTO pip_imp_plan_header h
                                         USING (SELECT " + aProjectID + @" AS project_id
                                                     ," + aRowID + @" AS row_id
                                                     ,'" + aResponsible + @"' AS responsible
                                                     ,to_date('" + aTargetDate + @"','mm/dd/yyyy') AS target_date
                                                     ,'" + aActionStatus + @"' as status_id
                                                 FROM dual) d
                                        ON (h.project_id = d.project_id AND h.row_id = d.row_id)
                                        WHEN MATCHED THEN
                                          UPDATE
                                             SET h.responsible = d.responsible
                                                ,h.target_date = d.target_date
                                                ,h.status_id = d.status_id
                                        WHEN NOT MATCHED THEN
                                          INSERT
                                            (h.project_id
                                            ,h.row_id
                                            ,h.responsible
                                            ,h.target_date
                                            ,h.status_id)
                                          VALUES
                                            (d.project_id
                                            ,d.row_id
                                            ,d.responsible
                                            ,d.target_date
                                            ,d.status_id)"
                                    );

                        //insert imp plan detail
                        aQuery.Add(@"MERGE INTO pip_imp_plan_detail h
                                            USING (SELECT " + aProjectID + @" AS project_id
                                                     ," + aRowID + @" AS row_id
                                                     ,'" + theCommon.getLang() + @"' AS lang
                                                     ,'" + WSString.FilterSQL(Request.Form["inputAction" + i.ToString()]) + @"' AS action
                                                     FROM dual) d
                                            ON (h.project_id = d.project_id AND h.row_id = d.row_id AND h.lang = d.lang)
                                            WHEN MATCHED THEN
                                              UPDATE SET h.action = d.action
                                            WHEN NOT MATCHED THEN
                                              INSERT
                                                (h.project_id
                                                ,h.row_id
                                                ,h.lang
                                                ,h.action)
                                              VALUES
                                                (d.project_id
                                                ,d.row_id
                                                ,d.lang
                                                ,d.action)"
                                    );
                        aRowID++;
                    }
                }
                //remove current imp plan rows
                aQuery.Add(" DELETE from pip_imp_plan_detail WHERE project_id = " + aProjectID +
                                           " AND row_id >= " + aRowID);
                aQuery.Add(" DELETE from pip_imp_plan_header WHERE project_id = " + aProjectID +
                                            " AND row_id >= " + aRowID);
                

                #endregion

                #region merge file rows

                //delete files
                aRowID = 0;
                for (int i=0; i < aFilesCT; i++){
                    //file count needs to be in order with no gaps
                    //this update file to current number, used if file before was deleted
                    if (aRowID != i)
                    {
                        aQuery.Add(string.Format(
                                   @"update pip_files set row_id='{1}' where project_id='{0}' and row_id='{2}'"
                                  , aProjectID
                                  , aRowID
                                  , i.ToString()
                                  ));
                    }
                    if (Request.Form["cbFiles"+i.ToString()] != null){
                        aQuery.Add(string.Format(
                                    @"delete from pip_files where project_id='{0}' and row_id='{1}'"
                                   , aProjectID
                                   , aRowID
                                   ));
                        aRowID--;
                    }
                    
                    aRowID++;
                }

                //insert new files
                if (fileInput.HasFile)
                {
                    //checkt to keep from duplicating a file if page is refreshed
                    //could use this below too when file data is being saved, but doesn't really matter
                    aDR = theCommon.GetReader("select count(1) as ct from pip_files where project_id = '"+aProjectID+"' and row_id = '"+aRowID+"'");
                    
                    if (aDR.Read() && aDR["ct"].ToString() == "0")
                    {
                        aQuery.Add(string.Format(
                                    @"insert into pip_files (project_id, row_id, file_name,file_type,lang) values ({0}, '{1}','{2}','{3}','{4}')"
                                   , aProjectID
                                   , aRowID
                                   , WSString.FilterSQL(fileInput.FileName)
                                   , WSString.FilterSQL(fileInput.PostedFile.ContentType)
                                   , theCommon.getLang()
                                   ));
                    }
                }
//                aQuery.Add(@"MERGE INTO pip_files h
//                                            USING (SELECT " + aProjectID + @" AS project_id
//                                                     ," + aRowID + @" AS row_id
//                                                     ,'" + theCommon.getLang() + @"' AS lang
//                                                     ,'" + WSString.FilterSQL(Request.Form["inputAction" + i.ToString()]) + @"' AS action
//                                                     FROM dual) d
//                                            ON (h.project_id = d.project_id AND h.row_id = d.row_id AND h.lang = d.lang)
//                                            WHEN MATCHED THEN
//                                              UPDATE SET h.action = d.action
//                                            WHEN NOT MATCHED THEN
//                                              INSERT
//                                                (h.project_id
//                                                ,h.row_id
//                                                ,h.lang
//                                                ,h.action)
//                                              VALUES
//                                                (d.project_id
//                                                ,d.row_id
//                                                ,d.lang
//                                                ,d.action)"
//                                    );
                #endregion

                //error ck
                //if ok update project id otherwise show fail message
                if (theCommon.ExecuteCommand(aQuery))
                {
                    inputProjectID.Value = aProjectID;

                     //,'" + WSString.FilterSQL(Request.Form["textareaDesc"]) + @"' as detailed_description
                     //                         ,'" + WSString.FilterSQL(Request.Form["textareaCurrState"]) + @"' as current_state
                     //                         ,'" + WSString.FilterSQL(Request.Form["textareaFutureState"]) + @"' as future_state
                     //                         ,'" + WSString.FilterSQL(Request.Form["textareaSavingsCalc"]) + @"' as savings_calc
                    theCommon.execProc("pip_detail", "detailed_description", Request.Form["textareaDesc"], "project_id", aProjectID);
                    if (!theCopy)
                    {
                        theCommon.execProc("pip_detail", "current_state", Request.Form["textareaCurrState"], "project_id", aProjectID);
                    }
                    theCommon.execProc("pip_detail", "future_state", Request.Form["textareaFutureState"], "project_id", aProjectID);
                    theCommon.execProc("pip_detail", "savings_calc", Request.Form["textareaSavingsCalc"], "project_id", aProjectID);

                    if (fileInput.HasFile)
                    {
                        theCommon.execProcBlob("pip_files", "file_data", fileInput.FileBytes, "project_id", aProjectID, "row_id", aRowID.ToString());
                    }
                }
                else
                {
                    labelMSG.InnerHtml = theCommon.getText("labelMSGUpdateFail");
                    labelMSG.Attributes.Add("class", "msgDivError");
                }
                
        //    }

            if (theDelete == "Y")
            {
                aProjectID = "";
               // loadData(theCommon, aProjectID);
            }
            //else
            //{
            //    //add imp plan rows back to page
            //    writeMSTable(theCommon,aImpPlanRowCT, false, false,1);
            //    writeImpPlanTable(theCommon,aImpPlanRowCT, false, false);
            //    writeFiles(theCommon, aProjectID,false);
            //    //setSecurity(false);
            //}

            //if don't loaddata here targets don't sort
            //which is much better then lose all saved data if hits check
            loadData(theCommon, aProjectID);
            
            //fileInput.PostedFile.SaveAs(ConfigurationManager.AppSettings["gAppPath"]+"\\asdF");

            return aProjectID;
        }

        protected void writeMSTable(common theCommon, int theNumRows, bool theInsert, bool theReadonly, float theRate)
        {
            #region log

            if (theCommon.getLogging())
            {
                theCommon.logError("index.aspx.cs", "writeMSTable", "theNumRows:" + theNumRows.ToString() + "  theInsert:" + theInsert.ToString() + "  theReadonly:" + theReadonly.ToString(), "", "LOG");
            }

            #endregion

            //int currRow = 0;
            int i = 0;
            if (theRate == 1)
            {
                for (; i < theNumRows; i++)
                {
                    if (Request.Form["inputMS1_" + i.ToString()] != null)
                    {
                        writeMSRow(theCommon, i.ToString(), Request.Form["selSavingsCategory" + i.ToString()], Request.Form["inputMS1_" + i.ToString()], Request.Form["inputMS2_" + i.ToString()], Request.Form["inputMS3_" + i.ToString()], Request.Form["inputMS4_" + i.ToString()], Request.Form["inputMS5_" + i.ToString()], Request.Form["inputMS6_" + i.ToString()], Request.Form["inputMS7_" + i.ToString()], Request.Form["inputMS8_" + i.ToString()], Request.Form["inputMS9_" + i.ToString()], Request.Form["inputMS10_" + i.ToString()], Request.Form["inputMS11_" + i.ToString()], Request.Form["inputMS12_" + i.ToString()], theReadonly);
                    }
                }
            }
            else
            {
                for (; i < theNumRows; i++)
                {
                    if (Request.Form["inputMS1_" + i.ToString()] != null)
                    {
                        writeMSRow(theCommon, i.ToString(), Request.Form["selSavingsCategory" + i.ToString()], Convert.ToString(Convert.ToSingle(Request.Form["inputMS1_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS2_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS3_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS4_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS5_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS6_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS7_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS8_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS9_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS10_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS11_" + i.ToString()]) * theRate), Convert.ToString(Convert.ToSingle(Request.Form["inputMS12_" + i.ToString()]) * theRate), theReadonly);
                    }
                }
            }

            if (theInsert)
            {
                writeMSRow(theCommon, i.ToString(), "12","", "", "","", "", "","", "", "","", "", "", theReadonly);
                i++;
            }

            hiddenMSRowCT.Value = i.ToString();
            
        }

        protected void writeMSRow(common theCommon, string theRowNum, string theSavingsCalc, string theMS1, string theMS2,string theMS3,string theMS4,string theMS5,string theMS6,string theMS7,string theMS8,string theMS9,string theMS10,string theMS11,string theMS12, bool theReadonly)
        {
            #region log

            if (theCommon.getLogging())
            {
                theCommon.logError("index.aspx.cs", "writeMSRow", "theRowNum:" + theRowNum + "  theSavingsCalc:" + theSavingsCalc + "  theMS1:" + theMS1 + "  theMS2:" + theMS2 + "  theMS3:" + theMS3 + "  theMS4:" + theMS4 + "  theMS5:" + theMS5 + "  theMS6:" + theMS6 + "  theMS7:" + theMS7 + "  theMS8:" + theMS8 + "  theMS9:" + theMS9 + "  theMS10:" + theMS10 + "  theMS11:" + theMS11 + "  theMS12:" + theMS12 + "  theReadonly:" + theReadonly.ToString(), "", "LOG");
            }

            #endregion

            HtmlInputButton aButton;
            HtmlInputText aInput;
            DropDownList aSel;
            HtmlTableRow aTR;
            HtmlTableCell aTC;

            aTR = new HtmlTableRow();
            //deleting row by parentnode in js
            //if add container make sure to modify it

            //delete button
            aTC = new HtmlTableCell();
            aButton = new HtmlInputButton();
            aButton.ID = "buttonMSDel" + theRowNum;
            aButton.Attributes.Add("onclick", "deleteMSRow(this)");
            aButton.Value = theCommon.getText("buttonDel");
            if (theReadonly)
            {
                aButton.Attributes.Add("disabled", "true");
            }
            aTC.Controls.Add(aButton);
            aTR.Controls.Add(aTC);
            //savings calc select
            aTC = new HtmlTableCell();
            aTC.InnerHtml = "&nbsp;";
            aSel = new DropDownList();
            aSel.ID = "selSavingsCategory" + theRowNum;
            aSel.ToolTip = theCommon.getText("tipCategory");
            aSel.DataSource = (DataView)Application[theCommon.getLang() + "SavingsSel"];
            aSel.DataTextField = "member_desc";
            aSel.DataValueField = "id";
            aSel.SelectedValue = theSavingsCalc;
            aSel.DataBind();
            if (theReadonly)
            {
                aSel.Attributes.Add("disabled", "true");
                aSel.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aSel);
            aTR.Controls.Add(aTC);
            //ms1 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS1_" + theRowNum;
            aInput.Value = theMS1;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress","alphaBlock();");
            aInput.Attributes.Add("onchange","calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms2 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS2_" + theRowNum;
            aInput.Value = theMS2;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms3 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS3_" + theRowNum;
            aInput.Value = theMS3;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms4 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS4_" + theRowNum;
            aInput.Value = theMS4;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms5 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS5_" + theRowNum;
            aInput.Value = theMS5;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms6 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS6_" + theRowNum;
            aInput.Value = theMS6;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms7 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS7_" + theRowNum;
            aInput.Value = theMS7;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms8 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS8_" + theRowNum;
            aInput.Value = theMS8;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms9 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS9_" + theRowNum;
            aInput.Value = theMS9;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms10 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS10_" + theRowNum;
            aInput.Value = theMS10;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms11 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS11_" + theRowNum;
            aInput.Value = theMS11;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //ms12 input
            aTC = new HtmlTableCell();
            aInput = new HtmlInputText();
            aInput.ID = "inputMS12_" + theRowNum;
            aInput.Value = theMS12;
            aInput.Size = 3;
            aInput.Attributes.Add("onkeypress", "alphaBlock();");
            aInput.Attributes.Add("onchange", "calcValues();");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);

            tableBodyMSRow.Controls.Add(aTR);
        }

        protected void writeImpPlanTable(common theCommon, int theNumRows, bool theInsert, bool theReadonly)
        {
            #region log

            if (theCommon.getLogging())
            {
                theCommon.logError("index.aspx.cs", "writeImpPlanTable", "theNumRows:" + theNumRows.ToString() + "  theInsert:" + theInsert.ToString() + "  theReadonly:" + theReadonly.ToString(), "", "LOG");
            }

            #endregion

            //int currRow = 0;
            int i = 0;
            for (; i < theNumRows; i++)
            {
                if (Request.Form["inputAction" + i.ToString()] != null)
                {
                    writeImpPlanRow(theCommon, i.ToString(), Request.Form["inputAction" + i.ToString()], Request.Form["selResponsible" + i.ToString()], Request.Form["inputTargetDate" + i.ToString()], Request.Form["selActionStatus" + i.ToString()], theReadonly);
                    //writeImpPlanRow(theCommon, i.ToString(), Request.Form["inputAction" + i.ToString()], Request.Form["inputResponsible" + i.ToString()], Request.Form["inputTargetDate" + i.ToString()], Request.Form["selActionStatus" + i.ToString()], theReadonly);
                }
            }

            if (theInsert)
            {
                writeImpPlanRow(theCommon, i.ToString(), "", "", "",ConfigurationManager.AppSettings["defaultActionStatus"], theReadonly);
                i++;
            }

            hiddenImpPlanRowCT.Value = i.ToString();
            
        }

        protected void writeImpPlanRow(common theCommon, string theRowNum, string theAction, string theResponsible, string theTargetDate, string theStatus, bool theReadonly)
        {
            #region log

            if (theCommon.getLogging())
            {
                theCommon.logError("index.aspx.cs", "writeImpPlanRow", "theRowNum:" + theRowNum + "  theAction:" + theAction + "  theResponsible:" + theResponsible + "  theTargetDate:" + theTargetDate + "  theReadonly:" + theReadonly.ToString(), "", "LOG");
            }

            #endregion

            HtmlInputButton aButton;
            TextBox aInput;
            CalendarExtender aCE;
            //AutoCompleteExtender aACE;
            DropDownList aSel;
            HtmlTableRow aTR;
            HtmlTableCell aTC;

            aTR = new HtmlTableRow();
            //deleting row by parentnode in js
            //if add container make sure to modify it

            //delete button
            aTC = new HtmlTableCell();
            aButton = new HtmlInputButton();
            aButton.ID = "buttonDel" + theRowNum;
            aButton.Attributes.Add("onclick", "deleteImpPlanRow(this)");
            aButton.Value = theCommon.getText("buttonDel");
            if (theReadonly)
            {
                aButton.Attributes.Add("disabled", "true");
            }
            aTC.Controls.Add(aButton);
            aTR.Controls.Add(aTC);
            //action input
            aTC = new HtmlTableCell();
            aInput = new TextBox();
            aInput.ID = "inputAction" + theRowNum;
            aInput.Text = theAction;
            aInput.ToolTip = theCommon.getText("tipAction");
            aInput.Attributes.Add("size", "65");
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aTR.Controls.Add(aTC);
            //Responsible Select
            aTC = new HtmlTableCell();
            aSel = new DropDownList();
            aSel.ID = "selResponsible" + theRowNum;
            aSel.Width = 205;
            aTC.Controls.Add(aSel);

            aSel.DataSource = (DataTable)Application["PeopleSel"];
            aSel.DataTextField = "full_name";
            aSel.DataValueField = "full_name";
            aSel.DataBind();
            aSel.Items.Insert(0, new ListItem("", ""));
            aSel.SelectedValue = theResponsible;
            //  inputProjectOwner.Text = Request.Form["inputProjectOwner"] == "" ? aCommon.getUserName() : Request.Form["inputProjectOwner"];

            //aInput = new TextBox();
            //aInput.ID = "inputResponsible" + theRowNum;
            //aInput.ToolTip = theCommon.getText("tipResponsible");
            //aInput.Text = theResponsible;
            if (theReadonly)
            {
                aSel.Attributes.Add("disabled", "true");
                aSel.Attributes.Add("class", "readonly");
                //aInput.Attributes.Add("readonly", "readonly");
                //aInput.Attributes.Add("class", "readonly");
            } 
            //aTC.Controls.Add(aInput);
            //aACE = new AutoCompleteExtender();
            //aACE.ID = "inputResponsibleACE" + theRowNum;
            //aACE.TargetControlID = aInput.ID;
            //aACE.MinimumPrefixLength = 1;
            //aACE.ServiceMethod = "getUser";
            //aACE.ServicePath = ConfigurationManager.AppSettings["peopleServicePath"];
            //aTC.Controls.Add(aACE);
            aTR.Controls.Add(aTC);
            //Target Date input 
            aTC = new HtmlTableCell();
            aInput = new TextBox();
            aInput.ID = "inputTargetDate" + theRowNum;
            aInput.Text = theTargetDate;
            aInput.ToolTip = theCommon.getText("tipTargetDate");
            aInput.Width = 75;
            if (theReadonly)
            {
                aInput.Attributes.Add("readonly", "readonly");
                aInput.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aInput);
            aCE = new CalendarExtender();
            aCE.ID = "calExtTargetDate" + theRowNum;
            aCE.TargetControlID = "inputTargetDate" + theRowNum;
            aTC.Controls.Add(aCE);
            aTR.Controls.Add(aTC);
            //action status select
            aTC = new HtmlTableCell();
            aSel = new DropDownList();
            aSel.ID = "selActionStatus" + theRowNum;
            aSel.DataSource = (DataView)Application[theCommon.getLang()+"ActionStatusSel"];
            aSel.DataTextField = "member_desc";
            aSel.DataValueField = "id";
            aSel.SelectedValue = theStatus;
            aSel.DataBind();
            if (theReadonly)
            {
                aSel.Attributes.Add("disabled", "true");
                aSel.Attributes.Add("class", "readonly");
            }
            aTC.Controls.Add(aSel);
            aTR.Controls.Add(aTC);

            //branch select
            

            tableBodyImpPlanRow.Controls.Add(aTR);
        }

            //        FileUpload aFL = new FileUpload();
            //aFL.ID = "testFL";
            //mEntryForm.Controls.Add(aFL);


        protected void writeFiles(common theCommon, string theProject, bool theReadonly)
        {
            #region log

            if (theCommon.getLogging())
            {
                theCommon.logError("index.aspx.cs", "writeFiles", "theProject:" + theProject + "  theReadOnly:" + theReadonly.ToString(), "", "LOG");
            }

            #endregion

            tableBodyFiles.Controls.Clear();
            int aCT = 0;
            //changed link below to rowid since unescaped chars in url cause problems
            OracleDataReader aDR = theCommon.GetReader(@"SELECT file_name, row_id
                                          FROM pip_files
                                         WHERE project_id = '" + theProject + "'");

            
            if (aDR != null)
            {
                HtmlInputCheckBox aCB ;
                HtmlTableCell aTC;
                HtmlTableRow aTR;

                while (aDR.Read())
                {
                    //write remove header
                    if (aCT == 0)
                    {
                        aTR = new HtmlTableRow();
                        aTC = new HtmlTableCell();
                        aTC.InnerHtml = theCommon.getText("labelRemoveFiles");
                        aTC.Attributes.Add("class", "labelR");
                        aTR.Controls.Add(aTC);
                        tableBodyFiles.Controls.Add(aTR);
                    }

                    aTR = new HtmlTableRow();
                    //file button
                    aTC = new HtmlTableCell();
                    aTC.Attributes.Add("class", "inputR");
                    aCB = new HtmlInputCheckBox();
                    aCB.ID = "cbFiles"+aCT.ToString();
                    if (theReadonly)
                    {
                        aCB.Disabled = true;
                    }
                    aTC.Controls.Add(aCB);
                    aTR.Controls.Add(aTC);
                    //file link
                    aTC = new HtmlTableCell();
                    aTC.Attributes.Add("class", "input");
                    aTC.InnerHtml += string.Format("<a target='_blank' href='{0}/pt/portlets/pip/downloadFile.aspx?pid={1}&rowid={2}'>{3}</a><br>"
                                        , ConfigurationManager.AppSettings["gRootWebPath"]
                                        , theProject
                                        , aDR["row_id"].ToString()
                                        , aDR["file_name"].ToString()
                                        );
                    aTR.Controls.Add(aTC);

                    tableBodyFiles.Controls.Add(aTR);
                    aCT++;
                }
                aDR.Close();
                aDR.Dispose();
            }

            hiddenFilesCT.Value = aCT.ToString();
        }
    }
}
