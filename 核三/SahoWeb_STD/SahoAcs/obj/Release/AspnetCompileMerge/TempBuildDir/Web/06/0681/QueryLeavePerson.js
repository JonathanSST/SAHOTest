$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
/*    SetRefresh();*/
});


//設定進階查詢功能
function SetAdvArea() {
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $("#popOverlay").css("display", "block");
    $("#AdvanceArea").css("display", "block");
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#AdvanceArea").css("left", 0);
    $("#AdvanceArea").css("top", 0);
    $("#AdvanceArea").css("left", ($(document).width() - $("#AdvanceArea").width()) / 2);
    $("#AdvanceArea").css("top", $(document).scrollTop() + 50);
}

function CancelAdvArea() {
    $("#AdvanceArea").css("display", "none");
    $("#popOverlay").css("display", "none");
}

//function SetShowCardLogDetail() {
//    $("#popOverlay").width($(document).width());
//    $("#popOverlay").height($(document).height());
//    $("#popOverlay").css("display", "block");
//    $("#AdvanceArea").css("display", "block");
//    $("#popOverlay").css("background", "#000");
//    $("#popOverlay").css("opacity", "0.5");
//    $("#AdvanceArea").css("left", 0);
//    $("#AdvanceArea").css("top", 0);
//    $("#AdvanceArea").css("left", ($(document).width() - $("#AdvanceArea").width()) / 2);
//    $("#AdvanceArea").css("top", $(document).scrollTop() + 50);
//}

//function CancelOneLogArea() {
//    $("#OneLogArea").css("display", "none");
//    $("#popOverlay").css("display", "none");
//}




//function PopURL(obj) {

//}

//function SetRefresh() {
//    $.ajax({
//        type: "POST",
//        url: "../0639/CountDateDefine.aspx",
//        dataType: 'json',
//        data: { "PageEvent": "GetCountData" },
//        success: function (data) {
//            console.log(data);
//            $("#OutCount").text(data.today_count);
//            $("#OutTotalCount").text(data.total_count);
//        }
//    });
//}

function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {
        $("#PsnETime").val($('[name*="CalendarE"]').val());
        $("#PsnSTime").val($('[name*="CalendarS"]').val());
    }
    else {
        $("#PsnSTime").val($('[name*="ADVCalendar_PsnSTime"]').val());
        $("#PsnETime").val($('[name*="ADVCalendar_PsnETime"]').val());

        var arr = new Array();
        $('[name*="$ADVDropDownList_Com"]').each(function (i) {
            if ($(this).prop("checked") == true) {
                arr.push($(this).val());
            }
        });
        $("#ComID").val(arr.join(','));

        var arr2 = new Array();
        $('[name*="ADVDropDownList_Dep"]').each(function (i) {
            if ($(this).prop("checked") == true) {
                arr2.push($(this).val());
            }
        });
        $("#DepID").val(arr2.join(','));

        var arr3 = new Array();
        $('[name*="ADVDropDownList_Tit"]').each(function (i) {
            if ($(this).prop("checked") == true) {
                arr3.push($(this).val());
            }
        });
        $("#TitID").val(arr3.join(','));

        $("#AdvanceArea").css("display", "none");
        $("#popOverlay").css("display", "none");
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


function SetPrintExcel() {
    $("#PageEvent").val("PrintExcel");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}

//function SetPrintPDF() {
//    $("#PageEvent").val("PrintPDF");
//    $("#form1").attr("target", "_blank");
//    $("#form1").submit();
//}


function SetQuery() {
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: "QueryLeavePerson.aspx",
        //dataType: 'json',
        data: $("#form1").find("input,select").serialize(),
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
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
    $('.DataRow').each(function (outIndex) {
        var that = $(this);
        $(that).find('td').each(function (index) {
            if (index < $(that).find('td').length - 1) {
                $(this).css("width", $(this).find('[name*="DataCol"]').val());
            }
        });
    });
}


//function dragElement(elmnt) {
//    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
//    console.log(elmnt.id);
//    if (document.getElementById(elmnt.id + "header")) {
//        // if present, the header is where you move the DIV from:
//        document.getElementById(elmnt.id + "header").onmousedown = dragMouseDown;
//    } else {
//        // otherwise, move the DIV from anywhere inside the DIV:
//        elmnt.onmousedown = dragMouseDown;
//    }

//    function dragMouseDown(e) {
//        e = e || window.event;
//        e.preventDefault();
//        // get the mouse cursor position at startup:
//        pos3 = e.clientX;
//        pos4 = e.clientY;
//        document.onmouseup = closeDragElement;
//        // call a function whenever the cursor moves:
//        document.onmousemove = elementDrag;
//    }

//    function elementDrag(e) {
//        e = e || window.event;
//        e.preventDefault();
//        // calculate the new cursor position:
//        pos1 = pos3 - e.clientX;
//        pos2 = pos4 - e.clientY;
//        pos3 = e.clientX;
//        pos4 = e.clientY;
//        // set the element's new position:
//        elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
//        elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
//    }

//    function closeDragElement() {
//        // stop moving when mouse button is released:
//        document.onmouseup = null;
//        document.onmousemove = null;
//    }
//}