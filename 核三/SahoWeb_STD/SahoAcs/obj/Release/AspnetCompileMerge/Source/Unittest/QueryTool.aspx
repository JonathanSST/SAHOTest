<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryTool.aspx.cs" Inherits="SahoAcs.Unittest.QueryTool" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="<%=".."+Pub.JqueyNowVer %>"></script>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script type="text/javascript">

        function AddParameter() {
            var paradata = '<div><input type="text" id="ParaName" name="ParaName" style="width:150px" value="@"/><input type="text" id="ParaValue" name="ParaValue" style="width:150px"/>'
            paradata += '<select name="ParaType" id="ParaType"><option value="utf-8">nvarchar字串</option><option value="ansi">varchar字串</option><option value="datetime">日期</option><option value="number">數字</option>';
            paradata += '<option value="double">浮點數</option></select>'
            paradata += '<input type="button" value="移除" onclick="Remove(this)"/></div>';
            $("#ParaArea").append(paradata);
            funTabEnter();
        }

        function Remove(obj) {
            $(obj).parent().remove();
        }
        var lastScrollTop = 0;
        $(document).ready(function () {
            /*
            $("#TableDetail").find('tr:eq(0)').find(".DataRow").each(function () {
                //console.log($(this).html());
                var that = $(this);
                //console.log($('#TableTitle').find('tr:eq(0)').find('td').length);
                if ($(that).index() < $('#TableTitle').find('tr:eq(0)').find('td').length) {
                    //console.log($(that).index);
                    $("#TableTitle").find("tr:eq(0)").find("td:eq(" + $(that).index() + ")").width($(that).width());
                }
            });
            */
            
            funTabEnter();
        });
        

        function DoOutput() {
            $("#PageEvent").val("Export");
            $("#form1").submit();
        }

        function DoUpload() {
            $("#PageEvent").val("Upload");
            $("#form1").submit();
        }

        function DoQuery() {
            $("#PageEvent").val("Query");
            $("#ResultArea").html("查詢中....")
            $.ajax({
                type: "POST",
                url: "QueryTool.aspx",
                data: $("#form1").find("input,select,textarea").serialize(),
                success: function (data) {
                    //$("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());                    
                    $("#ResultArea").html($(data).find("#ResultArea").html());
                    SetDataScroll();
                }                
            });          
        }

        function funTabEnter() {
            var focusables = $(':input[type=text],select').not($("#NewRow").find(':input[type=text],select'));            
            focusables.keydown(function (e) {
                if (e.keyCode == 13) {
                    var current = focusables.index(this),
                    next = focusables.eq(current + 1).length ? focusables.eq(current + 1) : focusables.eq(0);                    
                    next.focus();                     
                }
            });
        }


        function DoAppendQuery() {
            $("#PageEvent").val("SkipQuery");
            $.ajax({
                type: "POST",
                url: "QueryTool.aspx",
                data: $("#form1").find("input,select,textarea").serialize(),
                async: true,
                success: function (data) 
                {
                    console.log($(data).find("#SkipCount").val());
                    if ($(data).find("#SkipCount").val() == $("#SkipCount").val()) {

                    } else {
                        $("#TableTitle tbody").append($(data).find("#TableTitle tbody").html());
                        $("#SkipCount").val($(data).find("#SkipCount").val());
                        $('#SpanLoad').remove();
                    }
                    
                }
            });            
        }

        function SetDataScroll() {
            $("#DataScroll").scroll(function (event) {
                var st = $(this).scrollTop();
                console.log("lt==>"+lastScrollTop);
                console.log("st==>"+st);
                console.log("sh==>"+$("#DataScroll").prop("scrollHeight"));
                if (st > lastScrollTop) {
                    if (($("#DataScroll").prop("scrollHeight") - 310) <= st) {
                        $('#DataScroll').append('<span id="SpanLoad">載入中....</span>');
                        DoAppendQuery();
                        //lastScrollTop = st+5;
                    }
                } else {
                    console.log('Scroll down');
                }
                lastScrollTop = st;
            });
        }

    </script>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <input id="PageEvent" name="PageEvent" value="Query" type="hidden" />
        <div id="CommandArea" style="vertical-align: top">
            命令區        
        <textarea id="CommandTxt" name="CommandTxt" style="width: 50%; height: 200px"><%=this.MainCmdStr %></textarea>
            <input type="button" id="BtnOutput" onclick="DoOutput()" value="匯出查詢設定檔" />
            <input type="file" value="匯入查詢設定檔" name="InputFile" id="InputFile" multiple="multiple" />
            <input type="button" id="BtnUpload" onclick="DoUpload()" value="匯入查詢設定檔" />
        </div>
        <div id="ParaArea">
            參數區
            <input type="button" id="BtnAdd" value="加入新參數" onclick="AddParameter()" />
            <%int index = 0; %>
            <%if (this.ParaNames != null && this.ParaNames.Length > 0)
                { %>
            <%foreach (var o in this.ParaNames)
                { %>
            <div>
                <input type="text" value="<%=o %>" name="ParaName" id="ParaName" />
                <input type="text" value="<%=this.ParaValues[index] %>" name="ParaValue" id="ParaValue" />
                <select name="ParaType" id="ParaType">
                    <%foreach (var p in this.ParaModels)
                        { %>
                    <%if (p.EquModel == ParaTypes[index])
                        { %>
                    <option value="<%=p.EquModel %>" selected="selected"><%=p.EquName %></option>
                    <%}
                    else
                    { %>
                    <option value="<%=p.EquModel %>"><%=p.EquName %></option>
                    <%} %>
                    <%} %>
                </select>
                <input type="button" value="移除" onclick="Remove(this)" />
            </div>
            <%index++;
            } %>
            <%} %>
        </div>
        <div id="ResultArea">
            查詢結果區<input type="button" value="查詢" onclick="DoQuery()" />共 <%=this.MasterResult.Count %> 筆記錄
            <div>
                <%=this.ErrorMsg %>
            </div>
            <div style="width: 1234px; height: 300px; overflow-y: scroll; width: 100%" id="DataScroll">
                <input type="hidden" name="SkipCount" value="<%=this.SkipCount %>" id="SkipCount" />
                 <table style="width: 1234px" id="TableTitle">
                <tr>
                    <%foreach (System.Data.DataColumn c in this.DataResult.Columns)
                        { %>
                    <td class="TitleRow" style="border-color: black;border-width:1px;border-style:solid"><%=c.ColumnName %></td>
                    <%} %>
                </tr>
                <%foreach (System.Data.DataRow r in this.DataResult.Rows)
                    { %>
                <tr>
                    <%foreach (System.Data.DataColumn c in this.DataResult.Columns)
                        { %>
                    <td class="DataRow"><%=r[c.ColumnName].ToString() %></td>
                    <%} %>
                </tr>
                <%} %>
            </table>            
            </div>           
        </div>
    </form>
</body>
</html>
