<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowAlarmMap.aspx.cs" Inherits="SahoAcs.Web._06._0633.ShowAlarmMap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
         <div id="box">
              <img src="MapAlarm.ashx?CardNo=<%=this.logdata.CardNo%>&RecordID=<%=this.logdata.RecordID %>&DateTime=<%=DateTime.Now.ToString() %>" 
                id="company" style="width:800px;height:600px" />
        </div>
        <input type="button" class="IconLeave" value="關閉" id="BtnClose" />
        <input type="hidden" id="MapCardNo" name="MapCardNo" value="<%=Request["CardNo"] %>" />
        <p style="height: 3px">&nbsp;</p>
    </form>
</body>
</html>
