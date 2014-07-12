<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="PIP.main" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <pt:standard.displaymode pt:mode="Hosted" pt:title="Algeco Scotsman Intranet" pt:subtitle="Productivity Improvement Project"
        xmlns:pt='http://www.plumtree.com/xmlschemas/ptui/' />
    <title>Main</title>
    <link href="style.css" type="text/css" rel="stylesheet" />
   
    <script type="text/javascript">
        function submitEntryForm(theValue){
            document.getElementById('hiddenEntryAction').value = theValue;
            document.mMainForm.submit();
        }
        function loadProject(){
            window.location.href = "<%= ConfigurationManager.AppSettings["indexPage"] %>?pid="+document.getElementById('selSearchProjectTitle').value;
        }
    </script>
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
<body>
    <form id="mMainForm" runat="server">
        <input type="hidden" id="hiddenEntryAction" value="" runat="server" />
       
        <div class="mainPageDiv">
            <div class="titleDiv"><img id="imgHeader" alt="header" runat="server" /></div>
            <div class="langBarDiv">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <span id="labelLang" runat="server"></span>&nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="selLang" onchange="submitEntryForm('changeLang');" runat="server"></asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;
                <span id="labelSearchProjectTitle" runat="server"></span>&nbsp;&nbsp;&nbsp;
                <span id="selSearchProjectTitleTD" runat="server"></span>&nbsp;&nbsp;&nbsp;&nbsp;
                <input type="button" id="buttonSearch" onclick="loadProject('search');" runat="server" />
            </div>
            <div class="pipBody">
                <div class="appMsgDiv" id="appMsg" runat="server"></div>
                <!-- Links -->
                <div class="linksDiv">
                  <div class="mainLinksDiv">
                        <div id="labelLinksTitle" class="mainLinksLabel" runat="server"></div>
                        <div class="input">&nbsp;&nbsp;<img alt="bullet.gif" src="images/bullet.gif" />&nbsp;&nbsp;<a id="linkEnterUpdate" onmouseover="window.status='';return true;" runat="server"></a></div>
                       <%-- <div class="input">&nbsp;&nbsp;<img alt="bullet.gif" src="images/bullet.gif" />&nbsp;&nbsp;<a id="linkAdminPage" onmouseover="window.status='';return true;" runat="server"></a></div>--%>
                    </div>
                    <div class="space"></div>
                    <div class="reportLinksDiv">
                        <div id="labelReportTitle" class="reportLinksLabel" runat="server"></div>
                        <div class="input">&nbsp;&nbsp;<img alt="bullet.gif" src="images/bullet.gif" />&nbsp;&nbsp;<a id="linkPlans" target="_blank" onmouseover="window.status='';return true;" runat="server"></a></div>
                        <div class="input">&nbsp;&nbsp;<img alt="bullet.gif" src="images/bullet.gif" />&nbsp;&nbsp;<a id="linkGraph" target="_blank" onmouseover="window.status='';return true;" runat="server"></a></div>
                        <div class="input">&nbsp;&nbsp;<img alt="bullet.gif" src="images/bullet.gif" />&nbsp;&nbsp;<a id="linkSearch" target="_blank" onmouseover="window.status='';return true;" runat="server"></a></div>
                    </div>
                    <div class="space"></div>
                    <div class="adminLinksDiv" style="display:none;">
                        <div id="labelAdminTitle" class="adminLinksLabel" runat="server"></div>
                    </div>
                    <div class="space"></div>
                    <div class="docLinksDiv">
                        <div id="labelDocLinks" class="docLinksLabel" runat="server"></div>
                        <div class="input">&nbsp;&nbsp;<img alt="bullet.gif" src="images/bullet.gif" />&nbsp;&nbsp;<a id="linkAccessingDoc" target="_blank" onmouseover="window.status='';return true;" runat="server"></a></div>
                        <div class="input">&nbsp;&nbsp;<img alt="bullet.gif" src="images/bullet.gif" />&nbsp;&nbsp;<a id="linkEnterProjectDoc" target="_blank" onmouseover="window.status='';return true;" runat="server"></a></div>
                        <div class="input">&nbsp;&nbsp;<img alt="bullet.gif" src="images/bullet.gif" />&nbsp;&nbsp;<a id="linkReportDoc" target="_blank" onmouseover="window.status='';return true;" runat="server"></a></div>
                    </div>
                    <div class="space"></div>
                </div>
                <!-- Search -->
                <div class="searchDiv">
                    <div  class="searchTitleLabel"><span id="labelSearchTitle" runat="server"></span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="selProjStatus" onchange="submitEntryForm('changeProj');" runat="server"></asp:DropDownList></div>
                    <div id="divSearchResults" class="searchResultsContainerDiv" runat="server"></div>
                </div>
            </div>
         </div> 
            
            
    </form>
</body>
</html>