<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardRuleSetting.aspx.cs" Inherits="SahoAcs.CardRuleSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>CardRuleSetting</title>
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
        </div>
        <div>
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <table border="0">
                <tr>
                    <td>
                        <fieldset id="CardRule_List" runat="server" style="width: 380px">
                            <legend id="CardRule_Legend" runat="server">規則清單</legend>
                            <asp:UpdatePanel ID="CardRule_UpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table class="TableS3">
                                        <asp:Literal ID="popli_header" runat="server" />
                                        <tr>
                                            <td id="td_showGridView" runat="server" style="padding: 0">
                                                <asp:Panel ID="CardRule_Panel" runat="server" Height="270px" ScrollBars="Vertical">
                                                    <asp:GridView ID="CardRule_GridView" runat="server" BorderWidth="0px" Width="100%" OnRowDataBound="CardRule_GridView_RowDataBound" OnDataBound="CardRule_GridView_DataBound" OnRowDeleting="CardRule_GridView_RowDeleting" AutoGenerateColumns="False">
                                                        <Columns>
                                                            <asp:BoundField DataField="CardRuleIndex" HeaderText="編號" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="CardRule" HeaderText="時區規則" HeaderStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Button ID="DeleteCardRule" runat="server"  Text="<%$ Resources:Resource, btnRemove%>" Height="20px" Font-Size="Smaller" CommandName="Delete" />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" />
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
                    <td>
                        <table border="0" class="Item">
                            <tr>
                                <th>
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Label ID="popL_CardRuleIndex" runat="server" Text="時區規則："></asp:Label>

                                            <asp:TextBox ID="popInput_CardRuleIndex" runat="server" Width="30px" MaxLength="2"></asp:TextBox>

                                            <asp:DropDownList ID="popInput_CardRule" runat="server" Font-Size="13px"></asp:DropDownList>
                                            <asp:Button ID="popB_Add" runat="server" CssClass="IconRight"  Text="<%$ Resources:Resource, btnJoin%>" />

                                            <cc1:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                                TargetControlID="popInput_CardRuleIndex" MessageValidatorTip="true"
                                                Mask="99" PromptCharacter="">
                                            </cc1:MaskedEditExtender>
                                            <cc1:MaskedEditValidator ID="MaskedEditValidator1" runat="server"
                                                ControlExtender="MaskedEditExtender1" ControlToValidate="popInput_CardRuleIndex">
                                            </cc1:MaskedEditValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </th>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="popL_FunctionRemind" runat="server" Font-Size="13px" ForeColor="#FFFFFF"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <asp:Button ID="popB_Save" runat="server" CssClass="IconSave" EnableViewState="False" OnClick="popB_Save_Click"  Text="<%$ Resources:Resource, btnSave%>"  />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

