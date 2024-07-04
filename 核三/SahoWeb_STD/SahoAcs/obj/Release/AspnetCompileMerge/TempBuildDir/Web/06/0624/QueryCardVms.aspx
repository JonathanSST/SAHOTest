<%@ Page Language="C#" Inherits="SahoAcs.QueryCardVms" MasterPageFile="~/Site1.Master" Debug="true" 
    AutoEventWireup="true" EnableEventValidation="false" Theme="UI" CodeBehind="QueryCardVms.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="~/uc/MultiSelectDropDown.ascx" TagPrefix="uc2" TagName="MultiSelectDropDown" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="<%$Resources:ttCardTime %>"></asp:Label>
                        </th>
                        <th  runat="server" id="ShowPsnInfo1">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardNo_PsnName" runat="server" Text="<%$Resources:lblPsnNo %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_LogStatus" runat="server" Text="<%$Resources:ttResult %>"></asp:Label>
                        </th>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeSDate" />
                        </td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeEDate" />
                        </td>
                        <td  runat="server" id="ShowPsnInfo2">
                            <asp:TextBox ID="TextBox_CardNo_PsnName" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <uc2:MultiSelectDropDown runat="server" ID="DropDownList_LogStatus" ListWidth="150" ListHeight="300" />
                        </td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
                        </td>
                        <td runat="server" id="ShowPsnInfo3">
                            <asp:Button ID="ADVQueryShowButton" runat="server" Text="<%$Resources:Resource,btnAdvance %>" CssClass="IconMultisearch" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1">
                <%--GridView Header的Html Code--%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight" Width="1300px">
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" Width="100%" PageSize="5"
                                EnableViewState="False" DataKeyNames="RecordId" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="CardTime" HeaderText="<%$Resources:ttCardTime %>" SortExpression="CardTime" />
                                    <asp:BoundField DataField="DepNo" HeaderText="<%$Resources:ttDeptNo %>" SortExpression="DepNo" />
                                    <asp:BoundField DataField="DepName" HeaderText="<%$Resources:ttDeptName %>" SortExpression="DepName" />
                                    <asp:BoundField DataField="PsnNo" HeaderText="<%$Resources:ttPsnNo %>" SortExpression="PsnNo" />
                                    <asp:BoundField DataField="PsnName" HeaderText="<%$Resources:ttPsnName %>" SortExpression="PsnName" />
                                    <asp:BoundField DataField="CardNo" HeaderText="<%$Resources:ttCardNo %>" SortExpression="CardNo" />
                                    <asp:BoundField DataField="CardVer" HeaderText="<%$Resources:ttCardVer %>" SortExpression="CardVer" />
                                    <%--<asp:BoundField DataField="CtrlNo" HeaderText="控制器編號" SortExpression="CtrlNo" />
                                    <asp:BoundField DataField="ReaderNo" HeaderText="讀卡機編號" SortExpression="ReaderNo" />--%>
                                    <%--<asp:BoundField DataField="EquNo" HeaderText="設備編號" SortExpression="EquNo" />--%>
                                    <asp:TemplateField HeaderText="<%$Resources:ttEquNo %>" SortExpression="EquNo">
                                        <ItemTemplate>                                            
                                            <asp:LinkButton ID="BtnVideo" runat="server" EnableViewState="false" CssClass="Video">
                                                <%# DataBinder.Eval(Container.DataItem, "EquNo")%>                                              
                                            </asp:LinkButton>                                            
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="EquName" HeaderText="<%$Resources:ttEquName %>" SortExpression="EquName" />
                                    <asp:BoundField DataField="LogStatus" HeaderText="<%$Resources:ttResult %>" SortExpression="LogStatus" />
                                    <asp:BoundField DataField="LogTime" HeaderText="<%$Resources:ttLogTime %>" SortExpression="LogTime" />
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
    <br />
    <div>
        <asp:Button ID="ViewButton" runat="server" Text="<%$Resources:Resource,btnView %>" CssClass="IconLook" />
        <asp:Button ID="ExportButton" runat="server" Text="<%$Resources:Resource,btnExport %>" CssClass="IconExport" />
    </div>

    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="650px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="<%$Resources:lblViewSysLog %>" Font-Bold="True" ForeColor="White"
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
                            <td>
                                <table class="TableWidth">
                                    <tr>
                                        <th style="text-align: left; width: 20%">
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_CardTime" runat="server" Text="<%$Resources:ttCardTime %>"></asp:Label>
                                        </th>
                                        <td style="text-align: left; width: 20%">
                                            <asp:TextBox ID="ADV_TextBox_CardTime" runat="server" Width="200" BorderColor="White" ReadOnly="true"></asp:TextBox>
                                        </td>
                                        <th style="text-align: right; width: 20%">
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_LogStatus" runat="server" Text="<%$Resources:ttResult %>" ReadOnly="True"></asp:Label>
                                        </th>
                                        <td style="text-align: right; width: 20%">
                                            <asp:TextBox ID="ADV_TextBox_LogStatus" runat="server" ReadOnly="True" Enabled="false"></asp:TextBox>
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
                                            <asp:Label ID="ADV_Label_DepNo" runat="server" Text="<%$Resources:ttDeptNo %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_DepName" runat="server" Text="<%$Resources:ttDeptName %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_PsnNo" runat="server" Text="<%$Resources:ttPsnNo %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_PsnName" runat="server" Text="<%$Resources:ttPsnName %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_DepNo" runat="server" ReadOnly="true" Enabled="false" Width="100px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_DepName" runat="server" ReadOnly="true" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_PsnNo" runat="server" ReadOnly="true" Enabled="false" Width="100px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_PsnName" runat="server" ReadOnly="true" Enabled="false" Width="150px"></asp:TextBox>
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
                                            <asp:Label ID="ADV_Label_CardNo" runat="server" Text="<%$Resources:ttCardNo %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_CardVer" runat="server" Text="<%$Resources:ttCardVer %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_EquNo" runat="server" Text="<%$Resources:ttEquNo %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_EquName" runat="server" Text="<%$Resources:ttEquName %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_CardNo" runat="server" ReadOnly="true" Enabled="false" Width="120px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_CardVer" runat="server" ReadOnly="true" Enabled="false" Width="80px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_EquNo" runat="server" ReadOnly="true" Enabled="false" Width="100px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_EquName" runat="server" ReadOnly="true" Enabled="false" Width="205px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table class="TableWidth">
                                    <tr>
                                        <th style="text-align: left; width: 17%">
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADV_Label_LogTime" runat="server" Text="<%$Resources:ttLogTime %>"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="ADV_TextBox_LogTime" runat="server" Width="200" BorderColor="White" ReadOnly="true"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
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
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$Resources:Resource,btnCancel %>" EnableViewState="False" class="IconLeave" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>

    <asp:Panel ID="PanelPopup2" runat="server" SkinID="PanelPopup" Width="500px" CssClass="PopBg">
        <asp:Panel ID="PanelDrag2" runat="server" SkinID="PanelDrag">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName2" runat="server" Text="<%$Resources:lblShowAdv %>" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
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
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <th colspan="3">
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_CardTime" runat="server" Text="<%$Resources:ttCardTime %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc1:Calendar runat="server" ID="ADVCalendar_CardTimeSDate" />
                                        </td>
                                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                                        <td>
                                            <uc1:Calendar runat="server" ID="ADVCalendar_CardTimeEDate" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <th colspan="3">
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_LogTime" runat="server" Text="<%$Resources:ttLogTime %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc1:Calendar runat="server" ID="ADVCalendar_LogTimeSDate" />
                                        </td>
                                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                                        <td>
                                            <uc1:Calendar runat="server" ID="ADVCalendar_LogTimeEDate" />
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
                                            <asp:Label ID="ADVQueryLabel_DepNoDepName" runat="server" Text="<%$Resources:ttDeptNo %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_Dep" runat="server" Text="<%$Resources:ttDeptName %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="ADVTextBox_DepNoDepName" runat="server" Width="180"></asp:TextBox>
                                        </td>
                                        <td>
                                            <uc2:MultiSelectDropDown runat="server" ID="ADVDropDownList_Dep" ListWidth="240" ListHeight="150" />
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
                                            <asp:Label ID="ADVQueryLabel_EquNoEquName" runat="server" Text="<%$Resources:ttEquName %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_Equ" runat="server" Text="<%$Resources:ttEquName %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="ADVTextBox_EquNoEquName" runat="server" Width="180"></asp:TextBox>
                                        </td>
                                        <td>
                                            <uc2:MultiSelectDropDown runat="server" ID="ADVDropDownList_Equ" ListWidth="240" ListHeight="150" />
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
                                            <asp:Label ID="ADVQueryLabel_PsnNo" runat="server" Text="<%$Resources:ttPsnNo %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_PsnNamePsnNameCardNo" runat="server" Text="<%$Resources:lblPsnNo %>"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_LogStatus" runat="server" Text="<%$Resources:ttResult %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="ADVTextBox_PsnNo" runat="server" Width="120"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ADVTextBox_PsnNameCardNo" runat="server" Width="120"></asp:TextBox>
                                        </td>
                                        <td>
                                            <uc2:MultiSelectDropDown runat="server" ID="ADVDropDownList_LogStatus" ListWidth="175" ListHeight="150" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="ADVQueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" EnableViewState="False" CssClass="IconSearch" />
                            </td>
                            <td>                            
                                <asp:Button ID="ADVCloseButton" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
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
</asp:Content>
