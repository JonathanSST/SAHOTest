<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="Documents.aspx.cs"
    Inherits="SahoAcs.Documents" Debug="true" EnableEventValidation="false" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="position: relative; top: 10px;">
        <span style="font-size: 1cm; color: #FF0000">參考文件清單</span><br />        
            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
        <table style="width:600px">            
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                <td style="font-size: 0.6cm; color: white; background: #000669"><%#DataBinder.Eval(Container.DataItem,"DocName").ToString() %>
                </td>
                <td style="font-size: 0.6cm; color: white; background: #000669; width:120px">
                    <asp:LinkButton ID="FileLink" runat="server" ForeColor="White" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"DocKey")%>' Font-Underline="false" OnClick="FileLink_Click">下載</asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td></td>                
            </tr>
                </ItemTemplate>                
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>                                                
        <%--<table style="width:400px">
            <tr>
                <td style="font-size: 0.6cm; color: white; background: #000669">商合行門禁管理系統 - 安裝手冊
                </td>
                <td style="font-size: 0.6cm; color: white; background: #000669">
                    <asp:LinkButton ID="File_1" runat="server" ForeColor="White" Font-Underline="false" OnClick="File_1_Click">下載</asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td></td>                
            </tr>
            <tr>
                <td style="font-size: 0.6cm; color: white; background: #000782">商合行門禁管理系統 - 操作手冊
                </td>
                <td style="font-size: 0.6cm; color: white; background: #000782">
                    <asp:LinkButton ID="File_2" runat="server" ForeColor="White" Font-Underline="false" OnClick="File_2_Click">下載</asp:LinkButton>
                </td>
            </tr>
        </table>--%>
    </div>
</asp:Content>
