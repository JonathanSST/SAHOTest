<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" 
    CodeBehind="QueryCardExt.aspx.cs" EnableEventValidation="false" 
    Inherits="SahoAcs.Web._06._0623.QueryCardExt" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hCtrlArea" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="GridView_Panel_scrollTop" runat="server" />
    </div>

    <table class="Item">
        <tr>
            <th><span class="Arrow01"></span></th>
            <th>
                <asp:Label ID="labCtrlArea" runat="server" Text="<%$Resources:ttCtrlArea %>"></asp:Label>
            </th>
            <th></th>
        </tr>
        <tr>
            <th></th>
            <td>
                <asp:DropDownList ID="ddlCtrlArea" runat="server" Font-Size="10pt" Width="300px">
                </asp:DropDownList></td>
            <td>
                <asp:Button ID="btnQuery" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" /></td>
        </tr>
    </table>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <table class="TableS1">
            <asp:Literal ID="li_header" runat="server" />
            <tr>
                <td id="td_showGridView" runat="server">
                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" Width="100%" Height="300px">
                        <asp:GridView ID="MainGridView" runat="server" EnableViewState="False" SkinID="GridViewSkin" 
                            Width="100%" AutoGenerateColumns="False" OnRowDataBound="MainGridView_RowDataBound" 
                            OnDataBound="MainGridView_DataBound" OnPageIndexChanging="MainGridView_PageIndexChanging"
                            OnSorting="GridView_Sorting" 
                            DataKeyNames="CardNo" AllowPaging="True" AllowSorting="True" PageSize="20" >
                            <Columns>
                                <asp:BoundField DataField="CardNo" HeaderText="<%$Resources:ttCardNo %>" SortExpression="CardNo" />
                                <asp:BoundField DataField="PsnNo" HeaderText="<%$Resources:ttPsnNo %>" SortExpression="PsnNo" />
                                <asp:BoundField DataField="PsnName" HeaderText="<%$Resources:ttPsnName %>" SortExpression="PsnName" />
                                <asp:BoundField DataField="DepNo" HeaderText="<%$Resources:ttDepNo %>" SortExpression="DepNo" />
                                <asp:BoundField DataField="DepName" HeaderText="<%$Resources:ttDepName %>" SortExpression="DepName" />
                                <asp:BoundField DataField="LastTime" HeaderText="<%$Resources:ttLastTime %>" SortExpression="LastTime" >
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>
                                <asp:BoundField DataField="LastDoorNo" HeaderText="<%$Resources:ttLastDoorNo %>" SortExpression="LastDoorNo" >
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>
                                <asp:BoundField DataField="CtrlAreaName" HeaderText="<%$Resources:ttCtrlArea %>" SortExpression="CtrlAreaName" />
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
            <asp:Literal ID="li_Pager" runat="server" />
        </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <br />
    <div>
        <asp:Button ID="ExportButton" runat="server" Text="<%$Resources:Resource,btnExport %>" CssClass="IconExport" />
    </div>
</asp:Content>

