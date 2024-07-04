<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LightTimeSetting.aspx.cs" Inherits="SahoAcs.LightTimeSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/uc/PickTime.ascx" TagPrefix="uc1" TagName="PickTime" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>LightTimeSetting</title>
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
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime1" />
                            </td>
                            <td style="width: 10px"></td>
                            <td>06.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime6" />
                            </td>
                        </tr>
                        <tr>
                            <td>02.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime2" />
                            </td>
                            <td style="width: 10px"></td>
                            <td>07.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime7" />
                            </td>
                        </tr>
                        <tr>
                            <td>03.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime3" />
                            </td>
                            <td style="width: 10px"></td>
                            <td>08.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime8" />
                            </td>
                        </tr>
                        <tr>
                            <td>04.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime4" />
                            </td>
                            <td style="width: 10px"></td>
                            <td>09.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime9" />
                            </td>
                        </tr>
                        <tr>
                            <td>05.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime5" />
                            </td>
                            <td style="width: 10px"></td>
                            <td>10.</td>
                            <td>
                                <uc1:PickTime runat="server" ID="TimeSet_PickTime10" />
                            </td>
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

