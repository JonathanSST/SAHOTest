<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MealOrderRecordInq.aspx.cs" Inherits="SahoAcs.Web._07._0702.MealOrderRecordInq" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
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
     </div>
       <table class="Item">
        <tr>
            <th colspan="3">
                <span class="Arrow01"></span>
                起迄時間
            </th>
            <th>
                <span class="Arrow01"></span>
                卡號
            </th>
            <th>
                <span class="Arrow01"></span>
               人員編號
            </th>
            <th>
                <span class="Arrow01"></span>
                姓名
            </th>
            <th>
                <span class="Arrow01"></span>
                部門
            </th>
            <th>
                <span class="Arrow01"></span>
                餐別
            </th>
        </tr>
        <tr>
           <td>
                <uc1:Calendar runat="server" ID="Calendar_CardTimeSDate" />
            </td>
            <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
            <td>
                <uc1:Calendar runat="server" ID="Calendar_CardTimeEDate" />
            </td>
             <td>
                <asp:TextBox ID="txt_CardNo" runat="server" Width="120px" MaxLength="10"></asp:TextBox>
            </td> 
             <td>
                <asp:TextBox ID="txt_PsnNo" runat="server" Width="120px" MaxLength="10"></asp:TextBox>
            </td> 
            <td>
                <asp:TextBox ID="txt_PsnName" runat="server" Width="120px" MaxLength="20"></asp:TextBox>
            </td> 
            <td>
                <asp:DropDownList ID="dropDepartment" runat="server" Width="200px"></asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="dropMealNo" runat="server" Width="200px">
                    <asp:ListItem Value="" Selected="True">== 全部 ==</asp:ListItem>
                    <asp:ListItem Value="1" >午餐</asp:ListItem>
                    <asp:ListItem Value="2" >宵夜</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
           <td>
                &nbsp;</td>
            <td style="font-size: 16px; color: white">&nbsp;</td>
            <td>
                &nbsp;</td>
             <td>
                 &nbsp;</td> 
             <td>
                 &nbsp;</td> 
            <td>
                &nbsp;</td> 
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
           <td colspan="3" style="font-size: 16px; color: white">
                葷食 / 素食
                <asp:DropDownList ID="dropMealFood" runat="server" Width="200px">
                    <asp:ListItem Value="" Selected="True">== 全部 ==</asp:ListItem>
                    <asp:ListItem Value="0" >葷食</asp:ListItem>
                    <asp:ListItem Value="1" >素食</asp:ListItem>
                </asp:DropDownList>
           </td>
             <td class="ShowPsnInfo">
                
             </td> 
             <td colspan="4" align="right">
                &nbsp;    
                <input type="button" class="IconSearch" value="查     詢" onclick="SetMode(1);" />
                <input type="button" class="IconSearch" value="清 除 資 料" onclick="location.reload();" />
                <input type="button" name="ExportXlsButton" value="匯出Excel" id="ExportXlsButton" class="IconExport" onclick="SetPrintExcel();" />
             </td> 
        </tr>
        </table>
    <div id="ContentPlaceHolder1_UpdatePanel1">
         <table class="TableS1"  style="width:800px">
             <tbody>
                 <tr class="GVStyle">             
                    <th scope="col" class="TitleRow" style="width:150px">
                        訂餐時間
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        餐別
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        卡號
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        人員編號
                    </th>
                      <th scope="col" class="TitleRow" style="width:100px">
                        姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        部門
                    </th>
                    <th scope="col" class="TitleRow">
                        葷素食
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="7">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 900px; overflow-y: scroll;">
                            <div id="DivSaveArea">
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.PagedList.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="7">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.PagedList)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">          
                                            <td style="width:154px">
                                             <%string OrderTime = "";
                                                        if (o.OrderTime.Equals(DateTime.MinValue))
                                                        {
                                                            OrderTime = "查無資料";
                                                        }
                                                        else 
                                                        {
                                                            OrderTime = Convert.ToString(o.OrderTime);
                                                        }                                                                                                      
                                                         %>
                                                    <%=OrderTime %>
                                               <%--<%=o.OrderTime %>--%>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.MealNoDesc %>
                                            </td>
                                            <td style="width:104px">
                                                <%=o.CardNo %>
                                            </td>
                                              <td style="width:104px">
                                                  <%=o.PsnNo %>
                                            </td>
                                              <td style="width:104px">  
                                                  <%=o.PsnName %>
                                            </td>                                          
                                              <td style="width:104px">
                                                  <%=o.OrgName %>
                                            </td>
                                            <td>
                                                <%=o.MealFoodDesc %>
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
                    <td colspan="9">
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
