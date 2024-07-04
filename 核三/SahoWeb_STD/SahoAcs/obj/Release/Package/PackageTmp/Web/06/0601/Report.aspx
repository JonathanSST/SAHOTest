<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="iTextSharp.text" %>
<%@ Import Namespace="iTextSharp.text.pdf" %>
<%@ Import Namespace="iTextSharp" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="iTextSharp.tool.xml" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="PagedList" %>


<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">

    DataTable DataReport = new DataTable();

    void Page_Load(object sender, EventArgs e)
    {

        if(Session["PdfReport"] != null)
        {
            this.DataReport = (DataTable)Session["PdfReport"];




        }
        MemoryStream outputStream = new MemoryStream();
        StringBuilder sb = new StringBuilder(@"<html><style> td
                {
                    border-style:solid; border-width:1px; border-color:Black;width:70px;
                } </style><body>");
        if (this.DataReport.Rows.Count > 0)
        {
            var PageData = this.DataReport.AsEnumerable().ToPagedList(1, 18);
            for(int p = 1; p <= PageData.PageCount; p++)
            {
                sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                sb.Append("<tr><td>卡號</td>");
                sb.Append("<td>工號</td>");
                sb.Append("<td>人員姓名</td>");
                sb.Append("<td>單位</td>");
                sb.Append("<td>版次</td>");
                sb.Append("<td>臨時卡號</td>");
                sb.Append("<td>讀卡時間</td>");
                sb.Append("<td>記錄時間</td>");
                sb.Append("<td>設備編號</td>");
                sb.Append("<td>設備名稱</td>");
                sb.Append("<td>讀卡結果</td></tr>");
                foreach(var r in this.DataReport.AsEnumerable().ToPagedList(p,18))
                {
                    sb.Append("<tr><td>"+r["CardNo"].ToString()+"</td>");
                    sb.Append(string.Format("<td>{0}</td>",r["PsnNo"]));
                    sb.Append(string.Format("<td>{0}</td>",r["PsnName"]));
                    sb.Append(string.Format("<td>{0}</td>",r["DepName"]));
                    sb.Append(string.Format("<td>{0}</td>",r["CardVer"]));
                    sb.Append(string.Format("<td>{0}</td>",r["TempCardNo"]));
                    sb.Append(string.Format("<td>{0:yyyy/MM/dd HH:mm:ss}</td>",r["CardTime"]));
                    sb.Append(string.Format("<td>{0:yyyy/MM/dd HH:mm:ss}</td>",r["LogTime"]));
                    sb.Append(string.Format("<td>{0}</td>",r["EquNo"]));
                    sb.Append(string.Format("<td>{0}</td>",r["EquName"]));
                    sb.Append(string.Format("<td>{0}</td></tr>",r["LogStatus"]));
                }
                sb.Append("</table>");
                if (p < PageData.PageCount)
                {
                    sb.Append("<p style='page-break-after:always'>&nbsp;</p>");
                }
            }
        }
        else
        {
            sb.Append("查無資料");
        }
        sb.Append("</body></html>");
        byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
        MemoryStream msInput = new MemoryStream(data);
        Document doc = new Document(PageSize.A4.Rotate(),3,3,3,3);
        PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
        doc.Open();
        XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new SahoAcs.UnicodeFontFactory());
        //將pdfDest設定的資料寫到PDF檔
        PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
        writer.SetOpenAction(action);
        doc.Close();
        msInput.Close();
        //這裡控制Response回應的格式內容
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment;filename=InvoiceData.pdf");
        Response.End();
    }

    void Page_LoadComplete(object sender,EventArgs e){

    }



    public static string GetHtmlSource2(string url)
    {
        //處理內容  
        string html = "";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Accept = "*/*"; //接受任意檔案
        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.1.4322)"; // 模擬使用IE在瀏覽 http://www.52mvc.com
        request.AllowAutoRedirect = true;//是否允許302
                                         //request.CookieContainer = new CookieContainer();//cookie容器，
        request.Referer = url; //當前頁面的引用
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream, Encoding.Default);
        html = reader.ReadToEnd();
        stream.Close();
        return html;
    }


</script>





<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        td {
            border-style: solid;
            border-width: 1px;
            border-color: Black;
            width: 70px;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div id="InvoArea" style="font-size: 5pt;">
            <br/>
            <br/>
            <br/>
            <table width="250" style="margin: 0px; font-size: 10pt;" cellspacing="0">
                <tbody>
                    <tr>
                        <td width="100%">
                            旭威貿易有限公司53982585
                        </td>
                    </tr>
                    <tr>
                        <td width="100%">
                            屏東市前進里崁頂16-6號
                        </td>
                    </tr>
                    <tr>
                        <td width="100%">
                            TEL:08-7530890.FAX:08-7561957
                        </td>
                    </tr>
                </tbody>
            </table>
            <table width="250" style="font-size: 10pt;" cellpadding="0">
                <tbody>
                    <tr>
                        <td width="95%">2019/05/27</td>
                    </tr>
                    <tr>
                        <td width="95%">邦迪建材有限公司<br/>統編：52696698</td>
                    </tr>
                </tbody>
            </table>
            <table width="240" style="font-size: 10pt;">
                <tbody>
                    <tr>
                        <td width="100%" colspan="4">
                            磁磚20X120
                        </td>
                    </tr>
                    <tr>
                        <td width="20%" align="right">
                            80.00
                        </td>
                        <td width="20%" align="right">
                            395.200000
                        </td>
                        <td width="20%" align="right">
                            31616
                        </td>
                        <td width="20%" align="right">
                            &nbsp;
                        </td>
                    </tr>
                </tbody>
            </table>
            <table width="240" style="font-size: 10pt;">
                <tbody>
                    <tr>
                        <td align="left" style="width: 20%;">
                            銷售額:
                        </td>
                        <td align="left" style="width: 20%;"></td>
                        <td align="right" style="width: 20%;">
                            31616
                        </td>
                        <td align="left" style="width: 20%;"></td>
                    </tr>
                    <tr>
                        <td align="left" style="width: 20%;">
                            稅額:
                        </td>
                        <td align="left" style="width: 20%;"></td>
                        <td align="right" style="width: 20%;">
                            1581
                        </td>
                        <td align="left" style="width: 20%;"></td>
                    </tr>
                    <tr>
                        <td align="left" style="width: 20%;">
                            總計:
                        </td>
                        <td align="left" style="width: 20%;"></td>
                        <td align="right" style="width: 20%;">
                            33197
                        </td>
                        <td align="left" style="width: 20%;"></td>
                    </tr>
                </tbody>
            </table>
        </div>

    </form>
        <%
        
         %>


</body>
</html>
