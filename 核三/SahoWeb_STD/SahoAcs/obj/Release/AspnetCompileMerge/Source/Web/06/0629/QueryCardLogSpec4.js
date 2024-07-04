$(document).ready(function () {
    $("#BtnQuery").click(function () {
        Block();
        //查詢資料
        $.post("QueryCardLogSpec4.aspx",
     {        
         "DateS": $('input[name*="CalendarTextBox"]:eq(0)').val(),
         "DateE": $('input[name*="CalendarTextBox"]:eq(1)').val(),
         "MgaName": $('[name*="DropMgaList"] option:selected').text(),
         "PageEvent": "Query"
     }
     , function (data) {
         var content = $(data).find("#ResultContent").html();
         $("#ResultContent").html("");
         $("#ResultContent").append(content);
         $.unblockUI();         
     });
    });    
    ColumnSet();
});

function ColumnSet() {
    $("#TableS2").find("tr:eq(0)").find("td").each(function () {
        var td = $(this);
        $("#TableS1").find("tr").find("th:eq(" + $(td).index() + ")").width($(td).width());
        //$(".DataCount:eq(0)").find("tr").find("td:eq(" + $(td).index() + ")").width($(td).width());
    });
}




function CloseWindows() {
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}


function OverlayContent(data) {
    $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
              + ' -webkit-transform: translate3d(0,0,0);"></div>');
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
          + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
    $("#ParaExtDiv").html(data);
    $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
    $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
}