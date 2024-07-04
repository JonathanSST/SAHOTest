<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryOsListZZ01.aspx.cs" Inherits="SahoAcs.QueryOsListZZ01" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>
<%@ Import Namespace="SahoAcs.DBClass" %>


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
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label5" runat="server" Text="公司"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label4" runat="server" Text="職稱"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label3" runat="server" Text="狀態"></asp:Label>
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
                            <input type="text" name="inputPsnCard" id="inputPsnCard" />
                        </td>                       
                        <td>
                            <select id="QueryComp" name="QueryComp" class="DropDownListStyle">
                                <option value="0">&nbsp;</option>
                                <%foreach (var o in this.odo.GetQueryResult<SahoAcs.DBModel.OrgDataEntity>(@"SELECT 
	                                                                                                        B.* 
                                                                                                        FROM 
	                                                                                                        OrgStrucAllData('Title') AS A
	                                                                                                        INNER JOIN OrgStrucAllData('Company') AS B ON A.OrgStrucID=B.OrgStrucID
                                                                                                        WHERE A.OrgNo IN @TitleList ", new { TitleList = WebAppService.GetSysParaData("OsTitle").Split(',') }).ToList())
                                    { %>
                                    <option value="<%=o.OrgNo %>"><%=string.Format("{1}_({0})",o.OrgNo,o.OrgName) %></option>                                                              
                                <%} %>                                
                            </select>
                        </td>
                        <td>
                            <select id="QueryTitle" name="QueryTitle" class="DropDownListStyle">
                                <%foreach (var o in this.odo.GetQueryResult(@"SELECT * FROM B01_OrgData WHERE OrgClass='Title' AND OrgNo IN @TitleList",new {TitleList = WebAppService.GetSysParaData("OsTitle").Split(',')}))
                                    { %>
                                    <option value="<%=Convert.ToString(o.OrgNo) %>"><%=string.Format("{1}_({0})",o.OrgNo,o.OrgName) %></option>                                                              
                                <%} %>
                                <%foreach (var o in this.odo.GetQueryResult(@"SELECT * FROM B01_OrgData WHERE OrgClass='Title' AND OrgNo NOT IN @TitleList ",new {TitleList = WebAppService.GetSysParaData("OsTitle").Split(',')}))
                                    { %>
                                    <option value="<%=Convert.ToString(o.OrgNo) %>"><%=string.Format("{1}_({0})",o.OrgNo,o.OrgName) %></option>                                                              
                                <%} %>
                                <option value="0">&nbsp;</option>
                            </select>
                        </td>
                         <td>
                            <select id="QueryLogStatus" name="QueryLogStatus" class="DropDownListStyle">
                                <option value="正常">正常</option>
                                <option value="異常">異常</option>                                
                                <option value="">&nbsp;</option>
                            </select>
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
        <table class="TableS1" style="width:1350px">
            <tbody>
                <tr class="GVStyle">                  
                    <th scope="col" class="TitleRow" style="width:105px">
                        公司
                    </th>
                    <th scope="col" class="TitleRow" style="width:105px">
                        職稱
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width:80px">
                        狀態
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        入廠日期
                    </th>                  
                    <th scope="col" class="TitleRow" style="width:100px">
                        入廠時間
                    </th>
                    <th scope="col" class="TitleRow"  style="width:150px">
                        入廠設備
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        出廠日期
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        出廠時間
                    </th>
                    <th scope="col" class="TitleRow"  style="">
                        出廠設備
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="11">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width:1350px; overflow-y: scroll;">
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
                                        <%foreach (var o in this.ListLog.OrderBy(i=>i.PsnName).OrderBy(i=>i.NewTitle).OrderBy(i=>i.CardDate).OrderBy(i=>i.CompNo).ToList())
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">                                                                                        
                                            <td style="width:108px">
                                                 <%=o.CompName %>
                                            </td>
                                            <td style="width:109px">
                                                 <%=o.TitleName %>
                                            </td>
                                            <td style="width:104px">
                                                 <%=o.PsnName %>
                                            </td>
                                            <td style="width:84px">                                                
                                                 <%=o.LogStatus %>
                                            </td>                                            
                                            <td style="width:104px">
                                                 <%=!string.IsNullOrEmpty(o.First)?o.CardDate:"" %>
                                            </td>
                                            <td style="width:104px">
                                                 <%=o.FirstReal %>
                                            </td>
                                            <td style="width:154px">
                                                 <%=o.EquName %>
                                            </td>
                                             <td style="width:104px">
                                                 <%=!string.IsNullOrEmpty(o.Last)?o.CardDate:"" %>
                                            </td>                
                                            <td style="width:104px">
                                                <%=o.LastReal %>
                                            </td>
                                            <td>
                                                <%=o.EquName2 %>
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
