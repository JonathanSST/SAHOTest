$(document).ready(function () {
    $("#BtnQuery").click(function () {
        Block();
        //查詢資料
        $.post("QueryCardLogMap.aspx",
     {
         //"PsnEName": $('input[name*="txtPsnEName"]').val(),
         "DateS": $('input[name*="CalendarTextBox"]:eq(0)').val(),
         "DateE": $('input[name*="CalendarTextBox"]:eq(1)').val(),
         "PageEvent": "Query"
     }
     , function (data) {
         var content = $(data).find("#ContentPlaceHolder1_td_showGridView").html();
         $("#ContentPlaceHolder1_td_showGridView").html("");
         $("#ContentPlaceHolder1_td_showGridView").append(content);
         $.unblockUI();         
         SetBindEvent();
     });
    });
});


function SetBindEvent() {
    $("#ContentPlaceHolder1_MainGridView").find("tr").each(function () {
        var cardno = $(this).find("#CardNo").val();
        var dates = $("")
        var that = $(this);
        $(this).find("#BtnRoute").click(function () {
            $.post("ShowMap2.aspx",
          {
              "DateS": $('input[name*="CalendarTextBox"]:eq(0)').val(),
              "DateE": $('input[name*="CalendarTextBox"]:eq(1)').val(),
              "CardNo": cardno,
              "DefaultRecordID": $(that).find("input[name='RecordID']").val()
          },
          function (data) {
              OverlayContent(data);
              $("#BtnClose").click(function () {
                  CloseWindows();
              });
              SetNavigatorEvent();
          });
        });
    });
}

function SetNavigatorEvent() {
    $("#BtnPrev").click(function () {
        $.post("ShowMap2.aspx",
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
        $.post("ShowMap2.aspx",
          {
              "DateS": $('input[name*="CalendarTextBox"]:eq(0)').val(),
              "DateE": $('input[name*="CalendarTextBox"]:eq(1)').val(),
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
    $(document).scrollTop(0);
    $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
    $("#popOverlay").height($(document).height());
}


function onSelect(td_id, record_1, record_2) {
    var nextid = "";
    $("#CardLogTable td").each(function () {        
        $(this).css("background", "#FFFFFF");
        $(this).css("color", "#000000");
    });
    $("#" + td_id).css("background", "#249AD1");
    $("#" + td_id).css("color", "#FFFFFF");
    $("#company").attr("src", "MapStory2.ashx?CardNo=" + $("#MapCardNo").val() + "&ID1=" + record_1 + "&ID2=" + record_2);
    //$("#companyDefault").attr("src", "MapStory2.ashx?CardNo=" + $("#CardNo").val() + "&ID1=" + record_1 + "&ID2" + record_2);
}