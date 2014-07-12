using System.Collections.Generic;
using System.Web.UI.WebControls;
using AjaxPro;
using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;
using Devart.Data.Oracle;
using WS20.CCT.App_Data;
using WS20.Framework;
using System.Web.UI;

namespace WS20.CCT
{
    public partial class index : System.Web.UI.Page
    {

        protected void mBOASearchButton_Click(object sender, EventArgs e)
        {

            WSDBConnection aConn = new WSDBConnection();
            OracleDataReader aDR = null;
            int aRowsPerPage;
            int.TryParse(mBOARowsPerPage.Value, out aRowsPerPage);

            int ct = 0;
            int aPage = 0;
            string aPageNav = "";
            string aQuery = "";
            string lazytotal = "";
            int aMaxRows = 500;
            string aExcel = "";
            string aTableHead = @"<table cellpadding='3' cellspacing='0'>
                                    <thead>
                                        <tr><td class='printLink' colspan='12'><a href=""#"" onclick=""printPage('E','boaExcel');return false;"">Export to Excel</a></td></tr>
                                        <tr>
                                            <th class='headLeft'><a href='#' onclick=""boaSort('group_id')"" title='Click to Sort by ID'>ID</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('type_desc')"" title='Click to Sort by Type'>Type</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('order_skey')"" title='Click to Sort by Order #'>Order #</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('rpdct')"" title='Click to Sort by Dct'>Dct</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('rpdoc')"" title='Click to Sort by Doc'>Doc</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('rpkco')"" title='Click to Sort by Company'>Company</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('create_date')"" title='Click to Sort by Create Date'>Create Date</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('create_user_id')"" title='Click to Sort by Create User'>Create User</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('chargeamt')"" title='Click to Sort by Amount'>Amount</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('payment_account_sub_type')"" title='Click to Sort by Account Type'>Account Type</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('cnfrm_number')"" title='Click to Sort by Cnfrm #'>Cnfrm #</a></th>
                                            <th class='head'><a href='#' onclick=""boaSort('status_desc')"" title='Click to Sort by Status'>Status</a></th>
                                        </tr>
                                    </thead>
                                    <tbody>";

            mBOAResultsDiv.InnerHtml = "";
            mBOASearchMsgDiv.InnerHtml = "";

            try
            {
                if (aConn.Open("CCKEY"))
                {
                    #region query

                    aQuery = string.Format(@"
                                        SELECT * FROM (
                                        SELECT /*+ Rule */ h.group_id
                                              ,tl.member_desc type_desc
                                              ,nvl(oh.order_skey,d.order_skey) as order_skey
                                              ,rp.rpdct
                                              ,rp.rpdoc
                                              ,rp.rpkco
                                              ,h.email_address
                                              ,TRIM(to_char(d.chargeamt / 100
                                                           ,'$999,999,999.99')) chargeamt
                                              ,TRIM(to_char(SUM(d.chargeamt) over(PARTITION BY 1) / 100
                                                               ,'$999,999,999.99')) lazytotal
                                              ,decode(h.payment_account_type
                                                     ,'CC'
                                                     ,h.payment_account_sub_type
                                                     ,decode(h.payment_account_sub_type
                                                            ,NULL
                                                            ,''
                                                            ,'ACH')) payment_account_sub_type
                                              ,CASE
                                                WHEN h.status_id = 5 THEN
                                                 'Pending'
                                                WHEN h.status_id = 11 THEN
                                                 'Returned'
                                                WHEN h.cnfrm_number IS NOT NULL THEN
                                                 'Complete'
                                                ELSE
                                                 'Incomplete'
                                              END status_desc
                                              ,h.cnfrm_number
                                              ,TRIM(to_char(h.amt / 100
                                                           ,'$999,999,999.99')) totalamt
                                              ,decode(h.payment_account_type
                                                     ,'CC'
                                                     ,'Exp Date'
                                                     ,'Routing Number') payment_account_type
                                              ,decode(h.payment_account_type
                                                     ,'CC'
                                                     ,h.credit_card_account_number
                                                     ,'ACH'
                                                     ,h.bank_account_number
                                                     ,NULL) account_number
                                              ,decode(h.payment_account_type
                                                     ,'CC'
                                                     ,decode(h.credit_card_expiration_date
                                                            ,NULL
                                                            ,' '
                                                            ,substr(lpad(h.credit_card_expiration_date,4,0)
                                                                   ,0
                                                                   ,2) || '/' ||
                                                             substr(lpad(h.credit_card_expiration_date,4,0)
                                                                   ,3))
                                                     ,'ACH'
                                                     ,h.bank_routing_number
                                                     ,NULL) account_other
                                              ,MIN(decode(rp.rppst
                                                         ,NULL
                                                         ,''
                                                         ,rp.rppst || ' - ' || dr.drdl01)) paystatus
                                              ,h.create_date
                                              ,h.create_user_id
                                              ,h.charge_date
                                              ,h.charge_user_id
                                              ,TRIM(to_char((nvl(SUM(rpag)
                                                                ,0) * .01)
                                                           ,'$999,999,999.99')) gross
                                              ,TRIM(to_char((nvl(SUM(rpaap)
                                                                ,0) * .01)
                                                           ,'$999,999,999.99')) openamount
                                              ,rp.rpodoc
                                              ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
                                                      ,'MM/DD/YY') AS invoice_date
                                              ,rp.rprmk remark
                                              ,ab.aban86 c
                                              ,ab.aban85 cb
                                              ,oh.ship_an8 cs
                                              ,rp.rpalph company
                                              ,h.returned_date
                                          FROM cckey.boa_trans_header h
                                              ,cckey.boa_trans_detail d
                                              ,cckey.boa_lookup       tl
                                              /*,cckey.boa_lookup       sl*/
                                              ,f03b11           rp
                                              ,f0005            dr
                                              ,f0101            ab
                                              ,omb_order_header oh
                                         WHERE h.group_id = d.group_id
                                           AND h.type_id = tl.id
                                           /*AND h.status_id = sl.id*/
                                              
                                           AND dr.drsy(+) = '00  '
                                           AND dr.drrt(+) = 'PS'
                                           AND TRIM(dr.drky(+)) = rp.rppst
                                              
                                           AND d.doc = rp.rpdoc(+)
                                            /* AND rp.rpsfx(+) = '001' */
                                           AND rp.rpan8 = ab.aban8(+)
                                           AND oh.ship_an8(+) = rp.rpan8
                                            
                                           AND h.type_id != 9
                                           {0}
                                           {1}
                                           {2}
                                           {3}
                                           {4}
                                           {5}
                                           {6}
                                           {7}
                                           {8}
                                           {9}
                                           {10}
                                           {11}
                                           {12}
                                           {13}
                                           {14}
                                           {17}
                                           {18}
                                           {19}
                                        GROUP BY h.group_id
                                             ,tl.member_desc
                                             ,nvl(oh.order_skey,d.order_skey)
                                             ,rp.rpdct
                                             ,rp.rpdoc
                                             ,rp.rpkco
                                             ,h.email_address
                                             ,d.chargeamt
                                             ,decode(h.payment_account_type
                                                    ,'CC'
                                                    ,h.payment_account_sub_type
                                                    ,decode(h.payment_account_sub_type
                                                           ,NULL
                                                           ,''
                                                           ,'ACH'))
                                             ,CASE
                                                WHEN h.status_id = 5 THEN
                                                 'Pending'
                                                WHEN h.status_id = 11 THEN
                                                 'Returned'
                                                WHEN h.cnfrm_number IS NOT NULL THEN
                                                 'Complete'
                                                ELSE
                                                 'Incomplete'
                                              END
                                             ,h.cnfrm_number
                                             ,h.amt
                                             ,decode(h.payment_account_type
                                                    ,'CC'
                                                    ,'Exp Date'
                                                    ,'Routing Number')
                                             ,decode(h.payment_account_type
                                                    ,'CC'
                                                    ,h.credit_card_account_number
                                                    ,'ACH'
                                                    ,h.bank_account_number
                                                    ,NULL)
                                             ,decode(h.payment_account_type
                                                     ,'CC'
                                                     ,decode(h.credit_card_expiration_date
                                                            ,NULL
                                                            ,' '
                                                            ,substr(lpad(h.credit_card_expiration_date,4,0)
                                                                   ,0
                                                                   ,2) || '/' ||
                                                             substr(lpad(h.credit_card_expiration_date,4,0)
                                                                   ,3))
                                                     ,'ACH'
                                                     ,h.bank_routing_number
                                                     ,NULL)
                                             ,h.create_date
                                             ,h.create_user_id
                                             ,h.charge_date
                                             ,h.charge_user_id
                                             ,rp.rpodoc
                                             ,rp.rpdivj
                                             ,rp.rprmk
                                             ,ab.aban86
                                             ,ab.aban85
                                             ,oh.ship_an8
                                             ,rp.rpalph
                                             ,h.returned_date
                                            ORDER BY {15} group_id)
                                             WHERE rownum <= {16}"

                                            , mBOATransID.Value == "" ? "" : " AND h.group_id = " + mBOATransID.Value
                                            , mBOACB.Value == "" ? "" : " AND ab.aban85 = " + mBOACB.Value
                                            , mBOACustName.Value == "" ? "" : " AND lower(ab.abalph) LIKE lower('%" + mBOACustName.Value + "%')"
                                            , mBOACS.Value == "" ? "" : " AND ab.aban8 = " + mBOACS.Value
                                            , mBOAOdoc.Value == "" ? "" : " AND rp.rpodoc = " + mBOAOdoc.Value
                                            , mBOAOrder.Value == "" ? "" : " AND oh.order_skey = " + mBOAOrder.Value
                                            , mBOADCT.Value == "" ? "" : " AND rp.rpdct = upper('" + mBOADCT.Value + "')"
                                            , mBOADOC.Value == "" ? "" : " AND rp.rpdoc = " + mBOADOC.Value
                                            , mBOAKCO.Value == "" ? "" : " AND rp.rpkco = '" + mBOAKCO.Value + "'"
                                            , mBOASuffx.Value == "" ? "" : " AND h.credit_card_account_number like '%" + mBOASuffx.Value + "%'"
                                            , mBOAChargeDateFrom.Text != "" && mBOAChargeDateTO.Text != "" ? " AND h.charge_date BETWEEN to_date('" + mBOAChargeDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS')-(4/24) /*subtract 4 hours so the range is 8-8*/ AND to_date('" + mBOAChargeDateTO.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')-(4/24) /*subtract 4 hours so the range is 8-8*/" :
                                            (mBOAChargeDateFrom.Text != "" ? " AND h.charge_date > to_date('" + mBOAChargeDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS')-(4/24) /*subtract 4 hours so the range is 8-8*/" :
                                            (mBOAChargeDateTO.Text != "" ? " AND h.charge_date < to_date('" + mBOAChargeDateTO.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')-(4/24) /*subtract 4 hours so the range is 8-8*/" : ""))
                                            , mBOAStatusID.Value == "P" ? " AND h.status_id = 5" :
                                             (mBOAStatusID.Value == "C" ? " AND h.status_id not in(5,11) AND cnfrm_number is not null" :
                                             (mBOAStatusID.Value == "X" ? " AND h.status_id not in(5,11) AND cnfrm_number is null " :
                                             (mBOAStatusID.Value == "R" ? " AND h.status_id = 11 " : "")))
                                             , mBOATypeID.Value == "AC" ? " AND h.type_id in (1,3,8)" :
                                             (mBOATypeID.Value == "AO" ? " AND h.type_id in (1,3)" :
                                             (mBOATypeID.Value == "WO" ? " AND h.type_id = 1" :
                                             (mBOATypeID.Value == "PE" ? " AND h.type_id = 2" :
                                             (mBOATypeID.Value == "PL" ? " AND h.type_id = 3" :
                                             (mBOATypeID.Value == "PR" ? " AND h.type_id = 4" :
                                             (mBOATypeID.Value == "PM" ? " AND h.type_id = 12" :
                                             (mBOATypeID.Value == "M" ? " AND h.type_id = 8" : "")))))))
                                            , mBOACCType.Value == "ACH" ? " AND h.payment_account_type = 'ACH' " :
                                            (mBOACCType.Value == "MSTRVISA" ? " AND h.payment_account_sub_type in ('MC','VISA')" :
                                            (mBOACCType.Value == "" ? "" : " AND h.payment_account_sub_type = '" + mBOACCType.Value + "'"))
                                            , mBOACnfrmNum.Value == "" ? "" : " AND h.cnfrm_number = " + mBOACnfrmNum.Value
                                            , boaSortBy.Value == "" ? "" : boaSortBy.Value + ","
                                            , aMaxRows
                                            , mBOAC.Value == "" ? "" : " AND ab.aban86 = " + mBOAC.Value
                                            , mBOACreateDateFrom.Text != "" && mBOACreateDateTo.Text != "" ? " AND h.create_date BETWEEN to_date('" + mBOACreateDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS') AND to_date('" + mBOACreateDateTo.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')" :
                                            (mBOACreateDateFrom.Text != "" ? " AND h.create_date > to_date('" + mBOACreateDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS')" :
                                            (mBOACreateDateTo.Text != "" ? " AND h.create_date < to_date('" + mBOACreateDateTo.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')" : ""))
                                            , mBOAReturnDateFrom.Text != "" && mBOAReturnDateTo.Text != "" ? " AND h.returned_date BETWEEN to_date('" + mBOAReturnDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS') AND to_date('" + mBOAReturnDateTo.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')" :
                                            (mBOAReturnDateFrom.Text != "" ? " AND h.returned_date > to_date('" + mBOAReturnDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS')" :
                                            (mBOAReturnDateTo.Text != "" ? " AND h.returned_date < to_date('" + mBOAReturnDateTo.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')" : ""))

                                            );

                    #endregion

                    aDR = aConn.GetReader(aQuery);

                    #region excel header

                    aExcel = string.Format(@"
                                <html xmlns:o=""urn:schemas-microsoft-com:office:office""
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
                                .boldTD{{
                                    font: bold 12px Verdana;
                                    text-align:left;
                                }}
                                .cellTD{{
                                    text-align:left;
                                }}
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
                                        <table>
                                            <tr><td class='boldTD'>BOA Search</td></tr>
                                            <tr><td></td></tr>
                                            <tr><td class='boldTD'>Transaction ID:</td><td>{0}</td><td class='boldTD'>Cnfrm #:</td><td>{1}</td></tr>
                                            <tr><td class='boldTD'>Customer ""CB"" #:</td><td>{2}</td><td class='boldTD'>Customer Name:</td><td>{3}</td></tr>
                                            <tr><td class='boldTD'>Customer ""CS"" #:</td><td>{4}</td><td class='boldTD'>ODOC:</td><td>{5}</td></tr>
                                            <tr><td class='boldTD'>Order #:</td><td>{6}</td><td class='boldTD'>Invoice #:</td><td>{7}</td></tr>
                                            <tr><td class='boldTD'>Credit Card Suffix:</td><td>{8}</td><td class='boldTD'>Charge Date:</td><td>{9}</td></tr>
                                            <tr><td class='boldTD'>Transaction Status:</td><td>{10}</td><td class='boldTD'>Transaction Type:</td><td>{11}</td></tr>
                                            <tr><td class='boldTD'>Account Type:</td><td>{12}</td><td></tr>
                                            <tr><td></td></tr>
                                            <tr><td></td></tr>
                                            <tr>
                                                <th class='boldTD'>ID</th>
                                                <th class='boldTD'>Type</th>
                                                <th class='boldTD'>Order #</th>
                                                <th class='boldTD'>Dct</th>
                                                <th class='boldTD'>Doc</th>
                                                <th class='boldTD'>Company</th>
                                                <th class='boldTD'>Create User</th>
                                                <th class='boldTD'>Create Date</th>
                                                <th class='boldTD'>Amount</th>
                                                <th class='boldTD'>Account Type</th>
                                                <th class='boldTD'>Cnfrm #</th>
                                                <th class='boldTD'>Status</th>
                                                <th class='boldTD'>Account Number #</th>
                                                <th class='boldTD'>Exp Date / Routing Number</th>
                                                <th class='boldTD'>Pay Status</th>
                                                <th class='boldTD'>Charge Date</th>
                                                <th class='boldTD'>Charge User</th>
                                                <th class='boldTD'>Total Amt</th>
                                                <th class='boldTD'>Gross</th>
                                                <th class='boldTD'>Open</th>
                                                <th class='boldTD'>ODOC</th>
                                                <th class='boldTD'>Invoice Date</th>
                                                <th class='boldTD'>Remark</th>
                                                <th class='boldTD'>Cust ""CB"" #</th>
                                                <th class='boldTD'>Cust ""CS"" #</th>
                                                <th class='boldTD'>Company</th>
                                                <th class='boldTD'>Email</th>
                                                <th class='boldTD'>Returned Date</th>
                                            </tr>
                                            "
                                            , mBOATransID.Value
                                            , mBOACnfrmNum.Value
                                            , mBOACB.Value
                                            , mBOACustName.Value
                                            , mBOACS.Value
                                            , mBOAOdoc.Value
                                            , mBOAOrder.Value
                                            , mBOADCT.Value + " " + mBOADOC.Value + " " + mBOAKCO.Value
                                            , mBOASuffx.Value
                                            , mBOAChargeDateFrom.Text + " - " + mBOAChargeDateTO.Text
                                            , mBOAStatusID.Items[mBOAStatusID.SelectedIndex].Text
                                            , mBOATypeID.Items[mBOATypeID.SelectedIndex].Text
                                            , mBOACCType.Items[mBOACCType.SelectedIndex].Text
                                            , "{"
                                            , "}");

                    #endregion

                    while (aDR.Read())
                    {
                        #region create header / add table to div / create nav

                        if (ct % aRowsPerPage == 0)
                        {
                            aPage++;
                            //closes current table & adds new table
                            if (ct != 0)
                            {
                                mBOAResultsDiv.InnerHtml += @"
                                        </table>
                                    </div>
                                    <div id='mBOAPage" + aPage + "' style='display:none;'>" + aTableHead;
                            }
                            else
                            {
                                lazytotal = aDR["lazytotal"].ToString();
                                mBOAResultsDiv.InnerHtml += "<div id='mBOAPage1'>" + aTableHead;
                            }

                            //update the nav
                            aPageNav += string.Format(@"<a href='#' id='mBOAPage{0}Link' {1} onClick=""changeTablePage('{0}')"">{0}</a>&nbsp;&nbsp;"
                                                    , aPage
                                                    , aPage == 1 ? "class='selectedPageLink'" : ""
                                                    );
                        }

                        #endregion

                        //main row leftCell
                        mBOAResultsDiv.InnerHtml += string.Format(@"
                                        <tr>
                                            <td class='leftCell' id='detailCell0{0}'><a href=""javascript:expandBOARow('{0}')"">{1}&nbsp;</a></td> 
                                            <td class='cell' id='detailCell1{0}'>{2}&nbsp;</td>
                                            <td class='cell' id='detailCell2{0}'>{3}&nbsp;</td>
                                            <td class='cell' id='detailCell3{0}'>{4}&nbsp;</td>
                                            <td class='cell' id='detailCell4{0}'>{5}&nbsp;</td>
                                            <td class='cell' id='detailCell5{0}'>{6}&nbsp;</td>
                                            <td class='cell' id='detailCell6{0}'>{7}&nbsp;</td>
                                            <td class='cell' id='detailCell7{0}'>{8}&nbsp;</td>
                                            <td class='cell right' id='detailCell8{0}'>{9}&nbsp;</td>
                                            <td class='cell' id='detailCell9{0}'>{10}&nbsp;</td>
                                            <td class='cell' id='detailCell10{0}'>{11}&nbsp;</td>
                                            <td class='cell' id='detailCell11{0}'>{12}&nbsp;</td>
                                        </tr>"
                                            , ct
                                            , aDR["group_id"].ToString()
                                            , aDR["type_desc"].ToString()
                                            , aDR["order_skey"].ToString()
                                            , aDR["rpdct"].ToString()
                                            , aDR["rpdoc"].ToString()
                                            , aDR["rpkco"].ToString()
                                            , aDR["create_date"].ToString()
                                            , aDR["create_user_id"].ToString()
                                            , aDR["chargeamt"].ToString()
                                            , aDR["payment_account_sub_type"].ToString()
                                            , aDR["cnfrm_number"].ToString()
                                            , aDR["status_desc"].ToString()
                                            );
                        //detail row
                        mBOAResultsDiv.InnerHtml += string.Format(@"
                                    <tr id='detailRow{0}' style='display:none;'>
                                        <td class='detailRow' colspan='12'>
                                            <table width:100%>
                                                <tr>
											        <td class='label'>Account Number #: </td><td class='input'>{2}</td>
											        <td class='label'>{1}: </td><td class='input'>{3}</td>
											        <td class='label'>Pay Status: </td><td class='input'>{4}</td>
										        </tr>
										        <tr>
											        <td class='label'>Charge Date: </td><td class='input'>{5}</td>
											        <td class='label'>Charge User: </td><td class='input'>{6}</td>
											        <td class='label'>Total Charge Amt: </td><td class='input'>{7}</td>
										        </tr>
                                                <tr>
											        <td class='label'>Invoice Gross: </td><td class='input'>{8}</td>
											        <td class='label'>Invoice Open: </td><td class='input'>{9}</td>
											        <td class='label'>ODOC: </td><td class='input'>{10}</td>
										        </tr>
										        <tr>
											        <td class='label'>Invoice Date: </td><td class='input'>{11}</td>
											        <td class='label'>Remark: </td><td class='input' colspan='3'>{12}</td>
										        </tr>
										        <tr>
                                                    <td class='label'>Cust ""C"" #: </td><td class='input'>{13}</td>
											        <td class='label'>Cust ""CB"" #: </td><td class='input'>{14}</td>
											        <td class='label'>Cust ""CS"" #: </td><td class='input'>{15}</td>
										        </tr>
                                                <tr>
                                                    <td class='label'>Company: </td><td class='input'>{16}</td>
                                                    <td class='label'>Email: </td><td class='input'>{17}</td>
                                                    <td class='label'>Returned Date: </td><td class='input'>{18}</td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>"
                                    , ct
                                    , aDR["payment_account_type"].ToString()
                                    , aDR["account_number"].ToString()
                                    , aDR["account_other"].ToString()
                                    , aDR["paystatus"].ToString()
                                    , aDR["charge_date"].ToString()
                                    , aDR["charge_user_id"].ToString()
                                    , aDR["totalamt"].ToString()
                                    , aDR["gross"].ToString()
                                    , aDR["openamount"].ToString()
                                    , aDR["rpodoc"].ToString()
                                    , aDR["invoice_date"].ToString()
                                    , aDR["remark"].ToString()
                                    , aDR["c"].ToString()
                                    , aDR["cb"].ToString()
                                    , aDR["cs"].ToString()
                                    , aDR["company"].ToString()
                                    , aDR["email_address"].ToString()
                                    , aDR["returned_date"].ToString()
                                    );

                        aExcel += string.Format(@"
                                            <tr>
                                                <td class='cellTD'>{0}</td>
                                                <td class='cellTD'>{1}</td>
                                                <td class='cellTD'>{2}</td>
                                                <td class='cellTD'>{3}</td>
                                                <td class='cellTD'>{4}</td>
                                                <td class='cellTD'>{5}</td>
                                                <td class='cellTD'>{26}</td>
                                                <td class='cellTD'>{27}&nbsp;</td>
                                                <td class='cellTD' x:num>{6}</td>
                                                <td class='cellTD'>{7}</td>
                                                <td class='cellTD'>{8}</td>
                                                <td class='cellTD'>{9}</td>
                                                <td class='cellTD'>{10}</td>
                                                <td class='cellTD'>{11}</td>
                                                <td class='cellTD'>{12}&nbsp;</td>
                                                <td class='cellTD'>{13}</td>
                                                <td class='cellTD'>{14}&nbsp;</td>
                                                <td class='cellTD'>{15}</td>
                                                <td class='cellTD' x:num>{16}</td>
                                                <td class='cellTD' x:num>{17}</td>
                                                <td class='cellTD' x:num>{18}</td>
                                                <td class='cellTD'>{19}</td>
                                                <td class='cellTD'>{20}&nbsp;</td>
                                                <td class='cellTD'>{21}</td>
                                                <td class='cellTD'>{22}</td>
                                                <td class='cellTD'>{23}</td>
                                                <td class='cellTD'>{24}</td>
                                                <td class='cellTD'>{25}</td>
                                            </tr>
                                            "
                                    , aDR["group_id"].ToString()
                                    , aDR["type_desc"].ToString()
                                    , aDR["order_skey"].ToString()
                                    , aDR["rpdct"].ToString()
                                    , aDR["rpdoc"].ToString()
                                    , aDR["rpkco"].ToString()
                                    , aDR["chargeamt"].ToString()
                                    , aDR["payment_account_sub_type"].ToString()
                                    , aDR["cnfrm_number"].ToString()
                                    , aDR["status_desc"].ToString()
                                    , aDR["account_number"].ToString()
                                    , aDR["account_other"].ToString()
                                    , aDR["paystatus"].ToString()
                                    , aDR["charge_date"].ToString()
                                    , aDR["charge_user_id"].ToString()
                                    , aDR["totalamt"].ToString()
                                    , aDR["gross"].ToString()
                                    , aDR["openamount"].ToString()
                                    , aDR["rpodoc"].ToString()
                                    , aDR["invoice_date"].ToString()
                                    , aDR["remark"].ToString()
                                    , aDR["cb"].ToString()
                                    , aDR["cs"].ToString()
                                    , aDR["company"].ToString()
                                    , aDR["email_address"].ToString()
                                    , aDR["returned_date"].ToString()
                                    , aDR["create_user_id"].ToString()
                                    , aDR["create_date"].ToString());

                        ct++;
                    }
                    mBOAResultsDiv.InnerHtml += string.Format(@"
                                    </tbody>
                                    <tfoot> 
                                        <tr>
                                            <td colspan='8' class='label'>Total:</td><td class='right'>{0}&nbsp;</td>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>"
                            , lazytotal
                            );

                    if (aPage > 1) { mBOAResultsDiv.InnerHtml += aPageNav; }
                    else if (ct == 0) { mBOAResultsDiv.InnerHtml = "No Results Found"; }

                    if (ct == aMaxRows) { mBOASearchMsgDiv.InnerHtml = "Only the first " + aMaxRows.ToString() + " rows are displayed."; }

                    if (Session["security_level"] != null && Session["security_level"].ToString() == "1")
                    {
                        mBOASearchMsgDiv.InnerHtml += "<br><br>" + aQuery;
                    }


                    aExcel += "</table></body></html>";
                    Session["boaExcel"] = aExcel;

                }
            }
            catch (Exception ex)
            {
                mBOASearchMsgDiv.InnerHtml = ex.ToString();
                mBOAResultsDiv.InnerHtml = aQuery;
            }
            finally
            {
                if (aDR != null)
                {
                    aDR.Close();
                    aDR.Dispose();
                }
                if (aConn != null) { aConn.Close(); }
            }


        }

        /// <summary>
        /// Logs error 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void HandleError(object sender, AsyncPostBackErrorEventArgs e)
        {
            Common aCommon = new Common();
            aCommon.logError("index.aspx.cs", "HandleError", "", WSSecurityPackage.NTLookup.GetNTUserName(this), e.Exception.ToString(), "ERROR");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //mBOAStatusID.Items.AddRange(((ListItem[])Application["boaStatus"]));
            //mBOATypeID.Items.AddRange(((ListItem[])Application["boaType"]));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(index));
            #region setup headers and footers

            mWSHeader.SetPageName("CCT");
            mWSHeader.SetAppTitleHtml("Version 1.4.0");
            mHeaderClientID.Value = mWSHeader.ClientID;

            #endregion //end of header/footer setup -  Put user code to initialize the page here

            #region if id is sent in clears current search values, set search id to id passed, sets page to search and calls search
            if (Request.QueryString["id"] != null)
            {
                //bad way of checking for an int, not sure of the actual function to use
                bool isInt = true;
                try { Convert.ToInt32(Request.QueryString["id"]); }
                catch { isInt = false; }

                if (isInt)
                {
                    searchClear();
                    Session["gSearchTransID"] = Request.QueryString["id"];
                    Session["SearchFlag"] = true;
                    Session["gPage"] = "search";
                }
            }
            #endregion

            #region check session for login, ntname,refresh_flag, server and current page


            string mNTName = "";
            if (Session["gNTName"] == null)
            {
                mNTName = WSSecurityPackage.NTLookup.GetNTUserName(this);
                Session["gNTName"] = mNTName;
            }
            else
            {
                mNTName = Session["gNTName"].ToString();
            }

            //if not logined in check
            bool login = CheckLogin();
            if (!login)
            {
                WSDBConnection aConn = new WSDBConnection();
                aConn.Open("JDE_WEB", 0);

                OracleDataReader mDR;
                string aQuery = string.Format(@"
												SELECT security_level
												  FROM jde_web.web_security t
													  ,mv_ws_emps           e
												 WHERE t.application_id = {0}
												   AND t.emp_key = e.employee_key
												   AND e.nt_user_id = '{1}'
												"
                                        , ConfigurationManager.AppSettings["WEB_APPLICATION_ID"]
                                        , mNTName
                                        );
                mDR = aConn.GetReader(aQuery);


                if (mDR.Read())
                {
                    login = true;
                    Session["login"] = true;
                    Session["security_level"] = mDR["security_level"];
                    //Session["showdebug"] = (Convert.ToInt32(mDR["security_level"]) == 0) ? "Y" : "N";
                }
                aConn.Close();
            }
            string mPage = (Session["gPage"] == null) ? "boaSearch" : Session["gPage"].ToString();

            string err = "";
            if (Convert.ToBoolean(Session["refresh"]))
            {
                err = "Session Refreshed";
                Session["refresh"] = false;
            }

            Session["gServer"] = Request.ServerVariables["SERVER_NAME"].ToString().ToLower();

            #endregion

            if (!login)
            {
                Dictionary<string, string> links = new Dictionary<string, string>
                    {
                        {"BOA Search", "javascript:menu('boaSearch','')"}
                    };

                mWSHeader.AddMenuItem(links);
                mJSConstSpan.InnerHtml = "<script language='JavaScript'> loadPage('boaSearch','') </script>";

                
            }
            else
            {
                loadData(mPage, err);
                mBody.Attributes.Add("onload", "getTrans()");
            }
        }

        private void loadData(string thePage, string theErr)
        {
            Dictionary<string, string> links = new Dictionary<string, string>
                {
                    {"Main Menu", "javascript:menu('menu','')"},
                    {"Transactions", "javascript:menu('trans','')"},
                    {"Search", "javascript:menu('search','')"},
                    {"Credit", "javascript:menu('credit','')"},
                    {"BOA Search", "javascript:menu('boaSearch','')"},
                };

            if (Session["security_level"] != null && Session["security_level"].ToString() == "1")
            {
                links.Add("Edit", "javascript:menu('edit','')");
            }

            mWSHeader.AddMenuItem(links);

            mJSConstSpan.InnerHtml = "<script language='JavaScript'> loadPage('boaSearch','') </script>";

            #region set defaults
            //set search defaults
            mSearchTransID.Value = (Session["gSearchTransID"] == null) ? "" : Session["gSearchTransID"].ToString();
            mSearchCustNum.Value = (Session["gSearchCustNum"] == null) ? "" : Session["gSearchCustNum"].ToString();
            mSearchCustName.Value = (Session["gSearchCustName"] == null) ? "" : Session["gSearchCustName"].ToString();
            mSearchOrderNum.Value = (Session["gSearchOrder"] == null) ? "" : Session["gSearchOrder"].ToString();
            mSearchDCT.Value = (Session["gSearchDCT"] == null) ? "" : Session["gSearchDCT"].ToString();
            mSearchDOC.Value = (Session["gSearchDOC"] == null) ? "" : Session["gSearchDOC"].ToString();
            mSearchKCO.Value = (Session["gSearchKCO"] == null) ? "" : Session["gSearchKCO"].ToString();
            mSearchCCSufix.Value = (Session["gSearchCCSufix"] == null) ? "" : Session["gSearchCCSufix"].ToString();
            mSearchSentTo.Value = (Session["gSearchSentTo"] == null) ? "" : Session["gSearchSentTo"].ToString();
            mSearchSentFrom.Value = (Session["gSearchSentFrom"] == null) ? "" : Session["gSearchSentFrom"].ToString();
            mSearchCardType.Value = (Session["gSearchCardType"] == null) ? "" : Session["gSearchCardType"].ToString();


            //set credit defaults
            mCreditTransID.Value = (Session["gCreditTransID"] == null) ? "" : Session["gCreditTransID"].ToString();
            mCreditCustNum.Value = (Session["gCreditCustNum"] == null) ? "" : Session["gCreditCustNum"].ToString();
            mCreditCustName.Value = (Session["gCreditCustName"] == null) ? "" : Session["gCreditCustName"].ToString();
            mCreditOrderNum.Value = (Session["gCreditOrder"] == null) ? "" : Session["gCreditOrder"].ToString();
            mCreditDCT.Value = (Session["gCreditDCT"] == null) ? "" : Session["gCreditDCT"].ToString();
            mCreditDOC.Value = (Session["gCreditDOC"] == null) ? "" : Session["gCreditDOC"].ToString();
            mCreditKCO.Value = (Session["gCreditKCO"] == null) ? "" : Session["gCreditKCO"].ToString();
            mCreditCCSufix.Value = (Session["gCreditCCSufix"] == null) ? "" : Session["gCreditCCSufix"].ToString();
            mCreditSentTo.Value = (Session["gCreditSentTo"] == null) ? "" : Session["gCreditSentTo"].ToString();
            mCreditSentFrom.Value = (Session["gCreditSentFrom"] == null) ? "" : Session["gCreditSentFrom"].ToString();
            mCreditCardType.Value = (Session["gCardType"] == null) ? "-1" : Session["gCardType"].ToString();
            #endregion

            #region write constants to js

            string jsConst = string.Format(@"
										<script language='JavaScript'>
											 AjaxPro.timeoutPeriod = {13}; 
											 AjaxPro.onTimeout = ajaxTimeOut;
											 AjaxPro.onError = ajaxError;
											 //constants
											 var STATUS_UNSENT = {0};
											 var STATUS_PROCESSING = {1};
											 var STATUS_COMPLETED = {2};
											 var STATUS_DECLINED = {3};
											 var STATUS_DELETED = {4};
											 var STATUS_DECLINED_DELETED = {5};
											 var STATUS_CREDITED = {6};
											 var TYPE_ONE_TIME = '{7}';
											 var TYPE_RECURRING = '{8}';
											 var SEARCH_FLAG = '{11}';
											 var CREDIT_SEARCH_FLAG = '{12}';
											 loadPage('{9}','{10}');	
											 getSearchSelects();
											 
										</script>
											"
                                            , ConfigurationManager.AppSettings["STATUS_UNSENT"]
                                            , ConfigurationManager.AppSettings["STATUS_PROCESSING"]
                                            , ConfigurationManager.AppSettings["STATUS_COMPLETED"]
                                            , ConfigurationManager.AppSettings["STATUS_DECLINED"]
                                            , ConfigurationManager.AppSettings["STATUS_DELETED"]
                                            , ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"]
                                            , ConfigurationManager.AppSettings["STATUS_CREDITED"]
                                            , ConfigurationManager.AppSettings["TYPE_ONE_TIME"]
                                            , ConfigurationManager.AppSettings["TYPE_RECURRING"]
                                            , thePage
                                            , theErr
                                            , (Convert.ToBoolean(Session["SearchFlag"])) ? "Y" : "N"
                                            , (Convert.ToBoolean(Session["CreditSearchFlag"])) ? "Y" : "N"
                                            , ConfigurationManager.AppSettings["TIMEOUT"]
                                            );
            mJSConstSpan.InnerHtml = jsConst;

            #endregion

        }

        #region common functions

        Common.RetStruct gRefresh = new Common.RetStruct();
        /**
         * CheckLogin
         * 
         * check to see if user is loged in
         **/
        public bool CheckLogin()
        {
            bool login;
            if (Session["login"] == null)
            {
                login = false;
                gRefresh.message = "REFRESH";
                Session["refresh"] = true;
            }
            else
            {
                login = Convert.ToBoolean(Session["login"]);
            }
            return login;

        }


        /**
         * menu
         * 
         * checks to see if user is loged in
         * moves user to page, if user isn't loged on
         * refreshes the page
         * 
         **/

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool menu(string page)
        {
            Session["gPage"] = page;
            return CheckLogin();
        }

        /**
         * getSearchSelects
         * 
         * gets the Drop down menu / adds types and statues to Session
         * Not checking for loging because function only called right after
         * user logs in
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string[] getSearchSelects()
        {
            Common common = new Common();

            return common.getSearchSelects(this);
        }


        /**
         * getFiles
         * 
         * returns a drop down menus of canada files that have been created
         * doesn't check for logon because currently does not use the session
         * if a session call is added, caues this function to sync
         **/
        [AjaxMethod]
        public object[] getFiles(bool all, string thePage)
        {
            object[] retVal = new object[2];
            retVal[0] = thePage;
            try
            {
                Common common = new Common();
                retVal[1] = common.fileSelect(thePage, all);
            }
            catch
            {
                retVal[1] = "Error Loading Files.";
            }

            return retVal;
        }

        /**
 * viewHistory
 * 
 * returns the history for a row
 **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct viewHistory(int theRow, string theLoc)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            string mQuery = "";
            Common mCommon = new Common();
            if (CheckLogin())
            {
                WSDBConnection aConn = new WSDBConnection();
                try
                {
                    aConn.Open("SISR", 0);
                    DataView mDV = (DataView)Session["g" + theLoc + "DV"];
                    DataTable mHistoryDT;

                    #region select history

                    mQuery = string.Format(@"
						SELECT k.audit_key
							  ,k.change_date
							  ,e.full_name AS user_name
							  ,k.iud_type
							  ,rs.new_value new_status
							  ,ra.new_value new_amount
							  ,re.new_value new_email
							  ,rn.new_value new_notes
						       
							  ,'' order_email
							  ,'' order_cname
							  ,'' order_cphone
							  ,'' order_first
							  ,'' order_last
							  ,'' order_street
							  ,'' order_city
							  ,'' order_state
							  ,'' order_zip
							  ,'' order_country
							  ,'' order_phone
						  FROM cct_audit_keys k
							  ,mv_ws_emps e
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'NOTES') rn
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'EMAIL') re
							  ,(SELECT audit_key
									  ,column_name
									  ,nvl(to_char(new_value * .01
												  ,'$999,999,999.99')
										  ,'') AS new_value
								  FROM cct_audit_record
								 WHERE column_name = 'AMOUNT') ra
							  ,(SELECT audit_key
									  ,column_name
									  ,status_name AS new_value
								  FROM cct_audit_record r
									  ,cct_status
								 WHERE column_name = 'STATUS_ID'
								   AND status_id = new_value) rs
						 WHERE k.audit_key = rn.audit_key(+)
						   AND k.audit_key = re.audit_key(+)
						   AND k.audit_key = ra.audit_key(+)
						   AND k.audit_key = rs.audit_key(+)
						   AND k.VALUE = {0}
						   AND k.column_name = 'id'
						   AND k.table_name = 'CCT_TRANS'
						   AND k.user_name = e.employee_key
						UNION
						SELECT k.audit_key
							  ,k.change_date
							  ,e.full_name AS user_name
							  ,k.iud_type
							  ,'' new_status
							  ,'' new_amount
							  ,'' new_email
							  ,'' new_notes
							  ,re.new_value   order_email
							  ,rcn.new_value  order_cname
							  ,rcp.new_value  order_cphone
							  ,rcfn.new_value order_first
							  ,rcln.new_value order_last
							  ,rcst.new_value order_street
							  ,rcc.new_value  order_city
							  ,rcsa.new_value order_state
							  ,rcz.new_value  order_zip
							  ,rcco.new_value order_country
							  ,rcph.new_value order_phone
						  FROM cct_audit_keys k
							  ,mv_ws_emps e
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'EMAIL') re
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CONTACT_NAME') rcn
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CONTACT_PHONE') rcp
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CARD_FIRST_NAME') rcfn
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CARD_LAST_NAME') rcln
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CARD_STREET') rcst
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CARD_CITY') rcc
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CARD_STATE') rcsa
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CARD_ZIP') rcz
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CARD_COUNTRY') rcco
							  ,(SELECT audit_key
									  ,column_name
									  ,new_value
								  FROM cct_audit_record
								 WHERE column_name = 'CARD_PHONE') rcph
						 WHERE k.audit_key = re.audit_key(+)
						   AND k.audit_key = rcn.audit_key(+)
						   AND k.audit_key = rcp.audit_key(+)
						   AND k.audit_key = rcfn.audit_key(+)
						   AND k.audit_key = rcln.audit_key(+)
						   AND k.audit_key = rcst.audit_key(+)
						   AND k.audit_key = rcc.audit_key(+)
						   AND k.audit_key = rcsa.audit_key(+)
						   AND k.audit_key = rcz.audit_key(+)
						   AND k.audit_key = rcco.audit_key(+)
						   AND k.audit_key = rcph.audit_key(+)
						   AND k.VALUE = {1}
						   AND k.column_name = 'order_skey'
						   AND k.table_name = 'CCT_ORDER_INFO'
						   AND k.user_name = e.employee_key
						 ORDER BY 1
					", mDV[theRow]["id"]
                     , mDV[theRow]["order_skey"]
                    );


                    mHistoryDT = aConn.GetDataTable(mQuery);

                    #endregion

                    #region create table

                    mRetVal.usHTML = @"
						<table cellpadding='3' cellspacing='0'>
							<thead>
								<tr>
									<th class=headLeft>Date</th>
									<th class=head>User</th>
									<th class=head>Type</th>
									<th class=head>Trans Email</th>
									<th class=head>Amount</th>
									<th class=head>Status</th>
									<th class=head>Notes</th>
									<th class=head>Order Email</th>
									<th class=head>Contact Name</th>
									<th class=head>Contact Phone</th>
									<th class=head>First Name</th>
									<th class=head>Last Name</th>
									<th class=head>Phone</th>
									<th class=head>Country</th>
									<th class=head>Street</th>
									<th class=head>City</th>
									<th class=head>State</th>
									<th class=head>Zip</th>		
								</tr>
							</thead>";

                    for (int i = 0; i < mHistoryDT.Rows.Count; i++)
                    {
                        mRetVal.usHTML += string.Format(@"
								<tr>
									<td class='leftCell'>{0}&nbsp;</td>
									<td class='cell'>{1}&nbsp;</td>
									<td class='cell'>{2}&nbsp;</td>
									<td class='cell'>{3}&nbsp;</td>
									<td class='cell'>{4}&nbsp;</td>
									<td class='cell'>{5}&nbsp;</td>
									<td class='cell'>{6}&nbsp;</td>
									<td class='cell'>{7}&nbsp;</td>
									<td class='cell'>{8}&nbsp;</td>
									<td class='cell'>{9}&nbsp;</td>
									<td class='cell'>{10}&nbsp;</td>
									<td class='cell'>{11}&nbsp;</td>
									<td class='cell'>{12}&nbsp;</td>
									<td class='cell'>{13}&nbsp;</td>
									<td class='cell'>{14}&nbsp;</td>
									<td class='cell'>{15}&nbsp;</td>
									<td class='cell'>{16}&nbsp;</td>
									<td class='cell'>{17}&nbsp;</td>
								</tr>
								", mHistoryDT.Rows[i]["change_date"]
                                 , mHistoryDT.Rows[i]["user_name"]
                                 , mHistoryDT.Rows[i]["iud_type"]
                                 , mHistoryDT.Rows[i]["new_email"]
                                 , mHistoryDT.Rows[i]["new_amount"]
                                 , mHistoryDT.Rows[i]["new_status"]
                                 , mHistoryDT.Rows[i]["new_notes"]
                                 , mHistoryDT.Rows[i]["order_email"]
                                 , mHistoryDT.Rows[i]["order_cname"]
                                 , mHistoryDT.Rows[i]["order_cphone"]
                                 , mHistoryDT.Rows[i]["order_first"]
                                 , mHistoryDT.Rows[i]["order_last"]
                                 , mHistoryDT.Rows[i]["order_phone"]
                                 , mHistoryDT.Rows[i]["order_country"]
                                 , mHistoryDT.Rows[i]["order_street"]
                                 , mHistoryDT.Rows[i]["order_city"]
                                 , mHistoryDT.Rows[i]["order_state"]
                                 , mHistoryDT.Rows[i]["order_zip"]
                                );
                    }

                    mRetVal.usHTML += "</table></div>";

                    #endregion
                }
                catch (Exception e)
                {
                    //save error here
                    mRetVal.error = true;
                    mCommon.logError("index.aspx.cs", "viewHistory", mQuery, Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }
                finally
                {
                    aConn.Close();
                }
            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }


        /**
         * GetMaintInfo
         * 
         * returns a string says who last updated the give id and when.
         **/
        public string GetMaintInfo(WSDBConnection theConn, string theID)
        {
            string mRetVal = "";
            OracleDataReader mDR = null;
            try
            {
                string mCommand = string.Format(@"
											SELECT maint_user_id
												  ,maint_date
											  FROM cct_trans
											 WHERE id = '{0}'
											"
                                                , theID
                                                );

                mDR = theConn.GetReader(mCommand);
                if (mDR.Read())
                {
                    mRetVal = string.Format(@"ID: {0} was not updated because {1} updated the request at {2}.  Please try again.<br>"
                                        , theID
                                        , mDR["maint_user_id"]
                                        , mDR["maint_date"]
                                    );
                }
                else
                {
                    mRetVal = string.Format(@"ID: {0} was not updated because the row was not found."
                                        , theID
                                    );
                }

            }
            catch (Exception e)
            {
                mRetVal = "";
                Common mCommon = new Common();
                mCommon.logError("index.aspx.cs", "GetMaintInfo", "", Session["gNTName"].ToString(), e.ToString(), "ERROR");
            }
            finally
            {
                if (mDR != null)
                {
                    mDR.Close();
                    mDR.Dispose();
                }
            }
            return mRetVal;
        }

        #endregion

        #region trans functions

        /**
		 * getTrans
		 * 
		 * gets the Transactions from App_D/Common.cs
		 * first checks user to be loged on
		 * if not loged on, refreshes the session
		 **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct getTrans()
        {
            if (CheckLogin())
            {
                Common common = new Common();
                return common.getTransData(this, "ALL", true);
            }
            else
            {
                return gRefresh;
            }
        }

        /**
         * updateUSTrans
         * 
         * First checks to see if user is loged in, if not 
         * returns REFRESH to reload the page
         * 
         * gets all arrays of transactions to be update
         * makes sure session is still good
         * walks through each row sent back
         * first checks to make sure row isn't null (.NET adds in a null row for any that were skipped when passed in
         *											 ex: only update row 3 then rows 0-2 will be null in the array)
         * if the row is in cct_trans updates the values changed for that row (updates status if check box is checked)
         * else if the row is checked insert the new row into cct_trans
         * if an email is set always update the cct_order_info with the new value
         *
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct updateUSTrans(Object[] theCK, Object[] theEmail, Object[] theStatus, Object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                Common mCommon = new Common();
                WSDBConnection aConn = new WSDBConnection();
                WebResponse mResponse = null;
                XmlTextReader mXML = null;
                OracleDataReader mDR = null;
                string mCommand = "";
                try
                {
                    aConn.Open("SISR", 0);
                    WebRequest mReq;
                    WSProcedure aProc;
                    string mPath;
                    //string mFailedEmail = "";
                    string mTransEmailBody;
                    string mSummaryEmailBody = "";
                    int mTotalProcess = 0;
                    int mTotalSuccess = 0;
                    string amountTax;
                    DataView mUSTransDV = (DataView)Session["gUSTransDV"];
                    string mTempMessage = "";

                    #region update all information changed on page, and set trans marked to send to processing

                    for (int i = 0; i < mUSTransDV.Count; i++)
                    {
                        if (i < theCK.Length)
                        {
                            if (theCK[i] != null)
                            {
                                mUSTransDV[i]["checked"] = (theCK[i].ToString() == "true") ? "checked" : "";
                                mUSTransDV[i]["update"] = "T";
                            }
                            if (theEmail[i] != null) { mUSTransDV[i]["email"] = theEmail[i]; mUSTransDV[i]["update"] = "T"; }
                            if (theStatus[i] != null) { mUSTransDV[i]["status_id"] = theStatus[i]; mUSTransDV[i]["update"] = "T"; }
                            if (theNotes[i] != null)
                            {
                                mUSTransDV[i]["update"] = "T";
                                mUSTransDV[i]["notes"] = (theNotes[i].ToString().Length > 39999) ? theNotes[i].ToString().Substring(0, 3999) : theNotes[i].ToString();
                            }

                        }

                        if (mUSTransDV[i]["update"].ToString() == "T")
                        {
                            #region update row in cct_trans

                            if (mUSTransDV[i]["checked"].ToString() == "checked")
                            {
                                mUSTransDV[i]["status_id"] = ConfigurationManager.AppSettings["STATUS_PROCESSING"];
                            }

                            mCommand = string.Format(@"
													UPDATE cct_trans
													   SET status_id = {0}
														  ,email = '{1}'
														  ,notes = '{4}'
														  ,maint_user_id = '{2}'
														  ,maint_date = sysdate
													 WHERE id = '{3}'
													   AND maint_date = to_date('{5}','mm/dd/yy hh:mi:ss am')
													", mUSTransDV[i]["status_id"].ToString()
                                 , mUSTransDV[i]["email"].ToString()
                                 , Session["gNTName"].ToString()
                                 , mUSTransDV[i]["id"]
                                 , WSString.FilterSQL(mUSTransDV[i]["notes"].ToString())
                                 , mUSTransDV[i]["maint_date"].ToString()
                                 );
                            #endregion

                            if (aConn.ExecuteCommand(mCommand) == 0)
                            {
                                #region write message since row didn't update
                                //uncheck so cc isn't processed below
                                mUSTransDV[i]["checked"] = "";

                                mTempMessage += GetMaintInfo(aConn, mUSTransDV[i]["id"].ToString());
                                #endregion
                            }
                            else
                            {
                                #region update f03b11 set rppst to M if status deleted or declined deleted

                                if (mUSTransDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_DELETED"].ToString() ||
                                    mUSTransDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"].ToString())
                                {


                                    mCommand = string.Format(@"
															UPDATE F03B11
															SET    rppst = 'M'
																  ,rpddnj = jde_pkg.pkg_jdeu.fn_jdate(SYSDATE)
																  ,rpuser = '{3}'
																  ,rpupmj = pkg_jdeu.fn_jdate()
																  ,rpupmt = pkg_jdeu.fn_jtime()
																  ,rppid = 'CCT'
																  ,rpjobn = 'CCT'
															WHERE  rpdct = '{0}'
															AND    rpdoc = {1}
															AND    rpkco = '{2}'
															AND    rppst != 'P'
														"
                                                , mUSTransDV[i]["dct"].ToString()
                                                , mUSTransDV[i]["doc"].ToString()
                                                , mUSTransDV[i]["kco"].ToString()
                                                , Session["gNTName"].ToString()
                                                );
                                    aConn.ExecuteCommand(mCommand);
                                }

                                #endregion

                                #region update email in cct_order_info

                                //only update the order/email on a recurring order
                                if (mUSTransDV[i]["type_id"].ToString() == ConfigurationManager.AppSettings["TYPE_RECURRING"])
                                {
                                    aProc = aConn.InitProcedure("pkg_cct.upsert_order_info");
                                    aProc.AddParam("pram_order", OracleDbType.Integer, ParameterDirection.Input, mUSTransDV[i]["order_skey"]);
                                    aProc.AddParam("pram_email", OracleDbType.VarChar, ParameterDirection.Input, mUSTransDV[i]["email"].ToString());
                                    aProc.AddParam("pram_nt_user_id", OracleDbType.VarChar, ParameterDirection.Input, Session["gNTName"].ToString());
                                    aProc.ExecuteNonQuery();
                                }

                                #endregion
                            }
                        }
                    }

                    #endregion

                    #region use cybersource to process trans

                    for (int i = 0; i < mUSTransDV.Count; i++)
                    {
                        if (mUSTransDV[i]["checked"].ToString() == "checked")
                        {
                            if (mUSTransDV[i]["exp"].ToString().Length > 3)
                            {
                                #region find tax amount
                                amountTax = "0";
                                if (mUSTransDV[i]["amount"].ToString() == mUSTransDV[i]["openamount"].ToString() && mUSTransDV[i]["openamount"].ToString() == mUSTransDV[i]["gross"].ToString())
                                {
                                    if (mUSTransDV[i]["rpar08"].ToString() == "GRP")
                                    {
                                        mCommand = string.Format(@"	SELECT nvl(SUM(decode(gl.globj
																						 ,'221101'
																						 ,(gl.glaa / 100) * -1
																						 ,0))
																			  ,0) AS domestic_tax
																	  FROM f0911 gl
																	 WHERE (gl.gldoc, gl.glkco) IN
																		   (SELECT rpdoc
																				  ,rpkco
																			  FROM f03b11
																			 WHERE rpvinv = '{0}'
																			   AND rpsfx  = '001')
																	   AND gl.gllt = 'AA'
																	   AND gl.gldct = 'AE'
																	   AND gl.glextl <> 'AM'
																	   AND gl.globj = '221101'
																	"
                                                                   , mUSTransDV[i]["rpvinv"]
                                                                   );
                                    }
                                    else
                                    {
                                        mCommand = string.Format(@"	SELECT nvl(SUM(decode(gl.globj
																						 ,'221101'
																						 ,(gl.glaa / 100) * -1
																						 ,0))
																			  ,0) AS domestic_tax
																	  FROM f0911 gl
																	 WHERE gl.gldoc = {0}
																	   AND gl.glkco = {1}
																	   AND gl.gllt = 'AA'
																	   AND gl.gldct = 'AE'
																	   AND gl.glextl <> 'AM'
																	   AND gl.globj = '221101'
																	"
                                                                   , mUSTransDV[i]["doc"]
                                                                   , mUSTransDV[i]["kco"]
                                                                   );
                                    }
                                    mDR = aConn.GetReader(mCommand);
                                    if (mDR.Read())
                                    {
                                        amountTax = mDR["domestic_tax"].ToString();
                                    }
                                }
                                #endregion

                                #region process req in cybersource

                                mPath = string.Format(@"{0}/CyberSourceService.aspx?credit=false&transID={1}&ccAcctNum={2}&ccExpMonth={3}&ccExpYear={4}&fName={7}&lName={8}&addr1={9}&addr2={10}&city={11}&state={12}&zip={13}&country={14}&email={15}&amount={5}&invoiceNum={6}&company={16}&phone={17}&amountTax={18}&userPO={19}"
                                                      , ConfigurationManager.AppSettings["CYBERSOURCE_PATH"]
                                                      , mUSTransDV[i]["id"]
                                                      , mUSTransDV[i]["credit_card_no"].ToString() + mUSTransDV[i]["credit_card_suffix"].ToString()
                                                      , mUSTransDV[i]["exp"].ToString().Substring(0, 2)
                                                      , "20" + mUSTransDV[i]["exp"].ToString().Substring(3)
                                                      , mUSTransDV[i]["amount"]
                                                      , mUSTransDV[i]["doc"]
                                                      , Uri.EscapeDataString(mUSTransDV[i]["card_first_name"].ToString())
                                                      , Uri.EscapeDataString(mUSTransDV[i]["card_last_name"].ToString())
                                                      , Uri.EscapeDataString(mUSTransDV[i]["card_street"].ToString())
                                                      , ""
                                                      , Uri.EscapeDataString(mUSTransDV[i]["card_city"].ToString())
                                                      , mUSTransDV[i]["card_state"].ToString()
                                                      , Uri.EscapeDataString(mUSTransDV[i]["card_zip"].ToString())
                                                      , Uri.EscapeDataString(mUSTransDV[i]["card_country"].ToString())
                                                      , Uri.EscapeDataString(mUSTransDV[i]["email"].ToString())
                                                      , Uri.EscapeDataString(mUSTransDV[i]["company"].ToString())
                                                      , Uri.EscapeDataString(mUSTransDV[i]["card_phone"].ToString())
                                                      , amountTax
                                                      , Uri.EscapeDataString(mUSTransDV[i]["userPO"].ToString())
                                                      );

                                // Initialize the WebRequest.
                                mReq = WebRequest.Create(mPath);
                                //mReq.Timeout = System.Threading.Timeout.Infinite;
                                mReq.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TIMEOUT"]);
                                try
                                {
                                    mResponse = mReq.GetResponse();
                                    mXML = new XmlTextReader(mResponse.GetResponseStream());
                                }
                                catch (Exception e)
                                {
                                    mCommon.logError("index.aspx.cs", "updateUSTrans-Response", mPath, Session["gNTName"].ToString(), e.ToString(), "ERROR");
                                }


                                string mTransID = "";
                                string mRequestID = "";
                                string mRequestToken = "";
                                string mResultCode = "";
                                string mStatus = "";
                                string mStatusID = ConfigurationManager.AppSettings["STATUS_FAILED"];
                                string mPST = "";

                                while (mXML.Read())
                                {
                                    if (mXML.NodeType == XmlNodeType.Element)
                                    {
                                        switch (mXML.Name)
                                        {
                                            case "status":
                                                mStatus = mXML.ReadString();
                                                switch (mStatus)
                                                {
                                                    case "SUCCESS":
                                                        mPST = "J";
                                                        mStatusID = ConfigurationManager.AppSettings["STATUS_COMPLETED"];
                                                        mTotalSuccess++;
                                                        break;
                                                    case "DECLINED":
                                                        mPST = "K";
                                                        mStatusID = ConfigurationManager.AppSettings["STATUS_DECLINED"];
                                                        //mFailedEmail = (mUSTransDV[i]["type_id"].ToString() == ConfigurationManager.AppSettings["TYPE_ONE_TIME"]) ? mUSTransDV[i]["create_user_id"].ToString() + "@willscot.com" : "";
                                                        break;
                                                    case "EXPIRED":
                                                        mPST = "L";
                                                        mStatusID = ConfigurationManager.AppSettings["STATUS_DECLINED"];
                                                        //mFailedEmail = (mUSTransDV[i]["type_id"].ToString() == ConfigurationManager.AppSettings["TYPE_ONE_TIME"]) ? mUSTransDV[i]["create_user_id"].ToString() + "@willscot.com" : "";
                                                        break;
                                                    case "FAILURE":
                                                        mStatusID = ConfigurationManager.AppSettings["STATUS_FAILED"];
                                                        break;
                                                }
                                                break;
                                            case "transID":
                                                mTransID = mXML.ReadString();
                                                break;
                                            case "requestID":
                                                mRequestID = mXML.ReadString();
                                                break;
                                            case "requestToken":
                                                mRequestToken = mXML.ReadString();
                                                break;
                                            case "resultCode":
                                                mResultCode = mXML.ReadString();
                                                break;
                                            case "error":
                                                mStatus = "FAILURE";
                                                mStatusID = ConfigurationManager.AppSettings["STATUS_FAILED"];
                                                break;
                                            case "message":
                                                mUSTransDV[i]["notes"] += "<br>" + mXML.ReadString();
                                                break;
                                            case "innerException":
                                                mUSTransDV[i]["notes"] += "<br>" + mXML.ReadString();
                                                break;
                                        }
                                    }
                                }

                                //mResponse.Close();
                                //mXML.Close();

                                #endregion

                                mTotalProcess++;
                                if (mStatusID != ConfigurationManager.AppSettings["STATUS_FAILED"].ToString())
                                {
                                    #region update f03b11

                                    mCommand = string.Format(@"
															UPDATE F03B11
															SET    rppst = '{3}'
																  ,rpddnj = jde_pkg.pkg_jdeu.fn_jdate(SYSDATE)
																  ,rpuser = '{4}'
																  ,rpupmj = pkg_jdeu.fn_jdate()
																  ,rpupmt = pkg_jdeu.fn_jtime()
																  ,rppid = 'CCT'
																  ,rpjobn = 'CCT'
															WHERE  rpdct = '{0}'
															AND    rpdoc = {1}
															AND    rpkco = '{2}'
															AND	   rppst != 'P'
														"
                                                , mUSTransDV[i]["dct"].ToString()
                                                , mUSTransDV[i]["doc"].ToString()
                                                , mUSTransDV[i]["kco"].ToString()
                                                , mPST
                                                , Session["gNTName"].ToString()
                                                );
                                    aConn.ExecuteCommand(mCommand);

                                    #endregion
                                }

                                #region update cct

                                mCommand = string.Format(@"
								UPDATE cct_trans
								   SET status_id = {0}
									  ,maint_user_id = '{1}'
									  ,maint_date = sysdate
									  ,notes = '{3}'
									  ,cyber_id = '{4}'
									  ,cyber_token = '{7}'
									  ,result_code = '{5}'
									  {6}
								 WHERE id = '{2}'
								", mStatusID
                                 , Session["gNTName"].ToString()
                                 , mUSTransDV[i]["id"].ToString()
                                 , WSString.FilterSQL(mUSTransDV[i]["notes"].ToString())
                                 , mRequestID
                                 , mResultCode
                                 , (mStatusID == ConfigurationManager.AppSettings["STATUS_COMPLETED"].ToString()) ? ",amount = 100*" + mUSTransDV[i]["amount"] : ""
                                 , mRequestToken
                                 );
                                aConn.ExecuteCommand(mCommand);

                                #endregion

                                #region send emails

                                //<td><a href='http://{2}/dn/cct/?id={0}'>{0}</a></td>

                                mTransEmailBody = string.Format(@"<tr>
																<td></td>
																<td>{1}</td>
																<td>{3}</td>
																<td>{4}</td>
																<td>{5}</td>
																<td style='text-align:right;'>{6}</td>
															  </tr>"
                                                                 , mUSTransDV[i]["id"]
                                                                 , mStatus
                                                                 , Session["gServer"]
                                                                 , mUSTransDV[i]["doc"]
                                                                 , mUSTransDV[i]["rpodoc"]
                                                                 , mUSTransDV[i]["cs"]
                                                                 , String.Format("{0:c}", mUSTransDV[i]["amount"]));

                                mSummaryEmailBody += mTransEmailBody;
                                //send emails
                                //email cash list of trans processed
                                //cc email connected to trans
                                //email requester if trans is rejected
                                if (mUSTransDV[i]["email"].ToString() != "")
                                {
                                    mCommon.emailWrapper(mUSTransDV[i]["email"].ToString(), ""
                                                        , string.Format(@"Merchant Email Receipt - {0}", mStatus)
                                                        , string.Format(@"
																		<tr>
																			<td id='captionTd'>
																				Credit Card Authorization - US - {5}
																			</td>
																		</tr>
																	
																		<tr>
																			<td><hr size='1' noshade></td>
																		</tr>
																		<tr><td>
																						========= GENERAL INFORMATION =========<br>
					<br>
																						Merchant : Williams Scotsman, Inc.<br>
																						Date/Time : {0}<br>
					<br>
																						========= ORDER INFORMATION =========<br>
																						Invoice : {1}<br>
																						Description : {2}<br>
																						Amount : {3} (USD)<br>
																						Credit Card : ****-****-****-{4}<br>
																						Type : Authorization and Capture<br>
					<br>
																						============== RESULTS ==============<br>
																						Response : {5}<br>
																						Transaction ID : {6}<br>
																						Transaction Token : {18}<br>
					<br>
																						==== CUSTOMER CONTACT INFORMATION ===<br>
																						Customer ID : {7}<br>
																						First Name : {8}<br>
																						Last Name : {9}<br>
																						Company : {10}<br>
																						Address : {11}<br>
																						City : {12}<br>
																						State/Province : {13}<br>
																						Zip/Postal Code : {14}<br>
																						Country : {15}<br>
																						Phone : {16}<br>
																						E-Mail : {17}<br>
																						</td></tr>
																						"
                                                                        , DateTime.Now.ToString()
                                                                        , mUSTransDV[i]["doc"]
                                                                        , mUSTransDV[i]["remark"]
                                                                        , String.Format("{0:c}", mUSTransDV[i]["amount"])
                                                                        , mUSTransDV[i]["credit_card_suffix"]
                                                                        , "<font style='font-weight:bold;" + (mStatus == "SUCCESS" ? "" : "color:red;") + "'>" + mStatus + "</font>"
                                                                        , mRequestID
                                                                        , mUSTransDV[i]["cb"]
                                                                        , mUSTransDV[i]["card_first_name"]
                                                                        , mUSTransDV[i]["card_last_name"]
                                                                        , mUSTransDV[i]["company"]
                                                                        , mUSTransDV[i]["card_street"]
                                                                        , mUSTransDV[i]["card_city"]
                                                                        , mUSTransDV[i]["card_state"]
                                                                        , mUSTransDV[i]["card_zip"]
                                                                        , mUSTransDV[i]["card_country"]
                                                                        , mUSTransDV[i]["card_phone"]
                                                                        , mUSTransDV[i]["email"]
                                                                        , mRequestToken
                                                                        )
                                                        , this
                                                        , "User Transaction Email", mUSTransDV[i]["HIFlag"].ToString() == "H");
                                }
                                #endregion

                            }
                            else
                            {
                                #region write error because of no cc # in omb
                                #region update cct

                                mCommand = string.Format(@"
								UPDATE cct_trans
								   SET status_id = {3}
									  ,maint_user_id = '{0}'
									  ,maint_date = sysdate
									  ,notes = '{2}
Transaction could not be processed because credit card does not exist in OMB.'
								 WHERE id = '{1}'
								", Session["gNTName"].ToString()
                                 , mUSTransDV[i]["id"].ToString()
                                 , WSString.FilterSQL(mUSTransDV[i]["notes"].ToString())
                                 , ConfigurationManager.AppSettings["STATUS_FAILED"]
                                 );
                                aConn.ExecuteCommand(mCommand);

                                #endregion

                                mTotalProcess++;

                                mTransEmailBody = string.Format(@"<tr>
																<td></td>
																<td>Transaction could not be processed because credit card does not exist in OMB.</td>
																<td>{2}</td>
																<td>{3}</td>
																<td>{4}</td>
																<td style='text-align:right;'>{5}</td>
															  </tr>"
                                                                 , mUSTransDV[i]["id"]
                                                                 , Session["gServer"]
                                                                 , mUSTransDV[i]["doc"]
                                                                 , mUSTransDV[i]["rpodoc"]
                                                                 , mUSTransDV[i]["cs"]
                                                                 , String.Format("{0:c}", mUSTransDV[i]["amount"]));

                                mSummaryEmailBody += mTransEmailBody;
                                #endregion
                            }
                        }
                    }

                    #endregion

                    #region send email

                    if (mTotalProcess > 0)
                    {
                        mCommon.emailWrapper(ConfigurationManager.AppSettings["CASH_EMAIL"]
                                    , ""
                                    , string.Format(@"CCT Credit Transaction Summary")
                                    , string.Format(@"<tr><td>Total Trans Processed:{0}</td></tr>
													                      <tr><td>Total Success:{1}</td></tr><tr><td><br></td></tr>
																		  <tr>
																			<td>
																				<table border=1 cellspacing=0 cellpadding=3>
																				  <tr>
																					  <td style='font-weight:bold;white-space:nowrap;'>Transaction</td>
																					  <td style='font-weight:bold;white-space:nowrap;'>Status</td>
																					  <td style='font-weight:bold;white-space:nowrap;'>Invoice Doc #</td>
																					  <td style='font-weight:bold;white-space:nowrap;'>ODOC #</td>
																					  <td style='font-weight:bold;white-space:nowrap;'>CS #</td>
																					  <td style='font-weight:bold;white-space:nowrap;'>Amount</td>
																				  </tr>
																				  {2}
																		  </table>
																		"
                                                    , mTotalProcess
                                                    , mTotalSuccess
                                                    , mSummaryEmailBody)
                                    , this
                                    , "Cash Summary Email");
                    }

                    #endregion

                    mRetVal = mCommon.getTransData(this, "US", true);
                    mRetVal.message = string.Format("{0}Update Complete", mTempMessage);
                }
                catch (Exception e)
                {
                    //save error here
                    //could send custom error to display on screen
                    mRetVal.error = true;

                    mCommon.logError("index.aspx.cs", "updateUSTrans", mCommand, Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }
                finally
                {
                    aConn.Close();
                    if (mResponse != null) { mResponse.Close(); }
                    if (mXML != null) { mXML.Close(); }
                    if (mDR != null)
                    {
                        mDR.Close();
                        mDR.Dispose();
                    }
                }
            }
            else
            {
                mRetVal = gRefresh;
            }
            return mRetVal;
        }


        /**
         * sortTransUS
         * 
         * saves the current info to the dv and sorts the row
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct sortTransUS(string theCol, object[] theCK, object[] theEmail, object[] theStatus, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                DataView mDV = (DataView)Session["gUSTransDV"];

                #region update dv

                for (int i = 0; i < theCK.Length; i++)
                {
                    if (theCK[i] != null)
                    {
                        mDV[i]["checked"] = (theCK[i].ToString() == "true") ? "checked" : "";
                        mDV[i]["update"] = "T";
                    }
                    if (theEmail[i] != null) { mDV[i]["email"] = theEmail[i]; mDV[i]["update"] = "T"; }
                    if (theStatus[i] != null) { mDV[i]["status_id"] = theStatus[i]; mDV[i]["update"] = "T"; }
                    if (theNotes[i] != null) { mDV[i]["notes"] = theNotes[i]; mDV[i]["update"] = "T"; }
                }

                #endregion


                if (mDV.Sort.ToString() == theCol)
                {
                    mDV.Sort = theCol + " desc";
                }
                else
                {
                    mDV.Sort = theCol;
                }

                Common mCommon = new Common();
                mRetVal = mCommon.getTransData(this, "US", false);

            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }

        /**
         * updateCATrans
         * 
         * first checks to see if user is loged in, if not refreshes the page
         * creates a text file named CATrans_Month-Day-Year_Hour;Min;Sec.txt
         * this file holds all the trans to be processed
         * also updates the database making any changes passed over and setting
         * all trans to be processed to completed.
         *
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct updateCATrans(object[] theCK, object[] theEmail, object[] theStatus, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                String mCommand = "";
                StreamWriter mSW = null;
                WSDBConnection aConn = new WSDBConnection();
                Common mCommon = new Common();


                try
                {
                    aConn.Open("SISR", 0);
                    WSProcedure aProc;
                    DataView mCATransDV = (DataView)Session["gCATransDV"];
                    DateTime mDT = DateTime.Now;
                    bool createFile = false;
                    string fileBody;
                    string mTempMessage = "";

                    #region file head

                    fileBody = @"<html><head><style>
									td.cell
									{
										border-right:	1px solid #d6d6d6;
										border-bottom:	1px solid #d6d6d6;
										empty-cells:show;
									}
									td.leftCell
									{
										border-right:	1px solid #d6d6d6;
										border-bottom:	1px solid #d6d6d6;
										border-left:	1px solid #d6d6d6;
										empty-cells:show;
									}

									 
									th.headLeft
									{
										text-align:			center;
										font:				bold 12px Verdana;
										color:				#000000;
										background-color: #f3f3f3;
										border-right:	1px solid #5c5c5c;
										border-bottom:	1px solid #5c5c5c;
										border-top:	1px solid #5c5c5c;
										border-left:	1px solid #5c5c5c;
										white-space:nowrap;
									}
									th.head
									{
										text-align:			center;
										font:				bold 12px Verdana;
										color:				#000000;
										background-color: #f3f3f3;
										border-right:	1px solid #5c5c5c;
										border-bottom:	1px solid #5c5c5c;
										border-top:	1px solid #5c5c5c;
										white-space:nowrap;
									}
									</style></head><body>
									<table cellpadding='3' cellspacing='0'><thead><tr><th class='headLeft'>ID</th><th class='head'>DCT</th><th class='head'>Doc</th><th class='head'>Inv Date</th><th class='head'>Company</th><th class='head'>Gross Amount</th><th class='head'>Open Amount</th><th class='head'>Amount</th><th class='head'>Credit Card No</th><th class='head'>Exp Date</th><th class='head'>Type</th><th class='head'>Notes</th></tr></thead>";
                    #endregion

                    for (int i = 0; i < mCATransDV.Count; i++)
                    {
                        if (i < theCK.Length)
                        {
                            if (theEmail[i] != null) { mCATransDV[i]["email"] = theEmail[i]; mCATransDV[i]["update"] = "T"; }
                            if (theStatus[i] != null) { mCATransDV[i]["status_id"] = theStatus[i]; mCATransDV[i]["update"] = "T"; }
                            if (theNotes[i] != null) { mCATransDV[i]["notes"] = theNotes[i]; mCATransDV[i]["update"] = "T"; }
                            if (theCK[i] != null)
                            {
                                if (theCK[i].ToString() == "true")
                                {
                                    mCATransDV[i]["checked"] = "checked";
                                    mCATransDV[i]["status_id"] = ConfigurationManager.AppSettings["STATUS_COMPLETED"];
                                }
                                else
                                {
                                    mCATransDV[i]["checked"] = "";
                                }
                                mCATransDV[i]["update"] = "T";
                            }
                        }

                        if (mCATransDV[i]["update"].ToString() == "T")
                        {
                            //if row is checked set the status to processing, make it uneditable and set it to be removed from the DV
                            //else either status is null or they are deleting the row

                            #region update row in cct_trans

                            mCommand = string.Format(@"
								UPDATE cct_trans
								   SET status_id = {0}
									  ,email = '{1}'
									  ,notes = '{4}'
									  ,amount = {5}
									  ,maint_user_id = '{2}'
									  ,maint_date = sysdate
								 WHERE id = '{3}'
								   AND maint_date = to_date('{6}','mm/dd/yy hh:mi:ss am')
								", mCATransDV[i]["status_id"]
                                 , mCATransDV[i]["email"]
                                 , Session["gNTName"]
                                 , mCATransDV[i]["id"]
                                 , WSString.FilterSQL(mCATransDV[i]["notes"].ToString())
                                 , Convert.ToDouble(mCATransDV[i]["amount"]) * 100
                                 , mCATransDV[i]["maint_date"]
                                 );
                            #endregion

                            if (aConn.ExecuteCommand(mCommand) == 0)
                            {
                                #region write message since row didn't update
                                //uncheck so cc isn't processed below
                                mCATransDV[i]["checked"] = "";
                                mTempMessage += GetMaintInfo(aConn, mCATransDV[i]["id"].ToString());
                                #endregion
                            }
                            else
                            {
                                if (mCATransDV[i]["checked"].ToString() == "checked")
                                {

                                    createFile = true;
                                    fileBody += string.Format(@"<tr><td class='leftCell'>{0}&nbsp;</td><td class='cell'>{1}&nbsp;</td><td class='cell'>{2}&nbsp;</td><td class='cell'>{3}&nbsp;</td><td class='cell'>{4}&nbsp;</td><td class='cell'>{5}&nbsp;</td><td class='cell'>{6}&nbsp;</td><td class='cell'>{7}&nbsp;</td><td class='cell'>{8}&nbsp;</td><td class='cell'>{9}&nbsp;</td><td class='cell'>{10}&nbsp;</td><td class='cell'>{11}&nbsp;</td></tr>"
                                                                 , mCATransDV[i]["id"].ToString().PadRight(7, ' ')
                                                                 , mCATransDV[i]["dct"].ToString().PadRight(3, ' ')
                                                                 , mCATransDV[i]["doc"].ToString().PadRight(20, ' ')
                                                                 , mCATransDV[i]["invoice_date"].ToString().PadRight(8, ' ')
                                                                 , mCATransDV[i]["company"].ToString().PadRight(40, ' ')
                                                                 , mCATransDV[i]["gross"].ToString().PadRight(12, ' ')
                                                                 , mCATransDV[i]["openamount"].ToString().PadRight(12, ' ')
                                                                 , mCATransDV[i]["amount"].ToString().PadRight(12, ' ')
                                                                 , mCATransDV[i]["credit_card_no"].ToString().PadRight(12, ' ') + mCATransDV[i]["credit_card_suffix"].ToString()
                                                                 , mCATransDV[i]["exp"].ToString().PadRight(8, ' ')
                                                                 , mCATransDV[i]["type_name"].ToString().PadRight(9, ' ')
                                                                 , mCATransDV[i]["notes"].ToString());
                                }


                                #region update email in cct_order_info

                                //only update the order/email on a recurring order
                                if (mCATransDV[i]["type_id"].ToString() == ConfigurationManager.AppSettings["TYPE_RECURRING"])
                                {
                                    aProc = aConn.InitProcedure("pkg_cct.upsert_order_info");
                                    aProc.AddParam("pram_order", OracleDbType.Integer, ParameterDirection.Input, mCATransDV[i]["order_skey"]);
                                    aProc.AddParam("pram_email", OracleDbType.VarChar, ParameterDirection.Input, mCATransDV[i]["email"].ToString());
                                    aProc.AddParam("pram_nt_user_id", OracleDbType.VarChar, ParameterDirection.Input, Session["gNTName"].ToString());
                                    aProc.ExecuteNonQuery();
                                }

                                #endregion

                                if (mCATransDV[i]["checked"].ToString() == "checked")
                                {
                                    #region update f03b11

                                    mCommand = string.Format(@"
															UPDATE F03B11
															SET    rppst = 'J'
																  ,rpryin = '#'
																  ,rpddnj = jde_pkg.pkg_jdeu.fn_jdate(SYSDATE)
																  ,rpuser = '{3}'
																  ,rpupmj = pkg_jdeu.fn_jdate()
																  ,rpupmt = pkg_jdeu.fn_jtime()
																  ,rppid = 'CCT'
																  ,rpjobn = 'CCT'
															WHERE  rpdct = '{0}'
															AND    rpdoc = {1}
															AND    rpkco = '{2}'
															AND	   rppst != 'P'
														"
                                                , mCATransDV[i]["dct"].ToString()
                                                , mCATransDV[i]["doc"].ToString()
                                                , mCATransDV[i]["kco"].ToString()
                                                , Session["gNTName"].ToString()
                                                );
                                    aConn.ExecuteCommand(mCommand);

                                    #endregion
                                }
                            }

                        }
                    }
                    fileBody += "</table></body></html>";

                    mRetVal = mCommon.getTransData(this, "CA", true);
                    mRetVal.message = string.Format("{0}Update Complete", mTempMessage);

                    if (createFile)
                    {
                        mRetVal.filePath = ConfigurationManager.AppSettings["FILE_LOC"] + "//CATrans_" + mDT.Month.ToString() + "-" + mDT.Day.ToString() +
                                    "-" + mDT.Year.ToString() + "_" +
                                   mDT.Hour.ToString().PadLeft(2, '0') + "_" + mDT.Minute.ToString().PadLeft(2, '0') + "_" +
                                   mDT.Second.ToString().PadLeft(2, '0') + ".html";
                        mSW = new StreamWriter(mRetVal.filePath);
                        mSW.WriteLine(fileBody);
                    }
                }
                catch (Exception e)
                {
                    //save error here
                    mRetVal.error = true;
                    mCommon.logError("index.aspx.cs", "updateCATrans", mCommand, Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }
                finally
                {
                    if (mSW != null) { mSW.Close(); }
                    aConn.Close();
                }
            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }


        /**
         * sortTransCA
         * 
         * saves the current info to the dv and sorts the row
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct sortTransCA(string theCol, object[] theCK, object[] theEmail, object[] theStatus, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                DataView mDV = (DataView)Session["gCATransDV"];

                #region update dv

                for (int i = 0; i < theCK.Length; i++)
                {
                    if (theCK[i] != null)
                    {
                        mDV[i]["checked"] = (theCK[i].ToString() == "true") ? "checked" : "";
                        mDV[i]["update"] = "T";
                    }
                    if (theEmail[i] != null) { mDV[i]["email"] = theEmail[i]; mDV[i]["update"] = "T"; }
                    if (theStatus[i] != null) { mDV[i]["status_id"] = theStatus[i]; mDV[i]["update"] = "T"; }
                    if (theNotes[i] != null) { mDV[i]["notes"] = theNotes[i]; mDV[i]["update"] = "T"; }
                }

                #endregion


                if (mDV.Sort.ToString() == theCol)
                {
                    mDV.Sort = theCol + " desc";
                }
                else
                {
                    mDV.Sort = theCol;
                }

                Common mCommon = new Common();
                mRetVal = mCommon.getTransData(this, "CA", false);

            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }

        #endregion

        #region search functions

        /**
		 * search
		 * 
		 * checks to see if user is loged in, if not refreshes the page
		 * otherwise sets search parms on the session and returns results
		 **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct search(string theTransID, string theCustNum, string theCustName, string theCSNum, string theODOC,
                               string theOrder, string theDCT, string theDOC, string theKCO, string theCCSuffix, string theSentFrom,
                               string theSentTo, string theStatus, string theType, string theCardType)
        {
            if (CheckLogin())
            {
                Session["gSearchTransID"] = theTransID;
                Session["gSearchCustNum"] = theCustNum;
                Session["gSearchCustName"] = theCustName;
                Session["gSearchCSNum"] = theCSNum;
                Session["gSearchODOC"] = theODOC;
                Session["gSearchOrder"] = theOrder;
                Session["gSearchDCT"] = theDCT;
                Session["gSearchDOC"] = theDOC;
                Session["gSearchKCO"] = theKCO;
                Session["gSearchCCSufix"] = theCCSuffix;
                Session["gSearchSentTo"] = theSentTo;
                Session["gSearchSentFrom"] = theSentFrom;
                Session["gSearchStatus"] = theStatus;
                Session["gSearchType"] = theType;
                Session["gSearchCardType"] = theCardType;
                Session["SearchFlag"] = true;
                Common common = new Common();

                return common.search(this, "ALL", true);
            }
            else
            {
                return gRefresh;
            }
        }

        /**
         * searchClear
         * 
         * clears the session of search values
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void searchClear()
        {
            Session["gSearchTransID"] = "";
            Session["gSearchCustNum"] = "";
            Session["gSearchCustName"] = "";
            Session["gSearchCSNum"] = "";
            Session["gSearchODOC"] = "";
            Session["gSearchOrder"] = "";
            Session["gSearchDCT"] = "";
            Session["gSearchDOC"] = "";
            Session["gSearchKCO"] = "";
            Session["gSearchCCSufix"] = "";
            Session["gSearchSentTo"] = "";
            Session["gSearchSentFrom"] = "";
            Session["gSearchStatus"] = "";
            Session["gSearchType"] = "";
            Session["SearchFlag"] = false;
        }

        /**
         * updateUSSearch
         * 
         * updates the US Search Table
         * first checks for log on, if user isn't loged on refreshed the page
         * 
         * only deleted and declined_deleted statues can be upaded
         * also updates the notes
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct updateUSSearch(object[] theStatus, object[] theEmail, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                WSDBConnection aConn = new WSDBConnection();
                Common mCommon = new Common();
                string mCommand = "";
                try
                {
                    aConn.Open("SISR", 0);
                    DataView mUSSearchDV = (DataView)Session["gUSSearchDV"];
                    string mTempMessage = "";
                    #region update cct

                    for (int i = 0; i < mUSSearchDV.Count; i++)
                    {
                        if (i < theStatus.Length)
                        {
                            if (theStatus[i] != null) { mUSSearchDV[i]["status_id"] = theStatus[i]; mUSSearchDV[i]["update"] = "T"; }
                            if (theNotes[i] != null) { mUSSearchDV[i]["notes"] = theNotes[i]; mUSSearchDV[i]["update"] = "T"; }
                            if (theEmail[i] != null) { mUSSearchDV[i]["email"] = theEmail[i]; mUSSearchDV[i]["update"] = "T"; }
                        }

                        if (mUSSearchDV[i]["update"].ToString() == "T")
                        {
                            mCommand = string.Format(@"
							UPDATE cct_trans
							   SET status_id = {0},
								   notes = '{2}',
								   email = '{3}'
								  ,maint_user_id = '{4}'
								  ,maint_date = sysdate
							 WHERE id = '{1}'
							   AND maint_date = to_date('{5}','mm/dd/yy hh:mi:ss am')
							", mUSSearchDV[i]["status_id"]
                             , mUSSearchDV[i]["id"]
                             , WSString.FilterSQL(mUSSearchDV[i]["notes"].ToString())
                             , mUSSearchDV[i]["email"]
                             , Session["gNTName"].ToString()
                             , mUSSearchDV[i]["maint_date"]
                             );

                            if (aConn.ExecuteCommand(mCommand) == 0)
                            {
                                //write message since row didn't update
                                mTempMessage += GetMaintInfo(aConn, mUSSearchDV[i]["id"].ToString());
                            }
                        }
                    }

                    #endregion

                    mRetVal = mCommon.search(this, "US", true);
                    mRetVal.message = string.Format("{0}Update Complete", mTempMessage);
                }
                catch (Exception e)
                {
                    //save error here
                    mRetVal.error = true;

                    mCommon.logError("index.aspx.cs", "updateUSSearch", mCommand, Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }
                finally
                {
                    aConn.Close();
                }
            }
            else
            {
                mRetVal = gRefresh;
            }
            return mRetVal;
        }


        /**
         * sortSearchUS
         * 
         * saves the current info to the dv and sorts the row
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct sortSearchUS(string theCol, object[] theEmail, object[] theStatus, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                DataView mDV = (DataView)Session["gUSSearchDV"];

                #region update dv

                for (int i = 0; i < theStatus.Length; i++)
                {
                    if (theEmail[i] != null) { mDV[i]["email"] = theEmail[i]; mDV[i]["update"] = "T"; }
                    if (theStatus[i] != null) { mDV[i]["status_id"] = theStatus[i]; mDV[i]["update"] = "T"; }
                    if (theNotes[i] != null) { mDV[i]["notes"] = theNotes[i]; mDV[i]["update"] = "T"; }
                }

                #endregion

                if (mDV.Sort.ToString() == theCol)
                {
                    mDV.Sort = theCol + " desc";
                }
                else
                {
                    mDV.Sort = theCol;
                }

                Common mCommon = new Common();
                mRetVal = mCommon.search(this, "US", false);

            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }



        /**
         * updateCASearch
         * 
         * first checks for logon, if user isn't loged on refreshes the page
         * updates statuses and notes for CA Search Trans
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct updateCASearch(object[] theStatus, object[] theEmail, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                WSDBConnection aConn = new WSDBConnection();
                Common mCommon = new Common();

                try
                {
                    aConn.Open("SISR", 0);
                    string mCommand;
                    DataView mCASearchDV = (DataView)Session["gCASearchDV"];
                    string mTempMessage = "";

                    #region update cct

                    for (int i = 0; i < mCASearchDV.Count; i++)
                    {
                        if (i < theStatus.Length)
                        {
                            if (theStatus[i] != null) { mCASearchDV[i]["status_id"] = theStatus[i]; mCASearchDV[i]["update"] = "T"; }
                            if (theNotes[i] != null) { mCASearchDV[i]["notes"] = theNotes[i]; mCASearchDV[i]["update"] = "T"; }
                            if (theEmail[i] != null) { mCASearchDV[i]["email"] = theEmail[i]; mCASearchDV[i]["update"] = "T"; }
                        }

                        if (mCASearchDV[i]["update"].ToString() == "T")
                        {
                            mCommand = string.Format(@"
							UPDATE cct_trans
							   SET status_id = {0}
								  ,notes = '{2}'
								  ,email = '{3}'
								  ,maint_user_id = '{4}'
								  ,maint_date = sysdate
							 WHERE id = '{1}'
							   AND maint_date = to_date('{5}','mm/dd/yy hh:mi:ss am')
							", mCASearchDV[i]["status_id"]
                             , mCASearchDV[i]["id"]
                             , WSString.FilterSQL(mCASearchDV[i]["notes"].ToString())
                             , mCASearchDV[i]["email"]
                             , Session["gNTName"].ToString()
                             , mCASearchDV[i]["maint_date"]
                             );

                            if (aConn.ExecuteCommand(mCommand) == 0)
                            {
                                //write message since row didn't update
                                mTempMessage += GetMaintInfo(aConn, mCASearchDV[i]["id"].ToString());
                            }
                        }
                    }

                    #endregion

                    mRetVal = mCommon.search(this, "CA", true);
                    mRetVal.message = string.Format("{0}Update Complete", mTempMessage);
                }
                catch (Exception e)
                {
                    //save error here
                    mRetVal.error = true;

                    mCommon.logError("index.aspx.cs", "updateCASearch", "", Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }
                finally
                {
                    aConn.Close();
                }
            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }


        /**
         * sortSearchCA
         * 
         * saves the current info to the dv and sorts the row
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct sortSearchCA(string theCol, object[] theEmail, object[] theStatus, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                DataView mDV = (DataView)Session["gCASearchDV"];

                #region update dv

                for (int i = 0; i < theStatus.Length; i++)
                {
                    if (theEmail[i] != null) { mDV[i]["email"] = theEmail[i]; mDV[i]["update"] = "T"; }
                    if (theStatus[i] != null) { mDV[i]["status_id"] = theStatus[i]; mDV[i]["update"] = "T"; }
                    if (theNotes[i] != null) { mDV[i]["notes"] = theNotes[i]; mDV[i]["update"] = "T"; }
                }

                #endregion


                if (mDV.Sort.ToString() == theCol)
                {
                    mDV.Sort = theCol + " desc";
                }
                else
                {
                    mDV.Sort = theCol;
                }

                Common mCommon = new Common();
                mRetVal = mCommon.search(this, "CA", false);

            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }

        #endregion

        #region credit functions

        /**
		 * credit search
		 * 
		 *
		 **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct creditSearch(string theTransID, string theCustNum, string theCustName, string theCSNum, string theODOC, string theOrder,
                               string theDCT, string theDOC, string theKCO, string theCCSuffix, string theSentFrom,
                               string theSentTo, string theType, string theCardType)
        {
            if (CheckLogin())
            {


                Session["gCreditTransID"] = theTransID;
                Session["gCreditCustNum"] = theCustNum;
                Session["gCreditCustName"] = theCustName;
                Session["gCreditCSNum"] = theCSNum;
                Session["gCreditODOC"] = theODOC;
                Session["gCreditOrder"] = theOrder;
                Session["gCreditDCT"] = theDCT;
                Session["gCreditDOC"] = theDOC;
                Session["gCreditKCO"] = theKCO;
                Session["gCreditCCSufix"] = theCCSuffix;
                Session["gCreditSentFrom"] = theSentFrom;
                Session["gCreditSentTo"] = theSentTo;
                Session["gCreditType"] = theType;
                Session["gCreditCardType"] = theCardType;
                Session["CreditSearchFlag"] = true;

                Common mCommon = new Common();
                return mCommon.creditSearch(this, "ALL", true);
            }
            else
            {
                return gRefresh;
            }
        }

        /**
         * creditClear
         * 
         * clears the session of credit values
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void creditClear()
        {
            Session["gCreditTransID"] = "";
            Session["gCreditCustNum"] = "";
            Session["gCreditCustName"] = "";
            Session["gCreditCSNum"] = "";
            Session["gCreditODOC"] = "";
            Session["gCreditOrder"] = "";
            Session["gCreditDCT"] = "";
            Session["gCreditDOC"] = "";
            Session["gCreditKCO"] = "";
            Session["gCreditCCSufix"] = "";
            Session["gCreditSentFrom"] = "";
            Session["gCreditSentTo"] = "";
            Session["gCreditType"] = "";
            Session["gCardType"] = "";
            Session["CreditSearchFlag"] = false;
        }

        /**
         * creditUSRow
         * 
         * first checks for logon, if user isn't loged on refreshes the page
         * first updates the row to be credited to processing
         * then trys to credit the row using the cybersourceservice
         * updates the status returned from the cybersourceservice and the notes passed in
         * updates f03b11 with new rppst if credit didn't fail
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct creditUSRow(int theRow, double theAmt, string theEmail, string theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                WSDBConnection aConn = new WSDBConnection();
                Common mCommon = new Common();

                WebResponse mResponse = null;
                XmlTextReader mXML = null;
                try
                {
                    aConn.Open("SISR", 0);
                    int mAmt = Convert.ToInt32(theAmt * 100);
                    DataView mUSCreditDV = (DataView)Session["gUSCreditDV"];
                    WebRequest mReq;
                    string mPath;
                    string mFailedEmail = "";
                    string mTempMessage = "";

                    #region update cct

                    string mCommand = string.Format(@"
									UPDATE cct_trans
									   SET status_id = {0}
										  ,notes = '{3}'
										  ,email = '{4}'
										  ,maint_user_id = '{2}'
										  ,maint_date = sysdate
									 WHERE id = '{1}'
									   AND maint_date = to_date('{5}','mm/dd/yy hh:mi:ss am')
								", ConfigurationManager.AppSettings["STATUS_PROCESSING"]
                                 , mUSCreditDV[theRow]["id"]
                                 , Session["gNTName"].ToString()
                                 , WSString.FilterSQL(theNotes)
                                 , theEmail
                                 , mUSCreditDV[theRow]["maint_date"]
                             );
                    #endregion

                    if (aConn.ExecuteCommand(mCommand) == 0)
                    {
                        //write message since row didn't update
                        mTempMessage += GetMaintInfo(aConn, mUSCreditDV[theRow]["id"].ToString());
                    }
                    else
                    {
                        string mStatus = "";
                        if (mUSCreditDV[theRow]["exp"].ToString().Length > 3)
                        {
                            #region process req in cybersource


                            mPath = string.Format(@"{0}/CyberSourceService.aspx?credit=true&requestID={7}&transID={1}&requestToken={19}&ccAcctNum={2}&ccExpMonth={3}&ccExpYear={4}&fName={8}&lName={9}&addr1={10}&addr2={11}&city={12}&state={13}&zip={14}&country={15}&email={16}&amount={5}&invoiceNum={6}&company={17}&phone={18}"
                                                  , ConfigurationManager.AppSettings["CYBERSOURCE_PATH"]
                                                  , mUSCreditDV[theRow]["id"]
                                                  , mUSCreditDV[theRow]["credit_card_no"].ToString() + mUSCreditDV[theRow]["credit_card_suffix"].ToString()
                                                  , mUSCreditDV[theRow]["exp"].ToString().Substring(0, 2)
                                                  , "20" + mUSCreditDV[theRow]["exp"].ToString().Substring(3)
                                                  , theAmt
                                                  , mUSCreditDV[theRow]["doc"]
                                                  , mUSCreditDV[theRow]["cyber_id"]
                                                  , Uri.EscapeDataString(mUSCreditDV[theRow]["card_first_name"].ToString())
                                                  , Uri.EscapeDataString(mUSCreditDV[theRow]["card_last_name"].ToString())
                                                  , Uri.EscapeDataString(mUSCreditDV[theRow]["card_street"].ToString())
                                                  , ""
                                                  , Uri.EscapeDataString(mUSCreditDV[theRow]["card_city"].ToString())
                                                  , mUSCreditDV[theRow]["card_state"].ToString()
                                                  , Uri.EscapeDataString(mUSCreditDV[theRow]["card_zip"].ToString())
                                                  , Uri.EscapeDataString(mUSCreditDV[theRow]["card_country"].ToString())
                                                  , Uri.EscapeDataString(mUSCreditDV[theRow]["email"].ToString())
                                                  , Uri.EscapeDataString(mUSCreditDV[theRow]["company"].ToString())
                                                  , mUSCreditDV[theRow]["card_phone"].ToString()
                                                  , mUSCreditDV[theRow]["cyber_token"]
                                                  );

                            // Initialize the WebRequest.
                            mReq = WebRequest.Create(mPath);
                            //mReq.Timeout = System.Threading.Timeout.Infinite;
                            mReq.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TIMEOUT"]);
                            mResponse = mReq.GetResponse();
                            mXML = new XmlTextReader(mResponse.GetResponseStream());



                            string mTransID = "";
                            string mRequestID = "-1";
                            string mRequestToken = "";
                            string mResultCode = "-1";
                            string mStatusID = ConfigurationManager.AppSettings["STATUS_FAILED"];
                            string mPST = "";

                            while (mXML.Read())
                            {
                                if (mXML.NodeType == XmlNodeType.Element)
                                {
                                    switch (mXML.Name)
                                    {
                                        case "status":
                                            mStatus = mXML.ReadString();
                                            switch (mStatus)
                                            {
                                                case "SUCCESS":
                                                    mPST = "J";
                                                    mStatusID = ConfigurationManager.AppSettings["STATUS_CREDITED"];
                                                    break;
                                                case "DECLINED":
                                                    mPST = "K";
                                                    mStatusID = ConfigurationManager.AppSettings["STATUS_DECLINED"];
                                                    mFailedEmail = (mUSCreditDV[theRow]["type_id"].ToString() == ConfigurationManager.AppSettings["TYPE_ONE_TIME"]) ? mUSCreditDV[theRow]["create_user_id"].ToString() + "@willscot.com" : "";
                                                    break;
                                                case "EXPIRED":
                                                    mPST = "L";
                                                    mStatusID = ConfigurationManager.AppSettings["STATUS_DECLINED"];
                                                    mFailedEmail = (mUSCreditDV[theRow]["type_id"].ToString() == ConfigurationManager.AppSettings["TYPE_ONE_TIME"]) ? mUSCreditDV[theRow]["create_user_id"].ToString() + "@willscot.com" : "";
                                                    break;
                                                case "FAILURE":
                                                    mStatusID = ConfigurationManager.AppSettings["STATUS_FAILED"];
                                                    break;
                                            }
                                            break;
                                        case "transID":
                                            mTransID = mXML.ReadString();
                                            break;
                                        case "requestID":
                                            mRequestID = mXML.ReadString();
                                            break;
                                        case "requestToken":
                                            mRequestToken = mXML.ReadString();
                                            break;
                                        case "resultCode":
                                            mResultCode = mXML.ReadString();
                                            break;
                                        case "error":
                                            mStatus = "FAILURE";
                                            mStatusID = ConfigurationManager.AppSettings["STATUS_FAILED"];
                                            break;
                                        case "message":
                                            theNotes += "<br>" + mXML.ReadString();
                                            break;
                                        case "innerException":
                                            theNotes += "<br>" + mXML.ReadString();
                                            break;
                                    }
                                }
                            }

                            mResponse.Close();
                            mXML.Close();

                            #endregion

                            if (mStatusID != ConfigurationManager.AppSettings["STATUS_FAILED"].ToString())
                            {
                                #region update f03b11

                                mCommand = string.Format(@"
										UPDATE F03B11
										SET    rppst = '{3}'
											  ,rpddnj = jde_pkg.pkg_jdeu.fn_jdate(SYSDATE)
											  ,rpuser = '{4}'
											  ,rpupmj = pkg_jdeu.fn_jdate()
											  ,rpupmt = pkg_jdeu.fn_jtime()
											  ,rppid = 'CCT'
											  ,rpjobn = 'CCT'
										WHERE  rpdct = '{0}'
										AND    rpdoc = {1}
										AND    rpkco = '{2}'
									    AND    rppst != 'P'
									"
                                            , mUSCreditDV[theRow]["dct"].ToString()
                                            , mUSCreditDV[theRow]["doc"].ToString()
                                            , mUSCreditDV[theRow]["kco"].ToString()
                                            , mPST
                                            , Session["gNTName"].ToString()
                                            );
                                aConn.ExecuteCommand(mCommand);

                                #endregion
                            }

                            #region update cct

                            mCommand = string.Format(@"
							UPDATE cct_trans
							   SET status_id = {0}
								  ,maint_user_id = '{1}'
								  ,maint_date = sysdate
							      ,notes = '{3}'
								  ,cyber_id = {4}
								  ,cyber_token = '{7}'
								  ,result_code = {5}
								 {6}
							 WHERE id = '{2}'
							", mStatusID
                                 , Session["gNTName"].ToString()
                                 , mUSCreditDV[theRow]["id"]
                                 , WSString.FilterSQL(theNotes)
                                 , mRequestID
                                 , mResultCode
                                 , mStatusID == ConfigurationManager.AppSettings["STATUS_CREDITED"] ? ",amount = amount-" + mAmt.ToString() : ""
                                 , mRequestToken
                                 );
                            aConn.ExecuteCommand(mCommand);

                            #endregion


                            //send emails
                            //email cash list of trans processed
                            //cc email connected to trans
                            //email requester if trans is rejected

                            #region send email to contact

                            if (theEmail != "")
                            {
                                mCommon.emailWrapper(theEmail
                                                    , ""
                                                    , string.Format(@"Merchant Email Receipt - {0}", mStatus)
                                                    , string.Format(@"
														<tr>
															<td id='captionTd'>
																Credit Card Credit - US - {5}
															</td>
														</tr>
													
														<tr>
															<td><hr size='1' noshade></td>
														</tr>
														<tr><td>
														========= GENERAL INFORMATION =========<br>
<br>
														Merchant : Williams Scotsman, Inc.<br>
														Date/Time : {0}<br>
<br>
														========= ORDER INFORMATION =========<br>
														Invoice : {1}<br>
														Description : {2}<br>
														Amount : {3} (USD)<br>
														Credit Card : ****-****-****-{4}<br>
														Type : Credit Money Back To A Credit Card<br>
<br>
														============== RESULTS ==============<br>
														Response : {5}<br>
														Transaction ID : {6}<br>
														Transaction Token : {18}<br>
<br>
														==== CUSTOMER CONTACT INFORMATION ===<br>
														Customer ID : {7}<br>
														First Name : {8}<br>
														Last Name : {9}<br>
														Company : {10}<br>
														Address : {11}<br>
														City : {12}<br>
														State/Province : {13}<br>
														Zip/Postal Code : {14}<br>
														Country : {15}<br>
														Phone : {16}<br>
														E-Mail : {17}<br>
														</td></tr>
														"
                                                                , DateTime.Now.ToString()
                                                                , mUSCreditDV[theRow]["doc"]
                                                                , mUSCreditDV[theRow]["remark"]
                                                                , string.Format("{0:c}", theAmt)
                                                                , mUSCreditDV[theRow]["credit_card_suffix"]
                                                                , "<font style='font-weight:bold;" + (mStatus == "SUCCESS" ? "" : "color:red;") + "'>" + mStatus + "</font>"
                                                                , mRequestID
                                                                , mUSCreditDV[theRow]["cb"]
                                                                , mUSCreditDV[theRow]["card_first_name"]
                                                                , mUSCreditDV[theRow]["card_last_name"]
                                                                , mUSCreditDV[theRow]["company"]
                                                                , mUSCreditDV[theRow]["card_street"]
                                                                , mUSCreditDV[theRow]["card_city"]
                                                                , mUSCreditDV[theRow]["card_state"]
                                                                , mUSCreditDV[theRow]["card_zip"]
                                                                , mUSCreditDV[theRow]["card_country"]
                                                                , mUSCreditDV[theRow]["card_phone"]
                                                                , theEmail
                                                                , mRequestToken
                                                                )
                                                    , this
                                                    , "User Credit Email", mUSCreditDV[theRow]["HIFlag"].ToString() == "H");

                            }


                            #endregion
                        }
                        else
                        {
                            #region processing error message for no cc

                            mCommand = string.Format(@"
							UPDATE cct_trans
							   SET maint_user_id = '{0}'
								  ,maint_date = sysdate
							      ,notes = '{2}
Transaction could not be processed because credit card does not exist in OMB.'
							 WHERE id = {1}
							", Session["gNTName"].ToString()
                                 , mUSCreditDV[theRow]["id"]
                                 , WSString.FilterSQL(theNotes)
                                 );
                            aConn.ExecuteCommand(mCommand);



                            mStatus = "Transaction could not be processed because credit card does not exist in OMB.";
                            #endregion
                        }
                        #region send email to cash

                        mCommon.emailWrapper(ConfigurationManager.AppSettings["CASH_EMAIL"]
                                            , ""
                                            , string.Format(@"CCT Credit Transaction {0} - {1}", mUSCreditDV[theRow]["id"], mStatus)
                                            , string.Format(@"
														  <tr><td>This transaction credited money back to the credit card for this invoice.</td></tr>
														  <tr>
															<td>
																<table border=1 cellspacing=0 cellpadding=3>
																  <tr>
																	  <td style='font-weight:bold;white-space:nowrap;'>Transaction</td>
																	  <td style='font-weight:bold;white-space:nowrap;'>Status</td>
																	  <td style='font-weight:bold;white-space:nowrap;'>Invoice Doc #</td>
																	  <td style='font-weight:bold;white-space:nowrap;'>ODOC #</td>
																	  <td style='font-weight:bold;white-space:nowrap;'>CS #</td>
																	  <td style='font-weight:bold;white-space:nowrap;'>Amount</td>
																  </tr>
																  <tr>
																	<td></td>
																	<td>{1}</td>
																	<td>{3}</td>
																	<td>{4}</td>
																	<td>{5}</td>
																	<td style='text-align:right;'>{6}</td>
																  </tr>
																</table>
															</td>
														 </tr>
														"
                                                            , mUSCreditDV[theRow]["id"]
                                                            , mStatus
                                                            , Session["gServer"]
                                                            , mUSCreditDV[theRow]["doc"]
                                                            , mUSCreditDV[theRow]["rpodoc"]
                                                            , mUSCreditDV[theRow]["cs"]
                                                            , string.Format("{0:c}", theAmt))
                                            , this
                                            , "Cash Credit Email");

                        #endregion
                    }

                    mRetVal = mCommon.creditSearch(this, "US", true);
                    mRetVal.message = string.Format("{0}Update Complete", mTempMessage);
                }
                catch (Exception e)
                {
                    //save error here
                    mRetVal.error = true;
                    mCommon.logError("index.aspx.cs", "creditUSRow", "", Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }
                finally
                {
                    if (mResponse != null) { mResponse.Close(); }
                    if (mXML != null) { mXML.Close(); }
                    aConn.Close();
                }
            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }


        /**
         * sortCreditUS
         * 
         * saves the current info to the dv and sorts the row
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct sortCreditUS(string theCol, object[] theEmail, object[] theAmount, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                DataView mDV = (DataView)Session["gUSCreditDV"];
                Common mCommon = new Common();

                #region update dv

                try
                {
                    for (int i = 0; i < theAmount.Length; i++)
                    {
                        if (theEmail[i] != null) { mDV[i]["email"] = theEmail[i]; }
                        if (theAmount[i] != null) { mDV[i]["amount"] = theAmount[i].ToString().Substring(1); }
                        if (theNotes[i] != null) { mDV[i]["notes"] = theNotes[i]; }
                    }
                }
                catch (Exception e)
                {
                    //save error here
                    mRetVal.error = true;
                    mCommon.logError("index.aspx.cs", "sortCreditUS", "", Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }

                #endregion


                if (mDV.Sort.ToString() == theCol)
                {
                    mDV.Sort = theCol + " desc";
                }
                else
                {
                    mDV.Sort = theCol;
                }


                mRetVal = mCommon.creditSearch(this, "US", false);

            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }


        /**
         * creditCARow
         * 
         * first checks for logon, refreshes the page if not loged on
         * updates the row passed in to credited
         * creates a file with all the info to credit this row, returns the file to the user
         * file name is CACredit_Month-Day-Year_Hour;Min;Sec.txt
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct creditCARow(int theRow, double theAmt, string theEmail, string theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                Common mCommon = new Common();
                WSDBConnection aConn = new WSDBConnection();
                StreamWriter mSW = null;
                try
                {
                    aConn.Open("SISR", 0);
                    DataView mCACreditDV = (DataView)Session["gCACreditDV"];
                    DateTime mDT = DateTime.Now;
                    String mPath = ConfigurationManager.AppSettings["FILE_LOC"] +
                                   "//CACredit_" + mDT.Month.ToString() + "-" + mDT.Day.ToString() + "-" + mDT.Year.ToString() + "_" +
                                   mDT.Hour.ToString() + "-" + mDT.Minute.ToString() + "-" + mDT.Second.ToString() + ".html";
                    mSW = new StreamWriter(mPath);
                    string mTempMessage = "";

                    #region file head
                    //mSW.WriteLine("ID      | DCT | Doc                  | Inv Date | Company                                  | Gross Amount | Open Amount  | Amount       | Credit Card No   | Exp Date | Type      | Notes");
                    mSW.WriteLine(@"<html><head><style>
									td.cell
									{
										border-right:	1px solid #d6d6d6;
										border-bottom:	1px solid #d6d6d6;
										empty-cells:show;
									}
									td.leftCell
									{
										border-right:	1px solid #d6d6d6;
										border-bottom:	1px solid #d6d6d6;
										border-left:	1px solid #d6d6d6;
										empty-cells:show;
									}

									 
									th.headLeft
									{
										text-align:			center;
										font:				bold 12px Verdana;
										color:				#000000;
										background-color: #f3f3f3;
										border-right:	1px solid #5c5c5c;
										border-bottom:	1px solid #5c5c5c;
										border-top:	1px solid #5c5c5c;
										border-left:	1px solid #5c5c5c;
										white-space:nowrap;
									}
									th.head
									{
										text-align:			center;
										font:				bold 12px Verdana;
										color:				#000000;
										background-color: #f3f3f3;
										border-right:	1px solid #5c5c5c;
										border-bottom:	1px solid #5c5c5c;
										border-top:	1px solid #5c5c5c;
										white-space:nowrap;
									}
									</style></head><body>
									<table cellpadding='3' cellspacing='0'><thead><tr><th class='headLeft'>ID</th><th class='head'>DCT</th><th class='head'>Doc</th><th class='head'>Inv Date</th><th class='head'>Company</th><th class='head'>Gross Amount</th><th class='head'>Open Amount</th><th class='head'>Amount</th><th class='head'>Credit Card No</th><th class='head'>Exp Date</th><th class='head'>Type</th><th class='head'>Notes</th></tr></thead>");
                    #endregion

                    long mCCNum = Convert.ToInt64(mCACreditDV[theRow]["credit_card_no"].ToString() + mCACreditDV[theRow]["credit_card_suffix"].ToString());

                    #region update cct

                    string mCommand = string.Format(@"
					UPDATE cct_trans
					   SET status_id = {0}
						  ,amount = amount - {3}
						  ,notes = '{4}'
						  ,email = '{5}'
					      ,maint_user_id = '{2}'
						  ,maint_date = sysdate
					 WHERE id = '{1}'
					   AND maint_date = to_date('{6}','mm/dd/yy hh:mi:ss am')
					", ConfigurationManager.AppSettings["STATUS_CREDITED"]
                     , mCACreditDV[theRow]["id"].ToString()
                     , Session["gNTName"].ToString()
                     , theAmt * 100
                     , WSString.FilterSQL(theNotes)
                     , theEmail
                     , mCACreditDV[theRow]["maint_date"]
                     );
                    #endregion

                    if (aConn.ExecuteCommand(mCommand) == 0)
                    {
                        //write message since row didn't update
                        mTempMessage += GetMaintInfo(aConn, mCACreditDV[theRow]["id"].ToString());
                    }
                    else
                    {
                        mSW.WriteLine(string.Format(@"<tr><td class='leftCell'>{0}&nbsp;</td><td class='cell'>{1}&nbsp;</td><td class='cell'>{2}&nbsp;</td><td class='cell'>{3}&nbsp;</td><td class='cell'>{4}&nbsp;</td><td class='cell'>{5}&nbsp;</td><td class='cell'>{6}&nbsp;</td><td class='cell'>{7}&nbsp;</td><td class='cell'>{8}&nbsp;</td><td class='cell'>{9}&nbsp;</td><td class='cell'>{10}&nbsp;</td><td class='cell'>{11}&nbsp;</td></tr></table></body></html>"
                                                                 , mCACreditDV[theRow]["id"].ToString().PadRight(7, ' ')
                                                                 , mCACreditDV[theRow]["dct"].ToString().PadRight(3, ' ')
                                                                 , mCACreditDV[theRow]["doc"].ToString().PadRight(20, ' ')
                                                                 , mCACreditDV[theRow]["invoice_date"].ToString().PadRight(8, ' ')
                                                                 , mCACreditDV[theRow]["company"].ToString().PadRight(40, ' ')
                                                                 , mCACreditDV[theRow]["gross"].ToString().PadRight(12, ' ')
                                                                 , mCACreditDV[theRow]["openamount"].ToString().PadRight(12, ' ')
                                                                 , theAmt.ToString().PadRight(12, ' ')
                                                                 , mCACreditDV[theRow]["credit_card_no"].ToString().PadRight(12, ' ') + mCACreditDV[theRow]["credit_card_suffix"].ToString()
                                                                 , mCACreditDV[theRow]["exp"].ToString().PadRight(8, ' ')
                                                                 , mCACreditDV[theRow]["type_name"].ToString().PadRight(9, ' ')
                                                                 , theNotes));




                    }

                    mRetVal = mCommon.creditSearch(this, "CA", true);
                    if (mTempMessage == "") { mRetVal.filePath = mPath; }
                    mRetVal.message = string.Format("{0}Update Complete", mTempMessage);

                }
                catch (Exception e)
                {
                    mRetVal.error = true;
                    mCommon.logError("index.aspx.cs", "creditCARow", "", Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }
                finally
                {
                    if (mSW != null) { mSW.Close(); }
                    aConn.Close();
                }
            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }

        /**
         * sortCreditCA
         * 
         * saves the current info to the dv and sorts the DV
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct sortCreditCA(string theCol, object[] theEmail, object[] theAmount, object[] theNotes)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                DataView mDV = (DataView)Session["gCACreditDV"];

                #region update dv

                for (int i = 0; i < theAmount.Length; i++)
                {
                    if (theEmail[i] != null) { mDV[i]["email"] = theEmail[i]; }
                    if (theAmount[i] != null) { mDV[i]["amount"] = theAmount[i].ToString().Substring(1); }
                    if (theNotes[i] != null) { mDV[i]["notes"] = theNotes[i]; }
                }

                #endregion


                if (mDV.Sort.ToString() == theCol)
                {
                    mDV.Sort = theCol + " desc";
                }
                else
                {
                    mDV.Sort = theCol;
                }

                Common mCommon = new Common();
                mRetVal = mCommon.creditSearch(this, "CA", false);
            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }

        #endregion

        #region edit functions

        /**
		 * search
		 * 
		 * checks to see if user is loged in, if not refreshes the page
		 * otherwise sets search parms on the session and returns results
		 **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct editSearch(string theTransID)
        {
            if (CheckLogin())
            {
                Common common = new Common();

                return common.editSearch(this, theTransID);
            }
            else
            {
                return gRefresh;
            }
        }



        /**
         * editRow
         * 
         * first checks for logon, refreshes the page if not loged on
         * updates the row passed in to credited
         * creates a file with all the info to credit this row, returns the file to the user
         * file name is CACredit_Month-Day-Year_Hour;Min;Sec.txt
         **/
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public Common.RetStruct editRow(int theRow, double theAmt, string theEmail, string theNotes, string theStatus)
        {
            Common.RetStruct mRetVal = new Common.RetStruct();
            if (CheckLogin())
            {
                Common mCommon = new Common();
                WSDBConnection aConn = new WSDBConnection();
                string mTempMessage = "";
                try
                {
                    aConn.Open("SISR", 0);
                    DataView mEditDV = (DataView)Session["gEditDV"];

                    #region update cct

                    string mCommand = string.Format(@"
					UPDATE cct_trans
					   SET status_id = {0}
						  ,amount = {3}
						  ,notes = '{4}'
						  ,email = '{5}'
					      ,maint_user_id = '{2}'
						  ,maint_date = sysdate
					 WHERE id = '{1}'
					   AND maint_date = to_date('{6}','mm/dd/yy hh:mi:ss am')
					", theStatus
                     , mEditDV[theRow]["id"].ToString()
                     , Session["gNTName"].ToString()
                     , theAmt * 100
                     , WSString.FilterSQL(theNotes)
                     , theEmail
                     , mEditDV[theRow]["maint_date"]
                     );
                    #endregion

                    if (aConn.ExecuteCommand(mCommand) == 0)
                    {
                        //write message since row didn't update
                        mTempMessage = GetMaintInfo(aConn, mEditDV[theRow]["id"].ToString());
                    }

                    mRetVal = mCommon.editSearch(this, mEditDV[theRow]["id"].ToString());
                    mRetVal.message = string.Format("{0}Update Complete", mTempMessage);

                }
                catch (Exception e)
                {
                    mRetVal.error = true;
                    mCommon.logError("index.aspx.cs", "editRow", "", Session["gNTName"].ToString(), e.ToString(), "ERROR");
                }
                finally
                {
                    aConn.Close();
                }
            }
            else
            {
                mRetVal = gRefresh;
            }

            return mRetVal;
        }


        #endregion
    }
}