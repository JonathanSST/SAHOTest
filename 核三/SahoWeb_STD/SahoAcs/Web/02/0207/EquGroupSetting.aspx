<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="EquGroupSetting.aspx.cs" Inherits="SahoAcs.EquGroupSetting" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>EquGroupSetting</title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css">
    <link href="../../../Css/StyleWindow.css" rel="stylesheet" type="text/css">
</head>
<body>
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquGrpID" runat="server" />
            <asp:HiddenField ID="hideEquGrpNo" runat="server" />
        </div>

        <div>
            <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
            <table class="Item">
                <tr>
                    <%--待加入清單GridView--%>
                    <td style="vertical-align: top">
                        <table border="0" >
                            <tr>
                                <td>
                                    <table border="0" class="Item">
                                        <tr>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="PendingLabelEquClass" runat="server" Text="<%$Resources:ResourceGrp,EquType %>" Font-Size="14px"></asp:Label>
                                            </th>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="PendingLabelEquNo" runat="server" Text="<%$Resources:ResourceGrp,EquNo %>" Font-Size="14px"></asp:Label></th>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="PendingInput_EquClass" runat="server" OnInit="PendingInput_EquClass_Init" Font-Size="13px"></asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="PendingInput_EquNo" runat="server" Font-Size="14px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="PendingButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" Font-Size="14px" CssClass="IconSearch" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <fieldset id="Pending_List" runat="server" style="width: 445px; height: 280px">
                                        <legend id="Pending_Legend" runat="server"><%=Resources.ResourceGrp.ttUnJoin %></legend>
                                        <asp:UpdatePanel ID="Pending_UpdatePanel" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <table class="TableS3">
                                                    <asp:Literal ID="Pendingli_header" runat="server" />
                                                    <tr>
                                                        <td colspan="4">
                                                            <asp:Panel ID="Pending_Panel" runat="server" Height="210px" ScrollBars="Vertical">
                                                                <asp:GridView ID="Pending_GridView" runat="server" Width="100%" OnRowDataBound="Pending_GridView_RowDataBound" AutoGenerateColumns="false">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="" ItemStyle-BorderWidth="1" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="PendingCheckBox" runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:ResourceGrp,EquNo %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1" />
                                                                        <asp:BoundField DataField="EquName" HeaderText="<%$Resources:ResourceGrp,EquName %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1" />
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
                        </table>
                    </td>
                    <%--控制欄位--%>
                    <td style="width: 30px; vertical-align: bottom">
                        <table style="height: 290px;">
                            <tr style="vertical-align: bottom">
                                <td>
                                    <asp:Button ID="AddEquButton" runat="server"  Text="<%$ Resources:Resource, btnJoin%>" CssClass="IconRight" />
                                </td>
                            </tr>
                            <tr style="height: 20px;">
                                <td></td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td>
                                    <asp:Button ID="RemoveEquButton" runat="server"  Text="<%$ Resources:Resource, btnRemove%>" CssClass="IconLeft" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <%--已加入清單GridView--%>
                    <td style="vertical-align: top">
                        <table border="0" >
                            <tr>
                                <td>
                                    <table border="0" class="Item">
                                        <tr>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="QueueLabelEquClass" runat="server" Text="<%$Resources:ResourceGrp,EquType %>" Font-Size="14px"></asp:Label>
                                            </th>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="QueueLabelEquNo" runat="server" Text="<%$Resources:ResourceGrp,EquNo %>" Font-Size="14px"></asp:Label></th>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="QueueInput_EquClass" runat="server" OnInit="QueueInput_EquClass_Init" Font-Size="13px"></asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="QueueInput_EquNo" runat="server" Font-Size="14px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="QueueButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" Font-Size="14px" CssClass="IconSearch" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <fieldset id="Queue_List" runat="server" style="width: 650px; height: 280px">
                                        <legend id="Queue_Legend" runat="server"><%=Resources.ResourceGrp.ttJoin %></legend>
                                        <asp:UpdatePanel ID="Queue_UpdatePanel" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <table border="0" class="TableS3">
                                                    <asp:Literal ID="Queueli_header" runat="server" />
                                                    <tr>
                                                        <td colspan="6">
                                                            <asp:Panel ID="Queue_Panel" runat="server" Height="190px" ScrollBars="Vertical">
                                                                <asp:GridView ID="Queue_GridView" runat="server" BorderWidth="1" Width="100%" OnRowDataBound="Queue_GridView_RowDataBound" AutoGenerateColumns="false">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="" ItemStyle-BorderWidth="1" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="QueueCheckBox" runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:ResourceGrp,EquNo %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1" />
                                                                        <asp:BoundField DataField="EquName" HeaderText="<%$Resources:ResourceGrp,EquName %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-BorderWidth="1" />
                                                                        <asp:TemplateField HeaderText="<%$Resources:ResourceGrp,Rule %>" ItemStyle-BorderWidth="1" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:DropDownList ID="QueueDropDownList" runat="server" Width="120px"></asp:DropDownList>
                                                                                <%--<asp:HiddenField ID="hideSelectedOptionValue" runat="server" />--%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="<%$Resources:ResourceGrp,lblFloorName %>" ItemStyle-BorderWidth="1" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Button ID="QueueFloorButton" runat="server" Text="<%$Resources:ResourceGrp,btnSetting2 %>" Enabled="false" CssClass="IconSet" Width="100px" />
                                                                                <%--<asp:HiddenField ID="hideSelectedCardExtDataValue" runat="server" />--%>
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
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Button ID="popB_Save" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" OnClick="popB_Save_Click" CssClass="IconSave" />
                        <asp:Button ID="popB_Cancel" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
