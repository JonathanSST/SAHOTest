<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardTransferMode.aspx.cs" Inherits="SahoAcs.CardTransferMode" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>CardTransferMode</title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="../../../Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body onload="LoadData()">
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <asp:UpdatePanel ID="UpdatePanel_ValueKeep" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="hideUserID" runat="server" />
                    <asp:HiddenField ID="hideEquID" runat="server" />
                    <asp:HiddenField ID="hideEquClass" runat="server" />
                    <asp:HiddenField ID="hideEquModel" runat="server" />
                    <asp:HiddenField ID="hideEquParaID" runat="server" />
                    <asp:HiddenField ID="hideParaValue" runat="server" />
                    <asp:HiddenField ID="hideCardLen" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div>
            <table border="0">
                <tr>
                    <td>
                        <fieldset id="CardRule_List" runat="server" style="width: 205px; height: 70px">
                            <legend id="CardRule_Legend" runat="server">刷卡傳輸模式</legend>
                            <table class="Item">
                                <tr>
                                    <th>
                                        <asp:Label ID="L_CardTransferMode" runat="server" Text="刷卡傳輸模式"></asp:Label>
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="Input_TransferMode" runat="server" BackColor="#FFE5E5" Font-Size="13px" Width="190px">
                                                    <asp:ListItem Value="00">正常傳輸模式</asp:ListItem>
                                                    <asp:ListItem Value="01">刷卡直接傳送模式</asp:ListItem>
                                                </asp:DropDownList>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 10px; text-align: center">
                        <asp:Button ID="popB_Save" runat="server" CssClass="IconSave" OnClick="popB_Save_Click" Text="<%$ Resources:Resource, btnSave%>" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

