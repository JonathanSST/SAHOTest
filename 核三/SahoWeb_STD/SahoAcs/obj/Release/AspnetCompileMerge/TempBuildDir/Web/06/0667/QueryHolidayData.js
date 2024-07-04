$(document).ready(function () {
    $("#PrintButton").click(function () {
        SetPrint();
    });
});

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

function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {
        $('#QryTimeS').val($('[name*="QueryTimeS"]').val());
        $('#QryTimeE').val($('[name*="QueryTimeE"]').val());
    }
    ShowPage(1);
}



function SetPrint() {
    $('#QryTimeS').val($('[name*="QueryTimeS"]').val());
    $('#QryTimeE').val($('[name*="QueryTimeE"]').val());
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}

function SetQuery() {
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: location.href,
        //dataType: 'json',
        //data: $("#form1").find("input,select").serialize(),
        data: {
            'PageEvent': 'Query',
            'PsnNo': $('#PsnNo').val(),
            'QryTimeS': $('[name*="QueryTimeS"]').val(),
            'QryTimeE': $('[name*="QueryTimeE"]').val(),
            'PageIndex': $("#PageIndex").val()
        },
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            //CheckMax();
            //BindEvent();
            $.unblockUI();
        }
    });
}

function BindEvent() {
    $('.GVStyle').find('th').each(function (index) {
        if (index < $('.GVStyle').find('th').length - 1) {
            $(this).css("width", $(this).find('[name*="TitleCol"]').val());
        }
    });
}