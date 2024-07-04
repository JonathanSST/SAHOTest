<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardFloor.aspx.cs" Inherits="SahoAcs.CardFloor" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script type="text/javascript">
        //全選
        function SelectAllCheckboxes() {
            window.alert = null;
            delete window.alert;
            elm = window.form1;
            var cb = document.getElementById('GridView1_CheckBox1');
            for (var i = 0; i < elm.length; i++) {
                var str = elm[i].name.split('$');
                var strlen = str.length - 1;
                if (str[strlen] == "RowCheckState1") {
                    if (elm.elements[i].checked != cb.checked)
                        elm.elements[i].click();
                }
            }
        }
    </script>
    <link href="../../Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="../../Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table border="1" style="border-collapse: collapse">
                    <%--GridView Header的Html Code--%>
                    <asp:Literal ID="li_header1" runat="server" />
                    <tr>
                        <td id="td_showGridView1" runat="server" style="padding: 0">
                            <asp:Panel ID="tablePanel1" runat="server" ScrollBars="Vertical" Width="390px" Height="315px">
                                <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                <asp:GridView runat="server" ID="GridView1" SkinID="GridViewSkin" Width="100%" PageSize="5"
                                    DataKeyNames="IOIndex" AutoGenerateColumns="False"
                                    OnRowDataBound="GridView1_Data_RowDataBound" OnDataBound="GridView1_Data_DataBound">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="CheckBox1" runat="server"></asp:CheckBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="RowCheckState1" runat="server"></asp:CheckBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                            <HeaderTemplate>
                                                <span>I/O</span><br />
                                                <span><%=GetGlobalResourceObject("ResourceEquData","lblIO") %></span>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <span><%# DataBinder.Eval(Container.DataItem, "IOIndex") %></span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField DataField="IOIndex" HeaderText="<%$Resources:ResourceEquData,lblIO %>" SortExpression="IOIndex" />--%>
                                        <asp:BoundField DataField="FloorName" HeaderText="<%$Resources:ResourceEquData,lblFloorName %>" SortExpression="FloorName" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </td>
                    </tr>
                    <%--GridView Pager的Html Code--%>
                    <asp:Literal ID="li_Pager" runat="server" />
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <table style="border-collapse: collapse">
            <tr>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="BtSave" runat="server" Text="<%$Resources:Resource,btnSave %>" OnClick="BtSave_Click" CssClass="IconOk" /></td>
                <td>
                    <asp:Button ID="BtCancel" runat="server" Text="<%$Resources:Resource,btnCancel %>" CssClass="IconCancel" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hFloor" runat="server" />
    </form>
</body>
</html>
