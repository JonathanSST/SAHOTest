<%@ Page Language="C#" CodeBehind="TempCardCreate.aspx.cs" Inherits="SahoAcs.TempCardCreate" MasterPageFile="~/Site1.Master" Debug="true" AutoEventWireup="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


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
    <table style="width: 100%">
        <tr>
            <td>
                <%-- cellspacing="0" border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死 --%>                
                <%-- ************************************************** 網頁畫面設計一 ************************************************** --%>

                <%-- 主要作業畫面：查詢部份 --%>                
                        <table class="Item">
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <th>
                                                <span class="Arrow01"></span>
                                            </th>
                                            <th>
                                                <asp:Label ID="QueryLabel_CardNo" runat="server" Text="<%$ Resources:QueryLabel_CardNo %>"></asp:Label>
                                            </th>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>                                                
                                                <input type="text" id="QueryInput_CardNo" name="QueryInput_CardNo" />
                                            </td>
                                            <td></td>
                                            <td>
                                                <%--<asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" />--%>
                                                <input type="button" id="QueryButton" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" class="IconSearch" onclick="SetQuery()" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>                    
            </td>
        </tr>
        <tr>
            <td>
                <%-- 主要作業畫面：表格部份 --%>
                <%--<asp:HiddenField ID="hMaxCard" runat="server" EnableViewState="False" />--%>
                <%--<asp:HiddenField ID="hCurrentCard" runat="server" EnableViewState="False" />--%>
                <input type="hidden" id="hMaxCard" name="hMaxCard" value="<%=this.MaxCard %>" />
                <input type="hidden" id="hCurrentCard" name="hCurrentCard" value="<%=this.CurrentCard %>" />
                <table class="TableS1" style="width: 100%">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 100px;"><a href="#" onclick="SetOrder('CardNo',this)"> <%=GetLocalResourceObject("CardNo") %></a></th>
                            <th scope="col" style="width: 120px;"><a href="#" onclick="SetOrder('PsnNo',this)"><%=GetLocalResourceObject("PsnID") %></a></th>
                            <th scope="col" style="width: 80px;"><a href="#" onclick="SetOrder('CardNum',this)"> <%=GetLocalResourceObject("CardAuthAllow") %></a></th>
                            <th scope="col"><a href="#" onclick="SetOrder('CardDesc',this)"> <%=GetLocalResourceObject("CardDesc") %></a></th>
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView" style="padding: 0" colspan="4">
                                <div id="tablePanel" style="height: 250px; overflow-y: scroll;">
                                    <div>
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1" id="MainGridView" style="border-collapse: collapse;">
                                            <tbody>
                                                <%foreach (var o in this.ListTempCard)
                                                    { %>
                                                <tr style="color: rgb(0, 0, 0);" ondblclick="CallEdit()" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" onclick="SingleRowSelect(0, this, $('#hSelectValue')[0],'<%=o.CardNo %>', '', '')">
                                                    <td title="<%=o.CardNo %>" style="text-align:center; width: 104px;"><%=o.CardNo %></td>
                                                    <td style="text-align:center; width: 124px;"><%=o.PsnNo %></td>
                                                    <td title="" style="text-align:center; width: 84px;"><%=o.CardNum %></td>
                                                    <td style="text-align:left"><%=o.CardDesc %></td>
                                                </tr>
                                                <%} %>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr class="GVStylePgr">
                            <td colspan="4">
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <%-- 主要作業畫面：按鈕部份 --%>
                <table>
                    <tr>
                        <td>
                            <%--<asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>
                            <input type="button" id="AddButton" value="<%=GetGlobalResourceObject("Resource","btnAdd") %>" class="IconNew" onclick="CallAdd()" />
                        </td>
                        <td>
                            <%--<asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit" />--%>
                            <input type="button" id="EditButton" value="<%=GetGlobalResourceObject("Resource","btnEdit") %>" class="IconEdit" onclick="CallEdit()" />
                        </td>
                        <td>
                            <%--<asp:Button ID="DeleteButton" runat="server" Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete" />--%>
                            <input type="button" id="DeleteButton" value="<%=GetGlobalResourceObject("Resource","btnDelete") %>" class="IconDelete" onclick="CallDelete('1','2','3')" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <%-- 主要作業畫面：隱藏欄位部份 --%>
    <asp:HiddenField ID="hUserID" runat="server" />
    <asp:HiddenField ID="hOwnerList" runat="server" />    
    <input type="hidden" id="hSelectValue" name="hSelectValue" value="" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hCardID" runat="server" EnableViewState="False" />    
    <input type="hidden" id="msgNonSelect" value="<%=this.GetGlobalResourceObject("Resource","NotSelectForEdit").ToString()%>" />
    <input type="hidden" id="msgNonDelete" value="<%=this.GetGlobalResourceObject("Resource","NotSelectForDelete").ToString() %>" />
    <input type="hidden" id="msgChkDelete" value="<%=this.GetLocalResourceObject("CallDelete_DelLabel").ToString() %>" />
    <input type="hidden" id="msgEdit" value="<%=this.GetLocalResourceObject("CallEdit_Title") %>" />
    <input type="hidden" id="msgAdd" value="<%=this.GetLocalResourceObject("CallAdd_Title") %>" />
    <input type="hidden" id="msgDel" value="<%=this.GetLocalResourceObject("CallDelete_Title") %>" />
    <input type="hidden" id="OrderName" value="CardNo" />
    <input type="hidden" id="OrderType" value="ASC" />
    


    <%-- ************************************************** 網頁畫面設計二 ************************************************** --%>
     <div id="popOverlay1" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <%-- 次作業畫面一：臨時卡借還作業新增與編輯 --%>
    <div id="ParaExtDiv1" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="<%$ Resources:lblTCardCreate %>" Font-Bold="True" ForeColor="White" EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">                        
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table style="width: 290px">
                        <tr>
                            <td>
                                <span class="Arrow01"></span>
                            </td>
                            <th>
                                <asp:Label ID="popLabel_CardNo" runat="server" Text="<%$ Resources:popLabel_CardNo %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td></td>
                            <td>                                
                                <input type="text" id="popInput_CardNo" name="popInput_CardNo" value="" style="width:90%" maxlength="<%=this.CardLen %>" must_keyin_yn="Y" field_name="<%=GetLocalResourceObject("popLabel_CardNo") %>" class="TextBoxRequired" />
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <span class="Arrow01"></span>
                            </td>
                            <th>
                                <asp:Label ID="popLabel_CardDesc" runat="server" Text="<%$ Resources:popLabel_CardDesc %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td></td>
                            <td>                                
                                <input type="text" id="popInput_CardDesc" name="popInput_CardDesc" value="" style="width:90%" maxlength="50" />
                            </td>
                        </tr>
                        <tr>
                            <th style="text-align: center" colspan="2">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label></th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
<%--                        <asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" CssClass="IconSave" />
                        <asp:Button ID="popB_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconSave" />--%>
                        <input type="button" id="popB_Add" name="popB_Add" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" class="IconSave" onclick="AddExcute()" />
                        <input type="button" id="popB_Edit" name="popB_Edit" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" class="IconSave" onclick="EditExcute()"/>
                        <input type="button" id="popB_Delete" name="popB_Delete" value="<%=GetGlobalResourceObject("Resource","btnDelete") %>" class="IconDelete" onclick="DeleteExcute()"/>
                        <input type="button" id="popB_Cancel" name="popB_Cancel" value="<%=GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel" onclick="DoCancel('1')" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>           
</asp:Content>
