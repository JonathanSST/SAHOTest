<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryPsnOverTime.aspx.cs" Inherits="SahoAcs.QueryPsnOverTime" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%--<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>--%>
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
        <input type="hidden" id="CardDate" name="CardDate" value="" />
        <input type="hidden" id="LogTime" name="LogTime" value="" />
        <input type="hidden" id="EquNo" name="EquNo" value="" />
        <input type="hidden" id="DepNo" name="DepNo" value="" />
        <input type="hidden" id="MgaName" name="MgaName" value="" />
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <input type="hidden" id="SortName" name="SortName" value="" />
        <input type="hidden" id="SortType" name="SortType" value="" />
        <input type="hidden" id="CardType" name="CardType" value="" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="查詢時間"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="卡片類型"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label3" runat="server" Text="在廠時間"></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTime" />
                        </td>
                        <td>
                            <uc2:MultiSelectDropDown runat="server" ID="DropDownList_CardType" ListWidth="150" ListHeight="200" />
                        </td>
                        <td>
                            <input type="text" name="inputOverTime" id="inputOverTime" value="" />
                        </td>
                        <td>
                            <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetMode(1)" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width: 1250px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" class="TitleRow" style="width: 120px">編號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 120px">姓名
                    </th>
                     <th scope="col" class="TitleRow" style="width: 120px">員編
                    </th>
                    <th scope="col" class="TitleRow" style="width: 120px">卡號
                    </th>
                    <%-- <th scope="col" class="TitleRow" style="width: 120px">版次
                    </th>--%>
                    <th scope="col" class="TitleRow" style="width: 120px">單位
                    </th>
                    <th scope="col" class="TitleRow">在廠時間加總
                    </th>
                </tr>
                <%--<tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="5">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1250px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if (this.ListLog.Count == 0)
                                            { %>
                                        <tr class="DataRow">
                                            <td colspan="5">
                                                <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <%foreach (System.Data.DataRow o in this.DataResult.Rows)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">
                                            <td style="width: 124px">
                                                <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}", o["CardTime"]) %>
                                            </td>
                                            <td style="width: 124px">
                                                <%=o["oDepName"]%>
                                            </td>
                                            <td style="width: 124px">
                                                <%=o["oDepName"]%>
                                            </td>
                                            <td style="width: 124px">
                                                <%=o["PsnNo"]%>
                                            </td>
                                            <td style="width: 124px">
                                                <%=o["PsnName"]%>
                                            </td>
                                            <td>
                                                <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",o["LogTime"]) %>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <%if (this.PagedList != null)
                    { %>
                <tr class="GVStylePgr">
                    <td colspan="11">
                        <a id="btnFirst" href="#" style="text-decoration: none;" onclick="ShowPage(1)">第一頁</a>
                        <a id="btnPrev" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PrePage%>)">前一頁</a>
                        <%for (int pageIndex = this.StartPage; pageIndex < EndPage; pageIndex++)
                            { %>
                        <%if (pageIndex == this.PageIndex)
                            { %>
                        <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none; color: white"><%=pageIndex %></a>
                        <%}
                            else
                            {%>
                        <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;"><%=pageIndex %></a>
                        <%} %>
                        <%} %>
                        <%=string.Format("{0} / {1}　                總共 {2} 筆", this.PagedList.PageNumber, this.PagedList.PageCount, this.PagedList.TotalItemCount) %>
                        <a id="btnNext" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.NextPage%>)">下一頁</a>
                        <a id="btnLast" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PagedList.PageCount %>)">最末頁</a>
                    </td>
                </tr>
                <%} %>--%>
            </tbody>
        </table>
    </div>
    <div>
        <input type="button" name="Export" value="匯出EXCEL" id="Export" class="IconExport" onclick="SetPrintExcel()" />
    </div>

</asp:Content>
