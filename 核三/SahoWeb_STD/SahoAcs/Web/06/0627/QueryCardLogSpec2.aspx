<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI"
    CodeBehind="QueryCardLogSpec2.aspx.cs" Inherits="SahoAcs.Web.QueryCardLogSpec2" %>

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
                              AmountB=groups.Count(i=>i.LogStatus=="49"),
                              AmountL=groups.Count(i=>i.LogStatus=="51"),
                              AmountD=groups.Count(i=>i.LogStatus=="53"),
                              Amount1=groups.Count(i=>i.LogStatus=="49"&&i.EquName.Contains("自助餐區")),
                              Amount2=groups.Count(i=>i.LogStatus=="51"&&i.EquName.Contains("自助餐區")),
                              Amount3=groups.Count(i=>i.LogStatus=="53"&&i.EquName.Contains("自助餐區")),
                              Amount4=groups.Count(i=>i.LogStatus=="49"&&i.EquName.Contains("快餐區")),
                              Amount5=groups.Count(i=>i.LogStatus=="51"&&i.EquName.Contains("快餐區")),
                              Amount6=groups.Count(i=>i.LogStatus=="53"&&i.EquName.Contains("快餐區")),
                              Amount7=groups.Count(i=>i.LogStatus=="49"&&i.EquName.Contains("燴飯區")),
                              Amount8=groups.Count(i=>i.LogStatus=="51"&&i.EquName.Contains("燴飯區")),
                              Amount9=groups.Count(i=>i.LogStatus=="53"&&i.EquName.Contains("燴飯區")),
                              Amount10=groups.Count(i=>i.LogStatus=="49"&&i.EquName.Contains("麵食區")),
                              Amount11=groups.Count(i=>i.LogStatus=="51"&&i.EquName.Contains("麵食區")),
                              Amount12=groups.Count(i=>i.LogStatus=="53"&&i.EquName.Contains("麵食區")),
                              Amount13=groups.Count(i=>i.LogStatus=="49"&&i.EquName.Contains("觸控")),
                              Amount14=groups.Count(i=>i.LogStatus=="51"&&i.EquName.Contains("觸控")),
                              Amount15=groups.Count(i=>i.LogStatus=="53"&&i.EquName.Contains("觸控")),
                              Amount16=groups.Count(i=>i.LogStatus=="49"&&i.EquName.Contains("輕食區")),
                              Amount17=groups.Count(i=>i.LogStatus=="51"&&i.EquName.Contains("輕食區")),
                              Amount18=groups.Count(i=>i.LogStatus=="53"&&i.EquName.Contains("輕食區")),
                              AmountAll=groups.Count()
                              //ITEM_NAME = groups.Key.CsdItemName,
                              //Price = groups.Average(s => s.CsdSalePrice),
                              //Amount = groups.Average(s => s.CsdAmount)
                          };
         %>
    <table class="TableS1" style="width: 1250px" >
        <tbody>
            <tr class="GVStyle">
                <th scope="col" style="width: 115px;">廠商</th>
                <th scope="col" style="width: 85px;">日期</th>
                <th scope="col" style="width: 50px">自助餐區<br />
                    早餐</th>
                <th scope="col" style="width: 50px">自助餐區<br />
                    午餐</th>
                <th scope="col" style="width: 50px">自助餐區<br />
                    晚餐</th>
                <th scope="col" style="width: 50px">快餐區<br />
                    早餐</th>
                <th scope="col" style="width: 50px">快餐區<br />
                    午餐</th>
                <th scope="col" style="width: 50px">快餐區<br />
                    晚餐</th>
                <th scope="col" style="width: 50px">燴飯區<br />
                    早餐</th>
                <th scope="col" style="width: 50px">燴飯區<br />
                    午餐</th>
                <th scope="col" style="width: 50px">燴飯區<br />
                    晚餐</th>
                <th scope="col" style="width: 50px">麵食區<br />
                    早餐</th>
                <th scope="col" style="width: 50px">麵食區<br />
                    午餐</th>
                <th scope="col" style="width: 50px">麵食區<br />
                    晚餐</th>
                <th scope="col" style="width: 50px">輕食區<br />
                    早餐</th>
                <th scope="col" style="width: 50px">輕食區<br />
                    午餐</th>
                <th scope="col" style="width: 50px">輕食區<br />
                    晚餐</th>
                <th scope="col" style="width: 50px">觸控輸入<br />
                    早餐</th>
                <th scope="col" style="width: 50px">觸控輸入<br />
                    午餐</th>
                <th scope="col" style="width: 50px">觸控輸入<br />
                    晚餐</th>
                <th scope="col" style="width: 50px">早餐</th>
                <th scope="col" style="width: 50px">午餐</th>
                <th scope="col" style="width: 50px">晚餐</th>
                <th scope="col">總計</th>
            </tr>
            <%foreach(var o in groupresult)
                { %>
            <tr id="GV_Row1" class="GVStyle">
                <td style="width: 119px"><%=o.MgaName %></td>
                <td style="width: 89px"><%=string.Format("{0:yyyy/MM/dd}",o.CardDay) %></td>
                <td style="width: 54px"><%=o.Amount1 %></td>
                <td style="width: 54px"><%=o.Amount2 %></td>
                <td style="width: 54px"><%=o.Amount3 %></td>
                <td style="width: 54px"><%=o.Amount4 %></td>
                <td style="width: 54px"><%=o.Amount5 %></td>
                <td style="width: 54px"><%=o.Amount6 %></td>
                <td style="width: 54px"><%=o.Amount7 %></td>
                <td style="width: 54px"><%=o.Amount8 %></td>
                <td style="width: 54px"><%=o.Amount9 %></td>
                <td style="width: 54px"><%=o.Amount10 %></td>
                <td style="width: 54px"><%=o.Amount11 %></td>
                <td style="width: 54px"><%=o.Amount12 %></td>
                <td style="width: 54px"><%=o.Amount16 %></td>
                <td style="width: 54px"><%=o.Amount17 %></td>
                <td style="width: 54px"><%=o.Amount18 %></td>
                <td style="width: 54px"><%=o.Amount13 %></td>
                <td style="width: 54px"><%=o.Amount14 %></td>
                <td style="width: 54px"><%=o.Amount15 %></td>
                <td style="width: 54px"><%=o.AmountB %></td>
                <td style="width: 54px"><%=o.AmountL %></td>
                <td style="width: 54px"><%=o.AmountD %></td>
                <td style="text-align: center"><%=o.AmountAll %></td>
            </tr>
            <%} %>
            <tr id="GV_RowAll" class="GVStyle">
                <td style="width: 119px">合計</td>
                <td style="width: 89px"></td>                
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("自助餐區")&&i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("自助餐區")&&i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("自助餐區")&&i.LogStatus=="53").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("快餐區")&&i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("快餐區")&&i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("快餐區")&&i.LogStatus=="53").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("燴飯區")&&i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("燴飯區")&&i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("燴飯區")&&i.LogStatus=="53").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("麵食區")&&i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("麵食區")&&i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("麵食區")&&i.LogStatus=="53").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("輕食區")&&i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("輕食區")&&i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("輕食區")&&i.LogStatus=="53").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("觸控")&&i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("觸控")&&i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.EquName.Contains("觸控")&&i.LogStatus=="53").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.LogStatus=="49").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.LogStatus=="51").Count() %></td>
                <td style="width: 54px"><%=this.logmaps.Where(i=>i.LogStatus=="53").Count() %></td>
                <td style="text-align: center"><%=this.logmaps.Count() %></td>
            </tr>
        </tbody>
    </table>
    </div>
    <div>
        <asp:Button ID="ExportButton" runat="server" Text="匯　出" OnClick="ExportButton_Click" CssClass="IconExport" />
    </div>
</asp:Content>
