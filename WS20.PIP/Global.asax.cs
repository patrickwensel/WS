using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace PIP
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            common aCommon = new common(Application, "ENG", null);
            aCommon.loadApp();
            aCommon.cleanUp();
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}