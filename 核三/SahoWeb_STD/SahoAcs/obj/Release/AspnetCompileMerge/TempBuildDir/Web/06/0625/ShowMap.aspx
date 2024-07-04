<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowMap.aspx.cs" Inherits="SahoAcs.Web._06._0625.ShowMap" %>

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
        <input type="button" class="IconLeave" value="關閉" id="BtnClose" />
        <input type="hidden" id="MapCardNo" name="MapCardNo" value="<%=Request["CardNo"] %>" />
        <p style="height:3px">&nbsp;</p>
        
        <table class="TableWidth">
            <tr>
                <td>
                    <fieldset id="Holiday_Fieldset" runat="server" style="width: 870px; height: 220px">
                        <legend id="Holiday_Legend" runat="server">讀卡記錄清單</legend>
                        <asp:Panel ID="CardLogPanel" runat="server" ScrollBars="Vertical" Height="200px" CssClass="TableS2">
                            <asp:Table ID="CardLogTable" runat="server"></asp:Table>
                        </asp:Panel>
                    </fieldset>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
