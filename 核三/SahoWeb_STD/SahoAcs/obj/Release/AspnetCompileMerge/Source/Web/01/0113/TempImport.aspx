<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master"  CodeBehind="TempImport.aspx.cs" Inherits="SahoAcs.TempImport" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--  cellspacing="0"   border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死--%>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideErrorData" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <fieldset id="PsnCard_List" runat="server" style="width: 880px">
                    <legend id="PsnCard_Legend" runat="server">臨時人員資料匯入</legend>
                    <table>
                        <tr>
                            <td>
                                <asp:FileUpload ID="FileUpload_PsnCard" runat="server" Width="300px" Font-Size="20px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_PsnCard" runat="server" Text="匯入測試資料表" OnClick="UpLoadButton_PsnCard_Click" CssClass="IconImport" />
                            </td>
                            <td>
                                <asp:Button ID="CleanTable_PsnCard" runat="server" Text="清空資料表" OnClick="CleanTable_PsnCard_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="PsnCardState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_PsnCardState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                    <table class="TableWidth">
                        <tr>
                            <td style="text-align:right" colspan="5">
                                <asp:Button ID="PsnCard_ImportButton" runat="server" Text="確定匯入" OnClick="PsnCard_ImportButton_Click" CssClass="IconImport"/>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <th>
                <hr />
                   <asp:Label ID="Label_ShowMsg" runat="server" Text="訊息查看：" Font-Bold="true"></asp:Label>
            </th>
        </tr>
        <tr>
            <td style="text-align:left; vertical-align:,middle">
                <asp:UpdatePanel ID="EquMsg_UpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="TextBox_EquMsg" runat="server" Width="100%" Height="120px" TextMode="MultiLine"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>


