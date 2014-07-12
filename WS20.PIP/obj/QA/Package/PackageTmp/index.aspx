<%@ Page Language="C#" AutoEventWireup="true" Codebehind="index.aspx.cs" Inherits="PIP._Default" %>

<%@ Register Assembly="AjaxControlToolkit, Version=1.0.10618.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- should move text here into web_lang table -->
    <pt:standard.displaymode pt:mode="Hosted" pt:title="Algeco Scotsman Intranet" pt:subtitle="Productivity Improvement Project"
        xmlns:pt='http://www.plumtree.com/xmlschemas/ptui/' />
    <title>Productivity Improvement Project</title>
    <link href="style.css" type="text/css" rel="stylesheet" />
    <link href="printStyle.css" type="text/css" rel="stylesheet" media="print"/>
	
    <script type="text/javascript" language="javascript" src='parsedate.js'></script>
    <script type="text/javascript" language="javascript" src='index.js'></script>
    <script type="text/javascript" language="javascript" src='drag.js'></script>

  <!-- per gary to get rid of space-->
    <!--[if LTE IE 7]>
		<style type="text/css">
			table#outerTable {
				position: relative;
				bottom: 10px;
			}
		</style>
	<![endif]-->
</head>
<body onload="init();">
    <form id="mEntryForm" enctype="multipart/form-data"  runat="server">
        <asp:ScriptManager ID="ScriptManager" LoadScriptsBeforeUI="true" runat="server">
        </asp:ScriptManager>
        <!-- this makes no sense, if loadscriptsbeforeui = false and I try to add the below script (in the tag) i get a weird error 
               OnAsyncPostBackError="HandleError" -->
        <input type="hidden" id="hiddenEntryAction" value="" runat="server" />
        <input type="hidden" id="hiddenSearchProjectTitle" value="" runat="server" />
        <input type="hidden" id="hiddenCurrCurrency" value="" runat="server" />
        <input type="hidden" id="hiddenReadonly" value="" runat="server" />
      
        <div class="mainPageDiv">
            <div class="titleDiv"><img id="imgHeader" alt="header" runat="server" /></div>
            <div class="langBarDiv">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <span id="labelLang" runat="server"></span>&nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="selLang" onchange="submitEntryForm('changeLang');" runat="server"></asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;
                <span id="labelSearchProjectTitle" runat="server"></span>&nbsp;&nbsp;&nbsp;
                <span id="selSearchProjectTitleTD" runat="server"></span>&nbsp;&nbsp;&nbsp;&nbsp;
                <input type="button" id="buttonSearch" onclick="submitEntryForm('search');" runat="server" />
            </div>
            <div class="pipBody">
                <div style="float:left;margin:5px;font-weight:bold;"><a id="mainMenuLink" runat="server"></a></div>
                <div style="float:right;margin:5px;font-weight:bold;"><a href="#" onclick="window.print();return false" id="printLink" runat="server">Print</a></div>
                <div class="appMsgDiv" id="appMsg" runat="server"></div>
                <div class="space"></div>
                <%--<div></div>--%>
                <div class="space"></div>
                <div id="mEntryPageDiv" runat="server">
                    <table cellspacing="0">
                        <tbody>
                            <tr><td id="labelMSG" runat="server" colspan="6"></td></tr>
                            <!-- Project Title -->
                            <tr>
                                <td id="labelProjectTitleReq" runat="server">*</td>
                                <td id="labelProjectTitle" runat="server">
                                </td>
                                <td colspan="4" id="labelProjectTitleInput" runat="server">
                                    <input type="text" id="inputProjectTitle" class="inputWide" runat="server" /></td>
                            </tr>
                            <tr id="projectTitleEnglishTR" runat="Server">
                                <td id="labelProjectTitleEnglishReq" runat="server">*</td>
                                <td id="labelProjectTitleEnglish" runat="server">
                                </td>
                                <td colspan="4" id="labelProjectTitleEnglishInput" runat="server">
                                    <input type="text" id="inputProjectTitleEnglish" class="inputWide" runat="server" /></td>
                            </tr>
                            <!-- Project Owner / Project ID -->
                            <tr>
                                <td>
                                </td>
                                <td class="label" id="labelProjectOwner" runat="server">
                                </td>
                                <td class="input">
                                    <%--<asp:DropDownList ID="selProjectOwner" runat="server"></asp:DropDownList>--%>
                                    <%--<asp:TextBox ID="inputProjectOwner" Width="245" runat="server"></asp:TextBox>
                                    <cc1:AutoCompleteExtender ID="autoExtenderProjectOwner" MinimumPrefixLength="1" ServiceMethod="getUser"
                                        TargetControlID="inputProjectOwner" runat="server">
                                    </cc1:AutoCompleteExtender>--%>
                                    <asp:DropDownList ID="selProjectOwner" style="width:250px;" runat="server"></asp:DropDownList>
                                </td>
                                <td></td>
                                <td class="label" id="labelProjectID" runat="server">
                                </td>
                                <td class="inputR">
                                    <input type="text" id="inputProjectID" style="width:169px;" runat="server" readonly="readonly"
                                        class="readonly" />&nbsp;</td>
                            </tr>
                            <!-- Branch  /  -->
                            <tr>
                                <td id="labelBranchReq" runat="server">
                                </td>
                                <td id="labelBranch" runat="server">
                                </td>
                                <td id="labelBranchInput" colspan="4" runat="server">
                                    <asp:DropDownList ID="selBranch" style="width:250px;" runat="server">
                                    </asp:DropDownList></td>
                            </tr>
                            <!-- Key Team / Products -->
                            <tr>
                                <td>
                                </td>
                                <td class="label" id="labelTeamMembers" runat="server">
                                </td>
                                <td class="input">
                                    <%--<asp:TextBox ID="inputTeamMembers" Width="245" runat="server"></asp:TextBox><input type="button" id="buttonTeamMembersAdd" onclick="addTeamMember()" runat="server" />
                                    <cc1:AutoCompleteExtender ID="autoExtenderTeamMembers" MinimumPrefixLength="1"  ServiceMethod="getUser"
                                        TargetControlID="inputTeamMembers" ServicePath="portalUsers.asmx" runat="server">
                                    </cc1:AutoCompleteExtender>--%>
                                    <%--<asp:DropDownList ID="selOneTeamMembers" style="width:250px;" runat="server"></asp:DropDownList>--%>
                                    <input type="text" ID="selOneTeamMembers" style="width:210px;" runat="server" /><input type="button" id="buttonTeamMembersAdd" onclick="addTeamMember()" runat="server" />

                                    <%--<asp:ListBox ID="selTeamMembers" SelectionMode="Multiple" runat="server"></asp:ListBox>--%></td>
                                <td id="labelProductReq" runat="server"></td>
                                <td id="labelProduct" runat="server"></td>
                                <td id="labelProductInput" runat="server">
                                    <asp:DropDownList ID="selProduct" style="width:175px;" runat="server">
                                    </asp:DropDownList>&nbsp;</td>
                            </tr>
                             <!-- Remove Key Team /process  -->
                            <tr>
                                <td>
                                </td>
                                <td class="input" style="text-align:right;">
                                    <input type="button" id="buttonTeamMembersRemove" onclick="removeTeamMember()" runat="server" />
                                </td>
                                <td class="input" rowspan="2">
                                    <input type="hidden" id="hiddenTeamMembers" runat="server" />
                                    <select id="selTeamMembers" multiple="true" style="width:250px;" runat="server"></select>
                                    </td>
                               <td id="labelProcessReq" runat="server"></td>
                               <td id="labelProcess" runat="server"></td>
                               <td id="labelProcessInput" runat="server">
                                    <asp:DropDownList ID="selProcess" style="width:175px;" runat="server">
                                    </asp:DropDownList>&nbsp;</td>
                            </tr>
                            <!--status-->
                            <tr>
                                <td></td><td></td>
                                <td></td>
                                <td class="label" id="labelStatus" runat="server">
                                </td>
                                <td class="inputR" valign="top">
                                    <asp:DropDownList ID="selStatus" style="width:175px;" onchange="setReq();" runat="server">
                                    </asp:DropDownList>&nbsp;</td>
                            </tr>
                            <!-- Savings / Capx / Expense / Payback -->
                            <tr>
                                <td>
                                </td>
                                <td style="text-align:center;" colspan="5">
                                    <table cellspacing="0">
                                        <tr>
                                            <td id="labelCurrency" class="label" runat="server"></td>
                                            <td><asp:DropDownList ID="selCurrency" onchange="submitEntryForm('changeCurrency');" runat="server">
                                                </asp:DropDownList></td>
                                            <td id="labelSavingsReq" runat="server"></td>
                                            <td id="labelSavings" runat="server"></td>
                                            <td id="labelSavingsInput" runat="server"><input type="text" id="inputSavings" size="9" readonly="readonly"  class="readonly" runat="server" /></td>
                                            <td id="labelCapx" class="label" runat="server"></td>
                                            <td><input type="text" id="inputCapx" onkeypress="alphaBlock();" onchange="calcValues();"
                                                size="9" runat="server" /></td>
                                            <td id="labelExpense" class="label" runat="server"></td>
                                            <td><input type="text" id="inputExpense" onkeypress="alphaBlock();" onchange="calcValues();"
                                                size="9" runat="server" /></td>
                                            <td id="labelPayback" class="label" runat="server"></td>
                                            <td><input type="text" id="inputPayback" size="9" readonly="readonly" class="readonly"
                                                runat="server" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <!-- key words -->
                            <tr>
                                <td id="labelKeyWordsReq" runat="server"></td>
                                <td id="labelKeyWords" colspan="5" runat="server">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td class="inputTA" colspan="5">
                                    <textarea id="textareaKeyWords" rows="2" cols="110" class="inputWide" runat="server"></textarea></td>
                            </tr>
                            <!-- Detail Desc -->
                            <tr>
                                <td id="labelDescReq" runat="server"></td>
                                <td id="labelDesc" colspan="5" runat="server">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td class="inputTA" colspan="5">
                                    <textarea id="textareaDesc" class="inputWide" rows="4" cols="110" runat="server"></textarea></td>
                            </tr>
                            <!-- Current State / Future State -->
                            <tr>
                                <td>
                                </td>
                                <td colspan="5">
                                    <table style="width:100%">
                                        <tr>
                                            <td class="label" style="width:50%" id="labelCurrState" colspan="2" runat="server">
                                            </td>
                                            <td class="label" style="width:50%" id="labelFutureState" colspan="2" runat="server">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="inputTA" style="width:50%" colspan="2">
                                                <textarea id="textareaCurrState" class="inputWide" rows="5" cols="50" runat="server"></textarea></td>
                                            <td class="inputTA" style="width:50%" colspan="2">
                                                <textarea id="textareaFutureState" class="inputWide" rows="5" cols="50" runat="server"></textarea></td>
                                        </tr>
                                    </table>
                                </td>
                                
                            </tr>
                            <!-- Savings Calc -->
                            <tr>
                                <td id="labelSavingsCalcReq" runat="server"></td>
                                <td id="labelSavingsCalc"  colspan="5" runat="server">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td class="inputTA" colspan="5">
                                    <textarea id="textareaSavingsCalc" class="inputWide" rows="4" cols="110" runat="server"></textarea></td>
                            </tr>
                            <!-- Start Date / Current Year Savings -->
                            <tr>
                                <td>
                                </td>
                                <td class="label" id="labelStartDate" runat="server">
                                </td>
                                <td class="input">
                                    <asp:TextBox ID="inputStartDate" Width="75" onchange="checkDate(this);setMonths();calcValues()"
                                        runat="server"></asp:TextBox>
                                    <cc1:CalendarExtender ID="CalendarExtenderStartDate" TargetControlID="inputStartDate"
                                        runat="server">
                                    </cc1:CalendarExtender>
                                </td>
                                <td></td>
                                <td class="label" id="labelCurrYearSaving" runat="server">
                                </td>
                                <td class="inputR">
                                    <input type="text" id="inputCurrYearSaving" style="width:169px;" runat="server" readonly="readonly" class="readonly" /></td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td><br />
                                </td>
                            </tr>
                            <!-- Monthly Savings -->
                            <tr>
                                <td>
                                </td>
                                <td class="label" id="labelMonthlySaving" colspan="5" runat="server">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td style="text-align:left;" colspan="5">
                                    <input type="hidden" id="hiddenMSRowCT" runat="server" value="0" />
                                    <table id="tableMSRow" cellspacing="0">
                                        <thead>
                                        <tr>
                                            <td></td>
                                            <td class="label" id="labelSavingsCategory" runat="server"></td>
                                            <td class="label" id="labelMS1">
                                            </td>
                                            <td class="label" id="labelMS2">
                                            </td>
                                            <td class="label" id="labelMS3">
                                            </td>
                                            <td class="label" id="labelMS4">
                                            </td>
                                            <td class="label" id="labelMS5">
                                            </td>
                                            <td class="label" id="labelMS6">
                                            </td>
                                            <td class="label" id="labelMS7">
                                            </td>
                                            <td class="label" id="labelMS8">
                                            </td>
                                            <td class="label" id="labelMS9">
                                            </td>
                                            <td class="label" id="labelMS10">
                                            </td>
                                            <td class="label" id="labelMS11">
                                            </td>
                                            <td class="label" id="labelMS12">
                                            </td>
                                        </tr>
                                        </thead>
                                        <tbody id="tableBodyMSRow" runat="Server"></tbody>
                                        <tfoot>
                                            <tr>
                                                <td colspan="4">
                                                <input type="button" id="buttonInsMSRow" onclick="submitEntryForm('addMSRow');" runat="server" />
                                                </td>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td><br />
                                </td>
                            </tr>
                            <!-- Implementation Plan -->
                            <tr>
                                <td>
                                </td>
                                <td class="label" id="labelImplementationPlan" colspan="5" runat="server">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td colspan="5" style="text-align:left;">
                                    <%-- <asp:UpdatePanel ID="mPagePanel" UpdateMode="Conditional" runat="server">
                                        <ContentTemplate>--%>
                                    <input type="hidden" id="hiddenImpPlanRowCT" runat="server" value="0" />
                                    <table id="tableImpPlanRow">
                                        <thead>
                                            <tr>
                                                <td>
                                                </td>
                                                <td class="label" id="labelAction" runat="server">
                                                </td>
                                                <td class="label" id="labelResponse" runat="server">
                                                </td>
                                                <td class="label" id="labelTargetDate" runat="server">
                                                </td>
                                                <td class="label" id="labelActionStatus" runat="server">
                                                </td>
                                            </tr>
                                        </thead>
                                        <tbody id="tableBodyImpPlanRow" runat="Server"></tbody>
                                        <tfoot>
                                            <tr>
                                            <td colspan="4">
                                            <input type="button" id="buttonInsRow" onclick="submitEntryForm('addImpPlanRow');" runat="server" />
                                            </td>
                                         </tr>
                                        </tfoot>
                                    </table>
                                </td>
                            </tr>
                            <!-- files -->
                            <tr>
                                <td>
                                </td>
                                <td><br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td class="label" id="labelFiles" colspan="5" runat="server">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td colspan="5" style="text-align:left;">
                                    <input type="hidden" id="hiddenFilesCT" runat="server" value="0" />
                                    <table>
                                        <tbody id="tableBodyFiles" runat="Server"></tbody>
                                        <tfoot>
                                            <tr>
                                                <td id="labelAddFiles" class="labelR" runat="server"></td>
                                                <td colspan="4" class="label"><asp:FileUpload id="fileInput" runat="server" /></td>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td class="label" id="labelTemplate" runat="server">Template</td>
                                <td class="input" id="inputTemplate" runat="server"><input type="checkbox" id="templateCK" value="Y" runat="server" /></td>
                            </tr>
                            <tr>
                                <td colspan="6" style="text-align: center;">
                                    <input type="button" id="buttonSaveProject" onclick="submitEntryForm('updateEntryForm');" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                                    <input type="button" id="buttonCopyProject" onclick="submitEntryForm('copyEntryForm');" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                                    <input type="button" id="buttonDeleteProject" onclick="submitEntryForm('deleteEntryForm');" runat="server" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
