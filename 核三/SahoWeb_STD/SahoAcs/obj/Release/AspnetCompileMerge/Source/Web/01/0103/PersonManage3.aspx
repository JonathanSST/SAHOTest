<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI" CodeBehind="PersonManage3.aspx.cs" Inherits="SahoAcs.Web.PersonManage3" %>
<%@ Register Src="../../../uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>
<%@ Register src="../../../uc/CalendarFrm.ascx" tagname="CalendarFrm" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblCompany" runat="server" Text="<%$ Resources:lblCompany %>"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblDepartment" runat="server" Text="<%$ Resources:lblDepartment %>"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblPsnType" runat="server" Text="<%$ Resources:lblPsnClass %>"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblKeyWord" runat="server" Text="<%$ Resources:lblKeyWord %>"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <%--<asp:DropDownList ID="dropCompany" runat="server" Width="150px"></asp:DropDownList>--%>
                <select style="width: 200px" class="DropDownListStyle" id="CompanyList" name="CompanyList">
                    <option value=""><%=Resources.Resource.ddlSelectDefault %></option>
                    <%foreach (var o in this.OrgDataInit.Where(i => i.OrgClass == "Company"))
                        { %>
                    <option value="<%=o.OrgID %>"><%=o.OrgName+"."+o.OrgNo%></option>
                    <%} %>
                </select>
            </td>
            <td>
                <select style="width: 200px" class="DropDownListStyle" id="DeptList" name="DeptList">
                    <option value=""><%=Resources.Resource.ddlSelectDefault %></option>
                    <%foreach (var o in this.OrgDataInit.Where(i => i.OrgClass == "Unit" || i.OrgClass == "Department"))
                        { %>
                    <option value="<%=o.OrgID %>"><%=o.OrgName+"."+o.OrgNo%></option>
                    <%} %>
                </select>
            </td>
            <td>
                <select style="width: 200px" class="DropDownListStyle" id="PsnTypeList">
                    <option value=""><%=Resources.Resource.ddlSelectDefault %></option>
                    <%foreach (var o in this.PsnTypeList)
                        { %>
                    <option value="<%=Convert.ToString(o.ItemNo) %>"><%=string.Format("{0}.{1}",o.ItemName,o.ItemNo)%></option>
                    <%} %>
                </select>
            </td>
            <td>
                <asp:TextBox ID="InputWord" runat="server"></asp:TextBox>
            </td>
            <td>
                <input type="button" id="BtnQuery" value="<%=Resources.Resource.btnQuery %>" class="IconSearch" onclick="SetMainQuery('1')" />
                <input type="button" id="BtnPsnAdd" value="<%=Resources.Resource.btnAdd %>" class="IconNew" onclick="AddNewData()" />
            </td>
        </tr>
    </table>
    <table style="width: 100%; border-collapse: collapse; border: solid 0px;">
        <tr>
            <td style="vertical-align: top; width: 500px">
                <fieldset id="Pending_Data" runat="server" style="height: 610px">
                    <legend id="Legend1" runat="server"><%=GetLocalResourceObject("ttPerson") %></legend>
                    <div id="ResultArea">
                    </div>
                </fieldset>
            </td>
            <td style="vertical-align: top">
                <fieldset id="Pending_List" runat="server" style="height: 610px">
                    <legend id="Pending_Legend" runat="server"><%=GetLocalResourceObject("ttPersonEdit") %></legend>
                    <div id="EditArea">
                        <table class="Item">
                            <tr>
                                <td style="vertical-align: top; width: 250px;">
                                    <div>
                                        <span><%=GetLocalResourceObject("lblPsnNo") %></span>
                                    </div>
                                    <div>
                                        <input type="text" name="PsnNo" id="PsnNo" style="width:180px" must_keyin_yn="Y" field_name="人員編號" value="<%=this.PersonObj.PsnNo %>" maxlength="6" />
                                    </div>
                                    <div>
                                        <span><%=GetLocalResourceObject("lblPsnName") %></span>
                                    </div>
                                    <div>
                                        <input type="text" name="PsnName" id="PsnName" style="width:180px" must_keyin_yn="Y" field_name="人員姓名" value="<%=this.PersonObj.PsnName %>" />
                                    </div>
                                    <div>
                                        <span><%=GetLocalResourceObject("lblPsnEName") %></span>
                                    </div>
                                    <div>
                                        <input type="text" name="PsnEName" id="PsnEName" style="width:180px" must_keyin_yn="N" field_name="英文姓名" value="<%=this.PersonObj.PsnEName %>" />
                                    </div>
                                    <div>
                                        <span><%=GetLocalResourceObject("lblPsnClass") %></span>
                                    </div>
                                    <div>
                                        <select style="width: 200px" class="DropDownListStyle" id="PsnType" name="PsnType">
                                            <%foreach (var o in this.PsnTypeList)
                                                { %>
                                            <option value="<%=Convert.ToString(o.ItemNo) %>"><%=string.Format("{0}.{1}",o.ItemName,o.ItemNo)%></option>
                                            <%} %>
                                        </select>
                                    </div>
                                     <div>
                                        <span><%=GetLocalResourceObject("lblIDNum") %></span>
                                    </div>
                                    <div>
                                        <input type="text" name="IDNum" id="IDNum" style="width:180px" must_keyin_yn="N" field_name="身份證號" value="<%=this.PersonObj.IDNum %>" />
                                    </div>
                                    <div>
                                        <span>生日：</span>
                                    </div>
                                    <div>
                                        <uc2:CalendarFrm ID="BirthDay" runat="server" />                                        
                                    </div>
                                    <%foreach (var o in this.ItemList)
                                        { %>
                                    <div>
                                        <%=string.Format("{0}：",o.ItemName) %>
                                    </div>
                                    <div>
                                        <select style="width: 200px" class="DropDownListStyle" id="<%=string.Format("OrgData{0}",o.ItemNo) %>" name="OrgDataNo">
                                            <option value=""><%=Resources.Resource.ddlSelectDefault %></option>
                                            <%foreach (var c in this.OrgDataInit.Where(i => i.OrgClass == Convert.ToString(o.OrgClass)))
                                                { %>
                                            <%if (this.OrgInfoList2.Contains(c.OrgID.ToString()))
                                                { %>                                            
                                            <option selected="selected" value="<%=c.OrgID %>"><%=c.OrgName+"."+c.OrgNo%></option>
                                            <%}
                                            else
                                            { %>
                                            <option value="<%=c.OrgID %>"><%=c.OrgName+"."+c.OrgNo%></option>
                                            <%} %>

                                            <%} %>
                                        </select>
                                    </div>
                                    <%} %>
                                    <div>
                                        <span id="ContentPlaceHolder1_lblPsnAccount">登入帳號：</span>
                                    </div>
                                    <div>
                                        <input name="PsnAccount" type="text" id="PsnAccount" style="width: 180px;" value="<%=this.PersonObj.PsnAccount %>" must_keyin_yn="N" field_name="登入帳號"  />
                                    </div>
                                    <div>
                                        <span id="ContentPlaceHolder1_lblPsnPW">登入密碼：</span>
                                    </div>
                                    <div>
                                        <input name="PsnPW" type="password" id="PsnPW" style="width: 180px;" value="<%=this.PersonObj.PsnPW %>" must_keyin_yn="N" field_name="登入密碼"  />
                                    </div>
                                </td>
                                <td style="vertical-align:top;width:250px">
                                    <%if (this.PersonObj.PsnNo == null)
                                        { %>
                                     <div>
                                    <asp:Label ID="lblCardNo" runat="server" Text="<%$ Resources:lblCardNo %>"></asp:Label>
                                </div>
                                <div>                                    
                                    <input type="text" id="CardNo" style="width:180px;" name="CardNo" must_keyin_yn="Y" maxlength="<%=this.CardLen %>" />
                                </div>   
                                    <%} %>
                                    <div>
                                        <span id="lblPsnAuthAllow"><%=GetLocalResourceObject("lblPsnAuthAllow") %></span>
                                    </div>
                                    <div>
                                        <select name="PsnAuthAllow" id="PsnAuthAllow" class="DropDownListStyle" style="width: 80px;">
                                            <%if (this.PersonObj.PsnAuthAllow == "0")
                                                {%>
                                                    <option selected="selected" value="0"><%=GetLocalResourceObject("SelectDisabled")  %></option>
                                                <%}
                                                else
                                                { %>
                                                    <option value="0"><%=GetLocalResourceObject("SelectDisabled")  %></option>
                                            <%} %>
                                            <%if (this.PersonObj.PsnAuthAllow == "1")
                                                {%>
                                                    <option selected="selected" value="1"><%=GetLocalResourceObject("SelectEnabled") %></option>
                                                <%}
                                                else
                                                { %>
                                                    <option value="1"><%=GetLocalResourceObject("SelectEnabled") %></option>
                                            <%} %>
                                        </select>
                                    </div>
                                    <div>
                                    <asp:Label ID="lblPsnSTime" runat="server" Text="到職日"></asp:Label>
                                </div>
                                <div>
                                    <uc1:Calendar ID="PsnSTime" runat="server" />
                                </div>
                                    <div>
                                    <asp:Label ID="lblPsnETime" runat="server" Text="使用到期日"></asp:Label>
                                </div>
                                <div>
                                    <uc1:Calendar ID="PsnETime" runat="server" />
                                </div>
                                    <%foreach (var o in this.TextInfo)
                                    { %>
                                    <div>
                                        <span id="<%="lbl"+Convert.ToString(o.ParaNo) %>"><%=Convert.ToString(o.ParaName) %>：</span>
                                    </div>
                                    <div>
                                        <input type="text" name="<%=Convert.ToString(o.ParaNo) %>" id="<%=Convert.ToString(o.ParaNo)  %>" value="<%=Convert.ToString(o.ParaValue) %>"/>
                                    </div>
                                    <%} %>
                                    <div>
                                        <span>班別</span>
                                    </div>
                                    <div>
                                        <select id="Text5" name="Text5" class="DropDownListStyle">
                                            <%foreach (var p in this.DictClass)
                                                { %>
                                            <%if (this.PersonObj.Text5.Equals(p.Key))
                                                { %>
                                            <option selected="selected" value="<%=p.Key %>"><%=p.Value %></option>
                                            <%}
                                            else
                                            { %>
                                            <option value="<%=p.Key %>"><%=p.Value %></option>
                                            <%} %>
                                            <%} %>
                                        </select>
                                    </div>
                                    <div>
                                        <span id="ContentPlaceHolder1_lblRemark"><%=GetLocalResourceObject("lblRemark") %></span>
                                    </div>
                                    <div>
                                        <textarea name="Remark" rows="2" cols="20" id="Remark" style="height:52px;width:190px;"></textarea>
                                    </div>
                                </td>
                                <td style="vertical-align: top; width: 250px">
                                    <%if (this.PersonObj.PsnNo == null)
                                        { %>
                                    <div>
                                        <span id="ContentPlaceHolder1_lbl_CardVer" style="font-weight: bold;"><%=GetLocalResourceObject("popLabel_CardVer") %>：</span>
                                    </div>
                                    <div>
                                         <input <%if(!CardVer.Equals("Y"))
                                            { %>
                                            disabled="disabled"
                                            <%} %>
                                            name="CardVer" type="text" maxlength="1" id="CardVer" style="width: 90px;"  field_name="<%=GetLocalResourceObject("popLabel_CardVer")%>" must_keyin_yn="<%=this.CardVer %>"  />
                                    </div>
                                    <%} %>
                                    <div id="ContentPlaceHolder1_DivVerifiTitle">
                                        <span id="ContentPlaceHolder1_lblVerifiMode"><%=GetLocalResourceObject("AuthMode") %>：</span>
                                    </div>
                                    <div id="ContentPlaceHolder1_DivVerifiContent">
                                        <select name="VerifiMode" id="VerifiMode" class="DropDownListStyle" style="width: 180px">
                                            <%foreach (var o in this.AuthMode)
                                                { %>
                                            <%if (this.PersonObj.VerifiMode == Convert.ToString(o.ItemNo))
                                                { %>
                                            <option value="<%=o.ItemNo %>" selected="selected"><%=o.ItemName %></option>
                                            <%}
                                            else
                                            { %>
                                                <option value="<%=o.ItemNo %>"><%=o.ItemName %></option>
                                            <%} %>

                                            <%} %>
                                        </select>
                                        <%if (this.PersonObj.PsnNo != null)
                                            { %>
                                        <p style="height: 3px">&nbsp;</p>
                                        <input type="button" name="BtnAuthMode" value="設定認證模式至所有卡片" onclick="SetAuthMode(); return false;" id="BtnAuthMode" class="IconTransit" />
                                        <%} %>
                                    </div>                                    
                                    <%if (this.PersonObj.PsnNo != null)
                                        { %>                                    
                                    <div>
                                        <span id="ContentPlaceHolder1_lblCardInfo" style="display: inline;">卡片資訊：</span>
                                    </div>
                                    <div>
                                        <select size="4" name="CardInfo" id="CardInfo" class="ListBoxStyle" style="height: 50px; width: 180px;">
                                            <%foreach(var o in this.CardList){ %>
                                                <option value="<%=Convert.ToString(o.CardID) %>"><%=string.Format("[{1}]{0}",o.CardNo,o.ItemName) %></option>
                                            <%} %>
                                        </select>
                                    </div>
                                    <div>
                                        <p style="height: 3px">&nbsp;</p>
                                        <input type="button" name="btCardInfo" value="<%=GetLocalResourceObject("btCardInfo") %>" id="btCardInfo" class="IconLook" onclick="CallCardEdit('update')"/>
                                        <p style="height: 3px">&nbsp;</p>
                                        <input type="button" name="btCardAdd" value="<%=GetGlobalResourceObject("resource","btnAdd") %>" id="btCardAdd" onclick="CallCardEdit('add')" class="IconNew" style="display: inline;"/>
                                        <p style="height: 3px">&nbsp;</p>
                                        <input type="button" class="IconSet" value="設備權限調整" id="BtCardGroup" style="display: inline;" onclick="SetCardAuthEdit()"/>
                                        <p style="height: 3px">&nbsp;</p>
                                        <input type="button" class="IconPassword" value="快速設碼" id="BtSetting" style="display: inline;" onclick="SetCardCode()"/>
                                        <p style="height: 3px">&nbsp;</p>
                                        <input type="button" class="IconChange" value="變更卡號" id="btnChange" name="btnChange" onclick="ChangeCard2(1)" disabled="" style="display: inline;"/>
                                        <p style="height: 3px">&nbsp;</p>
                                        <input type="button" class="IconSet" value="照片編輯" id="btnPic" name="btnPic" onclick="UploadPicSource()" disabled="" style="display: inline;"/>
                                    </div>
                                    <%} %>
                                </td>
                            </tr>
                        </table>
                          <table class="Item">
                        <tr>
                            <td colspan="4" style="text-align: center; width: 1250px;">
                                <hr />                                                                
                                <input type="button" id="btSave" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" class="IconSave" onclick="SaveData()" />
                                <input type="button" id="btUpdate" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" class="IconSave" onclick="SaveData()" />
                                <input type="button" id="btDelete" value="<%=GetGlobalResourceObject("Resource","btnDelete") %>" class="IconDelete" onclick="DeleteData()" />
                                <input type="hidden" value="<%=this.ErrMessage %>" id="ErrMsg" />
                                <%--<input type="hidden" value="<%=this.ErrResult %>" id="ErrResult" />--%>
                            </td>
                        </tr>
                    </table>
                         <div id="HiddenDiv">
                            <input type="hidden" name="PsnID" id="PsnID" value="<%=this.PersonObj.PsnID %>" />
                             <input type="hidden" name="SelectValue" id="SelectValue" value="<%=this.PersonObj.PsnID %>" />
                            <input type="hidden" name="PageEvent" id="PageEvent" value="Save" />
                            <input type="hidden" name="AuthList" id="AuthList" value="<%=this.AuthList %>" />             
                                 <input type="hidden" name="MaxCard" id="MaxCard" value="<%=SahoAcs.DBClass.DongleVaries.GetMaxPerson() %>" />
                                <input type="hidden" name="CurrentCard" id="CurrentCard" value="<%=SahoAcs.DBClass.DongleVaries.GetCurrentCard() %>" />                                             
                        </div>
                    </div>
                </fieldset>
            </td>
        </tr>
    </table>
</asp:Content>
