﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" CodeBehind="WorkHours.aspx.cs" Inherits="SahoAcs.WorkHours" %>

<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblCompany" runat="server" Text="公司"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblDepartment" runat="server" Text="部門"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblPsnType" runat="server" Text="類型"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblPsnNoOrName" runat="server" Text="人員編號或姓名"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblCardNo" runat="server" Text="卡號"></asp:Label>
                        </th>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="dropCompany" runat="server" Width="100px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="dropDept" runat="server" Width="100px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="dropPsnType" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPsnNoOrName" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCardNo" runat="server" Width="100px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="查　詢" CssClass="IconSearch" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblDateBetween" runat="server" Text="日期時段"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeSDate" />
                        </td>
                        <th>至</th>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeEDate" />
                        </td>
                    </tr>
                </table>

            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1" style="width: 800px">
                <%--GridView Header的Html Code--%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight" Width="1225px" >
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" Width="100%" PageSize="5" EnableViewState="False"
                                AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound" OnPageIndexChanging="GridView_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="CompanyName" HeaderText="公司" SortExpression="CompanyName" />
                                    <asp:BoundField DataField="DeptName" HeaderText="部門" SortExpression="DeptName" />
                                    <asp:BoundField DataField="ItemName" HeaderText="類型" SortExpression="ItemName" />
                                    <asp:BoundField DataField="PsnName" HeaderText="人員" SortExpression="PsnName" />
                                    <asp:BoundField DataField="PsnNo" HeaderText="人員編號" SortExpression="PsnNo" />
                                    <asp:BoundField DataField="CardNo" HeaderText="卡號" SortExpression="CardNo" />
                                    <asp:BoundField DataField="WorkTime" HeaderText="工時小計" SortExpression="WorkTime" />
                                    <asp:BoundField DataField="RealWorkTime" HeaderText="扣除午休實際工時" SortExpression="RealWorkTime" />
                                </Columns>
                                <PagerTemplate>
                                    <asp:LinkButton ID="lbtnFirst" runat="server" CommandName="Page" Font-Overline="false" Text="<%$ Resources:Resource, lbtnFirst %>"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnPrev" runat="server" Font-Overline="false" Text="<%$ Resources:Resource, lbtnPrev %>"></asp:LinkButton>
                                    <asp:PlaceHolder ID="phdPageNumber" runat="server"></asp:PlaceHolder>
                                    <asp:LinkButton ID="lbtnNext" runat="server" Font-Overline="false" Text="<%$ Resources:Resource, lbtnNext %>"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnLast" runat="server" CommandName="Page" Font-Overline="false" Text="<%$ Resources:Resource, lbtnLast %>"></asp:LinkButton>
                                    <asp:PlaceHolder ID="phdPsnCount" runat="server"></asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phdTotalWorkTime" runat="server"></asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phdTotalRealWorkTime" runat="server"></asp:PlaceHolder>
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
    <br />
    <div>
        <asp:Button ID="ExportButton" runat="server" Text="匯　出" OnClick="ExportButton_Click" CssClass="IconExport" />
    </div>
</asp:Content>
