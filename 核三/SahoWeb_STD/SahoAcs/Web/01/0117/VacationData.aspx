﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="VacationData.aspx.cs" Inherits="SahoAcs.VacationData" Debug="true" EnableEventValidation="false" Theme="UI" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <style type="text/css">
        .style1{
            color:black;
            background-color:white;
            position:absolute;
            top:10px;
            left:10px;
        }
    </style>
    <table class="Item">
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1" style="width: 860px">
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5" DataKeyNames="VID" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="False" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="VID" HeaderText="<%$ Resources:VID %>" SortExpression="VID" />
                                    <asp:BoundField DataField="VNo" HeaderText="<%$ Resources:VNo %>" SortExpression="VNo" />
                                    <asp:BoundField DataField="VName" HeaderText="<%$ Resources:VName %>" SortExpression="VName" />
                                    <asp:BoundField DataField="CreateUserID" HeaderText="<%$ Resources:CreateUserID %>" SortExpression="CreateUserID" />
                                    <asp:BoundField DataField="CreateTime" HeaderText="<%$ Resources:CreateTime %>" SortExpression="CreateTime" />
                                    <asp:BoundField DataField="UpdateUserID" HeaderText="<%$ Resources:UpdateUserID %>" SortExpression="UpdateUserID" />
                                    <asp:BoundField DataField="UpdateTime" HeaderText="<%$ Resources:UpdateTime %>" SortExpression="UpdateTime" />
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
    <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectYear" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hSelectState" runat="server" EnableViewState="False" />  
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectNowNo" runat="server" EnableViewState="False" />
    <!--input type="hidden" id="ddlSelectDefault" value="<!--%=GetGlobalResourceObject("Resource","ddlSelectDefault") %>" /-->
    <table>
        <tr>
            <td>
                <asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />
            </td>
            <td>
                <asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit" />
            </td>
            <td>
                <asp:Button ID="DeleteButton" runat="server" Text="<%$ Resources:Resource, btnDelete %>" CssClass="IconDelete" />
            </td>
        </tr>
    </table>

    <!-- ************************************************** PanelPopup1 ************************************************** -->

    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="430px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
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
                    <table>
                        <tr>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_VNo" runat="server" Text="<%$ Resources:VNo %>" Font-Bold="True"></asp:Label>
                            </th>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_VName" runat="server" Text="<%$ Resources:VName %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_VNo" runat="server" Width="185px" BorderWidth="1" CssClass="TextBoxRequired" MaxLength="2"></asp:TextBox>
                            </td>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_VName" runat="server" Width="185px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="popItem">
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave %>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave %>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>

    <!-- ************************************************** PanelPopup2 ************************************************** -->

    <asp:Panel ID="PanelPopup2" runat="server" SkinID="PanelPopup" Width="430px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag2" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName2" runat="server" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton2" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLable_VNo2" runat="server" Text="<%$ Resources:VNo %>" Font-Bold="True"></asp:Label>
                            </th>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLable_VName2" runat="server" Text="<%$ Resources:VName %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_VNo2" runat="server" Width="185px" BorderWidth="1" CssClass="TextBoxRequired" MaxLength="15"></asp:TextBox>
                            </td>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_VName2" runat="server" Width="185px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText2" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="popItem">
                    <asp:Panel ID="PanelEdit2" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete %>" EnableViewState="False" CssClass="IconDelete" />
                        <asp:Button ID="popB_Cancel2" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender2" runat="server"></cc1:ModalPopupExtender>
</asp:Content>