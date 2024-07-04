<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryWorkRng.aspx.cs" Inherits="SahoAcs.QueryWorkRng" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/PickDate.ascx" TagPrefix="uc1" TagName="PickDate" %>
<%@ Register src="../../../uc/CalendarFrm.ascx" tagname="CalendarFrm" tagprefix="uc2" %>

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
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="啟始日"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="結束日"></asp:Label>
                        </th>                                                                        
                        <td></td>
                    </tr>
                    <tr>                              
                        <td>
                            <uc2:CalendarFrm ID="PickDateS" runat="server" />
                        </td>
                        <td>
                            <uc2:CalendarFrm ID="PickDateE" runat="server" />
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
        <table class="TableS1" style="width:1050px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" class="TitleRow" style="width:110px">
                        部門
                    </th>
                    <th scope="col" class="TitleRow" style="width:110px">
                        部門名稱
                    </th>                    
                    <th scope="col" class="TitleRow" style="width:105px">
                        編號
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width:105px">
                        日期
                    </th>                    
                    <th scope="col" class="TitleRow" style="width:120px">
                        上班
                    </th>
                    <th scope="col" class="TitleRow" style="width:120px">
                        下班
                    </th>
                    <th scope="col" class="TitleRow" style="">
                        備註
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="11">
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
                                            <tr class="DataRow" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">                                                
                                                <td style="width:114px">
                                                     <%=o.OrgNo %>
                                                </td>
                                                <td style="width:114px">
                                                     <%=o.OrgName %>
                                                </td>
                                                <td style="width:109px">
                                                     <%=o.PsnNo %>
                                                </td>
                                                <td style="width:104px">
                                                     <%=o.PsnName %>
                                                </td>
                                                <td style="width:109px">
                                                     <%=o.CardDate %>
                                                </td>
                                                <td style="width:124px">
                                                     <%=o.RealTimeS %>
                                                </td>
                                                <td style="width:124px">
                                                     <%=o.RealTimeE %>
                                                </td>                                                
                                                <td>
                                                    <%string Desc = "";
                                                        if (o.RealTimeS == string.Empty || o.RealTimeE == string.Empty)
                                                        {
                                                            Desc = "未打卡";
                                                        }
                                                        else if (o.RealTimeS.CompareTo("08:31:01") >= 0)
                                                        {
                                                            Desc = "遲到";
                                                        }
                                                        else if (o.RealTimeE.CompareTo("18:26:01") >= 0)
                                                        {
                                                            Desc = "下班時間超時";
                                                        }
                                                        else if (o.RealTimeS.CompareTo("07:30:00") <= 0)
                                                        {
                                                            Desc = "提早上班打卡";
                                                        }                                                        
                                                         %>
                                                    <%=Desc %>
                                                </td>
                                            </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr class="GVStyle">
                     <td colspan="11">
                        <%=string.Format("共{0} 筆紀錄",this.ListLog.Count) %>
                    </td>                    
                </tr>
            </tbody>
        </table>
    </div>  
    <br />
    <div>        
        <input type="button" name="ExportButton" value="匯　出" id="ExportButton" class="IconExport" onclick="SetPrint()" />
    </div>
    <div id="popOverlay" style="display:none;position:absolute; top:0; left:0; z-index:29999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>
     <div id="dvContent" style="position:absolute;left:20px;top:30px;z-index:1000000000" ></div>
</asp:Content>
