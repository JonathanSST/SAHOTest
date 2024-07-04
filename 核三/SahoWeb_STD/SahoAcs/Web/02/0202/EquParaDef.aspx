<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EquParaDef.aspx.cs" Inherits="SahoAcs.EquParaDef" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideEquModel" runat="server" EnableViewState="False" />
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
                            <asp:Label ID="Label_EquType" runat="server" Text="<%$ Resources:ResourceEquData,EquType %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_EquModel" runat="server" Text="<%$ Resources:ResourceEquData,EquModel %>"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="Input_EquType" runat="server" OnSelectedIndexChanged="Input_EquType_SelectedIndexChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="Input_EquModel" runat="server" Font-Size="13px">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
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
                                            DataKeyNames="ParaName" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="ParaName" HeaderText="<%$ Resources:ResourceEquData,ParaName %>" SortExpression="ParaName" />
                                                <asp:BoundField DataField="ParaDesc" HeaderText="<%$ Resources:ResourceEquData,ParaDesc %>" SortExpression="ParaDesc" />
                                                <asp:BoundField  DataField="InputType" HeaderText="<%$ Resources:ResourceEquData,InputMode %>" SortExpression="InputType" />
                                                <asp:BoundField HeaderText="<%$ Resources:ResourceEquData,ParaDef %>" />
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
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="410px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"><%=GetGlobalResourceObject("ResourceEquData","EquParaDefineEdit")%></asp:Label>                        
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
                            <td colspan="2">
                                <table>
                                    <tr>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="popLabel_InputType" runat="server" Text="<%$ Resources:ResourceEquData,InputMode %>"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:DropDownList ID="popInput_InputType" runat="server">
                                                <asp:ListItem Text="<%$ Resources:ResourceEquData,ParaTextField%>" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="<%$ Resources:ResourceEquData,ParaNumField %>" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="<%$ Resources:ResourceEquData,ParaItemSelect %>" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="<%$ Resources:ResourceEquData,ParaRefLink %>" Value="3"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ParaName" runat="server" Text="<%$Resources:ResourceEquData,ParaName %>"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ParaDesc" runat="server" Text="<%$Resources:ResourceEquData,ParaDesc %>"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_ParaName" runat="server" Width="170" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_ParaDesc" runat="server" Width="170" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <div id="div_Value" runat="server">
                                    <table>
                                        <tr>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="popLabel_MinValue" runat="server" Text="<%$Resources:ResourceEquData,MinValue %>"></asp:Label>
                                            </th>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="popLabel_MaxValue" runat="server" Text="<%$Resources:ResourceEquData,MaxValue %>"></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="popInput_MinValue" runat="server" Width="170" CssClass="TextBoxRequired"></asp:TextBox>
                                                <cc1:MaskedEditExtender ID="MaskedEditExtender_MinValue" runat="server" TargetControlID="popInput_MinValue" Mask="9999999999" PromptCharacter=""></cc1:MaskedEditExtender>
                                                <cc1:MaskedEditValidator ID="MaskedEditValidator_MinValue" runat="server" ControlToValidate="popInput_MinValue" ControlExtender="MaskedEditExtender_MinValue" Font-Size="Smaller"></cc1:MaskedEditValidator>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="popInput_MaxValue" runat="server" Width="170" CssClass="TextBoxRequired"></asp:TextBox>
                                                <cc1:MaskedEditExtender ID="MaskedEditExtender_MaxValue" runat="server" TargetControlID="popInput_MaxValue" Mask="9999999999" PromptCharacter=""></cc1:MaskedEditExtender>
                                                <cc1:MaskedEditValidator ID="MaskedEditValidator_MaxValue" runat="server" ControlToValidate="popInput_MaxValue" ControlExtender="MaskedEditExtender_MinValue" Font-Size="Smaller"></cc1:MaskedEditValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="div_Option" runat="server">
                                    <table>
                                        <tr>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="popLabel_ValueOptions" runat="server" Text="<%$ Resources:ResourceEquData,SelectItems %>"></asp:Label>
                                                <asp:Label ID="popLabel_ValueOptionstip" runat="server" Font-Size="13px" ForeColor=" #B5EDFD" Text="<%$Resources:ResourceEquData,ItemParaSetting %>"></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="popInput_ValueOptions" runat="server" Width="350" CssClass="TextBoxRequired"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="div_URL" runat="server">
                                    <table>
                                        <tr>
                                            <th colspan="2">
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="popLabel_EditFormURL" runat="server" Text="<%$Resources:ResourceEquData,EditUrl %>"></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:TextBox ID="popInput_EditFormURL" runat="server" Width="350" CssClass="TextBoxRequired"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="popLabel_Height" runat="server" Text="<%$Resources:ResourceEquData,EditHeight %>"></asp:Label>
                                            </th>
                                            <th>
                                                <span class="Arrow01"></span>
                                                <asp:Label ID="popLabel_Width" runat="server" Text="<%$Resources:ResourceEquData,EditWidth %>"></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="popInput_Height" runat="server" Width="170" CssClass="TextBoxRequired"></asp:TextBox>
                                                <cc1:MaskedEditExtender ID="MaskedEditExtender_Height" runat="server" TargetControlID="popInput_Height" Mask="9999" PromptCharacter=""></cc1:MaskedEditExtender>
                                                <cc1:MaskedEditValidator ID="MaskedEditValidator_Height" runat="server" ControlToValidate="popInput_Height" ControlExtender="MaskedEditExtender_Height" Font-Size="Smaller"></cc1:MaskedEditValidator>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="popInput_Width" runat="server" Width="170" CssClass="TextBoxRequired"></asp:TextBox>
                                                <cc1:MaskedEditExtender ID="MaskedEditExtender_Width" runat="server" TargetControlID="popInput_Width" Mask="9999" PromptCharacter=""></cc1:MaskedEditExtender>
                                                <cc1:MaskedEditValidator ID="MaskedEditValidator_Width" runat="server" ControlToValidate="popInput_Width" ControlExtender="MaskedEditExtender_Width" Font-Size="Smaller"></cc1:MaskedEditValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="popLabel_DefaultValue" runat="server" Text="<%$Resources:ResourceEquData,DefaultValue %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="popInput_DefaultValue" runat="server" Width="350" CssClass="TextBoxRequired"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
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
</asp:Content>
