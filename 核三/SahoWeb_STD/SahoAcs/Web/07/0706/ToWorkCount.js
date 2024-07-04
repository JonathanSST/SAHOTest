$(document).ready(function () {
    $("#BtnQuery").click(function () {
        SetQuery();
    });

    $("#ExportButton").click(function () {
        SetPrint();
    });
});

function SetBindEvent() {
    $("#ContentPlaceHolder1_MainGridView").find("tr").each(function () {
        $(this).find("#BtnDetail").click(function () {
            $.post("WorkDetail.aspx",
                {
                    "StartTime": $("#ContentPlaceHolder1_QueryTimeS_CalendarTextBox").val(),
                    "EndTime": $("#ContentPlaceHolder1_QueryTimeE_CalendarTextBox").val(),
                    "CompanyNo": $('#HidCompanyNo').val(),
                    "DepartmentNo": $('#HidDepartmentNo').val(),
                    "CompanyName": $('#HidCompanyName').val(),
                    "DepartmentName": $('#HidDepartmentName').val()
                },
                function (data) {
                    OverlayContent(data);
                    $("#BtnClose").click(function () {
                        CloseWindows();
                    });
                    //SetNavigatorEvent();
                });
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

function SetQuery() {

    if ($("#ContentPlaceHolder1_QueryTimeS_CalendarTextBox").val() == "") {
        alert('請設定請假起始日期!!');
        return false;
    }

    if ($("#ContentPlaceHolder1_QueryTimeE_CalendarTextBox").val() == "") {
        alert('請設定請假訖止日期!!');
        return false;
    }

    var StartTime = $("#ContentPlaceHolder1_QueryTimeS_CalendarTextBox").val();
    var EndTime= $("#ContentPlaceHolder1_QueryTimeE_CalendarTextBox").val();

    if (Date.parse(StartTime).valueOf() > Date.parse(EndTime).valueOf()) {
        alert('開始時間不可晚於結束時間!!');
        return false;
    }

    Block();
    //查詢資料
    $.post("ToWorkCount.aspx",
        {
            //"PsnEName": $('input[name*="txtPsnEName"]').val(),
            "StartTime": $("#ContentPlaceHolder1_QueryTimeS_CalendarTextBox").val(),
            "EndTime": $("#ContentPlaceHolder1_QueryTimeE_CalendarTextBox").val(),
            "PageEvent": "QueryData",
            "Company": $('#ContentPlaceHolder1_dropCompany').val(),
            "Department": $('#ContentPlaceHolder1_dropDepartment').val(),
            "PageIndex": $("#PageIndex").val()
        }
        , function (data) {
            var content = $(data).find("#ContentPlaceHolder1_td_showGridView").html();
            $("#ContentPlaceHolder1_td_showGridView").html("");
            $("#ContentPlaceHolder1_td_showGridView").append(content);
            $.unblockUI();
            SetBindEvent();
        });
}

function SetPrint() {

    if ($("#ContentPlaceHolder1_QueryTimeS_CalendarTextBox").val() == "") {
        alert('請設定請假起始日期!!');
        return false;
    }

    if ($("#ContentPlaceHolder1_QueryTimeE_CalendarTextBox").val() == "") {
        alert('請設定請假訖止日期!!');
        return false;
    }

    var StartTime = $("#ContentPlaceHolder1_QueryTimeS_CalendarTextBox").val();
    var EndTime = $("#ContentPlaceHolder1_QueryTimeE_CalendarTextBox").val();

    if (Date.parse(StartTime).valueOf() > Date.parse(EndTime).valueOf()) {
        alert('開始時間不可晚於結束時間!!');
        return false;
    }

    $('#StartTime').val($("#ContentPlaceHolder1_QueryTimeS_CalendarTextBox").val());
    $('#EndTime').val($("#ContentPlaceHolder1_QueryTimeE_CalendarTextBox").val());
    $('#Company').val($('#ContentPlaceHolder1_dropCompany').val());
    $('#Department').val($('#ContentPlaceHolder1_dropDepartment').val());
    $("#PageEvent").val("Print");

    //var tds = $("#ContentPlaceHolder1_MainGridView tr").children('td').length;
    //if (tds <= 5) {
    //    alert("查無資料");
    //}
    //else {
        $("#form1").attr("target", "_blank");
        $("#form1").submit();
    //}

    ////$("#form1").attr("target", "_blank");
    ////$("#form1").submit();
}
