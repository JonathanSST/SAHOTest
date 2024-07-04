﻿<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryCourseLog.aspx.cs" Inherits="SahoAcs.QueryCourseLog" %>

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
        <input type="hidden" id="DateS" name="DateS" value="" />
        <input type="hidden" id="DateE" name="DateE" value="" />
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
                        <%--<th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardDate" runat="server" Text="刷卡日期起迄"></asp:Label>
                        </th>--%>
                         <th>
                             <span>課程名稱</span>
                         </th>                                                                                                     
                        <td></td>
                    </tr>
                    <tr>
                       <%-- <td>
                           <uc2:CalendarFrm ID="CalendarS" runat="server" />
                        </td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc2:CalendarFrm ID="CalendarE" runat="server" />
                        </td> --%>
                        <td>
                             <select id="CourseName" name="CourseName" style="width:200px" class="DropDownListStyle">
                                    <%foreach(var o in this.ListCourse) { %>
                                        <option value="<%=o.CourseID %>"><%=string.Format("{0}....{1}",o.CourseName,o.EquName) %></option>
                                    <%} %>
                                    <%if(this.ListCourse.Count==0) { %>
                                        <option value="0">無資料</option>
                                    <%} %>
                                </select>
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
        <table class="TableS1" style="width:1680px">
            <tbody>
                <tr class="GVStyle">
                    <%foreach (var o in this.ListCols.Where(i=>i.DataWidth!=0)) {%>
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
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="14">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 100%; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.DataResult.Rows.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="14">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (System.Data.DataRow r in this.DataResult.Rows)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">                                            
                                            <%foreach (var c in this.ListCols.Where(i=>i.DataWidth!=0))
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
               <tr class="GVStyle">
                     <td colspan="14">
                         <%=string.Format("總人數：{0}",this.DataResult.Rows.Count) %>
                    </td>                    
                </tr>
            </tbody>
        </table>
    </div>  
    <br />
    <div>        
        <input type="button" name="ExportButton" value="匯　出" id="ExportButton" class="IconExport" onclick="SetPrint()" />
    </div>
    <div id="popOverlay" style="display:none;position:absolute; top:0; left:0; z-index:29999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>    
</asp:Content>
