var CountRealPsnList = false;

$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }

    SetOpenAreaInfo();
    $("#BtnOpen").click(function () {
        SetOpenAreaInfo();
    });
});

function SetOpenAreaInfo() {
    $(document).ready(function () {
        $.post("AreaInfo.aspx", {
        },
            function (data) {
                $(document.body).append('<div id="popOverlay2" style="position:absolute; top:0; left:0; z-index:30000; overflow:hidden;'
                    + ' -webkit-transform: translate3d(0,0,0);"></div>');
                $("#popOverlay2").css("background", "#000");
                $("#popOverlay2").css("opacity", "0.5");
                $("#popOverlay2").width($(document).width());
                $("#popOverlay2").height($(document).height());
                $(document.body).append('<div id="ParaExtDiv2" style="position:absolute;z-index:30001;background-color:white;'
                    + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                $("#ParaExtDiv2").html(data);
                $("#ParaExtDiv2").css("left", 90);
                $(document).scrollTop(0);
                $("#ParaExtDiv2").css("top", $(document).scrollTop() + 50);
            });
    });
}






function SetShowOneLog(recordid) {
    $.ajax({
        type: "POST",
        url: "QueryCardlog.aspx",
        data: { "RecordID": recordid, "PageEvent": "QueryOneLog" },
        dataType: "json",
        success: function (data) {
            console.log(data);
            $("#ShowPsnNo").text(data.card_log.PsnNo);
            $("#ShowCardNo").text(data.card_log.CardNo);
            $("#ShowEquNo").text(data.card_log.EquNo);
            $("#HeatResult").text(data.card_log.HeatResult);
            var date = new Date(parseInt(data.card_log.CardTime.substr(6)));
            var formatted = date.getFullYear() + "/" +
                  ("0" + (date.getMonth() + 1)).slice(-2) + "/" +
                  ("0" + date.getDate()).slice(-2) + " " + ("0" + date.getHours()).slice(-2) + ":" +
                  ("0" + date.getMinutes()).slice(-2) + ":" + ("0" + date.getSeconds()).slice(-2);
            $("#ShowCardTime").text(formatted);

            $("#PsnPic").prop("src", data.PsnPicSource);
            if (data.card_log.CardPicSource != null && data.card_log.CardPicSource != "")
                $("#LogPic").prop("src", "data:image/png;base64, " + data.card_log.CardPicSource);
            if (data.card_log.CardPicPath != null && data.card_log.CardPicPath !="")
                $("#LogPic").prop("src", data.card_log.CardPicPath);
            
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
        },
        fail: function () {
            console.log("error");
        }
    });

    
}



function PopURL(obj) {
   
}



function SetMode(mode) {
    $("#QueryMode").val(mode);    
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
        data: $("#form1").find("input,select").serialize(),
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
            $.unblockUI();
        }
    });
}


///設定拖曳功能設定
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
    //$(".GVStyle").sortable({
    //    opacity: 0.6,    //拖曳時透明
    //    cursor: 'pointer',  //游標設定
    //    axis: 'x,y',       //只能垂直拖曳
    //    update: function () {            
    //        SetQuery();
    //    }
    //});
}



