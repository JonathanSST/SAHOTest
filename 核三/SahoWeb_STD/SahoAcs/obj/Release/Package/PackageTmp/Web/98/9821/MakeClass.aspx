<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="MakeClass.aspx.cs" Inherits="SahoAcs.MakeClass" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/PickDate.ascx" TagPrefix="uc1" TagName="PickDate" %>

<%@ Register src="../../../uc/CalendarFrm.ascx" tagname="CalendarFrm" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--  cellspacing="0"   border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死--%>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    </div>
    <table class="TableWidth">
        <tr>
            <td>
                <fieldset id="Holiday_Fieldset" runat="server" style="width: 1190px; height: 320px">
                    <legend id="Holiday_Legend" runat="server"><%=this.GetLocalResourceObject("ttSysHolidayList").ToString() %></legend>
                    <asp:UpdatePanel ID="HolidayUpdatePanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="HolidayPanel" runat="server" ScrollBars="Vertical" Height="300px"  CssClass="TableS2">
                                <asp:Table ID="HolidayTable" runat="server"></asp:Table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
            </td>
        </tr>
    </table>
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="Label_Date" runat="server" Text="<%$Resources:ttSysHoliday %>"></asp:Label>
            </th>
            <td>
                <uc2:CalendarFrm ID="PickDate_Holiday" runat="server" />
            </td>
            <th>
                 <span class="Arrow01"></span>
                <asp:Label ID="Label_DayOfWeek" runat="server" Text="<%$Resources:ttWorkday %>"></asp:Label>
            </th>
            <td>
                <select id="TimeDay" name="TimeDay" style="width:100px" class="DropDownListStyle">
                    <%for (int i = 1; i <= 7; i++)
                        { %>
                            <option value="<%=string.Format("{0:00}",i) %>"><%=this.ListWeek[i-1] %></option>
                    <%} %>
                </select>
            </td>
            <td style="padding-left:15px">
                <asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew"/>
                <asp:Button ID="DeleteButton" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete"/>
               <%-- <input type="button" value="<%=this.GetLocalResourceObject("btnSetCode") %>" onclick="SetCode()" class="IconSet" />--%>
           </td>
        </tr>
    </table>
</asp:Content>
