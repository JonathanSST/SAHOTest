<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardBuzzerModePop.aspx.cs" Inherits="SahoAcs.CardBuzzerModePop" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>CardBuzzerModeSetting</title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
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
            <%--<cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>--%>
            <table border="0">
                <tr>
                    <td>
                        <fieldset id="CardBuzzer_List" runat="server" style="width: 400px; height: 260px">
                            <legend id="CardBuzzer_Legend" runat="server">卡片格式和蜂鳴器模式</legend>
                                    <table border="0" class="Table04">
                                        <tr>
                                            <td colspan="2" style="border-bottom: 1px solid #FFF;">
                                                <asp:Label ID="popL_CardFormat" runat="server" Text="卡片格式" CssClass="Title02"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr onclick="RadioSelected('popInput_CardFormat0')">
                                            <td>
                                                <asp:RadioButton ID="popInput_CardFormat0" runat="server" GroupName="CardFormat" value="00" />
                                            </td>
                                            <td style="border-bottom: 1px solid #6BB1D8;">
                                                <asp:Label ID="popL_CardFormatText0" runat="server" Text="WIEGAND 26用SAHOEM格式<br>WIEGAND 32,34使用原READER傳來的HEX碼" Font-Size="Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr onclick="RadioSelected('popInput_CardFormat1')">
                                            <td>
                                                <asp:RadioButton ID="popInput_CardFormat1" runat="server" GroupName="CardFormat" value="01" />
                                            </td>
                                            <td style="border-bottom: 1px solid #6BB1D8;">
                                                <asp:Label ID="popL_CardFormatText1" runat="server" Text="WIEGAND 26用SAHOCS格式<br>WIEGAND 32,34使用原READER傳來的HEX碼" Font-Size="Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr onclick="RadioSelected('popInput_CardFormat2')">
                                            <td>
                                                <asp:RadioButton ID="popInput_CardFormat2" runat="server" GroupName="CardFormat" value="02" />
                                            </td>
                                            <td style="border-bottom: 1px solid #6BB1D8;">
                                                <asp:Label ID="popL_CardFormatText2" runat="server" Text="WIEGAND 26用SAHOEM格式<br>WIEGAND 32,34前2個BYTE轉10進位, 後2個BYTE轉10進位在合併" Font-Size="Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr onclick="RadioSelected('popInput_CardFormat3')">
                                            <td>
                                                <asp:RadioButton ID="popInput_CardFormat3" runat="server" GroupName="CardFormat" value="03" />
                                            </td>
                                            <td>
                                                <asp:Label ID="popL_CardFormatText3" runat="server" Text="WIEGAND 26,32,34用SAHOCS格式都轉10進位" Font-Size="Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="border-bottom: 1px solid #FFF;">
                                                <asp:Label ID="popL_Buzzer" runat="server" Text="蜂鳴器模式" CssClass="Title02"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table style="border-collapse: collapse">
                                                    <tr>
                                                        <td onclick="RadioSelected('popInput_Buzzer0')">
                                                            <asp:RadioButton ID="popInput_Buzzer0" runat="server" GroupName="Buzzer" value="00" />
                                                        </td>
                                                        <td onclick="RadioSelected('popInput_Buzzer0')">
                                                            <asp:Label ID="popL_Buzzer0" runat="server" Text="使用頻率式" Font-Size="Small"></asp:Label>
                                                        </td>
                                                        <td onclick="RadioSelected('popInput_Buzzer1')">
                                                            <asp:RadioButton ID="popInput_Buzzer1" runat="server" GroupName="Buzzer" value="AA" />
                                                        </td>
                                                        <td onclick="RadioSelected('popInput_Buzzer1')">
                                                            <asp:Label ID="popL_Buzzer1" runat="server" Text="使用電壓式" Font-Size="Small"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 10px 0 5px 0; text-align: center">
                        <input type="button" value="<%=Resources.Resource.btnSave %>" class="IconSave" onclick="DoSave()" />
                        <input type="button" value="<%=Resources.Resource.btnCancel %>" class="IconCancel" onclick="DoClose()" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

