$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
    //SetUserLevel();
    $("#ClassMonth").click(function () {
        ShowMonthDate($(this)[0]);
    })

    $("#PsnName").dblclick(function () {
        let data = $("#PsnName").val();
        $.ajax({
            type: "POST",
            url: window.location.href,
            //dataType: 'json',
            data: { "PageEvent": "QueryPerson", "PsnName": data },
            success: function (data) {
                $("#PersonListArea").html($(data).find("#PersonListArea").html());
                $("#PersonListArea").css("display", "block");
                adjustAutoLocation($("#PersonListArea")[0], $("#PsnName")[0], 20, 20);
                $("#PersonListArea").find(".PsnArea").each(function () {
                    $(this).click(function () {
                        //console.log($(this).html());
                        $("#PsnName").val($(this).find("#NameSpan").text());
                        $("#PsnNo").val($(this).find("#HiddenPsnNo").val());
                        $("#PersonListArea").css("display", "None");
                    });
                });

            }
        });
    });

    $('#BtnQuery').click(function () {
        $("#DefMonth").val($("#ClassMonth").val());
        $("#PsnName").val($(this).find("#NameSpan").text());
        $("#PsnNo").val($(this).find("#HiddenPsnNo").val());
        DoRefresh();
    });
});

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

function InputYearMonth(year, month) {
    if (month.length == 1) {
        month = "0" + month;
    }
    $("#ClassMonth").val(year + "/" + month);
    $("#dvContent").html("");
    $("#DefMonth").val($("#ClassMonth").val());
    //DoRefresh();
}


function CancelOneLogArea() {
    $("#OneLogArea").css("display", "none");
    $("#popOverlay").css("display", "none");
}

function SetSort(sort_name) {
    $("#SortName").val(sort_name);
    if ($("#SortType").val() == "ASC") {
        $("#SortType").val("DESC");
    } else {
        $("#SortType").val("ASC");
    }
    DoRefresh();
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

function DoRefresh() {
    
    //if ($('#PsnNo').val() == "") {
    //    alert('請設定人員姓名!!');
    //    return false;
    //}

    if ($("#ClassMonth").val() == "") {
        alert('請設定開始指定月份!!');
        return false;
    }
  
    CheckSessionStatus().then(function (answer) {
        if (answer) {
            Block();
            
            $.ajax({
                type: "POST",
                url: window.location.href,
                //dataType: 'json',
                data: {
                    'ClassMonth': $('#ClassMonth').val(),
                    'PageEvent': 'QueryData',
                    //'PsnNo': $('#PsnNo').val(),
                    'PsnName':$("#PsnName").val(),
                    'DeptList': $("#DeptList").val(),
                    'PageIndex': $("#PageIndex").val()
                },
                success: function (data) {
                    $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
                    BindEvent();
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

function ShowPage(page_index) {
    $("#PageIndex").val(page_index);
    DoRefresh();
}



function SetPrint() {
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
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

function SetShowOneLog(recordid) {
    $.ajax({
        type: "POST",
        url: "QueryMobileLog.aspx",
        data: { "RecordID": recordid, "PageEvent": "QueryOneLog" },
        dataType: "json",
        success: function (data) {
            console.log(data);
            $("#ShowEmpID").text(data.card_log.EmpID);
            //var date = new Date(parseInt(data.card_log.CardTime.substr(6)));        
            $("#ShowCardTime").text(data.card_time);
            $("#LogPic").prop("src", data.PsnPicSource);
            $("#popOverlay").width($(document).width());
            $("#popOverlay").height($(document).height());
            $("#popOverlay").css("display", "block");
            $("#OneLogArea").css("display", "block");
            $("#popOverlay").css("background", "#000");
            $("#popOverlay").css("opacity", "0.5");
            $("#OneLogArea").css("left", 0);
            $("#OneLogArea").css("top", 0);
            $("#OneLogArea").css("left", ($(document).width() - $("#OneLogArea").width()) / 2);
            $("#OneLogArea").css("top", $(document).scrollTop() + 50);
        }, fail: function () {
            console.log("error");
        }
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
    console.log(DoEdit);
    if (DoEdit == false) {
        console.log(DoEdit);
        $('[name*="BtnShowImage"]').prop('disabled', true);
    }
}

function CancelOneLogArea() {
    $("#OneLogArea").css("display", "none");
    $("#popOverlay").css("display", "none");
    $("#map").remove();
    $("#ShowLogMap").css("display", "none");
}
