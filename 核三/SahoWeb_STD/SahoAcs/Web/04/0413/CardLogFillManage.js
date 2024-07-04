$(document).ready(function () {
    BindEvent();
    if($("#PsnID").val()!="")
    {
        $(".ShowPsnInfo").css("display", "none");
    }
});


function PopURL(obj) {
   
}

function QueryPsnCard() {
    var txtObj = $("#PsnNo")[0];
    $.post('PsnCardList.aspx', { 'PsnNo': $("#PsnNo").val() }, function (data) {        
        $("#PsnListContent").html(data);
        var mainDiv = document.getElementById("PsnListContent");
        //var objTop = cumulativeOffset(txtObj)[1];
        //var objLeft = cumulativeOffset(txtObj)[0];
        //$("#PsnListContent").css("left", objLeft - $("#ParaExtDiv").css("left"));
        //$("#PsnListContent").css("top", objTop - 25);
        adjustAutoLocation(mainDiv, txtObj, 44, 42);
    });
}

function InputPerson(val1, val2, val3) {
    $("#PsnNo").val(val1);
    $("#PsnName").val(val2);
    $("#CardID").val(val3);
    $("#PsnListContent").html("");
}

function SetAdd() {
    $.post("AddCourseLog.aspx", { "PageEvent": "Add", "DateS": $('input[name*="CardTimeSDate"]').val(), "DateE": $('input[name*="CardTimeEDate"]').val() },
    function (data) {
        OverlayContent(data);
        $("#PsnNo").focus();
        $("#BtnSave").click(function () {
            SaveData();
        });
        $("#BtnCancel").click(function () {
            DoCancel();
        });
    });
}

function SaveData() {
    if ($("#CardID").val() == "") {
        alert("未選擇卡號!!");
        return false;
    }
    if ($("#CourseName").val() == "0") {
        alert("未選擇課程!!");
        return false;
    }
    $.post("AddCourseLog.aspx", { "PageEvent": "Save","CardID":$("#CardID").val(),"CourseID":$("#CourseName").val()},
   function (data) {
       DoCancel();
   });
}

function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {
        $("#CardTimeE").val($('[name*="$Calendar_CardTimeEDate"]').val());
        $("#CardTimeS").val($('[name*="$Calendar_CardTimeSDate"]').val());
        $("#DepID").val($('[name*="$ddlDept"]').val());
    } else {
        
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
    //$("#CardTimeE").val($('[name*="Calendar_CardTimeEDate"]').val());
    //$("#CardTimeS").val($('[name*="Calendar_CardTimeSDate"]').val());
    //$("#form1").submit();    
    //window.open("0601Rpt.aspx?" + $(".Content").find("input,select").not('[name*="DataCol"]').serialize());
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}


function SetQuery() {    
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: "CardLogFillManage.aspx",
        //dataType: 'json',
        data: {
            "DateS": $('input[name*="CardTimeSDate"]').val(), "DateE": $('input[name*="CardTimeEDate"]').val(),
            "PageEvent": "Query", "PsnNo": $("#QueryNo").val(), "SortType": $("#SortType").val(), "SortName": $("#SortName").val()
        },
        success: function (data) {
            //alert(data);
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
    $(".GVStyle").sortable({
        opacity: 0.6,    //拖曳時透明
        cursor: 'pointer',  //游標設定
        axis: 'x,y',       //只能垂直拖曳
        update: function () {            
            SetQuery();
        }
    });
}




function OverlayContent(data) {
    $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
              + ' -webkit-transform: translate3d(0,0,0);"></div>');
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
          + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
    $("#ParaExtDiv").html(data);
    $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
    $(document).scrollTop(0);
    $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
}

function DoCancel() {
    $("#PsnListContent").html("");
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
}

