<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EquMapAlive.aspx.cs" Inherits="SahoAcs.Web._06._0640.EquMapAlive" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:DropDownList ID="ddlMapList" runat="server" CssClass="DropDownListStyle" AutoPostBack="true" ></asp:DropDownList>
    <div id="MapDiv">

    </div>
    <table>
        <tr>
            <td>
                <%--<asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>
                
            </td>
        </tr>
    </table>    
    <input type="hidden" name="PageEvent" value="Save" />
    <input type="hidden" name="PageMapSrc" id="PageMapSrc" value="<%=this.MapSrc %>" />
</asp:Content>
