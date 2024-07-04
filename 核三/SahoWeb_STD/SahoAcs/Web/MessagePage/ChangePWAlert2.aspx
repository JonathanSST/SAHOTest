<%@ Page Language="C#" AutoEventWireup="True" Inherits="SahoAcs.Web.MessagePage.ChangePWAlert2" CodeBehind="ChangePWAlert2.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
        </div>
        <div>
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <div id="AlertDiv">
                <table style="text-align:center" style="font-weight: normal; font-size: xx-large; width: 500px; color: blue; font-family: 標楷體; text-align: center">
                    <tr>
                        <td>很&nbsp; 抱&nbsp; 歉 !!</td>
                    </tr>
                    <tr>
                        <td style="height: 135px">您為初次登入系統<br />
                            或者逾三個月以上未變更密碼<br />
                            將進行密碼變更動作！
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btGoChangePW" runat="server" Font-Bold="True" Font-Names="標楷體" Font-Size="Large"
                                Text="前往變更密碼" />
                        </td>
                    </tr>
                </table>
            </div>
            <dir id="UIDiv" style="display: none">
                <table style="text-align:center" cellspacing="0" style="border: 1px solid black; font-weight: normal; font-size: x-large; color: black; font-family: 標楷體; text-align: center">
                    <tr>
                        <td style="border-bottom: 1px solid black;">密碼變更</td>
                    </tr>
                    <tr>
                        <td>
                            <table cellspacing="5" style="font-family: 微軟正黑體,Arial; font-size: 14px; color: black; text-align: left">
                                <tr>
                                    <td style="text-align:right">
                                        <asp:Label ID="Label_OldPWD" runat="server" Text="舊密碼："></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="Input_OldPWD" runat="server" TextMode="Password"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:right">
                                        <asp:Label ID="Label_NewPWD" runat="server" Text="新密碼："></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="Input_NewPWD" runat="server" TextMode="Password"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:right">
                                        <asp:Label ID="Label_CheckPWD" runat="server" Text="確認密碼："></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="Input_CheckPWD" runat="server" TextMode="Password"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btChangePW" runat="server" Text="變更密碼" Style="font-family: 微軟正黑體,Arial;" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:Label ID="Label_Remind" runat="server" Text="密碼原則：英文大寫、英文小寫、特殊符號、數字，四條件取三種，長度至少12碼"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </dir>
        </div>
    </form>
</body>
</html>
