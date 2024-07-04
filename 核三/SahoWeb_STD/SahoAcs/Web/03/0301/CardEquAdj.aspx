<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CardEquAdj.aspx.cs" Inherits="SahoAcs.Web.CardEquAdj" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <table>
        <tr>
            <td>關鍵字</td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="InputWord" runat="server"></asp:TextBox></td>
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" /></td>
            <td>
                <asp:Button ID="QueryAllButton" runat="server" Text="複合查詢" /></td>
        </tr>
    </table>

    <table>
        <tr>
            <td style="vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table border="1" style="border-collapse: collapse">
                            <%--GridView Header的Html Code--%>
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" Height="200px">
                                        <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" Width="350px" PageSize="5"
                                            DataKeyNames="PsnID" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound" OnPageIndexChanging="GridView_PageIndexChanging" OnRowCommand="GridView_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="PsnID" HeaderText="ID" SortExpression="PsnID" Visible="false" />
                                                <asp:BoundField DataField="PsnNo" HeaderText="編號" SortExpression="PsnNo" />
                                                <asp:BoundField DataField="PsnName" HeaderText="姓名" SortExpression="PsnName" />
                                                <asp:BoundField DataField="VCard" HeaderText="" SortExpression="VCard" />
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
            <td style="vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <table style="text-align: left; width: 100%;">
                            <tr>
                                <td>
                                    <asp:Label ID="Label_lPsnNo" runat="server" Text="選擇編號："></asp:Label>
                                    <span>
                                        <asp:Label ID="Label_PsnNo" runat="server" Text=""></asp:Label></span>
                                </td>
                            </tr>
                        </table>
                        <table cellspacing="0" border="1" style="border-collapse: collapse">
                            <%--GridView Header的Html Code--%>
                            <asp:Literal ID="li_header2" runat="server" />
                            <tr>
                                <td id="td_showGridView2" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel2" runat="server" ScrollBars="Vertical" Height="150px">
                                        <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                        <asp:GridView runat="server" ID="MainGridView2" SkinID="GridViewSkin" Width="280px" PageSize="5"
                                            DataKeyNames="CardID" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound2" OnDataBound="GridView_Data_DataBound2" OnPageIndexChanging="GridView_PageIndexChanging2">
                                            <Columns>
                                                <asp:BoundField DataField="CardID" HeaderText="ID" SortExpression="CardID" Visible="false" />
                                                <asp:BoundField DataField="ItemName" HeaderText="卡別" SortExpression="ItemName" />
                                                <asp:BoundField DataField="CardNo" HeaderText="卡號" SortExpression="CardNo" />
                                            </Columns>
                                            <PagerTemplate>
                                                <asp:LinkButton ID="lbtnFirst2" runat="server" CommandName="Page" Font-Overline="false">第一頁</asp:LinkButton>
                                                <asp:LinkButton ID="lbtnPrev2" runat="server" Font-Overline="false">前一頁</asp:LinkButton>
                                                <asp:PlaceHolder ID="phdPageNumber2" runat="server"></asp:PlaceHolder>
                                                <asp:LinkButton ID="lbtnNext2" runat="server" Font-Overline="false">下一頁</asp:LinkButton>
                                                <asp:LinkButton ID="lbtnLast2" runat="server" CommandName="Page" Font-Overline="false">最末頁</asp:LinkButton>
                                            </PagerTemplate>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <%--GridView Pager的Html Code--%>
                            <asp:Literal ID="li_Pager2" runat="server" />
                        </table>
                        <table style="text-align: right; width: 100%;">
                            <tr>
                                <td>
                                    <br />
                                    <asp:Button ID="Button1" runat="server" Text="Button" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="SelectPsnID" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectCardID" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hSelectState" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectPsnNo" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectCardNo" runat="server" EnableViewState="False" />

</asp:Content>
