<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquMapSet2.aspx.cs" Inherits="SahoAcs.Web._98._9825.EquMapSet2" %>

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
        <div id="dragx" class="drag" style="border-width:1px;border-color:orangered;top:-700px;left:10px">
            <p style="position:absolute;top:40%;left:40%">
                    <img src="../../../Img/Maps/Go.png" style="margin-bottom:27px;width:15px;height:15px; position:relative;" />
                </p>           
            <br /><br /><br /><br />
            <div>
               <span style="background-color:blue;color:#fff; display:none; "><%=this.PicName %></span>
                設備
                <select id="EquNo">
                    <%foreach (var o in this.datalist)
                        { %>
                            <%--<option value="<%=o.EquID %>"><%=string.Format("{1}_({0})",o.EquNo,o.EquName) %></option>--%>
                            <option value="<%=o.EquID %>"><%=string.Format("{0}",o.EquNo) %></option>
                        <%} 
                    %>
                </select>
                <input type="button" id="BtnSaveAndRefresh" value="顯示座標點" class="IconMultisearch"  style="display:none;"/>
                <select id="MapRotate" style="display:none;">
                    <%for(int i=-359;i<360;i++) { %>
                    
                    <%if (this.rotate == i)
                        {%>
                                        <option selected="selected" value="<%=i %>"><%=string.Format("地圖方位角__ {0} 度 ", i) %></option>
                        <%}
                        else
                        { %>
                        <option value="<%=i %>"><%=string.Format("地圖方位角__ {0} 度 ", i) %></option>
                    <%} %>

                    <%} %>
                </select>
                <input type="button" id="BtnSave" value="新增設備位置" class="IconSave" />
                <input type="button" id="BtnDelete" value="刪除設備位置" class="IconDelete" />
                <input type="button" id="BtnMain" value="返回主頁" class="IconLeave" />
            </div>
        </div>
    </div>
    <%--<input type="button" id="BtnShow" value="Point" />    --%>
    <input type="hidden" value="<%=Request["PicID"] %>" name="PicID" id="PicID" />
    </form>
</body>
</html>
