<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SysParam.aspx.cs"
    Inherits="SahoAcs.SysParam" Debug="true" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>--%>

    <br />
    <div id="UpdatePanel1">        
        <table class="TableS1">

            <tbody>
                <tr class="GVStyle">
                    <th scope="col" style="width: 80px;"><a href="#" onclick="Sort('ParaClass',this)"><%=GetLocalResourceObject("ttFuncType") %></a><span id="SortSpan">▲</span></th>
                    <th scope="col" style="width: 90px;"><a href="#" onclick="Sort('ParaNo',this)"><%=GetLocalResourceObject("ttFuncCode") %></a></th>
                    <th scope="col" style="width: 140px;"><a href="#" onclick="Sort('ParaName',this)"><%=GetLocalResourceObject("ttName") %></a></th>
                    <th scope="col" style="width: 200px;"><a href="#" onclick="Sort('ParaValue',this)"><%=GetLocalResourceObject("ttValue") %></a></th>
                    <th scope="col" style="width: 80px;"><a href="#" onclick="Sort('ParaType',this)"><%=GetLocalResourceObject("ttDataType") %></a></th>
                    <th scope="col"><a href="#" onclick="Sort('ParaDesc',this)"><%=GetLocalResourceObject("ttDesc") %></a></th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" style="padding: 0" colspan="6">
                        <div id="tablePanel" class="MinHeight" style="overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="border-collapse: collapse;">
                                    <tbody>
                                        <%foreach (var o in this.ListParaData)
                                            { %>
                                        <tr id="GV_Row46" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" onclick="SingleRowSelect(0, this, $('#hSelectValue')[0],'<%=o.RecordID %>', '', '')" ondblclick="CallEdit()" style="color: rgb(0, 0, 0);">
                                            <td title="<%=o.ParaClass %>" style="width: 83px;"><%=o.ParaClass %></td>
                                            <td title="<%=o.ParaNo %>" style="width: 94px;"><%=o.ParaNo %></td>
                                            <td title="<%=o.ParaName %>" style="width: 144px;"><%=o.ParaName %></td>
                                            <td style="width: 204px;"><%=o.ParaValue %></td>
                                            <td title="<%=o.ParaType %>" style="width: 84px;"><%=o.ParaType %></td>
                                            <td title="<%=o.ParaDesc %>"><%=o.ParaDesc %></td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr class="GVStylePgr">
                    <td colspan="6">
                        
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hUserId" runat="server" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <input type="hidden" id="ParaClass" name="ParaClass" value="HideSystem" />
    <input type="hidden" id="SortName" name="SortName" value="<%=this.OrderName %>" />
    <input type="hidden" id="hSelectValue" name="hSelectValue" value="" />
    <input type="hidden" id="SortType" name="SortType" value="<%=this.OrderType %>" />
    <input type="hidden" id="SysParameter" value="<%=this.GetLocalResourceObject("ttSysParaEdit").ToString() %>" />

    <table>
        <tr>
            <td>
                <asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" Visible="false" CssClass="IconNew"/>
            </td>
            <td>                
                <input type="button" id="EditButton" value="<%=GetGlobalResourceObject("Resource","btnEdit") %>" class="IconEdit" onclick="CallEdit('<%=GetGlobalResourceObject("Resource", "NotSelectForEdit")%>')" />
            </td>
            <td>
                <asp:Button ID="DeleteButton" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" Visible="false" CssClass="IconDelete"/>
            </td>
        </tr>
    </table>
    <%-- ************************************************** 網頁畫面設計二 ************************************************** --%>
     <div id="popOverlay1" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <%-- 次作業畫面一：臨時卡借還作業新增與編輯 --%>
    <div id="ParaExtDiv1" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="<%$Resources:ttSysParaEdit %>" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                        <%--<span><%=this.GetLocalResourceObject("ttSysParaEdit") %></span>--%>
                    </td>
                    <td style="text-align: right;">                        
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Class" runat="server" Text="<%$Resources:ttFuncType %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">                                
                                <input id="popInput_Class" type="text" style="width:370px" readonly="readonly" />
                            </td>
                        </tr>

                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No" runat="server" Text="<%$Resources:ttFuncCode %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="<%$Resources:ttName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>                                
                                <input type="text" id="popInput_No" style="width:180px;border-width:0px"  readonly="readonly"  />
                            </td>
                            <td>                                
                                <input type="text" id="popInput_Name" style="width:180px;"  readonly="readonly"  />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Value" runat="server" Text="<%$Resources:ttValue %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">                                
                                <input type="text" id="popInput_Value" style="width:170px" class="TextBoxRequired" FIELD_NAME="<%=GetLocalResourceObject("ttValue") %>"/>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Type" runat="server" Text="<%$Resources:ttDataType %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">                                
                                <input type="text" id="popInput_Type" style="width:370px"  readonly="readonly"  />
                            </td>
                        </tr>

                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Desc" runat="server" Text="<%$Resources:ttDesc %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">                                
                                <input type="text" id="popInput_Desc" style="width:370px"  readonly="readonly"  />
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
                <td colspan="2" style="text-align:center">
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <%--<asp:Button ID="popB_Add" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave"/>--%>                        
                        <input type="button" id="popB_Edit" name="popB_Edit" value="<%=GetGlobalResourceObject("Resource","btnSave") %>" class="IconSave" onclick="EditExcute()" />
                        <%--<asp:Button ID="popB_Delete" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete"/>        --%>                
                        <input type="button" id="popB_Cancel" name="popB_Cancel" value="<%=GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel" onclick="DoCancel('1') "/>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>    
</asp:Content>


