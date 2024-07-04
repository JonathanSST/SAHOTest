<%@ Page Language="C#" CodeBehind="QueryTRTFirstEndCardLog.aspx.cs" Inherits="SahoAcs.QueryTRTFirstEndCardLog" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register src="/uc/CalendarFrm.ascx" tagname="CalendarFrm" tagprefix="uc2" %>

<%-- cellspacing="0" border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死 --%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <%-- ************************************************** 網頁畫面設計一 ************************************************** --%>
    <%-- 主要作業畫面：隱藏欄位部份 --%>
    <asp:HiddenField ID="hUserID" runat="server" />
    <asp:HiddenField ID="hPsnID" runat="server" />
    <asp:HiddenField ID="hOwnerList" runat="server" />
    <asp:HiddenField ID="hComplexQueryWheresql" runat="server" />
    <asp:HiddenField ID="hSelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <%-- 主要作業畫面：查詢部份 --%>
    <table class="Item">
        <tr>
            <th colspan="3">
                <span class="Arrow01"></span>
                <asp:Label ID="QueryLabel_CardTime" runat="server" Text="<%$Resources:ttCardTime %>"></asp:Label>
            </th>
            <th runat="server" id="ShowPsnInfo4">
                <span class="Arrow01"></span>
                <asp:Label ID="QueryLabel_CardNo" runat="server" Text="<%$Resources:ttCardNo %>"></asp:Label>
            </th>
            <th runat="server" id="ShowPsnInfo1">
                <span class="Arrow01"></span>
                <asp:Label ID="QueryLabel_PsnNo" runat="server" Text="<%$Resources:ttPsnNo %>"></asp:Label>
            </th>
            <th  runat="server" id="ShowPsnInfo5">
                <span class="Arrow01"></span>
                <asp:Label ID="QueryLabel_Dept" runat="server" Text="<%$Resources:ttDeptName %>"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <uc2:CalendarFrm ID="QueryLabel_PickDate_SDate" runat="server" />
            </td>
            <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
            <td>
                <uc2:CalendarFrm ID="QueryLabel_PickDate_EDate" runat="server" />
            </td>
            <td runat="server" id="ShowPsnInfo3">
                <asp:TextBox ID="QueryTextBox_CardNo" runat="server"></asp:TextBox>
            </td>
            <td runat="server" id="ShowPsnInfo2">
                <asp:TextBox ID="QueryTextBox_PsnNo" runat="server"></asp:TextBox>
            </td>
            <td runat="server" id="ShowPsnInfo6">
                <asp:DropDownList ID="ddlDept" runat="server" Width="150px">
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
            </td>
        </tr>
    </table>

    <%-- 主要作業畫面：表格部份 --%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1">
                <%-- GridView Header's HTML Code --%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                            <%-- GridView Body's HTML Code --%>
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" Width="100%" PageSize="5"
                                AllowPaging="True" AutoGenerateColumns="False"
                                OnDataBound="GridView_Data_DataBound" OnRowDataBound="GridView_Data_RowDataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="False" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="LogStatusName" HeaderText="<%$Resources:ttResult %>" SortExpression="LogStatusName" />
                                    <asp:BoundField DataField="Date" HeaderText="<%$Resources:ttCardDate %>" SortExpression="Date" />
                                    <asp:BoundField DataField="First" HeaderText="<%$Resources:ttFirst %>" SortExpression="First" />
                                    <asp:BoundField DataField="Last" HeaderText="<%$Resources:ttLast %>" SortExpression="Last" />
                                    <asp:BoundField DataField="CardNo" HeaderText="<%$Resources:ttCardNo %>" SortExpression="CardNo" />
                                    <asp:BoundField DataField="Dept" HeaderText="<%$Resources:ttDeptName %>" SortExpression="Dept" />
                                    <asp:BoundField DataField="PsnNo" HeaderText="<%$Resources:ttPsnNo %>" SortExpression="PsnNo" />
                                    <asp:BoundField DataField="PsnName" HeaderText="<%$Resources:ttPsnName %>" SortExpression="PsnName" />
                                </Columns>
                                <PagerTemplate>
                                    <asp:LinkButton ID="lbtnFirst" runat="server" CommandName="Page" Font-Overline="false" Text="<%$ Resources:Resource, lbtnFirst %>"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnPrev" runat="server" Font-Overline="false" Text="<%$ Resources:Resource, lbtnPrev %>"></asp:LinkButton>
                                    <asp:PlaceHolder ID="phdPageNumber" runat="server"></asp:PlaceHolder>
                                    <asp:LinkButton ID="lbtnNext" runat="server" Font-Overline="false" Text="<%$ Resources:Resource, lbtnNext %>"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnLast" runat="server" CommandName="Page" Font-Overline="false" Text="<%$ Resources:Resource, lbtnLast %>"></asp:LinkButton>
                                </PagerTemplate>
                            </asp:GridView>
                        </asp:Panel>
                    </td>
                </tr>
                <%-- GridView Pager's HTML Code --%>
                <asp:Literal ID="li_Pager" runat="server" />
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div>
        <asp:Button ID="ExportButton" runat="server" Text="<%$Resources:Resource,btnExport %>" OnClick="ExportButton_Click" OnClientClick="alert('檔案匯出需等待請勿關閉視窗!!');" CssClass="IconExport" />
        <asp:Button ID="PdfButton" runat="server" Text="匯出 PDF" OnClick="PdfButton_Click" CssClass="IconExport" />
    </div>
    <%-- 主要作業畫面：按鈕部份 --%>
</asp:Content>
