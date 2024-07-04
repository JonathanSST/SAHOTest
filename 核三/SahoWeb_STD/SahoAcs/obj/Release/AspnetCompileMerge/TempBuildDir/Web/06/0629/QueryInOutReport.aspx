<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI"
    CodeBehind="QueryInOutReport.aspx.cs" Inherits="SahoAcs.Web.QueryInOutReport" %>

<%@ Import Namespace="SahoAcs.DBModel" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/CalendarFrm.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="~/uc/MultiSelectDropDown.ascx" TagPrefix="uc2" TagName="MultiSelectDropDown" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="EquDir" name="EquDir" value="進" />
        <input type="hidden" id="Range" name="Range" value="3" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th runat="server" id="ShowPsnInfo1">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardNo_PsnName" runat="server" Text="關鍵字(卡號、工號、姓名)"></asp:Label>
                        </th>
                        <th>日期
                        </th>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td runat="server" id="ShowPsnInfo2">
                            <input type="text" name="PsnNo" id="PsnNo" value="" style="width: 200px" />
                        </td>
                        <td>
                            <uc1:Calendar ID="Calendar1" runat="server" />
                        </td>
                        <td>
                            <input type="button" value="<%=Resources.Resource.btnQuery %>" class="IconSearch" id="BtnQuery" />
                        </td>
                        <td runat="server" id="ShowPsnInfo3"></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table class="TableS1" style="width: 1050px">
        <tbody>
            <tr class="GVStyle">
                <th scope="col" style="width: 120px;">讀卡時間</th>
                <th scope="col" style="width: 90px;">公司</th>
                <th scope="col" style="width: 90px;">工號</th>
                <th scope="col" style="width: 90px;">姓名</th>
                <th scope="col" style="width: 90px">卡號</th>
                <th scope="col" style="width: 40px;">進出</th>
                <th scope="col" style="width: 120px;">設備編號</th>
                <th scope="col" style="width: 120px">設備名稱</th>
                <th scope="col" style="">讀卡結果</th>
            </tr>
            <tr>
                <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                    <asp:HiddenField ID="HiddenField1" runat="server" EnableViewState="False" />
                    <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1050px; overflow-y: scroll;">
                        <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                            <tbody>
                                <%foreach (var o in this.logmaps)
                                    { %>
                                <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                    <td style="width: 124px"><%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",o.CardTime) %></td>
                                    <td style="width: 94px"><%=o.DepName %></td>
                                    <td style="width: 94px"><%=o.PsnNo %></td>
                                    <td style="width: 94px"><%=o.PsnName %></td>
                                    <td style="width: 94px"><%=o.CardNo %></td>
                                    <td style="width: 44px"><%=o.EquDir %></td>
                                    <td style="width: 124px"><%=o.EquNo %></td>
                                    <td style="width: 124px"><%=o.EquName %></td>
                                    <td><%=o.LogStatus %></td>
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                    </div>
                </td>
            </tr>
            <tr class="GVStyle">
                <th scope="col" style="width: 120px;">合計人次：</th>
                <th scope="col" style="width: 90px;" id="RowCount"><%=this.logmaps.Count %></th>
                <th scope="col" style="width: 90px;"></th>
                <th scope="col" style="width: 90px;"></th>
                <th scope="col" style="width: 90px"></th>
                <th scope="col" style="width: 40px;"></th>
                <th scope="col" style="width: 120px;"></th>
                <th scope="col" style="width: 120px;"></th>
                <th scope="col" style=""></th>
            </tr>
        </tbody>
    </table>
    <div>
        <asp:Button ID="ExportButton" runat="server" Text="匯　出" OnClick="ExportButton_Click" CssClass="IconExport" />
    </div>
</asp:Content>
