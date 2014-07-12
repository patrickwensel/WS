<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="PIP.admin" %>

<%@ Register Assembly="AjaxControlToolkit, Version=1.0.10618.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <pt:standard.displaymode pt:mode="Hosted" pt:title="PIP" pt:subtitle="Admin" xmlns:pt='http://www.plumtree.com/xmlschemas/ptui/' />
    <title>PIP Admin</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager" EnablePageMethods="true" LoadScriptsBeforeUI="false" OnAsyncPostBackError="HandleError" runat="server">
        </asp:ScriptManager>

        <h3>Access</h3>
        <div>
            <asp:DataGrid ID="dgFiles" runat="server"></asp:DataGrid>
            <br />
            New Admin Name:<asp:DropDownList ID="selOneTeamMembers" style="width:250px;" runat="server"></asp:DropDownList>

        </div>
        
        <asp:Button ID="buttonSaveAccess" OnClick="buttonSaveAccess_click" runat="server" />
    </form>
</body>
</html>
