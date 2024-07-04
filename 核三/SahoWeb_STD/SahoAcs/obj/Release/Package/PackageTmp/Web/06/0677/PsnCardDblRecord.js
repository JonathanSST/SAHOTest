﻿$(document).ready(function () {
    BindEvent();
    if($("#PsnID").val()!="")
    {
        $(".ShowPsnInfo").css("display", "none");
    }
});


function PopURL(obj) {
   
}



function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {
        $("#DateE").val($('[name*="$Calendar_CardTimeEDate"]').val());
        $("#DateS").val($('[name*="$Calendar_CardTimeSDate"]').val());
        $("#DepID").val($('[name*="$ddlDept"]').val());
    } else {
        
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
    $("#DateE").val($('[name*="Calendar_CardTimeEDate"]').val());
    $("#DateS").val($('[name*="Calendar_CardTimeSDate"]').val());
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}

function SetPdf() {
    $("#DateE").val($('[name*="Calendar_CardTimeEDate"]').val());
    $("#DateS").val($('[name*="Calendar_CardTimeSDate"]').val());
    $("#PageEvent").val("Pdf");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}



function SetQuery() {    
    $("#DateE").val($('[name*="Calendar_CardTimeEDate"]').val());
    $("#DateS").val($('[name*="Calendar_CardTimeSDate"]').val());
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: location.href,
        //dataType: 'json',
        data: $("#form1").find("input,select").serialize(),
        success: function (data) {
            //alert(data);
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
    //$(".GVStyle").sortable({
    //    opacity: 0.6,    //拖曳時透明
    //    cursor: 'pointer',  //游標設定
    //    axis: 'x,y',       //只能垂直拖曳
    //    update: function () {            
    //        SetQuery();
    //    }
    //});
}