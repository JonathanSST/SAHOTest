<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI"
    CodeBehind="OrgTRTMapping.aspx.cs" Inherits="SahoAcs.Web.OrgTRTMapping" %>

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
        <input type="hidden" name="OrgIDList" value="" id="OrgIDList" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th runat="server" id="ShowPsnInfo1">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardNo_PsnName" runat="server" Text="關鍵字(編號、名稱)"></asp:Label>
                        </th>
                        <th>
                            <%--設定版次--%>
                        </th>                       
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td runat="server" id="ShowPsnInfo2">
                            <input type="text" name="OrgName" id="OrgName" value="" style="width: 200px" />
                        </td>
                        <td>
                           <%-- <input type="text" name="CardVer" id="CardVer" value="" style="width: 100px" maxlength="1" />--%>
                        </td>                        
                        <td>
                            <input type="button" value="<%=Resources.Resource.btnQuery %>" class="IconSearch" id="BtnQuery"  />
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
                <th scope="col" style="width: 125px;">編號</th>                
                <th scope="col" style="width: 150px;">組織編號列表</th>
                <th scope="col" style="width: 250px;">組織名稱列表</th>                
                <th scope="col" style="width:120px">創建人</th>
                <th scope="col" style="">設定</th>
            </tr>
            <tr>
                <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                    <asp:HiddenField ID="HiddenField1" runat="server" EnableViewState="False" />
                    <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 950px; overflow-y: scroll;">
                        <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                            <tbody>
                                <%foreach (var o in this.OrgDataList)
                                    { %>
                                <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                    <td style="width: 129px"><%=o.OrgStrucNo %></td>                                    
                                    <td style="width: 154px"><%=o.OrgNoList %></td>
                                    <td style="width: 254px"><%=o.OrgNameList %>
                                        <input type="hidden" id="OrgStrucID" name="OrgStrucID" value="<%=o.OrgStrucID %>" />
                                        <%--<input type="hidden" id="OrgIDList" name="OrgIDList" value="<%=o.OrgIDList %>" />--%>
                                    </td>
                                    <td style="width:124px">
                                        <%=o.OrgName %>
                                    </td>
                                    <td style="">
                                        <input type="button" id="BtnUpdate" name="BtnUpdate" value="設定單位考勤表" class="IconSet" onclick="SetQuery('<%=o.OrgIDList.Replace(@"\","@") %>')" />
                                    </td>
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                    </div>
                </td>
            </tr>
           <tr class="GVStyle">
                <th scope="col" colspan="9" id="RowCount"><%=string.Format("合計 {0} 筆",this.OrgDataList.Count) %></th>
            </tr>
        </tbody>
    </table>
    <div>        
        <%--<input type="button" id="BtnUpdate" name="BtnUpdate" value="設定單位考勤表" class="IconSave" />--%>
    </div>
     <div id="EquMapping" style="display: none; position:absolute; z-index:30000;background-color:#1275BC;border-style:solid; border-width:2px; border-color:#069">
         <fieldset id="Fieldset3" runat="server" style="width: 780px; height: 430px">
             <legend id="Legend3" runat="server">
                 <asp:Label ID="Label4" runat="server">考勤設備對照表</asp:Label>
             </legend>
             <table class="TableS1">
                 <tbody>
                     <tr class="GVStyle">
                         <th scope="col" style="width: 70px;">選取</th>
                         <th scope="col" style="width: 240px">設備編號</th>
                         <th scope="col">設備名稱</th>
                     </tr>
                     <tr>
                         <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="4">
                             <div id="ContentPlaceHolder1_tablePanel2" style="height: 385px; width: 780px; overflow-y: scroll;">
                                 <div>
                                     <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                         id="ContentPlaceHolder1_GridViewFloor" style="border-collapse: collapse;">
                                         <tbody>
                                             <%foreach (var a in this.TrtEquList)
                                                 { %>
                                             <tr>
                                                 <td style="width: 74px; text-align: center">
                                                     <%if (a.CtrlID == 0)
                                                         { %>
                                                     <input type="checkbox" name="EquID" id="EquID" value="<%=a.EquID %>"  style="width:25px;height:25px"/>
                                                     <%}else{ %>
                                                     <input type="checkbox" name="EquID" id="EquID" value="<%=a.EquID %>"  style="width:25px;height:25px"  checked="checked"/>
                                                     <%} %>
                                                 </td>
                                                 <td style="width: 244px">
                                                     <%=a.EquNo %>
                                                 </td>
                                                 <td>
                                                     <%=a.EquName %>
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
             <div>
                 <input type="button" id="btnUpdate" value="<%=this.GetGlobalResourceObject("Resource","btnOK") %>" class="IconSave"  onclick="SetSaveData()"/>
                 <input type="button" id="btnCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel" onclick="CloseWindows()" />
             </div>
         </fieldset>
    </div>
</asp:Content>
