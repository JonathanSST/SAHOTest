<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardAuthProc.aspx.cs" Inherits="SahoAcs.Web._03._0305.CardAuthProc" %>
<%@ Import Namespace="SahoAcs.Web._03._0305" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="<%="../../.."+Pub.JqueyNowVer %>"></script>
     
    <script type="text/javascript">
        /**
        var count = 0;
        var error_list = "";
        function SetCardAuthProc() {
            $(".ResultArea").html("");
            $("#MessageArea").css("visibility", "visible");
            var all_count = $("li").length;
            //$("li").each(function (index) {                
            if (count < all_count) {                
                $.ajax({
                    type: "POST",
                    url: "CardAuthProc.aspx",
                    dataType: "json",
                    data: { card_no: $("li:eq(" + count + ")").text(), action: "Save" }
                }).success(function (data) {
                    if (data.message != "OK") {
                        $(".ResultArea").prepend(data.card_no);
                        $(".ResultArea").prepend(data.error_msg);
                        $(".ResultArea").prepend("<br/>");
                        console.log(data.card_no + "....." + data.error_msg);
                        error_list += data.card_no+",";
                        count++;
                        SetCardAuthProc();
                    } else {
                        $(".ResultArea").prepend(data.card_no + "____" + data.message);
                        $(".ResultArea").prepend("<br/>");
                        console.log($("li:eq(" + count + ")").text() + ".....Success");
                        count++;
                        SetCardAuthProc();
                    }
                }).fail(function () {
                    $(".ResultArea").prepend("Faild!!");
                    $(".ResultArea").prepend("<br/>");
                    console.log($("li:eq(" + count + ")").text() + ".....Fails");
                    count++;
                    SetCardAuthProc();
                });
            } else {
                $(".ResultArea").html(error_list);
            }
            //});
            $("#MessageArea").css("visibility", "hidden");
        }
        *///
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
 <table>
            <tr>
                <td >
                    <div style="width:400px;height:400px;overflow-y:scroll">
                        <ul>        
                       <%foreach(CardData cd in this.card_list){ %>
                       <li><%=cd.CardNo %></li>     
                        <%} %>
                       </ul>
                    </div>                    
                </td>
                <td style="width:500px;">
                    <div style="width:400px;height:400px;overflow-y:scroll"  class="ResultArea">
                    </div>
                </td>
            </tr>
        </table>          
    <input type="button" value="重整" onclick="SetCardAuthProc()"/>
        <div id="MessageArea" style="visibility:hidden;background-color:red;color:white">
            重整中
        </div>  
    </form>
</body>
</html>
