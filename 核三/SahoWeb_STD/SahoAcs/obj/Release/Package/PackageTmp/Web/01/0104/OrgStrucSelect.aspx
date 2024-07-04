<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrgStrucSelect.aspx.cs" Inherits="SahoAcs.Web._01._0104.OrgStrucSelect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="PopDiv" class="PopBg" style="background-color: white; border-color: whitesmoke; border-width: 1px; border-style: solid; width: 720px;">
            <div id="ContentPlaceHolder1_PanelDrag1" style="background-color:#2D89EF;height:28px;">
                <table style="width: 100%">
                    <tbody>
                        <tr>
                            <td>
                                <span id="ContentPlaceHolder1_L_popName1" style="color: White; font-weight: bold; vertical-align: middle">組織資料選取</span>
                            </td>
                            <td style="text-align: right;">
                                <input type="image" name="btnClose" id="btnClose" src="/Img/close_button.png" onclick="CancelTrigger1.click(); return false;" style="height: 25px;" />
                            </td>
                        </tr>
                    </tbody>
                </table>            
                </div>
            <table class="popItem">
                <tbody>
                    <tr>
                        <td>
                            <table class="Item">
                                <tbody>
                                    <tr>
                                        <th colspan="4">
                                            <span class="Arrow01"></span>
                                            <span id="ContentPlaceHolder1_lblKeyword">關鍵字查詢：</span>
                                            <span id="ContentPlaceHolder1_lblTip" style="color: Red;">(查詢只顯示前100筆資料)</span>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="4" style="text-align: left">
                                            <input name="TxtQuery" type="text" id="ContentPlaceHolder1_Input_TxtQuery" style="width: 250px;"/>
                                            <input type="button" name="BtnQuery" value="查     詢" onclick="QueryOrgData(); return false;" id="ContentPlaceHolder1_popB_Query" class="IconSearch"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th colspan="4">
                                            <span class="Arrow01"></span>
                                            <span id="ContentPlaceHolder1_popLabel_OrgList" style="font-weight: bold;">組織架構</span>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td style="text-align: center">
                                            <select size="4" name="ctl00$ContentPlaceHolder1$popB_PsnList1" multiple="multiple" id="ContentPlaceHolder1_popB_PsnList1" class="ListBoxStyle" style="height: 150px; width: 300px;">
                                            </select>
                                        </td>
                                        <td colspan="2" style="text-align: center">
                                            <input type="button" name="ctl00$ContentPlaceHolder1$popB_Enter1" value="加入" onclick="DataEnterRemove('Add'); return false;" id="ContentPlaceHolder1_popB_Enter1" class="IconRight" />
                                            <br/>
                                            <br/>
                                            <input type="button" name="ctl00$ContentPlaceHolder1$popB_Remove1" value="移除" onclick="DataEnterRemove('Del'); return false;" id="ContentPlaceHolder1_popB_Remove1" class="IconLeft"/>
                                        </td>
                                        <td style="text-align: center">
                                            <select size="4" name="ctl00$ContentPlaceHolder1$popB_PsnList2" multiple="multiple" id="ContentPlaceHolder1_popB_PsnList2" class="ListBoxStyle" style="height: 150px; width: 300px;">
                                            </select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" style="text-align: center">
                                            <span id="ContentPlaceHolder1_DeleteLableText1" style="font-weight: bold;"></span>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="ContentPlaceHolder1_PanelEdit1">
                                <input type="button" name="btnOk" value="確　 定" onclick="LoadPsnDataList(); return false;" id="btnOk" class="IconOk" />
                                <input type="button" name="btnCancel" value="取     消" onclick="CancelTrigger1.click(); return false;" id="btnCancel" class="IconCancel" />
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
