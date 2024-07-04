<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="OrgStruc2.aspx.cs" Inherits="SahoAcs.OrgStruc2" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <table style="width: 850px">
        <tr>
            <td>
                <table class="Item">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblOrgStrucNo" runat="server" Text="<%$ Resources:lblOrgStrucNo %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblOrgNoList" runat="server" Text="<%$ Resources:lblOrgNoList %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblOrgNameList" runat="server" Text="<%$ Resources:lblOrgNameList %>"></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="Input_No" runat="server"></asp:TextBox></td>
                        <td>
                            <asp:TextBox ID="Input_OrgNo" runat="server"></asp:TextBox></td>
                        <td>
                            <asp:TextBox ID="Input_OrgName" runat="server"></asp:TextBox></td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="TableS1">
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                            DataKeyNames="OrgStrucNo" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="OrgStrucID" HeaderText="<%$ Resources:OrgStrucID %>" SortExpression="OrgStrucID" Visible="false" />
                                                <asp:BoundField DataField="OrgStrucNo" HeaderText="<%$ Resources:OrgStrucNo %>" SortExpression="OrgStrucNo" />
                                                <asp:BoundField DataField="OrgNoList" HeaderText="<%$ Resources:OrgNoList %>" SortExpression="OrgNoList" />
                                                <asp:BoundField DataField="OrgNameList" HeaderText="<%$ Resources:OrgNameList %>" SortExpression="OrgNameList" />
                                                <asp:BoundField DataField="CreateUserID" HeaderText="<%$ Resources:CreateUserID %>" SortExpression="CreateUserID" />
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
                <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
                <asp:HiddenField ID="SelectNowNo" runat="server" EnableViewState="False" />
                <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />
                <asp:HiddenField ID="hSelectState" runat="server" EnableViewState="False" />
                <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
            </td>
        </tr>
        <tr>
            <td>
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
                        <td>
                            <asp:Button ID="AuthEquGrButton" runat="server" Text="<%$ Resources:Resource, btnAuthEquGroup %>" CssClass="IconGroupSet" />
                        </td>
                        <td>
                            <asp:Button ID="AuthButton" runat="server" Text="<%$ Resources:Resource, btnAuth %>" CssClass="IconManageList" />
                        </td>
                        <td>
                            <asp:Button ID="EquListButton" runat="server" Text="<%$ Resources:Resource, btnEquList %>" CssClass="IconManageList" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="660px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton1" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th colspan="4"><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No" runat="server" Text="<%$ Resources:OrgStrucNo %>" Font-Bold="True"></asp:Label></th>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <asp:TextBox ID="popInput_No" runat="server" Width="615px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="4"><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OrgList" runat="server" Text="<%$ Resources:lblOrgNoList %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <asp:DropDownList ID="popB_ddlClass" runat="server" Width="625px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_OrgList1" runat="server" Width="270px" Height="150px" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                            <td colspan="2" style="text-align:center">
                                <asp:Button ID="popB_Enter" runat="server" Text="<%$ Resources:Resource, btnJoin %>" CssClass="IconRight" />
                                <br />
                                <br />
                                <asp:Button ID="popB_Remove" runat="server" Text="<%$ Resources:Resource, btnRemove %>" CssClass="IconLeft" />
                            </td>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_OrgList2" runat="server" Width="270px" Height="150px" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="padding-top: 10px">
                                <asp:TextBox ID="popB_OrgListStr" runat="server" Width="615px" Enabled="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="text-align:center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="auto-style1">
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

    <asp:Panel ID="PanelPopup2" runat="server" SkinID="PanelPopup" Width="430px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag2" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName2" runat="server" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton2" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No2" runat="server" Text="<%$ Resources:OrgStrucNo %>" Font-Bold="True"></asp:Label></th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_No2" runat="server" Width="385px" CssClass="TextBoxRequired" Enabled="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OrgList3" runat="server" Text="<%$ Resources:OrgNoList %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_OrgList3" runat="server" Width="390px" Height="150px" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popB_OrgListStr2" runat="server" Width="385px" Enabled="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Char" runat="server" Text="<%$ Resources:lblTransferObject %>" Font-Bold="True"></asp:Label></th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="popDropDownList_Char" Width="395px" runat="server"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <th style="text-align:center">
                                <asp:Label ID="DeleteLableText2" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit2" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Delete" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                        <asp:Button ID="popB_Cancel2" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender2" runat="server"></cc1:ModalPopupExtender>

    <asp:Panel ID="PanelPopup3" runat="server" SkinID="PanelPopup" Width="420px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag3" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName3" runat="server" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton3" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table style="width: 100%">
                        <tr>
                            <th><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No3" runat="server" Text="<%$ Resources:OrgStrucNo %>" Font-Bold="True"></asp:Label></th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_No3" runat="server" Width="380px" CssClass="TextBoxRequired" Enabled="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OrgList4" runat="server" Text="<%$ Resources:NoList %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_OrgList4" runat="server" Width="385px" Height="150px" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <th style="text-align:center">
                                <asp:Label ID="DeleteLableText3" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger3" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger3" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender3" runat="server"></cc1:ModalPopupExtender>

    <asp:Panel ID="PanelPopup4" runat="server" SkinID="PanelPopup" Width="660px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag4" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName4" runat="server" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton4" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table style="width: 100%">
                        <tr>
                            <th colspan="4"><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No4" runat="server" Text="<%$ Resources:OrgStrucNo %>" Font-Bold="True"></asp:Label></th>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <asp:TextBox ID="popInput_No4" runat="server" Width="620px" CssClass="TextBoxRequired" Enabled="False"></asp:TextBox>
                            </td>
                            <td colspan="2"></td>
                        </tr>
                        <tr>
                            <th colspan="4"><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EquGrList" runat="server" Text="<%$ Resources:lblEquGroupList %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_EquGrList1" runat="server" Width="270px" Height="150px" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                            <td colspan="2" style="text-align:center">
                                <asp:Button ID="popB_Enter2" runat="server"  Text="<%$ Resources:Resource, btnJoin%>" CssClass="IconRight" />
                                <br />
                                <br />
                                <asp:Button ID="popB_Remove2" runat="server"  Text="<%$ Resources:Resource, btnRemove%>" CssClass="IconLeft" />
                            </td>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_EquGrList2" runat="server" Width="270px" Height="150px" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="text-align:center">
                                <asp:Label ID="DeleteLableText4" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit4" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Auth2" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Cancel4" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger4" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger4" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender4" runat="server"></cc1:ModalPopupExtender>
</asp:Content>

