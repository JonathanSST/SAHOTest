<%@ Page Language="C#" CodeBehind="TempCardCreateWithArea.aspx.cs" Inherits="SahoAcs.TempCardCreateWithArea" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table style="width: 100%">
        <tr>
            <td>
                <%-- cellspacing="0" border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死 --%>
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

                <%-- ************************************************** 網頁畫面設計一 ************************************************** --%>

                <%-- 主要作業畫面：查詢部份 --%>
                <asp:UpdatePanel ID="UpdatePanel0" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="Item">
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <th>
                                                <span class="Arrow01"></span>
                                            </th>
                                            <th>
                                                <asp:Label ID="QueryLabel_CardNo" runat="server" Text="<%$ Resources:QueryLabel_CardNo %>"></asp:Label>
                                            </th>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="QueryInput_CardNo" runat="server"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td>
                                                <asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <%-- 主要作業畫面：表格部份 --%>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="TableS1" style="width: 100%">
                            <%-- GridView Header的Html Code --%>
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" Height="250">
                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" 
                                            DataKeyNames="CardNo" AllowPaging="True" AutoGenerateColumns="False" 
                                            OnDataBound="GridView_Data_DataBound" OnRowDataBound="GridView_Data_RowDataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="CardNo" HeaderText="<%$ Resources:CardNo %>" SortExpression="CardNo" />                                             
                                                <asp:BoundField DataField="PersonData" HeaderText="<%$ Resources:PsnID %>" SortExpression="PersonData" />
                                                <asp:BoundField DataField="CardAuthAllow" HeaderText="<%$ Resources:CardAuthAllow %>" SortExpression="CardAuthAllow" />
                                                <asp:BoundField DataField="Rev02" HeaderText="廠區" SortExpression="Rev02" />
                                                <asp:BoundField DataField="CardDesc" HeaderText="<%$ Resources:CardDesc %>" SortExpression="CardDesc" />
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
                            <%-- GridView Pager的Html Code --%>
                            <asp:Literal ID="li_Pager" runat="server" />
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <%-- 主要作業畫面：按鈕部份 --%>
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" /></td>
                        <td>
                            <asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit" /></td>
                        <td>
                            <asp:Button ID="DeleteButton" runat="server" Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete" /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <%-- 主要作業畫面：隱藏欄位部份 --%>
    <asp:HiddenField ID="hUserID" runat="server" />
    <asp:HiddenField ID="hOwnerList" runat="server" />
    <asp:HiddenField ID="hSelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hCardID" runat="server" EnableViewState="False" />

    <%-- ************************************************** 網頁畫面設計二 ************************************************** --%>

    <%-- 次作業畫面一：臨時卡借還作業新增與編輯 --%>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="300px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="<%$ Resources:lblTCardCreate %>" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton1" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table style="width: 290px">
                        <tr>
                            <td>
                                <span class="Arrow01"></span>
                            </td>
                            <th>
                                <asp:Label ID="popLabel_CardNo" runat="server" Text="<%$ Resources:popLabel_CardNo %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:TextBox ID="popInput_CardNo" runat="server" Width="90%" CssClass="TextBoxRequired" MaxLength="20"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="Arrow01"></span>
                            </td>
                            <th>
                                <asp:Label ID="popLabel_Area" runat="server" Text="廠區" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:DropDownList ID="ddlCardArea" runat="server" cssClass="DropDownListStyle" BackColor="#FFE5E5"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="Arrow01"></span>
                            </td>
                            <th>
                                <asp:Label ID="popLabel_CardDesc" runat="server" Text="<%$ Resources:popLabel_CardDesc %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:TextBox ID="popInput_CardDesc" runat="server" Width="90%" MaxLength="50"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <th style="text-align: center" colspan="2">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label></th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" CssClass="IconSave" />
                        <asp:Button ID="popB_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconSave" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
