<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarFrm.ascx.cs" Inherits="SahoAcs.uc.CalendarFrm" %>
<%--readonly='readonly'--%>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Scripts>
        <asp:ScriptReference Path="CalendarFrm.js" />
    </Scripts>
</asp:ScriptManagerProxy>
<table id="ucCalendar" runat="server" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <asp:TextBox ID="CalendarTextBox" runat="server" Width="88"></asp:TextBox>
        </td>
        <td>
            <asp:Image ID="Clock" runat="server" ImageUrl="~/Img/Clock.jpg" Width="28px" height="26px" />
        </td>
    </tr>
</table>
<div id="dvContent" style="position:absolute;left:20px;top:30px;z-index:1000000000" ></div>