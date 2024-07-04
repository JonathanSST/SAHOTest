<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="SahoAcs.uc.Calendar" %>
<%--readonly='readonly'--%>
<script src="<%=Request.ApplicationPath.TrimEnd('/') %>/Scripts/jquery.colorbox-min.js"></script>
<link href="<%=Request.ApplicationPath.TrimEnd('/') %>/Css/colorbox.css" rel="stylesheet" />
<%--<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Scripts>
        <asp:ScriptReference Path="Calendar.js" />
    </Scripts>
</asp:ScriptManagerProxy>--%>
<script type="text/javascript" src="<%=ResolveClientUrl("Calendar.js") %>"></script>
<table id="ucCalendar" runat="server" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <asp:TextBox ID="CalendarTextBox" runat="server" Width="138"></asp:TextBox>
        </td>
        <td>
            <asp:Image ID="Clock" runat="server" ImageUrl="~/Img/Clock.jpg" Width="28px" height="26px" />
        </td>
    </tr>
</table>
<%--<div id="CalendarShow" style="position:absolute;left:20px;top:30px;z-index:1000000000; background-color:blue" ></div>--%>