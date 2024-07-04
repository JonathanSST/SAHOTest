<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewCalendar.ascx.cs" Inherits="SahoAcs.uc.NewCalendar" %>
<script src="/Scripts/jquery.colorbox-min.js"></script>
<link href="/Css/colorbox.css" rel="stylesheet" />
<script src="/uc/Calendar.js"></script>

<table id="ucCalendar" cellpadding="0" cellspacing="0">
	<tbody>
        <tr>
		<td>
            <input name="CalendarTextBox" type="text" id="CalendarTextBox" onclick="Call_Calendar('CalendarTextBox')"
                value="<%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",DateTime.Now) %>" onblur="CheckTime('CalendarTextBox')" style="width:138px;">
        </td>
		<td>
            <img id="CardTime_Clock" onclick="Call_Calendar('CalendarTextBox')" src="/Img/Clock.jpg" style="height:26px;width:28px;">
        </td>
	</tr>
</tbody></table>