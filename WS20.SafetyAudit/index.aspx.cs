using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Devart.Data.Oracle;
using WS20.Framework;
using System.Data.OleDb;

namespace WS20.SafetyAudit
{
    public partial class Index : System.Web.UI.Page
    {
        string mDataSource = " ";  // Feith datasource
        string mForm = " ";  // FormSetID 
        ArrayList mQuestions = new ArrayList();
        string mFormID = " ";  //Form ID 

        protected void Page_Load(object sender, EventArgs e)
        {


            #region setup headers and footers

            //mWSHeader.SetPageName("Update Safety Audit Information");
            //mWSHeader.SetAppTitleHtml("Version 1.0.0");
            //mHeaderClientID.Value = mWSHeader.ClientID;
            //mWSHeader.AddMenuItem("Main", "#");
            //mWSHeader.AppLogoSrc = "safetyAuditInformation.gif";
            //mWSHeader.SetPageName("<div style='height:60px;'></div>");

            #endregion //end of header/footer setup

            WSDBConnection mConn = new WSDBConnection();

            if (mConn.Open("FDD", 0))
            {

                try
                {
                    loadsafetyquestions(mConn);
                    createQuestlist(mConn);
                    loadbranchsystemattrb(mConn);
                    createRegionalaudit(mConn);
                    loadPercentage(mConn);
                }

                catch (Exception ex)
                {

                    logerror("Page_Load", "", ex.ToString());
                    div1.InnerHtml = "Load Failed!!";
                }


            }
            mConn.Close();
        }

        protected void upd_but_Click(object sender, EventArgs e)
        {
            div1.InnerHtml = "Update Complete";

            WSDBConnection mConn = null;
            try
            {
                mConn = new WSDBConnection();
                if (mConn.Open("FDD", 0))
                {
                    mConn.BeginTransaction();
                    mConn.ExecuteCommand("update fdd.safety_audit_percentage set end_dt = sysdate - 1 where end_dt = '31-DEC-9999' ");

                    mConn.ExecuteCommand(string.Format(@"insert into fdd.safety_audit_percentage
                                                        values('{0}', '{1}', sysdate, sysdate, '31-DEC-9999')"
                                                        , Request.Form["pct_txt"]
                                                        , WSSecurityPackage.NTLookup.GetNTUserName(this)));

                    pct_txt.Value = Request.Form["pct_txt"];

                    mConn.CommitTransaction();
                }

                loadPercentage(mConn);
            }
            catch (Exception ex)
            {
                mConn.RollbackTransaction();
                logerror("upd_but_Click", "", ex.ToString());
                div1.InnerHtml = "Update Failed!!";
            }
            finally
            {
                if (mConn != null) { mConn.Close(); }
            }
        }

