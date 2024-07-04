<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PsnCardList.aspx.cs" Inherits="SahoAcs.Web._04._0413.PsnCardList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
        <input type="button" value="關閉人員清單" class="IconCancel" onclick="javascript: $('#PsnListContent').html('');"/>    
        <table class="TableS1" style="width:500px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" class="TitleRow" style="width: 130px">人員編號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 130px">姓名
                    </th>
                    <th scope="col" class="TitleRow">卡號
                    </th>
                </tr>
                 <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="11">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 500px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%foreach(var o in this.ListPerson){ %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);"
                                            onclick="InputPerson('<%=o.PsnNo %>','<%=o.PsnName %>','<%=o.CardID %>')">
                                            <td style="width:134px">
                                                <%=o.PsnNo %>
                                          </td>
                                            <td style="width:134px">
                                                <%=o.PsnName %>
                                          </td>
                                            <td>
                                                <%=o.CardNo %>
                                            </td>
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
</body>
</html>
