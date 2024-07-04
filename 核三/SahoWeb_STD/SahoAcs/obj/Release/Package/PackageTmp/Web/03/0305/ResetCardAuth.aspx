<%@ Page Language="C#" Inherits="SahoAcs.ResetCardAuth" MasterPageFile="~/Site1.Master" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" CodeBehind="ResetCardAuth.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="Label_CardType" runat="server" Text="重整卡片類型"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:DropDownList ID="Input_CardType" runat="server" OnSelectedIndexChanged="Input_CardType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="ResetAuthButton" runat="server" Text="權限重整" OnClientClick="Block()" OnClick="ResetAuthButton_Click" CssClass="IconRefresh" />
            </td>
        </tr>
    </table>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <fieldset id="PersonList_Fieldset" runat="server" style="width: 700px; height: 450px">
                                <legend id="PersonList_Legend" runat="server">
                                    <asp:Label ID="lblPersonList" runat="server" Text="卡片權限重整清單(共0筆)"></asp:Label>
                                </legend>
                                <asp:UpdatePanel ID="PersonListUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PersonListPanel" runat="server" Height="440px" ScrollBars="Auto">
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
    </table>
    <table>
        <tr>
            <td>
                <fieldset id="MsgResult_Fieldset" runat="server" style="width: 700px; height: 150px">
                    <legend id="MsgResult_Legend" runat="server">重整結果</legend>
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
    <asp:HiddenField ID="hideUserID" runat="server" />
    <asp:HiddenField ID="hOwnerList" runat="server" />
    <asp:HiddenField ID="hSelectValue" runat="server" EnableViewState="False" />
</asp:Content>
