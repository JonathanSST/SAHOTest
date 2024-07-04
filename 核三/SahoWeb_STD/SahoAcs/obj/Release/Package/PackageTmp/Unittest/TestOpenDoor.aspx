<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestOpenDoor.aspx.cs" Inherits="SahoAcs.Unittest.TestOpenDoor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript">
        function SendOpen(val1) {
            document.getElementById("EquNo").value = val1;
            form1.submit();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="hidden" id="EquNo" name="EquNo" value="C004_1" />
        <input type="button" onclick="SendOpen('C004_1')" value="開門(C004_1)" />
        <input type="button" onclick="SendOpen('C005_1')" value="開門(C005_1)" />
    </div>
    </form>
</body>
</html>
