<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CardEquAdj2.aspx.cs" Inherits="SahoAcs.CardEquAdj2" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span><%=Resources.Resource.lblCondition %>：
                <asp:DropDownList ID="ddl_input" runat="server" Width="150px">
                    <asp:ListItem Value="Card">卡片(Card)</asp:ListItem>
                    <asp:ListItem Value="Org">組織(Organzation)</asp:ListItem>
                    <asp:ListItem Value="No">工號(Person No)</asp:ListItem>
                    <asp:ListItem Value="Name">姓名(Name)</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btSelectData" runat="server" Text="<%$ Resources:Resource, ddlSelectDefault %>" CssClass="IconChoose" />
            </th>
        </tr>
    </table>
    <table border="0" class="Item">
        <tr>
            <th style="width: 260px">
                <span class="Arrow01"></span><%=Resources.Resource.lblDataList %>：
            </th>
            <th>
                <span class="Arrow01"></span><%=Resources.Resource.lblAuthDataList %>：
                <asp:DropDownList ID="ddlEquType" runat="server" Width="100px">
                    <asp:ListItem Value="Equ">設備</asp:ListItem>
                    <asp:ListItem Value="EquGrp">設備群組</asp:ListItem>
                </asp:DropDownList>
            </th>
            <td>
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources:Resource,btnAuthData %>" OnClick="btnSearch_Click" CssClass="IconChoose" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <table>
                    <tr>
                        <td colspan="2" style="vertical-align: top;">
                            <asp:ListBox ID="DataList" runat="server" Height="540px" Width="240px"></asp:ListBox>
                        </td>
                        <td style="vertical-align: top;">
                            <table>
                                <tr>
                                    <td>
                                        <fieldset id="Pending_List" runat="server" style="width: 300px; height: 320px">
                                            <legend id="Pending_Legend" runat="server"><%=Resources.Resource.lblUnJoin %></legend>
                                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                <ContentTemplate>
                                                    <table class="TableS1">
                                                        <%--GridView Header的Html Code--%>
                                                        <asp:Literal ID="li_header2" runat="server" />
                                                        <tr>
                                                            <td id="td_showGridView2" runat="server" style="padding: 0">
                                                                <asp:Panel ID="tablePanel2" runat="server" ScrollBars="Vertical" Width="300px" Height="250px">
                                                                    <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                                                    <%-- 設備-待加入 --%>
                                                                    <asp:GridView runat="server" ID="GridView2" SkinID="GridViewSkin" PageSize="5"
                                                                        DataKeyNames="EquID" AutoGenerateColumns="False"
                                                                        OnRowDataBound="GridView2_Data_RowDataBound" OnDataBound="GridView2_Data_DataBound">
                                                                        <Columns>
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                                <HeaderTemplate>
                                                                                    <asp:CheckBox ID="CheckBox2" runat="server" onclick="spanChk2=this;SelectAllCheckboxes(this);" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="RowCheckState2" runat="server" onclick="spanChk2=ContentPlaceHolder1_GridView2_CheckBox2;GV2State();"></asp:CheckBox>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:EquNo %>" SortExpression="EquNo" />
                                                                            <asp:BoundField DataField="EquName" HeaderText="<%$Resources:EquName %>" SortExpression="EquName" />
                                                                            <asp:BoundField DataField="CardRule" HeaderText="<%$Resources:Rule %>" SortExpression="CardRule" />
                                                                            <asp:BoundField DataField="Floor" HeaderText="<%$Resources:Elevator %>" SortExpression="Floor" />
                                                                            <asp:BoundField DataField="EquID" HeaderText="ID" SortExpression="EquID" />
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </asp:Panel>
                                                                <asp:Panel ID="tablePanel2_Grp" runat="server" ScrollBars="Vertical" Width="300px" Height="250px" Visible="false">
                                                                    <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                                                    <%-- 設備群組-待加入 --%>
                                                                    <asp:GridView runat="server" ID="GridView4" SkinID="GridViewSkin" PageSize="5"
                                                                        DataKeyNames="EquID" AutoGenerateColumns="False"
                                                                        OnRowDataBound="GridView4_Data_RowDataBound" OnDataBound="GridView4_Data_DataBound">
                                                                        <Columns>
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                                <HeaderTemplate>
                                                                                    <asp:CheckBox ID="CheckBox4" runat="server" onclick="spanChk4=this;SelectAllCheckboxes(this);" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="RowCheckState4" runat="server" onclick="spanChk4=ContentPlaceHolder1_GridView4_CheckBox4;GV4State();"></asp:CheckBox>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>                                                                            
                                                                            <asp:BoundField DataField="EquNo" HeaderText="<%$ Resources:EquGroupNo %>" SortExpression="EquNo" />                                                                            
                                                                            <asp:BoundField DataField="EquName" HeaderText="<%$ Resources:EquGroupName %>" SortExpression="CardRule" />
                                                                            <asp:BoundField DataField="CardRule" HeaderText="<%$ Resources:Rule %>" SortExpression="CardRule" />
                                                                            <asp:BoundField DataField="Floor" HeaderText="樓層" SortExpression="Floor" />
                                                                            <asp:BoundField DataField="EquID" HeaderText="ID" SortExpression="EquID" />
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                        <%--GridView Pager的Html Code--%>
                                                        <asp:Literal ID="Literal2" runat="server" />
                                                    </table>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </fieldset>
                                    </td>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                            <ContentTemplate>
                                                <asp:Button ID="AddEquButton" runat="server" Text="右移" Font-Size="X-Small" OnClick="AddEquButton_Click" CssClass="IconRight" />
                                                <br />
                                                <br />
                                                <br />
                                                <asp:Button ID="RemoveEquButton" runat="server" Text="左移" Font-Size="X-Small" OnClick="RemoveEquButton_Click" CssClass="IconLeft" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                    <td>
                                        <fieldset id="Queue_List" runat="server" style="width: 550px; height: 320px">
                                            <legend id="Queue_Legend" runat="server"><%=Resources.Resource.lblJoin %></legend>
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <table class="TableS1">
                                                        <%--GridView Header的Html Code--%>
                                                        <asp:Literal ID="li_header1" runat="server" />
                                                        <tr>
                                                            <td id="td_showGridView1" runat="server" style="padding: 0">
                                                                <asp:Panel ID="tablePanel1" runat="server" ScrollBars="Vertical" Width="550px" Height="250px">
                                                                    <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                                                    <%-- 設備-已加入 --%>
                                                                    <asp:GridView runat="server" ID="GridView1" SkinID="GridViewSkin" PageSize="5"
                                                                        DataKeyNames="EquID" AutoGenerateColumns="False"
                                                                        OnRowDataBound="GridView1_Data_RowDataBound" OnDataBound="GridView1_Data_DataBound">
                                                                        <Columns>
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                                <HeaderTemplate>
                                                                                    <asp:CheckBox ID="CheckBox1" runat="server" onclick="spanChk1=this;SelectAllCheckboxes(this);" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="RowCheckState1" runat="server" onclick="spanChk1=ContentPlaceHolder1_GridView1_CheckBox1;GV1State();"></asp:CheckBox>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:EquNo %>" SortExpression="EquNo" />
                                                                            <asp:BoundField DataField="EquName" HeaderText="<%$Resources:EquName %>" SortExpression="EquName" />
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="CardRule" runat="server" Text="<%$Resources:Rule %>"></asp:Label>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:DropDownList ID="ddlCardRule" runat="server"></asp:DropDownList>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="Floor" runat="server" Text="<%$Resources:Elevator %>"></asp:Label>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Button ID="btFloor" runat="server" Text="樓層" CssClass="IconGroupSet" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="EquID" HeaderText="ID" SortExpression="EquID" />
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </asp:Panel>
                                                                <asp:Panel ID="tablePanel1_Grp" runat="server" ScrollBars="Vertical" Width="550px" Height="250px" Visible="false">
                                                                    <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                                                    <%-- 設備群組-已加入 --%>
                                                                    <asp:GridView runat="server" ID="GridView3" SkinID="GridViewSkin" PageSize="5"
                                                                        DataKeyNames="EquID" AutoGenerateColumns="False"
                                                                        OnRowDataBound="GridView3_Data_RowDataBound" OnDataBound="GridView3_Data_DataBound">
                                                                        <Columns>
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                                <HeaderTemplate>
                                                                                    <asp:CheckBox ID="CheckBox3" runat="server" onclick="spanChk3=this;SelectAllCheckboxes(this);" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="RowCheckState3" runat="server" onclick="spanChk3=ContentPlaceHolder1_GridView3_CheckBox3;GV3State();"></asp:CheckBox>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="EquNo" HeaderText="<%$ Resources:EquGroupNo %>" SortExpression="EquNo" />
                                                                            <asp:BoundField DataField="EquName" HeaderText="<%$Resources:EquGroupName %>" SortExpression="EquName" />
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="CardRule" runat="server" Text="<%$Resources:Rule %>"></asp:Label>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:DropDownList ID="ddlCardRule" runat="server"></asp:DropDownList>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="Floor" runat="server" Text="<%$Resources:Elevator %>"></asp:Label>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Button ID="btFloor" runat="server" Text="<%$Resources:Elevator %>" CssClass="IconGroupSet" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="EquID" HeaderText="ID" SortExpression="EquID" />
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                        <%--GridView Pager的Html Code--%>
                                                        <asp:Literal ID="li_Pager" runat="server" />
                                                    </table>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <table border="0" class="Item" style="margin-top: 10px">
                                            <tr>
                                                <th>
                                                    <span class="Arrow01"></span>
                                                    <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, lblProcessType %>"></asp:Label>
                                                </th>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:DropDownList ID="ddlOpType" Width="100px" runat="server">
                                                        <asp:ListItem Value="Add">增加</asp:ListItem>
                                                        <asp:ListItem Value="Del">減少</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td></td>
                                                <td>&nbsp;</td>
                                                <td>
                                                    <asp:Button ID="btStart" runat="server" Text="重整" OnClientClick="Block();" OnClick="btStart_Click" CssClass="IconRefresh" />
                                                    <asp:Button ID="btRemove" runat="server" Text="清除電梯樓層" OnClientClick="Block();" OnClick="btRemove_Click" CssClass="IconRefresh" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <table border="0" class="Item" style="width: 100%">
                                            <tr>
                                                <th>
                                                    <span class="Arrow01"></span><%=Resources.Resource.lblMsg %>：
                                                </th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <asp:ListBox ID="List_Msg" runat="server" Width="500px" Height="100px"></asp:ListBox>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hUserId" runat="server" />
    <asp:HiddenField ID="hAddData" runat="server" />
    <asp:HiddenField ID="hRemoveData" runat="server" />
    <asp:HiddenField ID="hddlState" runat="server" />
    <asp:HiddenField ID="hFloorData" runat="server" />
    <asp:HiddenField ID="hFinalFloorData" runat="server" />
    <asp:HiddenField ID="hSelectState" runat="server" />
    <asp:HiddenField ID="hFinalEquData" runat="server" />
    <asp:HiddenField ID="hDataList" runat="server" />
    <!--彈出視窗-->
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="530px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
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
                <td class="auto-style1">
                    <table>
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OrgList" runat="server" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <asp:TextBox ID="popTxt_Query" runat="server"></asp:TextBox>
                                <asp:Button ID="popB_Query" runat="server" Text="<%$Resources:Resource,btnQuery %>" EnableViewState="False" CssClass="IconSearch" />
                                <font style="color: #FF0000; font-size: 16px">※查詢條件不得為空※</font>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: center">
                                <asp:ListBox ID="popB_CardList1" runat="server" Width="200px" Height="150px" SelectionMode="Multiple" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                            <td colspan="2" style="text-align: center">
                                <asp:Button ID="popB_Enter1" runat="server" Text="<%$ Resources:Resource, btnJoin%>" CssClass="IconRight" />
                                <br />
                                <br />
                                <asp:Button ID="popB_Remove1" runat="server" Text="<%$ Resources:Resource, btnRemove%>" CssClass="IconLeft" />
                            </td>
                            <td style="text-align: center">
                                <asp:ListBox ID="popB_CardList2" runat="server" Width="200px" Height="150px" SelectionMode="Multiple" SkinID="ListBoxSkin"></asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="text-align: center">
                                <asp:Label ID="DeleteLableText1" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="auto-style1">
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_OK1" runat="server" Text="<%$ Resources:Resource, btnOK %>" CssClass="IconOk" />
                        <asp:Button ID="popB_Cancel1" runat="server" Text="<%$ Resources:Resource, btnCancel %>" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
        <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
        <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
    </asp:Panel>
</asp:Content>

