<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeFile="LimitTextSample.aspx.cs" CodeBehind="LimitTextSample.aspx.cs" Inherits="SahoAcs.Web.Sample.LimitTextSample" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    欄位長度 : 
    <asp:TextBox ID="TextBox1" runat="server" Width="100px"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" Text="設定" OnClick="Button1_Click" />
    <br />
    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    <br />
    <br />
    <asp:Table ID="Table1" runat="server" CellSpacing="0"></asp:Table>
</asp:Content>
