<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QuerySysLog.aspx.cs"
    Inherits="SahoAcs.QuerySysLog" Debug="true" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <%-- ************************************************** 網頁畫面設計一 ************************************************** --%>

    <%-- 主要作業畫面：查詢部份 --%>
    <asp:UpdatePanel ID="UpdatePanel0" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="Item">
                <tr>
                    <th>
                        <span class="Arrow01"></span>
                        <asp:Label ID="QueryLabel_KeyWord" runat="server" Text="<%$Resources:lblKeyword %>"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="QueryInput_KeyWord" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
                    </td>
                    <td>
                        <asp:Button ID="ComplexQueryButton" runat="server" Text="<%$Resources:Resource,btnAdvance %>" class="IconMultisearch" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <%-- 主要作業畫面：表格部份 --%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellspacing="0" class="TableS1">
                <%-- GridView Header's HTML Code --%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight" Width="1500px">
                            <%-- GridView Body's HTML Code --%>
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="7"
                                DataKeyNames="RecordID" AllowPaging="True" AutoGenerateColumns="False"
                                OnDataBound="GridView_Data_DataBound" OnRowDataBound="GridView_Data_RowDataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="RecordID" HeaderText="識別碼" SortExpression="RecordID" Visible="false" />
                                    <%--<asp:BoundField DataField="SyncMarkName" HeaderText="MIS同步" SortExpression="SyncMarkName" />--%>
                                    <%--<asp:BoundField DataField="LogStatusName" HeaderText="讀卡結果" SortExpression="LogStatusName" />--%>
                                    <asp:BoundField DataField="LogTime" HeaderText="<%$Resources:ttRecordTime %>" SortExpression="LogTime" />
                                    <asp:BoundField DataField="LogType" HeaderText="<%$Resources:ttRecordType %>" SortExpression="LogType" />
                                    <asp:BoundField DataField="UserID" HeaderText="<%$Resources:ttUserID %>" SortExpression="UserID" />
                                    <asp:BoundField DataField="UserName" HeaderText="<%$Resources:ttUserName %>" SortExpression="UserName" />
                                    <asp:BoundField DataField="LogFrom" HeaderText="<%$Resources:ttFuncName %>" SortExpression="LogFrom" />
                                    <asp:BoundField DataField="LogIP" HeaderText="<%$Resources:ttIP %>" SortExpression="LogIP" />
                                    <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:ttEquNo %>" SortExpression="EquNo" />
                                    <asp:BoundField DataField="EquName" HeaderText="<%$Resources:ttEquName %>" SortExpression="EquName" />
                                    <asp:BoundField DataField="LogInfo" HeaderText="<%$Resources:ttInfo %>" SortExpression="LogInfo" />
                                    <asp:BoundField DataField="LogDesc" HeaderText="<%$Resources:ttRecordDesc %>" SortExpression="LogDesc" />
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
    <table cellspacing="5">
        <tr>
            <td>
                <asp:Button ID="ViewButton" runat="server" Text="<%$Resources:Resource,btnView %>" CssClass="IconLook" />
            </td>
            <td>
                <asp:Button ID="ExcelButton" runat="server" Text="<%$Resources:Resource,btnExport %>" OnClick="ExcelButton_Click" CssClass="IconExport" />
            </td>
        </tr>
    </table>

    <%-- 主要作業畫面：隱藏欄位部份 --%>
    <asp:HiddenField ID="hUserID" runat="server" />
    <asp:HiddenField ID="hOwnerList" runat="server" />
    <asp:HiddenField ID="hComplexQueryWheresql" runat="server" />
    <asp:HiddenField ID="hSaveComplexQueryData" runat="server" />
    <asp:HiddenField ID="hSelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />

    <%-- ************************************************** 網頁畫面設計二 ************************************************** --%>

    <%-- 複合查詢畫面：設定「系統事件紀錄查詢」功能的複合查詢條件 --%>
    <asp:Panel ID="PanelPopup0" runat="server" SkinID="PanelPopup" Width="470px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag0" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName0" runat="server" Text="設定「複合查詢」條件資料" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
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
                                <asp:Label ID="popLabel0_STime" runat="server" Text="<%$Resources:Resource,lblStart %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_ETime" runat="server" Text="<%$Resources:Resource,lblEnd %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <uc1:Calendar runat="server" ID="popInput0_STime" />
                            </td>
                            <td>
                                <uc1:Calendar runat="server" ID="popInput0_ETime" />
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_UserID" runat="server" Text="<%$Resources:ttUserID %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_UserName" runat="server" Text="<%$Resources:ttUserName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput0_UserID" runat="server" Width="196px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput0_UserName" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_LogFrom" runat="server" Text="<%$Resources:ttFuncName %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_LogIP" runat="server" Text="<%$Resources:ttIP %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput0_LogFrom" runat="server" Width="196px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput0_LogIP" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_EquNo" runat="server" Text="<%$Resources:ttEquNo %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_EquName" runat="server" Text="<%$Resources:ttEquName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput0_EquNo" runat="server" Width="196px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput0_EquName" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_LogInfo" runat="server" Text="<%$Resources:ttInfo %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput0_LogInfo" runat="server" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel0_LogDesc" runat="server" Text="<%$Resources:ttRecordDesc %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput0_LogDesc" runat="server" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit0" runat="server" EnableViewState="False">
                        <asp:Button ID="popBtn0_Query" runat="server" Text="<%$Resources:Resource,btnQuery %>" EnableViewState="False" CssClass="IconSearch" />
                        <asp:Button ID="popBtn0_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                        <asp:Button ID="popBtn0_ClearQueryParam" runat="server" Text="<%$Resources:Resource,btnClear %>" EnableViewState="False" CssClass="IconClearData" />
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

    <%-- 彈出作業畫面：檢視「系統事件紀錄查詢」功能的表格指定資料內容 --%>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="450px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="檢視「系統事件」記錄資料" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
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
                                <asp:Label ID="popLabel1_RecordID" runat="server" Text="<%$Resources:ttRecordID %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_RecordID" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_LogTime" runat="server" Text="<%$Resources:ttRecordTime %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_LogType" runat="server" Text="<%$Resources:ttRecordType %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_LogTime" runat="server" Width="196px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput1_LogType" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_UserID" runat="server" Text="<%$Resources:ttUserID %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_UserName" runat="server" Text="<%$Resources:ttUserName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_UserID" runat="server" Width="196px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput1_UserName" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_LogFrom" runat="server" Text="<%$Resources:ttFuncName %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_LogIP" runat="server" Text="<%$Resources:ttIP %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_LogFrom" runat="server" Width="196px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput1_LogIP" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_EquNo" runat="server" Text="<%$Resources:ttEquNo %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_EquName" runat="server" Text="<%$Resources:ttEquName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_EquNo" runat="server" Width="196px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput1_EquName" runat="server" Width="196px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_LogInfo" runat="server" Text="<%$Resources:ttInfo %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_LogInfo" runat="server" Width="420px" Height="50px" TextMode="MultiLine"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel1_LogDesc" runat="server" Text="<%$Resources:ttRecordDesc %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput1_LogDesc" runat="server" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <asp:Panel ID="Panel1" runat="server" EnableViewState="False">
                        <asp:Button ID="popBtn1_Exit" runat="server" Text="<%$Resources:Resource,btnExit %>" EnableViewState="False" class="IconLeave" />
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
