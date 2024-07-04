//var SelectValue = 0;
$(document).ready(function () {
    BindEvent();
    if($("#PsnID").val()!="")
    {
        $(".ShowPsnInfo").css("display", "none");
    }
});


function PopURL(obj) {
   
}


function SetAdd() {
    $.post("AddNews.aspx", { "PageEvent": "Add"},
    function (data) {
        OverlayContent(data);
        $("#NewsTitle").focus();
        $("#BtnSave").click(function () {
            SaveData();
        });
        $("#BtnCancel").click(function () {
            DoCancel();
        });
    });
}

function SetEdit() {
    if ($('input[name*="SelectValue"]').val() != "")
    {
        $.post("AddNews.aspx", { "PageEvent": "Edit", "NewsID":$('input[name*="SelectValue"]').val() },
           function (data) {
               OverlayContent(data);
               $("#NewsTitle").focus();
               $("#BtnSave").click(function () {
                   SaveData();
               });
               $("#BtnCancel").click(function () {
                   DoCancel();
               });
        });
    } else {
        alert("請選擇要編輯的公告訊息");
    }
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
    $.post("AddNews.aspx", { "PageEvent": "Save", "NewsID": $("#NewsID").val(), "NewsTitle": $("#NewsTitle").val(), "NewsContent": $("#NewsContent").val(), "NewsDate": $('input[name*="NewsDate"]').val() },
   function (data) {
       SetQuery();
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
    $("#DateS").val($('input[name*="CardTimeSDate"]').val());
    $("#DateE").val($('input[name*="CardTimeEDate"]').val());
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}


function SetQuery() {    
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: "NewsInfo.aspx",
        //dataType: 'json',
        data: {
            "DateS": $('input[name*="CardTimeSDate"]').val(), "DateE": $('input[name*="CardTimeEDate"]').val(),"PageEvent": "Query"
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


function ShowData(val1)
{
    $.post("ShowNews.aspx", { "NewsID": val1,"PageEvent":"Edit"},
    function (data) {
        OverlayContent(data);
        //$("#NewsTitle").focus();        
        $("#BtnCancel").click(function () {
            DoCancel();
        });
    });
}



function SetUserLevel() {
    var funcs = $("#FunAuthSet").val();
    var data = funcs.split(',');
    $("#BtnEdit").hide();
    $("#BtnQuery").hide();
    $("#BtnExport").hide();
    $("#ButtonAdd").hide();
    for (var i = 0; i < data.length; i++) {
        if (data[i] == "Enter") {            
            $("#BtnQuery").show();
            $("#BtnExport").show();
        }
        if (data[i] == "Auth") {
            $("#BtnEdit").show();            
            $("#ButtonAdd").show();
        }

    }
}