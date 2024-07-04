<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryLeavePerson.aspx.cs" Inherits="SahoAcs.QueryLeavePerson" %>

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
        <input type="hidden" id="PsnSTime" name="PsnSTime" value="" />
        <input type="hidden" id="PsnETime" name="PsnETime" value="" />
        <input type="hidden" id="ComID" name="ComID" value="" />
        <input type="hidden" id="DepID" name="DepID" value="" />
        <input type="hidden" id="TitID" name="TitID" value="" />
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
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="起迄時間"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="人員編號或姓名"></asp:Label>
                        </th>
                         <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label3" runat="server" Text="身份證號"></asp:Label>
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
                            <input type="text" name="inputPsn" id="inputPsn" value="<%=this.txt_inputPsn  %>" />
                        </td>
                         <td>
                            <input type="text" name="inputIDNum" id="inputIDNum" value="<%=this.txt_inputIDNum  %>" />
                        </td>
                        <td class="ShowPsnInfo">
                            <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnAdvance") %>" onclick="SetAdvArea()" />
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
        <table class="TableS1" style="width: 1200px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" class="TitleRow" style="width: 120px">人員編號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 120px">人員姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width: 120px">身份證號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 140px">到職日
                    </th>
                    <th scope="col" class="TitleRow" style="width: 140px">離職日
                    </th>
                    <th scope="col" class="TitleRow" style="width: 160px">單位
                    </th>
                    <th scope="col" class="TitleRow">職稱
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="7">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1250px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if (this.PsnList.Count == 0)
                                            { %>
                                        <tr class="DataRow">
                                            <td colspan="7">
                                                <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <%foreach (System.Data.DataRow o in this.DataResult.Rows)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">
                                            <td style="width: 124px">
                                                <%=o["PsnNo"]%>
                                            </td>
                                            <td style="width: 124px">
                                                <%=o["PsnName"]%>
                                            </td>
                                            <td style="width: 124px">
                                                <%=o["IDNum"]%>
                                            </td>
                                            <td style="width: 144px">
                                                 <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}", o["PsnSTime"]) %>
                                            </td>
                                            <td style="width: 144px">
                                                <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}", o["PsnETime"]) %>
                                            </td>
                                            <td style="width: 164px">
                                                [<%=o["OrgNo3"]%>] <%=o["OrgName3"]%> 
                                            </td>
                                            <td>
                                               [<%=o["OrgNo4"]%>] <%=o["OrgName4"]%>
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
                    <td colspan="7">
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
    <div>
        <input type="button" name="Export" value="匯出EXCEL" id="Export" class="IconExport" onclick="SetPrintExcel()" />
        <%--<input type="button" name="ExportButton" value="匯出PDF" id="ExportButton" class="IconExport" onclick="SetPrintPDF()" />--%>
    </div>
   
    <!-- 進階搜尋 -->
    <div id="popOverlay" style="display:none;position:absolute; top:0; left:0; z-index:29999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>
    <div id="AdvanceArea" style="display: none; position:absolute; z-index:30000;background-color:#1275BC;border-style:solid; border-width:2px; border-color:#069">
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <th colspan="3">
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_PsnTime" runat="server" Text="起迄時間"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc1:Calendar runat="server" ID="ADVCalendar_PsnSTime" />
                                        </td>
                                        <td style="font-size: 16px; color: white">至</td>
                                        <td>
                                            <uc1:Calendar runat="server" ID="ADVCalendar_PsnETime" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_Com" runat="server" Text="公司"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc2:MultiSelectDropDown runat="server" ID="ADVDropDownList_Com" ListWidth="350" ListHeight="150" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_Dep" runat="server" Text="部門"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc2:MultiSelectDropDown runat="server" ID="ADVDropDownList_Dep" ListWidth="350" ListHeight="150" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_Tit" runat="server" Text="職稱"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc2:MultiSelectDropDown runat="server" ID="ADVDropDownList_Tit" ListWidth="350" ListHeight="150" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_PsnNoPsnName" runat="server" Text="人員編號/姓名"></asp:Label>
                                        </th>
                                        <th>
                                            <span class="Arrow01"></span>
                                            <asp:Label ID="ADVQueryLabel_IDNum" runat="server" Text="身份證號"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>                                            
                                            <input type="text" id="ADVPsnNoPsnNam" name="ADVPsnNoPsnNam" style="width:160px" />
                                        </td>
                                        <td>                                            
                                            <input type="text" id="ADVIDNum" name="ADVIDNum" style="width:160px" />
                                        </td>  
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        
                    </table>
                    <table> 
                        <tr>
                            <td>
                                <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetMode(2)" />
                            </td>
                            <td>
                                <input type="button" name="btnCancel" value="取消" class="IconCancel" onclick="CancelAdvArea()" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
