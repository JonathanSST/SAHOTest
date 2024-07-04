<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryAbnormal638.aspx.cs" Inherits="SahoAcs.QueryAbnormal638" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

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
        <input type="hidden" id="CardDateS" name="CardDateS" value="" />
        <input type="hidden" id="CardDateE" name="CardDateE" value="" />
        <input type="hidden" id="LogTimeS" name="LogTimeS" value="" />
        <input type="hidden" id="LogTimeE" name="LogTimeE" value="" />
        <input type="hidden" id="LogStatus" name="LogStatus" value="" />
        <input type="hidden" id="EquNo" name="EquNo" value="" />
        <input type="hidden" id="DepNo" name="DepNo" value="" />
        <input type="hidden" id="MgaName" name="MgaName" value="" />        
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
        <input type="hidden" id ="PsnID" name="PsnID" value="<%=this.PsnID %>" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="刷卡時間起迄"></asp:Label>
                        </th>
                        <%--<td></td>--%>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                           <uc1:Calendar ID="CalendarS" runat="server" />
                        </td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc1:Calendar ID="CalendarE" runat="server" />
                        </td>
                       <%-- <td>
                            <select class="DropDownListStyle" style="width:150px" id="AbnormalType" name="AbnormalType">
                                <option value="">全部</option>
                                <option value="0">刷進未刷出</option>
                                <option value="1">刷出未刷進</option>
                                <option value="2">入廠一小時</option>
                            </select>
                        </td>--%>
                        <td>                            
                            <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetMode(1)" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width:1350px">
            <tbody>
                <tr class="GVStyle">                  
                    <th scope="col" class="TitleRow" style="width:135px">
                        2道門入廠時間
                    </th>
                    <th scope="col" class="TitleRow" style="width:135px">
                        2道門出廠時間
                    </th>
                    <th scope="col" class="TitleRow" style="width:120px">
                        1道門入廠時間
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        人員ID
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        廠商名稱
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        廠商編號
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        工程編號
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        入廠卡號
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        部門
                    </th>
                    <th scope="col" class="TitleRow" >
                        部門名稱
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="11">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1350px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.ListLog.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="11">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.ListLog)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">                                                                                        
                                            <td style="width:139px">
                                                 <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",o.InTime2nd) %>
                                            </td>
                                            <td style="width:139px">
                                                 <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",o.OutTime2nd) %>
                                            </td>
                                            <td style="width:124px">
                                                 <%=o.IPLTTM %>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.IDNO%>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.NM%>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.VNDNM%>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.VNDID%>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.VHNO%>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.IPLTLIC%>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.DPID%>
                                            </td>
                                            <td>
                                                 <%=o.DPNM%>
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
                        <a id="btnPrev" href="#" style="text-decoration: none;"  onclick="ShowPage(<%=this.PrePage%>)">前一頁</a>
                        <%for (int pageIndex = this.StartPage; pageIndex < EndPage; pageIndex++)
                        { %>
                        <%if (pageIndex == this.PageIndex)
                        { %>
                                    <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;color:white"><%=pageIndex %></a>
                        <%}
                        else
                        {%>
                                <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;"><%=pageIndex %></a>
                                <%} %>
                        <%} %>
                            <%=string.Format("{0} / {1}　                總共 {2} 筆", this.PagedList.PageNumber, this.PagedList.PageCount, this.PagedList.TotalItemCount) %>       
                        <a id="btnNext" href="#" style="text-decoration: none; " onclick="ShowPage(<%=this.NextPage%>)">下一頁</a>
                        <a id="btnLast" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PagedList.PageCount %>)">最末頁</a>
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
    </div>  
    <br />
    <div>        
        <input type="button" name="ExportButton" value="匯　出" id="ExportButton" class="IconExport" onclick="SetPrint()" />
    </div>
    <div id="popOverlay" style="display:none;position:absolute; top:0; left:0; z-index:29999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>    
</asp:Content>
