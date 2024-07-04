﻿<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryStay0638.aspx.cs" Inherits="SahoAcs.QueryStay0638" %>

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
                        <td>                            
                            <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetMode(1)" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width:970px">
            <tbody>
                <tr class="GVStyle">
                    <%foreach (var o in this.ListCols) {%>
                    <th scope="col" class="TitleRow">
                        <a href="#" onclick="SetSort('<%=o.DataRealName %>')"><%=o.TitleName %></a>
                        <input type="hidden" name="TitleCol" value="<%=o.TitleWidth %>" />
                        <input type="hidden" name="ColName" value="<%=o.ColName%>" />
                        <%if (o.DataRealName == SortName)
                            { %>
                            <%if (this.SortType == "ASC")
                                { %>
                            <span>▲</span>
                        <%}else{ %>
                            <span>▼</span>
                        <%} %>
                        <%} %>
                    </th>
                <%} %>
<%--                    <th scope="col" style="width:120px;"><%=GetLocalResourceObject("ttCardTime") %></th>
                    <th scope="col" style="width: 120px;"><%=GetLocalResourceObject("ttDeptName") %></th>
                    <th scope="col" style="width: 80px;"><%=GetLocalResourceObject("ttPsnNo") %></th>
                    <th scope="col" style="width: 100px;"><%=GetLocalResourceObject("ttPsnName") %></th>
                    <th scope="col" style="width: 70px;"><%=GetLocalResourceObject("ttCardNo") %></th>
                    <th scope="col" style="width: 70px;"><%=GetLocalResourceObject("ttTempCardNo") %></th>
                    <th scope="col" style="width: 60px;"><%=GetLocalResourceObject("ttCardVer") %></th>
                    <th scope="col" style="width: 80px;"><%=GetLocalResourceObject("ttEquNo") %></th>
                    <th scope="col" style="width: 100px;"><%=GetLocalResourceObject("ttEquName") %></th>
                    <th scope="col" style="width: 100px;"><%=GetLocalResourceObject("ttResult") %></th>
                    <th scope="col"><%=GetLocalResourceObject("ttLogTime") %></th>--%>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="11">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 970px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.DataResult.Rows.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="11">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (System.Data.DataRow r in this.DataResult.Rows)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">                                            
                                            <%foreach (var c in this.ListCols)
                                            {%>
                                            <td>
                                                 <%=r[c.ColName].ToString() %>                            
                                                <input type="hidden" name="DataCol" value="<%=c.DataWidth %>" />
                                            </td>
                                            <%} %>
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
