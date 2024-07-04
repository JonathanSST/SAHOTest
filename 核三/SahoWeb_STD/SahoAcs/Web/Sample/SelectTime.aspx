<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeFile="SelectTime.aspx.cs" CodeBehind="SelectTime.aspx.cs" Inherits="SahoAcs.Web.Sample.SelectTime" Theme="UI" %>

<%@ Register Src="~/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="開始時間:"></asp:Label>

            </td>
            <td>
                <uc1:Calendar runat="server" ID="StartTime" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="結束時間:"></asp:Label>

            </td>
            <td>
                <uc1:Calendar runat="server" ID="EndTime" />
            </td>
        </tr>


        <tr>
            <td>
                <asp:Label ID="Label3" runat="server" Text="回設時間:"></asp:Label>

            </td>
            <td>
                <uc1:Calendar runat="server" ID="SetTime" />
            </td>
        </tr>


        <tr>
            <td>
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />

            </td>
        </tr>
    </table>
</asp:Content>
