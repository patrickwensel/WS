using Plumtree.Remote.Portlet;
using Devart.Data.Oracle;
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using WS20.Framework;


namespace PIP
{
    public class common
    {
        protected WSDBConnection mConn; //global connection used for all db access
        //protected string mDomain;
        protected string mUser; //current user used to log errors
        protected string mUserName;
        protected bool mGoodConn; //check to see if connection to db was made
        protected HttpApplicationState mApp;
        protected string mLang;
        protected bool mAdmin;
        protected bool mLogging;
        protected Hashtable mLangHT;

        /// <summary>
        /// Stores global vars
        /// </summary>
        /// <param name="theApp">Applicaiton of calling program, used to store app vars</param>
        /// <param name="theLang">Language used</param>
        /// <param name="theUser">user</param>
        /// <param name="theUserEmail">users email</param>
        public common(HttpApplicationState theApp, string theLang, IPortletContext thePortletContext)
        { 
            //mUserEmail = theUserEmail;
            mApp = theApp;
            mLogging = ConfigurationManager.AppSettings["LOG"] == "true";
            
            //need to move this to common so all can use
            if (thePortletContext != null)
            {
                #region get user info
                //bool aUserError = false;
                try
                {
                    //IPortletContext aPortletContext = PortletContextFactory.CreatePortletContext(Request, Response);
                    IPortletRequest aPortletRequest = thePortletContext.GetRequest();

                    //get the User Preferences for the search string and folder ID's to search.  These are set when the user
                    mUser = thePortletContext.GetUser().GetUserID().ToString();
                    mUserName = aPortletRequest.GetSettingValue(SettingType.UserInfo, "FullName");

                }
                catch (Exception ex)
                {
                    //aUserError = true;
                    mUser = ConfigurationManager.AppSettings["adminPortalID"];
                    logError("index.aspx.cs", "Page_Load", "username:" + mUser, ex.ToString(), "ERROR");
                }
                //mUser = "macorbin";
                //mUserEmail = "macorbin@willscot.com";
                //mDomain = "ws";
                #endregion
            }
            else
            { 
                //should only be global getting here
                mUser = ConfigurationManager.AppSettings["adminPortalID"];
            }

            //need to do after the user is set
            //sets mLang and mAdmin
            setLang(theLang);
            mLangHT = (Hashtable)theApp[mLang + "LangHash"];

            if (mLogging)
            {
                logError("common.cs", "common", "mUser: " + mUser + "   mUserName:" + mUserName + "   mLang:" + mLang, "", "Log");
            }
        } 

        /// <summary>
        /// returns if connection was made
        /// </summary>
        /// <returns>true if connection is good</returns>
        public bool goodConn()
        {
            return mGoodConn;
        }

        /// <summary>
        /// creates a db connection
        /// </summary>
        private void createConn()
        {
            mConn = new WSDBConnection();
            mGoodConn = mConn.Open("WEB", 0);
        }

        /// <summary>
        /// closes connection
        /// </summary> 
        public void cleanUp()
        {
            if (mConn != null) { mConn.Close(); }
            mGoodConn = false;
            mConn = null;
        }

