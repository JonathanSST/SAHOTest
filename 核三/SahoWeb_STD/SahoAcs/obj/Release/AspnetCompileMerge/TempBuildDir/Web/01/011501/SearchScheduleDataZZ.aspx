<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SearchScheduleDataZZ.aspx.cs" Inherits="SahoAcs.Web._01._011501.SearchScheduleDataZZ" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ValueKeep">
        <script src="../../../scripts/Check/JS_CHECK.js" type="text/javascript"></script>
        <input type="hidden" id="SelectValue" name="SelectValue" />
        <input type="hidden" id="SelectNowNo" name="SelectNowNo" />
        <input type="hidden" id="SelectNowName" name="SelectNowName" />
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <%--  <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />--%>
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
        <input type="hidden" id="DepNo" name="DepNo" value="" />
        <input type="hidden" id="MgaName" name="MgaName" value="" />
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_PsnNo" runat="server" Text="人員編號或姓名"></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <input type="text" value="" name="PsnName" id="PsnName" />
                            <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetMode(1)" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width: 1050px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" class="TitleRow" style="width: 135px">人員編號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 135px">姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width: 205px">例假日期
                    </th>
                    <th scope="col" class="TitleRow" style="width: 130px">部門名稱
                    </th>
                    <th scope="col" class="TitleRow">備註
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="11">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 100%; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 1050px; border-collapse: collapse; text-align: center;">
                                    <tbody>
                                        <%if (this.ScheduleDatas.Count == 0)
                                        { %>
                                        <tr class="DataRow">
                                            <td colspan="4">
                                                <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <%foreach (var o in this.ScheduleDatas)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)"
                                            onclick="SingleRowSelect(0, this, $('#SelectValue')[0],'<%=o.ScheduleID %>'+'_'+'<%=o.VacationDate.ToString("yyyy/MM/dd") %>'+'_'+'<%=o.OrgName%>', '', '');"
                                            style="color: rgb(0, 0, 0);">
                                            <td style="width: 138px">
                                                <%=o.employeeID %>
                                                <input type="hidden" id="ScheduleID" name="ScheduleID" value="<%=o.ScheduleID %>" />
                                            </td>
                                            <td style="width: 138px">
                                                <%=o.employeeName %>
                                                <input type="hidden" id="OrgStrucID" name="OrgStrucID" value="<%=o.OrgStrucID %>" />
                                            </td>
                                            <td style="width: 209px">
                                                <%=o.VacationDate.ToString("yyyy/MM/dd") %>
                                            </td>
                                            <td style="width: 138px">
                                                <%=o.OrgName %>
                                            </td>
                                            <td>
                                                <%=o.VacationInfo %>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
<%--                
                <%if (this.PagedList != null)
                    { %>
                <tr class="GVStylePgr">
                    <td colspan="11">
                        <a id="btnFirst" href="#" style="text-decoration: none;" onclick="ShowPage(1)">第一頁</a>
                        <a id="btnPrev" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PrePage%>)">前一頁</a>
                        <%for (int pageIndex = this.StartPage; pageIndex < EndPage; pageIndex++)
                            { %>
                        <%if (pageIndex == this.PageIndex)
                            { %>
                        <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none; color: white"><%=pageIndex %></a>
                        <%}
                            else
                            {%>
                        <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;"><%=pageIndex %></a>
                        <%} %>
                        <%} %>
                        <%=string.Format("{0} / {1}　                總共 {2} 筆", this.PagedList.PageNumber, this.PagedList.PageCount, this.PagedList.TotalItemCount) %>
                        <a id="btnNext" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.NextPage%>)">下一頁</a>
                        <a id="btnLast" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PagedList.PageCount %>)">最末頁</a>
                    </td>
                </tr>
                <%} %>--%>
            </tbody>
        </table>
        <table>
            <tbody>
                <tr>
                    <td>
                        <input type="button" id="AddButton" name="AddButton" class="IconNew" value="<%=GetGlobalResourceObject("Resource", "btnAdd") %>" onclick="CallAdd()" />
                    </td>
                    <td>
                        <input type="button" id="EditButton" name="EditButton" class="IconEdit" value="<%=GetGlobalResourceObject("Resource", "btnEdit") %>" onclick="CallEdit()" />
                    </td>
                    <td>
                        <input type="button" id="DeleteButton" name="DeleteButton" class="IconDelete" value="<%=GetGlobalResourceObject("Resource", "btnDelete") %>" onclick="CallDelete()" />
                    </td>
                </tr>
            </tbody>
        </table>
        <input type="hidden" id="MaxMobile" name="MaxMobile" value="<%=SahoAcs.DBClass.DongleVaries.GetMaxMobile() %>" />
        <input type="hidden" id="CurrentMobile" name="CurrentMobile" value="<%=SahoAcs.DBClass.DongleVaries.GetCurrentMobile() %>" />
    </div>
    <br />
    <div id="popOverlay" style="display: none; position: absolute; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0);"></div>

    <div id="ParaExtDiv1" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="角色資料" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <input type="image" name="ImgCloseButton1" id="ImgCloseButton1" src="/Img/close_button.png" style="height: 25px;" onclick="DoCancel('1'); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No" runat="server" Text="員工編號" Font-Bold="True"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="員工姓名" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>                                
                                <input type="text" id="TpPsnNo" name="TpPsnNo" class="TextBoxRequired" style="width:180px;border-width:1px" must_keyin_yn="Y" field_name="" />
                            </td>
                            <td>                                
                                <input type="text" id="TPPsnName" name="TPPsnName" class="TextBoxRequired" style="width:180px;border-width:1px" must_keyin_yn="Y" field_name="" />
                            </td>
                        </tr>
                        <tr class="IsMgaVacationDate">
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_VacationDate" runat="server" Text="例假日期" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr class="IsMgaVacationDate">
                            <td colspan="2">                                
                                <input type="text" id="MgaVacationDate" name="MgaVacationDate" style="width:370px;border-width:1px" />
                            </td>
                        </tr>
                        <tr class="IsOrgName">
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_MgaOrgName" runat="server" Text="部門名稱" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                         <tr class="IsOrgName">
                            <td colspan="2">                                
                                <input type="text" id="MgaOrgName" name="MgaOrgName" style="width:370px;border-width:1px" />
                            </td>
                         </tr>
                        <tr class="IsVacationDate">
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <span id="ContentPlaceHolder1_popLabel_Char2" style="font-weight: bold;">例假日期</span>
                            </th>
                        </tr>
                        <tr class="IsVacationDate">
                            <th colspan="2">
                                <select name="popDropDownList_Char2" id="popDropDownList_Char2" class="DropDownListStyle" style="width: 300px;" tabindex="0">
                                    <option value="0">請選擇例假日期</option>
                                </select>
                            </th>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Remark" runat="server" Text="備註" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <textarea name="Remark" rows="4" cols="20" id="Remark" style="width:100%;resize: none;" tabindex="0"></textarea>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">                        
                        <input type="button" name="popB_Add" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" id="popB_Add" class="IconSave" onclick="AddExcute()" />
                        <input type="button" name="popB_Edit" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" id="popB_Edit" class="IconSave" onclick="EditExcute()" />
                        <input type="button" name="popB_Delete" value="<%=GetGlobalResourceObject("Resource","btnDelete") %>" id="popB_Delete" class="IconSave" onclick="DeleteExcute()" />
                        <input type="button" id="popB_Cancel" name="popB_Cancel" class="IconCancel" value="<%=GetGlobalResourceObject("Resource","btnCancel") %>" onclick="DoCancel('1')" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
