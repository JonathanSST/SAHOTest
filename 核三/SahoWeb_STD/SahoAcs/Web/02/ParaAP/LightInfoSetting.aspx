<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LightInfoSetting.aspx.cs" Inherits="SahoAcs.LightInfoSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>LightInfoSetting</title>
</head>
<body onload="LoadData()">
    <form id="form1" runat="server">
        <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
        <div id="ValueKeep">
            <asp:UpdatePanel ID="UpdatePanel_ValueKeep" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="hideUserID" runat="server" />
                    <asp:HiddenField ID="hideEquID" runat="server" />
                    <asp:HiddenField ID="hideEquParaID" runat="server" />
                    <asp:HiddenField ID="hideParaValue" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td>01.</td>
                            <td>
                                <asp:TextBox ID="MsgSet1" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                            <td style="width: 10px"></td>
                            <td>06.</td>
                            <td>
                                <asp:TextBox ID="MsgSet6" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>02.</td>
                            <td>
                                <asp:TextBox ID="MsgSet2" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                            <td style="width: 10px"></td>
                            <td>07.</td>
                            <td>
                                <asp:TextBox ID="MsgSet7" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>03.</td>
                            <td>
                                <asp:TextBox ID="MsgSet3" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                            <td style="width: 10px"></td>
                            <td>08.</td>
                            <td>
                                <asp:TextBox ID="MsgSet8" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>04.</td>
                            <td>
                                <asp:TextBox ID="MsgSet4" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                            <td style="width: 10px"></td>
                            <td>09.</td>
                            <td>
                                <asp:TextBox ID="MsgSet9" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>05.</td>
                            <td>
                                <asp:TextBox ID="MsgSet5" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                            <td style="width: 10px"></td>
                            <td>10.</td>
                            <td>
                                <asp:TextBox ID="MsgSet10" runat="server" MaxLength="4" Width="80"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td colspan="5" style="text-align: center">
                                <asp:Button ID="popB_Save" runat="server"  Text="<%$ Resources:Resource, btnSave%>" UseSubmitBehavior="false" OnClick="popB_Save_Click" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>
    </form>
</body>
</html>

