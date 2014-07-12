<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="index.aspx.cs" Inherits="WS20.SafetyAudit.Index" %>
<%@ Register TagPrefix="WS" TagName="WSHeader" Src="~/WSHeader.ascx" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
--%>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Update Safety Audit Information</title>
    <link href="common.css" rel="stylesheet" type="text/css" />
</head> 
<body>
    <form id="form1" runat="server">
        <input type="hidden" value="" runat="server" id='mHeaderClientID' /> 
        <WS:WSHeader ID="mWSHeader" runat="server"/>
        <asp:ScriptManager ID="ScriptManager" LoadScriptsBeforeUI="false" OnAsyncPostBackError="HandleError" runat="server" >
            <Scripts>
                <asp:ScriptReference path="common.js" />
            </Scripts>
            </asp:ScriptManager>
       <%--<script language="javascript" src='common.js'></script>--%>
        
        <%--Update Percentage--%>
        <div id="update_page" runat="server"> 
            
            <asp:UpdatePanel ID="update_pcnt" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <table >
                        <tr>
                            <td><b>Lowest Possible Passing Percentage:</b></td>
                            <td><input type="text" id="pct_txt" runat="server" style="width: 67px" /></td>
                        </tr>
                        <tr>
                            <td><b>Date Last Changed:</b></td>
                            <td><input type="text" id="lst_upd_dt" runat="server" disabled="disabled" /></td>
                        </tr>
                        <tr>
                            <td><b>Changed By:</b></td>
                            <td><input type="text" id="upd_by" runat="server" disabled="disabled" /></td>
                        </tr>
                        <tr>
                            <td><asp:Button ID="upd_but" runat="server" Text="Update Rate" OnClick="upd_but_Click" /></td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            
            
            
            
        </div>
                
        <%--Add Questions/Headings--%>
        <div id="insert_page" runat="server" style="display:none;">
            <asp:UpdatePanel ID="mQuestionHeading" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
            
                    <table border="1" width="550px">
                        <tr>
                            <th colspan="2"  style="font-size: 24px"><b>Add A New Question</b></th>
                        </tr>
                        <tr>
                            <td><b>Section:</b></td>
                            <td><asp:ListBox ID="add_list" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><b>Question Text:</b></td>
                            <td><textarea rows="4" cols = "50" runat="server" id="add_ques_txt"></textarea></td>
                        </tr>
                    </table>
                    <table border="1" width="550px">
                        <tr>
                            <th colspan="2"  style="font-size: 24px"><b>Add A New Section</b></th>
                        </tr>
                        <tr>
                            <td><b>Section Text:</b></td>
                            <td><textarea rows="4" cols = "50" runat="server" id="add_sec_txt"></textarea></td>
                        </tr>
                    </table>
             </ContentTemplate>
             <Triggers>
                <asp:AsyncPostBackTrigger ControlID="add_ques_sec_button" EventName="Click" />
             </Triggers>
           </asp:UpdatePanel>
           
           <asp:Button ID="add_ques_sec_button" runat="server" Text="Add"  OnClick="addQuesSec_click" />
                          
         </div>
        
        <%--Update Questions--%>
        <div id="question_page" style="display:none;">
            <asp:UpdatePanel ID="updQuestpanel" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div id="question_div"  runat="server"> </div>
                    <input type="hidden" runat="server" id="hid_quest_cnt" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="question_button" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="add_ques_sec_button" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            
            <table>
                <tr>
                    <td><b>Current Percentage</b></td>
                    <td><input type="text" id="chk_pct" runat="server" disabled="disabled" /></td>
                </tr>
                <tr>
                    
                    <td><input type="button" id="check_pct_button" value="Check Percentage" onclick="chk_current_pct()" /></td>
                    <td><asp:Button ID="question_button" runat="server" Text="Update Questions" OnClick="upd_question_click" /></td>
                </tr>
            </table>            
        </div>
        
        <%--Upload Excel--%>
        <div id="excel_page" style="display:none;">
            <table cellpadding="5px">
                <tr> 
                    <td><font color="red">*</font> Please select a branch:</td>
                    <td><asp:DropDownList ID="brn_lst" runat="server" /></td>
                </tr>
                <tr>
                    <td><font color="red">*</font> Please enter the inspection date (MM/DD/YYYY):</td>
                    <td><input type="text" id="insp_dt" runat="server" onblur="IsDate()" /></td>
                </tr>
                <tr>
                    <td>
                        <font color="red">*</font> Select a service manager:</td>
                    <td><asp:DropDownList ID="svc_mgr_lst" runat="server" /></td>
                </tr>
                <tr>
                    <td><font color="red">*</font> Please select a regional service manager:</td>
                    <td><asp:DropDownList ID="reg_svc_mgr_lst" runat="server" /></td>
                </tr>
                <tr>
                    <td><font color="red">*</font> Select a file to upload:</td>
                    <td><asp:FileUpload ID="fu_excel" runat="server"/></td>
                </tr>     
                <tr>
                    <td><div id="excel_but"><asp:Button ID="excel_button" runat="server" Text="Submit" OnClick="uploadExcel" /></div></td>
                </tr>             
            </table>
             
            
            <%--<asp:TextBox ID="SearchReceived" CssClass="myField" maxlength="30" runat="server"></asp:TextBox>									

                        <cc1:calendarextender id="CalendarExtender1" runat="server" targetcontrolid="SearchReceived__PORTLET_ID__" OnClientDateSelectionChanged="function hideCalendar(cb) { cb.hide(); }"></cc1:calendarextender>
                        
                        <asp:RegularExpressionValidator runat="server" ID="SearchReceivedValidator__PORTLET_ID__"
                            ControlToValidate="SearchReceived__PORTLET_ID__"
                            Display="None"
                            ValidationExpression="^\d{1,2}\/\d{1,2}\/\d{4}$"
                            
                            ErrorMessage="<b>Invalid Field</b><br />Please enter a date in the format:<br />mm/dd/yyyy" />
                        <cc1:ValidatorCalloutExtender runat="Server" ID="SearchReceivedCallout__PORTLET_ID__"
                            TargetControlID="SearchReceivedValidator__PORTLET_ID__"
                            HighlightCssClass="" />--%>

            
            
        </div>
        
        <div id="cc_page" style="display:none;">
        
            <asp:UpdatePanel ID="costCenterupdate" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div id="branch_main" runat="server"></div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="branch_main_button" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel> 
            
            <asp:Button ID="branch_main_button" runat="server" Text="Save Changes" OnClick="saveBranchmain" />
        
        </div>
        
        <asp:DataGrid ID="tmp" runat="server"></asp:DataGrid>
        <div id="Div0" class="menu menuUpdate"  onclick="changeMenuPage('update_')">Update<br />Percentage</div>
        <div id="Div2" class="menu menuBranch"  onclick="changeMenuPage('question_')">Update<br />Questions</div>
        <div id="Div3" class="menu menuConst"   onclick="changeMenuPage('excel_')">Regional<br />Audit</div>
        <div id="Div4" class="menu menuAdd"     onclick="changeMenuPage('insert_')">Add<br />Question/Heading</div>
        <div id="Div5" class="menu menuCC"      onclick="changeMenuPage('cc_')">Branch<br />Maintenance</div>
        
        <asp:UpdatePanel ID="mContentPanel" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <div id="div1" runat="server"></div>
                <div id="preview" runat="server"></div>  
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="upd_but" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="add_ques_sec_button" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="question_button" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="branch_main_button" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        
        <%--<WS:WSFooter ID="mWSFooter" runat="server" />--%>
    
    </form>
</body>
</html>
