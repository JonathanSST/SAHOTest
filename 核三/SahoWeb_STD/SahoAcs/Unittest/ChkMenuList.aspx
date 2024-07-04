<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChkMenuList.aspx.cs" Inherits="SahoAcs.Unittest.ChkMenuList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <table>
        <tr>
            <td style="vertical-align:top">
           <span style="background-color:aqua">人員資料</span>
                <%foreach (var o in this.ListMenus.Where(i => i.UpMenuNo == "01"))
                    { %>
                <div><%=string.Format("{0}___{1}",o.MenuNo,o.MenuName) %></div>
                <%} %>
            </td>
            <td style="vertical-align:top">
           <span style="background-color:aqua">設備管理</span>
                <%foreach (var o in this.ListMenus.Where(i => i.UpMenuNo == "02"))
                    { %>
                <div><%=string.Format("{0}___{1}",o.MenuNo,o.MenuName) %></div>
                <%} %>
            </td>
            <td style="vertical-align:top">
           <span style="background-color:aqua">權限管理</span>
                <%foreach (var o in this.ListMenus.Where(i => i.UpMenuNo == "03"))
                    { %>
                <div><%=string.Format("{0}___{1}",o.MenuNo,o.MenuName) %></div>
                <%} %>
            </td>
            <td style="vertical-align:top">
           <span style="background-color:aqua">設備操作</span>
                <%foreach (var o in this.ListMenus.Where(i => i.UpMenuNo == "04"))
                    { %>
                <div><%=string.Format("{0}___{1}",o.MenuNo,o.MenuName) %></div>
                <%} %>
            </td>
            <td style="vertical-align:top">
           <span style="background-color:aqua">資料查詢</span>
                <%foreach (var o in this.ListMenus.Where(i => i.UpMenuNo == "06"))
                    { %>
                <div><%=string.Format("{0}___{1}",o.MenuNo,o.MenuName) %></div>
                <%} %>
            </td>
            <td style="vertical-align:top">
           <span style="background-color:aqua">系統設定</span>
                <%foreach (var o in this.ListMenus.Where(i => i.UpMenuNo == "98"))
                    { %>
                <div><%=string.Format("{0}___{1}",o.MenuNo,o.MenuName) %></div>
                <%} %>
            </td>
            <td style="vertical-align:top">
           <span style="background-color:aqua">說明</span>
                <%foreach (var o in this.ListMenus.Where(i => i.UpMenuNo == "99"))
                    { %>
                <div><%=string.Format("{0}___{1}",o.MenuNo,o.MenuName) %></div>
                <%} %>
            </td>
        </tr>        
    </table>
    </form>
</body>
</html>
