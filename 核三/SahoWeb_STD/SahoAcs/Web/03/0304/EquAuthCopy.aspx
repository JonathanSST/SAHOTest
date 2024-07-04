<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EquAuthCopy.aspx.cs" Inherits="SahoAcs.EquAuthCopy" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_Type" runat="server" Text="<%$Resources:lblEquType %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_No" runat="server" Text="<%$Resources:lblEquNo %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_Name" runat="server" Text="<%$Resources:lblEquName %>"></asp:Label>
                        </th>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="Input_Type" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_No" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_Name" runat="server"></asp:TextBox>
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
                <fieldset id="Source_List" runat="server" style="width: 425px">                    
                    <asp:UpdatePanel ID="SourceUpdatePanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <legend id="Source_Legend" runat="server">
                        <asp:Label ID="lblSource" runat="server" Text="來源設備(共0筆)"></asp:Label>
                    </legend>
                            <table class="GVStyle">
                                <%--GridView Header的Html Code--%>
                                <asp:Literal ID="li_Sourceheader" runat="server" />
                                <tr>
                                    <td id="td_showSourceGridView" runat="server" style="padding: 0">
                                        <asp:Panel ID="SourcePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                                            <asp:GridView runat="server" ID="SourceGridView" Width="100%" PageSize="5"
                                                DataKeyNames="EquNo" AutoGenerateColumns="False"
                                                OnRowDataBound="SourceGridView_Data_RowDataBound" OnDataBound="SourceGridView_Data_DataBound">
                                                <Columns>
                                                    <asp:BoundField DataField="Building" HeaderText="<%$Resources:ttBuild %>" SortExpression="Building" />
                                                    <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:lblEquNo %>" SortExpression="EquNo" />
                                                    <asp:BoundField DataField="EquName" HeaderText="<%$Resources:lblEquName %>" SortExpression="EquName" />
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
                        <asp:Label ID="lblTarget" runat="server" Text="目的設備(共0筆)"></asp:Label>
                    </legend>
                            <table class="GVStyle">
                                <%--GridView Header的Html Code--%>
                                <asp:Literal ID="li_Targetheader" runat="server" />
                                <tr>
                                    <td id="td_showTargetGridView" runat="server" style="padding: 0">
                                        <asp:Panel ID="TargetPanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                                            <asp:GridView runat="server" ID="TargetGridView" Width="100%" PageSize="5"
                                                DataKeyNames="EquNo" AutoGenerateColumns="False"
                                                OnRowDataBound="TargetGridView_Data_RowDataBound" OnDataBound="TargetGridView_Data_DataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="<%$Resources:ttCtrl %>" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="SelectControl" runat="server" />
                                                            <input id="EquID" name="EquID" type="hidden" value="<%#DataBinder.Eval(Container.DataItem, "EquID") %>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Building" HeaderText="<%$Resources:ttBuild %>" SortExpression="Building" />
                                                    <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:lblEquNo %>" SortExpression="EquNo" />
                                                    <asp:BoundField DataField="EquName" HeaderText="<%$Resources:lblEquName %>" SortExpression="EquName" />
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
    </table>
    <hr style="border: 0; height: 2px; background-color: #fff;" />
    <table>
        <tr>
            <td colspan="2">
                <table>
                    <tr>
                        <td>
                            <fieldset id="PersonList_Fieldset" runat="server" style="width: 925px; height: 150px">                                
                                <asp:UpdatePanel ID="PersonListUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <legend id="PersonList_Legend" runat="server">
                                    <asp:Label ID="lblPersonList" runat="server" Text="權限複製清單(共0筆)"></asp:Label>
                                </legend>
                                        <asp:Panel ID="PersonListPanel" runat="server" ScrollBars="Auto" Height="140px">
                                            <asp:Table ID="PersonList" runat="server"></asp:Table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="width: 600px">
                <table>
                    <tr>
                        <td>
                            <fieldset id="MsgResult_Fieldset" runat="server" style="width: 700px;">
                                <legend id="MsgResult_Legend" runat="server"><%=GetLocalResourceObject("lblResult") %></legend>
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
                            <input type="radio" id="Input_AuthAct1" value="Cover" name="AuthActType" />
                            <label for="Input_AuthAct1"><%=this.GetLocalResourceObject("AppendAuth") %></label>
                        </th>
                    </tr>
                    <tr>
                        <th>                           
                            <input type="radio" id="Input_AuthAct2" value="Add" name="AuthActType" />
                            <label for="Input_AuthAct2"><%=GetLocalResourceObject("CoverAuth") %></label>
                            <blockquote style="margin-left: 20px; margin-top: 5px; margin-bottom: 10px;">
                                <input type="radio" id="ConlictAct1" name="ConflictType" value="SourceValue" />
                                <label for="ConlictAct1"><%=GetLocalResourceObject("UseSource") %></label>
                                <input type="radio" id="ConflictAct2" name="ConflictType" value="TargetValue" />
                                <label for="ConflictAct2"><%=GetLocalResourceObject("UseTarget") %></label>
                            </blockquote>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="AuthCopyButton" runat="server" Text="<%$Resources:btnReplaceAuth %>" Width="100%" OnClientClick="SetAuthCopy();return false" CssClass="IconCopyPermissions" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

