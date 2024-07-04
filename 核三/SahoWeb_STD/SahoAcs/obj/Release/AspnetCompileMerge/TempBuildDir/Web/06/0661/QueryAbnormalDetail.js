$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
    SetUserLevel();
    $("#PsnName").dblclick(function () {
        let data = $("#PsnName").val();
        $.ajax({
            type: "POST",
            url: window.location.href,
            //dataType: 'json',
            data: {
                "PageEvent": "QueryPerson",
                "PsnName": data
            },
            success: function (data) {
                $("#PersonListArea").html($(data).find("#PersonListArea").html());
                $("#PersonListArea").css("display", "block");
                adjustAutoLocation($("#PersonListArea")[0], $("#PsnName")[0], 20, 20);
                $("#PersonListArea").find(".PsnArea").each(function () {
                    $(this).click(function () {
                        console.log($(this).html());
                        $("#PsnName").val($(this).find("#NameSpan").text());
                        $("#PsnNo").val($(this).find("#HiddenPsnNo").val());
                        $("#PersonListArea").css("display", "None");
                    });
                });

            }
        });
    });
});


function CancelOneLogArea() {
    $("#OneLogArea").css("display", "none");
    $("#popOverlay").css("display", "none");
}




function PopURL(obj) {

}



function SetMode(mode) {
    $("#QueryMode").val(mode);
    //進行查詢處理
    if (mode == 1) {
        $("#CardDateE").val($('[name*="CardDayE"]').val());
        $("#CardDateS").val($('[name*="CardDayS"]').val());
        $("#PsnName").val($('[name*="PsnName"]').val());
        ShowPage(1);
    }
    
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
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}

function SetOpenLocation() {

}


function SetQuery() {
    //$("#PageEvent").val("Query");
    var CardDateS = $("#CardDateS").val();
    var CardDateE = $("#CardDateE").val();
    var PsnName = $("#PsnName").val();
    Block();
    $.ajax({
        type: "POST",
        url: "QueryWorkAbnormal.aspx",
        data: {
            "PageEvent": "Query",
            "CardDateS": CardDateS,
            "CardDateE": CardDateE,
            "PsnName": PsnName
        },
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
            $.unblockUI();
            SetUserLevel();
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
