<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ClassScheduleReport2.aspx.cs" Inherits="SahoAcs.Web._97._9706.ClassScheduleReport2" Theme="UI" %>

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
                <span>單位</span>
            </th>
            <th>
                <span class="Arrow01"></span>
                <span>排班月份</span>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <input type="text" id="PsnNo" name="PsnNo" value="" />
            </td>
            <td>
                <select style="width: 230px" class="DropDownListStyle" id="DeptList" name="DeptList">
                    <option value=""><%=Resources.Resource.ddlSelectDefault %></option>
                    <%foreach (var o in this.OrgDataInit.Where(i => i.OrgClass.Equals("Unit")))
                        { %>
                    <option value="<%=o.OrgStrucID %>"><%=o.OrgName+"."+o.OrgNo%></option>
                    <%} %>
                </select>
            </td>
            <td>
                <input type="text" id="ClassMonth" name="ClassMonth" value="<%=DateTime.Now.ToString("yyyy/MM") %>" />
                <input type="hidden" id="DefMonth" name="DefMonth" value="" />
                <input type="hidden" id="StateMonth" name="StateMonth" value="<%=this.NowMonth %>" />
            </td>
            <th>
                <input type="button" id="BtnQuery" name="BtnQuery" value="查詢排班表" class="IconSearch" />               
                <input type="button" id="BtnPrint" name="BtnPrint" value="列印" class="IconExport" />
            </th>
        </tr>
    </table>
      <% List<string> ClassNoList = new List<string>();
                                                    ClassNoList.Add("0");
                                                    ClassNoList.Add("1");
                                                    ClassNoList.Add("2");
                                                    ClassNoList.Add("3");
                                                %>
    <table>
        <tr>
            <td style="height:400px;width: 1120px; vertical-align: top" id="RptArea">
                <div id="ScrollArea" style="overflow-x:scroll;overflow-y:scroll; width:inherit; height:inherit">                
                <table class="TableS1" style="width:1800px">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 105px">日期</th>
                            <%var DateCs = Date1;
                                string WeekName = "日一二三四五六";
                                %>
                            <%while (DateCs <= Date2)
                                { %>
                            <th scope="col" style="width: 40px">
                                <%=DateCs.ToString("dd") %>
                            </th>
                            <% DateCs = DateCs.AddDays(1);
                                } %>
                            <th scope="col" rowspan="2">員編</th>
                        </tr>
                        <tr class="GVStyle">
                            <th scope="col">姓名/星期</th>
                            <% DateCs = Date1; %>
                             <%while (DateCs <= Date2)
                                { %>
                            <th scope="col">
                                <%= WeekName.Substring((int)DateCs.DayOfWeek,1) %>
                            </th>
                            <% DateCs = DateCs.AddDays(1);
                                } %>
                        </tr>
                        <%foreach (var p in this.PersonList)
                            { %>
                        <tr class="GVStyle">
                            <td>
                                <%=p.PsnName %>
                            </td>
                            <% DateCs = Date1; %>
                             <%while (DateCs <= Date2)
                                { %>
                            <td>
                               <%var showday = "";
                                   if (p.Text5.Equals("ABCR") || p.Text5.Equals("0123"))
                                   {
                                       if (this.ClassList.Where(i =>i.PsnNo.Equals(p.PsnNo)&& i.ClassDate.ToString("yyyy/MM/dd").Equals(DateCs.ToString("yyyy/MM/dd"))).Count() > 0)
                                       {
                                           showday = this.ClassList.Where(i =>i.PsnNo.Equals(p.PsnNo)&& i.ClassDate.ToString("yyyy/MM/dd").Equals(DateCs.ToString("yyyy/MM/dd"))).First().ClassNo;
                                           if (showday.Equals("0"))
                                           {
                                               showday = "休";
                                           }
                                           //showday += "<br/>"+this.ClassList.Where(i => i.ClassDate.ToString("yyyy/MM/dd").Equals(DateCs.ToString("yyyy/MM/dd"))).First().AdjClassNo;
                                       }
                                   }
                                   else
                                   {
                                       showday = p.Text5;
                                       if (this.HolidayList.Where(i => i.HEDate.Equals(DateCs.ToString("yyyy-MM-dd"))).Count() > 0)
                                       {
                                           showday = "例";
                                       }
                                   }

                                   %>
                                <%=showday %>
                            </td>
                            <% DateCs = DateCs.AddDays(1);
                                } %>
                            <td>
                                <%=p.PsnNo %>
                            </td>
                        </tr>
                        <%} %>
                    </tbody>
                </table>
                    </div>
            </td>
        </tr>
    </table>
    <div id="dvContent" style="position: absolute; left: 20px; top: 30px; z-index: 1000000000"></div>
    <input type="hidden" id="PageEvent" name="PageEvent" value="Save" />
</asp:Content>
 