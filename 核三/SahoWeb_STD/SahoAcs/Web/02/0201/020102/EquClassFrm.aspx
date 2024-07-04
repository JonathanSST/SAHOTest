<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquClassFrm.aspx.cs" Inherits="SahoAcs.Web._02._0201._020102.EquClassFrm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form_class" runat="server">
        <div id="MasterDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid">
            <div>
                <table class="popItem">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="popLabel_OrgList" runat="server" Text="新增設備" Font-Bold="True"></asp:Label>
                        </th>
                    </tr>
                </table>
            </div>
            <table>
                <tr>
                    <td>
                        <input type="button" value="新增門禁設備" class="IconEdit" onclick="SetEquData('DoorAccess')" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <input type="button" value="新增考勤設備" class="IconEdit" onclick="SetEquData('TRT')"/>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <input type="button" value="新增電梯設備" class="IconEdit" onclick="SetEquData('Elev')"/>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <input type="button" value="取消" class="IconCancel" id="popB_Cancel" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
