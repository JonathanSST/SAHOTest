<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryPerson.aspx.cs" Inherits="SahoAcs.Web._06._0620.QueryPerson" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="MasterDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid">
            <table class="Item" style="background-color: #069">
                <tr>
                    <td>
                        <fieldset id="Elevator_List" name="Elevator_List" runat="server" style="width:880px;height:430px">
                            <legend id="Elevator_Legend" runat="server" style="font-size: 15px">人員識別車證清單</legend>
                            <table class="TableS3" border="0" style="width:900px">
                                <tbody>
                                    <tr>
                                        <th style="width: 100px;" scope="col">人員</th>
                                        <th style="width: 100px;" scope="col">人員編號</th>
                                        <th style="width: 100px;" scope="col">卡號</th>
                                        <th style="width: 100px;" scope="col">汽車一</th>
                                        <th style="width: 100px;" scope="col">汽車二</th>
                                        <th style="width: 100px;" scope="col">機車一</th>
                                        <th style="width: 100px;" scope="col">機車二</th>
                                        <th scope="col">證號</th>
                                    </tr>
                                    <tr>
                                        <td style="padding: 0px;" colspan="8">
                                            <div id="Elevator_Panel" style="height:300px;overflow-y: scroll;">
                                                <div>
                                                    <table id="ParaGridView" style="width:880px;border-width: 0px; border-collapse: collapse;" rules="all" cellspacing="0">
                                                        <tbody>
                                                            <%foreach(var o in this.PersonCarList){ %>
                                                            <tr style="background-color:#069; color:#fff" class="ResultRow">
                                                                <td style="width: 102px;"><%=o.PsnName %></td>
                                                                <td style="width: 102px;"><%=o.PsnNo %></td>
                                                                <td style="width: 102px;"><%=o.CardNo %></td>
                                                                <td style="width: 102px;"><%=o.CarA1 %></td>
                                                                <td style="width: 102px;"><%=o.CarA2 %></td>
                                                                <td style="width: 102px;"><%=o.CarB1 %></td>
                                                                <td style="width: 102px;"><%=o.CarB2 %></td>
                                                                <td><%=o.CertNo %></td>
                                                            </tr>                                                            
                                                            <%} %>
                                                            <%if(this.PersonCarList.Count==0){ %>
                                                            <tr style="background-color:#069; color:#fff">
                                                                <td colspan="8" style="text-align:center">查無資料</td>
                                                                </tr>
                                                            <%} %>
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
        </div>
    </form>
</body>
</html>
