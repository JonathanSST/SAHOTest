<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ClassScheduleAdj.aspx.cs" Inherits="SahoAcs.Web._97._9706.ClassScheduleAdj" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <span>工號-姓名</span>
            </th>
            <th>
                <span class="Arrow01"></span>
                <span>排班月份</span>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <select id="PsnNo" name="PsnNo" class="DropDownListStyle" style="width: 200px">
                    <%foreach (var o in this.PersonList)
                        { %>
                    <option value="<%=o.PsnNo %>"><%=string.Format("{1}({0})", o.PsnNo, o.PsnName) %></option>
                    <%} %>
                </select>
            </td>
            <td>
                <input type="text" id="ClassMonth" name="ClassMonth" value="<%=DateTime.Now.ToString("yyyy/MM") %>" />
                <input type="hidden" id="DefMonth" name="DefMonth" value="" />
                <input type="hidden" id="StateMonth" name="StateMonth" value="<%=this.NowMonth %>" />
            </td>
            <th>
                <input type="button" id="BtnQuery" name="BtnQuery" value="顯示當月預訂排班日" class="IconSearch" />       
                <input type="button" id="BtnJoin" name="BtnJoin" value="由本月開始重設分組排班" class="IconSetManage" />
                <input type="button" id="BtnReJoin" name="BtnReJoin" value="由次月開始合併至分組排班" class="IconSetManage" />
            </th>
        </tr>
    </table>
      <% List<string> ClassNoList = new List<string>();
                                                    ClassNoList.Add("0");
                                                    ClassNoList.Add("1");
                                                    ClassNoList.Add("2");
                                                    ClassNoList.Add("3");
                                                %>    
    <div id="MonthSchedule">        
        <%if (this.ClassList.Where(i => i.RecordID > 0).Count() > 0)
            { %>
        <%while ((int)this.ClassList.First().ClassDate.DayOfWeek > 0)
            {
                this.ClassList.Insert(0, new SahoAcs.DBModel.B03Empclassschedule() { RecordID = 0, ClassDate = this.ClassList.First().ClassDate.AddDays(-1) });
            }%>
        <table>
            <tr>
                <td style="border: 1px solid">
                    <table class="tablespacing2" style="width: 100%; text-align: center; border: 1px solid white;">
                        <tr>
                            <td style="font-weight: bold; background-color: #D7D7D7; width: 108px">日</td>
                            <td style="font-weight: bold; background-color: #D7D7D7; width: 108px">一</td>
                            <td style="font-weight: bold; background-color: #D7D7D7; width: 108px">二</td>
                            <td style="font-weight: bold; background-color: #D7D7D7; width: 108px">三</td>
                            <td style="font-weight: bold; background-color: #D7D7D7; width: 108px">四</td>
                            <td style="font-weight: bold; background-color: #D7D7D7; width: 108px">五</td>
                            <td style="font-weight: bold; background-color: #D7D7D7; width: 108px">六</td>
                        </tr>
                        <%int ClassCount = 0; %>
                        <%for (int i = 0; i < 5; i++)
                            { %>
                        <tr>
                            <%for (int j = 0; j < 7; j++)
                                { %>                     
                            <%ClassCount = i * 7 + j; %>
                            <td style="font-weight: bold; background-color:blue;color:white; width: 108px">
                                <%if (ClassCount < this.ClassList.Count && this.ClassList[ClassCount].RecordID != 0)
                                    { %>
                                <%=this.ClassList[ClassCount].ClassDate.ToString("yyyy/MM/dd") %><br />
                                預排班別：<%=this.ClassList[ClassCount].ClassNo.Equals("0") ? "休" : this.ClassList[ClassCount].ClassNo %><br />
                                調班後班別：
                                 <%if (new TimeSpan(DateTime.Now.Ticks - this.ClassList[i * 7 + j].ClassDate.Ticks).Days <= -2)
                                     { %>
                                <input type="hidden" value="<%=this.ClassList[ClassCount].RecordID %>" id="RecordID" name="RecordID" />
                                <select id="AdjClassNo" name="AdjClassNo" class="DropDownListStyle">
                                    <%foreach (var o in ClassNoList)
                                        { %>
                                    <%if (o.Equals(this.ClassList[ClassCount].AdjClassNo))
                                        { %>
                                    <option selected="selected" value="<%=o %>"><%=o.Equals("0") ? "休" : o %></option>
                                    <%}
                                        else
                                        { %>
                                    <option value="<%=o %>"><%=o.Equals("0") ? "休" : o %></option>
                                    <%} %>
                                    <%} %>
                                </select>
                                <%}
                                    else
                                    { %>
                                <%=this.ClassList[ClassCount].AdjClassNo.Equals("0") ? "休" : this.ClassList[ClassCount].AdjClassNo %>
                                <%} %>
                                <%} %>
                            </td>
                            <%} %>
                        </tr>
                        <%} %>
                    </table>                    
                </td>
            </tr>
        </table> 
        <table>
            <tr>
                <td>
                  <%--  <input type="button" id="BtnModifySchedule" name="BtnModifySchedule" value="修改排班" class="IconSave"/>--%>
                </td>
            </tr>
        </table>
        <%}
            else
            { %>
            <span style="color:white; font-size:14pt">本月份查無排班紀錄</span>
        <%} %>
    </div>
    <div id="dvContent" style="position: absolute; left: 20px; top: 30px; z-index: 1000000000"></div>
    <input type="hidden" name="PageEvent" value="Save" />
</asp:Content>
 