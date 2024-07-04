$(document).ready(function () {
    $("#AddButton").click(function () {
        var newrow = $('table[id="TableEmpty"]').find("tbody").last().html();
        $("#ContentPlaceHolder1_GridView2 tbody").append(newrow);
        $("#ContentPlaceHolder1_GridView2 tr:last").find("input[type='text']:eq(0)").focus();
    });
    $("#ClassMonth").click(function() {
        ShowMonthDate($(this)[0]);
    })
    $("#BtnCreate").click(function () {
        CreateSchedule();
    });
    $('#BtnQuery').click(function () {
        DoRefresh();
        $("#DefMonth").val($("#ClassMonth").val());
    });
});


function CreateSchedule() {
    if ($('[name*="PsnNo"]').serializeArray().length == 0) {
        alert('目前尚無未初始化人員資料，無法初始化');
        return false;
    }
    $.ajax({
        type: "POST",
        url: window.location.url,
        data: $('#form1').find('input,select').serialize(),
        //contentType: "application/json",
        dataType: "json",
        statusCode: {
            200: function (response) {
                console.log(response);
               // alert('系統已被登出');
            },
            201: function (response) {
                console.log(response);
                alert('系統已被登出');
            },
            302: function (response) {
                console.log(response);
                alert('系統已被登出');
            }
        },
        success: function (data) {
            if (data.success === true) {
                alert("更新完成");
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

function ValidateNumber(e, pnumber) {
    var rt3r = /^\d+/;
    var rt3 = /^\d+$/;
    if (!rt3.test(pnumber)) {
        e.value = rt3r.exec(e.value);
    }
    return false;
}





function DoSave() {
    //這裡要將結果帶回去選單
    //console.log($('[name*="DeviceNo"]').val());
    var result = $('[name*="DeviceNo"]').map(function () {
        return $(this).val();
    }).get().join(',');
    console.log(result);
    console.log($(that).find('#EvoName').val());
    $(that).find('#EvoName').val(result);
    $(that).find("#EvoInfo").text(result);
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}


//產生當月份的預計排班日
function DoRefresh()
{
    Block();
    //取得當月份的排班資訊
    $.ajax({
        type: "POST",
        url: window.location.href,
        //dataType: 'json',
        data: { 'ClassMonth': $('#ClassMonth').val(), 'PsnNo': $('#PsnNo').val(), 'PageEvent': 'QueryData' },       
        success: function (data) {
            $("#ResultData").html($(data).find("#ResultData").html());
            $.unblockUI();
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