<%@ Page Title="" Language="C#" Theme="UI" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="GroupClassSchedule.aspx.cs" Inherits="SahoAcs.Web._01._0119.GroupClassSchedule" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <span>組別</span>
            </th>
            <th>
                <span class="Arrow01"></span>
                <span>開始指定月份</span>
            </th>
            <th>
                <span class="Arrow01"></span>
                <span>結束指定月份</span>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <select id="GroupName" name="GroupName" class="DropDownListStyle" style="width: 200px" onchange="chgGroupName(this)">
                    <%foreach (var o in this.OrgDataList)
                        { %>
                    <option value="<%=o.OrgNo %>"><%=string.Format("{1}({0})", o.OrgNo, o.OrgName) %></option>
                    <%} %>
                </select>
            </td>
            <td>
                <input type="text" id="ClassMonth" name="ClassMonth"  />
                <input type="hidden" id="DefMonth" name="DefMonth" value="" />
                <input type="hidden" id="StateMonth" name="StateMonth" value="<%=this.NowMonth %>" />
            </td>
             <td>
                <input type="text" id="EndMonth" name="EndMonth"  />
                <input type="hidden" id="DefEndMonth" name="DefEndMonth" value="" />
                <input type="hidden" id="StateEndMonth" name="StateEndMonth" value="<%=this.NowMonth %>" />
            </td>
            <th>
                <input type="button" id="BtnQuery" name="BtnQuery" value="顯示排班資料" class="IconSearch" />
                <input type="button" id="BtnCreate" name="BtnCreate" value="產生班表" class="IconSet" />
            </th>
        </tr>
    </table>
      <% List<string> ClassNoList = new List<string>();
          ClassNoList.Add("0");  //休
          ClassNoList.Add("1");  //輪班晚班
          ClassNoList.Add("2");  //輪班早班
          ClassNoList.Add("3");  //輪班中班
          ClassNoList.Add("E");  //正常班
          ClassNoList.Add("F");  //工讀生
          ClassNoList.Add("G");  //其他
        
                                                %>
    <table style="">
        <tr>
            <td style="height:40px;width: 1120px; vertical-align: top" id="EquArea">
                <table class="TableS1">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 105px">年月</th>
                            <th scope="col" style="width: 105px">工作日</th>
                            <th scope="col" style="width: 105px">班別</th>
                            <th scope="col">備註</th>
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="5">
                                <div id="ContentPlaceHolder1_tablePanel2" style="width: 1120px; overflow-y: scroll;">
                                    <div id="ResultData">
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                            id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                            <tbody>
                                                <%foreach (var work in this.ClassLast2)
                                                    { %>
                                                <tr>
                                                    <td style="width: 109px">
                                                        <%=work.ClassMonth %>
                                                        <input type="hidden" id="ClassLast" name="ClassLast" value="<%=work.ClassMonth %>" />
                                                    </td>
                                                    <td style="width: 109px">
                                                        <%=work.ClassDate.ToString("yyyy/MM/dd") %>
                                                    </td>
                                                    <td style="width: 109px;">
                                                        <span <%if (work.ClassNo.Equals("0"))
                                                            { %>
                                                            style="color: red"
                                                            <%}%>>
                                                            <%=work.ClassNo.Equals("0")?"休":work.ClassNo %>
                                                        </span>
                                                        <input type="hidden" id="ClassNoDef" name="ClassNoDef" value="<%=work.ClassNo %>" />
                                                    </td>
                                                    <td>    
                                                        前期(月)最後兩天班
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
            </td>
        </tr>
    </table>
    <div id="MonthSchedule">        
        <%if (this.ClassList.Where(i=>i.RecordID>0).Count()>0)
            { %>
        <%while ((int)this.ClassList.First().ClassDate.DayOfWeek > 0)
            {
                this.ClassList.Insert(0, new SahoAcs.DBModel.B03GroupClassSchedule()
                {
                    RecordID =0,
                    ClassDate =this.ClassList.First().ClassDate.AddDays(-1)
                });
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
                        <%for (int i = 0; i < 6; i++)
                            { %>
                        <tr>
                            <%for (int j = 0; j < 7; j++)
                                { %>                     
                            <%ClassCount = i * 7 + j; %>
                                <td style="font-weight: bold; background-color:blue;color:white; width: 108px">
                                <%if (ClassCount < this.ClassList.Count && this.ClassList[ClassCount].RecordID!=0)
                                    { %>
                                <%=this.ClassList[ClassCount].ClassDate.ToString("yyyy/MM/dd") %><br />
                                預排班別：<br />
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
                                    <option value="<%=o %>">
                                        <%=o.Equals("0") ? "休" : o %>
                                    </option>
                                    <%} %>
                                    <%} %>
                                </select>
                                <%} %>
                            </td>
                            <%} %>
                        </tr>
                        <%} %>
                    </table>                    
                </td>
            </tr>
        </table> 
        <%--<table>
            <tr>
                <td>
                    <input type="button" id="BtnModifySchedule" name="BtnModifySchedule" value="修改排班" class="IconSave"/>
                </td>
            </tr>
        </table>--%>
        <%} %>
    </div>
    <div id="dvContent" style="position: absolute; left: 20px; top: 30px; z-index: 1000000000"></div>
    <div id="dvContent1" style="position: absolute; left: 20px; top: 30px; z-index: 1000000000"></div>
    <input type="hidden" name="PageEvent" value="Save" />
</asp:Content>
