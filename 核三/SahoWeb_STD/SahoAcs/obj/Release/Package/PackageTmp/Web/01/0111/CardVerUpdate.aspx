<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI"
    CodeBehind="CardVerUpdate.aspx.cs" Inherits="SahoAcs.Web.CardVerUpdate" %>

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
        <input type="hidden" id="EquDir" name="EquDir" value="進" />
        <input type="hidden" id="Range" name="Range" value="3" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th runat="server" id="ShowPsnInfo1">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardNo_PsnName" runat="server" Text="關鍵字(單位名稱、代碼)"></asp:Label>
                        </th>
                        <th>
                            <%--設定版次--%>
                        </th>                       
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td runat="server" id="ShowPsnInfo2">
                            <input type="text" name="PsnNo" id="PsnNo" value="" style="width: 200px" />
                        </td>
                        <td>
                           <%-- <input type="text" name="CardVer" id="CardVer" value="" style="width: 100px" maxlength="1" />--%>
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
    <table class="TableS1" style="width: 900px">
        <tbody>
            <tr class="GVStyle">
                <th scope="col" style="width: 55px;">選取</th>
                <th scope="col" style="width: 125px;">單位架構</th>
                <th scope="col" style="width: 190px;">架構名稱</th>
                <th scope="col" style="width: 90px;">單位代碼</th>
                <th scope="col" style="">單位名稱</th>
            </tr>
            <tr>
                <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                    <asp:HiddenField ID="HiddenField1" runat="server" EnableViewState="False" />
                    <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 950px; overflow-y: scroll;">
                        <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                            <tbody>
                                <%foreach (var o in this.logmaps)
                                    { %>
                                <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                    <td style="width: 59px; text-align:center">
                                        <input type="checkbox" name="ChkStrucID" id="ChkStrucID" value="<%=o.OrgStrucID %>"  style="width:25px;height:25px"/>
                                    </td>
                                    <td style="width: 129px"><%=o.OrgNoList %></td>
                                    <td style="width: 194px"><%=o.OrgNameList %></td>
                                    <td style="width: 94px"><%=o.OrgNo %></td>
                                    <td style=""><%=o.OrgName %>
                                        <input type="hidden" id="OrgStrucID" name="OrgStrucID" value="<%=o.OrgStrucID %>" />                                        
                                    </td>                             
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                    </div>
                </td>
            </tr>
           <tr class="GVStyle">
                <th scope="col" colspan="9" id="RowCount"><%=string.Format("合計 {0} 筆",this.logmaps.Count) %></th>
            </tr>
        </tbody>
    </table>
    <div>
        <table>
            <tr>
                <td>
                    <span style="font-weight:bold;color:#fff; font-size:15px ">設定版次： </span>
                </td>
                <td>
                    <input type="text" name="CardVer" id="CardVer" value="" style="width: 100px" maxlength="1" /> 
                </td>
                <td>
                    <span style="font-weight:bold;color:#fff; font-size:15px ">異動時間： </span>
                </td>
                <td>
                    <uc1:Calendar runat="server" ID="ProcessTime" />
                </td>
                <td>
                    <input type="button" id="BtnUpdate" name="BtnUpdate" value="變更版次" class="IconSave" />
                </td>
            </tr>
        </table>                       
    </div>
</asp:Content>
