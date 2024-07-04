<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquDataInsert2.aspx.cs" Inherits="SahoAcs.Web._02._0201._020102.EquDataInsert2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form_insert" runat="server">
    <div id="MainDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;">
        <table class="popItem">
            <tbody>
                <tr>
                    <td>
                        <table>
                            <tbody>
                                <tr>
                                    <th>
                                        <span id="Label1" style="font-weight:bold;">設備類型</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <select name="EquClass" id="EquClass" class="DropDownListStyle" onchange="ChangeEquType()" style="background-color:#FFE5E5;width:253px;">
                                            <option value="Door Access">門禁設備</option>
                                            <option value="Elevator">電梯設備</option>
                                            <option value="TRT">考勤設備</option>
                                        </select>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tbody>
                                <tr>
                                    <th>
                                        <span id="popLabel_Building" style="font-weight:bold;">建築物名稱</span>
                                    </th>
                                    <th>
                                        <span id="popLabel_Floor" style="font-weight:bold;">樓層</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <input name="Building" type="text" id="Building" class="TextBoxRequired" style="width:180px;"/>
                                    </td>
                                    <td>
                                        <input name="Floor" type="text" id="Floor" class="TextBoxRequired" style="width:130px;"/>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tbody>
                                <tr>
                                    <th>
                                        <span id="popLabel_EquModel" style="font-weight:bold;">設備型號</span>
                                    </th>
                                    <th>
                                        <span id="popLabel_EquNo" style="font-weight:bold;">設備編號</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <input name="EquModel" type="text" id="EquModel" readonly="true" class="TextBoxRequired" style="width:100px;" value=""/>
                                    </td>
                                    <td>
                                        <input name="EquNo" type="text" id="EquNo" class="TextBoxRequired" style="width:200px;"/>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tbody>
                                <tr>
                                    <th>
                                        <span id="popLabel_EquName" style="font-weight:bold;">設備名稱</span>
                                    </th>
                                    <th>
                                        <span id="popLabel_EquEName" style="font-weight:bold;">設備英文名稱</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <input name="EquName" type="text" id="EquName" class="TextBoxRequired" style="width:180px;" value="<%=Request["EquName"] %>"/>
                                    </td>
                                    <td>
                                        <input name="EquEName" type="text" id="EquEName" style="width:155px;"/>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tbody>
                                <tr>
                                    <th>
                                        <span id="popLabel_Dci" style="font-weight:bold;">設備連線</span>
                                    </th>
                                    <th>
                                        <span id="popLabel_CardNoLen" style="font-weight:bold;">卡號長度</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <select name="Dci" id="Dci" class="DropDownListStyle" style="background-color:#FFE5E5;width:253px;">
                                            <option value="1">DCI001 (DCI001)</option>
                                        </select>
                                    </td>
                                    <td>
                                        <input name="CardNoLen" type="text" value="10" readonly="readonly" id="CardNoLen" class="TextBoxRequired" style="width:60px;"/>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="ShowInTrt" style="display: none;">
                            <table>
                                <tbody>
                                    <tr>
                                        <th>
                                            顯示姓名
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input id="popIsShowName" type="checkbox" name="popIsShowName"/>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="ShowInDoor" style="display: block;">
                            <table>
                                <tbody>
                                    <tr>
                                        <th>
                                            進入管制區
                                        </th>
                                        <th>
                                            離開管制區
                                        </th>
                                        <th>
                                            卡鐘模式
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <select name="InToCtrlAreaID" id="InToCtrlAreaID" class="DropDownListStyle" style="background-color:#FFE5E5;width:130px;">                                                
                                            </select>
                                        </td>
                                        <td>
                                            <select name="OutToCtrlAreaID" id="OutToCtrlAreaID" class="DropDownListStyle" style="background-color:#FFE5E5;width:130px;">                                                
                                            </select>
                                        </td>
                                        <td>
                                            <select name="Input_Trt" id="Input_Trt" class="DropDownListStyle" style="background-color:#FFE5E5;width:90px;">
                                                <option value="1">是</option>
                                                <option selected="selected" value="0">否</option>
                                            </select>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th style="text-align: center">
                        <span id="DeleteLableText" style="font-weight:bold;"></span>
                    </th>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <input type="submit" value="儲存" class="IconSave" onclick="SaveEquData(); return false;"/>
                        <input type="submit" value="取消" class="IconCancel" onclick="Cancel(); return false;"/>
                    </td>
                </tr>
            </tbody>
        </table>
        <input type="hidden" id="DoAction" name="DoAction" value="insert"/>
    </div>    
    </form>
</body>
</html>
