<%@ Import Namespace="SahoAcs.DBClass" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SdAliveMsg.aspx.cs" Inherits="SahoAcs.SdAliveMsg" %>

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
                            <legend id="Elevator_Legend" style="font-size: 15px">SD狀態異常</legend>
                            <span style="font-size: 16pt">SD 狀態出現異常，請稍候再進行系統操作!!</span>
                        </fieldset>
                    </td>
                </tr>                
            </tbody>
        </table>
    </div>
</body>
</html>
