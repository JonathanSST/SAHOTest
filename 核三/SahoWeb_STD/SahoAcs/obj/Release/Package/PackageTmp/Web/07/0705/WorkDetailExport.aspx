<%@ Page Title="" Theme="UI"  Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" 
    CodeBehind="WorkDetailExport.aspx.cs" Inherits="SahoAcs.Web._07._0705.WorkDetailExport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/PickDate.ascx" TagPrefix="uc1" TagName="PickDate" %>
<%@ Register src="../../../uc/CalendarFrm.ascx" tagname="CalendarFrm" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
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
        <input type="hidden" id="CardMonth" name="CardMonth" value="<%=DateTime.Now.ToString("yyyy/MM") %>" />
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
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="結算時間起迄"></asp:Label>
                        </th>                                                                        
                        <td></td>
                    </tr>
                    <tr>                       
                        <td>
                           <uc2:CalendarFrm ID="CalcDayS" runat="server" />
                        </td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc2:CalendarFrm ID="CalcDayE" runat="server" />
                        </td>                                                
                        <td>                                                        
                             <input type="button" class="IconSave" value="差勤結算" id="BtnCalc" />
                            <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetQuery()" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width:1150px">
            <tbody>
                <tr class="GVStyle">             
                    <th scope="col" class="TitleRow" style="width:130px">
                        工號姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        工作日
                    </th>
                    <th scope="col" class="TitleRow" style="width:50px">
                        班別
                    </th>
                    <th scope="col" class="TitleRow" style="width:50px">
                        標上
                    </th>
                    <th scope="col" class="TitleRow" style="width:50px">
                        標下
                    </th>
                    <th scope="col" class="TitleRow" style="width:50px">
                        刷上
                    </th>
                    <th scope="col" class="TitleRow" style="width:50px">
                        刷下
                    </th>
                    <th scope="col" class="TitleRow" style="width:50px">
                        遲到
                    </th>
                    <th scope="col" class="TitleRow" style="width:50px">
                        早退
                    </th>
                    <th scope="col" class="TitleRow" style="width:50px">
                        加班
                    </th>
                    <th scope="col" class="TitleRow" style="width:120px">
                        狀態
                    </th>
                     <th>
                        備註
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="12">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1150px; overflow-y: scroll;">
                            <div id="DivSaveArea">
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.PagedList.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="12">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.PagedList)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">          
                                            <td style="width:134px">
                                               <%=o.PsnNo+" "+o.PsnName %>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.WorkDate %>
                                            </td>
                                            <td style="width:54px">
                                                <%=o.ClassNo %>
                                            </td>
                                              <td style="width:54px">
                                                  <%=o.WorkTimeS %>
                                            </td>
                                              <td style="width:54px">  
                                                  <%=o.WorkTimeE %>
                                            </td>                                          
                                              <td style="width:54px">
                                                  <%=o.RealTimeS %>
                                            </td>
                                              <td style="width:54px">
                                                  <%=o.RealTimeE %>
                                            </td>
                                              <td style="width:54px">
                                                  <%=o.Delay %>
                                            </td>
                                              <td style="width:54px">     
                                                  <%=o.StealTime%>
                                            </td>
                                              <td style="width:54px">
                                                  <%=o.OverTime %>
                                            </td>
                                            <td style="width:124px">
                                                <%=o.StatuDesc %>
                                            </td>   
                                            <td>                                                                                                
                                                <%=o.AttendanceDesc %>
                                                <input type="hidden" value="<%=o.RecordID %>" id="RecordID" name="RecordID" />
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
                    <td colspan="14">
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
    <div id="PersonListArea" style="display:none;position:absolute; background-color:navy">
        <div id="ScrollArea" style="height:200px; overflow-y:scroll">
        <%foreach (var o in this.PersonList)
            { %>
            <div id="PsnArea" class="PsnArea"><span id="NameSpan" style="font-size:10pt;color:white"><%=o.PsnName %>(<%=o.PsnNo %>)</span><input type="hidden" id="HiddenPsnNo" value="<%=o.PsnNo %>" /></div>
        <%} %>
            </div>
    </div>
    <div id="popOverlay" style="display:none;position:absolute; top:0; left:0; z-index:29999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>    
</asp:Content>
