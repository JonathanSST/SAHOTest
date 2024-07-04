<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkDetail.aspx.cs" Inherits="SahoAcs.Web._07._0706.WorkDetail" %>

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
                        <fieldset id="Elevator_List" name="Elevator_List" runat="server" style="width:600px; height:300px">
                            <asp:Label ID="lblDepartment" runat="server"></asp:Label>
                            <table class="TableS3" border="0" style="width:600px">
                                <tbody>
                                    <tr>
                                        <th style="width: 50px;" scope="col">人員</th>
                                        <th style="width: 50px;" scope="col">假別</th>
                                        <th style="width: 350px;" scope="col">請假起迄時間</th>
                                        <th style="width: 40px;" scope="col">天數</th>
                                        <th style="width: 40px;" scope="col">時數</th>
                                    </tr>
                                    <tr>
                                        <td style="padding: 0px;" colspan="5">
                                            <div id="Elevator_Panel" style="height:200px;overflow-y: scroll;">
                                                <div>
                                                    <table id="ParaGridView" style="width:600px;border-width: 0px; border-collapse: collapse;" rules="all" cellspacing="0">
                                                        <tbody>
                                                            <%foreach(var o in this.DataList){ %>
                                                            <tr style="background-color:#069; color:#fff" class="ResultRow">
                                                                <td style="width:54px;"><%=o.PsnName %></td>
                                                                <td style="width:54px;"><%=o.HoliNo %></td>
                                                                <td style="width:354px;">
                                                                    <%=o.StartTime.ToString("yyyy/MM/dd HH:mm:ss") %>
                                                                    ~
                                                                    <%=o.EndTime.ToString("yyyy/MM/dd HH:mm:ss") %>
                                                                </td>
                                                                <td style="width:44px;"><%=o.Daily %></td>
                                                                <td><%=o.Hours %></td>
                                                            </tr>                                                            
                                                            <%} %>
                                                            <%if(this.DataList.Count==0){ %>
                                                            <tr style="background-color:#069; color:#fff">
                                                                <td colspan="5" style="text-align:center">查無資料</td>
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
                            <input type="button" id="BtnClose" value="關閉" class="IconLeave" />
                        </fieldset>
                    </td>
                </tr>                
            </table>
        </div>
    </form>
</body>
</html>
