<%@ Import Namespace="SahoAcs.DBClass" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScAliveMsg.aspx.cs" Inherits="SahoAcs.ScAliveMsg" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div id="MasterDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid">
        <table class="Item" style="background-color: #069">
            <tbody>
                <tr>
                    <td>
                        <fieldset id="Elevator_List" name="Elevator_List" style="width: 600px">
                            <legend id="Elevator_Legend" style="font-size: 15px">SC狀態異常</legend>
                            <span style="font-size: 16pt">SC服務被停止或SC無法連線資料庫，請確認SC服務相關的LOG訊息排除後請重新登入!!</span>
                        </fieldset>
                    </td>
                </tr>                
            </tbody>
        </table>
    </div>
</body>
</html>
