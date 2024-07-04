<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseEdit.aspx.cs" Inherits="SahoAcs.Web.CourseEdit" %>

<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="/uc/CalendarFrm.ascx" TagName="NewCalendar" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
<form runat="server" id="EditForm">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div id="CourseEdit">
        <input type="hidden" id="PageEvent" name="PageEvent" value="Save" />
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <span>課程名稱</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>課程期別</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input name="CourseName" type="text" id="CourseName" style="width: 220px;" must_keyin_yn="Y" data_type="C" field_name="課程名稱" value="<%=entity.CourseName %>" />
                            </td>
                            <td>
                                <input name="Season" type="text" id="Season" style="width: 120px;" must_keyin_yn="Y" data_type="C" field_name="課程期別" value="<%=entity.Season %>" />
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
                                <span>課程類型</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>刷卡設備</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input name="CourseType" type="text" id="CourseType" style="width: 180px;" must_keyin_yn="Y" data_type="C" field_name="課程類型" value="<%=entity.CourseType %>" />
                            </td>
                            <td>
                                <select name="EquID" id="EquID" class="DropDownListStyle" style="width: 150px;">
                                    <%foreach (var o in this.ListEqu)
                                        { %>
                                    <option value="<%=o.EquID %>"><%=o.EquName %></option>
                                    <%} %>
                                </select>
                                <input type="hidden" id="EquName" name="EquName" value="<%=this.entity.EquID %>" />
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
                                <span>課程編號</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>所在縣市</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input name="CourseNo" type="text" id="CourseNo" style="width: 220px;"  value="<%=entity.CourseNo %>" />
                            </td>
                            <td>
                                <select name="City" id="City" class="DropDownListStyle" style="width: 140px">
                                    <%foreach (var o in this.Cities)
                                        { %>
                                        <option value="<%=o.CityVal %>"><%=o.CityName %></option>
                                    <%} %>
                                </select>
                                <input type="hidden" id="CityName" name="CityName" value="<%=this.entity.City %>" />
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
                                <span>開課起始時間</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>開課結束時間</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <uc1:Calendar ID="CourseTimeS" runat="server" />
                            </td>
                            <td>
                                <uc1:Calendar ID="CourseTimeE" runat="server" />
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
                                <span>開課實際起始時間</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>開課實際結束時間</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <uc1:Calendar ID="CourseRealTimeS" runat="server" />
                            </td>
                            <td>
                                <uc1:Calendar ID="CourseRealTimeE" runat="server" />
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
                                <span>學分學位</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>學習性質</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input name="CourseScore" type="text" id="CourseScore" style="width: 120px;" must_keyin_yn="Y" data_type="C" field_name="學分學位" value="<%=entity.CourseScore %>" />
                            </td>
                            <td>
                                <input name="CourseProp" type="text" id="CourseProp" style="width: 120px;" must_keyin_yn="Y" data_type="C" field_name="學習性質" value="<%=entity.CourseProp %>"/>
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
                                <span>訓練總數單位</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>訓練總數</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input name="CourseUnit" type="text" id="CourseUnit" style="width: 120px;" must_keyin_yn="Y" data_type="C" field_name="訓練課程單位" value="<%=entity.CourseUnit %>"/>
                            </td>
                            <td>
                                <input name="CourseAmount" type="text" id="CourseAmount"  style="width: 120px;"  must_keyin_yn="Y" data_type="N" field_name="訓練總數" value="<%=entity.CourseAmount %>"/>
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
                                <span>數位時數</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>實體時數</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input name="DigitHour" type="text" id="DigitHour" style="width: 120px;"  value="<%=entity.DigitHour %>" must_keyin_yn="Y" data_type="N" field_name="數位時數"  />
                            </td>
                            <td>
                                <input name="RealHour" type="text" id="RealHour" style="width: 120px;" value="<%=entity.RealHour %>" must_keyin_yn="Y" data_type="N" field_name="實體時數" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <input type="button" name="BtnSave" value="<%=Resources.Resource.btnSave %>" id="BtnSave" class="IconSave" />
                    <input type="button" name="BtnCancel" value="<%=Resources.Resource.btnCancel %>" id="BtnCancel" class="IconCancel" />
                    <input type="hidden" name="CourseID" id="CourseID" value="<%=this.entity.CourseID%>" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
