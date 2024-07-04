<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParaSetting.aspx.cs" Inherits="SahoAcs.ParaSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>ParaSetting</title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
    <link href="/Css/colorbox.css" rel="stylesheet" />
    <link href="/Css/jquery-ui.css" rel="stylesheet" type="text/css" />

</head>
<body style="background-color: #069">
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideCtrlID" runat="server" />
        </div>

        <div id="dialog" title=""></div>

        <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
        <table class="Item">
            <tr>
                <td>
                    <fieldset id="Elevator_List" name="Elevator_List" runat="server" style="width: 700px">
                        <legend id="Elevator_Legend" runat="server" style="font-size: 15px"><asp:Label ID="labTilte" runat="server" Text="參數列表"></asp:Label></legend>
                        <asp:UpdatePanel ID="ParaUpdataPanel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table border="0" class="TableS3">
                                    <asp:Literal ID="li_header" runat="server" />
                                    <tr>
                                        <td colspan="7" style="padding: 0">
                                            <asp:Panel ID="Elevator_Panel" runat="server" Height="300px" Width="100%" ScrollBars="Vertical">
                                                <asp:GridView ID="ParaGridView" Width="680px" runat="server" DataKeyNames="Seq" BorderWidth="0px" AutoGenerateColumns="False" OnRowDataBound="ParaGridView_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField="Seq" HeaderText="項目" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="ParaDesc" HeaderText="參數名稱" SortExpression="ParaDesc" />
                                                        <%--<asp:BoundField DataField="ParaUI" HeaderText="參數數值" SortExpression="ParaUI" />--%>
                                                        <asp:TemplateField HeaderText="參數數值" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:Button ID="B_ParaItem" runat="server" Text="設　　定" CssClass="IconSet" />
                                                                <asp:TextBox ID="T_ParaItem" runat="server"></asp:TextBox>
                                                                <asp:DropDownList ID="D_ParaItem" runat="server"></asp:DropDownList>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <input id="cbxHeader" type="checkbox" onclick="ChangeCbxStatus()" />重送
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="cbx" runat="server" />
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
                <td style="text-align:center">
                    <asp:Button ID="popB_Save" runat="server" Text="<%$ Resources:Resource, btnSave%>" OnClick="popB_Save_Click" CssClass="IconSave" />
                    <asp:Button ID="popB_Refresh" runat="server" Text="重新傳送" OnClick="popB_Refresh_Click" CssClass="IconSave" />
                    <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" OnClick="popB_Cancel_Click" CssClass="IconCancel" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
