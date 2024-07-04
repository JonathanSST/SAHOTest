
<%@ Page Language="C#" CodeBehind="TempCardRecord.aspx.cs" Inherits="SahoAcs.TempCardRecord" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>
<%--<%@ Register Src="/uc/Cal2.ascx" TagPrefix="uc1" TagName="Calendar" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%-- cellspacing="0" border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死 --%>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <%-- 主作業畫面一：查詢部份 --%>
    <table style="width: 100%">
        <tr>
            <td>
                <table class="Item">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                        </th>
                        <th>
                            <asp:Label ID="Label_No" runat="server" Text="<%$ Resources: ttCondition %>"></asp:Label>
                        </th>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:TextBox ID="Input_No" runat="server" Width="450px"></asp:TextBox></td>
                        <td></td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <%-- 主作業畫面二：表格部份 --%>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="TableS1">
                            <%-- GridView Header的Html Code --%>
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" Height="250">
                                        <%-- <asp:HiddenField ID="SelectValue" runat="server" Value="" /> --%>

                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin"
                                            DataKeyNames="RecordID" AllowPaging="True" AutoGenerateColumns="False"
                                              OnDataBound="GridView_Data_DataBound" OnRowDataBound="GridView_Data_RowDataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="RecordID" HeaderText="<%$ Resources:RecordID %>" SortExpression="RecordID" />
                                                <asp:BoundField DataField="PsnNo" HeaderText="<%$ Resources:PsnNo %>" SortExpression="PsnNo" />
                                                <asp:BoundField DataField="PsnName" HeaderText="<%$ Resources:PsnName %>" SortExpression="PsnName" />
                                                <asp:BoundField DataField="OrigCardNo" HeaderText="<%$ Resources:OrigCardNo %>" SortExpression="OrigCardNo" />
                                                <asp:BoundField DataField="CardNo" HeaderText="<%$ Resources:CardNo %>" SortExpression="CardNo" />
                                                <asp:BoundField DataField="BorrowTime" HeaderText="<%$ Resources:BorrowTime %>" SortExpression="BorrowTime" />
                                                <asp:BoundField DataField="ReturnTime" HeaderText="<%$ Resources:ReturnTime %>" SortExpression="ReturnTime" />
                                                <asp:BoundField DataField="TempDesc" HeaderText="<%$ Resources:TempDesc %>" SortExpression="Remark" />
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
                <asp:HiddenField ID="hUserId" runat="server" />
                <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
                <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
                <input type="hidden" id="AlertMsg" value="<%=GetLocalResourceObject("AlertMsg") %>" />
                <%-- 主作業畫面三：按鈕部份 --%>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="AddButton" runat="server" Text="<%$ Resources: btnAdd %>" CssClass="IconNew" />
                        </td>
                        <td>
                            <asp:Button ID="EditButton" runat="server" Text="<%$ Resources: btnEdit %>" CssClass="IconEdit" />
                        </td>
                        <td>
                            <asp:Button ID="DeleteButton" runat="server" Text="<%$ Resources: btnDelete%>" CssClass="IconDelete" Visible="False" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <%-- 次作業畫面一：臨時卡借還作業新增與編輯 --%>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="435px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="<%$ Resources:lblTCardLease %>" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
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
                            <th>
                                <asp:Label ID="popLabel_PsnNo" runat="server" Text="<%$ Resources:popLabel_PsnNo %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_PsnName" runat="server" Text="<%$ Resources:popLabel_PsnName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_PsnNo" runat="server" Width="180px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_PsnName" runat="server" Width="180px"></asp:TextBox>
                                
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <asp:Label ID="popLabel_OrigCardNo" runat="server" Text="<%$ Resources:popLabel_OrigCardNo %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_CardNo" runat="server" Text="<%$ Resources:popLabel_CardNo %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_OrigCardNo" runat="server" Width="180px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_CardNo" runat="server" Width="180px" CssClass="TextBoxRequired"></asp:TextBox><asp:DropDownList ID="ddlCardNo" runat="server" cssClass="DropDownListStyle" BackColor="#FFE5E5"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <asp:Label ID="popLabel_BorrowTime" runat="server" Text="<%$ Resources:popLabel_BorrowTime %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th id="th_popInput_ReturnTime">
                                <asp:Label ID="popLabel_ReturnTime" runat="server" Text="<%$ Resources:popLabel_ReturnTime %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <uc1:Calendar runat="server" ID="popInput_BorrowTime" />
                            </td>
                            <td id="td_popInput_ReturnTime">
                                <uc1:Calendar runat="server" ID="popInput_ReturnTime" />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <asp:Label ID="popLabel_TempDesc" runat="server" Text="<%$ Resources:popLabel_TempDesc %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align:left">
                                <asp:TextBox ID="popInput_TempDesc" runat="server" Width="390px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText" runat="server"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField ID="hRecordID" runat="server" />
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
