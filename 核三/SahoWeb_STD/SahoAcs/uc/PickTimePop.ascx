<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PickTimePop.ascx.cs" Inherits="SahoAcs.uc.PickTimePop" %>
<%--readonly='readonly'--%>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Scripts>
        <asp:ScriptReference Path="/uc/PickTimePop.js" />
    </Scripts>
</asp:ScriptManagerProxy>
<table id="ucPickTime" runat="server" cellpadding="0" cellspacing="0">
    <tr style="height: 20px">
        <td>
            <asp:TextBox ID="PickTimeTextBox" runat="server" Width="40" BorderStyle="None"></asp:TextBox>
        </td>
        <td>
            <asp:Image ID="Clock" runat="server" ImageUrl="/Img/Clock.jpg" Width="28px" height="26px" />
        </td>
    </tr>
</table>
