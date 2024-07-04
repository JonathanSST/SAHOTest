$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
    SetUserLevel();
    $("#BtnModify").click(function () {
        //console.log($("#ContentPlaceHolder1_tablePanel").find("input,select").serialize() + "&PageEvent=ModifyAbnormal");
        $.ajax({
            type: "POST",
            url: window.location.href,
            data: $("#ContentPlaceHolder1_tablePanel").find("input,select").serialize() + "&PageEvent=ModifyAbnormal",
            success: function (data) {
                alert("更新完成!!");
            }
        }); 
    });
    $("#BtnSetting").click(function(){
        $("#ParaExtDiv1").css("left", ($(document).width() - $("#ParaExtDiv1").width()) / 2);
        $("#ParaExtDiv1").css("top", $(document).scrollTop() + 50);
        $("#ParaExtDiv1").css("display", "block");
        $("#popOverlay1").css("display", "block");        
    });
    $("#BtnAlive").click(function () {
        $("#ParaExtDiv1").css("display", "none");
        $("#popOverlay1").css("display", "none");        
    });
    if ($("#IsSsl").val() == "1") {
        $("#UseSsl").prop("checked", "checked");
    }
    $("#BtnSave").click(function () {
        //console.log($("#ParaExtDiv1").find("input,select").serialize() + "&PageEvent=Save");        
        $.ajax({
            type: "POST",
            url: window.location.href,
            data: $("#ParaExtDiv1").find("input,select").serialize()+"&PageEvent=Save",
            success: function (data) {
                $("#ParaExtDiv1").css("display", "none");
                $("#popOverlay1").css("display", "none");        
            }
        });        
    });
    $("#PsnName").dblclick(function () {
        let data = $("#PsnName").val();
        /****
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
                        //console.log($(this).html());
                        $("#PsnName").val($(this).find("#NameSpan").text());
                        $("#PsnNo").val($(this).find("#HiddenPsnNo").val());
                        $("#PersonListArea").css("display", "None");
                    });
                });

            }
        });
        ***/
    });
});

function SetCheckAll(obj) {
    $('input[name*="ChkOne"]').prop('checked', $(obj).prop("checked"));
}

function chkChange() {
    if ($('input[name*="ChkOne"]:checked').length === $('input[name*="ChkOne"]').length)
    {
        $('input[type=checkbox][name=ChkAll]').prop("checked", true);
    }
    else
    {
        $('input[type=checkbox][name=ChkAll]').prop("checked", false);
    }
}


function CancelOneLogArea() {
    $("#OneLogArea").css("display", "none");
    $("#popOverlay").css("display", "none");
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

function SetQuery() {
    //$("#PageEvent").val("Query");
    var CardDateS = $("#CardDateS").val();
    var CardDateE = $("#CardDateE").val();
    var PsnName = $("#PsnName").val();
    var IsSend = $("#dllIsSend").val();
    var PsnNo = $("#PsnNo").val();

    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "PageEvent": "Query",
            "CardDateS": CardDateS,
            "CardDateE": CardDateE,
            "PsnName": PsnName,
            "PsnNo": PsnNo,
            "IsSend": IsSend,
            'PageIndex': $("#PageIndex").val()
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
        url: window.location.href,
        data: { "RecordID": recordid, "PageEvent": "QueryOneLog" },
        dataType: "json",
        success: function (data) {
            //console.log(data);
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
    //console.log(DoEdit);
    if (DoEdit == false) {
        //console.log(DoEdit);
        $('[name*="BtnShowImage"]').prop('disabled', true);

    }
}

function Send() {
    var Persons = "";
    if ($('input[name*="ChkOne"]:checked').length == 0) {
        alert('請勾選需發送人員');
        return false;
    }

    var arr = new Array();
    $('[name*="ChkOne"]').each(function (i) {
        if ($(this).prop("checked") == true) {
            Persons += "," + $(this).val();
        }
    });
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "PageEvent": "SendMail",
            "PersonList": Persons
        },
        dataType: "json",
        success: function (data) {
            if (data.message == "OK") {
                alert('發送成功!!');
            }
            else {
                alert(data.message);
            }


        }
    });
    SetQuery();
}