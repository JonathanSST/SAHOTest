<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DeviceConnInfo.aspx.cs" Inherits="SahoAcs.DeviceConnInfo" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    <table style="width: 100%">
        <tr>
            <td>
                <table class="Item">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_No" runat="server" Text="<%$Resources:ttConnNo %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_Name" runat="server" Text="<%$Resources:ttConnName %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_IsAssign" runat="server" Text="<%$Resources:ttLockedIP %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_Ip" runat="server" Text="<%$Resources:ttIP %>"></asp:Label>
                        </th>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="Input_No" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_Name" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList ID="Input_IsAssign" runat="server" Width="100px">
                                <asp:ListItem Value="1" Enabled="true" Text="<%$Resources:ddlEnabled %>"></asp:ListItem>
                                <asp:ListItem Value="0" Text="<%$Resources:ddlDisabled %>"></asp:ListItem>
                            </asp:DropDownList></td>
                        <td>
                            <asp:TextBox ID="Input_Ip" runat="server"></asp:TextBox>
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
                                            DataKeyNames="DciNo" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="DciNo" HeaderText="<%$Resources:ttConnNo %>" SortExpression="DciNo" />
                                                <asp:BoundField DataField="DciName" HeaderText="<%$Resources:ttConnName %>" SortExpression="DciName" />
                                                <asp:BoundField DataField="IsAssignIP" HeaderText="<%$Resources:ttLockedIP %>" SortExpression="IsAssignIP" />
                                                <asp:BoundField DataField="IpAddress" HeaderText="<%$Resources:ttIP %>" SortExpression="IpAddress" />
                                                <asp:BoundField DataField="TcpPort" HeaderText="<%$Resources:ttPort %>" SortExpression="TcpPort" />
                                                <asp:BoundField DataField="DciPassWD" HeaderText="<%$Resources:ttConnPW %>" SortExpression="DciPassWD" />
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
                            <asp:Button ID="DeleteButton" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="320px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="設備連線資料" Font-Bold="True" ForeColor="White"
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
                <td style="width:300px">
                    <table style="width:100%">
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No" runat="server" Text="<%$Resources:ttConnNo %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_No" runat="server" Width="95%" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="<%$Resources:ttConnName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_Name" runat="server" Width="95%" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_PassWD" runat="server" Text="<%$Resources:ttConnPW %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_PassWD" runat="server" Width="95%" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Ip" runat="server" Text="<%$Resources:ttIP %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_Ip" runat="server" Width="95%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Port" runat="server" Text="<%$Resources:ttPort %>" Font-Bold="True"></asp:Label>
                                <%-- <cc1:MaskedEditValidator ID="MaskedEditValidator2" runat="server"
                                    ControlExtender="MaskedEditExtender2" ControlToValidate="popInput_Port"
                                    MaximumValue="65534" MinimumValue="0" TooltipMessage="設定值為1001~65534"
                                    Font-Size="12px" ForeColor="SteelBlue">
                                </cc1:MaskedEditValidator>--%>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_Port" runat="server" Width="95%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_IsAssign" runat="server" Text="<%$Resources:ttLockedIP %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align:left">
                                <asp:DropDownList ID="popInput_IsAssign" runat="server" width="95%">
                                    <asp:ListItem Value="1" Enabled="true" Text="<%$Resources:ddlEnabled %>"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="<%$Resources:ddlDisabled %>"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th colspan="3" style="text-align:center">
                    <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Add" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Edit" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Delete" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                        <asp:Button ID="popB_Cancel" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
