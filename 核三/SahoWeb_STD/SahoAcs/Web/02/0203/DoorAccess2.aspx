<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DoorAccess2.aspx.cs" Inherits="SahoAcs.DoorAccess2" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <link href="../../../Css/colorbox.css" rel="stylesheet" />
    <script src="../../../Scripts/jquery.colorbox-min.js"></script>
    <script>
        $(function () {
            $("#tabs").tabs();
        });
    </script>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    <table style="width: 100%">
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
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_Trt" runat="server" Text="<%$Resources:ResourceCtrls,Clock %>"></asp:Label>
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
                            <asp:DropDownList ID="Input_Trt" runat="server">
                                 <asp:ListItem Value="" Enabled="true" Text="<%$ Resources:Resource,ddlSelectDefault %>"></asp:ListItem>                                    
                                    <asp:ListItem Value="1" Text="<%$ Resources:Resource,Yes %>"></asp:ListItem>
                                <asp:ListItem Value="0" Text="<%$ Resources:Resource,No %>"></asp:ListItem>
                            </asp:DropDownList>
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
                                                <asp:BoundField DataField="Floor" HeaderText="<%$ Resources:Resource,Floor %>" SortExpression="Floor" />
                                                <asp:BoundField DataField="EquNo" HeaderText="<%$ Resources:Resource,EquNo %>" SortExpression="EquNo" />
                                                <asp:BoundField DataField="EquName" HeaderText="<%$ Resources:Resource,EquName %>" SortExpression="EquName" />
                                                <asp:BoundField DataField="EquModel" HeaderText="<%$Resources:ResourceCtrls,EquModel %>" SortExpression="EquModel" />
                                                <asp:BoundField DataField="IsAndTrt" HeaderText="<%$Resources:ResourceCtrls,Clock %>" SortExpression="CtrlStatus" />
                                                <asp:BoundField DataField="InCtrlAreaName" HeaderText="<%$ Resources:Resource,InMgn %>" SortExpression="CtrlStatus" />
                                                <asp:BoundField DataField="OutCtrlAreaName" HeaderText="<%$ Resources:Resource,OutMgn %>" SortExpression="CtrlStatus" />
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
                            <asp:Button ID="Button2" runat="server" Text="Test" Visible="False" />
                        </td>
                        <td>
                            <asp:Button ID="SetButton" runat="server" Text="<%$ Resources:Resource, CodeSetting %>" OnClick="SetButton_Click" CssClass="IconPassword" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <%-- ==========設備參數資料定義========== --%>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="480px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
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
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Building" runat="server" Text="建築物名稱" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Floor" runat="server" Text="樓層" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_Building" runat="server" Width="195px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_Floor" runat="server" Width="154px" CssClass="TextBoxRequired"></asp:TextBox>
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
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EquModel" runat="server" Text="設備型號" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EquNo" runat="server" Text="設備編號" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="popInput_EquModel" runat="server" OnInit="popInput_EquModel_Init" Width="124px" BackColor="#FFE5E5"></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_EquNo" runat="server" Width="229px" CssClass="TextBoxRequired"></asp:TextBox>
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
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EquName" runat="server" Text="設備名稱" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EquEName" runat="server" Text="設備英文名稱" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_EquName" runat="server" Width="174px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_EquEName" runat="server" Width="174px"></asp:TextBox>
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
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_InToCtrlArea" runat="server" Text="進入管制區" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OutToCtrlArea" runat="server" Text="離開管制區" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="popInput_InToCtrlArea" runat="server" OnInit="popInput_InToCtrlArea_Init" Width="178px"></asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="popInput_OutToCtrlArea" runat="server" OnInit="popInput_OutToCtrlArea_Init" Width="178px"></asp:DropDownList>
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
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Trt" runat="server" Text="卡鐘設定" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Dci" runat="server" Text="設備連線" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_CardNoLen" runat="server" Text="卡號長度" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_BlackMode" runat="server" Text="名單模式" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="popInput_Trt" runat="server" Width="90px">
                                    <asp:ListItem Value="1" Enabled="True" Selected="True">是</asp:ListItem>
                                    <asp:ListItem Value="0">否</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="popInput_Dci" runat="server" OnInit="popInput_Dci_Init" BackColor="#FFE5E5" Width="150px"></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_CardNoLen" runat="server" Width="80px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:DropDownList ID="popInput_BlackMode" runat="server" Width="100px">
                                    <asp:ListItem Value="0" Enabled="True" Selected="True">無名單</asp:ListItem>
                                    <asp:ListItem Value="1">黑名單</asp:ListItem>
                                    <asp:ListItem Value="2">白名單</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th style="text-align: center;">
                    <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                </th>
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
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
    <%-- ==========設備參數設定資料
    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
        <ContentTemplate>
            <asp:Panel ID="PanelPopup2" runat="server" SkinID="PanelPopup" Width="600px" EnableViewState="False" CssClass="PopBg">
                <asp:Panel ID="PanelDrag2" runat="server" SkinID="PanelDrag" EnableViewState="False">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <asp:Label ID="L_popName2" runat="server" Text="設備參數設定資料" Font-Bold="True" ForeColor="White"
                                    EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                            </td>
                            <td style="text-align: right;">
                                <asp:ImageButton ID="ImgCloseButton2" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                                    EnableViewState="False" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <div id="tabs">
                    <ul>
                        <li>
                            <a href="#tabs-1">
                                <asp:Label ID="lblTabs1" runat="server" Text="控制器參數"></asp:Label>
                            </a>
                        </li>
                        <li>
                            <a href="#tabs-2">
                                <asp:Label ID="lblTabs2" runat="server" Text="管制參數"></asp:Label>
                            </a>
                        </li>
                    </ul>
                    <div id="tabs-1">
                        <table class="popItem">
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_AuthCode" runat="server" Text="認證碼設定" Font-Bold="true"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_ControlCode" runat="server" Text="卡片管制碼設定" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="popInput_AuthCode" runat="server" CssClass="TextBoxRequired" MaxLength="20" Width="180px"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="popInput_ControlCode" runat="server" CssClass="TextBoxRequired" MaxLength="16" Width="140px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_SenseSendMode" runat="server" Text="卡片傳輸模式" Font-Bold="True"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_HolidayData" runat="server" Text="假日資料設定" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="popDrop_SenseSendMode" runat="server" BackColor="#FFE5E5" Width="190px">
                                        <asp:ListItem Text="正常傳輸" Value="00"></asp:ListItem>
                                        <asp:ListItem Text="讀卡直接傳送" Value="01"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="popDrop_HolidayData" runat="server" BackColor="#FFE5E5" Width="150px">
                                        <asp:ListItem Text="傳送" Value="01" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="清除" Value="03"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_TwoByOne" runat="server" Text="二對一控制" Font-Bold="true"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_OneByOne" runat="server" Text="一進一出控制" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButtonList ID="popRdo_TwoByOne" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="Disable" Value="" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Enable" Value=""></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="popRdo_OneByOne" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="Disable" Value="" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Enable" Value=""></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th colspan="2">
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_GeneralTimeZone" runat="server" Text="一般時區設定" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:DropDownList ID="popDrop_GeneralTimeZone" runat="server" BackColor="#FFE5E5" Width="430px" OnInit="popDrop_GeneralTimeZone_Init">
                                    </asp:DropDownList>
                                    <asp:Button ID="popBtn_GeneralTimeZoneIns" runat="server" Text="新增" CssClass="IconNew" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="popLabel_GeneralTimeZoneComment" runat="server" Text="※最多設定99組時區資料※" ForeColor="Maroon" Font-Bold="True"></asp:Label>
                                    <hr />
                                    <asp:TextBox ID="popInput_GeneralTimeZoneData" runat="server" TextMode="MultiLine" BackColor="#FFE5E5" Height="100px" Width="430px"></asp:TextBox>&nbsp
                                    <asp:Button ID="popBtn_GeneralTimeZoneDel" runat="server" Text="刪除" CssClass="IconDelete" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabs-2">
                        <table class="popItem">
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_ControlMode" runat="server" Text="管制模式設定" Font-Bold="true"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_ControlModeTimeZone" runat="server" Text="管制模式時區設定" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="popDrop_ControlMode" runat="server" BackColor="#FFE5E5" Width="250px">
                                        <asp:ListItem Text="禁止模式" Value="00"></asp:ListItem>
                                        <asp:ListItem Text="開放模式" Value="01"></asp:ListItem>
                                        <asp:ListItem Text="管制模式" Value="02"></asp:ListItem>
                                        <asp:ListItem Text="時區模式" Value="03"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="popDrop_ControlModeTimeZone" runat="server" BackColor="#FFE5E5" Width="250px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_SenseMode" runat="server" Text="讀卡模式設定" Font-Bold="true"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_SenseModeTimeZone" runat="server" Text="讀卡模式時區設定" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="popDrop_SenseMode" runat="server" BackColor="#FFE5E5" Width="250px">
                                        <asp:ListItem Text="無需輸入密碼" Value="00"></asp:ListItem>
                                        <asp:ListItem Text="需要輸入密碼" Value="01"></asp:ListItem>
                                        <asp:ListItem Text="無需檢查設碼" Value="02"></asp:ListItem>
                                        <asp:ListItem Text="讀卡時區模式" Value="03"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="popDrop_SenseModeTimeZone" runat="server" BackColor="#FFE5E5" Width="250px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_BellMode" runat="server" Text="讀卡機蜂鳴器響聲設定" Font-Bold="true"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_AlertSense" runat="server" Text="警報偵測設定" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="popDrop_BellMode" runat="server" BackColor="#FFE5E5" Width="250px">
                                        <asp:ListItem Text="讀卡機蜂鳴器響一長音" Value="01"></asp:ListItem>
                                        <asp:ListItem Text="讀卡機蜂鳴器響一短音" Value="02"></asp:ListItem>
                                        <asp:ListItem Text="讀卡機蜂鳴器響三短音" Value="03"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="popRdo_AlertSense" runat="server" BackColor="#FFE5E5" Width="250px">
                                        <asp:ListItem Text="警告/警報偵測功能關閉" Value="00"></asp:ListItem>
                                        <asp:ListItem Text="警告/警報發生時只要將門關上就解除" Value="01"></asp:ListItem>
                                        <asp:ListItem Text="警告發生時只要將門關上就解除;當警報發生時需要有合法卡(已設碼)或按特殊密碼開門解除" Value="02"></asp:ListItem>
                                        <asp:ListItem Text="警告發生時需要有合法卡(已設碼)或按特殊密碼開門解除;警報發生時需要用母卡來讀卡開門(命令'1')解除" Value="03"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_OpenLock" runat="server" Text="電鎖開門時間設定(單位：秒)" Font-Bold="true"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_TimeOutLock" runat="server" Text="逾時關門時間設定(單位：秒)" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="popInput_OpenLock" runat="server" CssClass="TextBoxRequired" MaxLength="3" Width="240px"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="popInput_TimeOutLock" runat="server" CssClass="TextBoxRequired" MaxLength="3" Width="240px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_HolidayMode" runat="server" Text="假日模式" Font-Bold="true"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_ButtonOpen" runat="server" Text="按鈕開門" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButtonList ID="popRdo_HolidayMode" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="Disable" Value="00"></asp:ListItem>
                                        <asp:ListItem Text="Enable" Value="01" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="popRdo_ButtonOpen" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="Disable" Value="00"></asp:ListItem>
                                        <asp:ListItem Text="Enable" Value="01" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th colspan="3">
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="popLabel_SpecialPwd" runat="server" Text="特殊密碼設定" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <table>
                                        <tr>
                                            <td>01：<asp:TextBox ID="popInput_SpecialPwd1" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                02：<asp:TextBox ID="popInput_SpecialPwd2" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                03：<asp:TextBox ID="popInput_SpecialPwd3" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                04：<asp:TextBox ID="popInput_SpecialPwd4" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>05：<asp:TextBox ID="popInput_SpecialPwd5" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                06：<asp:TextBox ID="popInput_SpecialPwd6" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                07：<asp:TextBox ID="popInput_SpecialPwd7" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                08：<asp:TextBox ID="popInput_SpecialPwd8" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>09：<asp:TextBox ID="popInput_SpecialPwd9" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                10：<asp:TextBox ID="popInput_SpecialPwd10" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                11：<asp:TextBox ID="popInput_SpecialPwd11" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                12：<asp:TextBox ID="popInput_SpecialPwd12" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>13：<asp:TextBox ID="popInput_SpecialPwd13" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                14：<asp:TextBox ID="popInput_SpecialPwd14" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                15：<asp:TextBox ID="popInput_SpecialPwd15" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                                16：<asp:TextBox ID="popInput_SpecialPwd16" runat="server" CssClass="TextBoxRequired" MaxLength="4" Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div style="text-align: center">
                    <asp:Button ID="popBtn_Save" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                    <asp:Button ID="popBtn_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Label ID="PopupTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender2" runat="server"></cc1:ModalPopupExtender>
    --%>
</asp:Content>
