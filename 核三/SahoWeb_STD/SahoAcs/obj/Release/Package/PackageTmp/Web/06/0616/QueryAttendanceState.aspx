<%@ Page Language="C#" CodeBehind="QueryAttendanceState.aspx.cs" Inherits="SahoAcs.QueryAttendanceState" MasterPageFile="~/Site1.Master" 
    Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/CalendarFrm.ascx" TagPrefix="uc1" TagName="PickDate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hidePsnID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideEquModel" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    </div>
    <table cellspacing="5" class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th colspan="3">
                            <asp:Label ID="lblDate" runat="server" Text="刷卡日期："></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="lblDept" runat="server" Text="部門："></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="lblPsnNo" runat="server" Text="員工編號或姓名："></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <uc1:PickDate ID="pdSDate" runat="server" />
                        </td>
                        <td>至</td>
                        <td>
                            <uc1:PickDate ID="pdEDate" runat="server" />
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlDept" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPsnNo" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="查　詢" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table cellspacing="0" border="1" style="border-collapse: collapse">
                            <%--GridView Header的Html Code--%>
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" Height="350px">
                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" Width="1300px" PageSize="5"
                                            DataKeyNames="PsnNo" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound" OnPageIndexChanging="GridView_PageIndexChanging">
                                            <Columns>
                                                <asp:BoundField DataField="DateData" HeaderText="日期" SortExpression="DateData" />
                                                <asp:BoundField DataField="OrgName" HeaderText="部門" SortExpression="OrgName" />
                                                <asp:BoundField DataField="PsnNo" HeaderText="員工編號" SortExpression="PsnNo" />
                                                <asp:BoundField DataField="PsnName" HeaderText="員工姓名" SortExpression="PsnName" />
                                                <asp:BoundField DataField="CardLog" HeaderText="刷卡時間1" SortExpression="CardLog" />
                                                <asp:BoundField HeaderText="刷卡時間2" />
                                                <asp:BoundField HeaderText="刷卡時間3" />
                                                <asp:BoundField HeaderText="刷卡時間4" />
                                                <asp:BoundField HeaderText="刷卡時間5" />
                                                <asp:BoundField HeaderText="刷卡時間6" />
                                                <asp:BoundField DataField="ItemName" HeaderText="假別" SortExpression="ItemName" />
                                                <asp:BoundField DataField="AskTime" HeaderText="請假時間" SortExpression="AskTime" />
                                            </Columns>
                                            <PagerTemplate>
                                                <asp:LinkButton ID="lbtnFirst" runat="server" CommandName="Page" Font-Overline="false">第一頁</asp:LinkButton>
                                                <asp:LinkButton ID="lbtnPrev" runat="server" Font-Overline="false">前一頁</asp:LinkButton>
                                                <asp:PlaceHolder ID="phdPageNumber" runat="server"></asp:PlaceHolder>
                                                <asp:LinkButton ID="lbtnNext" runat="server" Font-Overline="false">下一頁</asp:LinkButton>
                                                <asp:LinkButton ID="lbtnLast" runat="server" CommandName="Page" Font-Overline="false">最末頁</asp:LinkButton>
                                            </PagerTemplate>
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
            <td>最後同步時間：<asp:Label ID="lblLastTime" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
