<%@ Page Language="C#" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" CodeBehind="CardLogInput0411.aspx.cs" Inherits="SahoAcs.Web.CardLogInput0411" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <th colspan="2">
                <span class="Arrow01"></span>
                <asp:Label ID="lblCondition" runat="server" Text="學號、姓名或卡號"></asp:Label>
            </th>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtCondition" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="查　詢" CssClass="IconSearch" />
            </td>
        </tr>
    </table>
    <table class="TableWidth">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="TableS1" style="width:700px">
                            <%--GridView Header的Html Code--%>
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight" Width="700px">
                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                            AllowPaging="True" AutoGenerateColumns="False" Width="100%"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound" OnPageIndexChanging="GridView_PageIndexChanging">
                                            <Columns>
                                                <asp:TemplateField HeaderText="補登" SortExpression="CardID">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>                                                        
                                                        <input type="button" class="IconEdit" value="補登" onclick="CallEdit('<%#DataBinder.Eval(Container.DataItem, "CardID") %>')" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="PsnNo" HeaderText="學號" SortExpression="PsnNo" />
                                                <asp:BoundField DataField="PsnName" HeaderText="人員姓名" SortExpression="PsnName" />
                                                <asp:BoundField DataField="CardNo" HeaderText="卡號" SortExpression="CardNo" />                                                
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
            </td>
        </tr>
        <tr>
            <td>
                <%--<asp:Button ID="btnEdit" runat="server" Text="補登記錄" CssClass="IconEdit" />--%>
                <%--<input type="button" id="btnEdit" value="補登記錄" class="IconEdit" onclick="CallEdit('')" />--%>                
            </td>
        </tr>
    </table>    
</asp:Content>
