<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowMap2.aspx.cs" Inherits="SahoAcs.Web._06._0625.ShowMap2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="box">
            <%if (this.logs.Count > 0)
                {
                     %>
            <img src="MapStory2.ashx?CardNo=<%=Request["CardNo"]%>&ID1=<%=this.DefaultRec1 %>&ID2=<%=this.DefaultRec2 %>&DateTime=<%=DateTime.Now.ToString() %>" 
                id="company" style="width:800px;height:600px" />
            <% }else{ %>
                 <img src="MapStory.ashx?CardNo=<%="00002531" %>&DateS=<%="2017/03/10 00:00:00" %>&DateE=<%="2017/03/10 23:59:59" %>&DateTime=<%=DateTime.Now.ToString() %>" 
                id="companyDefault" style="width:800px;height:600px" />
            <%} %>
        </div>
        <%--進行上下筆記錄的巡覽--%>
        <%if (this.DefaultRec0 == this.DefaultRec1)
            { %>
        <input type="button" class="IconLeft" value="上一筆" id="BtnPrev" disabled="disabled"/>
        <%}
    else
    { %>
         <input type="button" class="IconLeft" value="上一筆" id="BtnPrev"/>
        <%} %>
         <%if (this.DefaultRec2 == this.DefaultRec1)
            { %>
        <input type="button" class="IconRight" value="下一筆" id="BtnNext" disabled="disabled"/>
        <%}
    else
    { %>
         <input type="button" class="IconRight" value="下一筆" id="BtnNext"/>
        <%} %>        
        <input type="hidden" value="<%=this.DefaultRec0 %>" id="HiddenPrev" />
        <input type="hidden" value="<%=this.DefaultRec2 %>" id="HiddenNext" />
        <input type="button" class="IconLeave" value="關閉" id="BtnClose" />
        <input type="hidden" id="MapCardNo" name="MapCardNo" value="<%=Request["CardNo"] %>" />
        <span style="color:white;font-weight:700">設備：<%=this.EquName %></span>&nbsp;&nbsp;<span style="color:white;font-weight:700">讀卡時間：<%=this.CardTime %></span>
        <p style="height:3px">&nbsp;</p>
    </form>
</body>
</html>
