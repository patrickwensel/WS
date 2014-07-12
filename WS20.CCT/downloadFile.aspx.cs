using System;
	using System.Data;
	using System.Configuration;
	using System.Collections;
	using System.IO;
	using System.Web;
	using System.Web.Security;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.WebControls.WebParts;
	using System.Web.UI.HtmlControls;
	using System.Text;
using WS20.CCT.App_Data;

public partial class downloadFile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool mLogin = (Session["login"] == null) ? false : Convert.ToBoolean(Session["login"]);

        Response.Clear();
        try
        {
            //file
            switch (Request.QueryString["type"])
            {
                case "F":
                    if (!mLogin) { throw new Exception("Not Logged in"); }
                    string path = Request.QueryString["source"].ToString();
                    //	string path = MapPath(fname);
                    string name = Path.GetFileName(path);
                    //string ext = Path.GetExtension(path);

                    Response.ContentType = "text/HTML";
                    Response.AppendHeader("content-disposition", "attachment; filename=" + name);
                    Response.WriteFile(path);
                    break;
                case "E":
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", "attachment;filename=cctExcel.xls");
                    Response.Write(Session[Request.QueryString["source"]].ToString());
                    break;
                case "D":
                    if (!mLogin) { throw new Exception("Not Logged in"); }
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", "attachment;filename=cctExcel.xls");
                    Response.Write(Write_Excel(Request.QueryString["source"]));
                    break;
                default:
                    throw new Exception("Bad Type");
            }
        }
        catch (Exception ex)
        {
            Response.AppendHeader("content-disposition", "attachment;");
            Response.ContentType = "text/plain";
            Response.Write("Error Opening File.");
            Common mCommon = new Common();
            mCommon.logError("downloadFile.aspx.cs", "Page_Load", Request.QueryString.ToString(), Session["gNTName"].ToString(), ex.ToString(), "ERROR");

        }
        Response.End();
    }

	private string Write_Excel(string theDVName)
	{
		string aRet = "";

		#region execl header
		aRet = string.Format(@"<html xmlns:o=""urn:schemas-microsoft-com:office:office""
								xmlns:x=""urn:schemas-microsoft-com:office:excel""
								xmlns=""http://www.w3.org/TR/REC-html40"">
								<head>
								<meta http-equiv=Content-Type content=""text/html; charset=windows-1252"">
								<meta name=ProgId content=Excel.Sheet>
								<meta name=Generator content=""Microsoft Excel 9"">
								<!--[if gte mso 9]><xml>
								 <o:DocumentProperties>
								  <o:Author>Williams Scotsman</o:Author>
								  <o:LastAuthor>Williams Scotsman</o:LastAuthor>
								  <o:LastPrinted>2006-02-15T20:59:06Z</o:LastPrinted>
								  <o:Created>2006-02-15T20:32:29Z</o:Created>
								  <o:LastSaved>2006-02-28T13:40:38Z</o:LastSaved>
								  <o:Company>Williams Scotsman, Inc.</o:Company>
								  <o:Version>1.0.0</o:Version>
								 </o:DocumentProperties>
								 <o:OfficeDocumentSettings>
								  <o:DownloadComponents/>
								  <o:LocationOfComponents HRef=""file:///d:/msowc.cab""/>
								 </o:OfficeDocumentSettings>
								</xml><![endif]-->
								<style>
								<!--table
									{{mso-displayed-decimal-separator:""\."";
									mso-displayed-thousand-separator:""\,"";}}
								@page
									{{margin:.5in .5in .5in .5in;
									mso-header-margin:.1in;
									mso-footer-margin:.1in;}}
								tr
									{{mso-height-source:auto;}}
								col
									{{mso-width-source:auto;}}
								br
									{{mso-data-placement:same-cell;}}
								.style0
									{{mso-number-format:General;
									text-align:general;
									vertical-align:bottom;
									white-space:nowrap;
									mso-rotate:0;
									mso-background-source:auto;
									mso-pattern:auto;
									color:windowtext;
									font-size:10.0pt;
									font-weight:400;
									font-style:normal;
									text-decoration:none;
									font-family:Arial;
									mso-generic-font-family:auto;
									mso-font-charset:0;
									border:none;
									mso-protection:locked visible;
									mso-style-name:Normal;
									mso-style-id:0;}}
								td
									{{mso-style-parent:style0;
									padding-top:1px;
									padding-right:1px;
									padding-left:1px;
									mso-ignore:padding;
									color:windowtext;
									font-size:10.0pt;
									font-weight:400;
									font-style:normal;
									text-decoration:none;
									font-family:Arial;
									mso-generic-font-family:auto;
									mso-font-charset:0;
									mso-number-format:General;
									text-align:general;
									vertical-align:bottom;
									border:none;
									mso-background-source:auto;
									mso-pattern:auto;
									mso-protection:locked visible;
									white-space:nowrap;
									mso-rotate:0;}}
								.xl24
									{{mso-style-parent:style0;
									font-size:8.0pt;
									font-family:Arial, sans-serif;
									mso-font-charset:0;}}
								.xl25
									{{mso-style-parent:style0;
									color:#003300;
									font-weight:700;
									font-family:Arial, sans-serif;
									mso-font-charset:0;}}
								.xl26
									{{mso-style-parent:style0;
									font-size:8.0pt;
									font-weight:700;
									font-family:Arial, sans-serif;
									mso-font-charset:0;
									border-top:none;
									border-right:none;
									border-bottom:.5pt solid windowtext;
									border-left:none;}}
								-->
								</style>
								<!--[if gte mso 9]><xml>
								 <x:ExcelWorkbook>
								  <x:ExcelWorksheets>
								   <x:ExcelWorksheet>
									<x:Name>Search Results</x:Name>
									<x:WorksheetOptions>
									 <x:DefaultRowHeight>225</x:DefaultRowHeight>
									 <x:FitToPage/>
									 <x:Print>
									  <x:ValidPrinterInfo/>
									  <x:HorizontalResolution>600</x:HorizontalResolution>
									  <x:VerticalResolution>600</x:VerticalResolution>
									 </x:Print>
									 <x:Selected/>
									 <x:ProtectContents>False</x:ProtectContents>
									 <x:ProtectObjects>False</x:ProtectObjects>
									 <x:ProtectScenarios>False</x:ProtectScenarios>
									</x:WorksheetOptions>
								   </x:ExcelWorksheet>
								  </x:ExcelWorksheets>
								  <x:WindowHeight>11505</x:WindowHeight>
								  <x:WindowWidth>17115</x:WindowWidth>
								  <x:WindowTopX>360</x:WindowTopX>
								  <x:WindowTopY>60</x:WindowTopY>
								  <x:ProtectStructure>False</x:ProtectStructure>
								  <x:ProtectWindows>False</x:ProtectWindows>
								 </x:ExcelWorkbook>
								</xml><![endif]-->
								</head>

								<body link=blue vlink=purple class=xl24>

								<table x:str border=0 cellpadding=0 cellspacing=0 width=2407 style='border-collapse:
								 collapse;table-layout:fixed;width:1806pt'>
								 <col class=xl24 width=124 span=17 style='mso-width-source:userset;mso-width-alt:
								 4534;width:93pt'>
								 <col class=xl24 width=70 style='mso-width-source:userset;mso-width-alt:2560;
								 width:53pt'>
								 <col class=xl24 width=111 style='mso-width-source:userset;mso-width-alt:4059;
								 width:83pt'>
								 <col class=xl24 width=118 style='mso-width-source:userset;mso-width-alt:4315;
								 width:89pt'>
								 "
							, theDVName);
		#endregion

		#region execl table head
		aRet += string.Format(@"<tr height=15 style='height:11.25pt'>
								  <td height=15 class=xl26 style='height:11.25pt'>ID</td>
								  <td class=xl26>Type</td>
								  <td class=xl26>Order #</td>
								  <td class=xl26>DCT</td>
								  <td class=xl26>DOC #</td>
								  <td class=xl26>Company</td>
								  <td class=xl26>Email</td>
								  <td class=xl26>Amount</td>
								  <td class=xl26>Reason</td>	
								  <td class=xl26>Card Type</td>	
								  <td class=xl26>Status</td>	
								  <td class=xl26>Credit Card #</td>	
								  <td class=xl26>Exp Date</td>	
								  <td class=xl26>Pay Status</td>	
								  <td class=xl26>Maint Date</td>	
								  <td class=xl26>Gross</td>	
								  <td class=xl26>Open</td>	
								  <td class=xl26>ODOC</td>	
								  <td class=xl26>Invoice Date</td>	
								  <td class=xl26>Remark</td>	
								  <td class=xl26>Cust {0}CB{0}</td>	
								  <td class=xl26>Cust {0}CS{0}</td>	
								  <td class=xl26>Company</td>	
								  <td class=xl26>Contact Name</td>	
								  <td class=xl26>Contact Phone</td>	
								  <td class=xl26>Name</td>	
								  <td class=xl26>Phone</td>	
								  <td class=xl26>Country</td>
								  <td class=xl26>Street</td>
								  <td class=xl26>City</td>
								  <td class=xl26>State</td>
								  <td class=xl26>Zip</td>
								  <td class=xl26>Notes</td>
							 </tr>"
					, "\"");
		#endregion

		#region execl table body
		DataView aDV = (DataView)Session[theDVName];
		Common mCommon = new Common();

		for (int i = 0; i < aDV.Count; i++)
		{
			aRet += string.Format(@"<tr height=15 style='height:11.25pt'>
												<td class=xl24>{0}</td>
												<td class=xl24>{1}</td>
												<td class=xl24>{2}</td>
												<td class=xl24>{3}</td>
												<td class=xl24>{4}</td>
												<td class=xl24>{5}</td>
												<td class=xl24>{6}</td>
												<td class=xl24 x:num>{7}</td>
												<td class=xl24>{8}</td>
												<td class=xl24>{9}</td>
												<td class=xl24>{10}</td>
												<td class=xl24>************&nbsp;{11}</td>
												<td class=xl24>{12}</td>
												<td class=xl24>{13}</td>
												<td class=xl24>{14}</td>
												<td class=xl24 x:num>{15}</td>
												<td class=xl24 x:num>{16}</td>
												<td class=xl24>{17}</td>
												<td class=xl24>{18}</td>
												<td class=xl24>{19}</td>
												<td class=xl24>{20}</td>
												<td class=xl24>{21}</td>
												<td class=xl24>{22}</td>
												<td class=xl24>{23}</td>
												<td class=xl24>{24}</td>
												<td class=xl24>{25}&nbsp;{26}</td>
												<td class=xl24>{27}</td>
												<td class=xl24>{28}</td>
												<td class=xl24>{29}</td>
												<td class=xl24>{30}</td>
												<td class=xl24>{31}</td>
												<td class=xl24>{32}</td>
												<td class=xl24>{33}</td>
											</tr>
								", aDV[i]["id"]
								 , aDV[i]["type_name"]
								 , aDV[i]["order_skey"]
								 , aDV[i]["dct"]
								 , aDV[i]["doc"]
								 , aDV[i]["kco"]
								 , aDV[i]["email"]
								 , aDV[i]["amount"]
								 , aDV[i]["reason_code"]
								 , mCommon.getCCType(aDV[i]["credit_card_no"].ToString())
								 , aDV[i]["status_name"]
								 , aDV[i]["credit_card_suffix"]
								 , aDV[i]["exp"]
								 , aDV[i]["paystatus"]
								 , aDV[i]["maint_date"]
								 , aDV[i]["gross"]
								 , aDV[i]["openamount"]
								 , aDV[i]["rpodoc"]
								 , aDV[i]["invoice_date"]
								 , aDV[i]["remark"]
								 , aDV[i]["cb"]
								 , aDV[i]["cs"]
								 , aDV[i]["company"]
								 , aDV[i]["contact_name"]
								 , aDV[i]["contact_phone"]
								 , aDV[i]["card_first_name"]
								 , aDV[i]["card_last_name"]
								 , aDV[i]["card_phone"]
								 , aDV[i]["card_country"]
								 , aDV[i]["card_street"]
								 , aDV[i]["card_city"]
								 , aDV[i]["card_state"]
								 , aDV[i]["card_zip"]
								 , aDV[i]["notes"]
								 );
		}
		#endregion


		aRet += @"<![if supportMisalignedColumns]>
						 <tr height=0 style='display:none'>
						  <td width=124 style='width:93pt'></td>
						  <td width=124 style='width:93pt'></td>
						  <td width=124 style='width:93pt'></td>
						  <td width=124 style='width:93pt'></td>
						  <td width=124 style='width:93pt'></td>
						  <td width=124 style='width:93pt'></td>
						  <td width=124 style='width:93pt'></td>
						  <td width=124 style='width:93pt'></td>
						 </tr>
						 <![endif]>
						</table>
						</body>
						</html>";
		return aRet;
	}
}