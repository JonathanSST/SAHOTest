$(document).ready(function () {
    $("#BtnQuery").click(function () {
        Block();
        console.log($('input[name*="EquDir"]:eq(0)').val());
        //查詢資料
        $.post("QueryInOutList.aspx",
     {
         "CalendarS": $('[name*="CalendarS"]:eq(0)').val(),
         "CalendarE": $('[name*="CalendarE"]:eq(0)').val(),
         "OrgID": $('[name*="dropCompany"]:eq(0)').val(),
         "PsnNo" : $("#PsnNo").val(),
         "PageEvent": "Query"
     }
     , function (data) {
         var content = $(data).find("#ContentPlaceHolder1_td_showGridView").html();
         //$("#ContentPlaceHolder1_td_showGridView").html("");
         //alert(content);
         $("#ContentPlaceHolder1_td_showGridView").html(content);
         $("#RowCount").html($(data).find("#RowCount").html());
         $.unblockUI();
     });
    });
});


function SetNavigatorEvent() {
    $("#BtnPrev").click(function () {
        $.post("QueryInOutList.aspx",
          {
              "DateS": $('input[name*="CalendarTextBox"]:eq(0)').val(),
              "DateE": $('input[name*="CalendarTextBox"]:eq(1)').val(),
              "CardNo": $("#MapCardNo").val(),
              "DefaultRecordID": $("#HiddenPrev").val()
          },
          function (data) {
              $("#ParaExtDiv").html(data);
              $("#BtnClose").click(function () {
                  CloseWindows();
              });
              SetNavigatorEvent();
          });
    });
    $("#BtnNext").click(function () {
        $.post("QueryInOutList.aspx",
          {
              "DateS": $('input[name*="CalendarS"]:eq(0)').val(),
              "DateE": $('input[name*="CalendarE"]:eq(1)').val(),
              "CardNo": $("#MapCardNo").val(),
              "DefaultRecordID": $("#HiddenNext").val()
          },
          function (data) {
              $("#ParaExtDiv").html(data);
              $("#BtnClose").click(function () {
                  CloseWindows();
              });
              SetNavigatorEvent();
          });
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