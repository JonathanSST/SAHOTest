<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AlertTest.aspx.cs" Inherits="SahoAcs.Unittest.AlertTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <%=string.Format(GetGlobalResourceObject("ResourceAlert","AlertType1").ToString(),"Equ01") %>
    </div>
    </form>
</body>
</html>
