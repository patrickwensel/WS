<%@ Control ClassName="ASP.WS20.CCT.WSHeader" Language="c#" Inherits="WS20.CCT.WSHeader" CodeBehind="WSHeader.ascx.cs" AutoEventWireup="True" %>
<link href="include/style.css" rel="stylesheet" />
<%--<link id="mLink" runat="server" type="text/css" rel="Stylesheet" />--%>
<div id="mAppHeader" runat="server" class="main_header">
    <img src="" width="770" height="83" alt="" border="0" id="mAppLogo" runat="server" />
    <div id="mAppTitle" runat="server" class="app_title"></div>
    <div id="mAppSearchBox" runat="server" class="app_subhead"></div>
    <div id="mAppMenuBar" runat="server" class="app_menu">
        <style type="text/css">
            div.main_header {
                height: 135px;
            }

            div.imsubc ul l a {
                text-decoration: none;
            }

            div#menuHead {
                width: 770px;
                height: 18px;
                font-size: 0px;
            }

            .menuRoundEnd {
                width: 10px;
                height: 18px;
                font-size: 0px;
                float: left;
            }

            div#menuCenterContainer {
                width: 750px;
                height: 18px;
                float: left;
                /*background-image: url(http://ws/version2/images/menuBackground_gray.gif);*/
            }
        </style>
        <div id="menuCenterContainer" runat="server">
            

<%--            <a href="javascript:menu('menu','')" onclick="">Main Menu</a> |
            <a href="javascript:menu('trans','')" onclick="">Transactions</a> |
            <a href="javascript:menu('search','')" onclick="">Search</a> |
            <a href="javascript:menu('credit','')" onclick="">Credit</a> |
            <a href="javascript:menu('boaSearch','')" onclick="">BOA Search</a> |--%>
        </div>
    </div>
    <div id="mAppPageName" runat="server" class="app_pagename"></div>
</div>
