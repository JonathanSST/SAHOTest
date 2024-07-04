$(document).ready(function () {
    $("#AddButton").click(function () {
        var newrow = $('table[id="TableEmpty"]').find("tbody").last().html();
        $("#ContentPlaceHolder1_GridView2 tbody").append(newrow);
        $("#ContentPlaceHolder1_GridView2 tr:last").find("input[type='text']:eq(0)").focus();
    });
    $("#ClassMonth").click(function () {
        ShowMonthDate($(this)[0]);
    });
    $('#BtnQuery').click(function () {
        $("#DefMonth").val($("#ClassMonth").val());
        DoRefresh();        
    });
    $("#BtnPrint").click(function () {
        $("#DefMonth").val($("#ClassMonth").val());
        DoPrint();
    });
});


function CreateSchedule() {
    if ($("#DefMonth").val() === "") {
        alert('請先設定本月預計排班日!!');
        return false;
    }
    if ($("#DefMonth").val() < $("#StateMonth").val()) {
        alert('排班月份必須為' + $("#StateMonth").val() + '以後的年月');
        return false;
    }
    var DefArr = new Array();
    $('[name*="ClassNoDef"]').serializeArray().map(function (item) {
        DefArr.push(item.value);
    });
    CheckSessionStatus().then(function (answer) {
        if (answer) {
            $.ajax({
                type: "POST",
                url: window.location.url,
                data: { 'ClassMonth': $('#ClassMonth').val(), 'PsnNo': $('#PsnNo').val(), 'PageEvent': 'CreateClass', 'ClassNoDef': DefArr.join(",") },
                //contentType: "application/json",
                dataType: "json",
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
                data: { 'ClassMonth': $('#ClassMonth').val(), 'PageEvent': 'QueryData', 'PsnNo': $('#PsnNo').val(),"DeptList":$("#DeptList").val() },
                success: function (data) {
                    if ($(data).find("#LoginSorryArea").length > 0) {
                        window.location.href = "/Default.aspx";
                    }
                    $("#ScrollArea").html($(data).find("#ScrollArea").html());
                    $.unblockUI();
                }
            });
        }
    });
  
}

function DoPrint() {
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
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