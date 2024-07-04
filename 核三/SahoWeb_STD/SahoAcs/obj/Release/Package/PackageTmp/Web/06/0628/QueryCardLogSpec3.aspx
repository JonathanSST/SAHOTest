<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI"
    CodeBehind="QueryCardLogSpec3.aspx.cs" Inherits="SahoAcs.Web.QueryCardLogSpec3" %>

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
                            <asp:Label ID="QueryLabel_CardNo_PsnName" runat="server" Text="學號、姓名或卡號"></asp:Label>
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
                            <input type="text" name="CardNo_PsnName" id="CardNo_PsnName" value="" style="width: 200px" />
                        </td>
                        <td>
                            <input type="button" value="<%=Resources.Resource.btnQuery %>" class="IconSearch" id="BtnQuery"/>
                        </td>
                        <td runat="server" id="ShowPsnInfo3"></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <%
         var groupresult = from g in this.logmaps
                          group g by new {g.CardTime,g.StateDesc,g.CardNo}
                                      into groups
                          select new
                          {
                              CardNo=groups.Key.CardNo,
                              PsnName=groups.First().PsnName,
                              StateDesc=groups.Key.StateDesc,
                              CardTime=groups.Key.CardTime,
                              OrgName=groups.First().OrgName,
                              PsnNo=groups.First().PsnNo,
                              Counts=groups.Count()
                              //ITEM_NAME = groups.Key.CsdItemName,
                              //Price = groups.Average(s => s.CsdSalePrice),
                              //Amount = groups.Average(s => s.CsdAmount)
                          };
         %>
    <table class="TableS1" style="width: 1050px">
        <tbody>
            <tr class="GVStyle">
               <th scope="col" style="width: 115px;">日期</th>
                <th scope="col" style="width: 65px;">餐別</th>
                <th scope="col" style="width: 65px;">重複次數</th>
                <th scope="col" style="width: 90px;">班級</th>
                <th scope="col" style="width: 80px;">學號</th>
                <th scope="col" style="width: 90px;">姓名</th>
                <th scope="col">卡號</th>                
            </tr>
            <tr>
                <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                    <asp:HiddenField ID="HiddenField1" runat="server" EnableViewState="False" />
                    <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1050px; overflow-y: scroll;">
                        <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                            <tbody>
                                <%foreach (var o in groupresult.Where(i=>i.Counts>1))
                                    { %>
                                <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                    <td style="width: 119px"><%=string.Format("{0:yyyy/MM/dd}",o.CardTime) %></td>
                                    <td style="width: 69px"><%=o.StateDesc %></td>
                                    <td style="width: 69px"><%=o.Counts-1 %></td>
                                    <td style="width: 94px"><%=o.OrgName %></td>
                                    <td style="width: 84px"><%=o.PsnNo %></td>
                                    <td style="width: 94px"><%=o.PsnName %></td>
                                    <td><%=o.CardNo %></td>                                    
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
     <div>        
        <asp:Button ID="ExportButton" runat="server" Text="匯　出" OnClick="ExportButton_Click" CssClass="IconExport" />
    </div>
</asp:Content>
