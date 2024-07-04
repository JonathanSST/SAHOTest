<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI"
    CodeBehind="QueryCardLogSpec4.aspx.cs" Inherits="SahoAcs.Web.QueryCardLogSpec4" %>

<%@ Import Namespace="SahoAcs.DBModel" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="~/uc/MultiSelectDropDown.ascx" TagPrefix="uc2" TagName="MultiSelectDropDown" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="<%$Resources:ttCardTime %>"></asp:Label>
                        </th>
                        <th runat="server" id="ShowPsnInfo1">
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblManageArea" runat="server" Text="廠商"></asp:Label>
                        </th>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeSDate" />
                        </td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeEDate" />
                        </td>
                        <td runat="server" id="ShowPsnInfo2">
                            <asp:DropDownList ID="DropMgaList" runat="server" Width="200px" CssClass="DropDownListStyle">
                             </asp:DropDownList>
                        </td>
                        <td>
                            <input type="button" value="<%=Resources.Resource.btnQuery %>" class="IconSearch" id="BtnQuery" />
                        </td>
                        <td runat="server" id="ShowPsnInfo3"></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="ResultContent">    
    <%
        var groupresult = from g in this.logmaps
                          group g by new {g.CardTime}
                                      into groups
                          select new
                          {
                              CardDay=groups.Key.CardTime,
                              MgaName=groups.First().MgaName,
                              AmountAll=groups.Count(i=>!i.EquName.Contains("滿意度")),
                              AmountSel=groups.Count(i=>i.EquName.Contains("滿意度")),
                              AmountBY=groups.Count(i=>i.LogStatus=="49"&&i.EquName.Contains("滿意度調查(右)")),
                              AmountBN=groups.Count(i=>i.LogStatus=="49"&&i.EquName.Contains("滿意度調查(左)")),
                              AmountLY=groups.Count(i=>i.LogStatus=="51"&&i.EquName.Contains("滿意度調查(右)")),
                              AmountLN=groups.Count(i=>i.LogStatus=="51"&&i.EquName.Contains("滿意度調查(左)")),
                              AmountDY=groups.Count(i=>i.LogStatus=="53"&&i.EquName.Contains("滿意度調查(右)")),
                              AmountDN=groups.Count(i=>i.LogStatus=="53"&&i.EquName.Contains("滿意度調查(左)")),
                              AmountY=groups.Count(i=>i.EquName.Contains("滿意度調查(右)")),
                              AmountN=groups.Count(i=>i.EquName.Contains("滿意度調查(左)"))                              
                          };
         %>
    <table class="TableS1" style="width: 1150px" >
        <tbody>
            <tr class="GVStyle">
                <th scope="col" style="width: 115px;">廠商</th>
                <th scope="col" style="width: 85px;">日期</th>
                <th scope="col" style="width: 50px">早餐<br />
                    滿意</th>
                <th scope="col" style="width: 50px">早餐<br />
                    不滿意</th>
                <th scope="col" style="width: 50px">午餐<br />
                    滿意</th>
                <th scope="col" style="width: 50px">午餐<br />
                    不滿意</th>
                <th scope="col" style="width: 50px">晚餐<br />
                    滿意</th>
                <th scope="col" style="width: 50px">晚餐<br />
                    不滿意</th>
                <th scope="col" style="width: 50px">當日<br />
                    滿意</th>
                <th scope="col" style="width: 50px">當日<br />
                    不滿意</th>
                <th scope="col" style="width: 50px">當日<br />
                    滿意度(%)</th>
                <th scope="col" style="width: 50px">當日<br />
                    投票人數</th>
                <th scope="col" style="width: 50px">當日<br />
                    用餐人數</th>
                <th scope="col">當日<br />
                    投票率(%)</th>               
            </tr>
            <%foreach(var o in groupresult)
                { %>
            <tr id="GV_Row1" class="GVStyle">
                <td style="width: 119px"><%=o.MgaName %></td>
                <td style="width: 89px"><%=string.Format("{0:yyyy/MM/dd}",o.CardDay) %></td>
                <td style="width: 54px"><%=o.AmountBY %></td>
                <td style="width: 54px"><%=o.AmountBN %></td>
                <td style="width: 54px"><%=o.AmountLY %></td>
                <td style="width: 54px"><%=o.AmountLN %></td>
                <td style="width: 54px"><%=o.AmountDY %></td>
                <td style="width: 54px"><%=o.AmountDN %></td>
                <td style="width: 54px"><%=o.AmountY %></td>
                <td style="width: 54px"><%=o.AmountN %></td>
                <%
                    var baseInt = 1;
                    if (o.AmountSel > 0)
                    {
                        baseInt=o.AmountSel;
                    } %>
                <td style="width: 54px"><%=string.Format("{0:0}",(o.AmountY/baseInt) * 100) %></td>
                <td style="width: 54px"><%=o.AmountSel %></td>
                <td style="width: 54px"><%=o.AmountAll %></td>
                <%                    
                    if (o.AmountAll > 0)
                    {
                        baseInt=o.AmountAll;
                    } %>
                <td><%=string.Format("{0:0.00}",(o.AmountSel/baseInt) * 100)%></td>               
            </tr>
            <%} %>
            <tr id="GV_RowAll" class="GVStyle">
                <td style="width: 119px">合計</td>
                <td style="width: 89px"></td>                
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查(右)")&&i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查(左)")&&i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查(右)")&&i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查(左)")&&i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查(右)")&&i.LogStatus=="53").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查(左)")&&i.LogStatus=="53").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查(右)")).Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查(左)")).Count() %></td>
                <td style="width: 54px"></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("滿意度調查")).Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>!i.EquName.Contains("滿意度調查")).Count() %></td>                
                <td style="text-align: center"></td>
            </tr>
        </tbody>
    </table>
    </div>
    <div>
        <asp:Button ID="ExportButton" runat="server" Text="匯　出" OnClick="ExportButton_Click" CssClass="IconExport" />
    </div>
</asp:Content>
