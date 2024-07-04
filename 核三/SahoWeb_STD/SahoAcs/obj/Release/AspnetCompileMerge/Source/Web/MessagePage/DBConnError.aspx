<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DBConnError.aspx.cs" Inherits="SahoAcs.Web.MessagePage.DBConnError" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
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
        <table style="text-align:center" style="font-weight: normal; font-size: xx-large; width: 500px;
            color: blue; font-family: 標楷體; text-align: center">
            <tr>
                <td>
                    很&nbsp; 抱&nbsp; 歉 !!
                </td>
            </tr>
            <tr>
                <td style="height: 135px">
                    由於資料庫無法連結<br />
                    系統必須暫停服務
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btGoLogin" runat="server" Font-Bold="True" Font-Names="標楷體" Font-Size="Large"
                        PostBackUrl="~/Default.aspx" Text="直接前往登入頁面" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