        protected void upd_question_click(object sender, EventArgs e)
        {
            WSDBConnection mConn = null;
            OracleDataReader a_dr = null;

            int rec_cnt = 0;
            int max_id = 0;
            string aOldsec = "0";
            string aCommand = "";

            try
            {
                mConn = new WSDBConnection();
                if (mConn.Open("FDD", 0))
                {


                    rec_cnt = Convert.ToInt32(hid_quest_cnt.Value);


                    a_dr = mConn.GetReader("select max(question_id) + 1 as mx_id from fdd.safety_branch_audit_categories");

                    if (a_dr.Read())
                    {
                        max_id = Convert.ToInt32(a_dr["mx_id"]);
                    }
                    else
                    {
                        throw new Exception("Could not get question count");
                    }

                    for (int aCurrRec = 1; aCurrRec <= rec_cnt; aCurrRec++)
                    {

                        aCommand = string.Format(@"update fdd.safety_branch_audit_categories
                                                          set description = '{0}',
                                                              weight = '{1}',
                                                              question_number = '{2}',
                                                              section = '{3}',
                                                              update_date = trunc(sysdate),
                                                              update_user = '{5}'
                                                        where question_id = '{4}'"
                                                       , WSString.FilterSQL(Request.Form["destxa" + aCurrRec.ToString()].ToString())
                                                       , Request.Form["weitxt" + aCurrRec.ToString()]
                                                       , Request.Form["qsttxt" + aCurrRec.ToString()]
                                                       , Request.Form["sectxt" + aCurrRec.ToString()]
                                                       , Request.Form["qidtxt" + aCurrRec.ToString()]
                                                       , WSSecurityPackage.NTLookup.GetNTUserName(this));

                        mConn.ExecuteCommand(aCommand);

                        if (Request.Form["ststxt" + aCurrRec.ToString()] == "Inactive" &&
                            Request.Form["hststxt" + aCurrRec.ToString()] == "Active")
                        {
                            aCommand = string.Format(@"update fdd.safety_branch_audit_categories 
                                                                 set end_date = trunc(sysdate),
                                                                     status   = '{1}',
                                                                     update_date = trunc(sysdate),
                                                                     update_user = '{2}' 
                                                              where question_id = '{0}'"
                                                             , Request.Form["qidtxt" + aCurrRec.ToString()]
                                                             , "Inactive"
                                                             , WSSecurityPackage.NTLookup.GetNTUserName(this));

                            mConn.ExecuteCommand(aCommand);
                        }
                        else if (Request.Form["ststxt" + aCurrRec.ToString()] == "Active" &&
                                 Request.Form["hststxt" + aCurrRec.ToString()] == "Inactive")
                        {
                            aCommand = string.Format(@"insert into fdd.safety_branch_audit_categories
                                                              (section, description, weight, question_number, status, begin_date, end_date, question_id, update_user, update_date)
                                                              values('{0}', '{1}', '{2}', '{3}', '{4}', trunc(sysdate), '{5}', '{6}', '{7}', trunc(sysdate))"
                                                             , Request.Form["sectxt" + aCurrRec.ToString()]
                                                             , WSString.FilterSQL(Request.Form["destxa" + aCurrRec.ToString()].ToString())
                                                             , Request.Form["weitxt" + aCurrRec.ToString()]
                                                             , Request.Form["qsttxt" + aCurrRec.ToString()]
                                                             , "Active"
                                                             , "31-DEC-9999"
                                                             , max_id
                                                             , WSSecurityPackage.NTLookup.GetNTUserName(this));

                            mConn.ExecuteCommand(aCommand);

                            max_id = max_id + 1;
                        }

                        if (Request.Form["hedtxt" + aCurrRec.ToString()] != null)
                        {
                            aOldsec = aCurrRec.ToString();

                            aCommand = string.Format(@"update fdd.safety_audit_headings
                                                       set description = '{0}'
                                                       where head_id = '{1}'"
                                                       , WSString.FilterSQL(Request.Form["hedtxt" + aCurrRec.ToString()])
                                                       , aOldsec);

                            mConn.ExecuteCommand(aCommand);
                        }

                    } //End Loop


                    div1.InnerHtml = "Update Complete";
                    pct_txt.Value = Request.Form["pct_txt"];
                    loadsafetyquestions(mConn);

                }
            }
            catch (Exception ex)
            {
                logerror("upd_question_click", aCommand, ex.ToString());
                div1.InnerHtml = "Update Failed!!";
            }
            finally
            {
                if (mConn != null) { mConn.Close(); }
                if (a_dr != null) { a_dr.Close(); a_dr.Dispose(); }
            }
        }

