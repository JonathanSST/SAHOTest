﻿$(document).ready(function () {
    $("#BtnQuery").click(function () {
        Block();
        //查詢資料
        $.post("QueryCardLogSpec3.aspx",
     {
         "CardNo_PsnName": $('input[name*="CardNo_PsnName"]').val(),
         "DateS": $('input[name*="CalendarTextBox"]:eq(0)').val(),
         "DateE": $('input[name*="CalendarTextBox"]:eq(1)').val(),
         "PageEvent": "Query"
     }
     , function (data) {
         var content = $(data).find("#ContentPlaceHolder1_td_showGridView").html();
         $("#ContentPlaceHolder1_td_showGridView").html("");
         $("#ContentPlaceHolder1_td_showGridView").append(content);
         $.unblockUI();         
     });
    });
});



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