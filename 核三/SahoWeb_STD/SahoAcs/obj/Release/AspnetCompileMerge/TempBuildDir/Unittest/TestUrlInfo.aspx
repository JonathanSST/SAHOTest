<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestUrlInfo.aspx.cs" Inherits="SahoAcs.Unittest.TestUrlInfo" %>
<%@ Import Namespace="TestRefDll" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%
                 //測試request
    StringBuilder sb = new StringBuilder();

    sb.Append("<table cellpadding=3 cellspacing=0 border=1>");

    sb.Append("<tr><td colspan=2>");
    sb.Append("網址：http://localhost:1897/News/Press/Content.aspx/123?id=1#toc");
    sb.Append("</td></tr>");

    // Request.ApplicationPath
    sb.Append("<tr><td>");
    sb.Append("Request.ApplicationPath");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.ApplicationPath + "</b>");
    sb.Append("</td></tr>");

    // Request.PhysicalPath
    sb.Append("<tr><td>");
    sb.Append("Request.PhysicalPath");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.PhysicalPath + "</b>");
    sb.Append("</td></tr>");

    // System.IO.Path.GetDirectoryName(Request.PhysicalPath)
    sb.Append("<tr><td>");
    sb.Append("System.IO.Path.GetDirectoryName(Request.PhysicalPath)");
    sb.Append("</td><td>");
    sb.Append("<b>" + System.IO.Path.GetDirectoryName(Request.PhysicalPath) + "</b>");
    sb.Append("</td></tr>");

    // Request.PhysicalApplicationPath
    sb.Append("<tr><td>");
    sb.Append("Request.PhysicalApplicationPath");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.PhysicalApplicationPath + "</b>");
    sb.Append("</td></tr>");

    // System.IO.Path.GetFileName(Request.PhysicalPath)
    sb.Append("<tr><td>");
    sb.Append("System.IO.Path.GetFileName(Request.PhysicalPath)");
    sb.Append("</td><td>");
    sb.Append("<b>" + System.IO.Path.GetFileName(Request.PhysicalPath) + "</b>");
    sb.Append("</td></tr>");

    // Request.CurrentExecutionFilePath
    sb.Append("<tr><td>");
    sb.Append("Request.CurrentExecutionFilePath");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.CurrentExecutionFilePath + "</b>");
    sb.Append("</td></tr>");

    // Request.FilePath
    sb.Append("<tr><td>");
    sb.Append("Request.FilePath");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.FilePath + "</b>");
    sb.Append("</td></tr>");

    // Request.Path
    sb.Append("<tr><td>");
    sb.Append("Request.Path");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Path + "</b>");
    sb.Append("</td></tr>");

    // Request.RawUrl
    sb.Append("<tr><td>");
    sb.Append("Request.RawUrl");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.RawUrl + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.AbsolutePath
    sb.Append("<tr><td>");
    sb.Append("Request.Url.AbsolutePath");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.AbsolutePath + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.AbsoluteUri
    sb.Append("<tr><td>");
    sb.Append("Request.Url.AbsoluteUri");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.AbsoluteUri + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.Scheme
    sb.Append("<tr><td>");
    sb.Append("Request.Url.Scheme");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.Scheme + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.Host
    sb.Append("<tr><td>");
    sb.Append("Request.Url.Host");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.Host + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.Port
    sb.Append("<tr><td>");
    sb.Append("Request.Url.Port");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.Port + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.Authority
    sb.Append("<tr><td>");
    sb.Append("Request.Url.Authority");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.Authority + "</b>");
    sb.Append("</td></tr>");

    // local Request.Url.LocalPath
    sb.Append("<tr><td>");
    sb.Append("Request.Url.LocalPath");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.LocalPath + "</b>");
    sb.Append("</td></tr>");

    // Request.PathInfo
    sb.Append("<tr><td>");
    sb.Append("Request.PathInfo");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.PathInfo + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.PathAndQuery
    sb.Append("<tr><td>");
    sb.Append("Request.Url.PathAndQuery");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.PathAndQuery + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.Query
    sb.Append("<tr><td>");
    sb.Append("Request.Url.Query");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.Query + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.Fragment
    // 原則上你應該無法從 Request.Url.Fragment 取得任何資料，因為通常 Browser 不會送出 #toc 這個部分
    sb.Append("<tr><td>");
    sb.Append("Request.Url.Fragment");
    sb.Append("</td><td>");
    sb.Append("<b>" + Request.Url.Fragment + "</b>");
    sb.Append("</td></tr>");

    // Request.Url.Segments
    sb.Append("<tr>");
    sb.Append("<td>");
    sb.Append("Request.Url.Segments");
    sb.Append("</td>");
    sb.Append("<td>");
    string[] segments = Request.Url.Segments;
    foreach (string s in segments)
    {
        sb.Append("<b>" + s + "</b>");
        sb.Append("<br/>");
    }
    sb.Append("</td>");
    sb.Append("</tr>");

    sb.Append("</table>");

                %>
            <%=sb.ToString() %>
        </div>

        <%
            CPCWorkDetail.CalcFunc func = new CPCWorkDetail.CalcFunc();
            func.UseMode = "1";
            func.WaitMin = 3;
            func.OverMin = 60;
            func.CustomType = 1;
            //func.CalcRunMode = 0;
            //func.CalcRunCycle = 60;
            //func.CalcBeforeDay = 2;
            func.SWTag = "ABCR";
            func.SWTagList = "1,04:00,06:00;2,12:00,13:00;3,18:00,19:00;0,00:00,00:00";
            //func.CalcRunTimes = "";
            //func.CalcDateRange = "";
            func.LogFileRoot = PublicUtility.LogFileRoot;
            func.LogKeepDay = 100;
            func.DbUserID = PublicUtility.DbUserID;
            func.DbUserPW = PublicUtility.DbUserPW;
            func.DbDataBase = PublicUtility.DbDataBase;
            func.DbDataSource = PublicUtility.DbDataSource;
            func.Start();
            var result = func.CalcCPCWorkDetail("2021/12/13", "2021/12/13");
            func.Stop();
            func.Close();
            func = null;
            %>
        <%=result %>
<%--        
        <%=PublicUtility.Custom %><br />
        <%=PublicUtility.SWTag %><br />
        <%=PublicUtility.SWTagList %><br />
        <%=PublicUtility.IsSrvMode %><br />
        <%=PublicUtility.DbUserID %><br />
        <%=PublicUtility.DbUserPW %><br />
        <%=PublicUtility.DbDataBase %><br />
        <%=PublicUtility.DbDataSource %><br />
        <br /><br />
       <%CoreInfo info = new CoreInfo(); %>
        <%=info.Custom %><br />
        <%=info.DbDataBase %><br />
        <%=info.DbDataSource %><br />
        <%=info.DbUserID %><br />
        <%=info.DbUserPW %><br />
        <%=info.SWTag %><br />
        <%=info.SWTagList %><br />
        <%=info.CalcCPCWorkDetail("2021/12/14","2021/12/15") %>
    
    --%>


    </form>
</body>
</html>
