<%@ Import Namespace="SahoAcs.DBClass" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AliveDate.aspx.cs" Inherits="SahoAcs.AliveDate" %>

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
                <%
                    var dateinfo = DongleVaries.GetDateAliveInfo();                    
                     %>     
                <%if (!dateinfo.Item1)
                    { %>
                <tr>
                    <td>
                        <fieldset id="Elevator_List" name="Elevator_List" style="width: 600px">
                            <legend id="Elevator_Legend" style="font-size: 15px">租用合約到期通知</legend>
                            <span style="font-size: 16pt">感謝您使用SMS管理平台，本系統將於<%=dateinfo.Item2.ToString("yyyy/MM/dd") %> 租用到期，若需要協助或是辦理續約，請連絡本公司服務人員。</span>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <input type="button" value="確定" class="BtnLogin" id="BtnAlive" style="margin:0" />
                    </td>
                </tr>
                <% } else { %>
                <tr>
                    <td>
                        <fieldset id="Elevator_List" name="Elevator_List" style="width: 600px">
                            <legend id="Elevator_Legend" style="font-size: 15px">租用合約到期通知</legend>
                            <span style="font-size: 16pt">感謝您使用SMS管理平台，本系統已於<%=dateinfo.Item2.ToString("yyyy/MM/dd") %> 租用到期，若需要協助或是辦理續約，請連絡本公司服務人員。</span>
                        </fieldset>
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
    </div>
</body>
</html>
