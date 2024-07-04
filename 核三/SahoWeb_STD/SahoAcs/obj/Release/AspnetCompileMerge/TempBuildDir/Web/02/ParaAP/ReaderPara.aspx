<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReaderPara.aspx.cs" Inherits="SahoAcs.ReaderPara" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>ReaderParaSetting</title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="../../../Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body onload="LoadData()">
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideEquParaID" runat="server" />
            <asp:HiddenField ID="hideParaValue" runat="server" />
            <asp:HiddenField ID="hideTarget" runat="server" />
        </div>
        <div>
            <table>
                <tr>
                    <td>
                        <fieldset id="ReaderPara_List" runat="server" style="width: 440px">
                            <legend id="ReaderPara_Legend" runat="server">讀卡機參數</legend>
                            <table border="0" class="Table04">
                                <tr>
                                    <td style="width: 40px">&nbsp </td>
                                    <td style="cursor: pointer" onclick="ChangeLabelColor('popL_LTitle');">
                                        <asp:Label ID="popL_LTitle" runat="server" Text="SR系列讀卡機" Font-Size="11pt" Font-Bold="true"></asp:Label>
                                    </td>
                                    <td style="cursor: pointer" onclick="ChangeLabelColor('popL_RTitle');">
                                        <asp:Label ID="popL_RTitle" runat="server" Text="SR-720MF or SR-3000MF" Font-Size="11pt" Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <%--輸出介面設定--%>
                                <tr>
                                    <td>&nbsp</td>
                                    <td style="border-bottom: 1px solid #FFF;">
                                        <asp:Label ID="popL_LFace" runat="server" Text="輸出介面設定" Font-Bold="true"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #FFF;">
                                        <asp:Label ID="popL_RFace" runat="server" Text="輸出介面設定" Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <tr onclick="RadioSelected('popInput_Face0')">
                                    <td style="text-align: center">
                                        <asp:RadioButton ID="popInput_Face0" runat="server" GroupName="Face" value="0A" />
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LFaceText0" runat="server" Text="ABA輸出介面模式"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RFaceText0" runat="server" Text="ABA輸出介面模式"></asp:Label>
                                    </td>
                                </tr>
                                <tr onclick="RadioSelected('popInput_Face1')">
                                    <td style="text-align: center">
                                        <asp:RadioButton ID="popInput_Face1" runat="server" GroupName="Face" value="0B" />
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LFaceText1" runat="server" Text="WIEGAND輸出介面模式"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RFaceText1" runat="server" Text="WIEGAND輸出介面模式"></asp:Label>
                                    </td>
                                </tr>
                                <tr onclick="RadioSelected('popInput_Face2')">
                                    <td style="text-align: center">
                                        <asp:RadioButton ID="popInput_Face2" runat="server" GroupName="Face" value="0C" />
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LFaceText2" runat="server" Text="RS-232HEX輸出介面模式"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RFaceText2" runat="server" Text="RS-232HEX輸出介面模式"></asp:Label>
                                    </td>
                                </tr>
                                <tr onclick="RadioSelected('popInput_Face3')">
                                    <td style="text-align: center">
                                        <asp:RadioButton ID="popInput_Face3" runat="server" GroupName="Face" value="1C" />
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LFaceText3" runat="server" Text="RS-232ABA輸出介面模式"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RFaceText3" runat="server" Text="RS-232ASC輸出介面模式"></asp:Label>
                                    </td>
                                </tr>
                                <tr onclick="RadioSelected('popInput_Face4')">
                                    <td style="text-align: center">
                                        <asp:RadioButton ID="popInput_Face4" runat="server" GroupName="Face" value="2C" />
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LFaceText4" runat="server" Text="無效參數"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RFaceText4" runat="server" Text="RS-232 POLLING介面模式"></asp:Label>
                                    </td>
                                </tr>
                                <%--斷行--%>
                                <tr style="height: 15px">
                                    <td colspan="3"></td>
                                </tr>
                                <%--卡片格式設定--%>
                                <tr>
                                    <td>&nbsp </td>
                                    <td style="border-bottom: 1px solid #FFF;">
                                        <asp:Label ID="popL_LLoadMode" runat="server" Text="卡片格式" Font-Bold="true"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #FFF;">
                                        <asp:Label ID="popL_RLoadMode" runat="server" Text="MIFARE KEY模式" Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <tr onclick="RadioSelected('popInput_LoadMode0')">
                                    <td style="text-align: center">
                                        <asp:RadioButton ID="popInput_LoadMode0" runat="server" GroupName="LoadMode" value="0A" />
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LLoadModeText0" runat="server" Text="SAHO EM格式"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RLoadModeText0" runat="server" Text="使用KEY A"></asp:Label>
                                    </td>
                                </tr>
                                <tr onclick="RadioSelected('popInput_LoadMode1')">
                                    <td style="text-align: center">
                                        <asp:RadioButton ID="popInput_LoadMode1" runat="server" GroupName="LoadMode" value="0B" />
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LLoadModeText1" runat="server" Text="SAHO CS格式"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RLoadModeText1" runat="server" Text="使用KEY B"></asp:Label>
                                    </td>
                                </tr>
                                <tr onclick="RadioSelected('popInput_LoadMode2')">
                                    <td style="text-align: center">
                                        <asp:RadioButton ID="popInput_LoadMode2" runat="server" GroupName="LoadMode" value="0C" />
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LLoadModeText2" runat="server" Text="SAHO T2格式"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RLoadModeText2" runat="server" Text="無效參數"></asp:Label>
                                    </td>
                                </tr>
                                <%--斷行--%>
                                <tr style="height: 15px">
                                    <td colspan="3"></td>
                                </tr>
                                <%--區塊長度設定--%>
                                <tr>
                                    <td>&nbsp </td>
                                    <td style="border-bottom: 1px solid #FFF;">
                                        <asp:Label ID="popL_LBlockLength" runat="server" Text="WIEGAND 輸出的BITs" Font-Bold="true" ForeColor="Yellow"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #FFF;">
                                        <asp:Label ID="popL_RBlockLength" runat="server" Text="讀取的BLOCK" Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center">
                                        <asp:TextBox ID="popInput_BlockLength" runat="server" Width="20px" Style="text-align: center"></asp:TextBox>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LBlockLengthText" runat="server" Text="待設定"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RBlockLengthText" runat="server" Text="待設定"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td colspan="2">
                                        <asp:Label ID="pop_BlockLengthRemind" runat="server" Text="設定值為0 ~ 64。" Font-Size="13px" Width="300px" ForeColor="#BFFFEF"></asp:Label>
                                    </td>
                                </tr>
                                <%--斷行--%>
                                <tr style="height: 15px">
                                    <td colspan="3"></td>
                                </tr>
                                <%--蜂鳴器設定--%>
                                <tr>
                                    <td>&nbsp </td>
                                    <td style="border-bottom: 1px solid #FFF;">
                                        <asp:Label ID="popL_LBuzzer" runat="server" Text="讀卡機蜂鳴器" Font-Bold="true"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #FFF;">
                                        <asp:Label ID="popL_RBuzzer" runat="server" Text="讀卡機蜂鳴器" Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center">
                                        <asp:TextBox ID="popInput_Buzzer" runat="server" Width="20px" Style="text-align: center"></asp:TextBox>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px; border-right: 1px dashed #6BB1D8">
                                        <asp:Label ID="popL_LBuzzerText" runat="server" Text="待設定"></asp:Label>
                                    </td>
                                    <td style="border-bottom: 1px solid #6BB1D8; padding: 5px;">
                                        <asp:Label ID="popL_RBuzzerText" runat="server" Text="待設定"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td colspan="2">
                                        <asp:Label ID="pop_BuzzerRemind" runat="server" Text="設定值為0 ~ 1。" Font-Size="13px" Width="300px" ForeColor="#BFFFEF"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>

                    </td>
                </tr>
                <tr>
                    <td style="padding: 10px 0 5px 0; text-align: center">
                        <asp:Button ID="popB_Save" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

