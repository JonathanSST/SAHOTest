$(document).ready(function () {
    $("#AddButton").click(function () {
        var newrow = $('table[id="TableEmpty"]').find("tbody").last().html();
        $("#ContentPlaceHolder1_GridView2 tbody").append(newrow);
        $("#ContentPlaceHolder1_GridView2 tr:last").find("input[type='text']:eq(0)").focus();
    });
    $("#ClassMonth").click(function () {
        ShowMonthDate($(this)[0]);
    })

    $("#EndMonth").click(function () {
        ShowMonthDate2($(this)[0]);
    })

    $("#BtnCreate").click(function () {
        CreateSchedule();
    });
    $('#BtnQuery').click(function () {
        $("#DefMonth").val($("#ClassMonth").val());
        $("#DefEndMonth").val($("#EndMonth").val());
        DoRefresh();
    });
    $('#GroupName').change(function () {
        $('#DefMonth').val('');
        $('#DefEndMonth').val('');
        $("#ResultData").html("");
        $("#MonthSchedule").html("");
    });

    chgGroupName(this);
});


function chgGroupName(e) {
    $.ajax({
        type: "POST",
        url: window.location.href,
        //dataType: 'json',
        data: {
            "PageEvent": "ChangeGroup",
            "GroupName": $('#GroupName').val()
        },
        success: function (data) {
            var result = $.parseJSON(data);
            $("#ClassMonth").val(result['startmonth']);
            $("#EndMonth").val(result['endmonth']);
        }
    });
}


function CreateSchedule() {
    if ($("#ClassMonth").val() == "") {
        alert('請先設定開始指定月份!!');
        return false;
    }
    if ($("#EndMonth").val() == "") {
        alert('請先設定結束指定月份!!');
        return false;
    }
    if ($("#ClassMonth").val() != "" && $("#EndMonth").val() != "") {
        var startmonth = $("#ClassMonth").val().replace("/", "");
        var endmonth = $("#EndMonth").val().replace("/", "");
        if (startmonth > endmonth) {
            alert('結束指定月份必須大於開始指定月份!!');
            return false;
        }
    }
    //if ($("#DefMonth").val() < $("#StateMonth").val()) {
    //    alert('排班月份必須為' + $("#StateMonth").val() + '以後的年月');
    //    return false;
    //}
    var DefArr = new Array();
    $('[name*="ClassNoDef"]').serializeArray().map(function (item) {
        DefArr.push(item.value);
    });
    if (DefArr.length != 2) {
        alert('查無前一月份班表，無法排班');
        return false;
    }
    CheckSessionStatus().then(function (answer) {
        if (answer) {
            Block();
            $.ajax({
                type: "POST",
                url: window.location.url,
                data: {
                    'ClassMonth': $('#ClassMonth').val(),
                    'EndMonth': $('#EndMonth').val(),
                    'GroupName': $('#GroupName').val(),
                    'PageEvent': 'CreateClass',
                    'ClassNoDef': DefArr.join(","),
                    'ClassLast':$('[name*="ClassLast"]').val()
                },
                //contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    if (data.success) {
                        alert("產生完成");
                        DoRefresh();
                    } else {
                        alert(data.message)
                    }
                    $.unblockUI();
                },
                fail: function () {
                    alert("產生失敗");
                    $.unblockUI();
                }
            });
        }
    });
}



function UpdateSchedule() {
    CheckSessionStatus().then(function (answer) {
        if (answer) {
            $.ajax({
                type: "POST",
                url: window.location.url,
                data: $('#MonthSchedule').find('input,select').serialize() + "&PageEvent=UpdateSchedule",
                //contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    if (data.success === true) {
                        alert("班表更新完成");
                        DoRefresh();
                    } else {
                        alert(data.message)
                    }
                },
                fail: function () {
                    alert("更新失敗");
                }
            });
        }
    });
}



function ValidateNumber(e, pnumber) {
    var rt3r = /^\d+/;
    var rt3 = /^\d+$/;
    if (!rt3.test(pnumber)) {
        e.value = rt3r.exec(e.value);
    }
    return false;
}

function CheckSessionStatus() {
    var defer = $.Deferred();
    $.ajax({
        type: "POST",
        url: "../../../ReCheck.ashx",    //執行驗證的頁面
        data: { "StatusName": "UserID", "PageEvent": "CheckStatus" },
        dataType: "json",
        success: function (data) {
            if (data.isSuccess) {
                defer.resolve(true);
            } else {
                alert(data.message);
                defer.resolve(false);
            }
            return defer.promise();
        }

    });
    return defer.promise();
}

//產生當月份的預計排班日
function DoRefresh() {
    if ($("#ClassMonth").val() == "") {
        alert('請設定開始指定月份!!');
        return false;
    }
    if ($("#EndMonth").val() == "") {
        alert('請設定結束指定月份!!');
        return false;
    }
    CheckSessionStatus().then(function (answer) {
        if (answer) {
            Block();
            //取得當月份的排班資訊
            $.ajax({
                type: "POST",
                url: window.location.href,
                //dataType: 'json',
                data: {
                    'ClassMonth': $('#ClassMonth').val(),
                    'EndMonth': $('#EndMonth').val(),
                    'PageEvent': 'QueryData',
                    'GroupName': $('#GroupName').val()
                },
                success: function (data) {
                    if ($(data).find("#LoginSorryArea").length > 0) {
                        window.location.href = "/Default.aspx";
                    }
                    console.log($(data).find("#MonthSchedule").html());
                    if ($(data).find("#MonthSchedule").html().length < 100) {
                        alert("查無資料!!");
                        $.unblockUI();
                    }
                    $("#ResultData").html($(data).find("#ResultData").html());
                    $("#MonthSchedule").html($(data).find("#MonthSchedule").html());
                    //$('#BtnModifySchedule').click(function () {
                    //    //修改排班表
                    //    UpdateSchedule();
                    //});
                    $.unblockUI();
                },
                fail: function () {
                    alert(data.message);
                    $.unblockUI();
                }
            });
        }
    });

}


function ShowMonthDate(obj) {
    $.ajax({
        type: "POST",
        url: "YearMonth.aspx",
        //dataType: 'json',
        data: {},
        success: function (data) {
            $('#dvContent').html(data);
            var mainDiv = document.getElementById("dvContent");
            $('#Year').val($('#ClassMonth').val().split('/')[0]);
            adjustAutoLocation(mainDiv, obj, 55, 22);          //改用遮罩方式處理定位
        }
    });
    
}

function ShowMonthDate2(obj) {
    $.ajax({
        type: "POST",
        url: "YearMonth1.aspx",
        //dataType: 'json',
        data: {},
        success: function (data) {
            $('#dvContent1').html(data);
            var mainDiv1 = document.getElementById("dvContent1");
            $('#Year').val($('#EndMonth').val().split('/')[0]);
            adjustAutoLocation(mainDiv1, obj, 55, 22);          //改用遮罩方式處理定位
        }
    });

}


function InputYearMonth(year, month) {
    if (month.length == 1) {
        month = "0" + month;
    }
    $("#ClassMonth").val(year + "/" + month);
    $("#dvContent").html("");
    $("#DefMonth").val($("#ClassMonth").val());
    //DoRefresh();
}

function InputYearMonth1(year, month) {
    if (month.length == 1) {
        month = "0" + month;
    }
    $("#EndMonth").val(year + "/" + month);
    $("#dvContent1").html("");
    $("#DefEndMonth").val($("#EndMonth").val());
    //DoRefresh();
}