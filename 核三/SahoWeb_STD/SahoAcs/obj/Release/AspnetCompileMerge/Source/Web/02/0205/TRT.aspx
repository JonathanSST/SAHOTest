<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="TRT.aspx.cs" Inherits="SahoAcs.TRT" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    <table class="TableWidth">
        <tr>
            <td>
                <table class="Item">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_Building" runat="server" Text="<%$Resources:ResourceCtrls,Build %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_EquNo" runat="server" Text="<%$Resources:ResourceCtrls,EquNo %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_EquName" runat="server" Text="<%$Resources:ResourceCtrls,EquName %>"></asp:Label>
                        </th>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="Input_Building" runat="server" Width="120px" Font-Size="13px">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_EquNo" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_EquName" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="TableS1">
                            <%--GridView Header的Html Code--%>
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                            DataKeyNames="EquNo" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="Floor" HeaderText="<%$Resources:Resource,Floor %>" SortExpression="Floor" />
                                                <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:Resource,EquNo %>" SortExpression="EquNo" />
                                                <asp:BoundField DataField="EquName" HeaderText="<%$Resources:Resource,EquName %>" SortExpression="EquName" />
                                                <asp:BoundField DataField="EquModel" HeaderText="<%$Resources:ResourceCtrls,EquModel %>" SortExpression="EquModel" />
                                                <asp:BoundField DataField="Dci" HeaderText="<%$ Resources:Resource,MasterConn %>" SortExpression="Dci" />
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
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />
                        </td>
                        <td>
                            <asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit" />
                        </td>
                        <td>
                            <asp:Button ID="DeleteButton" runat="server" Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete" />
                        </td>
                        <td>
                            <asp:Button ID="ParaButton" runat="server" Text="<%$ Resources:Resource, ParaSetting %>" CssClass="IconSet" />
                        </td>
                        <td>
                            <asp:Button ID="SetButton" runat="server" Text="<%$ Resources:Resource, CodeSetting %>" OnClick="SetButton_Click" CssClass="IconPassword" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="380px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="設備參數定義資料" Font-Bold="True" ForeColor="White"
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
                            <th>
                                <asp:Label ID="popLabel_Building" runat="server" Text="建築物名稱" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_Floor" runat="server" Text="樓層" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_Building" runat="server" Width="180px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_Floor" runat="server" Width="130px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <asp:Label ID="popLabel_EquModel" runat="server" Text="設備型號" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_EquNo" runat="server" Text="設備編號" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="popInput_EquModel" runat="server" OnInit="popInput_EquModel_Init" Width="115px" BackColor="#FFE5E5"></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_EquNo" runat="server" Width="200px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <asp:Label ID="popLabel_EquName" runat="server" Text="設備名稱" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_EquEName" runat="server" Text="設備英文名稱" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_EquName" runat="server" Width="155px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_EquEName" runat="server" Width="155px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <asp:Label ID="popLabel_Dci" runat="server" Text="設備連線" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_CardNoLen" runat="server" Text="卡號長度" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="popInput_Dci" runat="server" OnInit="popInput_Dci_Init" Width="253px" BackColor="#FFE5E5"></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_CardNoLen" runat="server" Width="60px" CssClass="TextBoxRequired"></asp:TextBox>                                
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                顯示姓名
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="popIsShowName" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th style="text-align: center">
                    <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                </th>
            </tr>
            <tr>
                <td style="text-align: center">
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
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
