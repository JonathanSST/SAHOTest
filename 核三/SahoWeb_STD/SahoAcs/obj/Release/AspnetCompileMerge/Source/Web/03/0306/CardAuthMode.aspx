<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" EnableEventValidation="false"
    Theme="UI" Debug="true" CodeBehind="CardAuthMode.aspx.cs"
    Inherits="SahoAcs.Web._03._0306.CardAuthMode" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tbody>
            <tr>
                <th>
                    <span class="Arrow01"></span>
                    <span id="ContentPlaceHolder1_lblCompany">條件類型</span>
                </th>
                <th>
                    <span class="Arrow01"></span>
                    <span id="ContentPlaceHolder1_lblKeyWord">關鍵字</span>
                </th>
                <td></td>
            </tr>
            <tr>
                <td>
                    <select name="QueryType" id="QueryType" class="DropDownListStyle" style="width: 150px;">
                        <option value="CardNo">卡號</option>
                        <option value="PsnNo">人員編號</option>
                        <option value="PsnName">人員姓名</option>
                        <option value="Org">單位部門代碼</option>
                    </select>
                </td>
                <td>
                    <input name="QueryName" type="text" id="QueryName" />
                </td>
                <td>
                    <input type="button" name="ctl00$ContentPlaceHolder1$QueryButton" value="查    詢" onclick="Query(); return false;" id="ContentPlaceHolder1_QueryButton" class="IconSearch" />
                    <input type="button" name="ctl00$ContentPlaceHolder1$btSave" value="重    設" id="ContentPlaceHolder1_btSave" class="aspNetDisabled IconSave" onclick="DoReset()" />
                </td>
            </tr>
        </tbody>
    </table>
    <table>
        <tr>
            <td>
                <fieldset id="ContentPlaceHolder1_Pending_List" style="width: 600px; height: 380px">
                    <legend id="ContentPlaceHolder1_Pending_Legend">資料列表</legend>
                    <table class="TableS1" style="width: 600px">
                        <tbody>
                            <tr class="GVStyle">
                                <th scope="col" style="width: 30px;">
                                    <input type="checkbox" id="CheckAll" onchange="SetCheckAll(this)" style="width: 20px; height: 20px" />
                                </th>
                                <th scope="col" style="width: 100px;">姓名</th>
                                <th scope="col" style="width: 100px;">人員編號</th>
                                <th scope="col" style="width: 100px;">卡號</th>
                                <th scope="col">單位部門</th>
                            </tr>
                            <tr>
                                <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                                    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
                                    <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 600px; height: 320px; overflow-y: scroll;">
                                        <div>
                                            <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                                <tbody>
                                                    <%foreach (var card in this.CardList)
                                                        { %>
                                                    <tr class="GV_Row2" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                                        <td style="width: 34px; text-align: center">
                                                            <input style="width: 20px; height: 20px" type="checkbox" name="ChkCardNo" id="ChkCardNo" value="<%=card.CardNo %>" /></td>
                                                        <td style="width: 104px;"><%=card.PsnName %></td>
                                                        <td style="width: 104px;"><%=card.PsnNo %></td>
                                                        <td style="width: 104px;"><%=card.CardNo %></td>
                                                        <td><%=card.OrgNameList %><br />
                                                            <%=card.OrgNoList %></td>
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
                </fieldset>
            </td>
            <td>
                <fieldset id="ContentPlaceHolder1_Pending_Equ" style="width: 600px; height: 380px">
                    <legend id="ContentPlaceHolder1_Pending_Legend2">驗證設備列表</legend>
                    <table class="TableS1" style="width: 600px">
                        <tbody>
                            <tr class="GVStyle">
                                <th scope="col" style="width: 80px;">設備類型</th>
                                <th scope="col" style="width: 80px;">設備編號</th>
                                <th scope="col" style="width: 200px;">設備名稱</th>
                                <th scope="col">驗證模式</th>
                            </tr>
                            <tr>
                                <td id="ContentPlaceHolder1_EquData" colspan="9">
                                    <asp:HiddenField ID="HiddenField1" runat="server" EnableViewState="False" />
                                    <div id="ContentPlaceHolder1_EquDataPanel" class="MinHeight" style="width: 600px; height: 320px; overflow-y: scroll;">
                                        <div>
                                            <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_EquGridView" style="width: 100%; border-collapse: collapse;">
                                                <tbody>
                                                    <%foreach (var equ in this.EquList)
                                                        { %>
                                                    <tr class="GV_Row2" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                                        <td style="width: 84px;"><%=equ.EquModel %></td>
                                                        <td style="width: 84px;"><%=equ.EquNo %></td>
                                                        <td style="width: 204px;"><%=equ.EquName %></td>
                                                        <td>
                                                            <input type="hidden" id="EquID" name="EquID" value="<%=equ.EquID %>" />
                                                            <%if (equ.EquModel == "ADM100FP")
                                                            { %>
                                                            <select id="VerifiMode" name="VerifiMode" style="width: 95%" class="DropDownListStyle">
                                                                <option value="0">指紋</option>
                                                                <option value="1">指紋或卡片</option>
                                                                <option value="2">指紋+卡片</option>
                                                            </select>
                                                            <%} %>
                                                            <%if (equ.ItemInfo3 == "ESD")
                                                            { %>
                                                            <select id="VerifiMode>" name="VerifiMode" style="width: 95%" class="DropDownListStyle">
                                                                <option value="0"><%=this.GetLocalResourceObject("EsdVerify1") %></option>
                                                                <option value="1"><%=this.GetLocalResourceObject("EsdVerify2") %></option>
                                                                <option value="2"><%=this.GetLocalResourceObject("EsdVerify3") %></option>
                                                                <option value="3"><%=this.GetLocalResourceObject("EsdVerify4") %></option>
                                                                <option value="4"><%=this.GetLocalResourceObject("EsdVerify5") %></option>
                                                            </select>
                                                            <%} %>
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
                </fieldset>
            </td>
        </tr>
    </table>
    <table class="Item">
        <tbody>
            <tr>
                <td style="text-align: center; width: 1250px;">
                                        
                </td>
            </tr>
        </tbody>
    </table>
    <div>
    </div>
</asp:Content>
