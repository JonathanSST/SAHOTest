<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="NewsInfo.aspx.cs" Inherits="SahoAcs.NewsInfo" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="/uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>
<%@ Register Src="~/uc/MultiSelectDropDown.ascx" TagPrefix="uc2" TagName="MultiSelectDropDown" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="SortDataContext" name="SortDataContext" value="" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Query" />
        <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
        <input type="hidden" id="CardTimeS" name="CardTimeS" value="" />
        <input type="hidden" id="CardTimeE" name="CardTimeE" value="" />
        <input type="hidden" id="DateS" name="DateS" value="" />
        <input type="hidden" id="DateE" name="DateE" value="" />
        <input type="hidden" id="LogStatus" name="LogStatus" value="" />
        <input type="hidden" id="EquNo" name="EquNo" value="" />
        <input type="hidden" id="DepID" name="DepID" value="" />
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
        <input type="hidden" id="PsnID" name="PsnID" value="<%=this.PsnID %>" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="公告日期區間"></asp:Label>
                        </th>
                        <%--<th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="人員編號"></asp:Label>
                        </th>--%>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <uc2:CalendarFrm runat="server" ID="Calendar_CardTimeSDate" />
                        </td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc2:CalendarFrm runat="server" ID="Calendar_CardTimeEDate" />
                        </td>
                        <%--<td>
                            <input type="text" id="QueryNo" name="QueryName"/>
                        </td>--%>
                        <td>
                            <%--<asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />--%>
                            <input type="button" name="BtnQuery" id="BtnQuery" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetMode(1)" />
                            <input type="button" name="ButtonAdd" value="新增系統公告" id="ButtonAdd" class="IconNew" tabindex="0" onclick="SetAdd()" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width: 970px">
            <tbody>
                <tr class="GVStyle ui-sortable">
                    <th scope="col" class="TitleRow ui-sortable-handle" style="width: 100px;">瀏覽明細
                    </th>
                    <th scope="col" class="TitleRow ui-sortable-handle" style="width: 180px;">公告日期
                    </th>
                    <th scope="col" class="TitleRow ui-sortable-handle" style="width: 520px;">主旨
                    </th>
                    <th scope="col" class="TitleRow ui-sortable-handle">公告人員
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="11">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 970px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle ui-sortable" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody class="ui-sortable-handle">
                                        <%foreach (var o in this.ListLog)
                                            { %>
                                        <tr class="DataRow" id="GV_Row<%=o.NewsID %>" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)"
                                            onclick="SingleRowSelect(0, this, SelectValue,'<%=o.NewsID %>', '', '');" style="color: rgb(0, 0, 0);">
                                            <td style="width: 104px;text-align:center">
                                                <input type="button" name="BtnShow" id="BtnShow" class="IconLook" onclick="ShowData('<%=o.NewsID.ToString() %>')"  value="瀏覽"/>
                                            </td>
                                            <td style="width: 184px;">
                                                <%=string.Format("{0:yyyy/MM/dd}", o.NewsDate) %>
                                            </td>
                                            <td style="width: 525px;">
                                                <%=o.NewsTitle %>
                                            </td>
                                            <td>
                                                <%=o.CreateUserID %>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <br />
    <div>
        <input type="button" name="BtnEdit" value="編    輯" id="BtnEdit" class="IconEdit" onclick="SetEdit()" />
        <input type="button" name="BtnExport" value="匯出公告" id="BtnExport" class="IconExport" onclick="SetPrint()" />
    </div>
    <div id="PsnListContent" style="position: absolute; z-index: 100000">
    </div>
</asp:Content>
