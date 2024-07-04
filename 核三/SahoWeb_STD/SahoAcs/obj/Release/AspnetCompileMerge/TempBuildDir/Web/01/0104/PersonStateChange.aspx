<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PersonStateChange.aspx.cs" Inherits="SahoAcs.Web.PersonStateChange" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Src="/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <table>
        <tr>
            <td>
                <table class="Item">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblDataList" runat="server" Text="<%$ Resources:lblDataList %>"></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ListBox ID="DataList" runat="server" Height="410px" Width="320px" SkinID="ListBoxSkin"></asp:ListBox>
                        </td>
                        <td style="vertical-align: top;">
                            <fieldset id="Pending_List" runat="server" style="height: 400px; margin-left: 10px">
                                <legend id="Pending_Legend" runat="server">
                                    <asp:Label ID="lblAuthStateAdj" runat="server" Text="<%$ Resources:lblAuthStateAdj %>"></asp:Label>
                                </legend>
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblAdjClass" runat="server" Text="<%$ Resources:lblAdjClass %>"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:DropDownList ID="ddlType" Width="170px" runat="server">
                                                        <asp:ListItem Value="0" Text="<%$Resources:SelectDisabled %>"></asp:ListItem>
                                                        <asp:ListItem Value="1" Text="<%$Resources:SelectEnabled %>"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btExec" runat="server" Text="<%$ Resources:btnExec %>" CssClass="IconChange" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 20px">
                                                    <asp:Label ID="lblMsg" runat="server" Text="<%$ Resources:lblMsg %>"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:ListBox ID="List_Msg" runat="server" Width="840px" Height="200px" SkinID="ListBoxSkin"></asp:ListBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btSelectData" runat="server" Text="<%$ Resources:btnSelectData %>" CssClass="IconChoose" />
                            <%--<input type="button" id="btSelectOrgData" value="組織資料選取" class="IconChoose" />--%>
                        </td>
                        <td></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="580px"
        EnableViewState="False" CssClass="PopBg">
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
                    <table class="Item">
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="lblKeyword" runat="server" Text="<%$ Resources:lblKeyword %>"></asp:Label><br />                                
                            </th>
                        </tr>
                        <tr>
                            <td colspan="4" style="text-align:left">
                                <asp:TextBox ID="Input_TxtQuery" runat="server" Width="250px"></asp:TextBox>
                                <asp:Button ID="popB_Query" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" /><br />
                                <asp:Label ID="lblTip" runat="server" Text="<%$ Resources:lblTip %>" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:popStatus %>"></asp:Label><br />
                                <asp:DropDownList ID="popddltype" Width="170px" runat="server">
                                    <asp:ListItem Value="0" Text="<%$Resources:SelectDisabled %>"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="<%$Resources:SelectEnabled %>"></asp:ListItem>
                                </asp:DropDownList>
                            </th>
                        </tr>
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OrgList" runat="server" Text="<%$ Resources:popLabel_OrgList %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_PsnList1" runat="server" Width="200px" Height="150px" SelectionMode="Multiple" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                            <td colspan="2" style="text-align:center">
                                <asp:Button ID="popB_Enter1" runat="server" Text="<%$ Resources:Resource, btnJoin%>" CssClass="IconRight" />
                                <br />
                                <br />
                                <asp:Button ID="popB_Remove1" runat="server" Text="<%$ Resources:Resource, btnRemove%>" CssClass="IconLeft" />
                            </td>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_PsnList2" runat="server" Width="200px" Height="150px" SelectionMode="Multiple" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="text-align:center">
                                <asp:Label ID="DeleteLableText1" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_OK1" runat="server" Text="<%$ Resources:Resource, btnOK %>" EnableViewState="False" CssClass="IconOk" />
                        <asp:Button ID="popB_Cancel1" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
    <style>
        #ContentPlaceHolder1_Label1{
            color:white
        }
    </style>
</asp:Content>
