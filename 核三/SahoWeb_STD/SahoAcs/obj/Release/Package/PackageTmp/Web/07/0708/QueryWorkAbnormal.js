$(document).ready(function () {
    $("#BtnSave").click(function () {
        Block();
        $.ajax({
            type: "POST",
            url: location.href,
            data: $("#DivSaveArea").find('input').serialize() + "&PageEvent=Save",
            dataType: "json",
            success: function (data) {
                $.unblockUI();
                alert(data.message);
                //console.log(data);
            }
        });
    });

    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
});

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
    $("#CardDateE").val($('[name*="CardDayE"]').val());
    $("#CardDateS").val($('[name*="CardDayS"]').val());
    $('#Company').val($('#ContentPlaceHolder1_dropCompany').val());
    $('#Department').val($('#ContentPlaceHolder1_dropDepartment').val());
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}

function SetQuery() {
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "CardDateS": $('[name*="CardDayS"]').val(),
            "CardDateE": $('[name*="CardDayE"]').val(),
            "PageEvent": "Query",
            "Company": $('#ContentPlaceHolder1_dropCompany').val(),
            "Department": $('#ContentPlaceHolder1_dropDepartment').val(),
            "PageIndex": $("#PageIndex").val()
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

