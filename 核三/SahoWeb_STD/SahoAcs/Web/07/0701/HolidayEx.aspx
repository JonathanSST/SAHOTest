<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="HolidayEx.aspx.cs" Inherits="SahoAcs._0701.HolidayEx" Debug="true" EnableEventValidation="false" Theme="UI" %>

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
        <tr>
            <th style="text-align:left">
                <asp:Label ID="lblYear" runat="server" Text="<%$ Resources:lblCondition %>"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td style="width: 120px">
                <asp:DropDownList ID="Input_Year" runat="server" Width="90%"></asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" />
                <asp:Button ID="BulidButton" runat="server" Text="<%$ Resources:btnBulid %>" CssClass="IconNew" />
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1" style="width: 700px">
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                DataKeyNames="HEID" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="False" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="HEID" HeaderText="<%$ Resources:HEID %>" SortExpression="HEID" />
                                    <asp:BoundField DataField="HEDate" HeaderText="<%$ Resources:HEDate %>" SortExpression="HEDate" />
                                    <asp:BoundField DataField="HEDesc" HeaderText="<%$ Resources:HEDesc %>" SortExpression="HEDesc" />
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
    <asp:HiddenField ID="SelectYear" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hSelectState" runat="server" EnableViewState="False" />  
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />  
    <input type="hidden" id="ddlSelectDefault" value="<%=GetGlobalResourceObject("Resource","ddlSelectDefault") %>" />
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
    
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="430px"
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
                    <table>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Year" runat="server" Text="<%$ Resources:lblCondition_Add %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="popLabel_YYYY" runat="server" Text="yyyy" Font-Size="X-Large" ForeColor="Black" Font-Bold="True"></asp:Label>
                                <asp:Label ID="Label1" runat="server" Text="年" Font-Bold="True"></asp:Label>
                                <asp:DropDownList ID="popInput_MM" runat="server" Width="70px"></asp:DropDownList>
                                <asp:Label ID="Label2" runat="server" Text="月" Font-Bold="True"></asp:Label>
                                <asp:DropDownList ID="popInput_DD" runat="server" Width="70px"></asp:DropDownList>
                                <asp:Label ID="Label3" runat="server" Text="日" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_HEDesc" runat="server" Text="<%$ Resources:HEDesc %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_HEDesc" runat="server" Width="390px"></asp:TextBox>
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
        <table border="0" class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_YMD" runat="server" Text="<%$ Resources:HEDate %>" Font-Bold="True"></asp:Label></th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_YMD" runat="server" Width="390px" CssClass="TextBoxRequired" MaxLength="15"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Desc" runat="server" Text="<%$ Resources:HEDesc %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2" class="auto-style1">
                                <asp:TextBox ID="popInput_Desc" runat="server" Width="390px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText2" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
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