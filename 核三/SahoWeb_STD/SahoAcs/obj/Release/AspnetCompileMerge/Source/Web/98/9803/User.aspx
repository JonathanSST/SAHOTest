<%@ Page Language="C#" CodeBehind="User.aspx.cs" Inherits="SahoAcs.User" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%-- cellspacing="0" border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死 --%>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <%-- 主作業畫面一：查詢部份 --%>
    <table class="Item">
        <tr>
            <th><span class="Arrow01"></span><asp:Label ID="Label_ID" runat="server" Text="<%$Resources:ttUserId %>"></asp:Label></th>
            <th><span class="Arrow01"></span><asp:Label ID="Label_Name" runat="server" Text="<%$Resources:ttUserName %>"></asp:Label></th>
            <th><span class="Arrow01"></span><asp:Label ID="Label_IsOptAllow" runat="server" Text="<%$Resources:ttOptAllow %>"></asp:Label></th>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="Input_ID" runat="server"></asp:TextBox></td>
            <td><asp:TextBox ID="Input_Name" runat="server"></asp:TextBox></td>
            <td>
                <asp:DropDownList ID="Input_IsOptAllow" runat="server" Width="100px">
                    <asp:ListItem Value="1" Enabled="true" Text="<%$Resources:ttStateOpen %>"></asp:ListItem>
                    <asp:ListItem Value="0" Text="<%$Resources:ttStateClose %>"></asp:ListItem>
                </asp:DropDownList></td>
            <td><asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" /></td>
        </tr>
    </table>

    <%-- 主作業畫面二：表格部份 --%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="MaxUser" runat="server" EnableViewState="False" />
            <asp:HiddenField ID="CurrentUser" runat="server" EnableViewState="False" />
            <table cellspacing="0" class="TableS1">
                <%-- GridView Header的Html Code --%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td ID="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight" Width="1450px">
                            <%-- <asp:HiddenField ID="SelectValue" runat="server" Value="" /> --%>
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="10"
                                DataKeyNames="UserID" AllowPaging="True" AutoGenerateColumns="False"
                                OnDataBound="GridView_Data_DataBound" OnRowDataBound="GridView_Data_RowDataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="UserID" HeaderText="<%$Resources:ttUserId %>" SortExpression="UserID" />
                                    <asp:BoundField DataField="UserName" HeaderText="<%$Resources:ttUserName %>" SortExpression="UserName" />
                                    <asp:BoundField DataField="UserEName" HeaderText="<%$Resources:ttUserEName %>" SortExpression="UserEName" />
                                    <asp:BoundField DataField="EMail" HeaderText="<%$Resources:ttUserEmail %>" SortExpression="EMail" />
                                    <asp:BoundField DataField="IsOptAllow" HeaderText="<%$Resources:ttOptAllow %>" SortExpression="IsOptAllow" />
                                    <asp:BoundField DataField="UserSTime" HeaderText="<%$Resources:ttUserSTime %>" SortExpression="UserSTime" />
                                    <asp:BoundField DataField="UserETime" HeaderText="<%$Resources:ttUserETime %>" SortExpression="UserETime" />
                                    <asp:BoundField DataField="PWChgTime" HeaderText="<%$Resources:ttPWChgTime %>" SortExpression="PWChgTime" />
                                    <asp:BoundField DataField="UserPWCtrl" HeaderText="<%$Resources:ttUserPWCtrl %>" SortExpression="UserPWCtrl" />
                                    <asp:BoundField DataField="Remark" HeaderText="<%$Resources:ttRemark %>" SortExpression="Remark" />
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
    <asp:HiddenField ID="hOwnerList" runat="server" EnableViewState="False"  />
    <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectNowName" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <input type="hidden" id="SysUserID" name="SysUserID" value="<%=Sa.Web.Fun.GetSessionStr(this,"UserID")%>" />
    <%-- 主作業畫面三：按鈕部份 --%>
    <table>
        <tr>
            <td><asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew"/></td>
            <td><asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit"/></td>
            <td><asp:Button ID="DeleteButton" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete"/></td>
            <td><asp:Button ID="MenuButton" runat="server" Text="<%$Resources:btnAuth %>" CssClass="IconSet"/></td>
            <td><asp:Button ID="RoleButton" runat="server" Text="<%$Resources:btnUserRole %>" CssClass="IconSetRoles"/></td>
            <td><asp:Button ID="MgnaButton" runat="server" Text="<%$Resources:btnMgnArea %>" CssClass="IconSetManage"/></td>
        </tr>
    </table>

    <%-- 次作業畫面一：使用者資料新增與編輯 --%>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="430px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="使用者資料" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
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
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ID" runat="server" Text="<%$Resources:ttUserId %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_PW" runat="server" Text="<%$Resources:ttUserPWD %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_ID" runat="server" Width="180px" BorderWidth="1" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_PW" runat="server" Width="180px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="<%$Resources:ttUserName %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_IsOptAllow" runat="server" Text="<%$Resources:ttOptAllow %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_Name" runat="server" Width="180px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:DropDownList ID="popInput_IsOptAllow" runat="server" Width="184px">
                                    <asp:ListItem Value="1" Enabled="true" Text="<%$Resources:ttStateOpen %>"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="<%$Resources:ttStateClose %>"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EName" runat="server" Text="<%$Resources:ttUserEName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_EName" runat="server" Width="367px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EMail" runat="server" Text="<%$Resources:ttUserEmail %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_EMail" runat="server" Width="367px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_STime" runat="server" Text="<%$Resources:ttUserSTime %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ETime" runat="server" Text="<%$Resources:ttUserETime %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <uc1:Calendar runat="server" ID="popInput_STime" />
                            </td>
                            <td>
                                <uc1:Calendar runat="server" ID="popInput_ETime" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_PWChgTime" runat="server" Text="<%$Resources:ttPWChgTime %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_PWCtrl" runat="server" Text="<%$Resources:ttUserPWCtrl %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_PWChgTime" runat="server" Width="180px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_PWCtrl" runat="server" Width="180px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Remark" runat="server" Text="<%$Resources:ttRemark %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_Remark" runat="server" TextMode="MultiLine" Rows="4" Width="367px" Style="resize: none;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2" style="text-align:center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Add" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave"/>
                        <asp:Button ID="popB_Edit" runat="server"  Text="<%$ Resources:Resource, btnSave%>" CssClass="IconSave"/>
                        <asp:Button ID="popB_Delete" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete"/>
                        <asp:Button ID="popB_Cancel" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel"/>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>

    <%-- 次作業畫面二：使用者功能清單權限設定 --%>
    <asp:Panel ID="PanelPopup2" runat="server" SkinID="PanelPopup" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag2" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName2" runat="server" Text="使用者功能清單權限設定" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
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
                    <asp:Table ID="popUserMenusTableHeader" runat="server" CellSpacing="0" CssClass="Title01"></asp:Table>
                    <asp:Panel ID="popUserMenusAuthPanel" runat="server" ScrollBars="Vertical" Height="400px">
                        <asp:Table ID="popUserMenusAuthTable" runat="server" CellSpacing="0" CssClass="Panel01"></asp:Table>
                    </asp:Panel>
                    <table>
                        <tr>
                            <td style="padding-top:10px">
                                <asp:Button ID="popB_UserMenusAuthAdd" runat="server"  Text="<%$ Resources:Resource, btnSave%>" CssClass="IconSave"/>
                                <asp:Button ID="popB_UserMenusAuthCancel" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" CssClass="IconCancel"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender2" runat="server"></cc1:ModalPopupExtender>

    <%-- 次作業畫面三：使用者角色清單權限設定 --%>
    <asp:Panel ID="PanelPopup3" runat="server" SkinID="PanelPopup" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag3" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName3" runat="server" Text="使用者角色清單權限設定" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton3" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <asp:Table ID="popUserRolesTableHeader" runat="server" CellSpacing="0" CssClass="Title01"></asp:Table>
                    <asp:Panel ID="popUserRolesAuthPanel" runat="server" ScrollBars="Vertical" Height="300px">
                        <asp:Table ID="popUserRolesAuthTable" runat="server" CellSpacing="0" CssClass="Panel01"></asp:Table>
                    </asp:Panel>
                    <table>
                        <tr>
                            <td style="padding-top:10px">
                                <asp:Button ID="popB_UserRolesAuthAdd" runat="server"  Text="<%$ Resources:Resource, btnSave%>" CssClass="IconSave"/>
                                <asp:Button ID="popB_UserRolesAuthCancel" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" CssClass="IconCancel"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger3" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger3" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender3" runat="server"></cc1:ModalPopupExtender>

    <%-- 次作業畫面四：使用者管理區清單權限設定 --%>
    <asp:Panel ID="PanelPopup4" runat="server" SkinID="PanelPopup" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag4" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName4" runat="server" Text="使用者管理區清單權限設定" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton4" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <asp:Table ID="popUserMgnsTableHeader" runat="server" CellSpacing="0" CssClass="Title01"></asp:Table>
                    <asp:Panel ID="popUserMgnsAuthPanel" runat="server" ScrollBars="Vertical" Height="300px">
                        <asp:Table ID="popUserMgnsAuthTable" runat="server" CellSpacing="0" CssClass="Panel01"></asp:Table>
                    </asp:Panel>
                    <table>
                        <tr>
                            <td style="padding-top:10px">
                                <asp:Button ID="popB_UserMgnsAuthAdd" runat="server"  Text="<%$ Resources:Resource, btnSave%>" CssClass="IconSave"/>
                                <asp:Button ID="popB_UserMgnsAuthCancel" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" CssClass="IconCancel"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger4" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger4" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender4" runat="server"></cc1:ModalPopupExtender>
</asp:Content>