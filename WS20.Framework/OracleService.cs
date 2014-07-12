using System.Configuration;
using Devart.Data.Oracle;

namespace WS30.Framework
{
    public class OracleService
    {
        public static OracleConnection GetConnection(string source)
        {
            switch (source)
            {
                case "WEB":
                    return null;
                case "JDE_WEB":
                    OracleConnection mConnection = new OracleConnection(ConfigurationManager.ConnectionStrings["JDE_WEB"].ConnectionString);
                    //OracleConnectionStringBuilder oracleConnectionStringBuilder = new OracleConnectionStringBuilder();
                    //oracleConnectionStringBuilder.Direct = true;
                    //oracleConnectionStringBuilder.Server = "ws85corp.willscot.com";
                    //oracleConnectionStringBuilder.Port = 1521;
                    //oracleConnectionStringBuilder.Sid = "devl";
                    //oracleConnectionStringBuilder.UserId = "jde_web";
                    //oracleConnectionStringBuilder.Password = "jde_web";
                    //OracleConnection myConnection = new OracleConnection(oracleConnectionStringBuilder.ConnectionString);
                    //myConnection.Open();
                    //return myConnection;
                    mConnection.Open();
                    return mConnection;
                case "OLI":
                    return null;

                default:
                    return null;

            }
        }
    }
}
