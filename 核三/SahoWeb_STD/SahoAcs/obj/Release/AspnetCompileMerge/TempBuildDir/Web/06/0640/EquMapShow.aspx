<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquMapShow.aspx.cs" Inherits="SahoAcs.Web._06._0640.EquMapShow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .drag {
            width: 55px;
            height: 55px;
            border: 1px solid;
            cursor: pointer;
            border-radius: 10px;
            text-align: center;
            /*background-color: white;*/
            /*opacity:0.4;*/
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="box" style="width:1024px;height:768px">
            <img src="Map.ashx?Pic=<%=Request["PicID"] %>&now=<%=DateTime.Now.ToString() %>" id="company" />
        </div>
        <input type="hidden" value="<%=Request["PicID"] %>" name="PicID" id="PicID" />
    </form>
   <input type="button" id="BtnMain" value="返回主頁" class="IconLeave" />
</body>
</html>
