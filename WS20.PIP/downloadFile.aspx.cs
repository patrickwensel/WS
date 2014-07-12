using Devart.Data.Oracle;
using Plumtree.Remote.Portlet;
using System;

namespace PIP
{
    public partial class downloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
			try
			{
                //could file when page loads and pull from that, but this way it works even if the page isn't up
                common aCommon = new common(Application, "ENG", PortletContextFactory.CreatePortletContext(Request, Response));

                OracleDataReader aDR =  aCommon.GetReader(string.Format(
                        @"SELECT file_type,file_data 
                            FROM pip_files 
                           WHERE project_id = '{0}'
                             AND row_id = '{1}'"
                        , Request.QueryString["pid"]
                        , Request.QueryString["rowid"]
                    ));

                if (aDR != null && aDR.Read())
                {
                    Response.ContentType = aDR["file_type"].ToString();
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + Request.QueryString["fname"]);
                    byte[] aByte = (byte[])aDR["file_data"];
                    Response.OutputStream.Write(aByte, 0, aByte.Length);
                }
                else
                {
                    Response.ContentType = "text/plain";
                    Response.AddHeader("Content-Disposition", "attachment;filename=fileNotFound.txt");
                    Response.Write("File Not Found.");
                    aCommon.logError("downloadFile.aspx.cs", "Page_Load", "File not Found", "pid=" + Request.QueryString["pid"] + "  fname=" + Request.QueryString["fname"], "INFO");
                }
			}
			catch (Exception ex)
			{
                Response.AppendHeader("content-disposition", "attachment;filename=error.txt");
                Response.ContentType = "text/plain";
                Response.Write("Error Loading File."+ex.ToString());
            }
			Response.End();
        }
    }
}