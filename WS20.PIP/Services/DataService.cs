using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Devart.Data.Oracle;
using PIP.Objects;
using Plumtree.Remote.Portlet;


namespace PIP.Services
{
    public class DataService : IDataService
    {
        public DataTable GetDataTable(OracleConnection oracleConnection, string query)
        {
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(query, oracleConnection);
            oracleConnection.Open();
            DataTable dataTable = new DataTable();
            try
            {
                oracleDataAdapter.Fill(dataTable);
            }
            finally
            {
                oracleConnection.Close();
            }
            return dataTable;
        }

        public List<People> GetPeopleList()
        {
            string query = @"SELECT decode(COUNT(1) over(PARTITION BY
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
                                           AND firstname.propertyid = " +
                    ConfigurationManager.AppSettings["firstpropertyid"] + @"
                                           AND u.objectid = lastname.objectid
                                           AND lastname.propertyid = " +
                    ConfigurationManager.AppSettings["lastpropertyid"] + @"
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
                                           and name not like ('%Mailbox%')";


            OracleConnection oracleConnection = new OracleConnection(ConfigurationManager.ConnectionStrings["Web"].ConnectionString);

            DataTable dataTable = GetDataTable(oracleConnection, query);

            List<People> peoples = dataTable.AsEnumerable().Select(row =>
                                                                  new People
                                                                      {
                                                                          FullName = row.Field<string>("full_name"),
                                                                          MultiFullName = row.Field<string>("multi_full_name"),
                                                                          ObjectID = row.Field<decimal>("objectid")
                                                                      }).ToList();

            return peoples;

        }

        public List<Branch> GetBranchList()
        {
            var query = @"SELECT NULL itemid
                                              ,NULL AS entity_desc
                                          FROM dual
                                        UNION ALL
                                        SELECT bottomitem
                                              ,bottomdesc
                                          FROM mart.v_hfm_pip_entity@rpt.world e
                                         ORDER BY entity_desc NULLS FIRST
                                         ";

            OracleConnection oracleConnection = new OracleConnection(ConfigurationManager.ConnectionStrings["Web"].ConnectionString);

            DataTable dataTable = GetDataTable(oracleConnection, query);

            List<Branch> branches = dataTable.AsEnumerable().Select(row =>
                                                      new Branch
                                                      {
                                                          ItemID = row.Field<decimal?>("itemid"),
                                                          EntityDescription = row.Field<string>("entity_desc")
                                                      }).ToList();

            return branches;

        }


        public PortalUser GetPortalUser()
        {
            PortalUser portalUser = new PortalUser();

            string environment = ConfigurationManager.AppSettings["environment"];
            if (environment != "Dev")
            {
                HttpRequest httpRequest = HttpContext.Current.Request;
                HttpResponse httpResponse = HttpContext.Current.Response;

                portalUser = FindPortalUser(PortletContextFactory.CreatePortletContext(httpRequest, httpResponse));

            }
            else
            {
                portalUser.User = ConfigurationManager.AppSettings["adminPortalID"];
                portalUser.Email = ConfigurationManager.AppSettings["adminEmail"];
                portalUser.UserName = ConfigurationManager.AppSettings["adminName"];
                portalUser.Region = ConfigurationManager.AppSettings["adminRegion"];
                
            }

            return portalUser;
        }


        private PortalUser FindPortalUser(IPortletContext thePortletContext)
        {
            PortalUser portalUser = new PortalUser();

            if (thePortletContext != null)
            {
                try
                {
                    IPortletRequest aPortletRequest = thePortletContext.GetRequest();

                    portalUser.User = thePortletContext.GetUser().GetUserID().ToString();
                    portalUser.Email = aPortletRequest.GetSettingValue(SettingType.UserInfo, "Email");
                    portalUser.UserName = aPortletRequest.GetSettingValue(SettingType.UserInfo, "FullName");
                    portalUser.Region = aPortletRequest.GetSettingValue(SettingType.UserInfo, "Region") != null ? aPortletRequest.GetSettingValue(SettingType.UserInfo, "Region").ToUpper() : "";
                }
                catch (Exception ex)
                {


                }
            }

            return portalUser;
        }
    }
}