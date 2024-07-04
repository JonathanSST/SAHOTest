<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DoBackup.aspx.cs" Inherits="SahoAcs.Web._98._9823.DoBackup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="TableWidth">
        <tr>
            <td>
                <fieldset id="Holiday_Fieldset" runat="server" style="width: 1190px; height: 320px">
                    <legend id="Holiday_Legend" runat="server">資料庫備份作業</legend>
                    <input type="button" id="BtnBackUp" name="BtnBackUp"/>
                </fieldset>
            </td>
        </tr>
    </table>
</asp:Content>
