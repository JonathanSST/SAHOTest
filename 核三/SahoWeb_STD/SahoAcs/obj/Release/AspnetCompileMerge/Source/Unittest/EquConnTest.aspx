<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquConnTest.aspx.cs" Inherits="SahoAcs.Unittest.EquConnTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="<%=".."+Pub.JqueyNowVer %>"></script>
    <script>
        var conn_count = 0;
        function setEquCheck() {
            var conn_message = "";
            $.ajax({
                type: "POST",
                url: "EquConnTest.aspx",
                dataType: "json",
                data: { "QueryConn": "Send" },
                success: function (data) {
                    $.each(data.equ_lists, function (i, item) {
                        if (conn_message !== "") {
                            conn_message += "<br/>"
                        }
                        conn_message += "「" + item.EquNo + "」";
                        conn_message += "目前的的連線狀態為";
                        conn_message += item.EquDesc;
                    });
                    $("#MessageDiv").html(conn_message);
                },
                fail: function () {

                }
            });
        }

        function getEquCheck() {
            if (conn_count > 10) {
                return false;
            }
            var conn_message = "";
            $.ajax({
                type: "POST",
                url: "EquConnTest.aspx",
                dataType: "json",
                data: { "QueryConn": "Get" },
                success: function (data) {
                    console.log(data);
                    $.each(data.equ_lists, function (i, item) {
                        if (conn_message !== "") {
                            conn_message += "<br/>"
                        }
                        conn_message += "「" + item.EquNo + "」";
                        conn_message += "目前的的連線狀態為";
                        conn_message += item.EquDesc;
                    });
                    if (data.equ_lists[0].EquDesc === "") {
                        setEquCheck();
                    }
                    $("#MessageDiv").html(conn_message);
                    conn_count += 1;
                },
                fail: function () {

                }
            });
        }
        setInterval("getEquCheck()", 3000);
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <input type="button" value="檢查設備連線" onclick="setEquCheck()" />
        <input type="button" value="取得設備狀態" onclick="getEquCheck()" />
    </div>
        <div id="MessageDiv">

        </div>
    </form>
</body>
</html>
