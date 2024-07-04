<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QRCodeReport.aspx.cs" Inherits="SahoAcs.QRCodeReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h3>手機開門掃描條碼</h3>
    </div>
    <div>
        <img src="data:image/png;base64, <%=this.Base64Image %>" alt="Red dot" />
    </div>
    <div>
        <h3>設備編號：<%=Request["EquNo"] %></h3>
    </div>
    <div>
        <h3>設備名稱：<%=this.EquName %></h3>
    </div>
    <div>
        <h3>位置：<%=this.EquBuilder %></h3>
    </div>
    </form>
</body>
</html>
