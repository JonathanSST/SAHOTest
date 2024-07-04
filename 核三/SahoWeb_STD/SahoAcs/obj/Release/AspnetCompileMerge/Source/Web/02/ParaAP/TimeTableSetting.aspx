<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeTableSetting.aspx.cs" Inherits="SahoAcs.TimeTableSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>TimeTableSetting</title>
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
            <asp:HiddenField ID="hideMode" runat="server" />
        </div>
        <div>
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <table border="0">
                <tr>
                    <td>
                        <fieldset id="TimeBlock_List" runat="server" style="width: 285px; height: 70px">
                            <legend id="TimeBlock_Legend" runat="server">模式時區設定</legend>
                            <table class="Item">
                                <tr>
                                    <td>
                                        <asp:Label ID="L_Mode" runat="server" Text="模式時區"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="Input_Mode" runat="server" BackColor="#FFE5E5" Font-Size="13px" Width="280px">
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
                        <asp:Button ID="popB_Save" runat="server" CssClass="IconSave" EnableViewState="False" OnClick="popB_Save_Click" Text="<%$ Resources:Resource, btnSave%>" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

