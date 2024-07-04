<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="YearMonth.aspx.cs" Inherits="SahoAcs.Web._06._0642.YearMonth" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
        <%--<link href="../Css/Style.css" rel="stylesheet" type="text/css" />--%>
    <style type="text/css">
        .tableClose {
            border: 0px solid white;
            border-spacing: 0px;
        }

        .tablespacing2 {
            border: 1px solid white;
            border-spacing: 2px;
        }
    </style>

    <script type="text/javascript">
        function ShowDate(obj) {
            //alert(document.getElementById("Year").value);
            //alert(obj.innerText);
            InputYearMonth(document.getElementById("Year").value, obj.innerText.trim());
        }
    </script>
</head>
<body>
     <div id="TargetYearMonth">
        <table class="tablespacing2" style="width: 100%; height: 26px; background-color: #005EBF;">
        <tr>
            <td style="font-size:12pt;color:navy;background-color:white;text-align:center">
                年度<select id="Year" name="Year">
                    <%for (int i = 2010; i < 2100; i++)
                        { %>
                    <option value="<%=i %>"><%=i %></option>
                    <%} %>
                </select>
            </td>
            </tr>
        <tr>
            <td style="font-size:12pt;color:navy;background-color:white;border-color:navy;text-align:center">
                月份<table style="width:200px">
                    <%for (int q = 0; q < 4; q++)
                        { %>
                    <tr>
                        <%for (int m = 1; m < 4; m++)
                            { %>
                        <td class="Box" style="font-size:12pt;color:white;background-color:navy;border-spacing:2px;border-width:1px" onclick="ShowDate(this)">
                            <%=(q*3+m) %>
                        </td>
                        <%} %>
                    </tr>
                    <%} %>
                </table>
            </td>
        </tr>
    </table>
    </div>
</body>
</html>
