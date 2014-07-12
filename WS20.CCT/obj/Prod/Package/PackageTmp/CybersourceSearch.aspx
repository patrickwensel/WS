<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CybersourceSearch.aspx.cs" Inherits="WS20.CCT.CybersourceSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register TagPrefix="WS" Namespace="WS20.Framework.Controls" Assembly="WS20.Framework" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=1.0.10618.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>
<%@ Register TagPrefix="WS" Namespace="WS20.CCT.Controls" Assembly="WS20.CCT" %>
<%@ Register TagPrefix="WS" Namespace="WS20.CCT" Assembly="WS20.CCT" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Cybersource Search</title>
    
    <link href="include/cctStyle.css" type="text/css" rel="stylesheet"/>
	<link href="include/tabstyle.css" type="text/css" rel="stylesheet"/>
	<link href="include/printStyle.css" type="text/css" rel="stylesheet" media="print"/>
	
	<script language="JavaScript" type="text/javascript" src="include/ws-lib.js"></script>
    
    <script language='javascript' type="text/javascript" src='include/getDate.js'></script>
    

</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager" OnAsyncPostBackError="HandleError" runat="server" AsyncPostBackTimeout="3000" ></asp:ScriptManager>
        <script language='JavaScript' type="text/javascript" src='include/common.js'></script>
        <script language='JavaScript' type="text/javascript" src='include/cyber.js'></script>
        
        <input type="hidden" value="" runat="server" id="mHeaderClientID" />
        <input type="hidden" id="CyberSortBy" value="" runat="server" />
        <WS:WSHeader ID="mWSHeader" runat="server" AppLogoSrc="images/cct.gif"/>
        
        <div id="main_body" class="main_body">
        <div id="mBOASearchHead" class="searchHead">
            <table>
                <tr>
                    <td class="input">Transaction ID:</td><td><input class="flat" id="mCyberTransID" runat="server" onkeypress="CyberAlphaBlock();" /></td>
                    <td class="input">Cnfrm #:</td><td colspan="3"><input class="flat" id="mCyberCnfrmNum" runat="server" onkeypress="alphaBlock();" /></td>  
                </tr>
                <tr>
                    <td class="input">Customer "C" #:</td><td><input class="flat" id="mCyberC" runat="server" onkeypress="alphaBlock();" /></td>
                    <td class="input">Customer Name:</td><td colspan="3"><input class="flat" id="mCyberCustName" runat="server" /></td>
                </tr>
                <tr>
                    <td class="input">Customer "CB" #:</td><td><input class="flat" id="mCyberCB" runat="server" onkeypress="alphaBlock();" /></td>
                    <td class="input">ODOC:</td><td colspan="3"><input class="flat" id="mCyberOdoc" runat="server" onkeypress="alphaBlock();"/></td>    
                </tr>
                <tr>
                    <td class="input">Customer "CS" #:</td><td><input class="flat" id="mCyberCS" runat="server" onkeypress="alphaBlock();" /></td>
                    <td class="input">Invoice #:</td><td colspan="3"><input class="flat" id="mCyberDCT" size="2" maxlength="2" runat="server" />&nbsp;<input class="flat" id="mCyberDOC" runat="server" onkeypress="alphaBlock();" />&nbsp;<input class="flat" id="mCyberKCO" size="5" maxlength="5" runat="server" /></td>
                </tr>
                <tr>
                    <td class="input">Order #:</td><td><input id="mCyberOrder" class="flat" runat="server" onkeypress="alphaBlock();" /></td>
                    <td class="input">Create Date:</td><td><asp:TextBox CssClass="flat" onblur="ws.FormatDate(this);" id="mCyberCreateDateFrom" runat="server" ></asp:TextBox>
                        <cc1:CalendarExtender ID="CalendarExtender4" TargetControlID="mCyberCreateDateFrom" runat="server">
                        </cc1:CalendarExtender>
                        <%-- size="10" <a href="javascript:show_calendar('mForm.mCyberCreateDateFrom');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                        <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
			        </td>
                    
			        <td class="input"> - </td><td><asp:TextBox CssClass="flat" id="mCyberCreateDateTo" runat="server"></asp:TextBox>
                        <cc1:CalendarExtender ID="CalendarExtender5" TargetControlID="mCyberCreateDateTo" runat="server">
                        </cc1:CalendarExtender>
                        <%--size="10" onBlur="ws.FormatDate(this);" <a href="javascript:show_calendar('mForm.mCyberCreateDateTo');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                        <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
			        </td>
                </tr>
                <tr>
                    <td class="input">Credit Card Suffix:</td><td><input class="flat" id="mCyberSuffx" maxlength="4" size="4" runat="server" onkeypress="alphaBlock();" /></td>
                    <td class="input">Charge Date:</td><td><asp:TextBox CssClass="flat" id="mCyberChargeDateFrom"  runat="server" ></asp:TextBox>
                    <cc1:CalendarExtender ID="CalendarExtender6" TargetControlID="mCyberChargeDateFrom" runat="server">
                        </cc1:CalendarExtender>
                        <%--size="10" onblur="ws.FormatDate(this);"<a href="javascript:show_calendar('mForm.mCyberChargeDateFrom');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                        <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
			        </td>
			        <td class="input"> - </td><td><asp:TextBox CssClass="flat" id="mCyberChargeDateTO" runat="server" ></asp:TextBox>
			        <cc1:CalendarExtender  ID="CalendarExtender3" TargetControlID="mCyberChargeDateTO" runat="server">
                        </cc1:CalendarExtender>
                   <%--  size="10" onBlur="ws.FormatDate(this);"<a href="javascript:show_calendar('mForm.mCyberChargeDateTO');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                        <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
			        </td>
                </tr>
                <tr>
                    <td class="input"></td><td></td>
                    <td class="input">Returned Date:</td><td><asp:TextBox CssClass="flat" id="mCyberReturnDateFrom"  runat="server"></asp:TextBox>
                        <cc1:CalendarExtender ID="CalendarExtender1" TargetControlID="mCyberReturnDateFrom"  runat="server">
                        </cc1:CalendarExtender>
                        <%--size="10"  onblur="ws.FormatDate(this);" <a href="javascript:show_calendar('mForm.mCyberReturnDateFrom');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                        <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
			        </td>
			        <td class="input"> - </td><td><asp:TextBox CssClass="flat" id="mCyberReturnDateTo" runat="server" ></asp:TextBox>
                        <cc1:CalendarExtender ID="CalendarExtender2" TargetControlID="mCyberReturnDateTo" runat="server">
                        </cc1:CalendarExtender>
                        <%--size="10" onBlur="ws.FormatDate(this);" <a href="javascript:show_calendar('mForm.mCyberReturnDateTo');" style="vertical-align:bottom;" onmouseover="window.status='Date Picker';return true;" onmouseout="window.status='';return true;" tabindex="-1">
                        <img src="images/cal.gif" width="19" height="18" border="0" tabindex="-1"/></a>--%>
			        </td>
                </tr>
                <tr>
                    <td class="input">Transaction Status:</td>
                    <td>
                        <select id="mCyberStatusID" runat="server">
                            <option value="">All Status</option>
                            <option value="P">Pending</option>
                            <option value="C">Complete</option>
                            <option value="X">Incomplete</option>
                            <option value="R">Returned</option>
                        </select>
                    </td>
                    <td class="input">Transaction Type:</td><td colspan="3">
                        <select id="mCyberTypeID" runat="server">
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
                        <select id="mCyberCCType" runat="server">
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
                        <input id="mCyberRowsPerPage" value="50" size="3" maxlength="5" onkeypress="alphaBlock();" runat="server" />
                     </td>
                </tr>
                
                <tr>
                    <td id="Td1" colspan="6" class="center" runat="server">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <span id="mCyberSearchMsgDiv" runat="server"><br /></span>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="mCyberSearchButton" EventName="Click" />
                                <%--<asp:AsyncPostBackTrigger ControlID="mCyberSearchClear" EventName="Click" />--%>
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" class="center">
                        <%--<input class="button" type="button" id="mCyberSearchButton" value="Search" onclick="CyberSearch()"\>
                        <input class="button" type="button" id="mCyberSearchClear" value="Clear" onclick="CyberSearchClear()"\>
                        --%>
                        <asp:Button ID="mCyberSearchButton" Text="Search" OnClick="mCyberSearchButton_Click" runat="server" />
                        <input type="button" id="mCyberSearchClear" value="Clear" onclick="CyberSearchClear()" />
                    </td>
                </tr>
            </table>
        </div>
        </div>
   
        <div>&nbsp;</div>
        <div>&nbsp;</div>

        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div id="mCyberSearchLoadingDiv" class="loadingDiv" runat="server" style="display:none">Searching ...</div>
                <div id="mCyberResultsDiv" runat="server"></div> 
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="mCyberSearchButton" EventName="Click" />
                <%--<asp:AsyncPostBackTrigger ControlID="mCyberSearchClear" EventName="Click" />--%>
            </Triggers>
        </asp:UpdatePanel>
                            
       <WS:WSFooter ID="mWSFooter" runat="server" />    
    </form>
</body>
</html>