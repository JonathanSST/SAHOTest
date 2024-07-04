<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryInOutList.aspx.cs" Inherits="SahoAcs.QueryInOutList" %>

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
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="人員編號或姓名"></asp:Label>
                        </th>
                        <td></td>
                        <td rowspan="2" style="">
                            <table class="popItem">
                                <tbody>
                                    <tr>
                                        <th style="width: 150px">今日未回廠
                                        </th>
                                        <th>
                                            <span id="OutCount">0</span>
                                        </th>
                                    </tr>
                                    <tr>
                                        <th style="width: 150px">未回廠合計
                                        </th>
                                        <th>
                                            <span id="OutTotalCount">0</span>
                                        </th>
                                    </tr>
                                    <tr>
                                        <th colspan="2">
                                            <input type="button" class="IconChange" value="更新" onclick="SetRefresh()" /></th>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
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
                            <input type="text" name="inputPsnCard" id="inputPsnCard" />
                        </td>
                       <%-- <td>
                            <select class="DropDownListStyle" style="width:150px" id="AbnormalType" name="AbnormalType">
                                <option value="">全部</option>
                                <option value="0">刷進未刷出</option>
                                <option value="1">刷出未刷進</option>
                                <option value="2">入廠一小時</option>
                            </select>
                        </td>--%>
                        <td>                            
                            <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetMode(1)" />
                        </td>                        
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width:1050px">
            <tbody>
                <tr class="GVStyle">                  
                    <th scope="col" class="TitleRow" style="width:135px">
                        讀卡日期
                    </th>                    
                    <th scope="col" class="TitleRow" style="width:120px">
                        人員編號
                    </th>
                    <th scope="col" class="TitleRow" style="width:135px">
                        人員名稱
                    </th>
                    <th scope="col" class="TitleRow" style="width:130px">
                        部門
                    </th>
                    <th scope="col" class="TitleRow" style="width:120px">
                        卡片號碼
                    </th>                  
                    <th scope="col" class="TitleRow" >
                        出廠
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="6">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1050px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.ListLog.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="11">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.ListLog)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">                                                                                        
                                            <td style="width:139px">
                                                 <%=string.Format("{0:yyyy/MM/dd}",o.CardTime) %>
                                            </td>
                                            <td style="width:124px">
                                                 <%=o.PsnNo %>
                                            </td>
                                            <td style="width:139px">
                                                 <%=o.PsnName %>
                                            </td>                                            
                                            <td style="width:134px">
                                                 <%=o.DepName%>
                                            </td>                                            
                                            <td style="width:124px">
                                                 <%=o.CardNo%>
                                            </td>
                                            <td>
                                                 <%=string.Format("{0:HH:mm:ss}",o.CardTime)%>
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
             
                <%} %>
            </tbody>
        </table>
    </div>
    <div>        
        <input type="button" name="ExportButton" value="匯　出" id="ExportButton" class="IconExport" onclick="SetPrint()" />
    </div>    
     <div id="popOverlay" style="display:none;position:fixed; top:0; left:0; z-index:29999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>
    <div id="ParaExtDiv4" style="display:none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069; top:30px;left:930px">
    <asp:Panel ID="Panel1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="進出統計" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                       <%-- <asp:ImageButton ID="ImgCloseButton3" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />--%>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        
    </div>
</asp:Content>