        /// <summary>
        /// loads app vars
        /// </summary>
        public void loadApp()
        {
            mApp["updateDT"] = DateTime.Now.ToString();
            //all codes
            mApp["LangCodes"] = GetDataTable(@" SELECT lang
                                                      ,name
                                                      ,currency
                                                      ,date_format
                                                  FROM web_lang_codes").DefaultView;
            //all text for each code
            foreach (DataRowView aDR in ((DataView)(mApp["LangCodes"])))
            {
                //all text
                mApp[aDR["lang"].ToString() + "LangHash"] =
                                    GetHashTable(@"SELECT name, text 
                                                    FROM WEB_LANG 
                                                   WHERE application_id = " + ConfigurationManager.AppSettings["APP_ID"] +
                                                   " AND lang = '" + aDR["lang"].ToString() + @"'");
                //lang select
                mApp[aDR["lang"].ToString() + "LangSel"] =
                             GetDataTable(@"SELECT member_code
                                                  ,member_desc
                                              FROM pip_lookup
                                             WHERE category_id = 3
                                               AND lang = '" + aDR["lang"].ToString() + @"'
                                                ORDER BY sort_order").DefaultView;
                //product select
                mApp[aDR["lang"].ToString() + "ProductSel"] =
                             GetDataTable(@"select null id,null member_desc,0 as sort_order from dual 
                                            union all 
                                            SELECT id
                                                  ,member_desc
                                                  ,sort_order
                                              FROM pip_lookup
                                             WHERE category_id = 2
                                               AND lang = '" + aDR["lang"].ToString() + @"'
                                                ORDER BY sort_order").DefaultView;
                //status select
                mApp[aDR["lang"].ToString() + "StatusSel"] =
                             GetDataTable(@"SELECT id
                                                  ,member_desc
                                              FROM pip_lookup
                                             WHERE category_id = 1
                                               AND lang = '" + aDR["lang"].ToString() + @"'
                                                ORDER BY sort_order");
                //savings select
                mApp[aDR["lang"].ToString() + "SavingsSel"] =
                             GetDataTable(@"SELECT id
                                                  ,member_desc
                                              FROM pip_lookup
                                             WHERE category_id = 4
                                               AND lang = '" + aDR["lang"].ToString() + @"'
                                                ORDER BY sort_order").DefaultView;
                //action status select
                mApp[aDR["lang"].ToString() + "ActionStatusSel"] =
                             GetDataTable(@"select null id,null member_desc,0 as sort_order from dual 
                                            UNION ALL 
                                            SELECT id
                                                  ,member_desc
                                                  ,sort_order 
                                              FROM pip_lookup
                                             WHERE category_id = 5
                                               AND lang = '" + aDR["lang"].ToString() + @"'
                                                ORDER BY sort_order").DefaultView;
                //product select
                mApp[aDR["lang"].ToString() + "processSel"] =
                             GetDataTable(@"select null id,null member_desc,0 as sort_order from dual 
                                            union all 
                                            SELECT id
                                                  ,member_desc
                                                  ,sort_order
                                              FROM pip_lookup
                                             WHERE category_id = 6
                                               AND lang = '" + aDR["lang"].ToString() + @"'
                                                ORDER BY sort_order").DefaultView;
                //currency select
                mApp[aDR["lang"].ToString() + "currencySel"] =
                             GetDataTable(@"SELECT id
                                                  ,member_desc
                                              FROM pip_lookup
                                             WHERE category_id = 7
                                               AND lang = '" + aDR["lang"].ToString() + @"'
                                                ORDER BY sort_order").DefaultView;
            }

            //people select
            mApp["PeopleSel"] =
                         GetDataTable(@"SELECT decode(COUNT(1) over(PARTITION BY
                                                           lastname.value || nvl2(lastname.value
                                                                                 ,', '
                                                                                 ,'') || firstname.value)
                                                     ,1
                                                     ,lastname.value || nvl2(lastname.value
                                                                            ,', '
                                                                            ,'') || firstname.value
                                                     ,lastname.value || nvl2(lastname.value
                                                                            ,', '
                                                                            ,'') || firstname.value || ' (' ||
                                                      u.loginname || ')') AS full_name
                                              ,'''' || u.objectid || '''' AS multi_full_name
                                              ,u.objectid
                                          FROM ali.ptusers       u
                                              ,ali.ptpropertydata firstname
                                              ,ali.ptpropertydata lastname
                                         WHERE u.objectid = firstname.objectid
                                           AND firstname.propertyid = " + ConfigurationManager.AppSettings["firstpropertyid"] + @"
                                           AND u.objectid = lastname.objectid
                                           AND lastname.propertyid = " + ConfigurationManager.AppSettings["lastpropertyid"] + @"
                                           AND NAME NOT LIKE ('%GeneralVMBox')
                                           AND NAME NOT LIKE ('%SalesVM')
                                           AND NAME NOT LIKE ('%ServiceVM%')
                                           AND NAME NOT LIKE ('%Conference%')
                                           AND NAME NOT LIKE ('IWAM%')
                                           AND NAME NOT LIKE ('IUSR%')
                                           AND NAME NOT LIKE ('IWAM%')
                                           AND lower(NAME) NOT LIKE ('%support%')
                                           AND lower(NAME) NOT LIKE ('%svc%')
                                           AND NAME NOT LIKE ('System%')
                                           AND NAME NOT LIKE ('Test%')
                                           AND NAME NOT LIKE ('%Training%')
                                           AND NAME NOT LIKE ('Template%')
                                           and name not like ('%Mailbox%')");

            //branch select 
            mApp["BranchSel"] =
 //                     GetDataTable(@"select null itemid, null as entity_desc from dual").DefaultView;
                         GetDataTable(@"SELECT NULL itemid
                                              ,NULL AS entity_desc
                                          FROM dual
                                        UNION ALL
                                        SELECT bottomitem
                                              ,bottomdesc
                                          FROM mart.v_hfm_pip_entity@rpt.world e
                                         ORDER BY entity_desc NULLS FIRST
                                         ").DefaultView;


        }

        public string setLang(string theLang)
        {
            if (mLogging)
            {
                logError("common.cs", "setLang1", theLang, "", "LOG");
            }


            OracleDataReader aDR = GetReader("SELECT last_lang, pip_admin FROM pip_user_info WHERE user_id = '" + mUser + "'");
            if (aDR != null && aDR.Read())
            {
                mLang = (theLang == null || theLang == "") ? aDR["last_lang"].ToString() : theLang;
                mAdmin = aDR["pip_admin"].ToString() == "Y" ? true : false;
                aDR.Close();
            }
            else
            {
                mLang = ConfigurationManager.AppSettings["defaultLang"];
                mAdmin = false;
            }


            ArrayList aAL = new ArrayList(1);
            aAL.Add(@"MERGE INTO pip_user_info i
                            USING (SELECT '" + mUser + @"' user_id FROM dual) d
                            ON (i.user_id = d.user_id)
                            WHEN MATCHED THEN
                              UPDATE
                                 SET last_lang    = '" + mLang + @"'
                                    ,last_login = SYSDATE
                            WHEN NOT MATCHED THEN
                              INSERT
                                (user_id
                                ,last_lang
                                ,last_login)
                              VALUES
                                (d.user_id
                                ,'" + mLang + @"'
                                ,SYSDATE)");
            ExecuteCommand(aAL);


            if (mLogging)
            {
                logError("common.cs", "setLang2", theLang, "", "LOG");
            }

            return mLang;
        }
         
        public string getLang() { return mLang; } 
        public string getUser() { return mUser; }
        public string getUserName() { return mUserName; }
        public bool isAdmin() { return mAdmin; }
        public bool getLogging() { return mLogging; }
        //should add a check to find missing text not sure if it would slow it down too much
        public string getText(string theKey)
        {
            if (theKey == null || !mLangHT.Contains(theKey))
            {
                logError("common.cs", "getText", theKey, "Key Missing", "ERROR");
                return "";
            }
            else
            {
                return mLangHT[theKey].ToString();
            }

        }

        public float getRate(string theFrom, string theTo, string theProjectID)
        {
            float aRate = 1;

            OracleDataReader aDR = GetReader(
                                        @"SELECT c1.rate * c2.rate as rate
                                          FROM pip_currency c1
                                              ,pip_currency c2
                                         WHERE nvl((SELECT create_date FROM pip_header WHERE project_id = '" + theProjectID + @"')
                                                  ,SYSDATE) BETWEEN c1.start_date AND
                                               nvl(c1.end_date
                                                  ,SYSDATE)
                                           AND c1.from_currency_id = " + theFrom + @"
                                           AND c1.to_currency_id = " + ConfigurationManager.AppSettings["defaultCurrency"] + @"
                                           AND nvl((SELECT create_date FROM pip_header WHERE project_id = '" + theProjectID + @"')
                                                  ,SYSDATE) BETWEEN c2.start_date AND
                                               nvl(c2.end_date
                                                  ,SYSDATE)
                                           AND c2.from_currency_id = " + ConfigurationManager.AppSettings["defaultCurrency"] + @"
                                           AND c2.to_currency_id = " + theTo + @"");

            if (aDR != null && aDR.Read())
            {
                aRate = Convert.ToSingle(aDR["rate"]);
            }

            return aRate;
        }

        public string getProjectsSel(string theSelProject)
        {
            string aRetVal = "";
            OracleDataReader aDR;
            string aPrevGroup = "";
            
            aDR = GetReader(@"SELECT h.project_id
                                      ,h.project_id || ' - ' ||decode(curr.lang
                                             ,NULL
                                             ,def.project_title
                                             ,curr.project_title) AS project_title
                                      ,h.project_owner
                                      ,'N' AS template
                                      ,decode(project_owner
                                             ,'" + getUser() + @"'
                                             ,'selOptMyProjects'
                                             ,'selOptOtherProjects') AS owner_sort
                                  FROM pip_header h
                                      ,pip_detail def
                                      ,pip_detail curr
                                 WHERE h.project_id = def.project_id
                                   AND h.defualt_lang = def.lang
                                   AND h.deleted = 'N'
                                   AND (h.project_owner = '" + getUser() + @"' OR
                                       h.create_user_id = '" + getUser() + @"')
                                   AND h.project_id = curr.project_id(+)
                                   AND '" + getLang() + @"' = curr.lang(+)
                                   AND (h.template is null or h.template <> 'Y')
                                UNION ALL
                                SELECT h.project_id
                                      ,h.project_id || ' - ' ||decode(curr.lang
                                             ,NULL
                                             ,def.project_title
                                             ,curr.project_title) AS project_title
                                      ,h.project_owner
                                      ,'Y'
                                      ,'selOptTemplate'
                                  FROM pip_header h
                                      ,pip_detail def
                                      ,pip_detail curr
                                 WHERE h.project_id = def.project_id
                                   AND h.defualt_lang = def.lang
                                   AND h.deleted = 'N'
                                   AND h.project_id = curr.project_id(+)
                                   AND '" + getLang() + @"' = curr.lang(+)
                                   AND h.template = 'Y'
                              ORDER BY owner_sort
                                      ,project_id"); 
            
            aRetVal = "<select id='selSearchProjectTitle' title='" + getText("tipProject") + "'><option value=''>" + getText("EnterNewProject") + "</option>";
            while (aDR.Read())
            {
                if (aPrevGroup != aDR["owner_sort"].ToString()){
                    if (aPrevGroup != "")
                    {
                        aRetVal += "</optgroup>";
                    }
                    aPrevGroup = aDR["owner_sort"].ToString();
                    aRetVal += "<optgroup label='" + getText(aDR["owner_sort"].ToString()) + "'>";
                }

                if (theSelProject == aDR["project_id"].ToString())
                {
                    aRetVal += "<option selected value='" + aDR["project_id"].ToString() + "'>" +
                                                         aDR["project_title"].ToString() + "</option>";
                }
                else
                {
                    aRetVal += "<option value='" + aDR["project_id"].ToString() + "'>" +
                                                         aDR["project_title"].ToString() + "</option>";
                }
            }
            aRetVal += "</optgroup></select>";
            aDR.Close();
            aDR.Dispose();

            return aRetVal;
        }
        

        /// <summary>
        /// Gets a DataTable from the current connection
        /// </summary>
        /// <param name="theQuery">querys to be run</param>
        /// <returns>rows updated</returns>
        public bool ExecuteCommand(ArrayList theQuery)
        {
            //send array and do as a transaction
            if (mConn == null)
            {
                createConn();
            }

            bool retVal = true;
            int rows = 0;
            if (mGoodConn)
            {
                try
                {
                    mConn.BeginTransaction();
                    foreach( string aQuery in theQuery )
                    {
                        rows = mConn.ExecuteCommand(aQuery);
                        if (mLogging)
                        {
                            logError("common.cs", "ExecuteCommand", aQuery, "Rows Update:"+rows.ToString() + " Querys in Trans:" + theQuery.Count.ToString(), "LOG");
                        }
                    }
                    mConn.CommitTransaction();
                }
                catch (Exception ex)
                {
                    retVal = false;
                    mConn.RollbackTransaction();
                    foreach (string aQuery in theQuery)
                    {
                        logError("common.cs", "ExecuteCommand", aQuery, ex.ToString(), "ERROR");
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets a DataTable from the current connection
        /// </summary>
        /// <param name="theQuery">query to be run</param>
        /// <returns>DataTable with results of query</returns>
        public DataTable GetDataTable(string theQuery)
        {
            if (mConn == null)
            {
                createConn();
            }

            DataTable aDT = null;
            if (mGoodConn)
            {
                try
                {
                    aDT = mConn.GetDataTable(theQuery);
                    if (mLogging)
                    {
                        logError("common.cs", "GetDataTable", theQuery, "", "LOG");
                    }
                }
                catch (Exception ex)
                {
                    logError("common.cs", "GetDataTable", theQuery, ex.ToString(), "ERROR");
                    aDT = null;
                }
            }
            return aDT;
        }

        /// <summary>
        /// Gets a HashTable from the current connection
        /// </summary>
        /// <param name="theQuery">query to be run</param>
        /// <returns>DataTable with results of query</returns>
        public Hashtable GetHashTable(string theQuery)
        {
            Hashtable aHT = null;
            OracleDataReader aDR = GetReader(theQuery);
            if (aDR != null)
            {
                aHT = new Hashtable();
                while (aDR.Read())
                {
                    aHT.Add(aDR["name"], aDR["text"]);
                }
            }
            return aHT;
        }

        /// <summary>
        /// Gets a DataReader from the current connection
        /// </summary>
        /// <param name="theQuery">query to be run</param>
        /// <returns>DataReader with results of querty</returns> 
        public OracleDataReader GetReader(string theQuery)
        {
            if (mConn == null)
            {
                createConn();
            }

            OracleDataReader aDR = null;
            if (mGoodConn)
            {
                try
                {
                    aDR = mConn.GetReader(theQuery);
                    if (mLogging)
                    {
                        logError("common.cs", "GetReader", theQuery, "", "LOG");
                    }
                }
                catch (Exception ex)
                {
                    logError("common.cs", "GetReader", theQuery, ex.ToString(), "ERROR");
                    aDR = null;
                }
            }
            return aDR;
        }

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="thePage">Page error is on</param>
        /// <param name="theFunction">Function error is in</param>
        /// <param name="theMessage">Message about the error</param>
        /// <param name="theError">Error Message</param>
        /// <param name="theType">Type of error</param>
        public void execProc(string theTable, string theColumn, string theValue, string theKeyColumn, string theKeyValue)
        {
            if (mConn == null)
            {
                createConn();
            }
            //maybe send email if no connection
            if (mGoodConn) 
            {
                WSProcedure aProc = mConn.InitProcedure("p_setLargeClobValue2Key");
                aProc.AddParam("v_tableName", OracleDbType.VarChar, ParameterDirection.Input, theTable);
                aProc.AddParam("v_clobColumnName", OracleDbType.VarChar, ParameterDirection.Input, theColumn);
                aProc.AddParam("b_clobValue", OracleDbType.Clob, ParameterDirection.Input, theValue);
                aProc.AddParam("v_keyField1", OracleDbType.VarChar, ParameterDirection.Input, theKeyColumn);
                aProc.AddParam("v_keyValue1", OracleDbType.Integer, ParameterDirection.Input, theKeyValue);
                aProc.AddParam("v_keyField2", OracleDbType.VarChar, ParameterDirection.Input, "lang");
                aProc.AddParam("v_keyValue2", OracleDbType.VarChar, ParameterDirection.Input, mLang);
                aProc.ExecuteNonQuery();
                if (mLogging)
                {
                    logError("common.cs", "execProc", "p_setLargeClobValue", "theTable:" + theTable + " theColumn:" + theColumn + " theValue:" + theValue + " theKeyColumn:" + theKeyColumn + " theKeyValue:" + theKeyValue, "LOG");
                }
            }
        }
         
        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="thePage">Page error is on</param>
        /// <param name="theFunction">Function error is in</param>
        /// <param name="theMessage">Message about the error</param>
        /// <param name="theError">Error Message</param>
        /// <param name="theType">Type of error</param>
        public void execProcBlob(string theTable, string theColumn, byte[] theValue, string theKeyColumn1, string theKeyValue1, string theKeyColumn2, string theKeyValue2)
        {
            if (mConn == null)
            {
                createConn();
            } 
            //maybe send email if no connection
            if (mGoodConn)
            {
                WSProcedure aProc = mConn.InitProcedure("p_setLargeBlobValue3Key");
                aProc.AddParam("v_tableName", OracleDbType.VarChar, ParameterDirection.Input, theTable);
                aProc.AddParam("v_clobColumnName", OracleDbType.VarChar, ParameterDirection.Input, theColumn);
                aProc.AddParam("b_clobValue", OracleDbType.Blob, ParameterDirection.Input, theValue);
                aProc.AddParam("v_keyField1", OracleDbType.VarChar, ParameterDirection.Input, theKeyColumn1);
                aProc.AddParam("v_keyValue1", OracleDbType.Integer, ParameterDirection.Input, theKeyValue1);
                aProc.AddParam("v_keyField2", OracleDbType.VarChar, ParameterDirection.Input, theKeyColumn2);
                aProc.AddParam("v_keyValue2", OracleDbType.VarChar, ParameterDirection.Input, theKeyValue2);
                aProc.AddParam("v_keyField3", OracleDbType.VarChar, ParameterDirection.Input, "lang");
                aProc.AddParam("v_keyValue3", OracleDbType.VarChar, ParameterDirection.Input, mLang);
                aProc.ExecuteNonQuery();
                if (mLogging)
                {
                    logError("common.cs", "execProcBlob", "p_setLargeClobValue2Key", "theTable:" + theTable + " theColumn:" + theColumn + " theValue Len:" + theValue.Length + " theKeyColumn1:" + theKeyColumn1 + " theKeyValue1:" + theKeyValue1 + " theKeyColumn2:" + theKeyColumn2 + " theKeyValue2:" + theKeyValue2, "LOG");
                }
            }
        }


        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="thePage">Page error is on</param>
        /// <param name="theFunction">Function error is in</param>
        /// <param name="theMessage">Message about the error</param>
        /// <param name="theError">Error Message</param>
        /// <param name="theType">Type of error</param>
        public void logError(string thePage, string theFunction, string theMessage, string theError, string theType)
        { 
            if (mConn == null)
            {
                createConn();
            }
            //maybe send email if no connection
            if (mGoodConn)
            {
                string aError = WSString.FilterSQL(theError);
                string aSubError;
                bool aCK;

                //break error into 4000char blocks and log all if its too long
                do
                {
                    if (aError.Length > 2000)
                    {
                        aSubError = aError.Substring(0, 2000);
                        aError = aError.Substring(2000);
                        aCK = true;
                    }
                    else
                    {
                        aSubError = aError;
                        aCK = false;
                    }

                    WSProcedure aErrProc = mConn.InitProcedure("pkg_util.P_MSG_A");
                    aErrProc.AddParam("pram_app", OracleDbType.VarChar, ParameterDirection.Input, "PIP");
                    aErrProc.AddParam("pram_loc", OracleDbType.VarChar, ParameterDirection.Input, thePage != null && thePage.Length > 30 ? thePage.Substring(0, 30) : thePage);
                    aErrProc.AddParam("pram_line", OracleDbType.VarChar, ParameterDirection.Input, theFunction != null && theFunction.Length > 255 ? theFunction.Substring(0, 255) : theFunction);
                    aErrProc.AddParam("pram_in", OracleDbType.VarChar, ParameterDirection.Input, theMessage != null && theMessage.Length > 2000 ? theMessage.Substring(0, 2000) : theMessage);
                    aErrProc.AddParam("pram_out", OracleDbType.VarChar, ParameterDirection.Input, " User:" + mUser + "  Lang:" + mLang);
                    aErrProc.AddParam("pram_serr", OracleDbType.Integer, ParameterDirection.Input, 0);
                    aErrProc.AddParam("pram_err", OracleDbType.VarChar, ParameterDirection.Input, aSubError);
                    aErrProc.AddParam("pram_type", OracleDbType.VarChar, ParameterDirection.Input, theType);
                    aErrProc.AddParam("pram_sec", OracleDbType.Integer, ParameterDirection.Input, 0);
                    aErrProc.ExecuteNonQuery();
                     
                } while (aCK);
            }
        }
    }
}
