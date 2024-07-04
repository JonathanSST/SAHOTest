$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
    SetUserLevel();
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
        $("#DateS").val($('[name*="_CardDateS"]').val());        
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
    $("#DateS").val($('[name*="$CardDateS$"]').val());
    $("#DateE").val($('[name*="$CardDateE$"]').val());
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}

function SetOpenLocation() {

}


function SetQuery() {    
    $("#DateS").val($('[name*="$CardDateS$"]').val());
    $("#DateE").val($('[name*="$CardDateE$"]').val());
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        //dataType: 'json',
        data: $("#form1").find("input,select").serialize(),
        success: function (data) {
            console.log(data);
            $("#UpdatePanel1").html($(data).find("#UpdatePanel1").html());
            //BindEvent();
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



function SaveNoteList() {
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: $('#ContentPlaceHolder1_tablePanel').find('input').serialize()+"&PageEvent=Save",
        dataType: "json",
        success: function (data) {
            alert(data.message);
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

