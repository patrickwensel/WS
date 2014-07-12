using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web.UI;
using Devart.Data.Oracle;
using WS20.Framework;

namespace WS20.CCT.App_Data
{
	/// <summary>
	/// Summary description for Common
	/// </summary>
	public class Common
	{
		//constant for max number of rows in a search, if used outside this page move to web config
		protected int gMaxRows = 500;

		public Common()
		{
		}

		/**
		 * RetStruct
		 * 
		 * hold values returned through ajax
		 **/
		public struct RetStruct
		{
			public string usHTML;
			public string caHTML;
			public string[] usEmail;
			public string[] caEmail;
			public bool error;
			public string message;
			public string filePath;
			public string[] usAmt;
			public string[] caAmt;
		} 
		
		/**
		 * getTransData
		 * 
		 * selects the status and types from cct_status andn cct_type
		 * creates js calls to load dropdown / array of these types
		 * 
		 * selects all one time trans from cct_trans and recurring trans from f03b11
		 * creates js to load trans table
		 * 
		 * puts everything in a try catch block since error doesn't get passed back from
		 * .cs file to the javascript (program just hangs)
		 * currently sending back an empty string throws an error 
		 **/
		public RetStruct getTransData(Page thePage, string theLoc, bool requery)
		{
			RetStruct mRetVal = new RetStruct();
			WSDBConnection aConn = new WSDBConnection();
			string aQuery = "";

			try
			{
				aConn.Open("CCKEY", 0);
				DataView mStatusDV;
				DataView mUSTransDV = null;
				DataView mCATransDV = null;
				DataTable mHoldDT;


				if (thePage.Session["gStatusDV"] == null)
				{
					getStatusType(thePage);
				}

				if (requery)
				{
					#region selects transactions

					string mLoc;
					if (theLoc == "US")
					{
						mLoc = "AND t.kco IN ('00010','00090')";
					}
					else if (theLoc == "CA")
					{
						mLoc = "AND t.kco = '00020'";
					}
					else
					{
						mLoc = "AND t.kco IN ('00010','00090','00020')";
					}


					aQuery = string.Format(@"
								SELECT t.id
									  ,t.type_id
									  ,y.type_name
									  ,t.order_skey
									  ,t.dct
									  ,t.doc
									  ,t.kco
									  ,t.status_id
									  ,(nvl(t.amount
										   ,-1) * .01) amount
									  ,t.email
									  ,t.maint_date
									  ,t.reason_code
									  ,t.notes
									  ,decode(t.notes,null,'','*') notes_flag
									  ,i.contact_name
									  ,i.contact_phone
									  ,i.card_first_name
									  ,i.card_last_name
									  ,i.card_street
									  ,i.card_city
									  ,i.card_state
									  ,i.card_zip
									  ,i.card_country
									  ,i.card_phone
									  ,CASE
										 WHEN k.key IS NOT NULL
											  AND ooh.credit_card_crypt IS NOT NULL THEN
										  TRIM(pkg_cc_crypt.decrypt(ooh.credit_card_crypt
																   ,(k.key)))
										 ELSE
										  ' '
									   END credit_card_no
									  ,decode(ooh.credit_card_suffix
											 ,NULL
											 ,t.cc_suffix
											 ,ooh.credit_card_suffix) AS credit_card_suffix
									  ,decode(ooh.credit_card_expiration_date
											 ,NULL
											 ,' '
											 ,substr(ooh.credit_card_expiration_date
													,0
													,2) || '/' || substr(ooh.credit_card_expiration_date
																		,3)) exp
									  ,(nvl(SUM(rpag)
										   ,0) * .01) gross
									  ,(nvl(SUM(rpaap)
										   ,0) * .01) openamount
									  ,rp.rpodoc
									  ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
											  ,'MM/DD/YY') AS invoice_date
									  ,rp.rprmk remark
									  ,min(decode(rp.rppst
											 ,NULL
											 ,''
											 ,rp.rppst || ' - ' || dr.drdl01)) paystatus
									  ,rp.rpalph company
									  ,ab.aban85 cb
									  ,ooh.ship_an8 cs
									  ,s.status_name
									  ,e.inv_type HIFlag
									  ,rpar08
									  ,rpvinv
									  ,nvl(rppo,rpvinv) as userPO
								  FROM cct_trans        t
									  ,cct_order_info   i
									  ,omb_order_header ooh
									  ,f03b11           rp
									  ,f0101            ab
									  ,cc_keys          k
									  ,cct_type         y
									  ,f0005            dr
									  ,sisr.cct_status  s
									  ,jde_pkg.inv_extract_branch_type e
								 WHERE rppst NOT IN ('J', 'P')
								   AND ((t.type_id = '{3}' AND kco != '00020') OR kco = '00020')
								   AND t.dct = rp.rpdct
								   AND t.doc = rp.rpdoc
								   AND t.kco = rp.rpkco
								   AND t.status_id IN ({0},{1})
								   AND t.order_skey = ooh.order_skey
								   AND ooh.order_skey = k.order_skey(+)
								   AND t.type_id = y.type_id
								   AND ab.aban8 = rp.rpan8
								   AND dr.drsy(+) = '00  '
								   AND dr.drrt(+) = 'PS'
								   AND TRIM(dr.drky(+)) = rppst
								   AND i.order_skey(+) = t.order_skey
								   AND t.status_id = s.status_id
								   AND ooh.oe_location = e.mcu(+)
								 GROUP BY t.id
										 ,t.type_id
										 ,y.type_name
										 ,t.order_skey
										 ,t.dct
										 ,t.doc
										 ,t.kco
										 ,t.status_id
										 ,t.amount
										 ,t.email
										 ,t.maint_date
										 ,t.reason_code
										 ,t.notes
										 ,decode(t.notes,null,'','*')
										 ,i.contact_name
										 ,i.contact_phone
										 ,i.card_first_name
										 ,i.card_last_name
										 ,i.card_street
										 ,i.card_city
										 ,i.card_state
										 ,i.card_zip
										 ,i.card_country
										 ,i.card_phone
										 ,CASE
											 WHEN k.key IS NOT NULL
												  AND ooh.credit_card_crypt IS NOT NULL THEN
											  TRIM(pkg_cc_crypt.decrypt(ooh.credit_card_crypt
																	   ,(k.key)))
											 ELSE
											  ' '
										   END
										 ,decode(ooh.credit_card_suffix
												,NULL
												,t.cc_suffix
												,ooh.credit_card_suffix)
										 ,decode(ooh.credit_card_expiration_date
												,NULL
												,' '
												,substr(ooh.credit_card_expiration_date
													   ,0
													   ,2) || '/' || substr(ooh.credit_card_expiration_date
																		   ,3))
										 ,rp.rpodoc
										 ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
												 ,'MM/DD/YY')
										 ,rp.rprmk
										 ,rp.rpalph
										 ,ab.aban85
										 ,ooh.ship_an8
										 ,s.status_name
										 ,e.inv_type 
										 ,rpar08
										 ,rpvinv
										 ,nvl(rppo,rpvinv)
								 ORDER BY t.type_id
										 ,t.order_skey
										 ,t.dct
										 ,t.doc
					"
                    , ConfigurationManager.AppSettings["STATUS_UNSENT"]
					, ConfigurationManager.AppSettings["STATUS_DECLINED"]
					, mLoc
					, ConfigurationManager.AppSettings["TYPE_RECURRING"]
					);


					mHoldDT = aConn.GetDataTable(aQuery);
					mHoldDT.Columns.Add("checked", System.Type.GetType("System.String"));
					mHoldDT.Columns.Add("update", System.Type.GetType("System.String"));

					if (theLoc == "US" || theLoc == "ALL")
					{
						mUSTransDV = new DataView(mHoldDT);
						mUSTransDV.RowFilter = "kco in ('00010','00090')";
						thePage.Session["gUSTransDV"] = mUSTransDV;
					}

					if (theLoc == "CA" || theLoc == "ALL")
					{
						mCATransDV = new DataView(mHoldDT);
						mCATransDV.RowFilter = "kco = '00020'";
						thePage.Session["gCATransDV"] = mCATransDV;
					}

					#endregion
				}
				else
				{
					mUSTransDV = (DataView)thePage.Session["gUSTransDV"];
					mCATransDV = (DataView)thePage.Session["gCATransDV"];
				}
				mStatusDV = (DataView)thePage.Session["gStatusDV"];

				double totalAmount;
				//object[] mUSEmailArr;
				//object[] mCAEmailArr;
				int expMonth;
				int expYear;

