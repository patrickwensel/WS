using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace PIP
{
    /// <summary>
    /// Summary description for portalUsers
    /// </summary>
    [WebService(Namespace = "http://www.willscot.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    [ToolboxItem(false)]
    public class portalUsers : System.Web.Services.WebService
    {

        [System.Web.Services.WebMethod]

        public string[] getUser(string prefixText, int count)
        {
            DataView aDV = new DataView((DataTable)Application["PeopleSel"]," full_name like '%" + prefixText + "%'","",DataViewRowState.CurrentRows);
            //aDV.RowFilter = " full_name like '" + prefixText + "%'";

            string[] aRetVal = new string[aDV.Count];
            for (int i=0;i<aDV.Count;i++){
                aRetVal[i] = aDV[i][0].ToString();
            }


            return aRetVal;
        }
    }
}
