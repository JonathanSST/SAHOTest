<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClearResult.aspx.cs" Inherits="SahoAcs.ClearResult" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="MainDiv">             
        <table class="GVStyle" cellspacing="0" rules="all" border="1" id="GridResult" style="width: 100%; border-collapse: collapse;">
            <tbody>
                <%foreach(var log in this.logs){ %>
                <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                    <td style="width: 34px;text-align:center">
                    <input style="width:25px;height:25px" type="checkbox" id="ChkOne" name="ChkOne" value="<%=log.RecordId %>" /></td>
                    <td style="width: 94px;"><%=log.PsnName %></td>
                    <td style="width: 94px;"><%=log.CardNo %></td>
                    <td style="width: 94px;"><%=log.PsnNo %></td>
                    <td style="width: 124px;"><%=log.CardTime.ToString("yyyy/MM/dd HH:mm:ss") %></td>
                    <td style="width: 104px;"><%=log.NewName %></td>
                    <td style="width: 104px;"><%=log.EquNo %></td>
                    <td>未授權</td>
                </tr>
                <%} %>
            </tbody>
        </table>
        </div>       
    </form>
</body>
</html>
