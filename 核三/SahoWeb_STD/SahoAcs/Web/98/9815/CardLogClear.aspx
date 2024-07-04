<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    EnableEventValidation="false" Theme="UI" Debug="true" CodeBehind="CardLogClear.aspx.cs" Inherits="SahoAcs.CardLogClear" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <%--<th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_Company" runat="server" Text="英文姓名"></asp:Label>
                        </th>--%>
                        <th runat="server" id="Th1" colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_Date" runat="server" Text="日期"></asp:Label>
                        </th>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <%--<td>
                            <asp:TextBox ID="txtPsnEName" runat="server" Text="" CssClass="TextBoxRequired"></asp:TextBox>
                        </td>--%>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeSDate" />
                        </td>
                        <td style="font-size: 16px; color: white">至</td>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeEDate" />
                        </td>
                        <td>
                            <%--<asp:Button ID="QueryButton" runat="server" Text="查　詢" CssClass="IconSearch" OnClick="QueryButtonButton_Click" />--%>
                            <input type="button" value="查詢" id="QueryButton" class="IconSearch" onclick="SetQuery()" />
                            <input type="button" value="刪除" id="DeleteButton" class="IconDelete" onclick="SetDelete()" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table class="TableS1" style="width: 1050px">
        <tbody>
            <tr class="GVStyle">
                <th scope="col" style="width: 30px;">
                    <input style="width:25px;height:25px" type="checkbox" id="ChkAll" onchange="CheckAll()" /></th>
                <th scope="col" style="width: 90px;">姓名</th>
                <th scope="col" style="width: 90px;">卡號</th>
                <th scope="col" style="width: 90px;">人員編號</th>
                <th scope="col" style="width: 120px;">刷卡時間</th>
                <th scope="col" style="width: 100px;">設備名稱</th>
                <th scope="col" style="width: 100px;">設備編號</th>
                <th scope="col">狀態</th>
            </tr>
            <tr>
                <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                    <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1050px; height: 300px; overflow-y: scroll;">
                        <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                            <tbody>
                                <%foreach (var log in this.logs)
                                  { %>
                                <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                    <td style="width: 34px; text-align: center">
                                        <input style="width:25px;height:25px" type="checkbox" id="ChkOne" name="ChkOne" value="<%=log.RecordId %>" /></td>
                                    <td style="width: 94px;"><%=log.PsnName %></td>
                                    <td style="width: 94px;"><%=log.CardNo %></td>
                                    <td style="width: 94px;"><%=log.PsnNo %></td>
                                    <td style="width: 124px;"><%=log.CardTime.ToString("yyyy/MM/dd HH:mm:ss") %></td>
                                    <td style="width: 104px;"><%=log.EquName %></td>
                                    <td style="width: 104px;"><%=log.EquNo %></td>
                                    <td>未授權</td>
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
    <div>
    </div>
</asp:Content>
