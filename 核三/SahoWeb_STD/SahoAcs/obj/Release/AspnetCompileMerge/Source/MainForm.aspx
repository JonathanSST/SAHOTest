<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MainForm.aspx.cs" Inherits="SahoAcs.MainForm"  Theme="UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <script type="text/javascript">  
        $(document).ready(function () {
            DisplayTime();
        });
    </script>   
</asp:Content>
