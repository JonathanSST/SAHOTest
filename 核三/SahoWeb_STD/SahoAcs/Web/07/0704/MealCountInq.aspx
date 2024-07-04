<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MealCountInq.aspx.cs" Inherits="SahoAcs.Web._07._0704.MealCountInq" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../../../uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
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
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
        <input type="hidden" id="SortDataContext" name="SortDataContext" value="" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Query" />
        <input type="hidden" id="CardTimeSDate" name="CardTimeSDate" value="" />
        <input type="hidden" id="CardTimeEDate" name="CardTimeEDate" value="" />
        <input type="hidden" id="CardNo" name="CardNo" value="" />
        <input type="hidden" id="PsnNo" name="PsnNo" value="" />
        <input type="hidden" id="PsnName" name="PsnName" value="" />
        <input type="hidden" id="Dep" name="Dep" value="" />
        <input type="hidden" id="MealNo" name="MealNo" value="" />
        <input type="hidden" id="MealFood" name="MealFood" value="" />
        <input type="hidden" id="OrderSrc" name="OrderSrc" value="" />
     </div>
 <table class="Item">
    <tr>
            <th colspan="3">
                <span class="Arrow01"></span>
                起迄時間
            </th>
            <th style="width:100px;">
                
            </th>
            <th style="width:100px;">
              
            </th>
            <th style="width:100px;">
              
            </th>
            <th style="width:100px;">
              
            </th>
            <th style="width:100px;">
             
            </th>
        </tr>
        <tr>
          <td>
                <uc2:CalendarFrm runat="server" ID="Calendar_CardTimeSDate" />
            </td>
            <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
            <td>
                <uc2:CalendarFrm runat="server" ID="Calendar_CardTimeEDate" />
            </td>
             <td class="ShowPsnInfo">
                      
             </td> 
             <td colspan="4" align="right">
                <input type="button" class="IconSearch" value="查     詢" onclick="SetMode(1);" />
                <input type="button" class="IconSearch" value="清 除 資 料" onclick="location.reload();" />    
                <input type="button" name="ExportXlsButton" value="匯出Excel" id="ExportXlsButton" class="IconExport" onclick="SetPrintExcel();" />
             </td> 
        </tr>
 </table>
<div id="ContentPlaceHolder1_UpdatePanel1">
    <table class="TableS1"  style="width:600px">
        <tbody>
            <tr class="GVStyle">       
                <th scope="col" class="TitleRow" style="width:180px">
                    員工編號
                </th>
                <th scope="col" class="TitleRow" style="width:180px">
                    姓名
                </th>
                <th scope="col" class="TitleRow">
                    用餐次數
                </th>
            </tr>
             <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="3">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 600px; overflow-y: scroll;">
                            <div id="DivSaveArea">
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.PagedList.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="3">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.PagedList)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">          
                                            <td style="width:184px">
                                               <%=o.PsnNo %>
                                            </td>
                                            <td style="width:184px">
                                                 <%=o.PsnName %>
                                            </td>
                                            <td>
                                                <%=o.MealCount %>
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
                    <td colspan="3">
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
</asp:Content>
