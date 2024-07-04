<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" EnableEventValidation="false" 
    Theme="UI" Debug="true" CodeBehind="SysMenuManage.aspx.cs" 
    Inherits="SahoAcs.Web._98._9810.SysMenuManage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />        
    </div>    
            <table class="TableS1" style="width: 1050px">
                <tbody>
                    <tr class="GVStyle">
                        <th scope="col" style="width: 150px;">目錄群組</th>
                        <th scope="col" style="width: 200px;">功能名稱</th>
                        <th scope="col" style="width: 250px;">目錄位置</th>
                        <th scope="col" style="width:100px">排序</th>
                        <th scope="col">開啟使用</th>                                                
                    </tr>
                    <tr>
                        <td id="ContentPlaceHolder1_td_showGridView" colspan="5">
                            <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
                            <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="height:500px;width: 1050px; overflow-y: scroll;">
                                <div>
                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                        <tbody>
                                            <%foreach(var m in this.SysMenus){ %>                                            
                                            <tr class="ChR" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                                <td style="width: 154px;"><%=m.UpMenuName %></td>
                                                <td style="width: 204px;"><input FIELD_NAME="功能名稱" MUST_KEYIN_YN="Y" style="width:95%" type="text" name="MenuName" value="<%=m.MenuName %>" /></td>
                                                <td style="width: 254px;"><input style="width:95%" type="text" name="FunTrack" value="<%=m.FunTrack %>" /></td>
                                                <td style="width: 104px; text-align:center">
                                                    <input  MUST_KEYIN_YN="Y" DATA_TYPE="N" FIELD_NAME="排序" style="width:95%" type="text" name="MenuOrder" value="<%=m.MenuOrder %>" />
                                                </td>
                                                <td>
                                                    <select name="MenuIsUse" class="DropDownListStyle">                                                        
                                                        <option value="0">未啟用</option>
                                                        <option value="1">啟用</option>
                                                    </select>
                                                    <input type="hidden" value="<%=m.MenuIsUse%>" name="hMenuUse" />
                                                    <input type="hidden" value="<%=m.MenuNo%>" name="MenuNo" />
                                                </td>
                                            </tr>
                                            <%                                                  
                                            } %>                                            
                                        </tbody>
                                    </table>                                    
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>        
    <input type="button" value="儲存" onclick="DoSave()" class="IconSave"/>
    <%--<input type="hidden" id="PageEvent" name="PageEvent" value="save" />--%>
    <div>               
    </div>
</asp:Content>
