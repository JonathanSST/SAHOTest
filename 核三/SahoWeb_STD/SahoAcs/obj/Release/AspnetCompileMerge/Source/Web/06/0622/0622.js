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



function PopURL(obj) {
   
}



function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {
        $("#CardTimeE").val($('[name*="$Calendar_CardTimeEDate"]').val());
        $("#CardTimeS").val($('[name*="$Calendar_CardTimeSDate"]').val());
        var arr = new Array();
        $('[name*="$DropDownList_LogStatus"]').each(function (i) {
            if ($(this).prop("checked") == true) {
                arr.push($(this).val());
            }
        });
        $("#LogStatus").val(arr.join(','));
        $("#EquClass").val($("#ddlEquClass").val());
    } else {
        $("#CardTimeS").val($('[name*="ADVCalendar_CardTimeSDate"]').val());
        $("#CardTimeE").val($('[name*="ADVCalendar_CardTimeEDate"]').val());
        $("#LogTimeS").val($('[name*="ADVCalendar_LogTimeSDate"]').val());
        $("#LogTimeE").val($('[name*="ADVCalendar_LogTimeEDate"]').val());
        $("#EquClass").val($("#advEquClass").val());
        var arr = new Array();
        $('[name*="$ADVDropDownList_LogStatus"]').each(function (i) {
            if ($(this).prop("checked") == true) {
                arr.push($(this).val());
            }
        });
        $("#LogStatus").val(arr.join(','));

        var arr2 = new Array();
        $('[name*="Dep$DDList"]').each(function (i) {
            if ($(this).prop("checked") == true) {
                arr2.push($(this).val());
            }
        });
        $("#DepNo").val(arr2.join(','));

        var arr3 = new Array();
        $('[name*="Equ$DDList"]').each(function (i) {
            if ($(this).prop("checked") == true) {
                arr3.push($(this).val());
            }
        });
        $("#EquNo").val(arr3.join(','));
        $("#AdvanceArea").css("display", "none");
        $("#popOverlay").css("display", "none");
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
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}


function SetQuery() {    
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: "0622.aspx",
        //dataType: 'json',
        data: $("#form1").find("input,select").serialize(),
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
    $(".GVStyle").sortable({
        opacity: 0.6,    //拖曳時透明
        cursor: 'pointer',  //游標設定
        axis: 'x,y',       //只能垂直拖曳
        update: function () {            
            SetQuery();
        }
    });
}