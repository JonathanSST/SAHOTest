$(document).ready(function () {
    $("#BtnQuery").click(function () {
        Block();
        //查詢資料
        $.post("CardVerUpdate.aspx",
     {
         "CalendarS": $('[name*="CalendarS"]:eq(0)').val(),
         "CalendarE": $('[name*="CalendarE"]:eq(0)').val(),
         "OrgID": $('[name*="dropCompany"]:eq(0)').val(),
         "PsnNo": $("#PsnNo").val(),
         "PageEvent": "Query"
     }
     , function (data) {
         var content = $(data).find("#ContentPlaceHolder1_td_showGridView").html();
         $("#ContentPlaceHolder1_td_showGridView").html(content);
         $("#RowCount").html($(data).find("#RowCount").html());
         $.unblockUI();
     });
    });
    $("#BtnUpdate").click(function (){

        var re = /^[A-F0-9]+$/;
        if (re.test($('input[name*="CardVer"]').val()) == false) {
            //版次為必要欄位
            alert("卡片版次為必要輸入欄位!!\n格式必須為數字!!");
            $('input[name*="CardVer"]').focus();
            return false;
        } else {
            SetCardVerUpdate();
        }
    });

});


function SetCardVerUpdate() {
    Block();
    $.ajax({
        type: "POST",
        url: "CardVerUpdate.aspx",
        data: $('input[name*="ChkStrucID"]').serialize() + "&PageEvent=Save&CardVer=" + $("#CardVer").val()+"&ProcessTime="+$('[name*="ProcessTime"]').val(),
        //contentType: "application/json",
        dataType: "json",
        success: function (data) {
            //console(data);        
            $.unblockUI();
            //SetQuery();
            alert('更新完成!!');
        },
        fail: function () {
            alert("更新失敗");
            $.unblockUI();
        }
    });
    
    console.log($('input[name*="CardID"]').serialize()+"&PageEvent=Save");
}

function SetQuery() {
    //查詢資料
    $.post("CardVerUpdate.aspx",
     {
         "CalendarS": $('[name*="CalendarS"]:eq(0)').val(),
         "CalendarE": $('[name*="CalendarE"]:eq(0)').val(),
         "OrgID": $('[name*="dropCompany"]:eq(0)').val(),
         "PsnNo": $("#PsnNo").val(),
         "PageEvent": "Query"
     }, function (data) {
         var content = $(data).find("#ContentPlaceHolder1_td_showGridView").html();
         $("#ContentPlaceHolder1_td_showGridView").html(content);
         $("#RowCount").html($(data).find("#RowCount").html());
     });
}

function SetNavigatorEvent() {
    $("#BtnPrev").click(function () {
        $.post("CardVerUpdate.aspx",
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
        $.post("CardVerUpdate.aspx",
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