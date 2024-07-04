<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InAreaList.aspx.cs" Inherits="SahoAcs.InAreaList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="Css/bootstrap.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <div class="row">
                <div class="col-lg-12">
                    <table cellspacing="0" rules="all" border="1" class="table table-striped table-bordered table-hover" style="width: 1050px; border-collapse: collapse;">
                        <tr class="GVStyle">
                            <th scope="col" style="width: 159px">公司</th>
                            <th scope="col" style="width: 110px;">工號</th>
                            <th scope="col" style="width: 100px;">姓名</th>
                            <th scope="col" style="width: 150px;">卡號</th>
                            <th scope="col" style="width: 120px;">設備編號</th>
                            <th scope="col" style="width: 120px">設備名稱</th>
                            <th scope="col" style="">讀卡結果</th>
                        </tr>
                        <tr>
                            <td id="showGridView" colspan="8">
                                <div id="tablePanel" style="width: 1050px; height: 500px; overflow-y: scroll;">
                                    <table cellspacing="0" rules="all" border="1" id="TableDetail" style="width: 100%; border-collapse: collapse;" class="table table-striped table-bordered table-hover">
                                        <tbody>
                                            <%foreach (var o in this.AllStayList)
                                                { %>
                                            <tr>
                                                <td scope="col" style="width: 153px"><%=o.DepName %></td>
                                                <td scope="col" style="width: 110px;"><%=o.PsnNo %></td>
                                                <td scope="col" style="width: 100px;"><%=o.PsnName %></td>
                                                <td scope="col" style="width: 150px;"><%=o.CardNo %></td>
                                                <td scope="col" style="width: 120px;"><%=o.EquNo %></td>
                                                <td scope="col" style="width:120px"><%=o.EquName %></td>
                                                <td scope="col" style=""><%=o.LogStatus %></td>
                                            </tr>
                                            <%} %>
                                        </tbody>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <%--<h1 style="color:brown">目前在廠人數：<%=InAreaCounts %> 人</h1>--%>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
