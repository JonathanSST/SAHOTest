<%@ Page Language="C#" CodeBehind="QueryCtrlEquRealtimeOnlineState.aspx.cs" Inherits="SahoAcs.QueryCtrlEquRealtimeOnlineState" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="True" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<%-- cellspacing="0" border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死 --%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <%-- ************************************************** 網頁畫面設計一 ************************************************** --%>

    <%-- 主要作業畫面：查詢部份 --%>
    <asp:UpdatePanel ID="UpdatePanel0" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="Item">
                <tr>
                    <th>
                        <span class="Arrow01"></span>
                        <asp:Label ID="QueryLable_Area" runat="server" Text="<%$Resources:ResourceCtrls,lblArea %>"></asp:Label>
                    </th>
                    <th>
                        <span class="Arrow01"></span>
                        <asp:Label ID="QueryLable_Build" runat="server" Text="<%$Resources:ResourceCtrls,Build %>"></asp:Label>
                    </th>
                    <th>
                        <span class="Arrow01"></span>
                        <asp:Label ID="QueryLable_Floor" runat="server" Text="<%$Resources:ResourceCtrls,lblFloorName %>"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <td style="width: 140px">
                        <asp:DropDownList ID="ddlLocArea" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                            onchange="SelectArea()">
                        </asp:DropDownList>
                    </td>
                    <td style="width: 200px">
                        <asp:DropDownList ID="ddlLocBuilding" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                            onchange="SelectBuilding()">
                        </asp:DropDownList>
                    </td>
                    <td style="width: 140px">
                        <asp:DropDownList ID="ddlLocFloor" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
                    </td>
                    <td>
                        <asp:Button ID="ComplexQueryButton" runat="server" Text="<%$Resources:Resource,btnAdvance %>" CssClass="IconMultisearch" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <%-- 主要作業畫面：表格部份 --%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1">
                <%-- GridView Header's HTML Code --%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                            <%-- GridView Body's HTML Code --%>
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" Width="100%" PageSize="10"
                                DataKeyNames="EquID" AllowPaging="True" AutoGenerateColumns="False"
                                OnDataBound="GridView_Data_DataBound" OnRowDataBound="GridView_Data_RowDataBound" OnPageIndexChanging="GridView_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="HeaderCheckState" runat="server"></asp:CheckBox>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="RowCheckState" runat="server"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="EquID" HeaderText="<%$Resources:ResourceCtrls,SerialKey %>" SortExpression="EquID" />
                                    <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:ResourceCtrls,EquNo %>" SortExpression="EquNo" />
                                    <asp:BoundField DataField="EquName" HeaderText="<%$Resources:ResourceCtrls,EquName %>" SortExpression="EquName" />
                                    <asp:BoundField DataField="IsAndTrtName" HeaderText="<%$Resources:ResourceCtrls,Clock %>" SortExpression="IsAndTrtName" />
                                    <asp:BoundField DataField="EquModelName" HeaderText="<%$Resources:ResourceCtrls,EquModel %>" SortExpression="EquModelName" />
                                    <asp:BoundField DataField="EquClass" HeaderText="<%$Resources:ResourceCtrls,EquType %>" SortExpression="EquClass" />
                                    <asp:BoundField DataField="LocLongName" HeaderText="<%$Resources:ResourceCtrls,lblLocation %>" SortExpression="LocLongName" />
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label ID="HeaderSubmitState" runat="server" Text="<%$Resources:ResourceCtrls,State %>" Font-Bold="true"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="RowSubmitState" runat="server" Text="" Font-Bold="true"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label ID="HeaderResponseState" runat="server" Text="<%$Resources:ResourceCtrls,Result %>" Font-Bold="true"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="RowResponseState" runat="server" Text="" Font-Bold="true"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
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
                <%-- GridView Pager's HTML Code --%>
                <asp:Literal ID="li_Pager" runat="server" />
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <%-- 主要作業畫面：按鈕部份 --%>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <table cellspacing="5" style="padding-top: 10px">
                <tr>
                    <td>
                        <asp:Button ID="ViewButton" runat="server" Text="<%$Resources:ResourceCtrls,btnViewer %>" CssClass="IconLook" />
                    </td>
                    <td>
                        <asp:Button ID="SubmitButton" runat="server" Text="<%$Resources:ResourceCtrls,btnSendCmd %>" OnClick="SubmitButton_Click" CssClass="IconTransit" />
                    </td>
                    <td>
                        <asp:Button ID="RefreshButton" runat="server" Text="<%$Resources:ResourceCtrls,btnResult %>" OnClick="RefreshButton_Click" CssClass="IconRefresh" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <%-- 主要作業畫面：隱藏欄位部份 --%>
    <asp:HiddenField ID="hUserID" runat="server" />
    <asp:HiddenField ID="hOwnerList" runat="server" />
    <asp:HiddenField ID="hComplexQueryWheresql" runat="server" />
    <asp:HiddenField ID="hSaveComplexQueryData" runat="server" />
    <asp:HiddenField ID="hSaveGVHCheckState" runat="server" />
    <asp:HiddenField ID="hSaveGVRCheckStateList" runat="server" />
    <asp:HiddenField ID="hSelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="sLocArea" runat="server" />
    <asp:HiddenField ID="sLocBuilding" runat="server" />
    <asp:HiddenField ID="sLocFloor" runat="server" />
    <asp:HiddenField ID="tmpLocArea" runat="server" />
    <asp:HiddenField ID="tmpLocBuilding" runat="server" />
    <asp:HiddenField ID="txtAreaList" runat="server" />
    <asp:HiddenField ID="txtBuildingList" runat="server" />
    <asp:HiddenField ID="txtFloorList" runat="server" />

    <%-- ************************************************** 網頁畫面設計二 ************************************************** --%>

    <%-- 複合查詢畫面：設定「複合查詢」功能的條件資料 --%>
    <asp:Panel ID="PanelPopup0" runat="server" SkinID="PanelPopup" Width="450px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag0" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName0" runat="server" Text="<%$Resources:ResourceCtrls,ttAdvance %>" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton0" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />
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
                                <asp:Label ID="popLabel0_EquNo" runat="server" Text="<%$Resources:ResourceCtrls,EquNo %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput0_EquNo" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_EquName" runat="server" Text="<%$Resources:ResourceCtrls,EquName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput0_EquName" runat="server" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_EquEName" runat="server" Text="<%$Resources:ResourceCtrls,EquEName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput0_EquEName" runat="server" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_IsAndTrt" runat="server" Text="<%$Resources:ResourceCtrls,Clock %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="popInput0_IsAndTrt" runat="server" Width="98px">
                                    <asp:ListItem Value="" Enabled="true" Text="<%$ Resources:Resource,ddlSelectDefault %>"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="<%$ Resources:Resource,No %>"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="<%$ Resources:Resource,Yes %>"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_EquModel" runat="server" Text="<%$Resources:ResourceCtrls,EquModel %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_EquClass" runat="server" Text="<%$Resources:ResourceCtrls,EquType %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align: left">
                                <asp:Panel ID="popPanel0_EquModel" runat="server" Width="200px"></asp:Panel>
                            </td>
                            <td style="text-align: right">
                                <asp:TextBox ID="popInput0_EquClass" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                       <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_Area" runat="server" Text="<%$Resources:ResourceCtrls,lblArea %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_Building" runat="server" Text="<%$Resources:ResourceCtrls,Build %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_Floor" runat="server" Text="<%$Resources:ResourceCtrls,lblFloorName %>" Font-Bold="true"></asp:Label>
                            </th>

                        </tr>
                        <tr>
                            <td style="width: 140px">
                                <asp:DropDownList ID="ADVddlLocArea" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                    onchange="SelectADVArea()">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 140px">
                                <asp:DropDownList ID="ADVddlLocBuilding" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                    onchange="SelectADVBuilding()">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 140px">
                                <asp:DropDownList ID="ADVddlLocFloor" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table style="margin-left: 10px">
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit0" runat="server" EnableViewState="False">
                        <asp:Button ID="popBtn0_Query" runat="server" Text="<%$ Resources:Resource, btnQuery %>" EnableViewState="False" CssClass="IconSearch" />
                        <asp:Button ID="popBtn0_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                        <asp:Button ID="popBtn0_ClearQueryParam" runat="server" Text="<%$ Resources:Resource, btnClear %>" CssClass="IconClearData" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <%--調整按鈕與畫面底部之間的高度--%>
        <table>
            <tr>
                <td></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger0" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger0" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender0" runat="server"></cc1:ModalPopupExtender>

    <%-- ************************************************** 網頁畫面設計三 ************************************************** --%>

    <%-- 彈出作業畫面：檢視「控制器」設備資料功能的表格指定資料內容 --%>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="450px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
           <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="<%$Resources:ResourceCtrls,ttViewer %>" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
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
                                <asp:Label ID="popLabel1_EquID" runat="server" Text="<%$ Resources:ResourceCtrls, SerialKey %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_EquNo" runat="server" Text="<%$ Resources:ResourceCtrls, EquNo %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_EquID" runat="server" Width="95px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput1_EquNo" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_EquName" runat="server" Text="<%$ Resources:ResourceCtrls, EquName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_EquName" runat="server" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_EquEName" runat="server" Text="<%$ Resources:ResourceCtrls, EquEName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_EquEName" runat="server" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_IsAndTrt" runat="server" Text="<%$ Resources:ResourceCtrls, Clock %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_IsAndTrt" runat="server" Width="95px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_EquModel" runat="server" Text="<%$ Resources:ResourceCtrls, EquModel %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_EquClass" runat="server" Text="<%$ Resources:ResourceCtrls, EquType %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_EquModel" runat="server" Width="196px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput1_EquClass" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_Loc" runat="server" Text="位置" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_Loc" runat="server" Width="400"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popBtn1_Exit" runat="server" Text="<%$ Resources:Resource, btnExit %>" EnableViewState="False" CssClass="IconLeave" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <%--調整按鈕與畫面底部之間的高度--%>
        <table>
            <tr>
                <td></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
