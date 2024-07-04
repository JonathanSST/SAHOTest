<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="OrgData2.aspx.cs" Inherits="SahoAcs.OrgData2" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <style type="text/css">
        .style1{
            color:black;
            background-color:white;
            position:absolute;
            top:10px;
            left:10px;
        }
    </style>
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblOrgNo" runat="server" Text="<%$ Resources:lblOrgNo %>"></asp:Label>                
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblOrgName" runat="server" Text="<%$ Resources:lblOrgName %>"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblOrgClass" runat="server" Text="<%$ Resources:lblOrgClass %>"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="Input_No" runat="server"></asp:TextBox></td>
            <td>
                <asp:TextBox ID="Input_Name" runat="server"></asp:TextBox></td>
            <td>
                <asp:DropDownList ID="Input_Class" runat="server" Width="100px"></asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" />
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1" style="width: 850px">
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                DataKeyNames="OrgID" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="False" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="OrgID" HeaderText="<%$ Resources:OrgID %>" SortExpression="OrgID" />
                                    <asp:BoundField DataField="OrgNo" HeaderText="<%$ Resources:OrgNo %>" SortExpression="OrgNo" />
                                    <asp:BoundField DataField="OrgName" HeaderText="<%$ Resources:OrgName %>" SortExpression="OrgName" />
                                    <asp:BoundField DataField="OrgClass" HeaderText="<%$ Resources:OrgClass %>" SortExpression="OrgClass" />
                                    <asp:BoundField DataField="CreateUserID" HeaderText="<%$ Resources:CreateUserID %>" SortExpression="CreateUserID" />
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
                <asp:Literal ID="li_Pager" runat="server" />
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectNowNo" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hSelectState" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectClassNo" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <input type="hidden" id="ddlSelectDefault" value="<%=GetGlobalResourceObject("Resource","ddlSelectDefault") %>" />
    <table>
        <tr>
            <td>
                <asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />
            </td>
            <td>
                <asp:Button ID="SaveButton" Enabled="false" runat="server" Text="<%$ Resources:Resource, btnSave %>" CssClass="IconSave" />
                <input type="hidden" id="PageEvent" name="PageEvent" value="" />
            </td>
            <td>
                <asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit" />
            </td>
            <td>
                <asp:Button ID="DeleteButton" runat="server" Text="<%$ Resources:Resource, btnDelete %>" CssClass="IconDelete" />
            </td>
        </tr>
    </table>
    <input type="hidden" value="0" id="RowIndex" />
    <div id="NewRow" style="display: none">
        <table id="TableEmpty">
            <tr id="GV_Row20" class="ChR" style="color: rgb(0, 0, 0);">
                <td style="width:26px">
                    <input type="checkbox" name="CHK_COL_0" style="width:25px;height:25px" value="0" />
                </td>
                <td title="組織編號" style="width: 103px;">
                    <input type="text" id="NewOrgNo" name="NewOrgNo" style="width: 95%" maxlength="15" 
                        MUST_KEYIN_YN="Y" FIELD_NAME="組織編號"/>
                </td>
                <td title="組織名稱" style="width: 204px;">
                    <input type="text" id="NewOrgName" name="NewOrgName" style="width: 95%" maxlength="15"
                         MUST_KEYIN_YN="Y"  FIELD_NAME="組織名稱"/>
                </td>
                <td title="組織類型" align="center" style="width: 154px;">
                    <select name="NewOrgClass" id="NewOrgClass" 
                        class="DropDownListStyle" style="width: 150px;">                        
                        <%foreach(System.Data.DataRow r in this.OrgClassData.Rows){ %>
                        <option value="<%=Convert.ToString(r["ItemNo"]) %>"><%=Convert.ToString(r["ItemName"]) %></option>
                        <%} %>
                    </select>
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="430px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
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
                <td>
                    <table>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No" runat="server" Text="<%$ Resources:lblOrgNo %>" Font-Bold="True"></asp:Label></th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_No_Title" runat="server" Width="20px" BorderWidth="1" CssClass="TextBoxRequired" Enabled="False"></asp:TextBox>
                                <asp:TextBox ID="popInput_No" runat="server" Width="350px" BorderWidth="1" CssClass="TextBoxRequired" MaxLength="15"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="<%$ Resources:lblOrgName %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_Name" runat="server" Width="390px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Class" runat="server" Text="<%$ Resources:lblOrgClass %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:DropDownList ID="popInput_Class" runat="server" Width="400px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="popItem">
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave %>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave %>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
    <asp:Panel ID="PanelPopup2" runat="server" SkinID="PanelPopup" Width="430px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag2" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName2" runat="server" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton2" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table border="0" class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No2" runat="server" Text="<%$ Resources:lblOrgNo %>" Font-Bold="True"></asp:Label></th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_No2" MaxLength="15" runat="server" Width="390px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name2" runat="server" Text="<%$ Resources:lblOrgName %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2" class="auto-style1">
                                <asp:TextBox ID="popInput_Name2" runat="server" Width="390px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Class2" runat="server" Text="<%$ Resources:lblOrgClass %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_Class2" runat="server" Width="390px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Class3" runat="server" Text="<%$ Resources:lblTransferClass %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:DropDownList ID="popInput_Class3" runat="server" Width="400px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText2" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit2" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete %>" EnableViewState="False" CssClass="IconDelete" />
                        <asp:Button ID="popB_Cancel2" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger2" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender2" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
