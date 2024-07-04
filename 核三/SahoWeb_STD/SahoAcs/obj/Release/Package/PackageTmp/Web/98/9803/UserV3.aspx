<%@ Page Language="C#" CodeBehind="UserV3.aspx.cs" Inherits="SahoAcs.UserV3" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>


<asp:Content ID="ContentHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../../../scripts/Check/JS_AJAX.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UTIL.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_BUTTON_PASS.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_CHECK.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_TOOLTIP.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LEVEL.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.DATE.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.ETEK.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.HT.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.REF.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TABLE.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TABS.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TOOLTIP.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI_FILE_UPLOAD.js" type="text/javascript"></script>
    <script src="../../../scripts/JsTabEnter.js" type="text/javascript"></script>
    <script src="../../../scripts/JsQueryWindow.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%-- cellspacing="0" border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死 --%>
    
    <%-- 主作業畫面一：查詢部份 --%>
    <table class="Item">
        <tr>
            <th><span class="Arrow01"></span><asp:Label ID="Label_ID" runat="server" Text="<%$Resources:ttUserId %>"></asp:Label></th>
            <th><span class="Arrow01"></span><asp:Label ID="Label_Name" runat="server" Text="<%$Resources:ttUserName %>"></asp:Label></th>
            <th><span class="Arrow01"></span><asp:Label ID="Label_IsOptAllow" runat="server" Text="<%$Resources:ttOptAllow %>"></asp:Label></th>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td><input type="text"  id="Input_ID" name="Input_ID"/></td>
            <td><input type="text" id="Input_Name" name="Input_Name" /></td>
            <td>
                <asp:DropDownList ID="Input_IsOptAllow" runat="server" Width="100px">
                    <asp:ListItem Value="1" Enabled="true" Text="<%$Resources:ttStateOpen %>"></asp:ListItem>
                    <asp:ListItem Value="0" Text="<%$Resources:ttStateClose %>"></asp:ListItem>
                </asp:DropDownList></td>
            <td><asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" /></td>
        </tr>
    </table>

    <%-- 主作業畫面二：表格部份 --%>
    <div id="MainDataArea" style="width:100%;overflow-x:scroll">
        <table class="TableS1" style="width: 1450px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" style="width: 80px;"><a href="#" onclick="SetSort('UserID')"><%=GetLocalResourceObject("ttUserID") %></a></th>
                    <th scope="col" style="width: 80px;"><a href="#" onclick="SetSort('UserName')"><%=GetLocalResourceObject("ttUserName") %></a></th>
                    <th scope="col" style="width: 120px;"><a href="#" onclick="SetSort('UserEName')"><%=GetLocalResourceObject("ttUserEName") %></a></th>
                    <th scope="col" style="width: 160px;"><a href="#" onclick="SetSort('Email')"><%=GetLocalResourceObject("ttUserEmail") %></a></th>
                    <th scope="col" style="width: 90px;"><a href="#" onclick="SetSort('IsOptAllow')"><%=GetLocalResourceObject("ttOptAllow") %></a></th>
                    <th scope="col" style="width: 90px;"><a href="#" onclick="SetSort('UserSTime')"><%=GetLocalResourceObject("ttUserSTime") %></a></th>
                    <th scope="col" style="width: 90px;"><a href="#" onclick="SetSort('UserETime')"><%=GetLocalResourceObject("ttUserETime") %></a></th>
                    <th scope="col" style="width: 100px;"><a href="#" onclick="SetSort('PWChgTime')"><%=GetLocalResourceObject("ttPWChgTime") %></a></th>
                    <th scope="col" style="width: 140px;"><a href="#" onclick="SetSort('UserPWCtrl')"><%=GetLocalResourceObject("ttUserPWCtrl") %></a></th>
                    <th scope="col" style=""><a href="#" onclick="SetSort('Remark')"><%=GetLocalResourceObject("Remark") %></a></th>
                </tr>
                <%if (this.UserList != null)
                    { %>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" style="padding: 0" colspan="11">
                        <div id="tablePanel" class="MinHeight" style="overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="MainGridView" style="border-collapse: collapse;">
                                    <tbody>
                                        <%foreach (var o in this.UserList)
                                            { %>
                                        <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" onclick="SingleRowSelect(0, this, $('#SelectValue')[0],'<%=o.UserID %>', '', '');"
                                            ondblclick="CallEdit('使用者維護', '請注意!!  目前無法進行編輯\n\n可能的原因：1.沒有資料可供編輯。\n　　　　　　2.尚未選擇要編輯的項目。\n')">
                                            <td title="<%=o.UserID %>" style="width: 83px;"><%=o.UserID %></td>
                                            <td title="<%=o.UserName %>" style="width: 84px;"><%=o.UserName %></td>
                                            <td title="<%=o.UserEName %>" style="width: 124px;"><%=o.UserEName %></td>
                                            <td title="<%=o.EMail %>" style="width: 164px;"><%=o.EMail %></td>
                                            <td title="<%=o.IsOptAllow %>" style="width: 94px;"><%=o.IsOptAllow %></td>
                                            <td title="<%=o.UserSTime %>" style="width: 94px;"><%=o.UserSTime.ToString("yyyy/MM/dd") %></td>
                                            <td title="<%=o.UserETime.Value %>" style="width: 94px;"><%=o.UserETime.Value.ToString("yyyy/MM/dd") %></td>
                                            <td title="<%=o.PWChgTime.Value %>" style="width: 104px;"><%=o.PWChgTime.Value.ToString("yyyy/MM/dd") %></td>
                                            <td title="<%=o.UserPWCtrl %>" style="width: 144px;"><%=o.UserPWCtrl %></td>
                                            <td title="<%=o.Remark %>" style="text-align: center"><%=o.Remark %></td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <%}
                    else
                    { %>
                <tr>
                    <td id="NullRow" style="padding: 0" colspan="5">
                        <div id="NullPanel" class="MinHeight" style="overflow-y: scroll;">
                            <%=GetGlobalResourceObject("Resource","NonData") %>
                        </div>
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
        <input type="hidden" id="MaxUser" name="MaxUser" value="<%=SahoAcs.DBClass.DongleVaries.GetMaxUser() %>" />
        <input type="hidden" id="CurrentUser" name="CurrentUser" value="<%=SahoAcs.DBClass.DongleVaries.GetCurrentUser() %>" />
       
    </div>


    <asp:HiddenField ID="hUserId" runat="server" />
    <asp:HiddenField ID="hOwnerList" runat="server" EnableViewState="False"  />
    <input type="hidden" id="SelectValue" name="SelectValue" />
    <input type="hidden" id="msgUserAdd" value="<%=this.GetLocalResourceObject("ttUserAdd") %>" />
    <input type="hidden" id="msgUserEdit" value="<%=this.GetLocalResourceObject("ttUserEdit") %>" />
    <input type="hidden" id="msgUserDel" value="<%=this.GetLocalResourceObject("ttUserDel") %>" />
    <input type="hidden" id="msgDelete" value="<%=this.GetLocalResourceObject("msgDelete") %>" />
    <input type="hidden" id="msgUserAuth" value="<%=this.GetLocalResourceObject("ttUserAuth") %>" />
    <input type="hidden" id="msgUserMgn" value="<%=this.GetLocalResourceObject("lblUserMgn") %>" />
    <input type="hidden" id="msgUserRole" value="<%=this.GetLocalResourceObject("lblUserRole") %>" />
    <input type="hidden" id="msgNonSelect" value="<%=this.GetGlobalResourceObject("Resource","NotSelectForEdit").ToString().Replace("\\n","|") %>" />
    <input type="hidden" id="msgNonDelete" value="<%=this.GetGlobalResourceObject("Resource","NotSelectForDelete").ToString().Replace("\\n","|") %>" />
    <asp:HiddenField ID="SelectNowName" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
     <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
    <input type="hidden" id="SysUserID" name="SysUserID" value="<%=Sa.Web.Fun.GetSessionStr(this,"UserID")%>" />
    <%-- 主作業畫面三：按鈕部份 --%>
    <table>
        <tr>
            <td><asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew"/></td>
            <td><asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit"/></td>
            <td><asp:Button ID="DeleteButton" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete"/></td>
            <td><asp:Button ID="MenuButton" runat="server" Text="<%$Resources:btnAuth %>" CssClass="IconSet"/></td>
            <td><asp:Button ID="RoleButton" runat="server" Text="<%$Resources:btnUserRole %>" CssClass="IconSetRoles"/></td>
            <td><asp:Button ID="MgnaButton" runat="server" Text="<%$Resources:btnMgnArea %>" CssClass="IconSetManage"/></td>
        </tr>
    </table>

    <div id="popOverlay1" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv1" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="使用者資料" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                       <%-- <asp:ImageButton ID="ImgCloseButton1" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />     --%>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ID" runat="server" Text="<%$Resources:ttUserId %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_PW" runat="server" Text="<%$Resources:ttUserPWD %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>                                
                                <input type="text" class="TextBoxRequired" id="UserID" name="UserID" must_keyin_yn="Y" field_name="<%=GetLocalResourceObject("ttUserId") %>" style="width:180px" />
                            </td>
                            <td>                                
                                <input type="text" class="TextBoxRequired" id="UserPW" name="UserPW" must_keyin_yn="Y" field_name="<%=GetLocalResourceObject("ttUserPW") %>" style="width:180px"   />
                                <input type="hidden" id="msgRegRule" value="密碼原則：英文大寫、英文小寫、特殊符號、數字，四條件取三種，長度至少12碼" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="<%$Resources:ttUserName %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_IsOptAllow" runat="server" Text="<%$Resources:ttOptAllow %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="text" class="TextBoxRequired" id="UserName" name="UserName" must_keyin_yn="Y" field_name="<%=GetLocalResourceObject("ttUserName") %>" style="width:180px" />
                            </td>
                            <td>                                
                                <select id="IsOptAllow" name="IsOptAllow" class="DropDownListStyle" style="width:184px">
                                    <option value="1"><%=this.GetLocalResourceObject("ttStateOpen") %></option>
                                    <option value="0"><%=this.GetLocalResourceObject("ttStateClose") %></option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EName" runat="server" Text="<%$Resources:ttUserEName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">                                
                                <input type="text" id="UserEName" name="UserEName" must_keyin_yn="N" field_name="<%=GetLocalResourceObject("ttUserId") %>" style="width:360px" />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EMail" runat="server" Text="<%$Resources:ttUserEmail %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">                                
                                <input type="text" id="Email" name="Email" must_keyin_yn="N" field_name="<%=GetLocalResourceObject("ttUserId") %>" style="width:360px" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_STime" runat="server" Text="<%$Resources:ttUserSTime %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ETime" runat="server" Text="<%$Resources:ttUserETime %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <uc1:Calendar runat="server" ID="UserSTime" />
                            </td>
                            <td>
                                <uc1:Calendar runat="server" ID="UserETime" />
                            </td>
                        </tr>
                       <%--原密碼啟用時間與密碼控制欄位--%>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Remark" runat="server" Text="<%$Resources:ttRemark %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <input name="Remark" type="text" id="Remark" style="width: 370px;" />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2" style="text-align:center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                       <input type="button" name="popB_Add" value="<%=GetGlobalResourceObject("Resource","btnSave")  %>" onclick="AddExcute(); return false;" id="popB_Add" class="IconSave" style="display: inline;" />
                            <input type="button" name="popB_Edit" value="<%=GetGlobalResourceObject("Resource","btnSave")  %>" onclick="EditExcute(); return false;" id="popB_Edit" class="IconSave" style="display: none;" />
                            <input type="button" name="popB_Reset" value="密碼重置" onclick="ResetExcute(); return false;" id="popB_Reset" class="IconRefresh" style="display: none;" />
                            <input type="button" name="popB_Delete" value="<%=GetGlobalResourceObject("Resource","btnDelete") %>" onclick="DeleteExcute(); return false;" id="popB_Delete" class="IconDelete" style="display: none;" />
                            <input type="button" name="popB_Cancel" value="<%=GetGlobalResourceObject("Resource","btnCancel")  %>" onclick="DoCancel('1')" id="popB_Cancel" class="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <div id="popOverlay2" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <%-- 次作業畫面三：使用者角色清單權限設定 --%>
    <div id="ParaExtDiv2" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
    <asp:Panel ID="PanelDrag2" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName3" runat="server" Text="使用者角色清單權限設定" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                       <%-- <asp:ImageButton ID="ImgCloseButton3" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />--%>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tbody>
            <tr>
                <td>
                    <table id="ContentPlaceHolder1_popUserRolesTableHeader" class="Title01" cellspacing="0" border="1" style="border-collapse: collapse;">
                        <tbody>
                            <tr>
                                <td rowspan="2" style="text-align:center;vertical-align:middle; width: 160px; white-space: nowrap; background-color: #016B0A; color: #FBFBFB; border-color: #FBFBFB;"><%=this.GetLocalResourceObject("popRoleName") %></td>
                                <td colspan="1" style="text-align:center; background-color: #24AC2F; color: #FBFBFB; border-color: #FBFBFB;"><%=this.GetLocalResourceObject("popAuth") %></td>
                            </tr>
                            <tr>
                                <td style="text-align:center; width: 90px; background-color: #24AC2F; color: #FBFBFB; border-color: #FBFBFB;"><%=this.GetLocalResourceObject("popSelect") %></td>
                            </tr>
                        </tbody>
                    </table>
                    <div id="ContentPlaceHolder1_popUserRolesAuthPanel" style="height: 300px; overflow-y: scroll;">
                        <table id="ContentPlaceHolder1_popUserRolesAuthTable" class="Panel01" cellspacing="0" border="1" style="border-collapse: collapse; word-break: break-all;">
                            <tbody>
                                <%foreach (var role in this.SysUserRole)
                                    { %>
                                <tr style="height: 30px;">
                                    <td style="width: 150px;"><%=string.Format("{0}({1})",role.RoleNo,role.RoleName) %></td>
                                    <td style="text-align: center">
                                        <input id="UserRolesAuth" type="checkbox" name="UserRolesAuth" tabindex="0" value="<%=role.RoleID %>" style="width:25px;height:25px" /></td>
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                    </div>
                    <table>
                        <tr>
                            <td style="padding-top:10px">                                
                                <input type="button" name="UserRolesAuthAdd" value="<%=this.GetGlobalResourceObject("Resource","btnSave") %>" onclick="UserRolesAuthSaveExcute()" id="UserRolesAuthAdd" class="IconSave"/>
                                <input type="button" name="UserRolesAuthCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" onclick="DoCancel('2')" id="UserRolesAuthCancel" class="IconCancel"/>                                
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            </tbody>
        </table>
    </div>
    <%--使用者目錄維護--%>    
    <div id="popOverlay3" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv3" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <div id="ContentPlaceHolder1_PanelDrag3" style="background-color: #2D89EF; height: 28px;">
            <table class="TableWidth">
                <tbody>
                    <tr>
                        <td>
                            <span id="ContentPlaceHolder1_L_popName2" style="color: White; font-weight: bold; vertical-align: middle">系統功能設定</span>
                        </td>
                        <td style="text-align: right;"></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <table class="popItem">
            <tbody>
                <tr>
                    <td>
                        <table id="ContentPlaceHolder1_popUserMenusTableHeader" class="Title01" cellspacing="0" border="1" style="border-collapse: collapse;">
                            <tbody>
                                <tr>
                                    <td align="center" valign="middle" rowspan="2" style="width: 160px; white-space: nowrap; background-color: #016B0A; color: #FBFBFB; border-color: #FBFBFB;"><%=GetLocalResourceObject("ttFuncName") %></td>
                                    <td align="center" valign="middle" rowspan="2" style="width: 90px; white-space: nowrap; background-color: #016B0A; color: #FBFBFB; border-color: #FBFBFB;"><%=GetLocalResourceObject("popAdj") %></td>
                                    <td align="center" colspan="<%=this.MenuAuthList.Count %>" style="background-color: #24AC2F; color: #FBFBFB; border-color: #FBFBFB;"><%=GetLocalResourceObject("ttAuthItem") %></td>
                                </tr>
                                <tr>
                                    <%foreach (var menu in this.MenuAuthList)
                                        { 
                                            string authname = Request.Cookies["i18n"].Value == "zh-TW" ? menu.ItemName: menu.ItemNo;
                                            %>
                                            <%if (this.MenuAuthList.Last().ItemNo != menu.ItemNo)
                                                { %>
                                            <td style="text-align:center;width: 90px; background-color: #24AC2F; color: #FBFBFB; border-color: #FBFBFB;"><%=authname %></td>
                                    <%}else{%>
                                        <td style="text-align:center;background-color: #24AC2F; color: #FBFBFB; border-color: #FBFBFB;"><%=authname %></td>
                                    <%} %>
                                    <%} %>
                                </tr>                                
                            </tbody>
                        </table>
                        <div id="ContentPlaceHolder1_popUserMenusAuthPanel" style="height: 400px; overflow-y: scroll;">
                            <table id="ContentPlaceHolder1_popUserMenusAuthTable" class="Panel01" cellspacing="0" border="1" style="border-collapse: collapse; word-break: break-all;">
                                <tbody>
                                    <%foreach (var menu in this.SysMenuList)
                                        { %>
                                    <tr>
                                        <td style="width:150px">
                                            <%=menu.MenuName %>
                                        </td>
                                        <td style="width:80px">
                                            <select name="UserMenusOPMode_<%=menu.MenuNo %>" id="UserMenusOPMode_<%=menu.MenuNo %>" class="DropDownListStyle" onclick="DropDownListSelected('0101_UserMenusOPMode', '*');" tabindex="0">
                                                <option value="-"><%=GetLocalResourceObject("popAuthRed") %></option>
                                                <option value="+"><%=GetLocalResourceObject("popAuthAdd") %></option>
                                            </select>
                                        </td>
                                        <%foreach (var auth in this.MenuAuthList)
                                        { %>                                            
                                            <td onclick="" style="width:80px;text-align:center">
                                                <%if (menu.FunAuthDef.Split(',').ToList().Contains(auth.ItemNo))
                                                    { %>
                                                        <input type="checkbox" style="width:25px;height:25px" name="Chk_<%=string.Format("{0}_{1}",menu.MenuNo,auth.ItemNo) %>" id="Chk_<%=string.Format("{0}_{1}",menu.MenuNo,auth.ItemNo) %>" value="<%=auth.ItemNo %>" />
                                                <%} %>                                                
                                             </td>
                                    <%} %>
                                    </tr>
                                    <%} %>
                                </tbody>
                            </table>
                        </div>
                        <table>
                            <tbody>
                                <tr>
                                    <td style="padding-top: 10px">
                                        <input type="button" name="popB_UserMenusAuthAdd" value="儲     存" onclick="UserMenusAuthSaveExcute()" id="popB_UserMenusAuthAdd" class="IconSave" />
                                        <input type="button" name="popB_UserMenusAuthCancel" value="取     消" onclick="DoCancel('3')" id="popB_UserMenusAuthCancel" class="IconCancel" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <%--使用者管理區設定--%>
    <div id="popOverlay4" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv4" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
    <asp:Panel ID="Panel1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="使用者管理區清單權限設定" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                       <%-- <asp:ImageButton ID="ImgCloseButton3" runat="server" Height="25px" ImageUrl="/Img/close_button.png" EnableViewState="False" />--%>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tbody>
            <tr>
                <td>
                    <table id="ContentPlaceHolder1_popUserMgasTableHeader" class="Title01" cellspacing="0" border="1" style="border-collapse: collapse;">
                        <tbody>
                            <tr>
                                <td rowspan="2" style="text-align:center;vertical-align:middle; width: 160px; white-space: nowrap; background-color: #016B0A; color: #FBFBFB; border-color: #FBFBFB;"><%=this.GetLocalResourceObject("popMgnName") %></td>
                                <td colspan="1" style="text-align:center; background-color: #24AC2F; color: #FBFBFB; border-color: #FBFBFB;"><%=this.GetLocalResourceObject("popAuth") %></td>
                            </tr>
                            <tr>
                                <td style="text-align:center; width: 90px; background-color: #24AC2F; color: #FBFBFB; border-color: #FBFBFB;"><%=this.GetLocalResourceObject("popSelect") %></td>
                            </tr>
                        </tbody>
                    </table>
                    <div id="ContentPlaceHolder1_popUserMgasAuthPanel" style="height: 300px; overflow-y: scroll;">
                        <table id="ContentPlaceHolder1_popUserMgasAuthTable" class="Panel01" cellspacing="0" border="1" style="border-collapse: collapse; word-break: break-all;">
                            <tbody>
                                <%foreach (var mga in this.SysUserMga)
                                    { %>
                                <tr style="height: 30px;">
                                    <td style="width: 150px;"><%=string.Format("{0}({1})",mga.MgaNo,mga.MgaName) %></td>
                                    <td style="text-align: center">
                                        <input id="UserMgaAuth" type="checkbox" name="UserMgaAuth" tabindex="0" value="<%=mga.MgaID %>" style="width:25px;height:25px" /></td>
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                    </div>
                    <table>
                        <tr>
                            <td style="padding-top:10px">                                
                                <input type="button" name="UserMgasAdd" value="<%=this.GetGlobalResourceObject("Resource","btnSave") %>" onclick="UserMgnsAuthSaveExcute()" id="UserMgasAdd" class="IconSave"/>
                                <input type="button" name="UserMgasCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" onclick="DoCancel('4')" id="UserMgasCancel" class="IconCancel"/>                                
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            </tbody>
        </table>
    </div>

</asp:Content>