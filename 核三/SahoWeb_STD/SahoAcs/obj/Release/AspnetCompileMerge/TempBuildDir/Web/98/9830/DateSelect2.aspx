<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DateSelect2.aspx.cs" Inherits="SahoAcs.Web._98._9830.DateSelect2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
       <div style="background-color:white">
        <table>
            <tr>
                <td>
                    <select id="DDListY" name="DDListY">
                        <%for (int i = 50; i >=0; i--)
                            { %>
                            <option value="<%=DateTime.Now.Year-i %>"><%=DateTime.Now.Year-i %></option>
                        <%} %>
                        <%for (int i = 1; i<50; i++)
                            { %>
                            <option value="<%=DateTime.Now.Year+i %>"><%=DateTime.Now.Year+i %></option>
                        <%} %>
                    </select>
                </td>
                <td>
                    <select id="DDListM" name="DDListM">
                        <%for (int i = 1; i <= 12; i++)
                            { %>
                        <option value="<%=string.Format("{0:00}",i) %>"><%=string.Format("{0:00}",i) %></option>
                        <%} %>
                    </select>
                </td>
            </tr>
        </table>
        <table style="background-color:blue">
            <tr>
                <td style="font-weight: bold; background-color: #D7D7D7; width: 30px">日</td>
                <td style="font-weight: bold; background-color: #D7D7D7; width: 30px">一</td>
                <td style="font-weight: bold; background-color: #D7D7D7; width: 30px">二</td>
                <td style="font-weight: bold; background-color: #D7D7D7; width: 30px">三</td>
                <td style="font-weight: bold; background-color: #D7D7D7; width: 30px">四</td>
                <td style="font-weight: bold; background-color: #D7D7D7; width: 30px">五</td>
                <td style="font-weight: bold; background-color: #D7D7D7; width: 30px">六</td>
            </tr>
            <%int ClassCount = 0;
                            List<DateTime> DateList = new List<DateTime>();
                            while (DateS <= DateE)
                            {
                                DateList.Add(DateS);
                                DateS = DateS.AddDays(1);
                            }
                            while (DateList[0].DayOfWeek > 0)
                            {
                                DateList.Insert(0, DateList[0].AddDays(-1));
                            }
            %>
            <%for (int i = 0; i < 6; i++)
                { %>
            <tr>
                <%for (int j = 0; j < 7; j++)
                    { %>
                <%ClassCount = i * 7 + j; %>
                <td style="font-weight: bold; width: 30px" id="Box<%=ClassCount+1 %>">
                 <%--   <%if (ClassCount < DateList.Count && DateList[ClassCount].ToString("MM").Equals(this.MyMonth))
                        { %>
                    <span><%=DateList[ClassCount].ToString("dd") %></span>
                    <%} %>--%>
                </td>
                <%} %>
            </tr>
            <%} %>
        </table>
        <span id="LaData"></span>
        <input type="hidden" id="para_date" name="para_date" value="<%=this.QueryDate%>" />
        <input type="button" id="EnterBtn" name="EnterBtn" value="確定" />
            </div>
    </form>
</body>
</html>
