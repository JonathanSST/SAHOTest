<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebUtcTest.aspx.cs" Inherits="SahoAcs.Unittest.WebUtcTest" %>
<%@Import Namespace="SahoAcs.DBClass" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <% var t1 = DateTime.Now; %> 
       目前本地時間：<% = string.Format("{0:yyyy/MM/dd HH:mm:ss}",t1) %> 
       <br />轉為dbutc：<%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",t1.GetUtcTime(this)) %>
       
        <div>
             example:
             if (!IsUtc)
            {
                return oTime;
            }
            string TimeOffset = "TimeOffset";
            int TimeDiff = 0;
            if (oPage.Session[TimeOffset] != null && oPage.Session[TimeOffset].ToString() != string.Empty)
            {
                TimeDiff = Convert.ToInt32(oPage.Session[TimeOffset]);
            }           
            //DateTime.Now.utc
            return oTime.AddMinutes(TimeDiff).AddHours(DbUTC);
        </div>
        <div>
            <% var t2 = DateTime.UtcNow; %> 
                  目前UTC時區時間：<% = string.Format("{0:yyyy/MM/dd HH:mm:ss}",t2) %> 
       <br />dbutc轉為本地時區：<%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",t1.GetZoneTime(this)) %>
        </div>
    </div>
    </form>
</body>
</html>
