<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ToWorkCount.aspx.cs" Inherits="SahoAcs.Web._07._0706.ToWorkCount" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="ValueKeep">
       <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="HiddenField1" runat="server" EnableViewState="False" />
        <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
        <input type="hidden" id="SortDataContext" name="SortDataContext" value="" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Query" />
        <input type="hidden" id="StartTime" name="StartTime" value="" />
        <input type="hidden" id="EndTime" name="EndTime" value="" />
        <input type="hidden" id="Department" name="Department" value="" />
        <input type="hidden" id="Company" name="Company" value="" />
    </div>
      <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblCompany" runat="server" Text="公司"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblDepartment" runat="server" Text="部門"></asp:Label>
            </th>
            <th colspan="3">
                <span class="Arrow01"></span>
                <asp:Label ID="lblStartDate" runat="server" Text="查詢時間起迄"></asp:Label>
            </th>
            <td></td>
            <td></td>
        </tr>
        <tr>
             <td>
                <asp:DropDownList ID="dropCompany" runat="server" Width="150px"></asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="dropDepartment" runat="server" Width="200px"></asp:DropDownList>
            </td>     
            <td>
               <uc1:Calendar ID="QueryTimeS" runat="server" />
            </td>
            <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
            <td>
               <uc1:Calendar ID="QueryTimeE" runat="server" />
            </td>
            <td>
                &nbsp;&nbsp;
            </td>
            <td>
                 <input type="button" id="BtnQuery" value="<%=Resources.Resource.btnQuery %>" class="IconSearch"/>
            </td>
        </tr>
    </table>
    <div id="ContentPlaceHolder1_UpdatePanel1"> 
        <table class="TableS1" style="width:1200px">
            <tbody>
              <tr class="GVStyle">       
                 <th scope="col" class="TitleRow" style="width: 85px">人員編號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 105px">姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width: 155px">請假起始時間
                    </th>
                    <th scope="col" class="TitleRow" style="width: 155px">請假訖止時間
                    </th>
                    <th scope="col" class="TitleRow" style="width: 180px">單位編號名稱
                    </th>
                    <th scope="col" class="TitleRow" style="width: 70px">假別
                    </th>
                    <th scope="col" class="TitleRow" style="width: 70px">天數
                    </th>
                     <th scope="col" class="TitleRow" style="width: 70px">時數
                    </th>
                    <th scope="col" class="TitleRow">備註
                    </th>
            </tr>
            <tr>
                 <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                    <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1200px; overflow-y: scroll;">
                        <div id="DivSaveArea">
                            <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                <tbody>
                                      <%if(this.PagedList.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="9">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                    <%} %>
                                    <%foreach (var o in this.PagedList)
                                        { %>
                                    <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)"
                                            onclick="SingleRowSelect('0', this, $('#SelectValue')[0],'<%=o.RecordID %>', '', '');"
                                            style="color: rgb(0, 0, 0);">
                                            <td style="width: 89px">
                                                <%=o.PsnNo %>                                                
                                            </td>
                                            <td style="width: 109px">
                                                <%=o.PsnName %>
                                                <input type="hidden" id="OrgStrucID" name="OrgStrucID" value="<%=o.OrgNo %>" />
                                            </td>
                                            <td style="width: 159px">
                                                <%=o.StartTime.ToString("yyyy/MM/dd HH:mm") %>
                                            </td>
                                             <td style="width: 159px">
                                                <%=o.EndTime.ToString("yyyy/MM/dd HH:mm") %>
                                            </td>
                                            <td style="width: 184px">
                                                <%=o.DepartmentNo %>
                                                <%=o.DepartmentName %>
                                            </td>
                                              <td style="width: 74px">
                                                <%=o.HoliNo %>
                                            </td>
                                              <td style="width: 74px">
                                                <%=o.Daily %>
                                            </td>
                                              <td style="width: 74px">
                                                <%=o.Hours %>
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
            <%--  <%if (this.PagedList != null)
                        { %>
                <tr class="GVStylePgr">
                    <td colspan="4">
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
                <%} %>--%>
            </tbody>
        </table>
    </div>
    <br />
    <div>        
      <input type="button" name="ExportButton" value="匯　出" id="ExportButton" class="IconExport" />
    </div>
</asp:Content>
