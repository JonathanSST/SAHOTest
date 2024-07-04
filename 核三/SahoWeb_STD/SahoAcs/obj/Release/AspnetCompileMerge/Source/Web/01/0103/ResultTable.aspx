<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResultTable.aspx.cs" Inherits="SahoAcs.Web._01._0103.ResultTable" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Linq" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <%
        int showRange = 5; //顯示快捷頁數
        int pageCount = this.PersonPage.PageCount;
        int pageIndex = this.PersonPage.PageNumber;
        int startIndex = (pageIndex  < showRange) ?
                       1 : (pageIndex + showRange / 2 >= pageCount) ? pageCount - (showRange-1) : pageIndex - showRange / 2;
        int endIndex = (startIndex > pageCount - (showRange-1)) ? pageCount : startIndex + showRange;
         %>
    <table class="TableS1" style="width:480px">
        <tbody>
            <tr class="GVStyle">
                <th scope="col" style="width: 150px;">員工編號</th>
                <th scope="col">人員姓名</th>
            </tr>
            <tr>
                <td id="ContentPlaceHolder1_td_showGridView" colspan="2">
                    <div id="tablePanel" style="height: 450px; overflow: auto;">
                        <div>
                            <table class="GVStyle" cellspacing="0" rules="all" border="1" id="MainGridView" style="border-collapse: collapse;">
                                <tbody>
                                    <%foreach(DataRow r in this.DataResult.Rows){ %>
                                    <tr onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">
                                        <td title="<%=r["PsnNo"].ToString() %>" style="width: 154px;"><%=r["PsnNo"].ToString() %></td>
                                        <td title="<%=r["PsnName"].ToString() %>"><%=r["PsnName"].ToString() %>
                                            <input type="hidden" name="RowSelectID" id="RowSelectID" value="<%=r["PsnID"].ToString() %>" />
                                        </td>
                                    </tr>                                    
                                    <%} %>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </td>
            </tr>
            <tr class="GVStylePgr">
                <td colspan="2" style="text-align: center">
                    <a href="#" style="text-decoration: none;" class="PageFirst">第一頁</a>
                    <%if (pageIndex > 1)
                        { %>
                        <a href="#" style="text-decoration: none;" class="PagePrev">上一頁</a>
                    <%} %>                    
                    <% for (int i = startIndex; i <= endIndex; i++)
                        { %>
                    <%if (i == pageIndex)
                        { %>
                            <a onclick="return false;" style="color: White; font-weight: bold; text-decoration: none;" class="NowPage"><%=i %></a>
                    <%}else{%>
                            <a href="#" style="text-decoration: none;" class="PageLink"><%=i %></a>
                    <%} %>                    
                    <%} %>
                    <%if (pageIndex < pageCount)
                        { %>
                    <a href="#" style="text-decoration: none;" class="PageNext">下一頁</a>
                    <%} %>
                    <a href="#" style="text-decoration: none;" class="PageLast">最末頁</a>
                    <br />
                    <br />
                    <%=string.Format("{0}/{1}",this.PersonPage.PageNumber,this.PersonPage.PageCount) %>&nbsp;&nbsp;&nbsp;&nbsp;總共 <%=this.PersonPage.TotalItemCount %> 筆
                    <input type="hidden" id="PageCount" value="<%=this.PersonPage.PageCount %>" />
                </td>
            </tr>
        </tbody>
    </table>
</body>
</html>
