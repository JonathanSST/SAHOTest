<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FloorNameList.aspx.cs" Inherits="SahoAcs.FloorNameList" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>FloorNameList</title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="../../../Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body onload="LoadData()">
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideEquParaID" runat="server" />
            <asp:HiddenField ID="hideParaValue" runat="server" />
            <asp:HiddenField ID="hideFloorName" runat="server" />
        </div>
        <div>
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <table>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="popL_FloorCount" runat="server" Text="樓層數：" Font-Size="13px" ForeColor="#FFFFFF"></asp:Label>
                                <asp:TextBox ID="popInput_FloorCount" runat="server" Width="50px" MaxLength="2"></asp:TextBox>
                                <asp:Button ID="popB_SetElevatorButton" runat="server" Text="樓層設定" CssClass="IconSet" />

                                <cc1:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                    TargetControlID="popInput_FloorCount" MessageValidatorTip="true"
                                    Mask="99" PromptCharacter="">
                                </cc1:MaskedEditExtender>
                                <cc1:MaskedEditValidator ID="MaskedEditValidator1" runat="server"
                                    ControlExtender="MaskedEditExtender1" ControlToValidate="popInput_FloorCount">
                                </cc1:MaskedEditValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <fieldset id="Elevator_List" runat="server" style="width: 495px">
                            <legend id="Elevator_Legend" runat="server">樓層清單</legend>
                            <asp:UpdatePanel ID="Elevator_UpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table border="0" class="TableS3">
                                        <asp:Literal ID="popli_header" runat="server" />
                                        <tr>
                                            <td colspan="2">
                                                <asp:Panel ID="Elevator_Panel" runat="server" Width="480px" Height="240px" ScrollBars="Vertical">
                                                    <asp:GridView ID="Elevator_GridView" runat="server" BorderWidth="0" Width="98%" OnRowDataBound="Elevator_GridView_RowDataBound" AutoGenerateColumns="false">
                                                        <Columns>
                                                            <asp:BoundField DataField="IOIndex" HeaderText="接點編號" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1" />
                                                            <asp:TemplateField HeaderText="樓層名稱" ItemStyle-BorderWidth="1">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="FloorName" runat="server" BorderWidth="0" Width="98%"></asp:TextBox>
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
                        <asp:Button ID="popB_Save" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" OnClick="popB_Save_Click" CssClass="IconSave" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

