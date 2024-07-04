<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeviceList.aspx.cs" Inherits="SahoAcs.Web._98._9820.DeviceList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>
                        <fieldset id="DeviceField" runat="server" style="width: 285px;">
                            <legend id="DeviceList_Legend" runat="server">攝影機清單設定</legend>
                            <table class="TableS3" style="width: 410px">
                                <tr>
                                    <th style="width: 39px;" scope="col">編號</th>
                                    <th style="width: 230px;" scope="col">攝影機</th>
                                    <th scope="col">刪除</th>
                                </tr>
                                <tr>
                                    <td style="padding: 0px;" colspan="3">
                                        <div>
                                            <div id="Elevator_Panel" style="height: 200px; overflow-y: scroll;">
                                                <table id="DeviceGridView" style="border-width: 1px; border-collapse: collapse;" rules="all" cellspacing="0">
                                                    <tbody>
                                                        <% int rowcnt = 1;%>
                                                        <%foreach (var o in Request["EvoName"].Split(','))
                                                            { %>
                                                        <tr>
                                                            <td style="text-align: center; width: 41px;"><%=rowcnt %></td>
                                                            <td style="width: 232px;">
                                                                <%=o %>
                                                                <input type="hidden" value="<%=o %>" name="DeviceNo" />
                                                            </td>
                                                            <td style="text-align: center">
                                                                <input type="button" value="刪除" class="IconDelete" onclick="Remove(this)" />
                                                                <input type="hidden" value="<%=o %>" id="RemoveID" />
                                                            </td>
                                                        </tr>
                                                        <%
                                                                rowcnt++;
                                                            } %>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" class="Item">
                            <tr>
                                <th>
                                    <span>攝影機定義：</span>                                    
                                    <select class="DropDownListStyle" id="SelectEvo" name="SelectEvo">
                                        <%foreach (var option in this.AllResourceList)
                                            { %>
                                            <option value="<%=option %>"><%=option %></option>
                                        <%} %>
                                    </select>
                                    <input type="button" value="執行新增" class="IconRight" onclick="DoAdd()" />
                                    <input type="button" value="更新攝影機清單" class="IconRight" onclick="DoRefresh()" />
                                </th>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input type="button" value="儲存" class="IconSave" onclick="DoSave()" />
                        <input type="button" id="BtnAlive" value="離開" class="IconCancel"/>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
