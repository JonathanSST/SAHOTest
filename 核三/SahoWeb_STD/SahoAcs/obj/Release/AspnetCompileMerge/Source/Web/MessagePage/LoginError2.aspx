<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginError2.aspx.cs" Inherits="SahoAcs.Web.MessagePage.LoginError2" %>

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
        <table style="text-align: center; font-weight: normal; font-size: xx-large; width: 100%; color: blue; font-family: 微軟正黑體;">
            <tr>
                <td>
                    <%=Resources.ResourceMsg.MsgSorry %>!!</td>
            </tr>
            <tr>
                <td style="height: 135px">
                    <%=Resources.ResourceMsg.MsgExpired %></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btGoLogin" runat="server" Font-Bold="True" Font-Names="微軟正黑體" 
                        Font-Size="Large" PostBackUrl="~/Default.aspx" Text="<%$Resources:ResourceMsg,MsgToLogin %>" />
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
