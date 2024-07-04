$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
    //SetUserLevel();
    $("#PsnName").dblclick(function () {
        let data = $("#PsnName").val();
        /**
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
        });     **/
    });

    $("#BtnSave").click(function () {
        if ($('[name*="WorkDesc"]').length > 0){
            Block();
            $.ajax({
                type: "POST",
                url: location.href,
                data: $("#DivSaveArea").find('input').serialize() + "&PageEvent=Save",
                dataType: "json",
                success: function (data) {
                    alert(data.message);
                    $.unblockUI();
                }
            });
        }
      

    });
});

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
    }
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


function SetPrint() {
    $("#PageEvent").val("Print");
    var PsnNo = $("#PsnNo").val();
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}


function SetQuery() {
    $("#PageEvent").val("Query");
    var PsnName = $("#PsnName").val();
    var PsnNo = $("#PsnNo").val();
    var DateS = $("#CardDateS").val();
    var DateE = $("#CardDateE").val();
    var DeptList = $("#DeptList").val();
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        //dataType: 'json',
        //data: $("#form1").find("input,select").serialize(),
        data: {
            "PageEvent": "Query",
            "PsnName": PsnName,
            "PsnNo": PsnNo,
            "DateS": DateS,
            "DateE": DateE,
            "DeptList": DeptList,
            'PageIndex': $("#PageIndex").val()
        },
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
            $.unblockUI();
            //SetUserLevel();
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
        console.log(DoEdit);
        $('[name*="BtnShowImage"]').prop('disabled', true);
        //$('.TableS1 th:eq(6)').html('');
        //$('.TableS1 th:eq(6)').remove();
        //$('.TableS1 th:eq(5)').css('width', '');
        //$("#ContentPlaceHolder1_MainGridView tr").each(function () {
        //    $(this).find('td:eq(6)').remove();
        //    $(this).find('td:eq(5)').css('width', '');
        //});
    }
}

function CancelOneLogArea() {
    $("#OneLogArea").css("display", "none");
    $("#popOverlay").css("display", "none");
    $("#map").remove();
    $("#ShowLogMap").css("display", "none");
}