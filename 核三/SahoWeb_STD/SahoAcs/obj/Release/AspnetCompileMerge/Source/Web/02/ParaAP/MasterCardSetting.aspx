<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MasterCardSetting.aspx.cs" Inherits="SahoAcs.MasterCardSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>MasterCardSetting</title>
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
                    <asp:HiddenField ID="hideEquParaID" runat="server" />
                    <asp:HiddenField ID="hideParaValue" runat="server" />
                    <asp:HiddenField ID="hideCardLen" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div>

            <table>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <fieldset id="MasterCard_List" runat="server" style="width: 265px">
                                    <legend id="MasterCard_Legend" runat="server">母卡資料</legend>
                                    <table style="border-spacing: 5px">
                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:Label ID="L_CardNo" runat="server" Text="卡號"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="L_PassWord" runat="server" Text="密碼"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>1.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo1" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord1" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo2" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord2" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>3.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo3" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord3" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>4.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo4" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord4" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>5.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo5" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord5" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>6.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo6" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord6" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>7.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo7" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord7" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>8.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo8" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord8" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>9.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo9" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord9" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>10.</td>
                                            <td>
                                                <asp:TextBox ID="Input_CardNo10" runat="server" Width="155px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="Input_PassWord10" runat="server" Width="60px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="popL_FunctionRemind" runat="server" Font-Size="13px" ForeColor="#339933"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <asp:Button ID="popB_Save" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" OnClick="popB_Save_Click" CssClass="IconSave" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

