<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LeaveQrCheck.aspx.cs" Inherits="SahoAcs.LeaveQrCheck" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta name="viewport" content="width=device-width" />
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript">
        //localStorage.clear();
        //window.history.forward(1);
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h3>開門密碼已到期</h3>
        <%Response.Redirect("CheckQrCode.aspx"); %>
    </div>
    </form>
</body>
</html>
