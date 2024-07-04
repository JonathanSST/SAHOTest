<%@ Page Language="C#" Inherits="SahoAcs.CopyCardAuth" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" CodeBehind="CopyCardAuth.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SourcePanelScrollValue" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <th>
                                        <span class="Arrow01"></span>
                                    </th>
                                    <th>
                                        <asp:Label ID="Label_SourceType" runat="server" Text="<%$Resources:lblCardType %>"></asp:Label>
                                    </th>
                                    <th></th>

                                    <th>
                                        <span class="Arrow01"></span>
                                    </th>
                                    <th>
                                        <asp:Label ID="Label_Condition1" runat="server" Text="<%$Resources:lblCondition %>"></asp:Label>
                                    </th>
                                    <th></th>

                                    <th></th>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="Input_SourceType" runat="server">
                                        </asp:DropDownList></td>
                                    <td></td>

                                    <td></td>
                                    <td style="width:220px">
                                        <asp:TextBox ID="Input_Condition1" runat="server" Width="95%"></asp:TextBox></td>
                                    <td></td>

                                    <td>
                                        <asp:Button ID="SourceQueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td></td>
            <td>
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <th>
                                        <span class="Arrow01"></span>
                                    </th>
                                    <th>
                                        <asp:Label ID="Label_Condition2" runat="server" Text="<%$Resources:lblCondition %>"></asp:Label>
                                    </th>
                                    <th></th>
                                    <th></th>

                                </tr>
                                <tr>
                                    <td></td>
                                    <td style="width:220px">
                                        <asp:TextBox ID="Input_Condition2" runat="server" Width="95%"></asp:TextBox>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="TargetQueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <fieldset id="Source_List" runat="server" style="width: 435px">                    
                    <asp:UpdatePanel ID="SourceUpdatePanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <legend id="Source_Legend" runat="server">
                        <asp:Label ID="lblSource" runat="server" Text="來源卡片(共0筆)"></asp:Label>
                    </legend>
                            <table class="GVStyle">
                                <%--GridView Header的Html Code--%>
                                <asp:Literal ID="li_Sourceheader" runat="server" />
                                <tr>
                                    <td id="td_showSourceGridView" runat="server" style="padding: 0">
                                        <asp:Panel ID="SourcePanel" runat="server" ScrollBars="Vertical" Height="250px">
                                            <asp:GridView runat="server" ID="SourceGridView" PageSize="5"
                                                DataKeyNames="CardNo" AutoGenerateColumns="False" Width="415px"
                                                OnRowDataBound="SourceGridView_Data_RowDataBound" OnDataBound="SourceGridView_Data_DataBound">
                                                <Columns>
                                                    <asp:BoundField DataField="CardNo" HeaderText="<%$Resources:lblCardNo %>" SortExpression="CardNo" >
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="PsnNo" HeaderText="<%$Resources:ttPsnNo %>" SortExpression="PsnNo" >
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="PsnName" HeaderText="<%$Resources:ttPsnName %>" SortExpression="PsnName" >
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                        </asp:Panel>
                                        
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    
                </fieldset>
            </td>
            <td style="width: 20px">&nbsp;</td>
            <td>
                <fieldset id="Target_List" runat="server" style="width: 465px">                    
                    <asp:UpdatePanel ID="TargetUpdatePanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <legend id="Target_Legend" runat="server">
                        <asp:Label ID="lblTarget" runat="server" Text="目的卡片(共0筆)"></asp:Label>
                    </legend>
                            <table class="GVStyle">
                                <%--GridView Header的Html Code--%>
                                <asp:Literal ID="li_Targetheader" runat="server" />
                                <tr>
                                    <td id="td_showTargetGridView" runat="server" style="padding: 0">
                                        <asp:Panel ID="TargetPanel" runat="server" ScrollBars="Vertical" Height="250px">
                                            <asp:GridView runat="server" ID="TargetGridView" PageSize="5"
                                                DataKeyNames="CardNo" AutoGenerateColumns="False" Width="445px"
                                                OnRowDataBound="TargetGridView_Data_RowDataBound" OnDataBound="TargetGridView_Data_DataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="<%$Resources:ttCtrl %>" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="SelectControl" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="CardNo" HeaderText="<%$Resources:lblCardNo %>" SortExpression="CardNo" />
                                                    <asp:BoundField DataField="PsnNo" HeaderText="<%$Resources:ttPsnNo %>" SortExpression="PsnNo" />
                                                    <asp:BoundField DataField="PsnName" HeaderText="<%$Resources:ttPsnName %>" SortExpression="PsnName" />
                                                </Columns>
                                            </asp:GridView>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnShowDetail" runat="server" Text="<%$Resources:lblAuthDetail %>" CssClass="IconSearch" />
            </td>
            <td>

            </td>
            <td>

            </td>
        </tr>
    </table>
    <hr style="border: 0; height: 2px; background-color: #fff;" />
    <table id="tbAuth" runat="server">
        <tr>
            <td colspan="2">
                <table>
                    <tr>
                        <td>
                            <fieldset id="EquList_Fieldset" runat="server" style="width: 580px; height: 150px">                                
                                <asp:UpdatePanel ID="EquListUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <legend id="EquList_Legend" runat="server">
                                    <asp:Label ID="lblEquList" runat="server" Text="權限複製清單(共0筆)"></asp:Label>
                                </legend>
                                        <asp:Panel ID="EquListPanel" runat="server" ScrollBars="Auto" Height="140px">
                                            <asp:Table ID="EquList" runat="server"></asp:Table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </td>

                        <td>
                            <fieldset id="Fieldset1" runat="server" style="width: 290px; height: 150px">                                
                                <asp:UpdatePanel ID="EquGroupListUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <legend id="CardEquGroupLegend" runat="server">
                                    <asp:Label ID="lblEquGroupList" runat="server" Text="設備群組權限複製清單(共0筆)"></asp:Label>
                                </legend>
                                        <asp:Panel ID="EquGroupListPanel" runat="server" ScrollBars="Auto" Height="140px">
                                            <asp:Table ID="EquGroupList" runat="server"></asp:Table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <fieldset id="MsgResult_Fieldset" runat="server" style="width: 650px;">
                                <legend id="MsgResult_Legend" runat="server"><%=GetLocalResourceObject("lblResult").ToString() %></legend>
                                <asp:UpdatePanel ID="MsgResultUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="MsgResultPanel" runat="server" Height="140px" ScrollBars="Auto">
                                            <asp:TextBox ID="MsgResultTextBox" runat="server" Width="99%" Height="95%" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="vertical-align: bottom">
                <table class="popItem">
                    <tr>
                        <th>
                            <asp:Button ID="AuthCopyButton" runat="server" Text="<%$Resources:btnReplaceAuth %>" OnClick="AuthCopyButton_Click" cssclass="IconCopyPermissions" />
                        </th>
                    </tr>
                    <tr>
                        <th>
                            <asp:Button ID="AuthAddButton" runat="server" Text="<%$Resources:btnAdditionAuth %>" OnClick="AuthAddButton_Click" cssclass="IconCopyPermissions" />
                        </th>

                        <th>
                            <asp:RadioButton ID="Input_ConflictAct1" runat="server" Text="<%$Resources:rbtParaNew %>" GroupName="ConflictType" Checked="True" /><br />
                            <asp:RadioButton ID="Input_ConflictAct2" runat="server" Text="<%$Resources:rbtParaOld %>" GroupName="ConflictType" />
                        </th>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

