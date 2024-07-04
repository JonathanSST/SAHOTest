<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddMasterController.aspx.cs" Inherits="SahoAcs.Unittest.AddMasterController" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="<%=".."+Pub.JqueyNowVer %>"></script>
    <script type="text/javascript">

        function SetAddCtrl(obj) {
            var data = JsFunFindParent(obj, "TR");
            console.log($(data).find("td:eq(0)").html());
            console.log($(data).find("td:eq(1)").html());
            console.log($(data).find("td:eq(2)").html());
            console.log($(data).find("td:eq(3)").html());
            console.log($(data).find("td:eq(4)").html());
            console.log($(data).find("td:eq(5)").html());

            $.ajax({
                type: "POST",
                url: "AddMasterController.aspx",
                dataType:"json",
                data: { "DciID": $(data).find("td:eq(6)").html(), "CtrlModel": $(data).find("td:eq(5)").html(), "MstID": $(data).find("td:eq(4)").html(), "CtrlNo": $(data).find("td:eq(3)").html(), "CtrlName": $(data).find("td:eq(2)").html(), "PageEvent": "InsertController" },
                success: function (data) {
                    //console.log(data);      
                    console.log(data);
                    if (data.message == "OK") {
                        window.location = window.location;
                    }
                },
                fail: function () {
                    console.log("Error....");
                }
            }).done(function () {

            });
        }

        function SetDelCtrl(obj)
        {
            var data = JsFunFindParent(obj, "TR");
            console.log($(data).find("td:eq(0)").html());
            console.log($(data).find("td:eq(1)").html());
            console.log($(data).find("td:eq(2)").html());
            console.log($(data).find("td:eq(3)").html());
            console.log($(data).find("td:eq(4)").html());
            console.log($(data).find("td:eq(5)").html());

            $.ajax({
                type: "POST",
                url: "AddMasterController.aspx",
                dataType: "json",
                data: { "CtrlID": $(data).find("td:eq(6)").html(), PageEvent: "Del" },
                success: function (data) {
                    console.log(data);
                    if (data.message == "OK") {
                        window.location = window.location;
                    }
                },
                fail: function () {
                    console.log("Error....");
                }
            }).done(function () {

            });
        }


        function JsFunFindParent(I_obj, I_TagName) {
            var oSRC = null;

            if (I_obj == undefined || I_obj == null)
                return;

            oSRC = (I_obj.length) ? I_obj[0] : I_obj;

            while (true) {
                if (oSRC["tagName"] == I_TagName)
                    return oSRC

                if (oSRC.parentNode)
                    oSRC = oSRC.parentNode;
                else
                    break;
            }
            return null;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <%foreach (var o in this.MasterData)
               { %>
            <tr>
                <td><%=o.MstNo %></td>
                <td><%=o.MstDesc %></td>
                <td><%=Convert.ToString(o.MstNo).Replace("M","") %></td>
                <td><%=Convert.ToString(o.MstNo).Replace("M","C") %></td>
                <td><%=Convert.ToInt32(o.MstID) %></td>
                <td><%=Convert.ToString(o.CtrlModel) %></td>
                <td><%=Convert.ToInt32(o.DciID) %></td>
                <td>
                    <input type="button" value="加入預設控制器" onclick="SetAddCtrl(this)" />
                </td>
            </tr>
        <%} %>
            <%foreach (var o in this.MasterData2)
               { %>
            <tr>
                <td><%=o.MstNo %></td>
                <td><%=o.MstDesc %></td>
                <td><%=Convert.ToString(o.CtrlName) %></td>
                <td><%=Convert.ToString(o.CtrlNo) %></td>
                <td><%=Convert.ToInt32(o.MstID) %></td>
                <td><%=Convert.ToString(o.EquName) %></td>
                <td><%=Convert.ToInt32(o.CtrlID) %></td>
                <td>
                    <input type="button" value="刪除" onclick="SetDelCtrl(this)" />
                </td>
            </tr>
        <%} %>
        </table>
           
    </div>
    </form>
</body>
</html>
