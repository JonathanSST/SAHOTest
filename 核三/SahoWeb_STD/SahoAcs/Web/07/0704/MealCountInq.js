$(document).ready(function () {
    //document.getElementById("ExportXlsButton").style.display = "none";
});

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

function SetMode(mode) {
    $("#QueryMode").val(mode);
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

function SetQuery() {
    if ($("#ContentPlaceHolder1_Calendar_CardTimeSDate_CalendarTextBox").val() == "") {
        alert('請設定起始日期!!');
        return false;
    }

    if ($("#ContentPlaceHolder1_Calendar_CardTimeEDate_CalendarTextBox").val() == "") {
        alert('請設定結束日期!!');
        return false;
    }

    var CardTimeSDate = $("#ContentPlaceHolder1_Calendar_CardTimeSDate_CalendarTextBox").val();
    var CardTimeEDate = $("#ContentPlaceHolder1_Calendar_CardTimeEDate_CalendarTextBox").val();

    if (Date.parse(CardTimeSDate).valueOf() > Date.parse(CardTimeEDate).valueOf()) {
        alert('開始時間不可晚於結束時間!!');
        return false;
    }

    $("#PageEvent").val("Query");
   
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "PageEvent": "Query",
            "CardTimeSDate": CardTimeSDate,
            "CardTimeEDate": CardTimeEDate,
            "PageIndex": $("#PageIndex").val()
        },
        error: function () {
            alert("讀取資料發生錯誤!!");
        },
        success: function (data) {
            //console.log(data.length);
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
            $.unblockUI();
            var tds = $("#ContentPlaceHolder1_MainGridView tr").children('td').length;
            if (tds == 1) {
                alert("查無資料");
            }
        }
    });
}

function SetPrintExcel() {
    if ($("#ContentPlaceHolder1_Calendar_CardTimeSDate1_CalendarTextBox1").val() == "") {
        alert('請設定起始日期!!');
        return false;
    }

    if ($("#ContentPlaceHolder1_Calendar_CardTimeEDate1_CalendarTextBox1").val() == "") {
        alert('請設定結束日期!!');
        return false;
    }

    var CardTimeSDate = $("#ContentPlaceHolder1_Calendar_CardTimeSDate_CalendarTextBox").val();
    var CardTimeEDate = $("#ContentPlaceHolder1_Calendar_CardTimeEDate_CalendarTextBox").val();

    if (Date.parse(CardTimeSDate).valueOf() > Date.parse(CardTimeEDate).valueOf()) {
        alert('開始時間不可晚於結束時間!!');
        return false;
    }

    $("#PageEvent").val("PrintExcel");
    $("#CardTimeSDate").val(CardTimeSDate);
    $("#CardTimeEDate").val(CardTimeEDate);
    
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "PageEvent": "Query",
            "CardTimeSDate": CardTimeSDate,
            "CardTimeEDate": CardTimeEDate,
            "PageIndex": $("#PageIndex").val()
        },
        error: function () {
            alert("讀取資料發生錯誤!!");
        },
        success: function (data) {
            //console.log(data.length);
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
            $.unblockUI();
            var tds = $("#ContentPlaceHolder1_MainGridView tr").children('td').length;
            if (tds == 1) {
                alert("查無資料");
            }
            else {
                $("#form1").attr("target", "_blank");
                $("#form1").submit();
            }
        }
    });

}