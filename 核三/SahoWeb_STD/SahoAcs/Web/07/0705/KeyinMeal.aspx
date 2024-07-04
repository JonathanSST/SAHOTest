<%@ Page Title="" Theme="UI"  Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="KeyinMeal.aspx.cs" Inherits="SahoAcs.Web._07._0705.KeyinMeal" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../../../uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ValueKeep">
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
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <input type="hidden" id="_CardNo" name="_CardNo" value="" runat="server"/>
        <input type="hidden" id="_CardVer" name="_CardVer" value=""  runat="server" />
        <input type="hidden" id="_PsnNo" name="_PsnNo" value="" runat="server"/>
        <input type="hidden" id="_PsnName" name="_PsnName" value="" runat="server"/>
        <input type="hidden" id="_MealNo" name="_MealNo" value="" runat="server"/>
        <input type="hidden" id="_MealDate" name="_MealDate" value="" runat="server"/>
        <input type="hidden" id="_MealFood" name="_MealFood" value="" runat="server"/>
    </div>
   <table class="Item">
        <tr>
            <th>
                <fieldset id="KeyIn" runat="server" style="width: 1200px">
                     <legend id="PsnCard_Legend" runat="server">用餐補登</legend>
                <table class="Item">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_PsnNo" runat="server" Text="人員編號"></asp:Label>
                        </th>
                        <th>
                           <asp:TextBox ID="Input_PsnNo" runat="server" Width="120px" MaxLength="10"></asp:TextBox>
                        </th>
                        <th>
                           <%-- <span class="Arrow01"></span>--%>
                            <asp:Label ID="Label_PsnName" runat="server" Text="姓名"></asp:Label>
                            <asp:TextBox ID="Input_PsnName" runat="server" Width="120px" MaxLength="10"></asp:TextBox>
                        </th>
                        <th>
                         <%--  <span class="Arrow01"></span>--%>
                           <asp:Label ID="Label_CardNo" runat="server" Text="卡號"></asp:Label>
                           <asp:TextBox ID="Input_CardNo" runat="server" Width="120px" MaxLength="10"></asp:TextBox>
                        </th>
                        <th>
                            (可擇一)
                            <input type="button" class="IconSearch" value="查     詢" id="BtnSearch" onclick="QuerySearch()" />
                            <input type="button" class="IconSearch" value="清 除 資 料" onclick="Clear();" />
                        </th>
                    </tr>
                    <tr>
                        <th>
                         <span class="Arrow01"></span>
                            <asp:Label ID="Label_OrderDate" runat="server" Text="用餐日"></asp:Label>
                        </th>
                        <th>
                            <uc2:CalendarFrm runat="server" ID="Calendar_CardTimeSDate" />
                        </th>
                        <th>&nbsp;</th>
                        <th>&nbsp;</th>
                        <th>&nbsp;</th>
                    </tr>
                   
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="餐別"></asp:Label> 
                        </th>
                        <th>
                             <asp:RadioButtonList ID="rdb_MealNo" runat="server"  RepeatDirection="Horizontal" Font-Size="Large" >
                                <asp:ListItem Selected="True" Value="1">午餐</asp:ListItem>
                                <asp:ListItem Value="2">宵夜</asp:ListItem>
                            </asp:RadioButtonList>
                        </th>
                        <th>
                            </th>
                        <th>
                            </th>
                        <th>
                            </th>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                         <td>&nbsp;</td>
                         <td>&nbsp;</td>
                         <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="5">
                        </td>
                    </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
    </table>
   <div style="text-align:center;">
        <input type="button" class="IconEdit" value="補    登" id="BtnKeyIn" onclick="KeyIn()" />
    </div>
</asp:Content>
