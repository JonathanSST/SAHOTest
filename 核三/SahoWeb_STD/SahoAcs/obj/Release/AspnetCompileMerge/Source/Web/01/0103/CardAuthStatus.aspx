<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardAuthStatus.aspx.cs" Inherits="SahoAcs.Web._01._0103.CardAuthStatus" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="MainDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid; height: 480px; width: 1000px">
            <div>
                <table class="popItem">
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <%=this.CardNo %>
                            <asp:Label ID="Label2" runat="server" Text="卡片設消碼狀態表" Font-Bold="True"></asp:Label>
                        </th>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td style="background-color: navy; color: white">有權限
                                    </td>
                                    <td style="background-color: yellow; color: black">設消碼處理中
                                    </td>
                                    <td style="background-color: red; color: white">處理異常
                                    </td>
                                    <td style="background-color: white; color: black">無權限
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 335px; width: 920px; vertical-align: top">
                            <fieldset id="Fieldset3" runat="server" style="width: 660px; height: 355px">
                                <legend id="Legend3" runat="server">
                                    <asp:Label ID="Label4" runat="server" Text="設備清單"></asp:Label>
                                </legend>
                                <table class="TableS1">
                                    <tbody>
                                        <tr class="GVStyle">
                                            <th scope="col" style="width: 20px;">項次</th>
                                            <th scope="col" style="width: 100px;">設備編號</th>
                                            <th scope="col" style="width: 220px">設備名稱</th>
                                            <th scope="col" style="width: 80px">設消碼狀態</th>
                                            <th scope="col">權限起迄時間</th>
                                        </tr>
                                        <tr>
                                            <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="5">
                                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 290px; width: 920px; overflow-y: scroll;">
                                                    <div>
                                                        <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                                            id="TableCode" style="border-collapse: collapse;">
                                                            <tbody>
                                                                <%foreach (var o in this.CardAuthList)
                                                                    { %>
                                                                <tr>
                                                                    <td style="width: 24px">
                                                                        <%=this.CardAuthList.IndexOf(o)+1 %>
                                                                    </td>
                                                                    <td style="width: 104px">
                                                                        <%=o.EquNo %>
                                                                    </td>
                                                                    <td style="width: 224px">
                                                                        <%= o.EquName%>
                                                                    </td>
                                                                    <td style="width: 84px">
                                                                        <%=o.OpStatus %>
                                                                    </td>
                                                                    <td>
                                                                        <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}~{1:yyyy/MM/dd HH:mm:ss}",o.BeginTime,o.EndTime) %>
                                                                        <input type="hidden" id="OpStatus" value="<%=o.EquNoList %>" name="OpStatus" />
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
                    <tr>
                        <td>
                            <input type="button" value="取消" class="IconCancel" onclick="CloseStatus()" />
                        </td>
                    </tr>
                </table>
                <input type="hidden" name="CardID" id="CardID" value="<%=Request["CardID"] %>" />
            </div>
        </div>
    </form>
</body>
</html>
