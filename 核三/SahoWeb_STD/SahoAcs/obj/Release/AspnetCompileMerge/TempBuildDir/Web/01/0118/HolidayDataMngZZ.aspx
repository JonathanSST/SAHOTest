<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="HolidayDataMngZZ.aspx.cs" Inherits="SahoAcs.Web._01._0118.HolidayDataMngZZ" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ValueKeep">
            <script src="../../../scripts/Check/JS_AJAX.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UTIL.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_BUTTON_PASS.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_CHECK.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_TOOLTIP.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LEVEL.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.DATE.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.ETEK.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.HT.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.REF.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TABLE.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TABS.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TOOLTIP.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI_FILE_UPLOAD.js" type="text/javascript"></script>
    <script src="../../../scripts/JsTabEnter.js" type="text/javascript"></script>
    <script src="../../../scripts/JsQueryWindow.js" type="text/javascript"></script>

        <input type="hidden" id="SelectValue" name="SelectValue" />
        <input type="hidden" id="SelectNowNo" name="SelectNowNo" />
        <input type="hidden" id="SelectNowName" name="SelectNowName" />
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <%--  <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />--%>
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="SortDataContext" name="SortDataContext" value="" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Query" />
        <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
        <input type="hidden" id="QryTimeS" name="QryTimeS" value="" />
        <input type="hidden" id="QryTimeE" name="QryTimeE" value="" />
        <input type="hidden" id="LogTimeS" name="LogTimeS" value="" />
        <input type="hidden" id="LogTimeE" name="LogTimeE" value="" />
        <input type="hidden" id="LogStatus" name="LogStatus" value="" />
        <input type="hidden" id="DepNo" name="DepNo" value="" />
        <input type="hidden" id="MgaName" name="MgaName" value="" />
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_PsnNo" runat="server" Text="請假起訖時間"></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                             <uc1:Calendar ID="QueryTimeS" runat="server" />
                        </td>
                        <th>
                            ~                            
                        </th>
                        <td>
                             <uc1:Calendar ID="QueryTimeE" runat="server" />
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
        <table class="TableS1" style="width: 1050px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" class="TitleRow" style="width: 135px">人員編號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 135px">姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width: 135px">假別
                    </th>
                    <th scope="col" class="TitleRow" style="width: 155px">請假起始時間
                    </th>
                    <th scope="col" class="TitleRow" style="width: 155px">請假訖止時間
                    </th>
                    <th scope="col" class="TitleRow">單位編號
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="11">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 100%; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 1050px; border-collapse: collapse; text-align: center;">
                                    <tbody>
                                        <%if (this.ScheduleDatas.Count == 0)
                                        { %>
                                        <tr class="DataRow">
                                            <td colspan="4">
                                                <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <%foreach (var o in this.ScheduleDatas)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)"  ondblclick="CallEdit()"
                                            onclick="SingleRowSelect('0', this, $('#SelectValue')[0],'<%=o.RecordID %>', '', '');"
                                            style="color: rgb(0, 0, 0);">
                                            <td style="width: 139px">
                                                <%=o.PsnNo %>                                                
                                            </td>
                                            <td style="width: 139px">
                                                <%=o.PsnName %>
                                                <input type="hidden" id="OrgStrucID" name="OrgStrucID" value="<%=o.OrgNo %>" />
                                            </td>
                                             <td style="width: 139px">
                                                <%=o.HoliNo %>                                                
                                            </td>
                                            <td style="width: 159px">
                                                <%=o.StartTime.ToString("yyyy/MM/dd HH:mm:ss") %>
                                            </td>
                                             <td style="width: 159px">
                                                <%=o.EndTime.ToString("yyyy/MM/dd HH:mm:ss") %>
                                            </td>
                                            <td>
                                                <%=o.OrgNo %>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
<%--                
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
        <table>
            <tbody>
                <tr>
                    <td>
                        <input type="button" id="AddButton" name="AddButton" class="IconNew" value="<%=GetGlobalResourceObject("Resource", "btnAdd") %>" onclick="CallAdd()" />
                    </td>
                    <td>
                        <input type="button" id="EditButton" name="EditButton" class="IconEdit" value="<%=GetGlobalResourceObject("Resource", "btnEdit") %>" onclick="CallEdit()" />
                    </td>
                    <td>
                        <%--<input type="button" id="DeleteButton" name="DeleteButton" class="IconDelete" value="<%=GetGlobalResourceObject("Resource", "btnDelete") %>" onclick="CallDelete()" />--%>
                    </td>
                </tr>
            </tbody>
        </table>
        <input type="hidden" id="MaxMobile" name="MaxMobile" value="<%=SahoAcs.DBClass.DongleVaries.GetMaxMobile() %>" />
        <input type="hidden" id="CurrentMobile" name="CurrentMobile" value="<%=SahoAcs.DBClass.DongleVaries.GetCurrentMobile() %>" />
    </div>
    <br />
    <div id="PersonListArea" style="display:none;position:absolute; background-color:blue;z-index:39999">
         <div>
             <input type="button" id="CloseArea" name="CloseArea" class="IconLeave" value="關閉"/>
         </div>
        <div id="ScrollArea" style="height:200px; overflow-y:scroll">
        <%foreach (var o in this.PersonList)
            { %>
            <div id="PsnArea" class="PsnArea" style="width:240px; border-color:white;border-width:1pt;border-style:dashed"><span id="NameSpan" style="font-size:10pt;color:white"><%=o.PsnName %></span><span id="NoSpan" style="font-size:10pt;color:white">(<%=o.PsnNo %>)</span>
                <input type="hidden" id="HiddenPsnNo" value="<%=o.PsnNo %>" /></div>
        <%} %>
            <%if (this.PersonList.Count == 0)
                { %>
            <span style="width:300px;color:white">查無人員</span>
            <%} %>
            </div>
    </div>
</asp:Content>
