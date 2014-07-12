<%@ Page Language="C#" AutoEventWireup="true" Inherits="WS20.CCT.index"  Codebehind="index.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=1.0.10618.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>
<%@ Register TagPrefix="WS" TagName="WSHeader" Src="~/WSHeader.ascx" %>
<%@ Register TagPrefix="WS" TagName="WSFooter" Src="~/WSFooter.ascx" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server" >
	<link href="include/cctStyle.css" type="text/css" rel="stylesheet"/>
	<link href="include/tabstyle.css" type="text/css" rel="stylesheet"/>
	<link href="include/printStyle.css" type="text/css" rel="stylesheet" media="print"/>
	
	<script language="JavaScript" type="text/javascript" src="include/ws-lib.js"></script>
    <script language='javascript' type="text/javascript" src='include/getDate.js'></script>
    
	<script language='JavaScript' type="text/javascript" src='include/trans.js'></script>
	<script language='JavaScript' type="text/javascript" src='include/search.js'></script>
	<script language='JavaScript' type="text/javascript" src='include/credit.js'></script>
	<script language='JavaScript' type="text/javascript" src='include/edit.js'></script>
	
    <title>CCT</title>
</head>
<body id="mBody" runat="server"> 
    
    <form id="mForm" runat="server">
        <asp:ScriptManager ID="ScriptManager" OnAsyncPostBackError="HandleError" runat="server" AsyncPostBackTimeout="3000" ></asp:ScriptManager>
        <script language='JavaScript' type="text/javascript" src='include/common.js'></script>
        <script language='JavaScript' type="text/javascript" src='include/boa.js'></script>
        
        <input type="hidden" value="" runat="server" id="mHeaderClientID" />
        <input type="hidden" id="boaSortBy" value="" runat="server" />
        <WS:WSHeader ID="mWSHeader" runat="server" AppLogoSrc="images/cct.gif"/>
        
        <div id="main_body" class="main_body">
    
			<div id="gErrorDiv" class="errorDiv"></div>
            
            <div id="mMainMenu" style="display:none;">
                <div id="mMenuTrans" class="MainMenuFont" >
                        <a class="MainMenuFont" id="mMenuTransA" href="#" >
                            <span id="mMenuTransSpan">Transactions</span>
                            <br /><span class="MainMenuFont" id="mMenuTransUSSpan">&nbsp;</span>
                            <br /><span class="MainMenuFont" id="mMenuTransCASpan">&nbsp;</span>
                        </a>
                </div>
                <div>&nbsp;</div>
                <div style="width:49%;float:left;">
                    <div class="MainMenuFont" id="mMenuSearch" >
                        <a class="MainMenuFont" id="mMenuSearchA" href="#" >
                            <span id="mMenuSearchSpan">Search</span>
                            <br /><span class="MainMenuFont" id="mMenuSearchUSSpan">&nbsp;</span>
                            <br /><span class="MainMenuFont" id="mMenuSearchCASpan">&nbsp;</span>
                        </a>
                    </div>
                </div>
                <div style="width:49%;float:right;">
                    <div class="MainMenuFont" id="mMenuCredit" >
                        <a class="MainMenuFont" id="mMenuCreditA" href="#" >
                            <span id="mMenuCreditSpan">Credit</span>
                            <br /><span class="MainMenuFont" id="mMenuCreditUSSpan">&nbsp;</span>
                            <br /><span class="MainMenuFont" id="mMenuCreditCASpan">&nbsp;</span>
                        </a>
                     </div>
                </div>
            </div>
            
            <div id="mTransactions" style="display:none">                
                <div id="mTransLoadingDiv" class='loadingDiv' style="display:none">Loading ... </div>
                <div class="wsTabSet" id="tabSet" style="display:none">
			        <a class="wsTabHead">US</a> 
			        <a class="wsTabHead">Canada</a>
			        <div class="wsTabPage"> 
			            <div class="printLink" style="width:1057px;">&nbsp;<a href="#" onclick="printPage('P');return false;">Print</a>&nbsp;|&nbsp;<a href="#" onclick="printPage('D','gUSTransDV');return false;">Export to Excel</a></div>                
                        <div id="mTransUSTableDiv" ></div>
                        <div>&nbsp;</div>
                        <div id="mTransUSErrorDiv" style="width:1057px;"></div>
                        <div>&nbsp;</div>
                        <div class="transTable">
                            <input class="button" type="button" id="mSubmitTransUS" value="Submit" onclick="updateUSTrans();" />
                            <input class="button" type="button" id="mRefreshUSTrans" value="Refresh" onclick="getTrans();" />
                        </div>
                    </div>
                    <div class="wsTabPage" style="display:none">
                        <div class="printLink" style="width:1057px;">&nbsp;<a href="#" onclick="printPage('P');return false;">Print</a>&nbsp;|&nbsp;<a href="#" onclick="printPage('D','gCATransDV');return false;">Export to Excel</a></div>
                        <div id="mTransCATableDiv"></div>
                        <div>&nbsp;</div>
                        <div id="mTransCAErrorDiv" style="width:1057px;"></div>
                        <div>&nbsp;</div>
                        <div class="transTable">
                            <input class="button" type="button" id="mSubmitTransCA" value="Submit" onclick="updateCATrans();" />
                            <input class="button" type="button" id="mRefreshCATrans" value="Refresh" onclick="getTrans();" />
                        </div>
                        <div>&nbsp;</div>
                        <div class="transTable" id="transFileSel">
                            Select Previously Created File:&nbsp;<span class="fileSelect" id="mCATransFileDiv"></span>
                        </div>
                    </div>
                </div>
            </div> <%--close transactions--%>
            
            <div id="mSearch" style="display:none">
                <div id="mSearchHead" class="searchHead">
                    <table>
                        <tr>
                            <td class="input">Transaction ID:</td><td colspan="3"><input class="flat" id="mSearchTransID" runat="server" onkeypress="alphaBlock();" /></td>
                        </tr>
                        <tr>
                            <td class="input">Customer "CB" #:</td><td><input class="flat" id="mSearchCustNum" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Customer Name:</td><td colspan="3"><input class="flat" id="mSearchCustName" runat="server" /></td>
                            
                        </tr>
                        <tr>
                            <td class="input">Customer "CS" #:</td><td><input class="flat" id="mSearchCSNum" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">ODOC:</td><td colspan="3"><input class="flat" id="mSearchODOC" runat="server" onkeypress="alphaBlock();"/></td>    
                        </tr>
                        <tr>
                            <td class="input">Order #:</td><td><input class="flat" id="mSearchOrderNum" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Invoice #:</td><td colspan="3"><input class="flat" id="mSearchDCT" size="2" maxlength="2" runat="server" />&nbsp;<input class="flat" id="mSearchDOC" runat="server" onkeypress="alphaBlock();" />&nbsp;<input class="flat" id="mSearchKCO" size="5" maxlength="5" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="input">Credit Card Suffix:</td><td><input class="flat" id="mSearchCCSufix" maxlength="4" size="4" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Sent Date:</td><td><input class="flat" id="mSearchSentFrom" size="10" onblur="ws.FormatDate(this);" runat="server" />
                            <a href="javascript:show_calendar('mForm.mSearchSentFrom');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"></a>
						    </td>
						    <td class="input"> - </td><td><input class="flat" id="mSearchSentTo" size="10" onblur="ws.FormatDate(this);" runat="server" />
                            <a href="javascript:show_calendar('mForm.mSearchSentTo');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1" /></a>
						    </td>
                        </tr>
                        <tr>
                            <td class="input">Transaction Status:</td><td id="mSearchStatusSelectDiv" runat="server">Loading ...</td>
                            <td class="input">Transaction Type:</td><td id="mSearchTypeSelectDiv" runat="server">Loading ...</td>
                        </tr>
                        <tr>
                        <td class="input">Card Type:</td><td>
                                                                <select id="mSearchCardType" runat="server">
                                                                    <option value="">All</option>
                                                                    <option value="3">American Express</option>
                                                                    <option value="54">MasterCard / Visa</option>
                                                                    <option value="5">MasterCard</option>
                                                                    <option value="4">Visa</option>
                                                                </select>
                                                             </td>
                        </tr>
                        <tr>
                            <td colspan="6" class="center" id="mSearchErrorTD"><br /></td>
                        </tr>
                        <tr>
                            <td colspan="6" class="center">
                                <input class="button" type="button" id="mSearchButton" disabled="disabled" value="Search" onclick="search()"\>
                                <input class="button" type="button" id="mSearchClearButton" disabled="disabled" value="Clear" onclick="searchClear()"\>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>&nbsp;</div>
                <div>&nbsp;</div>
                <div id="mSearchLoadingDiv" class="loadingDiv" style="display:none">Searching ...</div>
                <%--possible error with ie 5.5 need to code in id check for this div--%>
                <div class="wsTabSet" id="searchTabSet" style="display:none">
			        <a class="wsTabHead">US</a> 
			        <a class="wsTabHead">Canada</a>
			        <div class="wsTabPage">
			            <div class="printLink" style="width:1031px;">&nbsp;<a href="#" onclick="printPage('P');return false;">Print</a>&nbsp;|&nbsp;<a href="#" onclick="printPage('D','gUSSearchDV');return false;">Export to Excel</a></div>
                        <div id="mSearchUSTableDiv"></div>
                        <div>&nbsp;</div>
                        <div id="mSearchUSErrorDiv" style="width:1031px;"></div>
                        <div>&nbsp;</div>
                        <div class="searchTable"><input class="button" type="button" id="mSearchUSUpdateButton" value="Update" onclick="updateUSSearch();" /></div>
                    </div>
                    <div class="wsTabPage" style="display:none">
                        <div class="printLink" style="width:1031px;">&nbsp;<a href="#" onclick="printPage('P');return false;">Print</a>&nbsp;|&nbsp;<a href="#" onclick="printPage('D','gCASearchDV');return false;">Export to Excel</a></div>
                        <div id="mSearchCATableDiv"></div>
                        <div>&nbsp;</div>
                        <div id="mSearchCAErrorDiv" style="width:1031px;"><br /></div>
                        <div>&nbsp;</div>
                        <div class="searchTable"><input class="button" type="button" id="mSearchCAUpdateButton" value="Update" onclick="updateCASearch();" /></div>
                    </div>  
                </div>
            </div><%--close search--%>
            
            <div id="mCredit" style="display:none">
                <div id="mCreditHead" class="searchHead">
                    <table>
                        <tr>
                            <td class="input">Transaction ID:</td><td><input class="flat" id="mCreditTransID" runat="server" onkeypress="alphaBlock();" /></td>
                            
                        </tr>
                        <tr>
                            <td class="input">Customer "CB" #:</td><td><input class="flat" id="mCreditCustNum" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Customer Name:</td><td colspan="3"><input class="flat" id="mCreditCustName" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="input">Customer "CS" #:</td><td><input class="flat" id="mCreditCSNum" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">ODOC:</td><td colspan="3"><input class="flat" id="mCreditODOC" runat="server" onkeypress="alphaBlock();"/></td>
                        </tr>
                        <tr>
                            <td class="input">Order #:</td><td><input class="flat" id="mCreditOrderNum" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Invoice #:</td><td colspan="3"><input class="flat" id="mCreditDCT" size="2" maxlength="2" runat="server" />&nbsp;<input class="flat" id="mCreditDOC" runat="server" onkeypress="alphaBlock();" />&nbsp;<input class="flat" id="mCreditKCO" size="5" maxlength="5" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="input">Credit Card Suffix:</td><td><input class="flat" id="mCreditCCSufix" maxlength="4" size="4" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Sent Date:</td><td><input class="flat" id="mCreditSentFrom" size="10" onblur="ws.FormatDate(this);" runat="server" />
                            <a href="javascript:show_calendar('mForm.mCreditSentFrom');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1" /></a>
						    </td>
						    <td class="input"> - </td><td><input class="flat" id="mCreditSentTo" size="10" onblur="ws.FormatDate(this);" runat="server" />
                            <a href="javascript:show_calendar('mForm.mCreditSentTo');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>
						    </td>
                        </tr>
                        <tr>
