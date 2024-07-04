<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryPsnEquAuth.aspx.cs" Inherits="SahoAcs.QueryPsnEquAuth" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideEquModel" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                        </th>
                        <th>
                            <asp:Label ID="Label_Building" runat="server" Text="<%$Resources:ttBuild %>"></asp:Label>
                        </th>
                        <th></th>

                        <th>
                            <span class="Arrow01"></span>
                        </th>
                        <th>
                            <asp:Label ID="Label_Psn" runat="server" Text="<%$Resources:lblPerson %>" ></asp:Label>
                        </th>
                        <td></td>
                        <th>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="DropDown_Building" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td></td>

                        <td></td>
                        <td style="width:200px">
                            <asp:TextBox ID="Query_Psn" runat="server" CssClass="TextBoxRequired" Width="95%"></asp:TextBox>
                        </td>
                        <td></td>
                        <th>
                            <asp:CheckBox ID="ChkOutDate" runat="server" Width="20" Height="20"/>含過期權限資料
                        </th>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" OnClick="QueryButton_Click" CssClass="IconSearch" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1" style="width: 1000px">
                <%--GridView Header的Html Code--%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                DataKeyNames="EquNo" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting" EnableViewState="False">
                                <Columns>
                                    <asp:BoundField DataField="PsnNo" HeaderText="<%$Resources:ttPsnNo %>" SortExpression="PsnNo" />
                                    <asp:BoundField DataField="PsnName" HeaderText="<%$Resources:ttPsnName %>" SortExpression="PsnName" />
                                    <asp:BoundField DataField="Building" HeaderText="<%$Resources:ttBuild %>" SortExpression="Building" />
                                    <asp:BoundField DataField="Floor" HeaderText="<%$Resources:ttFloor %>" SortExpression="Floor" />
                                    <asp:BoundField DataField="EquClass" HeaderText="<%$Resources:ttEquType %>" SortExpression="EquClass" />
                                    <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:ttEquNo %>" SortExpression="EquNo" />
                                    <asp:BoundField DataField="EquName" HeaderText="<%$Resources:ttEquName %>" SortExpression="EquName" />
                                    <asp:BoundField DataField="TimeRange" HeaderText="有效時間"/>
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
                <%--GridView Pager的Html Code--%>
                <asp:Literal ID="li_Pager" runat="server" />
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div>        
        <asp:Button ID="ExportButton" runat="server" Text="<%$Resources:Resource,btnExport %>" OnClick="ExportButton_Click" CssClass="IconExport" />
    </div>
</asp:Content>
