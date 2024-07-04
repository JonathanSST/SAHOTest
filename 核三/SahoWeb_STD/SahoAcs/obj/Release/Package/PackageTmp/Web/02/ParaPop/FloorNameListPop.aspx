<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FloorNameListPop.aspx.cs" Inherits="SahoAcs.FloorNameListPop" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>FloorNameList</title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideEquParaID" runat="server" />
            <asp:HiddenField ID="hideParaValue" runat="server" />
            <asp:HiddenField ID="hideFloorName" runat="server" />
        </div>
        <div>           
            <table>
                <tr>
                    <td>
                        <asp:Label ID="popL_FloorCount" runat="server" Text="樓層數：" Font-Size="13px" ForeColor="#FFFFFF"></asp:Label>
                        <%--<asp:TextBox ID="popInput_FloorCount" runat="server" Width="50px" MaxLength="2"></asp:TextBox>--%>
                        <input type="text" name="FloorCount" id="FloorCount" style="width:50px" maxlength="2" onkeyup="return ValidateNumber(this,value)" />
                        <%--<asp:Button ID="popB_SetElevatorButton" runat="server" Text="樓層設定" CssClass="IconSet" />--%>
                        <input type="button" id="BtnSetElevator" value="樓層設定" class="IconSet" onclick="SetElevatorRow()" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <fieldset id="Elevator_List" runat="server" style="width: 495px">
                            <legend id="Elevator_Legend" runat="server">樓層清單</legend>
                                    <table border="0" class="TableS3">
                                        <asp:Literal ID="popli_header" runat="server" />
                                        <tr>
                                            <td colspan="2">
                                                <asp:Panel ID="Elevator_Panel" runat="server" Width="480px" Height="240px" ScrollBars="Vertical">
                                                    <asp:GridView ID="Elevator_GridView" runat="server" BorderWidth="0" Width="98%" OnRowDataBound="Elevator_GridView_RowDataBound" AutoGenerateColumns="false">
                                                        <Columns>
                                                            <asp:BoundField DataField="IoIndex" HeaderText="接點編號" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1" />
                                                            <asp:TemplateField HeaderText="樓層名稱" ItemStyle-BorderWidth="1">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="FloorName" runat="server" BorderWidth="0" Width="98%" Text='<%# DataBinder.Eval(Container.DataItem,"FloorName") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>                                
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 5px 0 0 0">
                        <asp:Label ID="popL_FunctionRemind" runat="server" Font-Size="13px" ForeColor="#BFFFEF"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 10px 0 5px 0; text-align: center">
                            <input type="button" value="<%=Resources.Resource.btnSave %>" class="IconSave" onclick="DoSave()" />
                          <input type="button" value="<%=Resources.Resource.btnCancel %>" class="IconCancel" onclick="DoClose()" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

