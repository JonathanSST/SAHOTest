<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="ChangePWD.aspx.cs" Inherits="SahoAcs.ChangePWD" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--  cellspacing="0"   border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死--%>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hidePsnID" runat="server" EnableViewState="False" />
        <input type="hidden" id="msgSuccess" value="<%=this.GetLocalResourceObject("msgSuccess") %>" />
    </div>
    <table class="Item">
        <tr>
            <th style="text-align:left">
                <span class="Arrow01"></span>
                <asp:Label ID="Label_OldPWD" runat="server" Text="<%$Resources:ttOldPWD %>"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="Input_OldPWD" runat="server" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th style="text-align:left">
                <span class="Arrow01"></span>
                <asp:Label ID="Label_NewPWD" runat="server" Text="<%$Resources:ttNewPWD %>"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="Input_NewPWD" runat="server" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th style="text-align:left">
                <span class="Arrow01"></span>
                <asp:Label ID="Label_CheckPWD" runat="server" Text="<%$Resources:ttCheckPWD %>"></asp:Label>
            </th>
            <td>
                <asp:TextBox ID="Input_CheckPWD" runat="server" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th colspan="2">
                <asp:Label ID="Label_Remind" runat="server" Text="<%$Resources:ttRule %>"></asp:Label>
            </th>
        </tr>
    </table>
    <asp:Button ID="SaveButton" runat="server" Text="<%$Resources:Resource,btnSave %>" CssClass="IconSave" />
</asp:Content>
