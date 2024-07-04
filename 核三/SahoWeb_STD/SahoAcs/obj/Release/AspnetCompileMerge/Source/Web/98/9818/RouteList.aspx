<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RouteList.aspx.cs" Inherits="SahoAcs.Web.RouteList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="MapForm" runat="server">
        <div>
            <table>
                <tr>
                    <td style="height: 250px; width: 600px; vertical-align: top" id="EquArea">
                        <table class="TableS1">
                            <tbody>
                                <tr class="GVStyle">
                                    <th scope="col" style="width: 230px">圖幅名稱</th>
                                    <th scope="col">圖幅路徑</th>
                                </tr>
                                <tr>
                                    <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="2">
                                        <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 600px; overflow-y: scroll;">
                                            <div id="DataResult">
                                                <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                                    id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                                    <tbody>
                                                        <%foreach (var s in this.RouteListData)
                                                            { %>
                                                        <tr>
                                                            <td style="width: 234px; text-align: left; background-color:navy; color: white"><%=this.mapobj.PicDesc %></td>
                                                            <td style="text-align: left; background-color: navy; color: white">
                                                                <a style="color:white" href="#" onclick="ShowLine('<%=s.PicID %>','<%=s.EquStart %>','<%=s.EquEnd %>')"><%=s.LineRoute %></a>
                                                            </td>
                                                        </tr>
                                                        <%} %>
                                                        <%foreach (var s in this.UnRouteListData)
                                                            { %>
                                                        <tr>
                                                            <td style="width: 234px; text-align: left"><%=this.mapobj.PicDesc %></td>
                                                            <td style="text-align: left">
                                                                 <a style="color:navy" href="#" onclick="ShowLine('<%=s.PicID %>','<%=s.EquStart %>','<%=s.EquEnd %>')"><%=s.LineRoute %></a>
                                                            </td>
                                                        </tr>
                                                        <%} %>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center" colspan="2">
                                        <input type="button" id="btnExit" value="<%=this.GetGlobalResourceObject("Resource","btnExit") %>" class="IconLeave" onclick="DoCancel()" />
                                        <input type="hidden" value="<%=this.mapobj.PicID %>" name="PicID" id="PicID" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
