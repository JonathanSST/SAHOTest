<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PsnItinerary.aspx.cs" Inherits="SahoAcs.PsnItinerary" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" href="https://cdn.rawgit.com/openlayers/openlayers.github.io/master/en/v5.2.0/css/ol.css" type="text/css">
    <style>
        .map {
            height: 800px;
            width: 100%;
        }
    </style>
    <script src="https://cdn.rawgit.com/openlayers/openlayers.github.io/master/en/v5.2.0/build/ol.js"></script>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="SortDataContext" name="SortDataContext" value="" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Query" />
        <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
        <input type="hidden" id="DateS" name="DateS" value="" />
        <input type="hidden" id="DateE" name="DateE" value="" />
        <input type="hidden" id="MgaName" name="MgaName" value="" />
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
        <input type="hidden" id="PsnID" name="PsnID" value="<%=this.PsnID %>" />
        <input type="hidden" name="AuthList" id="AuthList" value="<%=this.AuthList %>" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="打卡日期"></asp:Label>
                        </th>
                        <%if (string.IsNullOrEmpty(this.PsnID))
                            { %>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="工號"></asp:Label>
                        </th>
                        <%} %>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <uc2:CalendarFrm runat="server" ID="CardDateS" />
                        </td>
                        <th>~
                        </th>
                        <td>
                            <uc2:CalendarFrm runat="server" ID="CardDateE" />
                        </td>
                        <%if (string.IsNullOrEmpty(this.PsnID))
                            { %>
                        <td>
                            <select id="QueryPsnNo" name="QueryPsnNo" class="DropDownListStyle">
                                <%foreach (var o in this.ListPsnData)
                                    { %>
                                <option value="<%=o.PsnNo %>"><%=string.Format("{1}  ({0})",o.PsnNo,o.PsnName) %></option>
                                <%} %>
                            </select>
                        </td>
                        <%} %>
                        <td>
                            <input type="button" name="BtnPrint" value="查  詢" id="BtnPrint" class="IconSearch" onclick="SetQuery()" />
                            <input type="button" name="ExportButton" value="列  印" id="ExportButton" class="IconExport" onclick="SetPrint()" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="UpdatePanel1">
        <table class="TableS1" style="width: 1150px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" class="TitleRow" style="width: 105px">打卡項目
                    </th>
                    <th scope="col" class="TitleRow" style="width: 120px">打卡時間
                    </th>
                    <th scope="col" class="TitleRow" style="width: 420px">打卡地點
                    </th>
                    <th scope="col" class="TitleRow" style="">兩地距離
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="4">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1150px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if (this.ListLog.Count == 0)
                                            { %>
                                        <tr class="DataRow">
                                            <td colspan="4">
                                                <%=GetGlobalResourceObject("Resource", "NonData").ToString() %>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <%foreach (var o in this.ListLog)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">
                                            <td style="width: 109px">
                                                <%=o.StateDesc %>
                                            </td>
                                            <td style="width: 124px">
                                                <%=o.LocalTime.Value.ToString("yyyy/MM/dd HH:mm:ss") %>
                                            </td>
                                            <td style="width: 424px">
                                                <input type="text" name="Note" id="Note" style="width: 90%" value="<%=o.Note %>" />
                                                <input type="hidden" name="NowRecordID" id="NowRecordID" value="<%=o.NowRecordID %>" />
                                            </td>
                                            <td style="">
                                                <%=o.Distance %>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <%if (!string.IsNullOrEmpty(this.PsnID))
            { %>
        <table>
            <tr>
                <td>
                    <input type="button" name="BtnSave" value="修  改" id="BtnSave" class="IconSave" onclick="SaveNoteList()" />
                </td>
            </tr>
        </table>
        <%} %>
    </div>
    <div id="popOverlay" style="display: none; position: absolute; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0);"></div>
</asp:Content>
