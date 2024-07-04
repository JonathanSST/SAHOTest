<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpecialCodeSetting.aspx.cs" Inherits="SahoAcs.SpecialCodeSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>SpecialCodeSetting</title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="../../../Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body onload="LoadData()">
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideEquParaID" runat="server" />
            <asp:HiddenField ID="hideParaValue" runat="server" />
        </div>
        <div>
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <table border="0">
                <tr>
                    <td>
                        <fieldset id="SpecialCode_List" runat="server" style="width: 325px">
                            <legend id="SpecialCode_Legend" runat="server">特殊密碼設定</legend>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Table ID="SpecialCodeTable" runat="server"></asp:Table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="popL_FunctionRemind" runat="server" Font-Size="13px" ForeColor="#BFFFEF"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 10px 0 5px 0; text-align: center">
                        <asp:Button ID="popB_Save" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" OnClick="popB_Save_Click" CssClass="IconSave" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

