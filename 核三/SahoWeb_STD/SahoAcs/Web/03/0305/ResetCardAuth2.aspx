<%@ Page Language="C#" Inherits="SahoAcs.ResetCardAuth2" MasterPageFile="~/Site1.Master" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" CodeBehind="ResetCardAuth2.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="Label_CardType" runat="server" Text="<%$Resources:lblCardType %>"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:DropDownList ID="Input_CardType" runat="server" OnSelectedIndexChanged="Input_CardType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="ResetAuthButton" runat="server" Text="<%$Resources:btnRefresh %>" OnClientClick="Block();SetCardAuthProcBach();return false" CssClass="IconRefresh" />
            </td>
        </tr>
    </table>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:UpdatePanel ID="PersonListUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                            <fieldset id="PersonList_Fieldset" runat="server" style="width: 700px; height: 450px">
                                <legend id="PersonList_Legend" runat="server">
                                    <asp:Label ID="lblPersonList" runat="server" Text="卡片權限重整清單(共0筆)"></asp:Label>
                                </legend>
                                
                                        <asp:Panel ID="PersonListPanel" runat="server" Height="440px" ScrollBars="Auto">
                                            <asp:Table ID="PersonList" runat="server"></asp:Table>
                                        </asp:Panel>                                    
                            </fieldset>
                                        </ContentTemplate>
                                </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>            
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <fieldset id="MsgResult_Fieldset" runat="server" style="width: 700px; height: 150px">
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
       <%-- <tr>
            <td>
                <fieldset id="Fieldset1" runat="server" style="width: 700px;">
                    <legend id="Legend1" runat="server">重整失敗卡號</legend>
                    <table class="TableS1" style="width: 700px">
                <tbody>
                    <tr class="GVStyle">
                        <th scope="col" style="width: 90px;">姓名</th>
                        <th scope="col" style="width: 90px;">人員編號</th>
                        <th scope="col" style="width: 90px;">卡號</th>
                        <th scope="col" style="width:90px">卡別</th>                        
                        <th scope="col">重置</th>
                    </tr>
                    <tr>
                        <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                            <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
                            <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 700px;height:160px; overflow-y: scroll;">
                                <div>
                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                        <tbody>                                            
                                        </tbody>
                                    </table>                                    
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>      
                </fieldset>
            </td>
        </tr>--%>
    </table>    
    <asp:HiddenField ID="hideUserID" runat="server" />
    <asp:HiddenField ID="hOwnerList" runat="server" />
    <input type="hidden" value="<%=GetLocalResourceObject("lblRefreshFail") %>" id="lblError" />
    <asp:HiddenField ID="hSelectValue" runat="server" EnableViewState="False" />
</asp:Content>
