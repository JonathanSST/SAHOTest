<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageArea.aspx.cs" Inherits="SahoAcs.ManageArea" Debug="true" EnableEventValidation="false" Theme="UI" %>


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
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <%=GetLocalResourceObject("ttDeptNo") %>
            </th>
            <th>
                <span class="Arrow01"></span>
                <%=GetLocalResourceObject("ttDeptName") %>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>                
                <input type="text" id="Input_No" name="Input_No" />
            </td>
            <td>                
                <input type="text" id="Input_Name" name="Input_Name" />
            </td>
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
            </td>
        </tr>
    </table>

        <div id="MainDataArea" style="width:100%;overflow-x:scroll">
        <table class="TableS1" style="width: 1150px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" style="width: 80px;"><%=GetLocalResourceObject("ttNo") %></th>
                    <th scope="col" style="width: 80px;"><%=GetLocalResourceObject("ttName") %></th>
                    <th scope="col" style="width: 220px;"><%=GetLocalResourceObject("ttEngName") %></th>                    
                    <th scope="col" style="width: 220px;"><%=GetLocalResourceObject("ttCreateUser") %></th>
                    <th scope="col" style=""><%=GetLocalResourceObject("ttEmail") %></th>
                </tr>
                <%if (this.MainList.Count>0)
                    { %>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" style="padding: 0" colspan="11">
                        <div id="tablePanel" class="MinHeight"  style="overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="MainGridView" style="border-collapse: collapse;">
                                    <tbody>
                                        <%foreach (var o in this.MainList)
                                            { %>
                                        <tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" onclick="SingleRowSelect(0, this, $('#SelectValue')[0],'<%=o.MgaID %>', '', '');"
                                            ondblclick="CallEdit('管理區設定', '請注意!!  目前無法進行編輯\n\n可能的原因：1.沒有資料可供編輯。\n　　　　　　2.尚未選擇要編輯的項目。\n')">
                                            <td title="<%=o.MgaNo %>" style="width: 83px;"><%=o.MgaNo %></td>
                                            <td title="<%=o.MgaName %>" style="width: 84px;"><%=o.MgaName %></td>
                                            <td title="<%=o.MgaEName %>" style="width: 224px;"><%=o.MgaEName %></td>
                                            <td title="<%=o.CreateUserID %>" style="width: 224px;"><%=o.CreateUserID %></td>
                                            <td title="<%=o.Email %>" style=""><%=o.Email %></td> 
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

    
    <input type="hidden" id="SelectValue" name="SelectValue" />
    <input type="hidden" id="SelectNowNo" name="SelectNowNo" />
    <input type="hidden" id="SelectNowName" name="SelectNowName" />    
    <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hSelectState" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hOwnerList" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <input type="hidden" id="AreaEdit" value="<%=GetLocalResourceObject("ttMngAreaEdit") %>" />
    <input type="hidden" id="AreaAdd" value="<%=GetLocalResourceObject("ttMngAreaAdd") %>" />
    <input type="hidden" id="AreaDel" value="<%=GetLocalResourceObject("ttMngAreaDel") %>" />
    <input type="hidden" id="AuthEdit" value="<%=GetLocalResourceObject("ttMngAuthEdit") %>" />
    <input type="hidden" id="msgDelete" value="<%=GetLocalResourceObject("msgDelete") %>" />
    <input type="hidden" id="msgNonSelect" value="<%=this.GetGlobalResourceObject("Resource","NotSelectForEdit").ToString().Replace("\\n","|") %>" />
    <input type="hidden" id="msgNonDelete" value="<%=this.GetGlobalResourceObject("Resource","NotSelectForDelete").ToString().Replace("\\n","|") %>" />
    <table>
        <tr>
            <td>                
                <input type="button" id="AddButton" name="AddButton" class="IconNew" value="<%=GetGlobalResourceObject("Resource","btnAdd") %>" onclick="CallAdd()" />
            </td>
            <td>                
                <input type="button" id="EditButton" name="EditButton" class="IconEdit" value="<%=GetGlobalResourceObject("Resource","btnEdit") %>" />
            </td>
            <td>                
                <input type="button" id="DeleteButton" name="DeleteButton" class="IconDelete" value="<%=GetGlobalResourceObject("Resource","btnDelete") %>" />
            </td>
            <td>                
                <input type="button" id="AuthButton" name="AuthButton" class="IconOrganization" value="<%=GetLocalResourceObject("btnOrgStruc") %>" />
            </td>
            <td>                
                <input type="button" id="AuthButton2" name="AuthButton2" class="IconGroupSet" value="<%=GetLocalResourceObject("btnEquGroup") %>" />
            </td>
        </tr>
    </table>
     <div id="popOverlay1" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv1" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="角色資料" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <input type="image" name="ImgCloseButton1" id="ImgCloseButton1" src="/Img/close_button.png" style="height:25px;" onclick="DoCancel('1'); return false;" />
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
                                <asp:Label ID="popLabel_No" runat="server" Text="<%$Resources:ttNo %>" Font-Bold="True"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="<%$Resources:ttName %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>                                
                                <input type="text" id="MgaNo" name="MgaNo" class="TextBoxRequired" style="width:180px;border-width:1px" must_keyin_yn="Y" field_name="<%=GetLocalResourceObject("ttNo") %>" />
                            </td>
                            <td>                                
                                <input type="text" id="MgaName" name="MgaName" class="TextBoxRequired" style="width:180px;border-width:1px" must_keyin_yn="Y" field_name="<%=GetLocalResourceObject("ttName") %>" />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EName" runat="server" Text="<%$Resources:ttEngName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">                                
                                <input type="text" id="MgaEName" name="MgaEName" style="width:370px;border-width:1px" />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Email" runat="server" Text="<%$Resources:ttEmail %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                         <tr>
                            <td colspan="2">                                
                                <input type="text" id="MgaEmail" name="MgaEmail" style="width:370px;border-width:1px" />
                            </td>
                        </tr>
                        <tr class="IsDel">
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <span id="ContentPlaceHolder1_popLabel_Char2" style="font-weight:bold;"><%=GetLocalResourceObject("ttChange") %></span>
                            </th>
                        </tr>
                        <tr class="IsDel">
                            <th colspan="2">
                                <select name="popDropDownList_Char2" id="popDropDownList_Char2" class="DropDownListStyle" style="width: 300px;" tabindex="0">
                                    <option value="M999">[M999]全區</option>
                                </select>
                            </th>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Desc" runat="server" Text="<%$Resources:ttDesc %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">                               
                                <input type="text" id="MgaDesc" name="MgaDesc" style="width:370px;border-width:1px" />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Remark" runat="server" Text="<%$Resources:ttRemark %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <textarea name="Remark" rows="4" cols="20" id="Remark" style="width:100%;resize: none;" tabindex="0"></textarea>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">                        
                        <input type="button" name="popB_Add" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" id="popB_Add" class="IconSave" onclick="AddExcute()" />
                        <input type="button" name="popB_Edit" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" id="popB_Edit" class="IconSave" onclick="EditExcute()" />
                        <input type="button" name="popB_Delete" value="<%=GetGlobalResourceObject("Resource","btnDelete") %>" id="popB_Delete" class="IconSave" onclick="DeleteExcute()" />
                        <input type="button" id="popB_Cancel" name="popB_Cancel" class="IconCancel" value="<%=GetGlobalResourceObject("Resource","btnCancel") %>" onclick="DoCancel('1')" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <div id="popOverlay2" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv2" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
         <asp:Panel ID="PanelDrag3" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName3" runat="server" Text="角色資料" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <input type="image" name="ImgCloseButton2" id="ImgCloseButton2" src="/Img/close_button.png" style="height:25px;" onclick="DoCancel('2'); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td class="auto-style1">
                    <table>
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No3" runat="server" Text="<%$Resources:ttNo %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_No3" runat="server" Width="260px" BorderWidth="1" CssClass="TextBoxRequired" Enabled="False"></asp:TextBox>
                            </td>
                            <td colspan="2"></td>
                        </tr>
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_OrgList" runat="server" Text="<%$Resources:ttOrgStruc %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align: center">
                                <select size="4" name="popB_OrgList1" multiple="multiple" id="popB_OrgList1" class="ListBoxStyle" style="height:150px;width:270px;"></select>
                            </td>
                            <td colspan="2" style="padding-right: 5px; text-align: center">                                
                                <input type="button" id="popB_Enter" name="popB_Enter" value="<%=GetGlobalResourceObject("Resource","btnJoin") %>" class="IconRight" onclick="DataEnterRemove('Add')"/>
                                <br />
                                <br />
                                <input type="button" id="popB_Remove" name="popB_Remove" value="<%=GetGlobalResourceObject("Resource","btnRemove") %>" class="IconLeft" onclick="DataEnterRemove('Del')"/>
                            </td>
                            <td style="text-align: center">
                                <select size="4" name="popB_OrgList2" multiple="multiple" id="popB_OrgList2" class="ListBoxStyle" style="height:150px;width:270px;"></select>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="4" style="text-align: center">
                                <asp:Label ID="DeleteLableText3" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="auto-style1">
                    <asp:Panel ID="PanelEdit3" runat="server" EnableViewState="False">                       
                        <input type="button" id="popB_Auth" name="popB_Auth" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" class="IconSave" onclick="AuthExcute()" />
                        <input type="button" id="popB_Cancel3" name="popB_Cancel3" value="<%=GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel" onclick="DoCancel('2')" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <div id="popOverlay3" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv3" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <asp:Panel ID="PanelDrag4" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName4" runat="server" Text="角色資料" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <input type="image" name="ImgCloseButton3" id="ImgCloseButton3" src="/Img/close_button.png" style="height:25px;" onclick="DoCancel('3'); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td class="auto-style1">
                    <table>
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No4" runat="server" Text="<%$Resources:ttNo %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_No4" runat="server" Width="260px" BorderWidth="1" CssClass="TextBoxRequired" Enabled="False"></asp:TextBox>
                            </td>
                            <td colspan="2"></td>
                        </tr>
                        <tr>
                            <th colspan="4">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EquGrList" runat="server" Text="<%$Resources:ttEquGroup %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td style="text-align: center">
                                <select size="4" name="popB_EquGrList1" multiple="multiple" id="popB_EquGrList1" class="ListBoxStyle" style="height:150px;width:270px;"></select>
                            </td>
                            <td colspan="2" style="padding-right: 5px; text-align: center">
                                <input type="button" id="popB_Enter2" name="popB_Enter2" value="<%=GetGlobalResourceObject("Resource","btnJoin") %>" class="IconRight" onclick="DataEnterRemove2('Add')"/>
                                <br />
                                <br />
                                <input type="button" id="popB_Remove2" name="popB_Remove2" value="<%=GetGlobalResourceObject("Resource","btnRemove") %>" class="IconLeft" onclick="DataEnterRemove2('Del')"/>
                            </td>
                            <td style="text-align: center">
                                <select size="4" name="popB_EquGrList2" multiple="multiple" id="popB_EquGrList2" class="ListBoxStyle" style="height:150px;width:270px;"></select>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="4" style="text-align: center">
                                <asp:Label ID="DeleteLableText4" runat="server" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="auto-style1">
                    <asp:Panel ID="PanelEdit4" runat="server" EnableViewState="False">
                        <input type="button" id="popB_Auth2" name="popB_Auth" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" class="IconSave" onclick="AuthExcute2()" />
                        <input type="button" id="popB_Cancel4" name="popB_Cancel4" value="<%=GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel" onclick="DoCancel('3')" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
