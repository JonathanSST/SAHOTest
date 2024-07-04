<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginError3.aspx.cs" Inherits="SahoAcs.Web.MessagePage.LoginError3" %>

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
        <table style="text-align: center; font-weight: normal; font-size: xx-large; width: 100%; color: blue; font-family: 標楷體;">
            <tr>
                <td>
                    很&nbsp; 抱&nbsp; 歉 !!</td>
            </tr>
            <tr>
                <td style="height: 135px">
                    您所輸入的帳號不在使用期限內<br />
                    將回到登入頁面</td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btGoLogin" runat="server" Font-Bold="True" Font-Names="標楷體" 
                        Font-Size="Large" PostBackUrl="~/Default.aspx" Text="直接前往登入頁面" />
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
