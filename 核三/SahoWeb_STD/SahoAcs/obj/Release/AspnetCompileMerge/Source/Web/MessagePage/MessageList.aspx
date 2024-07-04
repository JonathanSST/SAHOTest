<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="SahoAcs.MessageList"  EnableEventValidation="false" Theme="UI" CodeBehind="MessageList.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
<form id="form1" runat="server">
      <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    </div>
    <table>
        <tr>
            <td>
                <asp:UpdatePanel ID="TypeListUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Timer ID="TypeListTimer" runat="server" OnTick="TypeListTimer_Tick" Interval="1000"></asp:Timer>
                        <asp:Panel ID="TypeListPanel" runat="server" ScrollBars="Vertical" Height="400px" BackColor="#0000FF">
                            <asp:Table ID="TypeList" runat="server"></asp:Table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td>
                <table style="border: 1px solid black">
                    <tr>
                        <td>
                            <asp:UpdatePanel ID="Msg_UpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table border="1" style="border-collapse: collapse">
                                        <%--GridView Header的Html Code--%>
                                        <asp:Literal ID="li_header" runat="server" />
                                        <tr>
                                            <td id="td_showGridView" runat="server" style="padding: 0">
                                                <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" Height="200px">
                                                    <asp:GridView ID="Msg_GridView" runat="server" PageSize="5"
                                                        DataKeyNames="LogTime" AutoGenerateColumns="False" Width="750"
                                                        OnRowDataBound="Msg_GridView_Data_RowDataBound" OnDataBound="Msg_GridView_Data_DataBound">
                                                        <Columns>
                                                            <asp:BoundField DataField="LogTime" HeaderText="時間" SortExpression="LogTime" />
                                                            <asp:BoundField DataField="Source" HeaderText="來源" SortExpression="Source" />
                                                            <asp:BoundField DataField="MsgContent" HeaderText="內容" SortExpression="MsgContent" />
                                                            <asp:BoundField DataField="ReadTime" HeaderText="已讀" SortExpression="ReadTime" />
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
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="TableWidth">
                                <tr>
                                    <td style="width: 10%">
                                        <asp:Label ID="Label_LogTime" runat="server" Text="時間："></asp:Label>
                                    </td>
                                    <td style="width: 40%">
                                        <asp:Label ID="Input_LogTime" runat="server"></asp:Label>
                                    </td>
                                    <td style="width: 10%">
                                        <asp:Label ID="Label_Source" runat="server" Text="來源："></asp:Label>
                                    </td>
                                    <td style="width: 40%">
                                        <asp:Label ID="Input_Source" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5">
                                        <asp:Label ID="Label_MsgContent" runat="server" Text="內容"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5">
                                        <asp:TextBox ID="Input_MsgContent" runat="server" ReadOnly="true" Width="99%" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
