using System;
using System.Web.UI;
using Devart.Data.Oracle;
using WS20.CCT.App_Data;
using WS20.Framework;

namespace WS20.CCT
{
    public partial class CybersourceSearch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region setup headers and footers

            mWSHeader.SetPageName("Cybersource Search");
            mWSHeader.SetAppTitleHtml("Version 1.0.0");
            mHeaderClientID.Value = mWSHeader.ClientID;
            mWSHeader.AddMenuItem("Cybersource Search", "CybersourceSearch.aspx");
            mWSHeader.AddMenuItem("BOA Search", "index.aspx");
            #endregion //end of header/footer setup -  Put user code to initialize the page here
        }

        protected void mCyberSearchButton_Click(object sender, EventArgs e)
        {

            WSDBConnection aConn = new WSDBConnection();
            OracleDataReader aDR = null;
            int aRowsPerPage;
            int.TryParse(mCyberRowsPerPage.Value, out aRowsPerPage);

            int ct = 0;
            int aPage = 0;
            string aPageNav = "";
            string aQuery = "";
            string lazytotal = "";
            int aMaxRows = 500;
            string aExcel = "";
            string aTableHead = @"<table cellpadding='3' cellspacing='0'>
                                    <thead>
                                        <tr><td class='printLink' colspan='12'><a href=""#"" onclick=""printPage('E','CyberExcel');return false;"">Export to Excel</a></td></tr>
                                        <tr>
                                            <th class='headLeft'><a href='#' onclick=""CyberSort('trans_id')"" title='Click to Sort by ID'>ID</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('type_desc')"" title='Click to Sort by Type'>Type</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('order_skey')"" title='Click to Sort by Order #'>Order #</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('rpdct')"" title='Click to Sort by Dct'>Dct</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('rpdoc')"" title='Click to Sort by Doc'>Doc</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('rpkco')"" title='Click to Sort by Company'>Company</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('create_date')"" title='Click to Sort by Create Date'>Create Date</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('create_user_id')"" title='Click to Sort by Create User'>Create User</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('chargeamt')"" title='Click to Sort by Amount'>Amount</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('payment_account_sub_type')"" title='Click to Sort by Account Type'>Account Type</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('cnfrm_number')"" title='Click to Sort by Cnfrm #'>Cnfrm #</a></th>
                                            <th class='head'><a href='#' onclick=""CyberSort('status_desc')"" title='Click to Sort by Status'>Status</a></th>
                                        </tr>
                                    </thead>
                                    <tbody>";

            mCyberResultsDiv.InnerHtml = "";
            mCyberSearchMsgDiv.InnerHtml = "";

            try
            {
                if (aConn.Open("CCKEY"))
                {
                    #region query

                    aQuery = string.Format(@"
                                        SELECT * FROM (
                                        SELECT /*+ Rule */ h.trans_id
      ,tl.member_desc type_desc
      ,nvl(oh.order_skey
          ,d.order_skey) AS order_skey
      ,rp.rpdct
      ,rp.rpdoc
      ,rp.rpkco
      ,h.billto_email email_address
      ,TRIM(to_char(d.chargeamt / 100
                   ,'$999,999,999.99')) chargeamt
      ,TRIM(to_char(SUM(d.chargeamt) over(PARTITION BY 1) / 100
                   ,'$999,999,999.99')) lazytotal
      ,NULL payment_account_sub_type
      ,sl.member_desc status_desc
      ,h.requestid cnfrm_number
      ,TRIM(to_char(h.orderamount / 100
                   ,'$999,999,999.99')) totalamt
      ,NULL payment_account_type
      ,NULL account_number
      ,NULL account_other
      ,MIN(decode(rp.rppst
                 ,NULL
                 ,''
                 ,rp.rppst || ' - ' || dr.drdl01)) paystatus
      ,h.create_date
      ,h.create_user_id
      ,NULL charge_date
      ,NULL charge_user_id
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
      ,NULL returned_date
  FROM cckey.cyber_trans_header h
      ,cckey.cyber_trans_detail d
      ,cckey.cyber_lookup       tl
      ,cckey.cyber_lookup       sl
      ,f03b11                   rp
      ,f0005                    dr
      ,f0101                    ab
      ,omb_order_header         oh
 WHERE h.trans_id = d.trans_id
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
                                        GROUP BY h.trans_id
         ,tl.member_desc
         ,nvl(oh.order_skey
             ,d.order_skey)
         ,rp.rpdct
         ,rp.rpdoc
         ,rp.rpkco
         ,h.billto_email
         ,d.chargeamt
          /* ,decode(h.payment_account_type
          ,'CC'
          ,h.payment_account_sub_type
          ,decode(h.payment_account_sub_type
                 ,NULL
                 ,''
                 ,'ACH'))*/
          /*   ,CASE
            WHEN h.status_id = 5 THEN
             'Pending'
            WHEN h.status_id = 11 THEN
             'Returned'
            WHEN h.cnfrm_number IS NOT NULL THEN
             'Complete'
            ELSE
             'Incomplete'
          END*/
          --  ,h.cnfrm_number
          --  ,h.amt
          /*   ,decode(h.payment_account_type
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
                        ,substr(lpad(h.credit_card_expiration_date
                                    ,4
                                    ,0)
                               ,0
                               ,2) || '/' || substr(lpad(h.credit_card_expiration_date
                                                        ,4
                                                        ,0)
                                                   ,3))
                 ,'ACH'
                 ,h.bank_routing_number
                 ,NULL)*/
         ,sl.member_desc
         ,h.requestid
         ,TRIM(to_char(h.orderamount / 100
                      ,'$999,999,999.99'))
         ,h.create_date
         ,h.create_user_id
          --  ,h.charge_date
          --  ,h.charge_user_id
         ,rp.rpodoc
         ,rp.rpdivj
         ,rp.rprmk
         ,ab.aban86
         ,ab.aban85
         ,oh.ship_an8
         ,rp.rpalph)
                                             WHERE rownum <= {16}"

                                            , mCyberTransID.Value == "" ? "" : " AND h.trans_id = " + mCyberTransID.Value
                                            , mCyberCB.Value == "" ? "" : " AND ab.aban85 = " + mCyberCB.Value
                                            , mCyberCustName.Value == "" ? "" : " AND lower(ab.abalph) LIKE lower('%" + mCyberCustName.Value + "%')"
                                            , mCyberCS.Value == "" ? "" : " AND ab.aban8 = " + mCyberCS.Value
                                            , mCyberOdoc.Value == "" ? "" : " AND rp.rpodoc = " + mCyberOdoc.Value
                                            , mCyberOrder.Value == "" ? "" : " AND oh.order_skey = " + mCyberOrder.Value
                                            , mCyberDCT.Value == "" ? "" : " AND rp.rpdct = upper('" + mCyberDCT.Value + "')"
                                            , mCyberDOC.Value == "" ? "" : " AND rp.rpdoc = " + mCyberDOC.Value
                                            , mCyberKCO.Value == "" ? "" : " AND rp.rpkco = '" + mCyberKCO.Value + "'"
                                            , mCyberSuffx.Value == "" ? "" : " AND h.credit_card_account_number like '%" + mCyberSuffx.Value + "%'"
                                            , mCyberChargeDateFrom.Text != "" && mCyberChargeDateTO.Text != "" ? " AND h.charge_date BETWEEN to_date('" + mCyberChargeDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS')-(4/24) /*subtract 4 hours so the range is 8-8*/ AND to_date('" + mCyberChargeDateTO.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')-(4/24) /*subtract 4 hours so the range is 8-8*/" :
                                            (mCyberChargeDateFrom.Text != "" ? " AND h.charge_date > to_date('" + mCyberChargeDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS')-(4/24) /*subtract 4 hours so the range is 8-8*/" :
                                            (mCyberChargeDateTO.Text != "" ? " AND h.charge_date < to_date('" + mCyberChargeDateTO.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')-(4/24) /*subtract 4 hours so the range is 8-8*/" : ""))
                                            , mCyberStatusID.Value == "P" ? " AND h.status_id = 5" :
                                             (mCyberStatusID.Value == "C" ? " AND h.status_id not in(5,11) AND cnfrm_number is not null" :
                                             (mCyberStatusID.Value == "X" ? " AND h.status_id not in(5,11) AND cnfrm_number is null " :
                                             (mCyberStatusID.Value == "R" ? " AND h.status_id = 11 " : "")))
                                             , mCyberTypeID.Value == "AC" ? " AND h.type_id in (1,3,8)" :
                                             (mCyberTypeID.Value == "AO" ? " AND h.type_id in (1,3)" :
                                             (mCyberTypeID.Value == "WO" ? " AND h.type_id = 1" :
                                             (mCyberTypeID.Value == "PE" ? " AND h.type_id = 2" :
                                             (mCyberTypeID.Value == "PL" ? " AND h.type_id = 3" :
                                             (mCyberTypeID.Value == "PR" ? " AND h.type_id = 4" :
                                             (mCyberTypeID.Value == "PM" ? " AND h.type_id = 12" :
                                             (mCyberTypeID.Value == "M" ? " AND h.type_id = 8" : "")))))))
                                            , mCyberCCType.Value == "ACH" ? " AND h.payment_account_type = 'ACH' " :
                                            (mCyberCCType.Value == "MSTRVISA" ? " AND h.payment_account_sub_type in ('MC','VISA')" :
                                            (mCyberCCType.Value == "" ? "" : " AND h.payment_account_sub_type = '" + mCyberCCType.Value + "'"))
                                            , mCyberCnfrmNum.Value == "" ? "" : " AND h.cnfrm_number = " + mCyberCnfrmNum.Value
                                            , CyberSortBy.Value == "" ? "" : CyberSortBy.Value + ","
                                            , aMaxRows
                                            , mCyberC.Value == "" ? "" : " AND ab.aban86 = " + mCyberC.Value
                                            , mCyberCreateDateFrom.Text != "" && mCyberCreateDateTo.Text != "" ? " AND h.create_date BETWEEN to_date('" + mCyberCreateDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS') AND to_date('" + mCyberCreateDateTo.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')" :
                                            (mCyberCreateDateFrom.Text != "" ? " AND h.create_date > to_date('" + mCyberCreateDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS')" :
                                            (mCyberCreateDateTo.Text != "" ? " AND h.create_date < to_date('" + mCyberCreateDateTo.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')" : ""))
                                            , mCyberReturnDateFrom.Text != "" && mCyberReturnDateTo.Text != "" ? " AND h.returned_date BETWEEN to_date('" + mCyberReturnDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS') AND to_date('" + mCyberReturnDateTo.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')" :
                                            (mCyberReturnDateFrom.Text != "" ? " AND h.returned_date > to_date('" + mCyberReturnDateFrom.Text + " 00:00:00','mm/dd/yyyy HH24:MI:SS')" :
                                            (mCyberReturnDateTo.Text != "" ? " AND h.returned_date < to_date('" + mCyberReturnDateTo.Text + " 23:59:59','mm/dd/yyyy HH24:MI:SS')" : ""))

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
                                            <tr><td class='boldTD'>Cyber Search</td></tr>
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
                                            , mCyberTransID.Value
                                            , mCyberCnfrmNum.Value
                                            , mCyberCB.Value
                                            , mCyberCustName.Value
                                            , mCyberCS.Value
                                            , mCyberOdoc.Value
                                            , mCyberOrder.Value
                                            , mCyberDCT.Value + " " + mCyberDOC.Value + " " + mCyberKCO.Value
                                            , mCyberSuffx.Value
                                            , mCyberChargeDateFrom.Text + " - " + mCyberChargeDateTO.Text
                                            , mCyberStatusID.Items[mCyberStatusID.SelectedIndex].Text
                                            , mCyberTypeID.Items[mCyberTypeID.SelectedIndex].Text
                                            , mCyberCCType.Items[mCyberCCType.SelectedIndex].Text
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
                                mCyberResultsDiv.InnerHtml += @"
                                        </table>
                                    </div>
                                    <div id='mCyberPage" + aPage + "' style='display:none;'>" + aTableHead;
                            }
                            else
                            {
                                lazytotal = aDR["lazytotal"].ToString();
                                mCyberResultsDiv.InnerHtml += "<div id='mCyberPage1'>" + aTableHead;
                            }

                            //update the nav
                            aPageNav += string.Format(@"<a href='#' id='mCyberPage{0}Link' {1} onClick=""changeTablePage('{0}')"">{0}</a>&nbsp;&nbsp;"
                                                    , aPage
                                                    , aPage == 1 ? "class='selectedPageLink'" : ""
                                                    );
                        }

                        #endregion

                        //main row leftCell
                        mCyberResultsDiv.InnerHtml += string.Format(@"
                                        <tr>
                                            <td class='leftCell' id='detailCell0{0}'><a href=""javascript:expandCyberRow('{0}')"">{1}&nbsp;</a></td> 
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
                                            , aDR["trans_id"].ToString()
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
                        mCyberResultsDiv.InnerHtml += string.Format(@"
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
                                    , aDR["trans_id"].ToString()
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
                    mCyberResultsDiv.InnerHtml += string.Format(@"
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

                    if (aPage > 1) { mCyberResultsDiv.InnerHtml += aPageNav; }
                    else if (ct == 0) { mCyberResultsDiv.InnerHtml = "No Results Found"; }

                    if (ct == aMaxRows) { mCyberSearchMsgDiv.InnerHtml = "Only the first " + aMaxRows.ToString() + " rows are displayed."; }

                    if (Session["security_level"] != null && Session["security_level"].ToString() == "1")
                    {
                        mCyberSearchMsgDiv.InnerHtml += "<br><br>" + aQuery;
                    }


                    aExcel += "</table></body></html>";
                    Session["CyberExcel"] = aExcel;

                }
            }
            catch (Exception ex)
            {
                mCyberSearchMsgDiv.InnerHtml = ex.ToString();
                mCyberResultsDiv.InnerHtml = aQuery;
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

    }
}
