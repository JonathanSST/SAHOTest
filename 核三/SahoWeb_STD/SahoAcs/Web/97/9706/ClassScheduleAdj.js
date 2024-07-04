$(document).ready(function () {
    $("#AddButton").click(function () {
        var newrow = $('table[id="TableEmpty"]').find("tbody").last().html();
        $("#ContentPlaceHolder1_GridView2 tbody").append(newrow);
        $("#ContentPlaceHolder1_GridView2 tr:last").find("input[type='text']:eq(0)").focus();
    });
    $("#ClassMonth").click(function() {
        ShowMonthDate($(this)[0]);
    })
    $("#BtnReJoin").click(function () {
        SetSchedule('MergeClass');
    });
    $("#BtnJoin").click(function () {
        SetSchedule('CreateClass');
    });
    $('#BtnQuery').click(function () {
        $("#DefMonth").val($("#ClassMonth").val());
        DoRefresh();        
    });
    $('#PsnNo').change(function () {
        $('#DefMonth').val('');
        $("#ResultData").html("");
        $("#MonthSchedule").html("");
    });
  
});


function SetSchedule(EventName) {  
    CheckSessionStatus().then(function (answer) {
        if (answer) {
            $.ajax({
                type: "POST",
                url: window.location.url,
                data: { 'ClassMonth': $('#ClassMonth').val(), 'PsnNo': $('#PsnNo').val(), 'PageEvent': EventName },
                //contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    if (data.success === true) {
                        alert("合併完成");
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



function UpdateSchedule() {
    if ($('[name*="AdjClassNo"]').length <= 0) {
        alert('目前已無班別可進行調整');
        return false;
    }
    CheckSessionStatus().then(function (answer) {
        if (answer) {
            $.ajax({
                type: "POST",
                url: window.location.url,
                data: $('#MonthSchedule').find('input,select').serialize() +"&PageEvent=UpdateSchedule",
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
        dataType:"json",
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
function DoRefresh()
{
    CheckSessionStatus().then(function (answer) {
        if (answer) {
            Block();
            //取得當月份的排班資訊
            $.ajax({
                type: "POST",
                url: window.location.href,
                //dataType: 'json',
                data: { 'ClassMonth': $('#ClassMonth').val(), 'PageEvent': 'QueryData', 'PsnNo': $('#PsnNo').val() },
                success: function (data) {
                    if ($(data).find("#LoginSorryArea").length > 0) {
                        window.location.href = "/Default.aspx";
                    }
                    $("#ResultData").html($(data).find("#ResultData").html());
                    $("#MonthSchedule").html($(data).find("#MonthSchedule").html());
                    $('#BtnModifySchedule').click(function () {
                        //修改排班表
                        UpdateSchedule();
                    });
                    $.unblockUI();
                }
            });
        }
    });
  
}


function ShowMonthDate(obj) {
    $.ajax({
        type: "POST",
        url: "../../06/0642/YearMonth.aspx",
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


function InputYearMonth(year, month) {
    if (month.length == 1) {
        month = "0" + month;
    }
    $("#ClassMonth").val(year + "/" + month);
    $("#dvContent").html("");
    $("#DefMonth").val($("#ClassMonth").val());
    DoRefresh();
}