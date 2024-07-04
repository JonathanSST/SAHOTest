<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HolidayEdit.aspx.cs" Inherits="SahoAcs.Web._01._0118.HolidayEdit" %>
<%@ Register Src="../../../uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form_card" runat="server">
        <div id="PanelPopup1"style="border-width: 1px; border-style: solid;">
            <div id="PanelDrag1" style="background-color: #2D89EF; height: 28px;">
                <table style="width: 100%; padding: 0px;">
                    <tbody>
                        <tr>
                            <td>
                                <span id="ContentPlaceHolder1_L_popName1" style="color: White; font-weight: bold; vertical-align: middle"><%=string.Format("{0}", this.PsnName, this.PsnNo) %>休假資料編輯</span>
                            </td>
                            <td style="text-align: right;"></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <table>
                <tbody>
                    <tr>
                        <td>
                            <table class="popItem">
                                <tbody>
                                    <tr>
                                        <td>
                                            <table style="padding: 0px; border-collapse: collapse;">
                                                <tbody>
                                                     <tr>
                                                        <th colspan="4">
                                                            <span id="lblHolidayPsn" style="font-weight: bold;">請休假人員</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="width:450px">
                                                            <input type="text" id="PsnName" name="PsnName" placeholder="點擊兩下查詢，並選擇人員工號" style="width:200px" value="<%=this.HolidayEntity.PsnName %>"/>
                                                            <input type="hidden" id="PsnNo" name="PsnNo" value="<%=this.HolidayEntity.PsnNo %>" /><span id="ShowPsnNo" style="color:white"><%=this.HolidayEntity.PsnNo %></span>
                                                        </td>
                                                        <td colspan="2"></td>
                                                    </tr>
                                                     <tr>
                                                        <th colspan="4">
                                                            <span id="lblHolidayClass" style="font-weight: bold;">假別</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <select id="HoliNo" name="HoliNo" class="DropDownListStyle">
                                                                <%foreach (var o in this.VacList)
                                                                    { %>
                                                                <%if (Convert.ToString(o.VNo).Equals(this.HolidayEntity.HoliNo))
                                                                    { %>
                                                                <option value="<%=o.VNo %>" selected="selected"><%=o.VName %></option>
                                                                <%}
                                                                else
                                                                { %>
                                                                <option value="<%=o.VNo %>"><%=o.VName %></option>
                                                                <%} %>
                                                                <%} %>
                                                            </select>
                                                        </td>
                                                        <td colspan="2"></td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="lblStartDate" style="font-weight: bold;">啟始日期時間</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <uc1:Calendar ID="MainStartTime" runat="server" />
                                                        </td>
                                                        <td colspan="2"></td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="lblEndDate" style="font-weight: bold;">訖止日期時間</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <uc1:Calendar ID="MainEndTime" runat="server" />
                                                        </td>
                                                        <td colspan="2"></td>
                                                    </tr>                                                   
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="lblDays" style="font-weight: bold;">天數</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <input type="text" id="Daily" name="Daily" value="<%=this.HolidayEntity.Daily %>"  data_type="N" field_name="天數" />
                                                        </td>
                                                        <td colspan="2"></td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="2">
                                                            <span id="lblHour" style="font-weight: bold;">時數</span>
                                                        </th>
                                                        <th colspan="2">                                                            
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <input type="text" id="Hours" name="Hours" value="<%=this.HolidayEntity.Hours %>"  data_type="N" field_name="時數"/>
                                                        </td>
                                                        <td colspan="2">                                                            
                                                        </td>
                                                    </tr>
                                                   <tr>
                                                        <th colspan="2">
                                                            <span id="lblMinute" style="font-weight: bold;">分數</span>
                                                        </th>
                                                        <th colspan="2">                                                            
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <input type="text" id="Minutes" name="Minutes" value="<%=this.HolidayEntity.Minutes %>" data_type="N" field_name="分數"/>
                                                        </td>
                                                        <td colspan="2">                                                            
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <div id="ContentPlaceHolder1_PanelEdit1">
                                                <input type="button" name="popB_Add" value="儲     存" id="popB_Add" class="IconSave" />
                                                <%if (this.HolidayEntity.RecordID != 0)
                                                    { %>
                                                <input type="button" name="popB_Delete" value="刪     除" id="popB_Delete" class="IconDelete" style="display: inline;" />
                                                <%} %>
                                                <input type="button" name="popB_Cancel" value="取     消" class="IconCancel" id="popB_Cancel" />                                                                   
                                                <input type="hidden" id="ErrMsg" value="<%=this.ErrMsg %>" />
                                                <input type="hidden" id="DoAction" name="DoAction" value="Save" />
                                                <input type="hidden" id="StartTime" name="StartTime" value="" />
                                                <input type="hidden" id="SelectValue" name="SelectValue" value="" />
                                                <input type="hidden" id="RecordID" name="RecordID" value="<%=this.HolidayEntity.RecordID %>" />
                                                <input type="hidden" id="EndTime" name="EndTime" value="" />
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
