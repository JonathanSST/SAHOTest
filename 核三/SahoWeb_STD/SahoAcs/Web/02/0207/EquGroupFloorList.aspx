<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquGroupFloorList.aspx.cs" Inherits="SahoAcs.EquGroupFloorList" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>FloorNameList</title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css">
    <link href="../../../Css/StyleWindow.css" rel="stylesheet" type="text/css">
</head>
<body>
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideCardExtData" runat="server" />
        </div>
        <div>
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <table border="0">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="Elevator_UpdatePanel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table border="0" class="TableS3">
                                    <asp:Literal ID="popli_header" runat="server" />
                                    <tr>
                                        <td colspan="3">
                                            <asp:Panel ID="Elevator_Panel" runat="server" Width="590" Height="262px" ScrollBars="Vertical">
                                                <asp:GridView ID="Elevator_GridView" runat="server" BorderWidth="1" Width="100%" OnRowDataBound="Elevator_GridView_RowDataBound" AutoGenerateColumns="false">
                                                    <Columns>
                                                        <asp:BoundField DataField="IOIndex" HeaderText="接點編號" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1" />
                                                        <asp:BoundField DataField="FloorName" HeaderText="樓層名稱" HeaderStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1" />
                                                        <asp:TemplateField HeaderText="控制" ItemStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="FloorControl" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="popB_Save" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