        protected void loadbranchsystemattrb(WSDBConnection mCommon)
        {
            HtmlTableRow aTR;
            HtmlTableCell aTC;
            HtmlTable aTT;
            OracleDataReader aDR;
            OracleDataReader aDR2 = null;
            HtmlInputCheckBox aCB = null;

            int asyscnt = 0;

            branch_main.Controls.Clear();


            aDR = mCommon.GetReader(@"select sl.system_desc
                                        from system_listing sl
                                    order by sl.system_desc");

            aTT = new HtmlTable();
            aTT.ID = "LDBRATR";
            aTT.CellPadding = 15;
            aTT.Border = 1;

            aTR = new HtmlTableRow();
            aTC = new HtmlTableCell("td");
            aTC.Style.Add("font-size", "20px");
            aTC.InnerHtml = "Branch";
            aTR.Controls.Add(aTC);

            while (aDR.Read())
            {
                aTC = new HtmlTableCell("td");
                aTC.Style.Add("font-size", "20px");
                aTC.InnerHtml = aDR["system_desc"].ToString();
                aTR.Controls.Add(aTC);
                aTT.Controls.Add(aTR);

                asyscnt++;

            }

            aDR = mCommon.GetReader(@"select cost_center, internet_display_name
                                        from branch_header bl,
                                             t_locationdir ld
                                        where bl.cost_center = ld.loc_cc
                                           and ld.status_id = 0
                                           and ld.type_id = 3
                                    order by internet_display_name");

            while (aDR.Read())
            {

                aTR = new HtmlTableRow();
                aTC = new HtmlTableCell("td");
                aTC.InnerHtml = aDR["internet_display_name"].ToString();
                aTR.Controls.Add(aTC);

                for (int a = 0; a < asyscnt; a++)
                {
                    aDR2 = mCommon.GetReader(string.Format(@"select count(1) vl from branch_listing where system_id = '{0}' and cost_center = '{1}'"
                                             , a
                                             , aDR["cost_center"].ToString()));

                    aTC = new HtmlTableCell("td");
                    aCB = new HtmlInputCheckBox();


                    while (aDR2.Read())
                    {

                        if (Convert.ToInt32(aDR2["vl"]) == 0)
                        {
                            aCB.ID = aDR["cost_center"].ToString() + a.ToString() + "ins";
                            aCB.Checked = false;

                        }
                        else
                        {
                            aCB.ID = aDR["cost_center"].ToString() + a.ToString() + "del";
                            aCB.Checked = true;

                        }

                        if (aCB.Checked == true)
                        {
                            aTC.BgColor = "#fedcba";
                        }

                    }
                    aTC.Controls.Add(aCB);
                    aTR.Controls.Add(aTC);

                }

                aTT.Controls.Add(aTR);

            }

            branch_main.Controls.Add(aTT);

            aTR.Dispose();
            aTC.Dispose();
            aTT.Dispose();
            aDR.Close();
            aDR.Dispose();
            if (aDR2 != null) { aDR2.Close(); aDR2.Dispose(); }
            if (aCB != null) { aCB.Dispose(); }
        }

        protected void loadsafetyquestions(WSDBConnection mCommon)
        {
            OracleDataReader aDR;  //reader to loop through data
            HtmlTable aTT = null;
            HtmlTableRow aTR;
            HtmlTableCell aTC = null;
            HtmlTextArea aTA;
            HtmlInputText aTX;
            HtmlSelect aSL;
            HtmlInputHidden aTH;
            Label aPageNav;

            int cnt = 0;
            int prior_section = 0;

            question_div.Controls.Clear();

            aDR = mCommon.GetReader(@"Select a.section,
                                             a.description as quest_desc,
                                             a.weight,
                                             a.question_number,
                                             a.status,
                                             a.question_id,
                                             b.description as head_desc
                                      From fdd.safety_branch_audit_categories a,
                                           fdd.safety_audit_headings b
                                      where a.section = b.head_id
                                   order by a.section, a.question_number
                                   ");

            aPageNav = new Label();
            aPageNav.Text = "<br>";

            while (aDR.Read())
            {
                if (prior_section != Convert.ToInt32(aDR["section"]))
                {

                    if (prior_section != 0)
                    {
                        question_div.Controls.Add(aTT);
                    }

                    aTT = new HtmlTable();
                    aTT.ID = "tbl_" + aDR["section"].ToString();
                    aTT.Border = 1;

                    prior_section = Convert.ToInt32(aDR["section"]);
                    // int.TryParse(aDR["section"].ToString(), out prior_section]);

                    aTR = new HtmlTableRow();
                    aTC = new HtmlTableCell("td");
                    aTX = new HtmlInputText();
                    aTC.ColSpan = 5;
                    aTC.Style.Add("text-align", "center");
                    aTC.Style.Add("font-size", "20px");
                    aTX.Value = aDR["head_desc"].ToString();
                    aTX.ID = "hedtxt" + aDR["section"].ToString();
                    aTX.Size = 80;
                    aTC.Controls.Add(aTX);
                    //aTC.InnerHtml = "<b>" + aDR["head_desc"].ToString() + "</b>";
                    aTR.Cells.Add(aTC);
                    aTT.Rows.Add(aTR);
                    aTR = new HtmlTableRow();
                    aTC = new HtmlTableCell("th");
                    aTC.InnerHtml = "Section";
                    aTR.Cells.Add(aTC);
                    aTC = new HtmlTableCell("th");
                    aTC.InnerHtml = "Question Number";
                    aTR.Cells.Add(aTC);
                    aTC = new HtmlTableCell("th");
                    aTC.InnerHtml = "Description";
                    aTR.Cells.Add(aTC);
                    aTC = new HtmlTableCell("th");
                    aTC.InnerHtml = "Weight";
                    aTR.Cells.Add(aTC);
                    aTT.Rows.Add(aTR);
                    aTC = new HtmlTableCell("th");
                    aTC.InnerHtml = "Status";
                    aTR.Cells.Add(aTC);
                    aTT.Rows.Add(aTR);

                    if (Convert.ToInt32(aDR["section"]) != 1)
                    {
                        aTT.Style.Add("display", "none");
                    }



                    aPageNav.Text += string.Format(@"<a href='#' id='{0}Link' {2} onClick=""changeTablePage('{0}')"">{1}</a>&nbsp;&nbsp;"
                                            , aTT.ID
                                            , Convert.ToInt32(aDR["section"])
                                            , Convert.ToInt32(aDR["section"]) == 1 ? "class='selectedPageLink'" : ""
                                            );

                }


                cnt++;

                aTR = new HtmlTableRow();

                aTH = new HtmlInputHidden();
                aTH.ID = "qidtxt" + cnt.ToString();
                aTH.Value = aDR["question_id"].ToString();

                aTC = new HtmlTableCell("td");
                aTX = new HtmlInputText();
                aTX.ID = "sectxt" + cnt.ToString();
                aTX.Value = aDR["section"].ToString();
                aTC.Controls.Add(aTX);
                aTC.Controls.Add(aTH);
                if (aDR["status"].ToString() == "Inactive")
                {
                    aTC.BgColor = "#e4f8ec";
                }

                aTR.Cells.Add(aTC);

                aTC = new HtmlTableCell("td");
                aTX = new HtmlInputText();
                aTX.ID = "qsttxt" + cnt.ToString();
                aTX.Value = aDR["question_number"].ToString();
                aTC.Controls.Add(aTX);
                if (aDR["status"].ToString() == "Inactive")
                {
                    aTC.BgColor = "#e4f8ec";
                }
                aTR.Cells.Add(aTC);

                aTC = new HtmlTableCell("td");
                aTA = new HtmlTextArea();
                aTA.Cols = 70;
                aTA.Rows = 5;
                aTA.ID = "destxa" + cnt.ToString();
                aTA.Value = aDR["quest_desc"].ToString();
                aTC.Controls.Add(aTA);
                if (aDR["status"].ToString() == "Inactive")
                {
                    aTC.BgColor = "#e4f8ec";
                }
                aTR.Cells.Add(aTC);

                aTC = new HtmlTableCell("td");
                aTX = new HtmlInputText();
                aTX.ID = "weitxt" + cnt.ToString();
                aTX.Value = aDR["weight"].ToString();
                aTX.Size = 3;
                aTC.Controls.Add(aTX);
                if (aDR["status"].ToString() == "Inactive")
                {
                    aTC.BgColor = "#e4f8ec";
                }
                aTR.Cells.Add(aTC);

                aTC = new HtmlTableCell("td");
                aSL = new HtmlSelect();
                aSL.ID = "ststxt" + cnt.ToString();
                aSL.Items.Add("Active");
                aSL.Items.Add("Inactive");
                aSL.DataBind();
                aSL.Value = aDR["status"].ToString();
                aSL.Size = 3;
                aTH = new HtmlInputHidden();
                aTH.ID = "hststxt" + cnt.ToString();
                aTH.Value = aDR["status"].ToString();
                aTC.Controls.Add(aSL);
                aTC.Controls.Add(aTH);
                if (aDR["status"].ToString() == "Inactive")
                {
                    aTC.BgColor = "#e4f8ec";
                }
                aTR.Cells.Add(aTC);

                aTT.Rows.Add(aTR);

            }

            hid_quest_cnt.Value = cnt.ToString();
            question_div.Controls.Add(aTT);
            question_div.Controls.Add(aPageNav);
        }

        protected void createQuestlist(WSDBConnection theConnection)
        {
            add_list.DataSource = theConnection.GetDataTable("select head_id, description from fdd.safety_audit_headings order by head_id");
            add_list.DataValueField = "head_id";
            add_list.DataTextField = "description";
            add_list.DataBind();
            add_sec_txt.Value = "";
            add_ques_txt.Value = "";
        }

        protected void loadPercentage(WSDBConnection theConnection)
        {
            OracleDataReader a_dr;


            a_dr = theConnection.GetReader("SELECT UPPER(value) as env FROM v$parameter WHERE UPPER(name) = 'DB_NAME'");

            if (a_dr.Read())
            {
                if (a_dr["env"].ToString() == "FDD")
                {
                    mDataSource = "fdd";
                }
                else
                {
                    mDataSource = "ws85corp";
                }
            }

            a_dr.Close();

            a_dr = theConnection.GetReader("select pct, upmd, usr from fdd.safety_audit_percentage where end_dt = '31-DEC-9999'");
            if (a_dr.Read())
            {
                pct_txt.Value = a_dr["pct"].ToString();
                lst_upd_dt.Value = a_dr["upmd"].ToString();
                upd_by.Value = a_dr["usr"].ToString();
            }

            a_dr.Close();

            a_dr = theConnection.GetReader("select max(formset_id) fsid from fdd.formsiq_info where form_name = 'Auditor Safety Inspection'");
            if (a_dr.Read())
            {
                mForm = a_dr["fsid"].ToString();
            }

            a_dr.Close();

            a_dr = theConnection.GetReader(string.Format(@"select form_id from formid_control where formsetid = '{0}'"
                                                   , mForm));
            if (a_dr.Read())
            {
                mFormID = a_dr["form_id"].ToString();
            }

            a_dr.Close();
        }

        protected void createRegionalaudit(WSDBConnection theConnection)
        {
            brn_lst.DataSource = theConnection.GetDataTable("select internet_display_name from (select internet_display_name from t_locationdir where status_id = 0 and type_id = 3 and country_id = 'US' union all select ' ' from dual) order by internet_display_name");
            brn_lst.DataValueField = "internet_display_name";
            brn_lst.DataTextField = "internet_display_name";
            brn_lst.DataBind();

            svc_mgr_lst.DataSource = theConnection.GetDataTable("select nm from (select last_name || ', ' || first_name nm from mv_ws_emps where status != 'T' and title_code in ('BRSVC160', 'BRSVC170') union all select ' ' from dual) order by nm");
            svc_mgr_lst.DataValueField = "nm";
            svc_mgr_lst.DataTextField = "nm";
            svc_mgr_lst.DataBind();

            reg_svc_mgr_lst.DataSource = theConnection.GetDataTable("select nm from (select last_name || ', ' || first_name nm from   mv_ws_emps where  title_code = 'BRSVC175' and   status != 'T' union all select ' ' from dual) order by nm");
            reg_svc_mgr_lst.DataValueField = "nm";
            reg_svc_mgr_lst.DataTextField = "nm";
            reg_svc_mgr_lst.DataBind();
        }

        protected void uploadExcel(object sender, EventArgs e)
        {
            string aSourceFileName;
            string aConnStr;
            string aSheetName;
            string theResult = "";

            OleDbConnection anExcelConn = null;
            OleDbDataAdapter anAdapter = null;
            OleDbCommand aCommand = null;
            OracleDataReader aDataReader = null;
            WSDBConnection mConn = null;
            OracleDataReader a_dr = null;
            DataTable aExcelDT2 = null;
            DataTable aSchemaTable = null;

            int aSchemaTableCT;

            if (Request.Form["brn_lst"].ToString() != " " && Request.Form["insp_dt"].ToString() != " " && Request.Form["svc_mgr_lst"].ToString() != " " && Request.Form["reg_svc_mgr_lst"].ToString() != " ")
            {

                try
                {
                    mConn = new WSDBConnection();
                    if (mConn.Open("FDD", 0))
                    {
                        aSourceFileName = String.Format("{0}\\Safety_Audit\\temp.xls", ConfigurationManager.AppSettings["gAppPath"]);
                        fu_excel.SaveAs(aSourceFileName);

                        aConnStr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties=Excel 8.0;", aSourceFileName);
                        anExcelConn = new OleDbConnection(aConnStr);
                        anAdapter = new OleDbDataAdapter();

                        anExcelConn.Open();

                        aSchemaTable = anExcelConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        aSchemaTableCT = 0;

                        //Load Questions into a data table
                        while (aExcelDT2 == null)
                        {

                            aSheetName = aSchemaTable.Rows[aSchemaTableCT]["TABLE_NAME"].ToString();
                            aCommand = new OleDbCommand(String.Format("SELECT * FROM [{0}]", aSheetName), anExcelConn);

                            if (aSheetName == "Questions$")
                            {
                                aExcelDT2 = new DataTable();
                                anAdapter.SelectCommand = aCommand;
                                anAdapter.Fill(aExcelDT2);
                            }

                            aSchemaTableCT++;
                        }

                        // Instantiate external Feith form
                        FWS.FormsIQExternalData theIQForm = new FWS.FormsIQExternalData();

                        // Instantiate external Feith service
                        FWS.FormsIQWebServiceService theService = new FWS.FormsIQWebServiceService();

                        // Instantiate system level data set; populate the info attribute
                        FWS.FormInfo theInfo = new FWS.FormInfo();
                        theInfo.formSetId = Convert.ToInt32(mForm);
                        theInfo.dataSource = mDataSource.ToString();
                        theInfo.formId = Convert.ToInt32(mFormID);
                        theIQForm.formInfo = theInfo;

                        // Set the array for the fields for the form
                        FWS.FormFieldEx[] theFields = new FWS.FormFieldEx[7];

                        // Name each array element
                        FWS.SingleField Inspection_Date = new FWS.SingleField();
                        FWS.SingleField Service_Mgr = new FWS.SingleField();
                        FWS.SingleField Branch_Name = new FWS.SingleField();
                        FWS.SingleField Regional_Service_Manager = new FWS.SingleField();
                        FWS.SingleField Status = new FWS.SingleField();
                        FWS.SingleField Submitted_By = new FWS.SingleField();

                        // Create the field IDs for the array 
                        FWS.FieldIdentifier fi_Inspection_Date = new FWS.FieldIdentifier();
                        FWS.FieldIdentifier fi_Service_Mgr = new FWS.FieldIdentifier();
                        FWS.FieldIdentifier fi_Branch_Name = new FWS.FieldIdentifier();
                        FWS.FieldIdentifier fi_Regional_Service_Manager = new FWS.FieldIdentifier();
                        FWS.FieldIdentifier fi_Status = new FWS.FieldIdentifier();
                        FWS.FieldIdentifier fi_Submitted_By = new FWS.FieldIdentifier();

                        // Set the reference name for the ID fields as reference to the FormsIQ form 
                        // Match to the Name in the field definition in Feith
                        fi_Inspection_Date.referenceName = "Inspection Date";
                        fi_Service_Mgr.referenceName = "Service Mgr";
                        fi_Branch_Name.referenceName = "BRANCH_NAME";
                        fi_Regional_Service_Manager.referenceName = "Regional Service Manager";
                        fi_Status.referenceName = "Status";
                        fi_Submitted_By.referenceName = "SUBMITTED_BY";

                        // Set the values and IDs for the whole set
                        Inspection_Date.value = Request.Form["insp_dt"].ToString();
                        Service_Mgr.value = Request.Form["svc_mgr_lst"].ToString();
                        Branch_Name.value = Request.Form["brn_lst"].ToString();
                        Regional_Service_Manager.value = Request.Form["reg_svc_mgr_lst"].ToString();
                        Status.value = "IN_PROGRESS";
                        Submitted_By.value = WSSecurityPackage.NTLookup.GetNTUserName(this);

                        Inspection_Date.identifier = fi_Inspection_Date;
                        Service_Mgr.identifier = fi_Service_Mgr;
                        Branch_Name.identifier = fi_Branch_Name;
                        Regional_Service_Manager.identifier = fi_Regional_Service_Manager;
                        Status.identifier = fi_Status;
                        Submitted_By.identifier = fi_Submitted_By;

                        // Populate the field array
                        theFields[0] = Inspection_Date;
                        theFields[1] = Service_Mgr;
                        theFields[2] = Branch_Name;
                        theFields[3] = Regional_Service_Manager;
                        theFields[4] = Status;
                        theFields[5] = Submitted_By;

                        // Create question/answer arrays
                        string[] questionArray = (string[])mQuestions.ToArray(typeof(string));

                        // Set up dynamic list with 5 columns 
                        FWS.ListField listField = new FWS.ListField();

                        // Create n rows based on number of questions/answer pairs
                        FWS.Row[] dynamicRowArray = new FWS.Row[aExcelDT2.Rows.Count - 2];

                        for (int i = 0; i < (aExcelDT2.Rows.Count - 2); i++)
                        {
                            // Create a row
                            dynamicRowArray[i] = new FWS.Row();

                            // Create 2 cells for each row
                            FWS.SingleField[] dynamicCols = new FWS.SingleField[7];

                            // Create a field object for each cell
                            dynamicCols[0] = new FWS.SingleField();
                            dynamicCols[0].identifier = new FWS.FieldIdentifier();
                            dynamicCols[1] = new FWS.SingleField();
                            dynamicCols[1].identifier = new FWS.FieldIdentifier();
                            dynamicCols[2] = new FWS.SingleField();
                            dynamicCols[2].identifier = new FWS.FieldIdentifier();
                            dynamicCols[3] = new FWS.SingleField();
                            dynamicCols[3].identifier = new FWS.FieldIdentifier();
                            dynamicCols[4] = new FWS.SingleField();
                            dynamicCols[4].identifier = new FWS.FieldIdentifier();
                            dynamicCols[5] = new FWS.SingleField();
                            dynamicCols[5].identifier = new FWS.FieldIdentifier();
                            dynamicCols[6] = new FWS.SingleField();
                            dynamicCols[6].identifier = new FWS.FieldIdentifier();

                            a_dr = mConn.GetReader(string.Format(@"select description from fdd.safety_branch_audit_categories where status = 'Active' and section = '{0}' and   question_number = '{1}' and   status = 'Active'",
                                                                    aExcelDT2.Rows[i][0].ToString(),
                                                                    aExcelDT2.Rows[i][1].ToString()));

                            // Populate cells
                            dynamicCols[0].identifier.referenceName = "Section";
                            dynamicCols[0].value = aExcelDT2.Rows[i][0].ToString();
                            dynamicCols[1].identifier.referenceName = "Question Number";
                            dynamicCols[1].value = aExcelDT2.Rows[i][1].ToString();
                            dynamicCols[2].identifier.referenceName = "Description";

                            if (a_dr.Read())
                            {
                                dynamicCols[2].value = a_dr["description"].ToString();
                            }
                            else
                            {
                                throw new Exception("Invalid section/question number combination");
                            }

                            dynamicCols[3].identifier.referenceName = "Answer";
                            dynamicCols[3].value = aExcelDT2.Rows[i][4].ToString();
                            dynamicCols[4].identifier.referenceName = "Comments";

                            if (aExcelDT2.Rows[i][5].ToString().Trim() == "")
                            {
                                dynamicCols[4].value = null;
                            }
                            else
                            {
                                dynamicCols[4].value = aExcelDT2.Rows[i][5].ToString();
                            }

                            dynamicCols[5].identifier.referenceName = "Attachment";
                            dynamicCols[5].value = null;
                            dynamicCols[6].identifier.referenceName = "Question ID";
                            dynamicCols[6].value = null;
                            dynamicRowArray[i].columns = dynamicCols;

                        }

                        listField.rows = dynamicRowArray;

                        listField.identifier = new FWS.FieldIdentifier();
                        listField.identifier.referenceName = "Question";

                        theFields[6] = listField;

                        // Put the array in the fieldlist of the form
                        theIQForm.fieldList = theFields;

                        // Submit the form
                        theService.Timeout = 300000000;
                        theResult = theService.submitForm(theIQForm);
                        div1.InnerHtml = "Load Successful!!!!!";
                    }
                }
                catch (Exception ex)
                {
                    logerror("uploadExcel", "", ex.ToString());
                    div1.InnerHtml = "Load Failed!!  Please Contact Tech Support for help!";
                }
                finally
                {
                    if (anExcelConn != null)
                    {
                        anExcelConn.Close();
                        anExcelConn.Dispose();
                    }
                    if (anAdapter != null)
                    {
                        anAdapter.Dispose();
                    }
                    if (aCommand != null)
                    {
                        aCommand.Dispose();
                    }
                    if (aDataReader != null)
                    {
                        aDataReader.Close();
                        aDataReader.Dispose();
                    }
                    if (aSchemaTable != null)
                    {
                        aSchemaTable.Dispose();
                    }
                    if (aExcelDT2 != null)
                    {
                        aExcelDT2.Dispose();
                    }

                }
            }
            else
            {
                div1.InnerHtml = "SUBMISSION FAILED!  You have not filled out one of the required fields.  Please make sure that all fields are filled out.";
            }
        }

        protected void addQuesSec_click(object sender, EventArgs e)
        {
            WSDBConnection mConn = null;
            OracleDataReader a_dr = null;

            string a_mx = "";
            string a_ab = "";

            try
            {
                mConn = new WSDBConnection();
                if (mConn.Open("FDD", 0))
                {
                    a_dr = mConn.GetReader(string.Format(@"select (max(question_number) + 1) mx from fdd.safety_branch_audit_categories where section = '{0}'"
                                                           , Request.Form["add_list"]));
                    if (a_dr.Read())
                    {
                        if (a_dr["mx"].ToString() == "")
                        {
                            a_mx = "1";
                        }
                        else
                        {
                            a_mx = a_dr["mx"].ToString();
                        }


                        a_dr.Close();
                    }

                    a_dr = mConn.GetReader(@"select (max(question_id) + 1) mx from safety_branch_audit_categories");

                    if (a_dr.Read())
                    {
                        a_ab = a_dr["mx"].ToString();
                        a_dr.Close();
                    }
                    if (Request.Form["add_ques_txt"] != "" && Request.Form["add_list"] != "")
                    {
                        string aCommand = string.Format(@"insert into fdd.safety_branch_audit_categories (section, description, weight, question_number, status, begin_date, end_date, question_id, update_date, update_user)
                                                          values ('{0}', '{1}', '{2}', '{3}', 'Inactive', trunc(sysdate), trunc(sysdate), '{4}', trunc(sysdate), '{5}' )"
                                                          , Request.Form["add_list"]
                                                          , WSString.FilterSQL(Request.Form["add_ques_txt"])
                                                          , 0
                                                          , a_mx
                                                          , a_ab
                                                          , WSSecurityPackage.NTLookup.GetNTUserName(this));
                        mConn.ExecuteCommand(aCommand);
                    }

                    a_dr = mConn.GetReader("select (max(head_id) + 1) mx from fdd.safety_audit_headings");

                    if (a_dr.Read())
                    {
                        a_mx = a_dr["mx"].ToString();
                        a_dr.Close();
                    }

                    if (Request.Form["add_sec_txt"] != "")
                    {
                        string aCommand = string.Format(@"insert into fdd.safety_audit_headings (head_id, description)
                                                          values ('{0}', '{1}')"
                                                          , a_mx
                                                          , WSString.FilterSQL(Request.Form["add_sec_txt"])
                                                          );
                        mConn.ExecuteCommand(aCommand);
                    }

                }

                div1.InnerHtml = "Update Complete";
                createQuestlist(mConn);
                loadsafetyquestions(mConn);


            }
            catch (Exception ex)
            {
                logerror("Page_Load", "", ex.ToString());
                div1.InnerHtml = "Update Failed!!  Please contact Tech Support for help";
            }
            finally
            {
                if (a_dr != null) { a_dr.Close(); a_dr.Dispose(); }

                if (mConn != null) { mConn.Close(); }
            }
        }

        protected void saveBranchmain(Object sender, EventArgs e)
        {
            WSDBConnection mConn = null;
            OracleDataReader a_dr = null;
            OracleDataReader a_dr2 = null;

            try
            {
                mConn = new WSDBConnection();
                if (mConn.Open("FDD", 0))
                {
                    a_dr = mConn.GetReader("select cost_center from branch_header");
                    a_dr2 = mConn.GetReader("select count(1) cnt from system_listing");

                    a_dr2.Read();

                    while (a_dr.Read())
                    {
                        for (int a = 0; a < Convert.ToInt32(a_dr2["cnt"]); a++)
                        {
                            if (Request.Form[a_dr["cost_center"].ToString() + a.ToString() + "ins"] != null)
                            {
                                string aCommand = string.Format(@"insert into branch_listing(cost_center, system_id) values('{0}', '{1}')"
                                                                , a_dr["cost_center"].ToString()
                                                                , a);

                                mConn.ExecuteCommand(aCommand);
                            }

                            else if (Request.Form[a_dr["cost_center"].ToString() + a.ToString() + "del"] == null)
                            {
                                string aCommand = string.Format(@"delete from branch_listing where cost_center = '{0}' and system_id = '{1}'"
                                                                , a_dr["cost_center"].ToString()
                                                                , a);


                                mConn.ExecuteCommand(aCommand);
                            }

                        }
                    }

                    loadbranchsystemattrb(mConn);

                }
            }
            catch (Exception ex)
            {
                logerror("Page_Load", "", ex.ToString());
                div1.InnerHtml = "Save Failed!!  Please contact Tech Support.";
            }

            finally
            {
                if (a_dr != null) { a_dr.Close(); a_dr.Dispose(); }

                if (mConn != null) { mConn.Close(); }
            }

        }

        protected void logerror(string theFunction, string theMessage, string theError)
        {
            WSDBConnection aConn = new WSDBConnection();
            aConn.Open("JDE_WEB");
            WSProcedure aErrProc = aConn.InitProcedure("pkg_util.P_MSG_A");
            aErrProc.AddParam("pram_app", OracleDbType.VarChar, ParameterDirection.Input, "Safety Audit");
            aErrProc.AddParam("pram_loc", OracleDbType.VarChar, ParameterDirection.Input, "index.aspx.cs");
            aErrProc.AddParam("pram_line", OracleDbType.VarChar, ParameterDirection.Input, theFunction);
            aErrProc.AddParam("pram_in", OracleDbType.VarChar, ParameterDirection.Input, theMessage);
            aErrProc.AddParam("pram_out", OracleDbType.VarChar, ParameterDirection.Input, WSSecurityPackage.NTLookup.GetNTUserName(this));
            aErrProc.AddParam("pram_serr", OracleDbType.Integer, ParameterDirection.Input, 0);
            aErrProc.AddParam("pram_err", OracleDbType.VarChar, ParameterDirection.Input, WSString.FilterSQL(theError));
            aErrProc.AddParam("pram_type", OracleDbType.VarChar, ParameterDirection.Input, "Error");
            aErrProc.AddParam("pram_sec", OracleDbType.Integer, ParameterDirection.Input, 0);
            aErrProc.ExecuteNonQuery();

            aConn.Close();
        }


        protected void HandleError(object sender, AsyncPostBackErrorEventArgs e)
        {

            logerror("HandleError", "", e.Exception.ToString());
            div1.InnerHtml = "Error: Please contact tech support.";
        }
    }
}
