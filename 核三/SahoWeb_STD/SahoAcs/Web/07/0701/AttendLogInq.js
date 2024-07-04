$(document).ready(function () {
    //document.getElementById("ExportTxtButton").style.display = "none";
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

function SetUserLevel() {
    var str = $("#AuthList").val();
    var data = str.split(",");
    var DoEdit = false;
    for (var i = 0; i < data.length; i++) {
        if (data[i] == 'ShowMap') {
            DoEdit = true;
        }
    }
    //console.log(DoEdit);
    if (DoEdit == false) {
        //console.log(DoEdit);
        $('[name*="BtnShowImage"]').prop('disabled', true);

    }
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
    if ($("[name*='CardTimeSDate1']:eq(0)").val() == "") {
        alert('請設定起始日期!!');
        return false;
    }

    if ($("[name*='CardTimeEDate1']:eq(0)").val() == "") {
        alert('請設定結束日期!!');
        return false;
    }

    var CardTimeSDate = $("[name*='CardTimeSDate1']:eq(0)").val();
    var CardTimeEDate = $("[name*='CardTimeEDate1']:eq(0)").val();

    if (Date.parse(CardTimeSDate).valueOf() > Date.parse(CardTimeEDate).valueOf()) {
        alert('開始時間不可晚於結束時間!!');
        return false;
    }

    $("#PageEvent").val("Query");
    var CardNo = $("#ContentPlaceHolder1_txt_CardNo").val();
    var PsnNo = $("#ContentPlaceHolder1_txt_PsnNo").val();
    var PsnName = $("#ContentPlaceHolder1_txt_PsnName").val();
    var Dep = $("#ContentPlaceHolder1_dropDepartment").val();
    var Control = $("#ContentPlaceHolder1_dropControl").val();
    var checkedValue = $("#ContentPlaceHolder1_chk_EmpCardNo").is(":checked");
    var rdb_byTime0 = $("#ContentPlaceHolder1_rdb_byTime_0").is(":checked");
    var rdb_byTime1 = $("#ContentPlaceHolder1_rdb_byTime_1").is(":checked");
    var OrderByTime;
    if (rdb_byTime0) {
        OrderByTime = "byReceiveTime";
    }
    else {
        OrderByTime = "byLogTime";
    }
    
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "PageEvent": "Query",
            "CardTimeSDate": CardTimeSDate,
            "CardTimeEDate": CardTimeEDate,
            "CardNo": CardNo,
            "PsnNo": PsnNo,
            "PsnName": PsnName,
            "Dep": Dep,
            "Control": Control,
            "IsEmp": checkedValue,
            "OrderByTime": OrderByTime,
            'PageIndex': $("#PageIndex").val()
        },
        error: function () {
            alert("讀取資料發生錯誤!!");
        },
        success: function (data) {
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

function SetPrintTxt() {
    if ($("[name*='CardTimeSDate1']:eq(0)").val() == "") {
        alert('請設定起始日期!!');
        return false;
    }

    if ($("[name*='CardTimeEDate1']:eq(0)").val() == "") {
        alert('請設定結束日期!!');
        return false;
    }

    $("#PageEvent").val("PrintTxt");
    //var CardTimeSDate = $("#ContentPlaceHolder1_Calendar_CardTimeSDate_CalendarTextBox").val();
    //var CardTimeEDate = $("#ContentPlaceHolder1_Calendar_CardTimeEDate_CalendarTextBox").val();
    var CardTimeSDate = $("[name*='CardTimeSDate1']:eq(0)").val();
    var CardTimeEDate = $("[name*='CardTimeEDate1']:eq(0)").val();
    var CardNo = $("#ContentPlaceHolder1_txt_CardNo").val();
    var PsnNo = $("#ContentPlaceHolder1_txt_PsnNo").val();
    var PsnName = $("#ContentPlaceHolder1_txt_PsnName").val();
    var Dep = $("#ContentPlaceHolder1_dropDepartment").val();
    var Control = $("#ContentPlaceHolder1_dropControl").val();
    var checkedValue = $("#ContentPlaceHolder1_chk_EmpCardNo").is(":checked");
    var rdb_byTime0 = $("#ContentPlaceHolder1_rdb_byTime_0").is(":checked");
    var rdb_byTime1 = $("#ContentPlaceHolder1_rdb_byTime_1").is(":checked");
    var OrderByTime;
    if (rdb_byTime0) {
        OrderByTime = "byReceiveTime";
    }
    else {
        OrderByTime = "byLogTime";
    }
    $("#CardTimeSDate").val(CardTimeSDate);
    $("#CardTimeEDate").val(CardTimeEDate);
    $("#CardNo").val(CardNo);
    $("#PsnNo").val(PsnNo);
    $("#PsnName").val(PsnName);
    $("#Dep").val(Dep);
    $("#Control").val(Control);
    $("#IsEmp").val(checkedValue);
    $("#OrderByTime").val(OrderByTime);

    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "PageEvent": "Query",
            "CardTimeSDate": CardTimeSDate,
            "CardTimeEDate": CardTimeEDate,
            "CardNo": CardNo,
            "PsnNo": PsnNo,
            "PsnName": PsnName,
            "Dep": Dep,
            "Control": Control,
            "IsEmp": checkedValue,
            "OrderByTime": OrderByTime,
            'PageIndex': $("#PageIndex").val()
        },
        error: function () {
            alert("讀取資料發生錯誤!!");
        },
        success: function (data) {
            //console.log(data);
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

function SetPrintExcel() {
    if ($("[name*='CardTimeSDate1']:eq(0)").val() == "") {
        alert('請設定起始日期!!');
        return false;
    }

    if ($("[name*='CardTimeEDate1']:eq(0)").val() == "") {
        alert('請設定結束日期!!');
        return false;
    }

    var CardTimeSDate = $("[name*='CardTimeSDate1']:eq(0)").val();
    var CardTimeEDate = $("[name*='CardTimeEDate1']:eq(0)").val();

    if (Date.parse(CardTimeSDate).valueOf() > Date.parse(CardTimeEDate).valueOf()) {
        alert('開始時間不可晚於結束時間!!');
        return false;
    }

    $("#PageEvent").val("PrintExcel");
    var CardNo = $("#ContentPlaceHolder1_txt_CardNo").val();
    var PsnNo = $("#ContentPlaceHolder1_txt_PsnNo").val();
    var PsnName = $("#ContentPlaceHolder1_txt_PsnName").val();
    var Dep = $("#ContentPlaceHolder1_dropDepartment").val();
    var Control = $("#ContentPlaceHolder1_dropControl").val();
    var checkedValue = $("#ContentPlaceHolder1_chk_EmpCardNo").is(":checked");
    var rdb_byTime0 = $("#ContentPlaceHolder1_rdb_byTime_0").is(":checked");
    var OrderByTime;
    if (rdb_byTime0) {
        OrderByTime = "byReceiveTime";
    }
    else {
        OrderByTime = "byLogTime";
    }
    $("#CardTimeSDate").val(CardTimeSDate);
    $("#CardTimeEDate").val(CardTimeEDate);
    $("#CardNo").val(CardNo);
    $("#PsnNo").val(PsnNo);
    $("#PsnName").val(PsnName);
    $("#Dep").val(Dep);
    $("#Control").val(Control);
    $("#IsEmp").val(checkedValue);
    $("#OrderByTime").val(OrderByTime);

    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "PageEvent": "Query",
            "CardTimeSDate": CardTimeSDate,
            "CardTimeEDate": CardTimeEDate,
            "CardNo": CardNo,
            "PsnNo": PsnNo,
            "PsnName": PsnName,
            "Dep": Dep,
            "Control": Control,
            "IsEmp": checkedValue,
            "OrderByTime": OrderByTime,
            'PageIndex': $("#PageIndex").val()
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



