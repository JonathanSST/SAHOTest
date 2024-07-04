<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddCourseLog.aspx.cs" Inherits="SahoAcs.Web.AddCourseLog" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register src="/uc/CalendarFrm.ascx" tagname="NewCalendar" tagprefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th colspan="2">                                
                                <span class="Arrow01"></span>
                                <span>工號</span>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <span>姓名</span>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="text" name="PsnNo" id="PsnNo" value="" style="width:130px" />
                                <input type="hidden" name="CardID" id="CardID" value="" />
                            </td>
                            <td>
                                <input type="button" class="IconSearch" onclick="QueryPsnCard()" value="查詢" />
                            </td>
                            <td>
                                <input type="text" name="PsnName" id="PsnName" value="" style="width:190px" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>            
            <tr>
                <td>
                    <table >
                        <tr>                           
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_LogStatus" runat="server" Text="課程名稱" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>                            
                            <td>                                
                                <select id="CourseName" name="CourseName" style="width:200px" class="DropDownListStyle">
                                    <%foreach(var o in this.ListCourse) { %>
                                        <option value="<%=o.CourseID %>"><%=string.Format("{0}....{1}",o.CourseName,o.EquName) %></option>
                                    <%} %>
                                    <%if(this.ListCourse.Count==0) { %>
                                        <option value="0">無資料</option>
                                    <%} %>
                                </select>
                            </td>                            
                        </tr>
                    </table>
                </td>
            </tr>           
            <tr>
                <td>                
                    <input type="button" name="BtnSave" value="<%=Resources.Resource.btnSave %>"  id="BtnSave" class="IconSave"/>
                    <input type="button" name="BtnCancel" value="<%=Resources.Resource.btnCancel %>"  id="BtnCancel" class="IconCancel"/>                          
                </td>
            </tr>
        </table>    
    </div>
       
    </form>
</body>
</html>