				if (theLoc == "US" || theLoc == "ALL")
				{

					#region load US trans

					mRetVal.usEmail = new string[mUSTransDV.Count];

					mRetVal.usHTML = string.Format(@"
								<div style='font-weight:bold;float:left;'>{1} {2}</div>
								<div class='transTable' >
									<div class='printHelper'>
										<div class='scrollHead' >
											<div class='headCK'><input type=checkbox name=selectAllUS onclick={0}toggleCB(this.checked,document.mForm.selectRowUS,'US'){0}></div>	
											<div class='scrollHeadID'><a href='#' onclick={0}sortTransUS('id');return false;{0}>ID<span></span></a></div>
											<div class='scrollType'><a href='#' onclick={0}sortTransUS('type_name');return false;{0}>Type<span></span></a></div>
											<div class='scrollOrder'><a href='#' onclick={0}sortTransUS('order_skey');return false;{0}>Order #<span></span></a></div>
											<div class='scrollDCT'><a href='#' onclick={0}sortTransUS('dct');return false;{0}>Dct<span></span></a></div>
											<div class='scrollDOC'><a href='#' onclick={0}sortTransUS('doc');return false;{0}>Doc #<span></span></a></div>
											<div class='scrollKCO'><a href='#' onclick={0}sortTransUS('kco');return false;{0}>Company<span></span></a></div>
											<div class='scrollEmail'><a href='#' onclick={0}sortTransUS('email');return false;{0}>Email<span></span></a></div>
											<div class='scrollAmt'><a href='#' onclick={0}sortTransUS('amount');return false;{0}>Amount<span></span></a></div>
											<div class='scrollReason'><a href='#' onclick={0}sortTransUS('reason_code');return false;{0}>Reason<span></span></a></div>
											<div class='scrollCardType'><a href='#' onclick={0}sortTransUS('credit_card_no');return false;{0}>Card Type<span></span></a></div>
											<div class='scrollNotes'><a href='#' onclick={0}sortTransUS('notes_flag');return false;{0}>Notes<span></span></a></div>
											<div class='headStatus'><a href='#' onclick={0}sortTransUS('status_name');return false;{0}>Status<span></span></a></div>
										</div>
									</div>
									<div class=ScrollTable>
									", "\"",mUSTransDV.Count,(mUSTransDV.Count > 1)?"Results":"Result");
					totalAmount = 0;
					for (int i = 0; i < mUSTransDV.Count; i++)
					{
						if (mUSTransDV[i]["amount"].ToString() == "-0.01")
						{
							mUSTransDV[i]["amount"] = mUSTransDV[i]["openamount"];
						}
						totalAmount += Convert.ToDouble(mUSTransDV[i]["amount"]);

						mRetVal.usEmail[i] = mUSTransDV[i]["email"].ToString();

						if (mUSTransDV[i]["exp"].ToString().Length > 3)
						{
							expMonth = Convert.ToInt32(mUSTransDV[i]["exp"].ToString().Substring(0, 2));
							expYear = Convert.ToInt32("20" + mUSTransDV[i]["exp"].ToString().Substring(3));
						}
						else
						{
							expMonth = -1;
							expYear = -1;
						}

						mRetVal.usHTML += string.Format(@"
									<div class='scrollRow' id='mScrollRowUS_{0}'>
										<div id='ckTDUS_{0}' class='rowCK' style='{13}'><input id=selectRowUS_{0}  {14} {15} type=checkbox name=selectRowUS onclick='updateIt(this);'></div>
										<div id='idTDUS_{0}' class='scrollID' style='{13}'>&nbsp;{11}</div>
										<div id='typeTDUS_{0}' class='scrollType' style='{13}'>&nbsp;<a onclick={1}displayRow({0},'US'){1} onmouseover={1}window.status='action Details';return true;{1} onmouseout={1}window.status='';return true;{1}>{2}</a></div>
										<div id='keyTDUS_{0}' class='scrollOrder' style='{13}'>&nbsp;{3}</div>
										<div id='dctTDUS_{0}' class='scrollDCT' style='{13}'>&nbsp;{4}</div>
										<div id='docTDUS_{0}' class='scrollDOC' style='{13}'>&nbsp;{5}</div>
										<div id='kcoTDUS_{0}'  class='scrollKCO' style='{13}'>&nbsp;{6}</div>
										<div id='emailTDUS_{0}' class='scrollEmail' style='{13}'>&nbsp;<input class='flat' id=emailUS_{0} value='{7}' type=text size=25 onchange='updateIt(this);'></div>
										<div id='amountTDUS_{0}' class='scrollAmt' style='{13}'>&nbsp;{12}</div>
										<div id='reasonTDUS_{0}'  class='scrollReason' style='{13}'>&nbsp;{10}</div>
										<div id='cardTypeTDUS_{0}'  class='scrollCardType' style='{13}'>&nbsp;{17}</div>
										<div id='notesTDUS_{0}'  class='scrollNotes' style='{13}'>&nbsp;{16}</div>
										<div id='statusTDUS_{0}' class='scrollStatus' style='{13}'>&nbsp;<select style='width:130px;' {8} id='statusUS_{0}' onchange='updateIt(this);'>{9}</select></div>
									</div>
									"
									, i
									, "\""
									, mUSTransDV[i]["type_name"]
									, mUSTransDV[i]["order_skey"].ToString()
									, mUSTransDV[i]["dct"].ToString()
									, mUSTransDV[i]["doc"].ToString()
									, mUSTransDV[i]["kco"].ToString()
									, mUSTransDV[i]["email"].ToString()
									, (mUSTransDV[i]["checked"].ToString() == "checked") ? "disabled" : ""
									, thePage.Session["gStatusOptUS_" + mUSTransDV[i]["status_id"]]
									, mUSTransDV[i]["reason_code"].ToString()
									, mUSTransDV[i]["id"]
									, string.Format("{0:c}",mUSTransDV[i]["amount"])
									, (expYear < DateTime.Now.Year ||
									   (expYear == DateTime.Now.Year && expMonth < DateTime.Now.Month)) ? "background: #f3f3f3;" : ""
									, mUSTransDV[i]["checked"]
									, (mUSTransDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_DELETED"].ToString() ||
									   mUSTransDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"].ToString()) ? "disabled" : ""
								    , mUSTransDV[i]["notes_flag"]
									, getCCType(mUSTransDV[i]["credit_card_no"].ToString())
									);

						mRetVal.usHTML += string.Format(@"
											<div class='hiddenNotes'>
												Notes: {0}
											</div>
											"
											, mUSTransDV[i]["notes"].ToString()
											);

						mRetVal.usHTML += string.Format(@"
											<div id='mHiddenRowUS_{0}' class='transDetailRowDiv' style='display:none;'>
												<table class='detailRow'>
													<tr>
														<td class='label'>Credit Card #: </td><td class='input'>************&nbsp;{2}</td>
														<td class='label'>Exp Date: </td><td class='input'>{3}</td>
														<td class='label'>Pay Status: </td><td class='input' colspan=3>{4}</td>
													</tr>
													<tr>
														<td class='label'>Maint Date: </td><td class='input'>{11}</td>
														<td class='label'>Gross: </td><td class='input'>{5}</td>
														<td class='label'>Open: </td><td class='input'>{6}</td>
														<td class='label'>ODOC: </td><td class='input'>{7}</td>
													</tr>
													<tr>
														<td class='label'>Invoice Date: </td><td class='input'>{8}</td>
														<td class='label'>Remark: </td><td class='input' colspan=5>{9}</td>
													</tr>
													<tr>
														<td class='label'>Cust {1}CB{1} #: </td><td class='input'>{12}</td>
														<td class='label'>Cust {1}CS{1} #: </td><td class='input'>{23}</td>
														<td class='label'>Company: </td><td class='input' colspan=5>{10}</td>
													</tr>
													<tr>
														<td class='label'>Contact Name: </td><td class='input' colspan=3>{24}</td>
														<td class='label'>Contact Phone: </td><td class='input'>{25}</td>
													</tr>
													<tr>
														<td class='label'>Name: </td><td class='input' colspan=3>{13}&nbsp;{14}</td>
														<td class='label'>Phone: </td><td class='input'>{20}</td>
														<td class='label'>Country: </td><td class='input'>{19}</td>
													</tr>
													<tr>
														<td class='label'>Street: </td><td class='input'>{15}</td>
														<td class='label'>City: </td><td class='input'>{16}</td>
														<td class='label'>State: </td><td class='input'>{17}</td>
														<td class='label'>Zip: </td><td class='input'>{18}</td>
													</tr>
													<tr>
														<td class='label' style='vertical-align:text-top;'>Notes:<br>{22}</td>
														<td class='input' colspan=7><textarea id=notesUS_{0} rows=3 cols=80 onchange='updateIt(this);'>{21}</textarea></td>
													</tr>
													<tr>
														<td class='label' colspan=8><a href='#' onclick={1}viewHistory({0},'USTrans');{1}>View History</a></td>
													</tr>
												</table>
											</div>
											"
											, i, "\""
											, mUSTransDV[i]["credit_card_suffix"].ToString()
											, mUSTransDV[i]["exp"].ToString(), mUSTransDV[i]["paystatus"].ToString()
											, string.Format("{0:c}",mUSTransDV[i]["gross"])
											, string.Format("{0:c}",mUSTransDV[i]["openamount"])
											, mUSTransDV[i]["rpodoc"].ToString()
											, mUSTransDV[i]["invoice_date"].ToString(), mUSTransDV[i]["remark"].ToString()
											, mUSTransDV[i]["company"].ToString()
											, mUSTransDV[i]["maint_date"].ToString()
											, mUSTransDV[i]["cb"].ToString()
											, mUSTransDV[i]["card_first_name"].ToString()
											, mUSTransDV[i]["card_last_name"].ToString()
											, mUSTransDV[i]["card_street"].ToString()
											, mUSTransDV[i]["card_city"].ToString()
											, mUSTransDV[i]["card_state"].ToString()
											, mUSTransDV[i]["card_zip"].ToString()
											, mUSTransDV[i]["card_country"].ToString()
											, mUSTransDV[i]["card_phone"].ToString()
											, mUSTransDV[i]["notes"].ToString()
											, resizeSelect("notesUS", i)
											, mUSTransDV[i]["cs"]
											, mUSTransDV[i]["contact_name"]
											, mUSTransDV[i]["contact_phone"]
											);

					}
					mRetVal.usHTML += string.Format(@"
												</div>
												<div class='scrollFoot'>	
													<div class='scrollTransFootLabel'>Total Amount: </div>
													<div class='scrollFootSpace'>
													<div class='scrollAmt'>{0}</div>
												</div>	
											</div>
										    "
											, string.Format("{0:c}", totalAmount)
										   );

					#endregion
				}

				if (theLoc == "CA" || theLoc == "ALL")
				{

					#region load ca trans

					mRetVal.caEmail = new string[mCATransDV.Count];

					mRetVal.caHTML = string.Format(@"
								<div style='font-weight:bold;float:left;'>{1} {2}</div>
								<div class='transTable' >
									<div class='printHelper'>
										<div class='scrollHead' >
											<div class='headCK'><input type=checkbox name=selectAllCA onclick={0}toggleCB(this.checked,document.mForm.selectRowCA,'CA'){0}></div>	
											<div class='scrollHeadID'><a href='#' onclick={0}sortTransCA('id');return false;{0}>ID<span></span></a></div>
											<div class='scrollType'><a href='#' onclick={0}sortTransCA('type_name');return false;{0}>Type<span></span></a></div>
											<div class='scrollOrder'><a href='#' onclick={0}sortTransCA('order_skey');return false;{0}>Order #<span></span></a></div>
											<div class='scrollDCT'><a href='#' onclick={0}sortTransCA('dct');return false;{0}>Dct<span></span></a></div>
											<div class='scrollDOC'><a href='#' onclick={0}sortTransCA('doc');return false;{0}>Doc #<span></span></a></div>
											<div class='scrollKCO'><a href='#' onclick={0}sortTransCA('kco');return false;{0}>Company<span></span></a></div>
											<div class='scrollEmail'><a href='#' onclick={0}sortTransCA('email');return false;{0}>Email<span></span></a></div>
											<div class='scrollAmt'><a href='#' onclick={0}sortTransCA('amount');return false;{0}>Amount<span></span></a></div>
											<div class='scrollReason'><a href='#' onclick={0}sortTransCA('reason_code');return false;{0}>Reason<span></span></a></div>
											<div class='scrollCardType'><a href='#' onclick={0}sortTransCA('credit_card_no');return false;{0}>Card Type<span></span></a></div>
											<div class='scrollNotes'><a href='#' onclick={0}sortTransCA('notes_flag');return false;{0}>Notes<span></span></a></div>
											<div class='headStatus'><a href='#' onclick={0}sortTransCA('status_name');return false;{0}>Status<span></span></a></div>
										</div>
									</div>
									<div class=ScrollTable>
								", "\"",mCATransDV.Count,(mCATransDV.Count > 1)?"Results":"Result");

					totalAmount = 0;
					for (int i = 0; i < mCATransDV.Count; i++)
					{
						if (mCATransDV[i]["amount"].ToString() == "-0.01")
						{
							mCATransDV[i]["amount"] = mCATransDV[i]["openamount"];
						}
						totalAmount += Convert.ToDouble(mCATransDV[i]["amount"]);

						mRetVal.caEmail[i] = mCATransDV[i]["email"].ToString();

						if (mCATransDV[i]["exp"].ToString().Length > 3)
						{
							expMonth = Convert.ToInt32(mCATransDV[i]["exp"].ToString().Substring(0, 2));
							expYear = Convert.ToInt32("20" + mCATransDV[i]["exp"].ToString().Substring(3));
						}
						else
						{
							expMonth = -1;
							expYear = -1;
						}

						mRetVal.caHTML += string.Format(@"
										<div class='scrollRow' id='mScrollRowCA_{0}'>
											<div id='ckTDCA_{0}' class='rowCK' style='{13}'><input id='selectRowCA_{0}' {15} {14} type=checkbox name=selectRowCA onclick='updateIt(this);'></div>
											<div id='idTDCA_{0}' class='scrollID' style='{13}'>&nbsp;{11}</div>
											<div id='typeTDCA_{0}' class='scrollType' style='{13}'>&nbsp;<a onclick={1}displayRow({0},'CA'){1} onmouseover={1}window.status='action Details';return true;{1} onmouseout={1}window.status='';return true;{1}>{2}</a></div>
											<div id='keyTDCA_{0}' class='scrollOrder' style='{13}'>&nbsp;{3}</div>
											<div id='dctTDCA_{0}' class='scrollDCT' style='{13}'>&nbsp;{4}</div>
											<div id='docTDCA_{0}' class='scrollDOC' style='{13}'>&nbsp;{5}</div>
											<div id='kcoTDCA_{0}'  class='scrollKCO' style='{13}'>&nbsp;{6}</div>
											<div id='emailTDCA_{0}' class='scrollEmail' style='{13}'>&nbsp;<input class='flat' id=emailCA_{0} value='{7}' type=text size=25 onchange='updateIt(this);'></div>
											<div id='amountTDCA_{0}' class='scrollAmt' style='{13}'>&nbsp;{12}</div>
											<div id='reasonTDCA_{0}'  class='scrollReason' style='{13}'>&nbsp;{10}</div>
											<div id='cardTypeTDCA_{0}'  class='scrollCardType' style='{13}'>&nbsp;{17}</div>
											<div id='notesTDCA_{0}'  class='scrollNotes' style='{13}'>&nbsp;{16}</div>
											<div id='statusTDCA_{0}' class='scrollStatus' style='{13}'>&nbsp;<select style='width:130px;' {8} id='statusCA_{0}' onchange='updateIt(this);'>{9}</select></div>
										</div>
										"
										, i, "\""
										, mCATransDV[i]["type_name"], mCATransDV[i]["order_skey"].ToString()
										, mCATransDV[i]["dct"].ToString()
										, mCATransDV[i]["doc"].ToString()
										, mCATransDV[i]["kco"].ToString()
										, mCATransDV[i]["email"].ToString()
										, (mCATransDV[i]["checked"].ToString() == "checked") ? "disabled" : ""
										, thePage.Session["gStatusOptCA_" + mCATransDV[i]["status_id"]]
										, mCATransDV[i]["reason_code"].ToString(), mCATransDV[i]["id"]
										, string.Format("{0:c}",mCATransDV[i]["amount"])
										, (expYear < DateTime.Now.Year ||
											(expYear == DateTime.Now.Year && expMonth < DateTime.Now.Month)) ? "background: #f3f3f3;" : ""
										, mCATransDV[i]["checked"]
										,(mCATransDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_UNSENT"].ToString() ||
											mCATransDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_DECLINED"].ToString()) ? "" : "disabled"
										, mCATransDV[i]["notes_flag"]
										, getCCType(mCATransDV[i]["credit_card_no"].ToString())
										);

						mRetVal.caHTML += string.Format(@"
											<div class='hiddenNotes'>
												Notes: {0}
											</div>
											"
											, mCATransDV[i]["notes"].ToString()
											);


						mRetVal.caHTML += string.Format(@"	
										<div id='mHiddenRowCA_{0}' class='transDetailRowDiv' style='display:none;'>
											<table class='detailRow'>
												<tr>
													<td class='label'>Credit Card #: </td><td class='input'>************&nbsp;{2}</td>
													<td class='label'>Exp Date: </td><td class='input'>{3}</td>
													<td class='label'>Pay Status: </td><td class='input' colspan=3>{4}</td>
										
												</tr>
												<tr>
													<td class='label'>Maint Date: </td><td class='input'>{11}</td>
													<td class='label'>Gross: </td><td class='input'>{5}</td>
													<td class='label'>Open: </td><td class='input'>{6}</td>
													<td class='label'>ODOC: </td><td class='input'>{7}</td>
												</tr>
												<tr>
													<td class='label'>Invoice Date: </td><td class='input'>{8}</td>
													<td class='label'>Remark: </td><td class='input' colspan=5>{9}</td>
												</tr>
												<tr>
													<td class='label'>Cust {1}CB{1} #: </td><td class='input'>{12}</td>
													<td class='label'>Cust {1}CS{1} #: </td><td class='input'>{23}</td>
													<td class='label'>Company: </td><td class='input' colspan=5>{10}</td>
												</tr>
												<tr>
													<td class='label'>Contact Name: </td><td class='input' colspan=3>{24}</td>
													<td class='label'>Contact Phone: </td><td class='input'>{25}</td>
												</tr>
												<tr>
													<td class='label'>Name: </td><td class='input' colspan=3>{13}&nbsp;{14}</td>
													<td class='label'>Phone: </td><td class='input'>{20}</td>
													<td class='label'>Country: </td><td class='input'>{19}</td>
												</tr>
												<tr>
													<td class='label'>Street: </td><td class='input'>{15}</td>
													<td class='label'>City: </td><td class='input'>{16}</td>
													<td class='label'>State: </td><td class='input'>{17}</td>
													<td class='label'>Zip: </td><td class='input'>{18}</td>
												</tr>
												<tr>
													<td class='label' style='vertical-align:text-top;'>Notes:<br>{22}</td>
													<td class='input' colspan=7><textarea id=notesCA_{0} rows=3 cols=80 onchange='updateIt(this);'>{21}</textarea></td>
												</tr>
												<tr>
													<td class='label' colspan=8><a href='#' onclick={1}viewHistory({0},'CATrans');{1}>View History</a></td>
												</tr>
											</table>
										</div>
										"
										, i, "\""
										, mCATransDV[i]["credit_card_suffix"].ToString()
										, mCATransDV[i]["exp"].ToString(), mCATransDV[i]["paystatus"].ToString()
										, string.Format("{0:c}",mCATransDV[i]["gross"])
										, string.Format("{0:c}",mCATransDV[i]["openamount"])
										, mCATransDV[i]["rpodoc"].ToString()
										, mCATransDV[i]["invoice_date"].ToString(), mCATransDV[i]["remark"].ToString()
										, mCATransDV[i]["company"].ToString()
										, mCATransDV[i]["maint_date"].ToString()
										, mCATransDV[i]["cb"].ToString()
										, mCATransDV[i]["card_first_name"].ToString()
										, mCATransDV[i]["card_last_name"].ToString()
										, mCATransDV[i]["card_street"].ToString()
										, mCATransDV[i]["card_city"].ToString()
										, mCATransDV[i]["card_state"].ToString()
										, mCATransDV[i]["card_zip"].ToString()
										, mCATransDV[i]["card_country"].ToString()
										, mCATransDV[i]["card_phone"].ToString()
										, mCATransDV[i]["notes"].ToString()
										, resizeSelect("notesCA", i)
										, mCATransDV[i]["cs"].ToString()
										, mCATransDV[i]["contact_name"].ToString()
										, mCATransDV[i]["contact_phone"].ToString()
										);

					}

					mRetVal.caHTML += string.Format(@"
												</div>
												<div class='scrollFoot'>	
													<div class='scrollTransFootLabel'>Total Amount: </div>
													<div class='scrollFootSpace'>
													<div class='scrollAmt'>{0}</div>
												</div>	
											</div>
										    "
											, string.Format("{0:c}", totalAmount)
										   );
					#endregion
				}
			}
			catch (Exception e)
			{
				mRetVal.error = true;

				logError("Common.cs","getTransData",aQuery,thePage.Session["gNTName"].ToString(),e.ToString(),"ERROR");
			}
			finally
			{
				aConn.Close();
			}


			return mRetVal;
		}

		
		/**
		 * getCCType
		 * 
		 * returns the type of a credit card number
		 **/
		public string getCCType(string theCCNum){
			return (theCCNum.Length < 2) ? "" :
					((theCCNum.Substring(0, 1) == "3") ? "Amex" :
					 ((theCCNum.Substring(0, 1) == "4") ? "Visa" :
					  ((theCCNum.Substring(0, 1) == "5") ? "MasterCard" : "")));
		}
		
		
		/**
		 * resizeSelect
		 * 
		 * 
		 **/
		private string resizeSelect(string theID, int theRow)
		{
			string retVal = string.Format(@"Resize Text Area:<br>
								<select onChange={1}document.getElementById('{2}_{0}').rows = this.value{1}>"
								, theRow, "\"", theID);
			for (int i = 1; i < 101; i++)
			{
				retVal += string.Format("<option {1} value='{0}'>{0}</option>", i, (i == 3) ? "selected" : "");
			}
			retVal += "</select>";

			return retVal;
		}

