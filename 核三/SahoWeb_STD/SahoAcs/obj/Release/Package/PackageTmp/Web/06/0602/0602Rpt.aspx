<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="0602Rpt.aspx.cs" Inherits="SahoAcs.Web._06._0602._0602Rpt" %>
<%@ Import Namespace="PagedList" %>
<%@ Import Namespace="iTextSharp.text" %>
<%@ Import Namespace="iTextSharp.text.pdf" %>
<%@ Import Namespace="System.IO" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<script runat="server">

   

</script>
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style media="print" type="text/css">
        .doPrint
        {
            display: block;
        }

        .noPrint
        {
            display: none;
        }

        .dataclass
        {
            /*border-color: #000000;
            border-width: 1px;
            border-style: solid;*/
            font-size: 10pt;
            /*font-family: 標楷體;*/
        }

        
        td
        {
            border-bottom-color: #000000;
            border-width: 1px;
            border-style: solid;
        }

    </style>
    <style type="text/css">
        .dataclass
        {
            /*border-color: #000000;
            border-width: 1px;
            border-style: solid;
                */
            font-size: 10pt;
            vertical-align: top;
        }

        td
        {
            border-bottom-color: #000000;
            border-width: 1px;
            border-style: solid;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div runat="server" id="PrintArea">
        <%for(int p=1;p<= this.ListLog.ToPagedList(1,10).PageCount; p++) { %>                        
        <table  style="border-style: solid; border-width: 0px; border-collapse: collapse;width:100%">
            <tr>
                <td style="width:10px">
                    查詢條件：
                </td>
                <td style="">
                    <span><%=string.Format("刷卡區間 {0}~{1}", Request["CardTimeS"],Request["CardTimeE"]) %></span>    
					<span style="margin-left:100px">姓名：<%=Request["CardNoPsnNo"] %></span>
                </td>
                <td>
                    
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    列印時間：
                </td>
                <td>
                    <span><%=DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") %></span>
                    <span style="margin-left:400px"><%=string.Format("頁次：{1}", this.ListLog.ToPagedList(1, 10).PageCount,p) %></span>
                </td>
                <td style="text-align:right">
                    
                </td>
            </tr>
        </table>
        <table  style="border-collapse: collapse">
            <tr class="dataclass">
                      <td style="width: 16%; text-align: left;"><span style="writing-mode:vertical-rl">刷卡時間</span>
                      </td>
                      <td style="width: 10%; text-align: left;">編號
                      </td>
                    <td style="width: 10%; text-align: left;">卡號
                      </td>
                      <td style="width: 8%; text-align: left;">版次
                      </td>
                      <td style="width: 8%; text-align: left;">姓名
                      </td>                      
                      <td style="width: 10%; text-align: left;">單位
                      </td>
                      <td style="width: 16%; text-align: left;">刷卡位置
                      </td>
                      <td style="width: 12%; text-align: left;">刷卡結果
                      </td>
                  </tr>
            <%foreach(var o in this.ListLog.ToPagedList(p,10)) { %>
                <tr>
                    <td style="text-align: left;vertical-align:top"><%=string.Format("{0:yyyy/MM/dd HH:mm:ss}",o.CardTime) %>
                      </td>
                      <td style="text-align: left;vertical-align:top"><%=o.PsnNo %>
                      </td>
                    <td style="text-align: left;vertical-align:top"><%=o.IDNum %>
                      </td>
                      <td style="text-align: left;vertical-align:top"><%=o.CardVer %>
                      </td>
                      <td style="text-align: left;vertical-align:top"><%=o.PsnName %>
                      </td>                      
                      <td style="text-align: left;vertical-align:top"><%=o.DepName.Split('[').Length>0?o.DepName.Split('[')[0]:"" %>
                      </td>
                      <td style="text-align: left;vertical-align:top"><%=o.EquName %>
                      </td>
                      <td style="text-align: left;vertical-align:top"><%=o.LogStatus %>
                      </td>
                </tr>
            <%} %>
        </table>
        <%} %>
        資料筆數： <%=this.ListLog.Count %>
    </div>
    <%
        StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:10pt
                } </style>
                <body style='font-size:10pt'>");
        if (this.ListLog.Count > 0)
        {
            var PageData = this.ListLog.ToPagedList(1, 25);
            for (int p = 1; p <= PageData.PageCount; p++)
            {
                sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>刷卡資料列印</div>");
                sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>刷卡時間區間：{0}~{1}</td><td>卡號：{2}</td></tr></table>",Request["CardTimeS"],Request["CardTimeE"], ""));
                sb.Append(string.Format("<table style='width:100%;font-size:10pt'><tr><td style='width:50%'>列印日期：{0:yyyy/MM/dd HH:mm:ss}</td><td>頁次：{1}</td></tr></table>",DateTime.Now,p));
                sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");                
                sb.Append("<tr><td class='DataBar' style='width:20%'><span style='writing-mode:vertical-rl'>刷卡時間</span></td>");
                sb.Append("<td class='DataBar' style='width:7%'>編號</td>");
                sb.Append("<td class='DataBar' style='width:10%'>卡號</td>");
                sb.Append("<td class='DataBar' style='width:6%'>版次</td>");
                sb.Append("<td class='DataBar' style='width:7%'>姓名</td>");
                sb.Append("<td class='DataBar' style='width:15%'>單位</td>");
                sb.Append("<td class='DataBar' style='width:15%'>刷卡位置</td>");
                sb.Append("<td class='DataBar' style='width:15%'>刷卡結果</td></tr>");
                foreach (var o in this.ListLog.ToPagedList(p,25))
                {
                    sb.Append("<tr><td class='DataBar' style='vertical-align:top'>" +string.Format("{0:yyyy/MM/dd HH:mm:ss}",o.CardTime) + "</td>");
                    sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnNo));
                    sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.IDNum));
                    sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.CardVer));
                    sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.PsnName));
                    sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.DepName.Split('[').Length>0?o.DepName.Split('[')[0]:""));
                    sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td>", o.EquName));
                    sb.Append(string.Format("<td class='DataBar' style='vertical-align:top'>{0}</td></tr>", o.LogStatus));
                }
                sb.Append("</table>");
                sb.Append("總筆數：" + this.ListLog.Count);
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
        Response.Clear();
        byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
        MemoryStream msInput = new MemoryStream(data);
        iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 5, 5, 5, 5);
        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.text.pdf.PdfDestination pdfDest = new iTextSharp.text.pdf.PdfDestination(iTextSharp.text.pdf.PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
        doc.Open();
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new SahoAcs.UnicodeFontFactory());
        //將pdfDest設定的資料寫到PDF檔
        iTextSharp.text.pdf.PdfAction action = iTextSharp.text.pdf.PdfAction.GotoLocalPage(1, pdfDest, writer);
        writer.SetOpenAction(action);
        doc.Close();
        msInput.Close();
        //這裡控制Response回應的格式內容

        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment;filename=個人讀卡記錄報表.pdf");
        Response.End();
         %>
    </form>
</body>
</html>
