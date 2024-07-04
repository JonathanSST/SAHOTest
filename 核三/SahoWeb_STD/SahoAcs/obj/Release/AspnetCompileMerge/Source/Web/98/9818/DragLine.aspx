<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DragLine.aspx.cs" Inherits="SahoAcs.Web._98._9818.DragLine" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
        
    <!-- CSS -->    
    <style type="text/css">
       
         .drag {
            width: 55px;
            height: 55px;
            border: 1px solid black;
            cursor: pointer;
            border-radius: 10px;
            text-align: center;
            /*background-color: white;*/
            /*opacity:0.4;*/
        }

        .drag1 {
            width: 270px;            
            border: 1px solid black;
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
    <div id="box" style="background-color: lightgreen; width: 1024px; height: 768px">
            <img src="Map.ashx?Pic=<%=Request["PicID"] %>" id="company" />
            <input type="hidden" value="<%=Request["PicID"] %>" name="PicID" id="PicID" />
            <div id="dragx" class="drag" style="border-width: 1px; border-color: black;top:-700px;vertical-align:middle;text-align:center">
                <p style="position:absolute;top:35%;left:35%">
                    <img src="../../../Img/Maps/Go.png" style="margin-bottom:27px;width:15px;height:15px; position:relative;" />
                </p>                
            </div>
             <div id="toolPanel" class="drag1" style="background-color:white;top:-600px">                
                <span style="background-color:blue;color:#fff"><%=this.PicName %></span><br />
                <span style="width: 200px">起始位置</span><br />
                <select id="EquNoS">
                    <%foreach (var o in this.datalist)
                        { %>
                    <option value="<%=o.EquID %>"><%=string.Format("{1}_({0})",o.EquNo,o.EquName) %></option>
                    <%} %>
                </select><br />
                <span style="width: 200px">結束位置</span><br />
                <select id="EquNoE">
                    <%foreach (var o in this.datalist)
                        { %>
                    <option value="<%=o.EquID %>"><%=string.Format("{1}_({0})",o.EquNo,o.EquName) %></option>
                    <%} %>
                </select>
                <%foreach (var o in this.datalist)
                    { %>
                <input type="hidden" value="<%=o.PointX %>" id="<%=o.EquID %>X" />
                <input type="hidden" value="<%=o.PointY %>" id="<%=o.EquID %>Y" />
                <%} %>
                 <input type="button" id="BtnStoryS" value="開始記錄" class="IconNew" />
                <input type="button" id="BtnStoryE" value="停止記錄" disabled="disabled" class="IconCancel" />
                <input type="button" id="BtnSave" value="儲存記錄" class="IconSave" />
                <input type="button" id="BtnShowLine" value="顯示路徑" class="IconMultisearch" />
                 <input type="button" id="BtnShowMap" value="顯示所有設備" class="IconMultisearch" />
                <input type="button" id="BtnMain" value="返迴主頁" class="IconLeave" />
            </div>
    </div>
    </form>
</body>
</html>
