<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="WorkHour24.aspx.cs" Inherits="SahoAcs.WorkHour24"  Theme="UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <script type="text/javascript">  
        $(document).ready(function () {
            DisplayTime();
        });
    </script>   
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
         <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width:1400px">
            <tbody>
                <tr class="GVStyle">             
                    <th scope="col" class="TitleRow" style="width:140px">
                        工號姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        日期
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        班別
                    </th>                    
                    <th scope="col" class="TitleRow" style="width:70px">
                        刷上
                    </th>
                    <th scope="col" class="TitleRow" style="width:70px">
                        刷下
                    </th>                 
                    <th scope="col" class="TitleRow" style="width:80px">
                        遲到
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        早退
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        狀態
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
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);"  ondblclick="SetShowOneLog(<%=o.RecordID %>)">          
                                            <td style="width:144px">
                                                 <%=o.PsnNo+"　"+o.PsnName %>
                                            </td>
                                            <td style="width:84px">
                                                 <%=o.WorkDate %>
                                            </td>
                                            <td style="width:84px">
                                                <%=o.ClassNo %>
                                            </td>                                                                 
                                              <td style="width:74px">
                                                  <%=o.RealTimeS %>
                                            </td>
                                              <td style="width:74px">
                                                  <%=o.RealTimeE %>
                                            </td>
                                              <td style="width:84px">
                                                  <%=o.Delay %>
                                            </td>
                                              <td style="width:84px">     
                                                  <%=o.StealTime%>
                                            </td>
                                            <td style="width:84px">
                                                <%=o.StatuDesc %>
                                            </td>                              
                                            <td>
                                                <%=o.AbnormalDesc  %>
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
    <span style="color:white; font-size:14pt">※ 點選資料列兩下進行紀錄清除</span>
    <br />
    <div id="popOverlay" style="display:none;position:absolute; top:0; left:0; z-index:29999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>    
    <div id="OneLogArea" style="display: none; position:absolute; z-index:30000;background-color:#1275BC;border-style:solid; border-width:2px; border-color:#069">
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <span>工作日</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>員工編號姓名</span>
                            </th>
                        </tr>
                        <tr>
                            <th>
                                <span id="WorkDateVal"></span>
                                <br />
                            </th>
                            <th>
                                <span id="PsnNameVal"></span>
                                <input type="hidden" id="RecordID" name="RecordID" value="" />
                            </th>             
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <span>異常原因</span>
                            </th>                            
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span id="AbnormalDesc"></span>
                                <br />
                            </th>                            
                        </tr>
                    </table>
                     <table>
                        <tr>
                            <td>
                                <input type="button" name="btnClear" id="btnClear" value="清除紀錄" class="IconCancel" />
                                <input type="button" name="btnCancel" value="關閉" class="IconCancel" onclick="CancelOneLogArea()" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <br />
</asp:Content>
