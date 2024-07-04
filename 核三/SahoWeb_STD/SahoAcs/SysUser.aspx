<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SysUser.aspx.cs" Inherits="SahoAcs.SysUser" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="SahoAcs" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <%foreach(DataRow r in this.DataResult.Rows){ %>
        <%=r["UserName"].ToString() %>
        <%} %>
    </div>
    </form>
</body>
</html>