		/**
		 * fileSelect
		 * 
		 * returns a drop down menu of canada transactions the user can view
		 * theAllFiles determins if all files should be returned or just the last 2 weeks.
		 * doesn't log errors in the DB since no DB connection is made
		 **/
		public string fileSelect(string thePage, Boolean theAllFiles)
		{

			string retVal;
			try
			{
				string[] mFiles = Directory.GetFiles(ConfigurationManager.AppSettings["FILE_LOC"], thePage + "*");
				Array.Sort(mFiles);
				Array.Reverse(mFiles);

				retVal = string.Format(@"<select id='m{0}Files' onchange=""printPage('F',this.value)"">
								      <option value=''>Select a File</option>", thePage);

				DateTime mTwoWeeks = DateTime.Now.AddDays(-14);
				DateTime mFileDate;


				for (int i = 0; i < mFiles.Length; i++)
				{
					mFileDate = File.GetCreationTime(mFiles[i]);
					if (theAllFiles || mFileDate >= mTwoWeeks)
					{
						retVal += string.Format(@"<option value='{0}'>{1}</option>"
													, mFiles[i]
													, Path.GetFileName(mFiles[i])
													);
					}
				}
				retVal += string.Format(@"</select>&nbsp;&nbsp;&nbsp;All Files <input type='checkbox' {2} id='mAll{0}Files' onClick={1}getFiles(this.checked,'{0}'){1}>"
					, thePage, "\"", (theAllFiles) ? "checked" : "");
			}
			catch
			{
				retVal = "Error Selecting Files.";
			}
			return retVal;
		}


		/**
		 * getSearchSelects
		 * 
		 * gets drop downs for search menu
		 * also creates all status drop options used in app and saves to session
		 * */
		public string[] getSearchSelects(Page thePage)
		{
			string[] retVal = new string[3];
			DataView mStatusDV;
			DataView mTypeDV;

			string mSearchStatusDefault = (thePage.Session["gSearchStatus"] == null) ? "" : thePage.Session["gSearchStatus"].ToString();
			string mSearchTypeDefault = (thePage.Session["gSearchType"] == null) ? "" : thePage.Session["gSearchType"].ToString();
			string mCreditTypeDefault = (thePage.Session["gCreditType"] == null) ? "" : thePage.Session["gCreditType"].ToString();


			#region get status and type

			if (thePage.Session["gStatusDV"] == null)
			{
				getStatusType(thePage);
			}

			#endregion

			mStatusDV = (DataView)thePage.Session["gStatusDV"];
			mTypeDV = (DataView)thePage.Session["gTypeDV"];

			#region load status and type

			string statusDrop = "<select id='mSearchStatusSelect' style='width:130px;' ><option value=''>All Status</option>";
			string typeDrop = "<select id='mSearchTypeSelect' ><option value=''>All Types</option>";
			string creditTypeDrop = "<select id='mCreditTypeSelect' ><option value=''>All Types</option>";
			//clears the default status options
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_UNSENT"]] = "";
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_PROCESSING"]] = "";
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_COMPLETED"]] = "";
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_DECLINED"]] = "";
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_DELETED"]] = "";
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"]] = "";
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_CREDITED"]] = "";
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_FAILED"]] = "";
			thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_HOLD"]] = "";
			//clears the us options for hold, unsent, deleted, declined and declined deleted
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_UNSENT"]] = "";
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DELETED"]] = "";
			thePage.Session["gStatusOptHoldUnsent"] = "";
			thePage.Session["gStatusOptHoldDeclined"] = "";
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"]] = "";
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DECLINED"]] = "";

			for (int i = 0; i < mStatusDV.Count; i++)
			{
				statusDrop += string.Format("<option value='{0}' {2}>{1}</option>"
												  , mStatusDV[i]["status_id"]
												  , mStatusDV[i]["name"]
												  , (mSearchStatusDefault == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");

				#region create default status options 
				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_UNSENT"]] += string.Format("<option {2} value='{0}'>{1}</option>"
										   , mStatusDV[i]["status_id"]
										   , mStatusDV[i]["name"]
										   , (ConfigurationManager.AppSettings["STATUS_UNSENT"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");
				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_PROCESSING"]] += string.Format("<option {2} value='{0}'>{1}</option>"
									   , mStatusDV[i]["status_id"]
									   , mStatusDV[i]["name"]
									   , (ConfigurationManager.AppSettings["STATUS_PROCESSING"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");

				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_COMPLETED"]] += string.Format("<option {2} value='{0}'>{1}</option>"
									   , mStatusDV[i]["status_id"]
									   , mStatusDV[i]["name"]
									   , (ConfigurationManager.AppSettings["STATUS_COMPLETED"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");

				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_DECLINED"]] += string.Format("<option {2} value='{0}'>{1}</option>"
									   , mStatusDV[i]["status_id"]
									   , mStatusDV[i]["name"]
									   , (ConfigurationManager.AppSettings["STATUS_DECLINED"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");

				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_DELETED"]] += string.Format("<option {2} value='{0}'>{1}</option>"
									   , mStatusDV[i]["status_id"]
									   , mStatusDV[i]["name"]
									   , (ConfigurationManager.AppSettings["STATUS_DELETED"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");

				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"]] += string.Format("<option {2} value='{0}'>{1}</option>"
									   , mStatusDV[i]["status_id"]
									   , mStatusDV[i]["name"]
									   , (ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");

				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_CREDITED"]] += string.Format("<option {2} value='{0}'>{1}</option>"
									   , mStatusDV[i]["status_id"]
									   , mStatusDV[i]["name"]
									   , (ConfigurationManager.AppSettings["STATUS_CREDITED"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");

				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_FAILED"]] += string.Format("<option {2} value='{0}'>{1}</option>"
									   , mStatusDV[i]["status_id"]
									   , mStatusDV[i]["name"]
									   , (ConfigurationManager.AppSettings["STATUS_FAILED"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");

				thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_HOLD"]] += string.Format("<option {2} value='{0}'>{1}</option>"
									   , mStatusDV[i]["status_id"]
									   , mStatusDV[i]["name"]
									   , (ConfigurationManager.AppSettings["STATUS_HOLD"] == mStatusDV[i]["status_id"].ToString()) ? "selected" : "");
				#endregion

				#region create us options for hold, unsent, deleted, declined, and declined deleted
				if (mStatusDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_UNSENT"])
				{
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_UNSENT"]] += string.Format(@"<option selected value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DELETED"]] += string.Format(@"<option value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptHoldUnsent"] += string.Format(@"<option value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
				}
				else if (mStatusDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_DELETED"])
				{
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DELETED"]] += string.Format(@"<option selected value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_UNSENT"]] += string.Format(@"<option value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
				}
				else if (mStatusDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_DECLINED"])
				{
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DECLINED"]] += string.Format(@"<option selected value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"]] += string.Format(@"<option value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptHoldDeclined"] += string.Format(@"<option value={0}>{1}</option>"
							, mStatusDV[i]["status_id"]
							, mStatusDV[i]["name"]
							);
				}
				else if (mStatusDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"])
				{
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DECLINED_DELETED"]] += string.Format(@"<option selected value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DECLINED"]] += string.Format(@"<option value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
				}
				else if (mStatusDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_HOLD"])
				{
					thePage.Session["gStatusOptHoldUnsent"] += string.Format(@"<option selected value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptHoldDeclined"] += string.Format(@"<option selected value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_UNSENT"]] += string.Format(@"<option value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
					thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_DECLINED"]] += string.Format(@"<option value={0}>{1}</option>"
												, mStatusDV[i]["status_id"]
												, mStatusDV[i]["name"]
												);
				}
				#endregion

			}
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_PROCESSING"]] = thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_PROCESSING"]];
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_COMPLETED"]] = thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_COMPLETED"]];
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_CREDITED"]] = thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_CREDITED"]];
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_FAILED"]] = thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_FAILED"]];
			thePage.Session["gStatusOptUS_" + ConfigurationManager.AppSettings["STATUS_HOLD"]] = thePage.Session["gStatusOptCA_" + ConfigurationManager.AppSettings["STATUS_HOLD"]];

			statusDrop += "</select>";


			for (int i = 0; i < mTypeDV.Count; i++)
			{
				typeDrop += string.Format("<option value='{0}' {2}>{1}</option>"
										   , mTypeDV[i]["type_id"]
										   , mTypeDV[i]["name"]
										   , (mSearchTypeDefault == mTypeDV[i]["type_id"].ToString()) ? "selected" : "");

				creditTypeDrop += string.Format("<option value='{0}' {2}>{1}</option>"
										   , mTypeDV[i]["type_id"]
										   , mTypeDV[i]["name"]
										   , (mCreditTypeDefault == mTypeDV[i]["type_id"].ToString()) ? "selected" : "");
			}
			typeDrop += "</select> ";
			creditTypeDrop += "</select> ";

			#endregion

			retVal[0] = statusDrop;
			retVal[1] = typeDrop;
			retVal[2] = creditTypeDrop;

			return retVal;
		}


		/**
		 * getStatusType
		 * 
		 * queries the db for all cct status and types, saves results to session
		 * */
		private void getStatusType(Page thePage)
		{
			DataTable mHold;
			DataView mStatusDV;
			DataView mTypeDV;
			WSDBConnection aConn = new WSDBConnection();

			try
			{
				aConn.Open("SISR", 0);
				//maybe add the desc back in later with a help box
				string aQuery = string.Format(@"
						SELECT s.status_id
							  ,'' type_id
							  ,s.status_name NAME
						  FROM cct_status s
						UNION
						SELECT NULL
							  ,t.type_id
							  ,t.type_name
						  FROM cct_type t
						 ORDER BY name
					");

				mHold = aConn.GetDataTable(aQuery);
				aConn.Close();

				mStatusDV = new DataView(mHold);
				mTypeDV = new DataView(mHold);
				mStatusDV.RowFilter = "status_id is not null";
				mTypeDV.RowFilter = "type_id is not null";

				thePage.Session["gStatusDV"] = mStatusDV;
				thePage.Session["gTypeDV"] = mTypeDV;
			}
			catch (Exception e)
			{
				logError("Common.cs","getStatusType","",thePage.Session["gNTName"].ToString(),e.ToString(),"ERROR");
			}
			finally
			{
				aConn.Close();
			}
		}


		/**
		 * search
		 * 
		 * Searchs credit card trans based on prams set on the session
		 * returns us and ca table of trans found
		 * tables include an editable status (only for Recurring Deleted/Declined_Deleted trans in the case of US)
		 * and editable notes.
		 * */
		public RetStruct search(Page thePage, string theLoc, bool requery)
		{
			DataView mStatusDV;
			DataView mUSSearchDV = null;
			DataView mCASearchDV = null;
			DataView mHoldDV = null;
			DataTable mHoldDT;
			RetStruct mRetVal = new RetStruct();
			WSDBConnection aConn = new WSDBConnection();	
			string aQuery = "";
			
			try
			{
				aConn.Open("CCKEY", 0);
				if (requery)
				{
					#region set search parms

					string mTransID = (thePage.Session["gSearchTransID"] == null || thePage.Session["gSearchTransID"].ToString() == "") ? "" : "AND t.id = " + thePage.Session["gSearchTransID"].ToString();
					string mCustNum = (thePage.Session["gSearchCustNum"] == null || thePage.Session["gSearchCustNum"].ToString() == "") ? "" : "AND ab.aban85 = " + thePage.Session["gSearchCustNum"].ToString();
					string mCustName = (thePage.Session["gSearchCustName"] == null || thePage.Session["gSearchCustName"].ToString() == "") ? "" : "AND rp.rpalph like '%" + thePage.Session["gSearchCustName"].ToString() + "%'";
					string mOrder = (thePage.Session["gSearchOrder"] == null || thePage.Session["gSearchOrder"].ToString() == "") ? "" : "AND t.order_skey = " + thePage.Session["gSearchOrder"].ToString();
					string mCSNum = (thePage.Session["gSearchCSNum"] == null || thePage.Session["gSearchCSNum"].ToString() == "") ? "" : "AND ooh.ship_an8 = " + thePage.Session["gSearchCSNum"].ToString();
					string mODOC = (thePage.Session["gSearchODOC"] == null || thePage.Session["gSearchODOC"].ToString() == "") ? "" : "AND rp.rpodoc = " + thePage.Session["gSearchODOC"].ToString();
					string mDCT = (thePage.Session["gSearchDCT"] == null || thePage.Session["gSearchDCT"].ToString() == "") ? "" : "AND t.dct = '" + thePage.Session["gSearchDCT"].ToString() + "'";
					string mDOC = (thePage.Session["gSearchDOC"] == null || thePage.Session["gSearchDOC"].ToString() == "") ? "" : "AND t.doc = " + thePage.Session["gSearchDOC"].ToString();
					string mKCO = (thePage.Session["gSearchKCO"] == null || thePage.Session["gSearchKCO"].ToString() == "") ? "" : "AND t.kco = '" + thePage.Session["gSearchKCO"].ToString() + "'";
					string mCCSuffix = (thePage.Session["gSearchCCSufix"] == null || thePage.Session["gSearchCCSufix"].ToString() == "") ? "" : "AND ooh.credit_card_suffix = " + thePage.Session["gSearchCCSufix"].ToString();

					string mSentFrom = (thePage.Session["gSearchSentFrom"] == null || thePage.Session["gSearchSentFrom"].ToString() == "") ? "" : "to_date('" + thePage.Session["gSearchSentFrom"].ToString() + " 00:00:00','mm/dd/yyyy HH24:MI:SS')";
					string mSentTo = (thePage.Session["gSearchSentTo"] == null || thePage.Session["gSearchSentTo"].ToString() == "") ? "" : "to_date('" + thePage.Session["gSearchSentTo"].ToString() + " 23:59:59','mm/dd/yyyy HH24:MI:SS')";
					string mSentDate = "";
					if (mSentFrom != "" && mSentTo != "")
					{
						mSentDate = "AND t.maint_date between " + mSentFrom + " AND " + mSentTo;
					}
					else if (mSentFrom != "")
					{
						mSentDate = "AND t.maint_date > " + mSentFrom;
					}
					else if (mSentTo != "")
					{
						mSentDate = "AND t.maint_date < " + mSentTo;
					}

					string mStatus = (thePage.Session["gSearchStatus"] == null || thePage.Session["gSearchStatus"].ToString() == "") ? "" : "AND t.status_id = " + thePage.Session["gSearchStatus"].ToString();
					string mType = (thePage.Session["gSearchType"] == null || thePage.Session["gSearchType"].ToString() == "") ? "" : "AND t.type_id = '" + thePage.Session["gSearchType"].ToString() + "'";
					//unlike the rest this value is selected in the dv not the actual query
					string mCardType = (thePage.Session["gSearchCardType"] == null || thePage.Session["gSearchCardType"].ToString() == "") ? "" :
										(thePage.Session["gSearchCardType"].ToString() == "54") ? "AND (credit_card_no like '5%' OR credit_card_no like '4%')" : "AND credit_card_no like '" + thePage.Session["gSearchCardType"].ToString() + "%'";


					string mLoc;
					if (theLoc == "US")
					{
						mLoc = "AND t.kco IN ('00010','00090')";
					}
					else if (theLoc == "CA")
					{
						mLoc = "AND t.kco = '00020'";
					}
					else
					{
						mLoc = "AND t.kco IN ('00010','00090','00020')";
					}
					#endregion

					#region search


					if (thePage.Session["gStatusDV"] == null)
					{
						getStatusType(thePage);
					}

					//maybe add the desc back in later with a help box
					aQuery = string.Format(@"
					SELECT * FROM (
						SELECT /*+ Rule */ t.id
							  ,t.type_id
							  ,y.type_name
							  ,t.order_skey
							  ,t.dct
							  ,t.doc
							  ,t.kco
							  ,t.status_id
							  ,(nvl(t.amount
								   ,-1) * .01) amount
							  ,t.email
							  ,t.maint_date
							  ,t.reason_code
							  ,t.notes
							  ,decode(t.notes,null,'','*') notes_flag
							  ,i.contact_name
							  ,i.contact_phone
							  ,i.card_first_name
							  ,i.card_last_name
							  ,i.card_street
							  ,i.card_city
							  ,i.card_state
							  ,i.card_zip
							  ,i.card_country
							  ,i.card_phone
							  ,CASE
								 WHEN k.key IS NOT NULL
									  AND ooh.credit_card_crypt IS NOT NULL THEN
								  TRIM(pkg_cc_crypt.decrypt(ooh.credit_card_crypt
														   ,(k.key)))
								 ELSE
								  ' '
							   END credit_card_no
							  ,nvl(ooh.credit_card_suffix,t.cc_suffix) AS credit_card_suffix
							  ,decode(ooh.credit_card_suffix
									 ,NULL
									 ,' '
									 ,substr(ooh.credit_card_expiration_date
											,0
											,2) || '/' || substr(ooh.credit_card_expiration_date
																,3)) exp
							  ,(nvl(SUM(rpag)
								   ,0) * .01) gross
							  ,(nvl(SUM(rpaap)
								   ,0) * .01) openamount
							  ,rp.rpodoc
							  ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
									  ,'MM/DD/YY') AS invoice_date
							  ,rp.rprmk remark
							  ,min(decode(rp.rppst
									 ,NULL
									 ,''
									 ,rp.rppst || ' - ' || dr.drdl01)) paystatus
							  ,rp.rpalph company
							  ,ab.aban85 cb
							  ,ooh.ship_an8 cs
							  ,s.status_name
							  FROM cct_trans        t
								  ,cct_order_info   i
								  ,omb_order_header ooh
								  ,f03b11           rp
								  ,f0101		    ab
								  ,cc_keys          k
								  ,cct_type         y
								  ,f0005            dr
								  ,sisr.cct_status       s
							 WHERE t.dct = rp.rpdct
							   AND t.doc = rp.rpdoc
							   AND t.kco = rp.rpkco				   
							   AND t.order_skey = ooh.order_skey
							   AND ooh.order_skey = k.order_skey(+)
							   AND t.type_id = y.type_id
							   AND ab.aban8 = rp.rpan8
							   AND dr.drsy(+) = '00  ' 
							   AND dr.drrt(+) = 'PS'
							   AND trim(dr.drky(+)) = rppst
							   AND i.order_skey(+) = t.order_skey
                               AND t.status_id = s.status_id
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
							 GROUP BY t.id
									 ,t.type_id
									 ,y.type_name
									 ,t.order_skey
									 ,t.dct
									 ,t.doc
									 ,t.kco
									 ,t.status_id
									 ,t.amount
									 ,t.email
									 ,t.maint_date
									 ,t.reason_code
                                     ,t.notes
									 ,i.contact_name
									 ,i.contact_phone
									 ,i.card_first_name
									 ,i.card_last_name
									 ,i.card_street
									 ,i.card_city
									 ,i.card_state
									 ,i.card_zip
									 ,i.card_country
									 ,i.card_phone
									 ,CASE
										 WHEN k.key IS NOT NULL
											  AND ooh.credit_card_crypt IS NOT NULL THEN
										  TRIM(pkg_cc_crypt.decrypt(ooh.credit_card_crypt
																   ,(k.key)))
										 ELSE
										  ' '
									   END
									  ,nvl(ooh.credit_card_suffix,t.cc_suffix)
									  ,decode(ooh.credit_card_suffix
											 ,NULL
											 ,' '
											 ,substr(ooh.credit_card_expiration_date
													,0
													,2) || '/' || substr(ooh.credit_card_expiration_date
																		,3)) 
									 ,rp.rpodoc
									 ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
											 ,'MM/DD/YY')
									 ,rp.rprmk
									 ,rp.rpalph
									 ,ab.aban85
									 ,ooh.ship_an8
									 ,s.status_name
							 ORDER BY t.type_id
									 ,t.order_skey
									 ,t.dct
									 ,t.doc)
					WHERE ROWNUM <= {14}
							"
                                , mTransID
								, mCustNum
								, mCustName
								, mOrder
								, mDCT
								, mDOC
								, mKCO
								, mCCSuffix
								, mSentDate
								, mStatus
								, mType
								, mLoc
								, mCSNum
								, mODOC
								, gMaxRows
								);


					mHoldDT = aConn.GetDataTable(aQuery);
					//holds 
					mHoldDT.Columns.Add("update", System.Type.GetType("System.String"));
					
					#region select hold old hold status to determine which select box to use
					mHoldDV = new DataView(mHoldDT);
					mHoldDV.RowFilter = string.Format("status_id = {0}",ConfigurationManager.AppSettings["STATUS_HOLD"]);

					if (mHoldDV.Count > 0)
					{
						string holdIDs = " ";
						for (int i = 0; i < mHoldDV.Count; i++)
						{
							holdIDs += mHoldDV[i]["id"] + ",";
						}
						holdIDs = holdIDs.Substring(0,holdIDs.Length-1);

						aQuery = string.Format(@"
												SELECT r.old_value 
													  ,k.VALUE
												  FROM sisr.cct_audit_keys   k
													  ,sisr.cct_audit_record r
												 WHERE k.audit_key = r.audit_key
												   AND k.audit_key = (SELECT MAX(sk.audit_key) akey
												                      
																		FROM sisr.cct_audit_keys   sk
																			,sisr.cct_audit_record sr
																	   WHERE sk.audit_key = sr.audit_key
																		 AND sr.column_name = 'STATUS_ID'
																		 AND sk.VALUE = k.VALUE)
												   AND k.column_name = 'id'
												   AND r.column_name = 'STATUS_ID'
												   AND k.VALUE IN ({0})"
												, holdIDs);
						mHoldDV = (aConn.GetDataTable(aQuery)).DefaultView;
						thePage.Session["gHoldDV"] = mHoldDV;
					}
					#endregion


					if (theLoc == "US" || theLoc == "ALL")
					{
						mUSSearchDV = new DataView(mHoldDT);
						mUSSearchDV.RowFilter = string.Format("kco in ('00010','00090') {0}",mCardType);
						thePage.Session["gUSSearchDV"] = mUSSearchDV;
					}

					if (theLoc == "CA" || theLoc == "ALL")
					{
						mCASearchDV = new DataView(mHoldDT);
						mCASearchDV.RowFilter = string.Format("kco = '00020' {0}",mCardType);
						thePage.Session["gCASearchDV"] = mCASearchDV;
					}
					#endregion
				}
				else
				{
					mHoldDV     = (DataView)thePage.Session["gHoldDV"];
					mUSSearchDV = (DataView)thePage.Session["gUSSearchDV"];
					mCASearchDV = (DataView)thePage.Session["gCASearchDV"];
				}

				mStatusDV = (DataView)thePage.Session["gStatusDV"];

				string mDrop = "";
				double totalAmount;
				int expMonth;
				int expYear;
				

				if (theLoc == "US" || theLoc == "ALL")
				{
					#region load us search

					mRetVal.usEmail = new string[mUSSearchDV.Count];

					mRetVal.usHTML = string.Format(@"
								<div style='font-weight:bold;float:left;'>{1} {2} {3}</div>

								<div class='searchTable'>
									<div class='printHelper'>
										<div class='scrollHead' >
												<div class='headID'><a href='#' onclick={0}sortSearchUS('id');return false;{0}>ID<span></span></a></div>
												<div class='scrollType'><a href='#' onclick={0}sortSearchUS('type_name');return false;{0}>Type<span></span></a></div>
												<div class='scrollOrder'><a href='#' onclick={0}sortSearchUS('order_skey');return false;{0}>Order #<span></span></a></div>
												<div class='scrollDCT'><a href='#' onclick={0}sortSearchUS('dct');return false;{0}>Dct<span></span></a></div>
												<div class='scrollDOC'><a href='#' onclick={0}sortSearchUS('doc');return false;{0}>Doc #<span></span></a></div>
												<div class='scrollKCO'><a href='#' onclick={0}sortSearchUS('kco');return false;{0}>Company<span></span></a></div>
												<div class='scrollEmail'><a href='#' onclick={0}sortSearchUS('email');return false;{0}>Email<span></span></a></div>
												<div class='scrollAmt'><a href='#' onclick={0}sortSearchUS('amount');return false;{0}>Amount<span></span></a></div>
												<div class='scrollReason'><a href='#' onclick={0}sortSearchUS('reason_code');return false;{0}>Reason<span></span></a></div>
												<div class='scrollCardType'><a href='#' onclick={0}sortSearchUS('credit_card_no');return false;{0}>Card Type<span></span></a></div>
												<div class='scrollNotes'><a href='#' onclick={0}sortSearchUS('notes_flag');return false;{0}>Notes<span></span></a></div>
												<div class='headStatus'><a href='#' onclick={0}sortSearchUS('status_name');return false;{0}>Status<span></span></a></div>
										</div>
									</div>	
								<div class=ScrollTable>
									", "\""
									 , mUSSearchDV.Count
									 , mUSSearchDV.Count > 1 ? "Results":"Result"
									 , mUSSearchDV.Count + (mCASearchDV == null? 0:mCASearchDV.Count) == gMaxRows && mUSSearchDV.Count > 1 ? 
												"  - There were more than " + mUSSearchDV.Count.ToString() + 
												" results to your search.  Only the first "+ mUSSearchDV.Count.ToString() + 
												" are displayed." : ""
									);

					totalAmount = 0;
					for (int i = 0; i < mUSSearchDV.Count; i++)
					{
						if (mUSSearchDV[i]["amount"].ToString() == "-0.01")
						{
							mUSSearchDV[i]["amount"] = mUSSearchDV[i]["openamount"];
						}

						totalAmount += Convert.ToDouble(mUSSearchDV[i]["amount"]);

						mRetVal.usEmail[i] = mUSSearchDV[i]["email"].ToString();

						if (mUSSearchDV[i]["exp"].ToString().Length > 3)
						{
							expMonth = Convert.ToInt32(mUSSearchDV[i]["exp"].ToString().Substring(0, 2));
							expYear = Convert.ToInt32("20" + mUSSearchDV[i]["exp"].ToString().Substring(3));
						}
						else
						{
							expMonth = -1;
							expYear = -1;
						}

						//selects the drop down needed from the premade drops
						//special case is to pick which drop for a hold status
						if (mUSSearchDV[i]["status_id"].ToString() != ConfigurationManager.AppSettings["STATUS_HOLD"].ToString()) {
							mDrop = thePage.Session["gStatusOptUS_" + mUSSearchDV[i]["status_id"]].ToString();
						}
						else{
							mHoldDV.RowFilter = string.Format("value={0}", mUSSearchDV[i]["id"]);
							if (mHoldDV.Count > 0)
							{
								if (mHoldDV[0]["old_value"].ToString() == ConfigurationManager.AppSettings["STATUS_DECLINED"].ToString())
								{
									mDrop = thePage.Session["gStatusOptHoldDeclined"].ToString();
								}
								else
								{
									mDrop = thePage.Session["gStatusOptHoldUnsent"].ToString();
								}
							}
							else
							{
								//if this happens then something when wrong with getting the old status 
								mDrop = thePage.Session["gStatusOptUS_" + mUSSearchDV[i]["status_id"]].ToString();
							}
						}


						mRetVal.usHTML += string.Format(@"
								<div class='scrollRow' id='mSearchScrollRowUS_{0}'>
									<div id='idSearchTDUS_{0}' class='rowID' style='{12}'>&nbsp;{10}</div>
									<div id='typeSearchTDUS_{0}' class='scrollType' style='{12}'>&nbsp;<a onclick={1}displaySearchRow({0},'US'){1} onmouseover={1}window.status='Transaction Details';return true;{1} onmouseout={1}window.status='';return true;{1}>{2}</a></div>
									<div id='keySearchTDUS_{0}' class='scrollOrder' style='{12}'>&nbsp;{3}</div>
									<div id='dctSearchTDUS_{0}' class='scrollDCT' style='{12}'>&nbsp;{4}</div>
									<div id='docSearchTDUS_{0}' class='scrollDOC' style='{12}'>&nbsp;{5}</div>
									<div id='kcoSearchTDUS_{0}'  class='scrollKCO' style='{12}'>&nbsp;{6}</div>
									<div id='emailSearchTDUS_{0}' class='scrollEmail' style='{12}'>&nbsp;<input class='flat' id=emailSearchUS_{0} value='{7}' type=text size=25 onchange='updateSearch(this);'></div>
									<div id='amountSearchTDUS_{0}' class='scrollAmt' style='{12}'>&nbsp;{11}</div>
									<div id='reasonSearchTDUS_{0}'  class='scrollReason' style='{12}'>&nbsp;{9}</div>
									<div id='cardTypeSearchTDUS_{0}'  class='scrollCardType' style='{12}'>&nbsp;{13}</div>
									<div id='notesSearchTDUS_{0}'  class='scrollNotes' style='{12}'>&nbsp;{15}</div>
									<div id='statusSearchTDUS_{0}' class='scrollStatus' style='{12}'>&nbsp;<select style='width:130px;' {14} id='statusSearchUS_{0}' onchange='updateSearch(this);'>{8}</select></div>
								</div>
							"
							, i
							, "\""
							, mUSSearchDV[i]["type_name"]
							, mUSSearchDV[i]["order_skey"].ToString()
							, mUSSearchDV[i]["dct"].ToString()
							, mUSSearchDV[i]["doc"].ToString()
							, mUSSearchDV[i]["kco"].ToString()
							, mUSSearchDV[i]["email"].ToString()
							, mDrop 
							, mUSSearchDV[i]["reason_code"].ToString()
							, mUSSearchDV[i]["id"]
							, string.Format("{0:c}", mUSSearchDV[i]["amount"])
							, (expYear < DateTime.Now.Year ||
							  (expYear == DateTime.Now.Year && expMonth < DateTime.Now.Month)) ? "background: #f3f3f3;" : ""
							, getCCType(mUSSearchDV[i]["credit_card_no"].ToString())
							, (mUSSearchDV[i]["type_id"].ToString() != ConfigurationManager.AppSettings["TYPE_RECURRING"] ||
							   mUSSearchDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_PROCESSING"] ||
							   mUSSearchDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_COMPLETED"] ||
							   mUSSearchDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_CREDITED"] ||
							   mUSSearchDV[i]["status_id"].ToString() == ConfigurationManager.AppSettings["STATUS_FAILED"]) ? "disabled='true'":""
						    , mUSSearchDV[i]["notes_flag"]
							);

						mRetVal.usHTML += string.Format(@"
											<div class='hiddenNotes'>
												Notes: {0}
											</div>
											"
							, mUSSearchDV[i]["notes"].ToString()
							);

						mRetVal.usHTML += string.Format(@"
													<div id='mHiddenSearchRowUS_{0}' class='searchDetailRowDiv' style='display:none;'>
														<table class='detailRow'>
															<tr>
																<td class='label'>Credit Card #: </td><td class='input'>************&nbsp;{2}</td>
																<td class='label'>Exp Date: </td><td class='input'>{3}</td>
																<td class='label'>Pay Status: </td><td class='input' colspan=3>{4}</td>
															</tr>
															<tr>
																<td class='label'>Maint Date: </td><td class='input'>{11}</td>
																<td class='label'>Gross: </td><td class='input'>{5}</td>
																<td class='label'>Open: </td><td class='input'>{6}</td>
																<td class='label'>ODOC: </td><td class='input'>{7}</td>
															</tr>
															<tr>
																<td class='label'>Invoice Date: </td><td class='input'>{8}</td>
																<td class='label'>Remark: </td><td class='input' colspan=5>{9}</td>
															</tr>
															<tr>
																<td class='label'>Cust {1}CB{1} #: </td><td class='input'>{12}</td>
																<td class='label'>Cust {1}CS{1} #: </td><td class='input'>{23}</td>
																<td class='label'>Company: </td><td class='input' colspan=5>{10}</td>
															</tr>
															<tr>
																<td class='label'>Contact Name: </td><td class='input' colspan=3>{24}</td>
																<td class='label'>Contact Phone: </td><td class='input'>{25}</td>
															</tr>
															<tr>
																<td class='label'>Name: </td><td class='input' colspan=3>{13}&nbsp;{14}</td>
																<td class='label'>Phone: </td><td class='input'>{20}</td>
																<td class='label'>Country: </td><td class='input'>{19}</td>
															</tr>
															<tr>
																<td class='label'>Street: </td><td class='input'>{15}</td>
																<td class='label'>City: </td><td class='input'>{16}</td>
																<td class='label'>State: </td><td class='input'>{17}</td>
																<td class='label'>Zip: </td><td class='input'>{18}</td>
															</tr>
															<tr>
																<td class='label' style='vertical-align:text-top;'>Notes:<br>{22}</td>
																<td class='input' colspan=7><textarea id=notesSearchUS_{0} rows=3 cols=80 onchange='updateSearch(this);'>{21}</textarea></td>
															</tr>
															<tr>
																<td class='label' colspan=8><a href='#' onclick={1}viewHistory({0},'USSearch');{1}>View History</a></td>
															</tr>
														</table>
													</div>
												"
												, i
												, "\""
												, mUSSearchDV[i]["credit_card_suffix"].ToString()
												, mUSSearchDV[i]["exp"].ToString()
												, mUSSearchDV[i]["paystatus"].ToString()
												, string.Format("{0:c}", mUSSearchDV[i]["gross"])
												, string.Format("{0:c}", mUSSearchDV[i]["openamount"])
												, mUSSearchDV[i]["rpodoc"].ToString()
												, mUSSearchDV[i]["invoice_date"].ToString()
												, mUSSearchDV[i]["remark"].ToString()
												, mUSSearchDV[i]["company"].ToString()
												, mUSSearchDV[i]["maint_date"].ToString()
												, mUSSearchDV[i]["cb"].ToString()
												, mUSSearchDV[i]["card_first_name"].ToString()
												, mUSSearchDV[i]["card_last_name"].ToString()
												, mUSSearchDV[i]["card_street"].ToString()
												, mUSSearchDV[i]["card_city"].ToString()
												, mUSSearchDV[i]["card_state"].ToString()
												, mUSSearchDV[i]["card_zip"].ToString()
												, mUSSearchDV[i]["card_country"].ToString()
												, mUSSearchDV[i]["card_phone"].ToString()
												, mUSSearchDV[i]["notes"].ToString()
												, resizeSelect("notesSearchUS", i)
												, mUSSearchDV[i]["cs"].ToString()
												, mUSSearchDV[i]["contact_name"].ToString()
												, mUSSearchDV[i]["contact_phone"].ToString()
												);



					}
					mRetVal.usHTML += string.Format(@"
												</div>
												<div class='scrollFoot'>	
													<div class='scrollFootLabel'>Total Amount: </div>
													<div class='scrollAmt'>{0}</div>
												</div>	
											</div>
										"
										, string.Format("{0:c}", totalAmount)
									   );
					#endregion
				}

				if (theLoc == "CA" || theLoc == "ALL")
				{
					#region load ca search

					mRetVal.caEmail = new string[mCASearchDV.Count];

					mRetVal.caHTML = string.Format(@"
								<div style='font-weight:bold;float:left;'>{1} {2} {3}</div>
								<div class='searchTable' >
									<div class='printHelper'>
										<div class='scrollHead' >
												<div class='headID'><a href='#' onclick={0}sortSearchCA('id');return false;{0}>ID<span></span></a></div>
												<div class='scrollType'><a href='#' onclick={0}sortSearchCA('type_name');return false;{0}>Type<span></span></a></div>
												<div class='scrollOrder'><a href='#' onclick={0}sortSearchCA('order_skey');return false;{0}>Order #<span></span></a></div>
												<div class='scrollDCT'><a href='#' onclick={0}sortSearchCA('dct');return false;{0}>Dct<span></span></a></div>
												<div class='scrollDOC'><a href='#' onclick={0}sortSearchCA('doc');return false;{0}>Doc #<span></span></a></div>
												<div class='scrollKCO'><a href='#' onclick={0}sortSearchCA('kco');return false;{0}>Company<span></span></a></div>
												<div class='scrollEmail'><a href='#' onclick={0}sortSearchCA('email');return false;{0}>Email<span></span></a></div>
												<div class='scrollAmt'><a href='#' onclick={0}sortSearchCA('amount');return false;{0}>Amount<span></span></a></div>
												<div class='scrollReason'><a href='#' onclick={0}sortSearchCA('reason_code');return false;{0}>Reason<span></span></a></div>
												<div class='scrollCardType'><a href='#' onclick={0}sortSearchCA('credit_card_no');return false;{0}>Card Type<span></span></a></div>
												<div class='scrollNotes'><a href='#' onclick={0}sortSearchCA('notes_flag');return false;{0}>Notes<span></span></a></div>
												<div class='headStatus'><a href='#' onclick={0}sortSearchCA('status_name');return false;{0}>Status<span></span></a></div>
										</div>
									</div>
									<div class=ScrollTable>
									", "\""
									 , mCASearchDV.Count
									 ,(mCASearchDV.Count > 1)?"Results":"Result"
									 , (mUSSearchDV == null ? 0 : mUSSearchDV.Count) + mCASearchDV.Count == gMaxRows && mCASearchDV.Count > 1 ? 
											"  - There were more than " + mCASearchDV.Count.ToString() +
											" results to your search.  Only the first " + mCASearchDV.Count.ToString() + 
											" are displayed." : ""
									);

					totalAmount = 0;
					for (int i = 0; i < mCASearchDV.Count; i++)
					{
						if (mCASearchDV[i]["amount"].ToString() == "-0.01")
						{
							mCASearchDV[i]["amount"] = mCASearchDV[i]["openamount"];
						}

						totalAmount += Convert.ToDouble(mCASearchDV[i]["amount"]);

						mRetVal.caEmail[i] = mCASearchDV[i]["email"].ToString();

						if (mCASearchDV[i]["exp"].ToString().Length > 3)
						{
							expMonth = Convert.ToInt32(mCASearchDV[i]["exp"].ToString().Substring(0, 2));
							expYear = Convert.ToInt32("20" + mCASearchDV[i]["exp"].ToString().Substring(3));
						}
						else
						{
							expMonth = -1;
							expYear = -1;
						}

						mRetVal.caHTML += string.Format(@"
								<div class='scrollRow' id='mSearchScrollRowCA_{0}'>
									<div id='idSearchTDCA_{0}' class='rowID' style='{12}'>&nbsp;{10}</div>
									<div id='typeSearchTDCA_{0}' class='scrollType' style='{12}'>&nbsp;<a onclick={1}displaySearchRow({0},'CA'){1} onmouseover={1}window.status='Transaction Details';return true;{1} onmouseout={1}window.status='';return true;{1}>{2}</a></div>
									<div id='keySearchTDCA_{0}' class='scrollOrder' style='{12}'>&nbsp;{3}</div>
									<div id='dctSearchTDCA_{0}' class='scrollDCT' style='{12}'>&nbsp;{4}</div>
									<div id='docSearchTDCA_{0}' class='scrollDOC' style='{12}'>&nbsp;{5}</div>
									<div id='kcoSearchTDCA_{0}'  class='scrollKCO' style='{12}'>&nbsp;{6}</div>
									<div id='emailSearchTDCA_{0}' class='scrollEmail' style='{12}'>&nbsp;<input class='flat' id=emailSearchCA_{0} value='{7}' type=text size=25 onchange='updateSearch(this);'></div>
									<div id='amountSearchTDCA_{0}' class='scrollAmt' style='{12}'>&nbsp;{11}</div>
									<div id='reasonSearchTDCA_{0}'  class='scrollReason' style='{12}'>&nbsp;{9}</div>
									<div id='cardTypeSearchTDCA_{0}'  class='scrollCardType' style='{12}'>&nbsp;{13}</div>
									<div id='notesSearchTDCA_{0}'  class='scrollNotes' style='{12}'>&nbsp;{14}</div>
									<div id='statusSearchTDCA_{0}' class='scrollStatus' style='{12}'>&nbsp;<select style='width:130px;' id='statusSearchCA_{0}' onchange='updateSearch(this);'>{8}</select></div>
								</div>
							"
							, i
							, "\""
							, mCASearchDV[i]["type_name"]
							, mCASearchDV[i]["order_skey"].ToString()
							, mCASearchDV[i]["dct"].ToString()
							, mCASearchDV[i]["doc"].ToString()
							, mCASearchDV[i]["kco"].ToString()
							, mCASearchDV[i]["email"].ToString()
							, thePage.Session["gStatusOptCA_" + mCASearchDV[i]["status_id"]]
							, mCASearchDV[i]["reason_code"].ToString()
							, mCASearchDV[i]["id"]
							, string.Format("{0:c}", mCASearchDV[i]["amount"])
							, (expYear < DateTime.Now.Year ||
							  (expYear == DateTime.Now.Year && expMonth < DateTime.Now.Month)) ? "background: #f3f3f3;" : ""
							, getCCType(mCASearchDV[i]["credit_card_no"].ToString())
						    , mCASearchDV[i]["notes_flag"]
							);

						mRetVal.caHTML += string.Format(@"
											<div class='hiddenNotes'>
												Notes: {0}
											</div>
											"
							, mCASearchDV[i]["notes"].ToString()
							);

							mRetVal.caHTML += string.Format(@"
												<div id='mHiddenSearchRowCA_{0}' class='searchDetailRowDiv' style='display:none;'>
													<table class='detailRow'>
														<tr>
															<td class='label'>Credit Card #: </td><td class='input'>************&nbsp;{2}</td>
															<td class='label'>Exp Date: </td><td class='input'>{3}</td>
															<td class='label'>Pay Status: </td><td class='input' colspan=3>{4}</td>
												
														</tr>
														<tr>
															<td class='label'>Maint Date: </td><td class='input'>{11}</td>
															<td class='label'>Gross: </td><td class='input'>{5}</td>
															<td class='label'>Open: </td><td class='input'>{6}</td>
															<td class='label'>ODOC: </td><td class='input'>{7}</td>
														</tr>
														<tr>
															<td class='label'>Invoice Date: </td><td class='input'>{8}</td>
															<td class='label'>Remark: </td><td class='input' colspan=5>{9}</td>
														</tr>
														<tr>
															<td class='label'>Cust {1}CB{1} #: </td><td class='input'>{12}</td>
															<td class='label'>Cust {1}CS{1} #: </td><td class='input'>{23}</td>
															<td class='label'>Company: </td><td class='input' colspan=5>{10}</td>
														</tr>
														<tr>
															<td class='label'>Contact Name: </td><td class='input' colspan=3>{24}</td>
															<td class='label'>Contact Phone: </td><td class='input'>{25}</td>
														</tr>
														<tr>
															<td class='label'>Name: </td><td class='input' colspan=3>{13}&nbsp;{14}</td>
															<td class='label'>Phone: </td><td class='input'>{20}</td>
															<td class='label'>Country: </td><td class='input'>{19}</td>
														</tr>
														<tr>
															<td class='label'>Street: </td><td class='input'>{15}</td>
															<td class='label'>City: </td><td class='input'>{16}</td>
															<td class='label'>State: </td><td class='input'>{17}</td>
															<td class='label'>Zip: </td><td class='input'>{18}</td>	
														</tr>
														<tr>
															<td class='label' style='vertical-align:text-top;'>Notes:<br>{22}</td>
															<td class='input' colspan=7><textarea id=notesSearchCA_{0} rows=3 cols=80 onchange='updateSearch(this);'>{21}</textarea></td>
														</tr>
														<tr>
															<td class='label' colspan=8><a href='#' onclick={1}viewHistory({0},'CASearch');{1}>View History</a></td>
														</tr>
													</table>
												</div>
												"
												, i, "\""
												, mCASearchDV[i]["credit_card_suffix"].ToString()
												, mCASearchDV[i]["exp"].ToString(), mCASearchDV[i]["paystatus"].ToString()
												, string.Format("{0:c}", mCASearchDV[i]["gross"])
												, string.Format("{0:c}", mCASearchDV[i]["openamount"])
												, mCASearchDV[i]["rpodoc"].ToString()
												, mCASearchDV[i]["invoice_date"].ToString(), mCASearchDV[i]["remark"].ToString()
												, mCASearchDV[i]["company"].ToString()
												, mCASearchDV[i]["maint_date"].ToString()
												, mCASearchDV[i]["cb"].ToString()
												, mCASearchDV[i]["card_first_name"].ToString()
												, mCASearchDV[i]["card_last_name"].ToString()
												, mCASearchDV[i]["card_street"].ToString()
												, mCASearchDV[i]["card_city"].ToString()
												, mCASearchDV[i]["card_state"].ToString()
												, mCASearchDV[i]["card_zip"].ToString()
												, mCASearchDV[i]["card_country"].ToString()
												, mCASearchDV[i]["card_phone"].ToString()
												, mCASearchDV[i]["notes"].ToString()
												, resizeSelect("notesSearchCA", i)
												, mCASearchDV[i]["cs"].ToString()
												, mCASearchDV[i]["contact_name"].ToString()
												, mCASearchDV[i]["contact_phone"].ToString()
												);

					}
					mRetVal.caHTML += string.Format(@"
												</div>
												<div class='scrollFoot'>	
													<div class='scrollFootLabel'>Total Amount: </div>
													<div class='scrollFootSpace'>
													<div class='scrollAmt'>{0}</div>
												</div>	
											</div>
											"
											, string.Format("{0:c}", totalAmount)
										   );
					#endregion
				}
			}
			catch (Exception e)
			{
				mRetVal.error = true;
				logError("Common.cs","search",aQuery,thePage.Session["gNTName"].ToString(),e.ToString(),"ERROR");
			}
			finally
			{
				aConn.Close();
			}

			return mRetVal;
		}

		
		/**
		 * creditSearch
		 * 
		 * connects to cckey
		 * if requery is true searchs for all us and cs credit trans that match the search params
		 * if loc is all creates both us and ca tables, otherwise creates table defined in loc
		 * logs any errors in the db
		 * */
		public RetStruct creditSearch(Page thePage, string loc, bool requery)
		{
			DataView mStatusDV;
			DataView mUSCreditDV = null;
			DataView mCACreditDV = null;
			DataTable mHoldDT;
			RetStruct mRetVal = new RetStruct();
			WSDBConnection aConn = new WSDBConnection();
			string aQuery = "";

			try
			{
				aConn.Open("CCKEY", 0);
				if (requery)
				{
					#region set Credit parms

					string mTransID = (thePage.Session["gCreditTransID"] == null || thePage.Session["gCreditTransID"].ToString() == "") ? "" : "AND t.id = " + thePage.Session["gCreditTransID"].ToString();
					string mCustNum = (thePage.Session["gCreditCustNum"] == null || thePage.Session["gCreditCustNum"].ToString() == "") ? "" : "AND ab.aban85 = " + thePage.Session["gCreditCustNum"].ToString();
					string mCustName = (thePage.Session["gCreditCustName"] == null || thePage.Session["gCreditCustName"].ToString() == "") ? "" : "AND rp.rpalph like '%" + thePage.Session["gCreditCustName"].ToString() + "%'";
					string mOrder = (thePage.Session["gCreditOrder"] == null || thePage.Session["gCreditOrder"].ToString() == "") ? "" : "AND t.order_skey = " + thePage.Session["gCreditOrder"].ToString();
					string mCSNum = (thePage.Session["gCreditCSNum"] == null || thePage.Session["gCreditCSNum"].ToString() == "") ? "" : "AND ooh.ship_an8 = " + thePage.Session["gCreditCSNum"].ToString();
					string mODOC = (thePage.Session["gCreditODOC"] == null || thePage.Session["gCreditODOC"].ToString() == "") ? "" : "AND rp.rpodoc = " + thePage.Session["gCreditODOC"].ToString();
					string mDCT = (thePage.Session["gCreditDCT"] == null || thePage.Session["gCreditDCT"].ToString() == "") ? "" : "AND t.dct = '" + thePage.Session["gCreditDCT"].ToString() + "'";
					string mDOC = (thePage.Session["gCreditDOC"] == null || thePage.Session["gCreditDOC"].ToString() == "") ? "" : "AND t.doc = " + thePage.Session["gCreditDOC"].ToString();
					string mKCO = (thePage.Session["gCreditKCO"] == null || thePage.Session["gCreditKCO"].ToString() == "") ? "" : "AND t.kco = '" + thePage.Session["gCreditKCO"].ToString() + "'";
					string mCCSuffix = (thePage.Session["gCreditCCSufix"] == null || thePage.Session["gCreditCCSufix"].ToString() == "") ? "" : "AND ooh.credit_card_suffix = " + thePage.Session["gCreditCCSufix"].ToString();

					string mSentFrom = (thePage.Session["gCreditSentFrom"] == null || thePage.Session["gCreditSentFrom"].ToString() == "") ? "" : "to_date('" + thePage.Session["gCreditSentFrom"].ToString() + " 00:00:00','mm/dd/yyyy HH24:MI:SS')";
					string mSentTo = (thePage.Session["gCreditSentTo"] == null || thePage.Session["gCreditSentTo"].ToString() == "") ? "" : "to_date('" + thePage.Session["gCreditSentTo"].ToString() + " 23:59:59','mm/dd/yyyy HH24:MI:SS')";
					string mSentDate = "";
					if (mSentFrom != "" && mSentTo != "")
					{
						mSentDate = "AND t.maint_date between " + mSentFrom + " AND " + mSentTo;
					}
					else if (mSentFrom != "")
					{
						mSentDate = "AND t.maint_date > " + mSentFrom;
					}
					else if (mSentTo != "")
					{
						mSentDate = "AND t.maint_date < " + mSentTo;
					}

					string mType = (thePage.Session["gCreditType"] == null || thePage.Session["gCreditType"].ToString() == "") ? "" : "AND t.type_id = '" + thePage.Session["gCreditType"].ToString() + "'";
					//unlike the rest this value is selected in the dv not the actual query
					string mCardType = (thePage.Session["gCreditCardType"] == null || thePage.Session["gCreditCardType"].ToString() == "") ? "" :
						(thePage.Session["gCreditCardType"].ToString() == "54") ? "AND (credit_card_no like '5%' OR credit_card_no like '4%')" : "AND credit_card_no like '" + thePage.Session["gCreditCardType"].ToString() + "%'";


					string mLoc;
					if (loc == "US")
					{
						mLoc = "AND t.kco IN ('00010','00090')";
					}
					else if (loc == "CA")
					{
						mLoc = "AND t.kco = '00020'";
					}
					else
					{
						mLoc = "AND t.kco IN ('00010','00090','00020')";
					}
					#endregion

					#region search Credit



					if (thePage.Session["gStatusDV"] == null)
					{
						getStatusType(thePage);
					}

					//maybe add the desc back in later with a help box
					aQuery = string.Format(@"
					SELECT * FROM (
						SELECT /*+ Rule */ t.id
							  ,t.type_id
							  ,y.type_name
							  ,t.order_skey
							  ,t.dct
							  ,t.doc
							  ,t.kco
							  ,t.status_id
							  ,(nvl(t.amount
								   ,-1) * .01) amount
							  ,t.email
							  ,t.maint_date
							  ,t.reason_code
							  ,t.notes
							  ,decode(t.notes,null,'','*') notes_flag
						      ,i.contact_name
							  ,i.contact_phone
							  ,i.card_first_name
							  ,i.card_last_name
							  ,i.card_street
							  ,i.card_city
							  ,i.card_state
							  ,i.card_zip
							  ,i.card_country
							  ,i.card_phone
							  ,t.create_user_id
							  ,t.cyber_id
							  ,t.cyber_token
							  ,CASE
								 WHEN k.key IS NOT NULL
									  AND ooh.credit_card_crypt IS NOT NULL THEN
								  TRIM(pkg_cc_crypt.decrypt(ooh.credit_card_crypt
														   ,(k.key)))
								 ELSE
								  ' '
							   END credit_card_no
							  ,nvl(ooh.credit_card_suffix,t.cc_suffix)  AS credit_card_suffix
							  ,decode(ooh.credit_card_suffix
									 ,NULL
									 ,' '
									 ,substr(ooh.credit_card_expiration_date
											,0
											,2) || '/' || substr(ooh.credit_card_expiration_date
																,3)) exp
							  ,(nvl(SUM(rpag)
								   ,0) * .01) gross
							  ,(nvl(SUM(rpaap)
								   ,0) * .01) openamount
							  ,rp.rpodoc
							  ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
									  ,'MM/DD/YY') AS invoice_date
							  ,rp.rprmk remark
							  ,min(decode(rp.rppst
									 ,NULL
									 ,''
									 ,rp.rppst || ' - ' || dr.drdl01)) paystatus
							  ,rp.rpalph company
							  ,ab.aban85 cb
							  ,ooh.ship_an8 cs
							  ,s.status_name
							  ,e.inv_type HIFlag
							  FROM cct_trans        t
								  ,cct_order_info   i
								  ,omb_order_header ooh
								  ,f03b11           rp
								  ,f0101		    ab
								  ,cc_keys          k
								  ,cct_type         y
								  ,f0005            dr
								  ,sisr.cct_status  s
								  ,jde_pkg.inv_extract_branch_type e
							 WHERE t.dct = rp.rpdct
							   AND t.doc = rp.rpdoc
							   AND t.kco = rp.rpkco					   
							   AND t.order_skey = ooh.order_skey
							   AND ooh.order_skey = k.order_skey(+)
							   AND t.type_id = y.type_id
							   AND ab.aban8 = rp.rpan8
							   AND t.status_id = {11}
							   AND dr.drsy(+) = '00  ' 
							   AND dr.drrt(+) = 'PS'
							   AND trim(dr.drky(+)) = rppst
							   AND i.order_skey = t.order_skey
							   AND t.status_id = s.status_id
							   AND ooh.oe_location = e.mcu(+)
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
							   {12}
							   {13}
							 GROUP BY t.id
									 ,t.type_id
									 ,y.type_name
									 ,t.order_skey
									 ,t.dct
									 ,t.doc
									 ,t.kco
									 ,t.status_id
									 ,t.amount
									 ,t.email
									 ,t.maint_date
									 ,t.reason_code
									 ,t.notes
									 ,i.contact_name
							         ,i.contact_phone
									 ,i.card_first_name
									 ,i.card_last_name
									 ,i.card_street
									 ,i.card_city
									 ,i.card_state
									 ,i.card_zip
									 ,i.card_country
									 ,i.card_phone
							         ,t.create_user_id
									 ,t.cyber_id
									 ,t.cyber_token
									 ,CASE
										 WHEN k.key IS NOT NULL
											  AND ooh.credit_card_crypt IS NOT NULL THEN
										  TRIM(pkg_cc_crypt.decrypt(ooh.credit_card_crypt
																   ,(k.key)))
										 ELSE
										  ' '
									   END 
									  ,nvl(ooh.credit_card_suffix,t.cc_suffix) 
									  ,decode(ooh.credit_card_suffix
											 ,NULL
											 ,' '
											 ,substr(ooh.credit_card_expiration_date
													,0
													,2) || '/' || substr(ooh.credit_card_expiration_date
																		,3))
									 ,rp.rpodoc
									 ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
											 ,'MM/DD/YY')
									 ,rp.rprmk
									 ,rp.rpalph
									 ,ab.aban85
									 ,ooh.ship_an8
									 ,s.status_name
									 ,e.inv_type
							 ORDER BY t.type_id
									 ,t.order_skey
									 ,t.dct
									 ,t.doc)
						 WHERE ROWNUM <= {14}
						"
                                , mTransID
								, mCustNum
								, mCustName
								, mOrder
								, mDCT
								, mDOC
								, mKCO
								, mCCSuffix
								, mSentDate
								, mType
								, mLoc
								, ConfigurationManager.AppSettings["STATUS_COMPLETED"]
								, mCSNum
								, mODOC
								, gMaxRows
								);


					mHoldDT = aConn.GetDataTable(aQuery);

					if (loc == "US" || loc == "ALL")
					{
						mUSCreditDV = new DataView(mHoldDT);
						mUSCreditDV.RowFilter = string.Format("kco in ('00010','00090') {0}",mCardType);
						thePage.Session["gUSCreditDV"] = mUSCreditDV;

					}

					if (loc == "CA" || loc == "ALL")
					{
						mCACreditDV = new DataView(mHoldDT);
						mCACreditDV.RowFilter = string.Format("kco = '00020' {0}", mCardType);
						thePage.Session["gCACreditDV"] = mCACreditDV;
					}

					#endregion
				}
				else
				{
					mUSCreditDV = (DataView)thePage.Session["gUSCreditDV"];
					mCACreditDV = (DataView)thePage.Session["gCACreditDV"];
				}

				mStatusDV = (DataView)thePage.Session["gStatusDV"];
				int i;
				double totalAmount;
				int expMonth;
				int expYear;

				if (loc == "US" || loc == "ALL")
				{
					#region load us Credit

					mRetVal.usAmt = new string[mUSCreditDV.Count + 1];
					mRetVal.usEmail = new string[mUSCreditDV.Count];

					mRetVal.usHTML = string.Format(@"
								<div style='font-weight:bold;float:left;'>{1} {2} {3}</div>
								<div class='creditTable' >
									<div class='printHelper'>
										<div class='scrollHead' >
												<div class='headID'><a href='#' onclick={0}sortCreditUS('id');return false;{0}>ID<span></span></a></div>
												<div class='scrollType'><a href='#' onclick={0}sortCreditUS('type_name');return false;{0}>Type<span></span></a></div>
												<div class='scrollOrder'><a href='#' onclick={0}sortCreditUS('order_skey');return false;{0}>Order #<span></span></a></div>
												<div class='scrollDCT'><a href='#' onclick={0}sortCreditUS('dct');return false;{0}>Dct<span></span></a></div>
												<div class='scrollDOC'><a href='#' onclick={0}sortCreditUS('doc');return false;{0}>Doc #<span></span></a></div>
												<div class='scrollKCO'><a href='#' onclick={0}sortCreditUS('kco');return false;{0}>Company<span></span></a></div>
												<div class='scrollEmail'><a href='#' onclick={0}sortCreditUS('email');return false;{0}>Email<span></span></a></div>
												<div class='scrollCreditAmt'><a href='#' onclick={0}sortCreditUS('amount');return false;{0}>Amount<span></span></a></div>
												<div class='scrollReason'><a href='#' onclick={0}sortCreditUS('reason_code');return false;{0}>Reason<span></span></a></div>
												<div class='scrollCardType'><a href='#' onclick={0}sortCreditUS('credit_card_no');return false;{0}>Card Type<span></span></a></div>
												<div class='scrollNotes'><a href='#' onclick={0}sortCreditUS('notes');return false;{0}>Notes<span></span></a></div>
												<div class='scrollStatus'><a href='#' onclick={0}sortCreditUS('status_id');return false;{0}>Status<span></span></a></div>
												<div class='headCreditEnd'>&nbsp;</div>
										</div>
									</div>
									<div class=ScrollTable>
									", "\""
									 , mUSCreditDV.Count
									 ,(mUSCreditDV.Count > 1)?"Results":"Result"
									 , mUSCreditDV.Count + (mCACreditDV == null?0:mCACreditDV.Count) == gMaxRows && mUSCreditDV.Count > 1 ?
													"  - There were more than " + mUSCreditDV.Count.ToString() +
													" results to your search.  Only the first " + mUSCreditDV.Count.ToString() +
													" are displayed." : ""
									);



					totalAmount = 0;
					for (i = 0; i < mUSCreditDV.Count; i++)
					{
						if (mUSCreditDV[i]["amount"].ToString() == "-0.01")
						{
							mUSCreditDV[i]["amount"] = mUSCreditDV[i]["openamount"];
						}
						totalAmount += Convert.ToDouble(mUSCreditDV[i]["amount"]);
						mRetVal.usAmt[i] = string.Format("{0:0.00}", mUSCreditDV[i]["amount"]);

						mRetVal.usEmail[i] = mUSCreditDV[i]["email"].ToString();

						if (mUSCreditDV[i]["exp"].ToString().Length > 3)
						{
							expMonth = Convert.ToInt32(mUSCreditDV[i]["exp"].ToString().Substring(0, 2));
							expYear = Convert.ToInt32("20" + mUSCreditDV[i]["exp"].ToString().Substring(3));
						}
						else
						{
							expMonth = -1;
							expYear = -1;
						}


						//updateCredit for Amount called if format amount works
						mRetVal.usHTML += string.Format(@"
								<div class='scrollRow' id='mCreditScrollRowUS_{0}'>
									<div id='idCreditTDUS_{0}' class='rowID' style='{12}'>&nbsp;{10}</div>
									<div id='typeCreditTDUS_{0}' class='scrollType' style='{12}'>&nbsp;<a onclick={1}displayCreditRow({0},'US'){1} onmouseover={1}window.status='Transaction Details';return true;{1} onmouseout={1}window.status='';return true;{1}>{2}</a></div>
									<div id='keyCreditTDUS_{0}' class='scrollOrder' style='{12}'>&nbsp;{3}</div>
									<div id='dctCreditTDUS_{0}' class='scrollDCT' style='{12}'>&nbsp;{4}</div>
									<div id='docCreditTDUS_{0}' class='scrollDOC' style='{12}'>&nbsp;{5}</div>
									<div id='kcoCreditTDUS_{0}'  class='scrollKCO' style='{12}'>&nbsp;{6}</div>
									<div id='emailCreditTDUS_{0}' class='scrollEmail' style='{12}'>&nbsp;<input class='flat' id=emailCreditUS_{0} value='{7}' type=text size=25 onchange='updateCredit(this);'></div>
									<div id='amountCreditTDUS_{0}' class='scrollCreditAmt' style='{12}'>&nbsp;<input type='text' id=amtCreditUS_{0} size=9 value='{11}' style='text-align:right;'></div>
									<div id='reasonCreditTDUS_{0}'  class='scrollReason' style='{12}'>&nbsp;{9}</div>
									<div id='cardTypeCreditTDUS_{0}'  class='scrollCardType' style='{12}'>&nbsp;{13}</div>
									<div id='notesCreditTDUS_{0}'  class='scrollNotes' style='{12}'>&nbsp;{14}</div>
									<div id='statusCreditTDUS_{0}' class='scrollStatus' style='{12}'>&nbsp;<select style='width:120px;' id='statusCreditUS_{0}' disabled='true'>{8}</select></div>
									<div id='creditButtonTDUS_{0}' class='rowCreditEnd' style='{12}'>&nbsp;<input type='button' id='mUSCreditButton_{0}'  value='credit' onclick='creditUSRow({0})'></div>
								</div>	
							"
							, i
							, "\""
							, mUSCreditDV[i]["type_name"]
							, mUSCreditDV[i]["order_skey"].ToString()
							, mUSCreditDV[i]["dct"].ToString()
							, mUSCreditDV[i]["doc"].ToString()
							, mUSCreditDV[i]["kco"].ToString()
							, mUSCreditDV[i]["email"].ToString()
							, thePage.Session["gStatusOptUS_" + mUSCreditDV[i]["status_id"]]
							, mUSCreditDV[i]["reason_code"].ToString()
							, mUSCreditDV[i]["id"]
							, string.Format("{0:c}",mUSCreditDV[i]["amount"])
							, (expYear < DateTime.Now.Year ||
							  (expYear == DateTime.Now.Year && expMonth < DateTime.Now.Month)) ? "background: #f3f3f3;" : ""
							, getCCType(mUSCreditDV[i]["credit_card_no"].ToString())
							, mUSCreditDV[i]["notes_flag"]
							);

						mRetVal.usHTML += string.Format(@"
											<div class='hiddenNotes'>
												Notes: {0}
											</div>
											"
							, mUSCreditDV[i]["notes"].ToString()
							);

						mRetVal.usHTML += string.Format(@"
												<div id='mHiddenCreditRowUS_{0}' class='creditDetailRowDiv' style='display:none;'>
													<table class='detailRow'>
														<tr>
															<td class='label'>Credit Card #: </td><td class='input'>************&nbsp;{2}</td>
															<td class='label'>Exp Date: </td><td class='input'>{3}</td>
															<td class='label'>Pay Status: </td><td class='input' colspan=3>{4}</td>
												
														</tr>
														<tr>
															<td class='label'>Maint Date: </td><td class='input'>{11}</td>
															<td class='label'>Gross: </td><td class='input'>{5}</td>
															<td class='label'>Open: </td><td class='input'>{6}</td>
															<td class='label'>ODOC: </td><td class='input'>{7}</td>
														</tr>
														<tr>
															<td class='label'>Invoice Date: </td><td class='input'>{8}</td>
															<td class='label'>Remark: </td><td class='input' colspan=5>{9}</td>
														</tr>
														<tr>
															<td class='label'>Cust {1}CB{1} #: </td><td class='input'>{12}</td>
															<td class='label'>Cust {1}CS{1} #: </td><td class='input'>{23}</td>
															<td class='label'>Company: </td><td class='input' colspan=5>{10}</td>
														</tr>
														<tr>
															<td class='label'>Contact Name: </td><td class='input' colspan=3>{24}</td>
															<td class='label'>Contact Phone: </td><td class='input'>{25}</td>
														</tr>
														<tr>
															<td class='label'>Name: </td><td class='input' colspan=3>{13}&nbsp;{14}</td>
															<td class='label'>Phone: </td><td class='input'>{20}</td>
															<td class='label'>Country: </td><td class='input'>{19}</td>
														</tr>
														<tr>
															<td class='label'>Street: </td><td class='input'>{15}</td>
															<td class='label'>City: </td><td class='input'>{16}</td>
															<td class='label'>State: </td><td class='input'>{17}</td>
															<td class='label'>Zip: </td><td class='input'>{18}</td>
														</tr>
														<tr>
															<td class='label' style='vertical-align:text-top;'>Notes:<br>{22}</td>
															<td class='input' colspan=7><textarea id=notesCreditUS_{0} rows=3 cols=80 onchange='updateCredit(this);'>{21}</textarea></td>
														</tr>
														<tr>
															<td class='label' colspan=8><a href='#' onclick={1}viewHistory({0},'USCredit');{1}>View History</a></td>
														</tr>
													</table>
												</div>
												"
												, i
												, "\""
												, mUSCreditDV[i]["credit_card_suffix"].ToString()
												, mUSCreditDV[i]["exp"].ToString()
												, mUSCreditDV[i]["paystatus"].ToString()
												, string.Format("{0:c}",mUSCreditDV[i]["gross"])
												, string.Format("{0:c}",mUSCreditDV[i]["openamount"])
												, mUSCreditDV[i]["rpodoc"].ToString()
												, mUSCreditDV[i]["invoice_date"].ToString()
												, mUSCreditDV[i]["remark"].ToString()
												, mUSCreditDV[i]["company"].ToString()
												, mUSCreditDV[i]["maint_date"].ToString()
												, mUSCreditDV[i]["cb"].ToString()
												, mUSCreditDV[i]["card_first_name"].ToString()
												, mUSCreditDV[i]["card_last_name"].ToString()
												, mUSCreditDV[i]["card_street"].ToString()
												, mUSCreditDV[i]["card_city"].ToString()
												, mUSCreditDV[i]["card_state"].ToString()
												, mUSCreditDV[i]["card_zip"].ToString()
												, mUSCreditDV[i]["card_country"].ToString()
												, mUSCreditDV[i]["card_phone"].ToString()
												, mUSCreditDV[i]["notes"].ToString()
												, resizeSelect("notesCreditUS", i)
												, mUSCreditDV[i]["cs"].ToString()
												, mUSCreditDV[i]["contact_name"]
												, mUSCreditDV[i]["contact_phone"]
												);

					}
					mRetVal.usAmt[i] = string.Format("{0:0.00}", totalAmount);
					mRetVal.usHTML += string.Format(@"
												</div>
												<div class='scrollFoot'>	
													<div class='scrollFootLabel'>Total Amount: </div>
													<div id=mCreditUSTAmt class='scrollCreditAmt'>{0}</div>
												</div>	
											</div>
										"
										, string.Format("{0:c}", totalAmount)
									   );
					#endregion
				}

				if (loc == "CA" || loc == "ALL")
				{
					#region load ca Credit

					mRetVal.caAmt = new string[mCACreditDV.Count + 1];
					mRetVal.caEmail = new string[mCACreditDV.Count];

					mRetVal.caHTML = string.Format(@"
								<div style='font-weight:bold;float:left;'>{1} {2} {3}</div>
								<div class='creditTable' >
									<div class='printHelper'>
										<div class='scrollHead' >
												<div class='headID'><a href='#' onclick={0}sortCreditCA('id');return false;{0}>ID<span></span></a></div>
												<div class='scrollType'><a href='#' onclick={0}sortCreditCA('type_name');return false;{0}>Type<span></span></a></div>
												<div class='scrollOrder'><a href='#' onclick={0}sortCreditCA('order_skey');return false;{0}>Order #<span></span></a></div>
												<div class='scrollDCT'><a href='#' onclick={0}sortCreditCA('dct');return false;{0}>Dct<span></span></a></div>
												<div class='scrollDOC'><a href='#' onclick={0}sortCreditCA('doc');return false;{0}>Doc #<span></span></a></div>
												<div class='scrollKCO'><a href='#' onclick={0}sortCreditCA('kco');return false;{0}>Company<span></span></a></div>
												<div class='scrollEmail'><a href='#' onclick={0}sortCreditCA('email');return false;{0}>Email<span></span></a></div>
												<div class='scrollCreditAmt'><a href='#' onclick={0}sortCreditCA('amount');return false;{0}>Amount<span></span></a></div>
												<div class='scrollReason'><a href='#' onclick={0}sortCreditCA('reason_code');return false;{0}>Reason<span></span></a></div>
												<div class='scrollCardType'><a href='#' onclick={0}sortCreditCA('credit_card_no');return false;{0}>Card Type<span></span></a></div>
												<div class='scrollNotes'><a href='#' onclick={0}sortCreditCA('notes_flag');return false;{0}>Notes<span></span></a></div>
												<div class='scrollStatus'><a href='#' onclick={0}sortCreditCA('status_id');return false;{0}>Status<span></span></a></div>
												<div class='headCreditEnd'>&nbsp;</div>
										</div>
									</div>
									<div class=ScrollTable>
									", "\""
									 , mCACreditDV.Count
									 , (mCACreditDV.Count > 1)?"Results":"Result"
									 , (mUSCreditDV == null ? 0 : mUSCreditDV.Count) + mCACreditDV.Count == gMaxRows && mCACreditDV.Count > 1 ? 
													"    - There were more than " + mCACreditDV.Count.ToString() +
													" results to your search.  Only the first " + mCACreditDV.Count.ToString() +
													" are displayed." : ""
									);

					totalAmount = 0;
					for (i = 0; i < mCACreditDV.Count; i++)
					{
						if (mCACreditDV[i]["amount"].ToString() == "-0.01")
						{
							mCACreditDV[i]["amount"] = mCACreditDV[i]["openamount"];
						}
						totalAmount += Convert.ToDouble(mCACreditDV[i]["amount"]);
						mRetVal.caAmt[i] = string.Format("{0:0.00}", mCACreditDV[i]["amount"]);

						mRetVal.caEmail[i] = mCACreditDV[i]["email"].ToString();

						if (mCACreditDV[i]["exp"].ToString().Length > 3)
						{
							expMonth = Convert.ToInt32(mCACreditDV[i]["exp"].ToString().Substring(0, 2));
							expYear = Convert.ToInt32("20" + mCACreditDV[i]["exp"].ToString().Substring(3));
						}
						else
						{
							expMonth = -1;
							expYear = -1;
						}

						mRetVal.caHTML += string.Format(@"
								<div class='scrollRow' id='mCreditScrollRowCA_{0}'>
									<div id='idCreditTDCA_{0}' class='rowID' style='{12}'>&nbsp;{10}</div>
									<div id='typeCreditTDCA_{0}' class='scrollType' style='{12}'>&nbsp;<a onclick={1}displayCreditRow({0},'CA'){1} onmouseover={1}window.status='Transaction Details';return true;{1} onmouseout={1}window.status='';return true;{1}>{2}</a></div>
									<div id='keyCreditTDCA_{0}' class='scrollOrder' style='{12}'>&nbsp;{3}</div>
									<div id='dctCreditTDCA_{0}' class='scrollDCT' style='{12}'>&nbsp;{4}</div>
									<div id='docCreditTDCA_{0}' class='scrollDOC' style='{12}'>&nbsp;{5}</div>
									<div id='kcoCreditTDCA_{0}'  class='scrollKCO' style='{12}'>&nbsp;{6}</div>
									<div id='emailCreditTDCA_{0}' class='scrollEmail' style='{12}'>&nbsp;<input class='flat' id=emailCreditCA_{0} value='{7}' type=text size=25 onchange='updateCredit(this);'></div>
									<div id='amountCreditTDCA_{0}' class='scrollCreditAmt' style='{12}'>&nbsp;<input type='text' id=amtCreditCA_{0} size=9 value='{11}' style='text-align:right;'></div>
									<div id='reasonCreditTDCA_{0}'  class='scrollReason' style='{12}'>&nbsp;{9}</div>
									<div id='cardTypeCreditTDCA_{0}' class='scrollCardType' style='{12}'>&nbsp;{13}</div>
									<div id='notesCreditTDCA_{0}' class='scrollNotes' style='{12}'>&nbsp;{14}</div>
									<div id='statusCreditTDCA_{0}' class='scrollStatus' style='{12}'>&nbsp;<select style='width:120px;' id='statusCreditCA_{0}' disabled='true'>{8}</select></div>
									<div id='creditButtonTDCA_{0}' class='rowCreditEnd' style='{12}'>&nbsp;<input type='button' id='mCACreditButton_{0}'  value='credit' onclick='creditCARow({0})'></div>
								</div>	
							"
							, i
							, "\""
							, mCACreditDV[i]["type_name"]
							, mCACreditDV[i]["order_skey"].ToString()
							, mCACreditDV[i]["dct"].ToString()
							, mCACreditDV[i]["doc"].ToString()
							, mCACreditDV[i]["kco"].ToString()
							, mCACreditDV[i]["email"].ToString()
							, thePage.Session["gStatusOptCA_" + mCACreditDV[i]["status_id"]]
							, mCACreditDV[i]["reason_code"].ToString()
							, mCACreditDV[i]["id"]
							, string.Format("{0:c}",mCACreditDV[i]["amount"])
							, (expYear < DateTime.Now.Year ||
											   (expYear == DateTime.Now.Year && expMonth < DateTime.Now.Month)) ? "background: #f3f3f3;" : ""
							, getCCType(mCACreditDV[i]["credit_card_no"].ToString())
						    , mCACreditDV[i]["notes_flag"]
							);

						mRetVal.caHTML += string.Format(@"
											<div class='hiddenNotes'>
												Notes: {0}
											</div>
											"
							, mCACreditDV[i]["notes"].ToString()
							);

							mRetVal.caHTML += string.Format(@"
												<div id='mHiddenCreditRowCA_{0}' class='creditDetailRowDiv' style='display:none;'>
													<table class='detailRow'>
														<tr>
															<td class='label'>Credit Card #: </td><td class='input'>************&nbsp;{2}</td>
															<td class='label'>Exp Date: </td><td class='input'>{3}</td>
															<td class='label'>Pay Status: </td><td class='input' colspan=3>{4}</td>
												
														</tr>
														<tr>
															<td class='label'>Maint Date: </td><td class='input'>{11}</td>
															<td class='label'>Gross: </td><td class='input'>{5}</td>
															<td class='label'>Open: </td><td class='input'>{6}</td>
															<td class='label'>ODOC: </td><td class='input'>{7}</td>
														</tr>
														<tr>
															<td class='label'>Invoice Date: </td><td class='input'>{8}</td>
															<td class='label'>Remark: </td><td class='input' colspan=5>{9}</td>
														</tr>
														<tr>
															<td class='label'>Cust {1}CB{1} #: </td><td class='input'>{12}</td>
															<td class='label'>Cust {1}CS{1} #: </td><td class='input'>{23}</td>
															<td class='label'>Company: </td><td class='input' colspan=5>{10}</td>
														</tr>
														<tr>
															<td class='label'>Contact Name: </td><td class='input' colspan=3>{24}</td>
															<td class='label'>Contact Phone: </td><td class='input'>{25}</td>
														</tr>
														<tr>
															<td class='label'>Name: </td><td class='input' colspan=3>{13}&nbsp;{14}</td>
															<td class='label'>Phone: </td><td class='input'>{20}</td>
															<td class='label'>Country: </td><td class='input'>{19}</td>
														</tr>
														<tr>
															<td class='label'>Street: </td><td class='input'>{15}</td>
															<td class='label'>City: </td><td class='input'>{16}</td>
															<td class='label'>State: </td><td class='input'>{17}</td>
															<td class='label'>Zip: </td><td class='input'>{18}</td>
														</tr>
														<tr>
															<td class='label' style='vertical-align:text-top;'>Notes:<br>{22}</td>
															<td class='input' colspan=7><textarea id=notesCreditCA_{0} rows=3 cols=80 onchange='updateIt(this);'>{21}</textarea></td>
														</tr>
														<tr>
															<td class='label' colspan=8><a href='#' onclick={1}viewHistory({0},'CACredit');{1}>View History</a></td>
														</tr>
													</table>
												</div>
												"
												, i, "\""
												, mCACreditDV[i]["credit_card_suffix"].ToString()
												, mCACreditDV[i]["exp"].ToString(), mCACreditDV[i]["paystatus"].ToString()
												, string.Format("{0:c}",mCACreditDV[i]["gross"])
												, string.Format("{0:c}",mCACreditDV[i]["openamount"])
												, mCACreditDV[i]["rpodoc"].ToString()
												, mCACreditDV[i]["invoice_date"].ToString(), mCACreditDV[i]["remark"].ToString()
												, mCACreditDV[i]["company"].ToString()
												, mCACreditDV[i]["maint_date"].ToString()
												, mCACreditDV[i]["cb"].ToString()
												, mCACreditDV[i]["card_first_name"].ToString()
												, mCACreditDV[i]["card_last_name"].ToString()
												, mCACreditDV[i]["card_street"].ToString()
												, mCACreditDV[i]["card_city"].ToString()
												, mCACreditDV[i]["card_state"].ToString()
												, mCACreditDV[i]["card_zip"].ToString()
												, mCACreditDV[i]["card_country"].ToString()
												, mCACreditDV[i]["card_phone"].ToString()
												, mCACreditDV[i]["notes"].ToString()
												, resizeSelect("notesCreditCA_", i)
												, mCACreditDV[i]["cs"].ToString()
												, mCACreditDV[i]["contact_name"].ToString()
												, mCACreditDV[i]["contact_phone"].ToString()
												);

					}
					mRetVal.caAmt[i] = string.Format("{0:0.00}", totalAmount);
					mRetVal.caHTML += string.Format(@"
												</div>
												<div class='scrollFoot'>	
													<div class='scrollFootLabel'>Total Amount: </div>
													<div id=mCreditCATAmt class='scrollCreditAmt'>{0}</div>
												</div>	
											</div>
											"
											, string.Format("{0:c}", totalAmount)
										   );
					#endregion
				}
			}
			catch (Exception e)
			{
				mRetVal.error = true;
				logError("Common.cs","creditSearch",aQuery,thePage.Session["gNTName"].ToString(),e.ToString(),"ERROR");
			}
			finally
			{
				aConn.Close();
			}

			return mRetVal;
		}


		/**
		 * edit
		 * 
		 * page to edit all requests
		 * */
		public RetStruct editSearch(Page thePage, string theTransID)
		{
			DataView mStatusDV;
			DataTable mHoldDT;
			DataView mEditDV = null;
			RetStruct mRetVal = new RetStruct();
			WSDBConnection aConn = new WSDBConnection();
			string aQuery = "";

			try
			{
				aConn.Open("CCKEY", 0);
				#region search

				if (thePage.Session["gStatusDV"] == null)
				{
					getStatusType(thePage);
				}

				//maybe add the desc back in later with a help box
				aQuery = string.Format(@"
						SELECT t.id
							  ,t.type_id
							  ,y.type_name
							  ,t.order_skey
							  ,t.dct
							  ,t.doc
							  ,t.kco
							  ,t.status_id
							  ,(nvl(t.amount
								   ,-1) * .01) amount
							  ,t.email
							  ,t.maint_date
							  ,t.reason_code
							  ,t.notes
							  ,decode(t.notes,null,'','*') notes_flag
							  ,i.contact_name
							  ,i.contact_phone
							  ,i.card_first_name
							  ,i.card_last_name
							  ,i.card_street
							  ,i.card_city
							  ,i.card_state
							  ,i.card_zip
							  ,i.card_country
							  ,i.card_phone
							  ,CASE
								 WHEN k.key IS NOT NULL
									  AND ooh.credit_card_crypt IS NOT NULL THEN
								  TRIM(pkg_cc_crypt.decrypt(ooh.credit_card_crypt
														   ,(k.key)))
								 ELSE
								  ' '
							   END credit_card_no
							  ,nvl(ooh.credit_card_suffix,t.cc_suffix) AS credit_card_suffix
							  ,decode(ooh.credit_card_suffix
									 ,NULL
									 ,' '
									 ,substr(ooh.credit_card_expiration_date
											,0
											,2) || '/' || substr(ooh.credit_card_expiration_date
																,3)) exp
							  ,(nvl(SUM(rpag)
								   ,0) * .01) gross
							  ,(nvl(SUM(rpaap)
								   ,0) * .01) openamount
							  ,rp.rpodoc
							  ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
									  ,'MM/DD/YY') AS invoice_date
							  ,rp.rprmk remark
							  ,min(decode(rp.rppst
									 ,NULL
									 ,''
									 ,rp.rppst || ' - ' || dr.drdl01)) paystatus
							  ,rp.rpalph company
							  ,ab.aban85 cb
							  ,ooh.ship_an8 cs
							  ,s.status_name
							  FROM cct_trans        t
								  ,cct_order_info   i
								  ,omb_order_header ooh
								  ,f03b11           rp
								  ,f0101		    ab
								  ,cc_keys          k
								  ,cct_type         y
								  ,f0005            dr
								  ,sisr.cct_status       s
							 WHERE t.dct = rp.rpdct
							   AND t.doc = rp.rpdoc
							   AND t.kco = rp.rpkco				   
							   AND t.order_skey = ooh.order_skey
							   AND ooh.order_skey = k.order_skey(+)
							   AND t.type_id = y.type_id
							   AND ab.aban8 = rp.rpan8
							   AND dr.drsy(+) = '00  ' 
							   AND dr.drrt(+) = 'PS'
							   AND trim(dr.drky(+)) = rppst
							   AND i.order_skey(+) = t.order_skey
                               AND t.status_id = s.status_id
							   AND id = {0}
							 GROUP BY t.id
									 ,t.type_id
									 ,y.type_name
									 ,t.order_skey
									 ,t.dct
									 ,t.doc
									 ,t.kco
									 ,t.status_id
									 ,t.amount
									 ,t.email
									 ,t.maint_date
									 ,t.reason_code
                                     ,t.notes
									 ,i.contact_name
									 ,i.contact_phone
									 ,i.card_first_name
									 ,i.card_last_name
									 ,i.card_street
									 ,i.card_city
									 ,i.card_state
									 ,i.card_zip
									 ,i.card_country
									 ,i.card_phone
									 ,CASE
										 WHEN k.key IS NOT NULL
											  AND ooh.credit_card_crypt IS NOT NULL THEN
										  TRIM(pkg_cc_crypt.decrypt(ooh.credit_card_crypt
																   ,(k.key)))
										 ELSE
										  ' '
									   END
									  ,nvl(ooh.credit_card_suffix,t.cc_suffix)
									  ,decode(ooh.credit_card_suffix
											 ,NULL
											 ,' '
											 ,substr(ooh.credit_card_expiration_date
													,0
													,2) || '/' || substr(ooh.credit_card_expiration_date
																		,3)) 
									 ,rp.rpodoc
									 ,to_char(jde_pkg.pkg_jdeu.fd_odate(rp.rpdivj)
											 ,'MM/DD/YY')
									 ,rp.rprmk
									 ,rp.rpalph
									 ,ab.aban85
									 ,ooh.ship_an8
									 ,s.status_name
							 ORDER BY t.type_id
									 ,t.order_skey
									 ,t.dct
									 ,t.doc
							"
							, theTransID
							);


				mHoldDT = aConn.GetDataTable(aQuery);
				//holds 
				mHoldDT.Columns.Add("update", System.Type.GetType("System.String"));

				mEditDV = mHoldDT.DefaultView;
				thePage.Session["gEditDV"] = mEditDV;

				#endregion

				mStatusDV = (DataView)thePage.Session["gStatusDV"];

				string mDrop = "";
				double totalAmount;
				int expMonth;
				int expYear;



				#region load edit

				mRetVal.usEmail = new string[mEditDV.Count];

				mRetVal.usHTML = string.Format(@"
								<div style='font-weight:bold;float:left;'>{1} {2}</div>

								<div class='creditTable'>
									<div class='printHelper'>
										<div class='scrollHead' >
												<div class='headID'>ID</div>
												<div class='scrollType'>Type</div>
												<div class='scrollOrder'>Order #</div>
												<div class='scrollDCT'>Dct</div>
												<div class='scrollDOC'>Doc #</div>
												<div class='scrollKCO'>Company</div>
												<div class='scrollEmail'>Email</div>
												<div class='scrollAmt'>Amount</div>
												<div class='scrollReason'>Reason</div>
												<div class='scrollCardType'>Card Type</div>
												<div class='scrollNotes'>Notes</div>
												<div class='scrollStatus'>Status</div>
												<div class='headCreditEnd'>&nbsp;</div>
										</div>
									</div>	
								<div class=ScrollTable>
									", "\""
								 , mEditDV.Count
								 , mEditDV.Count > 1 ? "Results" : "Result"
								);

				totalAmount = 0;
				for (int i = 0; i < mEditDV.Count; i++)
				{
					if (mEditDV[i]["amount"].ToString() == "-0.01")
					{
						mEditDV[i]["amount"] = mEditDV[i]["openamount"];
					}

					totalAmount += Convert.ToDouble(mEditDV[i]["amount"]);

					mRetVal.usEmail[i] = mEditDV[i]["email"].ToString();

					if (mEditDV[i]["exp"].ToString().Length > 3)
					{
						expMonth = Convert.ToInt32(mEditDV[i]["exp"].ToString().Substring(0, 2));
						expYear = Convert.ToInt32("20" + mEditDV[i]["exp"].ToString().Substring(3));
					}
					else
					{
						expMonth = -1;
						expYear = -1;
					}

					//selects the drop down needed from the premade drops
					//special case is to pick which drop for a hold status

					mDrop = thePage.Session["gStatusOptCA_" + mEditDV[i]["status_id"]].ToString();


					mRetVal.usHTML += string.Format(@"
								<div class='scrollRow' id='mEditScrollRow_{0}'>
									<div id='idEditTD_{0}' class='rowID' style='{12}'>&nbsp;{10}</div>
									<div id='typeEditTD_{0}' class='scrollType' style='{12}'>&nbsp;<a onclick={1}displayEditRow({0},'US'){1} onmouseover={1}window.status='Transaction Details';return true;{1} onmouseout={1}window.status='';return true;{1}>{2}</a></div>
									<div id='keyEditTD_{0}' class='scrollOrder' style='{12}'>&nbsp;{3}</div>
									<div id='dctEditTD_{0}' class='scrollDCT' style='{12}'>&nbsp;{4}</div>
									<div id='docEditTD_{0}' class='scrollDOC' style='{12}'>&nbsp;{5}</div>
									<div id='kcoEditTD_{0}'  class='scrollKCO' style='{12}'>&nbsp;{6}</div>
									<div id='emailEditTD_{0}' class='scrollEmail' style='{12}'>&nbsp;<input class='flat' id=emailEdit_{0} value='{7}' type=text size=25></div>
									<div id='amountEditTD_{0}' class='scrollAmt' style='{12}'>&nbsp;<input type='text' id=amtEdit_{0} size=9 value='{11}' style='text-align:right;'></div>
									<div id='reasonEditTD_{0}'  class='scrollReason' style='{12}'>&nbsp;{9}</div>
									<div id='cardTypeEditTD_{0}'  class='scrollCardType' style='{12}'>&nbsp;{13}</div>
									<div id='notesEditTD_{0}'  class='scrollNotes' style='{12}'>&nbsp;{14}</div>
									<div id='statusEditTD_{0}' class='scrollStatus' style='{12}'>&nbsp;<select style='width:130px;' id='statusEdit_{0}'>{8}</select></div>
									<div id='editButtonTD_{0}' class='rowCreditEnd' style='{12}'>&nbsp;<input type='button' id='mEditButton_{0}'  value='edit' onclick='editRow({0})'></div>		
								</div>
							"
						, i
						, "\""
						, mEditDV[i]["type_name"]
						, mEditDV[i]["order_skey"].ToString()
						, mEditDV[i]["dct"].ToString()
						, mEditDV[i]["doc"].ToString()
						, mEditDV[i]["kco"].ToString()
						, mEditDV[i]["email"].ToString()
						, mDrop
						, mEditDV[i]["reason_code"].ToString()
						, mEditDV[i]["id"]
						, string.Format("{0:c}", mEditDV[i]["amount"])
						, (expYear < DateTime.Now.Year ||
						  (expYear == DateTime.Now.Year && expMonth < DateTime.Now.Month)) ? "background: #f3f3f3;" : ""
						, getCCType(mEditDV[i]["credit_card_no"].ToString())
						, mEditDV[i]["notes_flag"]
						);

					mRetVal.usHTML += string.Format(@"
											<div class='hiddenNotes'>
												Notes: {0}
											</div>
											"
						, mEditDV[i]["notes"].ToString()
						);

					mRetVal.usHTML += string.Format(@"
													<div id='mHiddenEditRow_{0}' class='creditDetailRowDiv' style='display:none;'>
														<table class='detailRow'>
															<tr>
																<td class='label'>Credit Card #: </td><td class='input'>************&nbsp;{2}</td>
																<td class='label'>Exp Date: </td><td class='input'>{3}</td>
																<td class='label'>Pay Status: </td><td class='input' colspan=3>{4}</td>
															</tr>
															<tr>
																<td class='label'>Maint Date: </td><td class='input'>{11}</td>
																<td class='label'>Gross: </td><td class='input'>{5}</td>
																<td class='label'>Open: </td><td class='input'>{6}</td>
																<td class='label'>ODOC: </td><td class='input'>{7}</td>
															</tr>
															<tr>
																<td class='label'>Invoice Date: </td><td class='input'>{8}</td>
																<td class='label'>Remark: </td><td class='input' colspan=5>{9}</td>
															</tr>
															<tr>
																<td class='label'>Cust {1}CB{1} #: </td><td class='input'>{12}</td>
																<td class='label'>Cust {1}CS{1} #: </td><td class='input'>{23}</td>
																<td class='label'>Company: </td><td class='input' colspan=5>{10}</td>
															</tr>
															<tr>
																<td class='label'>Contact Name: </td><td class='input' colspan=3>{24}</td>
																<td class='label'>Contact Phone: </td><td class='input'>{25}</td>
															</tr>
															<tr>
																<td class='label'>Name: </td><td class='input' colspan=3>{13}&nbsp;{14}</td>
																<td class='label'>Phone: </td><td class='input'>{20}</td>
																<td class='label'>Country: </td><td class='input'>{19}</td>
															</tr>
															<tr>
																<td class='label'>Street: </td><td class='input'>{15}</td>
																<td class='label'>City: </td><td class='input'>{16}</td>
																<td class='label'>State: </td><td class='input'>{17}</td>
																<td class='label'>Zip: </td><td class='input'>{18}</td>
															</tr>
															<tr>
																<td class='label' style='vertical-align:text-top;'>Notes:<br>{22}</td>
																<td class='input' colspan=7><textarea id=notesEdit_{0} rows=3 cols=80>{21}</textarea></td>
															</tr>
															<tr>
																<td class='label' colspan=8><a href='#' onclick={1}viewHistory({0},'Edit');{1}>View History</a></td>
															</tr>
														</table>
													</div>
												"
											, i
											, "\""
											, mEditDV[i]["credit_card_suffix"].ToString()
											, mEditDV[i]["exp"].ToString()
											, mEditDV[i]["paystatus"].ToString()
											, string.Format("{0:c}", mEditDV[i]["gross"])
											, string.Format("{0:c}", mEditDV[i]["openamount"])
											, mEditDV[i]["rpodoc"].ToString()
											, mEditDV[i]["invoice_date"].ToString()
											, mEditDV[i]["remark"].ToString()
											, mEditDV[i]["company"].ToString()
											, mEditDV[i]["maint_date"].ToString()
											, mEditDV[i]["cb"].ToString()
											, mEditDV[i]["card_first_name"].ToString()
											, mEditDV[i]["card_last_name"].ToString()
											, mEditDV[i]["card_street"].ToString()
											, mEditDV[i]["card_city"].ToString()
											, mEditDV[i]["card_state"].ToString()
											, mEditDV[i]["card_zip"].ToString()
											, mEditDV[i]["card_country"].ToString()
											, mEditDV[i]["card_phone"].ToString()
											, mEditDV[i]["notes"].ToString()
											, resizeSelect("notesEdit", i)
											, mEditDV[i]["cs"].ToString()
											, mEditDV[i]["contact_name"].ToString()
											, mEditDV[i]["contact_phone"].ToString()
											);



				}
				mRetVal.usHTML += string.Format(@"
												</div>
												<div class='scrollFoot'>	
													<div class='scrollFootLabel'>Total Amount: </div>
													<div class='scrollAmt'>{0}</div>
												</div>	
											</div>
										"
									, string.Format("{0:c}", totalAmount)
								   );
				#endregion

			}
			catch (Exception e)
			{
				mRetVal.error = true;
				logError("Common.cs", "edit", aQuery, thePage.Session["gNTName"].ToString(), e.ToString(), "ERROR");
			}
			finally
			{
				aConn.Close();
			}

			return mRetVal;
		}

		/**
		 * logError
		 * 
		 * logs an error
		 * */
		public void logError(string thePage,string theFunction, string theMessage, string theUser, string theError, string theType)
		{
			WSDBConnection aConn = new WSDBConnection();
			try
			{
				aConn.Open("JDE_WEB");
				WSProcedure aErrProc = aConn.InitProcedure("pkg_util.P_MSG_A");
                aErrProc.AddParam("pram_app", OracleDbType.VarChar, ParameterDirection.Input, "CCT");
                aErrProc.AddParam("pram_loc", OracleDbType.VarChar, ParameterDirection.Input, thePage);
                aErrProc.AddParam("pram_line", OracleDbType.VarChar, ParameterDirection.Input, theFunction);
                aErrProc.AddParam("pram_in", OracleDbType.VarChar, ParameterDirection.Input, theMessage);
                aErrProc.AddParam("pram_out", OracleDbType.VarChar, ParameterDirection.Input, theUser);
				aErrProc.AddParam("pram_serr", OracleDbType.Integer, ParameterDirection.Input, 0);
                aErrProc.AddParam("pram_err", OracleDbType.VarChar, ParameterDirection.Input, WSString.FilterSQL(theError));
                aErrProc.AddParam("pram_type", OracleDbType.VarChar, ParameterDirection.Input, theType);
                aErrProc.AddParam("pram_sec", OracleDbType.Integer, ParameterDirection.Input, 0);
				aErrProc.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				WSEmail mEmail = new WSEmail("CCT", "Error Error Email","Error Logging CCT Error",string.Format("{0}@willscot.com",theUser),
											 ConfigurationManager.AppSettings["ERROR_EMAIL"].ToString(),
											 string.Format(@"Error: {0}

Original Error
Page: {1}
Function: {2}
Message: {3}
User: {4}
Error: {5}
Type: {6}"
														,e.ToString()
														,thePage
														,theFunction
														,theMessage
														,theUser	
														,theError
														,theType
													));
				mEmail.Send();
			}
			finally
			{
				aConn.Close();
			}
		}


		/**
		 * emailWrapper
		 * 
		 * inserts the body pased in into a default email
		 * sends the message to the to / cc passed in
		 * if program is being run from devl adds a devl message and sends to user running program
		 * */
		public bool emailWrapper(string theTo, string theCC, string theSubject, string theBody, Page thePage, string theMSG)
		{
			return emailWrapper(theTo, theCC, theSubject, theBody, thePage, theMSG, false);
		}
		public bool emailWrapper(string theTo, string theCC, string theSubject, string theBody, Page thePage, string theMSG, bool theHIFlag)
		{

			WSEmail mEmail = new WSEmail("CCT", theMSG);

			AlternateView htmlView;

			LinkedResource logo;
			if (theHIFlag)
			{
				logo = new LinkedResource(String.Format("{0}\\include\\images\\HILogo_small.gif", ConfigurationManager.AppSettings["gRootPath"]));
			}
			else
			{
				logo = new LinkedResource(String.Format("{0}\\include\\images\\space_by_logo.gif", ConfigurationManager.AppSettings["gRootPath"]));
			}
			logo.ContentId = "companylogo";
			string mDevl = "";

			#region devl code

			if (thePage.Session["gServer"].ToString() != "wsa")
			{
				mDevl = string.Format(@"	
					<tr>
						<td id='devlNote'>
							<b>Development Notice</b><br>In production this email would go to: {0} and CC: {1}.
						</td>
					</tr>
					<tr>
						<td><hr size='1' noshade></td>
					</tr>", theTo, theCC);

				theTo = string.Format("{0}@willscot.com", thePage.Session["gNTName"]);
				theCC = "";
				theSubject = "Devl: " + theSubject;
			}
			#endregion

			#region email body
			htmlView = AlternateView.CreateAlternateViewFromString(string.Format(@"
						<html>
							<head>
								<style type='text/css'>
								a, a:hover, a:visited, a:active{0}
									color: blue;
									text-decoration: underline;
								{1}	
								td {0}
									font: 11px Verdana, Arial, Helvetica, sans-serif;
								{1}
								td.label {0}
									font: bold 11px Verdana, Arial, Helvetica, sans-serif;
									width:	120;
								{1}
								##mainTable {0}
									width: 680;
									border: 3px ridge;
									padding: 6px;
								{1}
								span##wsText {0}
									font: bold 19px Garamond, Times New Roman;
									letter-spacing: 3px;
									color: ##004000;
								{1}
								span##tmaText {0}
									font: bold 15px Garamond, Times New Roman;
									color: ##000000;
								{1}
								##captionTd {0}
									font:11px Verdana, Arial, Helvetica, sans-serif;
								{1}
								##linksTd {0}
									font: bold 11px Verdana, Arial, Helvetica, sans-serif;
									text-align: center;
									margin-top: 5px;
								{1}
								##footerTd {0}
									font: 9px Verdana, Arial, Helvetica, sans-serif; 
									text-align: center;
									margin-top: 5px;
								{1}
								##devlNote {0}
									font: 11px Verdana, Arial, Helvetica, sans-serif; 
									text-align: center;
									color:	red;
								{1}	
								</style>
							</head>
							<body>
							<table id='mainTable' width='680'>
								<tr>
									<td>
										<table border='0' cellspacing='0' cellpadding='0' width='100%'>
											<tr>
												<td>
													{5}
												</tr>
										</table>				
									</td>
								</tr>
								<tr>
									<td><hr size='1' noshade></td>
								</tr>
								{2}
								{3}
								<tr>
									<td><hr size='1' noshade></td>
								</tr>
								<tr>
									<td id='footerTd'>
										<i>This message has been automatically sent from the Williams Scotsman CCT Application.</i><br><br>
									</td>
								</tr>
							</table>
							</body>
							</html> 
							", "{"
							 , "}"
							 , mDevl
							 , theBody
							 , thePage.Session["gServer"].ToString()
							 , theHIFlag ? @"
								<span id='wsText'>Hawaii Modular Space</span><br>(An Operation of Williams Scotsman, Inc.)<br>
									<span id='tmaText'>CCT</span>	
								</td>			
								<td style='text-align:right;'><img src='cid:companylogo' width='81' height='50' alt='Hawaii Modular Space' border='0'></td>"
								:
								@"<span id='wsText'>Williams Scotsman</span><br>
									<span id='tmaText'>CCT</span>	
								</td>			
								<td style='text-align:right;'><img src='cid:companylogo' width='99' height='50' alt='Williams Scotsman' border='0'></td>"				
							),null,"text/html"
							); 

			#endregion

			mEmail.SetSender(thePage.Session["gNTName"].ToString() + "@willscot.com");
			mEmail.SetRecipient(theTo);
			if (theCC != "") { mEmail.SetCC(theCC); }
			mEmail.SetSubject(theSubject);
			htmlView.LinkedResources.Add(logo);
			mEmail.AddAlternateView(htmlView);
			return mEmail.Send();
		}



        /*  boa code

        public void loadApplication(HttpApplicationState theApp)
        {
            string[] aBOASelects = new string[2]; //holds selects for boa search page
            WSDBConnection aConn = new WSDBConnection();
            OracleDataReader aDR;
            ListItem[] aTypeArr = null;
            ListItem[] aStatusArr = null; 
            int aTypeCT = 1;
            //int aStatusCT = 1;

            aStatusArr = new ListItem[4];
            aStatusArr[0] = new ListItem("All Status", "");
            aStatusArr[1] = new ListItem("Pending", "P");
            aStatusArr[2] = new ListItem("Complete", "C");
            aStatusArr[3] = new ListItem("Cancelled","X");
 
            if (aConn.Open("CCKEY")){

                
                 * SELECT id
                                              ,category_id
                                              ,member_desc
                                              ,COUNT(1) over(PARTITION BY category_id) ct
                                          FROM boa_lookup
                                         WHERE category_id IN (1, 2)
                                         ORDER BY category_id, sort_seq
                 

                aDR = aConn.GetReader(@"SELECT id
                                              ,category_id
                                              ,member_desc
                                              ,COUNT(1) over(PARTITION BY category_id) ct
                                          FROM boa_lookup
                                         WHERE category_id = 1 
                                         ORDER BY sort_seq");

                while (aDR.Read())
                {
                    //if (aDR["category_id"].ToString() == "1")
                    //{
                        if (aTypeArr == null) { 
                            aTypeArr = new ListItem[Convert.ToInt32(aDR["ct"])+1];
                            aTypeArr[0] = new ListItem("All Types","");
                        }
                        aTypeArr[aTypeCT] = new ListItem(aDR["member_desc"].ToString(),aDR["id"].ToString());
                        aTypeCT++;
                    //}
                    //else
                    //{
                    //    if (aStatusArr == null) { 
                    //        aStatusArr = new ListItem[Convert.ToInt32(aDR["ct"])+1];
                    //        aStatusArr[0] = new ListItem("All Status", "");
                    //    }
                    //    aStatusArr[aStatusCT] = new ListItem(aDR["member_desc"].ToString(),aDR["id"].ToString());
                    //    aStatusCT++;
                    //}
                }
                aDR.Close();
                aDR.Dispose();
            }
            aConn.Close();

            theApp["boaType"] = aTypeArr;
            theApp["boaStatus"] = aStatusArr;
        }
*/
	}
}