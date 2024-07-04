<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryHolidayDetail.aspx.cs" Inherits="SahoAcs.QueryHolidayDetail" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../../../uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <link rel="stylesheet" href="https://cdn.rawgit.com/openlayers/openlayers.github.io/master/en/v5.2.0/css/ol.css" type="text/css" />
    <style>
        .map {
            height: 800px;
            width: 100%;
        }
    </style>    
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="SortDataContext" name="SortDataContext" value="" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Query" />
        <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
        <input type="hidden" id="CardDateS" name="DateS" value="" />
        <input type="hidden" id="CardDateE" name="DateE" value="" />
        <input type="hidden" id="MgaName" name="MgaName" value="" />        
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
        <input type="hidden" id ="PsnID" name="PsnID" value="<%=this.PsnID %>" />
        <input type="hidden" name="AuthList" id="AuthList" value="<%=this.AuthList %>" />             
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                         <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="人員姓名"></asp:Label>
                        </th>       
                         <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label2" runat="server" Text="單位"></asp:Label>
                        </th>       
                         <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label3" runat="server" Text="假別"></asp:Label>
                        </th>      
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="請假起迄"></asp:Label>
                        </th>                                                                        
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <input type="text" id="PsnName" name="PsnName" />
                            <input type="hidden" id="PsnNo" name="PsnNo" />                            
                        </td>
                        <td>
                            <select style="width: 200px" class="DropDownListStyle" id="DeptList" name="DeptList">
                                <option value=""><%=Resources.Resource.ddlSelectDefault %></option>
                                <%foreach (var o in this.OrgDataInit.Where(i => i.OrgClass.Equals("Unit")))
                                    { %>
                                <option value="<%=o.OrgStrucID %>"><%=o.OrgName+"."+o.OrgNo%></option>
                                <%} %>
                            </select>
                        </td>
                        <td>
                            <select style="width: 200px" class="DropDownListStyle" id="VNoData" name="VNoData">
                                <option value=""><%=Resources.Resource.ddlSelectDefault %></option>
                                <%foreach (var o in this.VacationList)
                                    { %>
                                <option value="<%=o.VNo %>"><%=o.VName %></option>
                                <%} %>
                            </select>
                        </td>
                        <td>
                           <uc2:CalendarFrm ID="CardDayS" runat="server" />
                        </td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc2:CalendarFrm ID="CardDayE" runat="server" />
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
        <table class="TableS1" style="width:1400px">
            <tbody>
                <tr class="GVStyle">     
                     <th scope="col" class="TitleRow" style="width:150px">
                        工號  姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width:140px">
                        啟始日期
                    </th>
                    <th scope="col" class="TitleRow" style="width:140px">
                        結束日期
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        假別
                    </th>
                    <th scope="col" class="TitleRow" style="width:70px">
                        請假天數
                    </th>       
                    <th scope="col" class="TitleRow" style="width:70px">
                        請假時數
                    </th>                    
                    <th scope="col" class="TitleRow" style="width:70px">
                        請假分鐘
                    </th> 
                    <th>
                        備註
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="15">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1400px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.ListLog.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="15">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.PagedList)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">          
                                             <td style="width:154px">
                                                 <%=o.PsnNo+" "+o.PsnName %>
                                            </td>
                                            <td style="width:144px">
                                                 <%=string.Format("{0:yyyy/MM/dd HH:mm}", o.StartTime)%>
                                            </td>
                                            <td style="width:144px">
                                                 <%=string.Format("{0:yyyy/MM/dd HH:mm}", o.EndTime) %>
                                            </td>
                                            <td style="width:84px">
                                                <%=o.HoliNo %>
                                            </td>         
                                              <td style="width:74px">
                                                  <%=int.Parse(o.Daily) %>
                                            </td>
                                              <td style="width:74px">
                                                  <%=o.Hours %>
                                            </td>             
                                              <td style="width:74px">
                                                  <%=o.Minutes %>
                                            </td> 
                                            <td>
                                                <%="" %>
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
    <div id="OneLogArea" style="display: none; position:absolute; z-index:30000;background-color:#1275BC;border-style:solid; border-width:2px; border-color:#069">
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <span>打卡時間</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>員工編號</span>
                            </th>
                        </tr>
                        <tr>
                            <th>
                                <span id="ShowCardTime"></span>
                                <br />
                            </th>
                            <th>
                                <span id="ShowEmpID"></span>
                            </th>             
                        </tr>                                                
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <span>打卡相片</span>
                            </th>
                        </tr>
                        <tr>                            
                            <td>
                                <img id="LogPic" style="height:259px;width:228px" src=""/>
                            </td>             
                        </tr>
                    </table>
                     <table>
                        <tr>
                            <td>
                                <input type="button" name="btnCancel" value="關閉" class="IconCancel" onclick="CancelOneLogArea()" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div>        
      <input type="button" name="ExportButton" value="列　印" id="ExportButton" class="IconExport" onclick="SetPrint()" />
    </div>
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
