<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExamInfo.aspx.cs" Inherits="SahoAcs.ExamInfo" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideOwnerList" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="propReadOnly" value="<%=Resources.ResourceGrp.propReadOnly %>" />
    </div>
    <table class="TableWidth">
        <tr>
            <td>
                <table class="Item">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_No" runat="server" Text="考試代碼"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_Name" runat="server" Text="考試名稱"></asp:Label>
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
                            <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource, btnQuery%>" CssClass="IconSearch" />
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
                                            DataKeyNames="ExamNo" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="ExamNo" HeaderText="考試代碼" SortExpression="ExamNo" />
                                                <asp:BoundField DataField="ExamName" HeaderText="考試名稱" SortExpression="ExamName" />
                                                <asp:BoundField DataField="OrgName" HeaderText="閱卷負責單位" SortExpression="OrgName" />
                                                <asp:BoundField DataField="ExamBeginTime" HeaderText="閱卷起始時間" SortExpression="ExamBeginTime" />
                                                <asp:BoundField DataField="ExamEndTime" HeaderText="閱卷結束時間" SortExpression="ExamEndTime" />
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
                            <asp:Button ID="btSelectData" runat="server" Text="閱卷相關人員" CssClass="IconEdit" />
                        </td>
                        <td>
                            <asp:Button ID="btAuthConfirm" runat="server" Text="人員通行權限重整" CssClass="IconEdit" />
                        </td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="920px" Height="400px" EnableViewState="False" CssClass="PopBg" HorizontalAlign="NotSet" >
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="閱卷負責單位" Font-Bold="True" ForeColor="White"
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
                    <%--<asp:TextBox ID="popInput_OrgNo" runat="server" Width="150px" CssClass="TextBoxRequired" Visible="false"></asp:TextBox>--%>
                    <table>
                        <tr>
                             <th class="auto-style1" style="width:200px;">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No" runat="server" Text="考試代碼" Font-Bold="true" ></asp:Label>
                            </th>
                            <th class="auto-style1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="考試名稱" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_No" runat="server" Width="100px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_Name" runat="server" Width="650px" Height="100px" CssClass="TextBoxRequired" TextMode="MultiLine"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OrgNo" runat="server" Text="閱卷負責單位" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <%--<td colspan="2">
                                <asp:TextBox ID="popInput_OrgNo" runat="server" Width="370px"></asp:TextBox>
                            </td>--%>
                            <td colspan="2">
                                <%--<select class="DropDownListStyle" name="OrgNo" id="OrgNo">
                                    <%for (int i = 0; i < 5; i++)
                                    { %>
                                    <option value="<%=i %>"><%=i %>
                                    </option>
                                <%} %>
                                </select>--%>                                
                            <asp:DropDownList ID="popInput_OrgNo" runat="server" Width="200px" AutoPostBack="false">
                             </asp:DropDownList>
                             </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ExamBeginTime" runat="server" Text="閱卷起始時間" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ExamEndTime" runat="server" Text="閱卷結束時間" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <uc1:Calendar ID="popInput_ExamBeginTime" runat="server" />
                            </td>
                            <td>
                                <uc1:Calendar ID="popInput_ExamEndTime" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th colspan="3" style="text-align:center">
                    <asp:Label ID="DeleteLableText" runat="server"></asp:Label>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                        <asp:Button ID="popB_Confirm" runat="server" Text="通行權限重整" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>

    <asp:Panel ID="PanelPopup2" runat="server" SkinID="PanelPopup" Width="630px"  EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag2" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName2" runat="server" Text="閱卷相關人員" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton2" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table class="Item">
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="lblKeyword" runat="server" Text="搜尋條件(類別、工號、姓名、單位)"></asp:Label><br />                                
                            </th>
                        </tr>
                        <tr>
                            <td colspan="4" style="text-align:left">
                                <asp:TextBox ID="Input_TxtQuery" runat="server" Width="250px"></asp:TextBox>
                                <asp:Button ID="popB_Query" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" /><br />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OrgList" runat="server" Text="人員列表" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_PsnList1" runat="server" Width="250px" Height="150px" SelectionMode="Multiple" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                            <td colspan="2" style="text-align:center">
                                <asp:Button ID="popB_Enter1" runat="server" Text="<%$ Resources:Resource, btnJoin%>" CssClass="IconRight" />
                                <br />
                                <br />
                                <asp:Button ID="popB_Remove1" runat="server" Text="<%$ Resources:Resource, btnRemove%>" CssClass="IconLeft" />
                            </td>
                            <td style="text-align:center">
                                <asp:ListBox ID="popB_PsnList2" runat="server" Width="250px" Height="150px" SelectionMode="Multiple" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="text-align:center">
                                <asp:Label ID="DeleteLableText1" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="Panel3" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_OK1" runat="server" Text="<%$ Resources:Resource, btnOK %>" EnableViewState="False" CssClass="IconOk" />
                        <asp:Button ID="popB_Cancel1" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>        
    <asp:Label ID="PopupTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender2" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .auto-style1 {
            height: 15px;
        }
    </style>
</asp:Content>

