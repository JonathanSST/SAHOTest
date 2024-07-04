<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"  Theme="UI"    
    CodeBehind="ModifybackQuery.aspx.cs" Inherits="SahoAcs.ModifybackQuery" %>

<%@ Register Src="~/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>
<%@ Register Src="~/uc/MultiSelectDropDown.ascx" TagPrefix="uc1" TagName="MultiSelectDropDown" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <th>
                <asp:Label ID="Label_BackupTimeBegin" runat="server" Text="開始時間"></asp:Label>
            </th>
            <th>
                <asp:Label ID="Label_BackupTimeEnd" runat="server" Text="結束時間"></asp:Label>
            </th>
            <th>
                <asp:Label ID="Label_TableFrom" runat="server" Text="查詢來源"></asp:Label>
            </th>
            <th>
                <asp:Label ID="Label_ModifyMode" runat="server" Text="動作類型"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <%--<uc1:Calendar runat="server" id="Input_BackupTimeBegin"  DateValue="2014/03/05 00:00:00"/>--%>
                <uc1:Calendar runat="server" ID="Input_BackupTimeBegin" />
            </td>
            <td>
               <%-- <uc1:Calendar runat="server" ID="Input_BackupTimeEnd"  DateValue="2014/03/20 00:00:00" />--%>
                <uc1:Calendar runat="server" ID="Input_BackupTimeEnd" />
            </td>
            <td>
                <uc1:MultiSelectDropDown runat="server" ID="Input_TableFrom" />
                <%--<asp:DropDownList ID="Input_TableFrom" runat="server"></asp:DropDownList>--%>
            </td>
            <td>
                <asp:DropDownList ID="Input_ModifyMode" runat="server">
                    <asp:ListItem Text="- 全部 -" Value=""></asp:ListItem>
                    <asp:ListItem Text="編輯" Value="M"></asp:ListItem>
                    <asp:ListItem Text="刪除" Value="D"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="查詢" OnClick="QueryButton_Click" /></td>
        </tr>
    </table>
    <asp:UpdatePanel ID="ModifyInfo_UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Literal ID="ModifyInfo" runat="server"></asp:Literal>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