<%--                            <td class="input">Transaction Status:</td><td id=mCreditStatusSelect runat="server">Loading ...</td>--%>
                            <td class="input">Transaction Type:</td><td id="mCreditTypeSelectDiv" runat="server">Loading ...</td>
                            <td class="input">Card Type:</td><td colspan="3">
                                                                <select id="mCreditCardType" runat="server">
                                                                    <option value="">All</option>
                                                                    <option value="3">American Express</option>
                                                                    <option value="54">MasterCard / Visa</option>
                                                                    <option value="5">MasterCard</option>
                                                                    <option value="4">Visa</option>
                                                                </select>
                                                             </td>
                        </tr>
                        <tr>
                            <td colspan="6" class="center" id="mCreditErrorTD"><br /></td>
                        </tr>
                        <tr>
                            <td colspan="6" class="center">
                                <input class="button" type="button" id="mCreditButton" disabled="disabled" value="Search" onclick="creditSearch()"/>
                                <input class="button" type="button" id="mCreditClearButton" disabled="disabled" value="Clear" onclick="creditClear()"/>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>&nbsp;</div>
                <div>&nbsp;</div>
                <div id="mCreditLoadingDiv" class='loadingDiv' style="display:none">Searching ...</div>
                <%--possible error with ie 5.5 need to hard code div id into wsTab.js --%>
                <div class="wsTabSet" id="creditTabSet" style="display:none">
			        <a class="wsTabHead">US</a> 
			        <a class="wsTabHead">Canada</a>
			        <div class="wsTabPage"> 
			            <div class="printLink" style="width:1097px;">&nbsp;<a href="#" onclick="printPage('P');return false;">Print</a>&nbsp;|&nbsp;<a href="#" onclick="printPage('D','gUSCreditDV');return false;">Export to Excel</a></div>
                        <div id="mCreditUSTableDiv"></div>
                        <div>&nbsp;</div>
                        <div id="mCreditUSErrorDiv" style="width:1097px;"><br /></div>
                    </div>
                    <div class="wsTabPage" style="display:none">
                        <div class="printLink" style="width:1097px;">&nbsp;<a href="#" onclick="printPage('P');return false;">Print</a>&nbsp;|&nbsp;<a href="#" onclick="printPage('D','gCACreditDV');return false;">Export to Excel</a></div>
                        <div id="mCreditCATableDiv"></div>
                        <div>&nbsp;</div>
                        <div id="mCreditCAErrorDiv" style="width:1097px;"><br /></div>
                        <div>&nbsp;</div>
                        <div class="creditTable" id="creditFileSel">Select Previously Created File:&nbsp;<span class="fileSelect" id="mCACreditFileDiv"></span></div>
                    </div>
                    
                </div>
            </div><%--close credit--%>
            
            <div id="mEdit" style="display:none">
                <div id="mEditHead" class="searchHead">
                    <table>
                        <tr>
                            <td class="input">Transaction ID:</td><td><input class="flat" id="mEditTransID" runat="server" onkeypress="alphaBlock();" /></td>
                        </tr>
                        <tr>
                            <td colspan="6" class="center" id="mEditErrorTD"><br /></td>
                        </tr>
                        <tr>
                            <td colspan="6" class="center">
                                <input class="button" type="button" id="mEditButton" value="Edit" onclick="editSearch()"/>
                                <input class="button" type="button" id="mEditClearButton" value="Clear" onclick="editClear()"/>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>&nbsp;</div>
                <div>&nbsp;</div>
                <div id="mEditLoadingDiv" class='loadingDiv' style="display:none">Searching ...</div>
                <div id="mEditTableDiv"></div>
                <div>&nbsp;</div>
                <div id="mEditErrorDiv" style="width:1097px;"><br /></div>
            </div> 
            
            <div id="mBOASearch" style="display:none;">
                <div id="mBOASearchHead" class="searchHead">
                    <table>
                        <tr>
                            <td class="input">Transaction ID:</td><td><input class="flat" id="mBOATransID" runat="server" onkeypress="boaAlphaBlock();" /></td>
                            <td class="input">Cnfrm #:</td><td colspan="3"><input class="flat" id="mBOACnfrmNum" runat="server" onkeypress="alphaBlock();" /></td>  
                        </tr>
                        <tr>
                            <td class="input">Customer "C" #:</td><td><input class="flat" id="mBOAC" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Customer Name:</td><td colspan="3"><input class="flat" id="mBOACustName" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="input">Customer "CB" #:</td><td><input class="flat" id="mBOACB" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">ODOC:</td><td colspan="3"><input class="flat" id="mBOAOdoc" runat="server" onkeypress="alphaBlock();"/></td>    
                        </tr>
                        <tr>
                            <td class="input">Customer "CS" #:</td><td><input class="flat" id="mBOACS" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Invoice #:</td><td colspan="3"><input class="flat" id="mBOADCT" size="2" maxlength="2" runat="server" />&nbsp;<input class="flat" id="mBOADOC" runat="server" onkeypress="alphaBlock();" />&nbsp;<input class="flat" id="mBOAKCO" size="5" maxlength="5" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="input">Order #:</td><td><input id="mBOAOrder" class="flat" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Create Date:</td><td><asp:TextBox CssClass="flat" onblur="ws.FormatDate(this);" id="mBOACreateDateFrom" runat="server" ></asp:TextBox>
                                <cc1:CalendarExtender ID="CalendarExtender4" TargetControlID="mBOACreateDateFrom" runat="server">
                                </cc1:CalendarExtender>
                                <%-- size="10" <a href="javascript:show_calendar('mForm.mBOACreateDateFrom');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
						    </td>
                            
						    <td class="input"> - </td><td><asp:TextBox CssClass="flat" id="mBOACreateDateTo" runat="server"></asp:TextBox>
                                <cc1:CalendarExtender ID="CalendarExtender5" TargetControlID="mBOACreateDateTo" runat="server">
                                </cc1:CalendarExtender>
                                <%--size="10" onBlur="ws.FormatDate(this);" <a href="javascript:show_calendar('mForm.mBOACreateDateTo');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
						    </td>
                        </tr>
                        <tr>
                            <td class="input">Credit Card Suffix:</td><td><input class="flat" id="mBOASuffx" maxlength="4" size="4" runat="server" onkeypress="alphaBlock();" /></td>
                            <td class="input">Charge Date:</td><td><asp:TextBox CssClass="flat" id="mBOAChargeDateFrom"  runat="server" ></asp:TextBox>
                            <cc1:CalendarExtender ID="CalendarExtender6" TargetControlID="mBOAChargeDateFrom" runat="server">
                                </cc1:CalendarExtender>
                                <%--size="10" onblur="ws.FormatDate(this);"<a href="javascript:show_calendar('mForm.mBOAChargeDateFrom');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
						    </td>
						    <td class="input"> - </td><td><asp:TextBox CssClass="flat" id="mBOAChargeDateTO" runat="server" ></asp:TextBox>
						    <cc1:CalendarExtender  ID="CalendarExtender3" TargetControlID="mBOAChargeDateTO" runat="server">
                                </cc1:CalendarExtender>
                           <%--  size="10" onBlur="ws.FormatDate(this);"<a href="javascript:show_calendar('mForm.mBOAChargeDateTO');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
						    </td>
                        </tr>
                        <tr>
                            <td class="input"></td><td></td>
                            <td class="input">Returned Date:</td><td><asp:TextBox CssClass="flat" id="mBOAReturnDateFrom"  runat="server"></asp:TextBox>
                                <cc1:CalendarExtender ID="CalendarExtender1" TargetControlID="mBOAReturnDateFrom"  runat="server">
                                </cc1:CalendarExtender>
                                <%--size="10"  onblur="ws.FormatDate(this);" <a href="javascript:show_calendar('mForm.mBOAReturnDateFrom');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
						    </td>
						    <td class="input"> - </td><td><asp:TextBox CssClass="flat" id="mBOAReturnDateTo" runat="server" ></asp:TextBox>
                                <cc1:CalendarExtender ID="CalendarExtender2" TargetControlID="mBOAReturnDateTo" runat="server">
                                </cc1:CalendarExtender>
                                <%--size="10" onBlur="ws.FormatDate(this);" <a href="javascript:show_calendar('mForm.mBOAReturnDateTo');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                                <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
						    </td>
                        </tr>
                        <tr>
                            <td class="input">Transaction Status:</td>
                            <td>
                                <select id="mBOAStatusID" runat="server">
                                    <option value="">All Status</option>
                                    <option value="P">Pending</option>
                                    <option value="C">Complete</option>
                                    <option value="X">Incomplete</option>
                                    <option value="R">Returned</option>
                                </select>
                            </td>
                            <td class="input">Transaction Type:</td><td colspan="3">
                                <select id="mBOATypeID" runat="server">
                                    <option value="">All Types</option>
                                    <option value="AC">All Charges</option>
                                    <option value="AO">&nbsp;&nbsp;&nbsp;All Onetime</option>
                                    <option value="WO">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WS - Onetime</option>
                                    <option value="PL">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Portal - Login</option>
                                    <option value="M">&nbsp;&nbsp;&nbsp;MAM</option>
                                    <option value="PE">Portal - Enrollment</option>
                                    <option value="PR">Portal - Recurring</option>
                                    <option value="PM">Portal - Modify</option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td class="input">Account Type:</td>
                            <td>
                                <select id="mBOACCType" runat="server">
                                    <option value="">All</option>
                               <%--     <optgroup label="Bank Account">
                                        <option value="B">Business Checking</option>
                                        <option value="BS">Business Savings</option>
                                        <option value="C">Personal Checking</option>
                                        <option value="S">Personal Savings</option>--%>
                                        <option value="ACH">ACH</option>
                                   <%-- </optgroup>
                                    <optgroup label="Credit Card">--%>
                                        <option value="AMEX">American Express</option>
                                        <option value="MSTRVISA">MasterCard/Visa</option>
                                        <option value="MC">MasterCard</option>
                                        <option value="VISA">Visa</option>
                                        <%--  <option value="DISC">Discover</option>
                                    </optgroup>--%>
                                </select> 
                             </td>
                             <td class="input">Rows Per Page:</td>
                             <td colspan="3">
                                <input id="mBOARowsPerPage" value="50" size="3" maxlength="5" onkeypress="alphaBlock();" runat="server" />
                             </td>
                        </tr>
                        <tr>
                            <td id="Td1" colspan="6" class="center" runat="server">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <span id="mBOASearchMsgDiv" runat="server"><br /></span>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="mBOASearchButton" EventName="Click" />
                                        <%--<asp:AsyncPostBackTrigger ControlID="mBOASearchClear" EventName="Click" />--%>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" class="center">
                                <%--<input class="button" type="button" id="mBOASearchButton" value="Search" onclick="boaSearch()"\>
                                <input class="button" type="button" id="mBOASearchClear" value="Clear" onclick="boaSearchClear()"\>
                                --%>
                                <asp:Button ID="mBOASearchButton" Text="Search" OnClick="mBOASearchButton_Click" runat="server" />
                                <input type="button" id="mBOASearchClear" value="Clear" onclick="boaSearchClear()" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div>&nbsp;</div>
                <div>&nbsp;</div>

                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div id="mBOASearchLoadingDiv" class="loadingDiv" runat="server" style="display:none">Searching ...</div>
                        <div id="mBOAResultsDiv" runat="server"></div> 
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="mBOASearchButton" EventName="Click" />
                        <%--<asp:AsyncPostBackTrigger ControlID="mBOASearchClear" EventName="Click" />--%>
                    </Triggers>
                </asp:UpdatePanel>
                            
                
            </div>
        
        <iframe id="grayedOutIframe" style="display:none;"></iframe>
        <div id="grayedOutDiv" style="display:none;"></div>
        <div id="historyHead" style="display:none;">History<span style="font-size:11px;float:right;"><a href='#' onclick='closeHistory();'>Close</a></span></div>
        <div id="historyDiv" style="display:none;"></div>
        <span id="mJSConstSpan" runat="server"></span>
     </div>
    <WS:WSFooter ID="mWSFooter" runat="server" />
    </form>
</body>
</html>