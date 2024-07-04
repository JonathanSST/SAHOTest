<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="LocationSetting.aspx.cs" Inherits="SahoAcs.LocationSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="/Css/colorbox.css" rel="stylesheet" />
    <link href="/Css/jquery-ui.css" rel="stylesheet" type="text/css" />

    <script src="/Scripts/jquery.colorbox-min.js"></script>
    <script src="/Scripts/jquery.blockUI.js"></script>

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>


    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" />
    </div>

    <div runat="server" id="RightClickmenu" class="RightClickmenuskin" onmouseover="highlightMenu()" onmouseout="lowlightMenu()">
        <div id="new_childItem" class="menuitems" onclick="clickNewMenu();">
        </div>
        <div id="del_childItem" class="menuitems" onclick="clickdelMenu();">
        </div>
    </div>

    <div id="dialog" title=""></div>
    <div id="dialog1" title=""></div>

    <table>
        <tr>
            <td style="vertical-align: top; width: 300px">
                <asp:UpdatePanel ID="TreeView_UpdatePanel" runat="server" UpdateMode="Conditional" style="height: 430px">
                    <ContentTemplate>
                        <asp:Panel ID="TreeView_Panel" runat="server" ScrollBars="Vertical" Height="100%" class="TableS1">
                            <asp:TreeView ID="Location_TreeView" runat="server" Width="100%"></asp:TreeView>
                            <asp:HiddenField ID="txt_NodeTypeList" runat="server" />
                            <asp:HiddenField ID="txt_NodeIDList" runat="server" />
                            <asp:HiddenField ID="NodeAct" runat="server" />
                            <asp:HiddenField ID="RightClickItem" runat="server" />
                            <%--<asp:HiddenField ID="ClickItem" runat="server" />--%>
                           <%-- <asp:HiddenField ID="tmpEquClass" runat="server" />
                            <asp:HiddenField ID="DciIDValue" runat="server" />
                            <asp:HiddenField ID="MstIDValue" runat="server" />
                            <asp:HiddenField ID="hfEquID" runat="server" />
                            <asp:HiddenField ID="CtrlIDValue" runat="server" />
                            <asp:HiddenField ID="hfReaderID" runat="server" />--%>
                            <asp:HiddenField ID="event_srcElement_id" runat="server" />
                            <asp:HiddenField ID="TreeView_Panel_scrollTop" runat="server" />
                           <%-- <asp:HiddenField ID="ControlCount" runat="server" />--%>
                          <%--  <asp:HiddenField ID="hfDciID" runat="server" />--%>
                            <asp:HiddenField ID="hfLocID" runat="server" />
                            <asp:HiddenField ID="hfLocType" runat="server" />
                            <asp:HiddenField ID="LocIDValue" runat="server" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>

            <td style="vertical-align: top">
                <div runat="server" id="Div_Loc">
                    <table style="margin-left: 15px; width: 98%">
                        <%--區域介面--%>
                        <tr>
                            <td id="LocUI_Title" style="text-align: center; width: 100%" class="title01">
                                <div runat="server" id="Div_AreaUI_Title">
                                    <asp:Label ID="Loc_Label_Title" runat="server" Text="顯示介面" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 10px 0 15px 0; text-align: center" class="Bg01">
                                <div runat="server" id="LocUI">
                                    <table class="TableWidth">
                                        <tr>
                                            <td>
                                                <table class="Item">
                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th colspan="2">
                                                            <asp:Label ID="popLab_No" runat="server" Text="編號 " Font-Bold="true"></asp:Label>
                                                        </th>
                                                        <th style="width: 100px"></th>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th colspan="2">
                                                            <asp:Label ID="popLab_Name" runat="server" Text="名稱" Font-Bold="true"></asp:Label>
                                                        </th>
                                                        <th style="width: 10px"></th>
                                                    </tr>

                                                    <tr>
                                                        <td></td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="Loc_Input_No" runat="server" Width="98%" CssClass="TextBoxRequired" MaxLength="20" EnableViewState="False"></asp:TextBox></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="Loc_Input_Name" runat="server" Width="98%" CssClass="TextBoxRequired" MaxLength="50" EnableViewState="False"></asp:TextBox></td>
                                                        <td></td>
                                                    </tr>

                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th colspan="7">
                                                            <asp:Label ID="popLab_Type" runat="server" Text="類型" Font-Bold="true"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <td></td>
                                                        <td colspan="7" style="text-align: left">
                                                            <asp:TextBox ID="Loc_Input_Type" runat="server" Width="98%" CssClass="TextBoxRequired" MaxLength="50" EnableViewState="False" Enabled="False"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th colspan="7">
                                                            <asp:Label ID="popLab_PList" runat="server" Text="上層位置" Font-Bold="true"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <td></td>
                                                        <td colspan="7" style="text-align: left">
                                                            <asp:DropDownList ID="Loc_Input_PList" runat="server" BackColor="#FFE5E5" Font-Bold="True" Width="98%"></asp:DropDownList>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th colspan="7">
                                                            <asp:Label ID="popLab_Map" runat="server" Text="Map圖號" Font-Bold="true"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <td></td>
                                                        <td colspan="7" style="text-align: left">
                                                            <asp:TextBox ID="Loc_Input_Map"  runat="server" Width="98%"></asp:TextBox>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th colspan="7">
                                                            <asp:Label ID="popLab_Desc" runat="server" Text="說明" Font-Bold="true"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <td></td>
                                                        <td colspan="7" style="text-align: left">
                                                            <asp:TextBox ID="Loc_Input_Desc" TextMode="MultiLine" Rows="4" runat="server" Width="98%"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td></td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: center">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Button ID="Loc_B_Add" runat="server" Text="新增" EnableViewState="False" CssClass="IconSave" />
                                                            <asp:Button ID="Loc_B_Edit" runat="server" Text="儲存" EnableViewState="False" CssClass="IconSave" />
                                                            <asp:Button ID="Loc_B_Delete" runat="server" Text="刪除 " EnableViewState="False" CssClass="IconDelete" />
                                                            <asp:Button ID="Loc_B_Cancel" runat="server" Text="取消" EnableViewState="False" CssClass="IconCancel" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="260px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%; padding: 0px;">
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
                <th colspan="2">
                    <asp:Label ID="lblKeyWord" runat="server" Text="關鍵字："></asp:Label>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtKeyWord" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="搜尋" EnableViewState="False" CssClass="IconSearch" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>

</asp:Content>


