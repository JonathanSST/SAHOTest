<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnitTest.aspx.cs" Inherits="SahoAcs.Web._06._0608.UnitTest" %>

<%@ Import Namespace="SahoAcs.Web._06._0608" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="<%="../../.."+Pub.JqueyNowVer %>"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#Query").click(function () {
                $.post("UnitTest.aspx", {
                    "PageEvent": "Query",
                    "PageNumber": $("#PageNumber").val()
                }, function (data) {
                    //console.log($(data).find("#DataResultContent").html());
                    $("#DataResultContent").html($(data).find("#DataResultContent").html());
                });
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="DataResultContent">
            <div>
                <%foreach (var o in this.person_equ_list)
                  { %>
                <div><%=string.Format("{0}........{1}.....{2}",o.EquClass,o.EquName,o.PsnNo) %></div>
                <%} %>
            </div>                    
        </div>
        <select id="PageNumber" name="PageNumber">
                    <%for (int i = 1; i <= this.person_equ_list.PageCount; i++)
                      { %>
                    <option value="<%=i %>">第<%=i %>頁</option>
                    <%} %>
                </select>
        <input type="button" value="Change" id="Query" />
    </form>
</body>
</html>
