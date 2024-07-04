<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardEditV2.aspx.cs" Inherits="SahoAcs.Web._01._0103.CardEditV2" %>
<%@ Register Src="../../../uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form_card" runat="server">
        <div id="PanelPopup1"style="border-width: 1px; border-style: solid; width: 400px;">
            <div id="PanelDrag1" style="background-color: #2D89EF; height: 28px;">
                <table style="width: 100%; padding: 0px;">
                    <tbody>
                        <tr>
                            <td>
                                <span id="ContentPlaceHolder1_L_popName1" style="color: White; font-weight: bold; vertical-align: middle">卡片資料編輯</span>
                            </td>
                            <td style="text-align: right;"></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <table>
                <tbody>
                    <tr>
                        <td>
                            <table class="popItem">
                                <tbody>
                                    <tr>
                                        <td>
                                            <table style="padding: 0px; border-collapse: collapse;">
                                                <tbody>
                                                    <tr>
                                                        <th colspan="2">
                                                            <span id="ContentPlaceHolder1_popLabel_CardNo" style="font-weight: bold;">卡片號碼</span>
                                                        </th>
                                                        <th colspan="2">
                                                            <span id="ContentPlaceHolder1_popLabel_CardVer" style="font-weight: bold;">版次</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <input name="popCardNo" type="text" maxlength="<%=this.CardLen %>" id="popCardNo" style="width: 180px;" disabled="disabled" value="<%=this.CardObj.CardNo %>"
                                                                 must_keyin_yn="Y" field_name="卡號"/>
                                                        </td>
                                                        <td colspan="2">
                                                            <input name="popCardVer" type="text" maxlength="1" id="popCardVer" disabled="disabled" class="aspNetDisabled" style="width: 70px;" value="<%=this.CardObj.CardVer %>" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="ContentPlaceHolder1_popLabel_CardPW" style="font-weight: bold;">卡片密碼</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <input name="popCardPW" type="text" maxlength="4" id="popCardPW" style="width: 180px;" value="<%=this.CardObj.CardPW %>" must_keyin_yn="Y" field_name="卡片密碼" />
                                                        </td>
                                                        <td colspan="2"></td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="ContentPlaceHolder1_popLabel_CardSerialNo" style="font-weight: bold;">卡片序號</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <input name="popCardSerialNo" type="text" id="popCardSerialNo" style="width: 180px;" value="<%=this.CardObj.CardSerialNo %>" />
                                                        </td>
                                                        <td colspan="2"></td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="ContentPlaceHolder1_popLabel_CardNum" style="font-weight: bold;">卡片編號</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <input name="popCardNum" type="text" id="popCardNum" style="width: 180px;" value="<%=this.CardObj.CardNum %>"/>
                                                        </td>
                                                        <td colspan="2"></td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="2">
                                                            <span id="ContentPlaceHolder1_popLabel_CardType" style="font-weight: bold;">卡片類型</span>
                                                        </th>
                                                        <th colspan="2">
                                                            <span id="ContentPlaceHolder1_popLabel_CardAuthAllow0" style="font-weight: bold;">卡片權限</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <select name="popCardType" id="popCardType" class="DropDownListStyle" style="width: 190px;">
                                                                <%foreach (var cardtype in this.CardTypes)
                                                                    { %>
                                                                        <option value="<%=cardtype.ItemNo %>"><%=cardtype.ItemName %></option>
                                                                <%} %>
                                                            </select>
                                                        </td>
                                                        <td colspan="2">
                                                            <select name="popCardAuthAllow" id="popCardAuthAllow" class="DropDownListStyle" style="width: 85px;">
                                                                <%foreach (var o in this.CardAuth)
                                                                    { %>
                                                                <%if (this.CardObj.CardAuthAllow == Convert.ToInt32(o.CardAuthAllow)){ %>
                                                                <option selected="selected" value="<%=Convert.ToInt32(o.CardAuthAllow) %>"><%=Convert.ToString(o.Name) %></option>
                                                                <%} else { %>
                                                                <option value="<%=Convert.ToInt32(o.CardAuthAllow) %>"><%=Convert.ToString(o.Name) %></option>
                                                                <%} %>
                                                                
                                                                <%} %>
                                                            </select>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="ContentPlaceHolder1_popLabel_CardSTime" style="font-weight: bold;">啟用時間</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="4">
                                                            <uc1:Calendar ID="CardSTime" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="ContentPlaceHolder1_popLabel_CardETime" style="font-weight: bold;">停用時間</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="4">
                                                            <uc1:Calendar ID="CardETime" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="ContentPlaceHolder1_popLabel_CardDesc" style="font-weight: bold;">卡片描述</span>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="4">
                                                            <textarea name="popCardDesc" rows="2" cols="20" id="popCardDesc" style="height: 45px; width: 290px;"><%=this.CardObj.CardDesc %></textarea>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="4" style="align-content: center">
                                                            <span id="ContentPlaceHolder1_DeleteLableText" style="font-weight: bold;"></span>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <div id="ContentPlaceHolder1_PanelEdit1">
                                                <input type="button" name="popB_Add" value="儲     存" onclick="SaveCardData(); return false;" id="popB_Add" class="IconSave" />
                                                <%if (this.CardObj.CardID != 0)
                                                    { %>
                                                <input type="button" name="popB_Delete" value="刪     除" onclick="DeleteCardData(); return false;" id="popB_Delete" class="IconDelete" style="display: inline;" />
                                                <%} %>
                                                <input type="button" name="popB_Cancel" value="取     消" onclick="DoCancel()" id="popB_Cancel" class="IconCancel" />
                                                <input type="hidden" name="CardID" id="CardID" value="<%=this.CardObj.CardID %>" />
                                                <input type="hidden" name="NowCardNo" id="NowCardNo" value="<%=this.CardObj.CardNo %>" />
                                                <input type="hidden" id="CardErrMsg" value="<%=this.ErrMsg %>" />
                                                <input type="hidden" id="DoAction" name="DoAction" value="Save" />
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
