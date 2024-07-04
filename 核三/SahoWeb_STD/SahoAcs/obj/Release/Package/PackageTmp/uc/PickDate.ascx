<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PickDate.ascx.cs" Inherits="SahoAcs.uc.PickDate" %>
<%--readonly='readonly'--%>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Scripts>
        <asp:ScriptReference Path="PickDate.js" />
    </Scripts>
</asp:ScriptManagerProxy>
<table id="ucPickDate" runat="server" cellpadding="0" cellspacing="0">
    <tr style="height: 20px">
        <td>
            <asp:TextBox ID="PickDateTextBox" runat="server" Width="80" BorderStyle="None"></asp:TextBox>
        </td>
        <td>
            <asp:Image ID="Clock" runat="server" ImageUrl="~/Img/Clock.jpg" Width="28px" height="26px" />
        </td>
    </tr>
</table>
