﻿$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
});


//設定進階查詢功能
function SetAdvArea() {    
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $("#popOverlay").css("display", "block");
    $("#AdvanceArea").css("display", "block");
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#AdvanceArea").css("left", 0);
    $("#AdvanceArea").css("top", 0);
    $("#AdvanceArea").css("left", ($(document).width() - $("#AdvanceArea").width()) / 2);
    $("#AdvanceArea").css("top", $(document).scrollTop() + 50);
}

function CancelAdvArea() {
    $("#AdvanceArea").css("display", "none");
    $("#popOverlay").css("display", "none");
}

function SetShowCardLogDetail() {
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $("#popOverlay").css("display", "block");
    $("#AdvanceArea").css("display", "block");
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#AdvanceArea").css("left", 0);
    $("#AdvanceArea").css("top", 0);
    $("#AdvanceArea").css("left", ($(document).width() - $("#AdvanceArea").width()) / 2);
    $("#AdvanceArea").css("top", $(document).scrollTop() + 50);
}

function CancelOneLogArea() {
    $("#OneLogArea").css("display", "none");
    $("#popOverlay").css("display", "none");
}




function PopURL(obj) {
   
}



function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {
        $("#DateE").val($('[name*="CalendarE"]').val());
        $("#DateS").val($('[name*="CalendarS"]').val());        
    }
    ShowPage(1);
}

function SetSort(sort_name) {
    $("#SortName").val(sort_name);
    if ($("#SortType").val() == "ASC") {
        $("#SortType").val("DESC");
    } else {
        $("#SortType").val("ASC");
    }
    SetQuery();
}

function ShowPage(page_index) {
    $("#PageIndex").val(page_index);
    SetQuery();
}


function SetPrint() {
    //$("#CardTimeE").val($('[name*="Calendar_CardTimeEDate"]').val());
    //$("#CardTimeS").val($('[name*="Calendar_CardTimeSDate"]').val());
    //$("#form1").submit();    
    //window.open("0601Rpt.aspx?" + $(".Content").find("input,select").not('[name*="DataCol"]').serialize());
    $("#PageEvent").val("Print");
    //$("#DateS").val($('input[name*="CalendarS"]').val());
    //$("#DateE").val($('input[name*="CalendarE"]').val());
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}


function SetQuery() {    
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: "QueryCourseLog.aspx",
        //dataType: 'json',
        data: { "CourseName":$("#CourseName").val(), "PageEvent": "Query", "SortType": $("#SortType").val(), "SortName": $("#SortName").val() },
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
            $.unblockUI();
        }
    });
}

function BindEvent() {
    $('.GVStyle').find('th').each(function (index) {
        if (index < $('.GVStyle').find('th').length - 1) {
            $(this).css("width", $(this).find('[name*="TitleCol"]').val());
        }
    });
    $('.DataRow').each(function (outIndex) {
        var that = $(this);
        $(that).find('td').each(function (index) {
            if (index < $(that).find('td').length - 1) {
                $(this).css("width", $(this).find('[name*="DataCol"]').val());
            }
        });
    }); 
}