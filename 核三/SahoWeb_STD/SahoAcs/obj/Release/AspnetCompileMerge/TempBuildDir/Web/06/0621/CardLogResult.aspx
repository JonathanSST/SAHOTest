<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardLogResult.aspx.cs" Inherits="SahoAcs.Web._06._0621.CardLogResult" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                        <tbody>
                                            <%foreach(var log in this.logs){ %>
                                            <%if(this.OldPsnName!=""&&this.OldPsnName!=log.PsnName){%>
                                            <tr id="GV_Row1">
                                                <td colspan="9">總數：<%=this.group_count %></td>                                                
                                            </tr>    
                                            <%
                                                  this.group_count = 0;
                                            } %>
                                            <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                                <td style="width: 103px;"><%=log.PsnEName %></td>
                                                <td style="width: 94px;"><%=log.PsnName %></td>
                                                <td style="width: 94px;"><%=log.PsnNo %></td>
                                                <td style="width: 94px;"><%=log.PsnNo %></td>
                                                <td style="width: 124px;"><%=log.CardTime.ToString("yyyy/MM/dd HH:mm:ss") %></td>
                                                <td style="width: 104px;"><%=log.EquName %></td>
                                                <td style="width: 84px;"><%=log.LogStatus=="0"?"成功":"" %></td>
                                                <td style="width: 124px;"><%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",log.PsnETime) %></td>
                                                <td><%=log.Undertaker %></td>
                                            </tr>
                                            <%
                                                  this.group_count++;
                                                  this.OldPsnName = log.PsnName;
                                            } %>
                                            <tr id="GV_Row1">
                                                <td colspan="9">總數：<%=this.group_count %></td>                                                
                                            </tr>
                                        </tbody>
                                    </table>                 
    </form>
</body>
</html>
