<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CopyCardAuthDetail.aspx.cs" Inherits="SahoAcs.Web._03._0303.CopyCardAuthDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
<form id="form1" runat="server">

<div id="MainDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;">
    <table>
        <tr>
            <td>
                <fieldset id="EquList_Fieldset" runat="server" style="width: 580px; height: 150px">                                    
                    <legend id="EquList_Legend" runat="server">
                        <asp:Label ID="lblEquList" runat="server" Text="權限複製清單(共0筆)"></asp:Label>
                    </legend>
                    <asp:Panel ID="EquListPanel" runat="server" ScrollBars="Auto" Height="140px">
                        <asp:Table ID="EquList" runat="server"></asp:Table>
                    </asp:Panel>
                </fieldset>
            </td>

            <td>
                <fieldset id="Fieldset1" runat="server" style="width: 290px; height: 150px">                                
                    <legend id="CardEquGroupLegend" runat="server">
                        <asp:Label ID="lblEquGroupList" runat="server" Text="設備群組權限複製清單(共0筆)"></asp:Label>
                    </legend>
                    <asp:Panel ID="EquGroupListPanel" runat="server" ScrollBars="Auto" Height="140px">
                        <asp:Table ID="EquGroupList" runat="server"></asp:Table>
                    </asp:Panel>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align: center">
                <input type="button" value="關閉此頁面" class="IconCancel" id="btnClose"/>
            </td>
        </tr>
    </table>

</div>
</form>
</body>
</html>
