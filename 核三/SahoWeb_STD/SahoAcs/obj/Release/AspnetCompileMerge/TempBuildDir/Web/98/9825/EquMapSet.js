$(document).ready(function () {
    MainListInit();
});


function AddMap() {
    $.post("EquMapUpload.aspx",
        {},
        function (data) {
            OverlayContent(data);
            SetInit();
        });
}

function SetInit() {
    $("#PicDesc").focus();
    $("#Uploadfile").bind("change", function (event) {
        if (event.target.files.length > 0) {
            $("div[data-id='fileContainer'] > ul").empty();
            for (var i = 0; i < event.target.files.length; i++) {
                var file = event.target.files[i];
                $("div[data-id='fileContainer'] > ul").append(
                    $("<li style='padding: 3px' />").html("檔名: " + file.name + "<br /> 大小: " + file.size + "<br /> 類型: " + file.type)
                );
            }
        }
        else {
            $("div[data-id='fileContainer'] > ul").empty();
        }
    });

    if ($("#PicID").val() != "0") {
        $("#PageEvent").val("Update");
    }

    $("#btnFileUpload").click(function () {
        var form_data = new FormData($('#MapForm')[0]);      
        $.ajax({
            url: 'EquMapUpload.aspx',
            type: 'POST',
            xhr: function () {
                var myXhr = $.ajaxSettings.xhr();
                if (myXhr.upload) { // Check if upload property exists
                    myXhr.upload.addEventListener('progress', progressHandlingFunction, false); // For handling the progress of the upload
                }
                return myXhr;
            },
            success: function (data) {
                alert("更新完成!!");
                DoCancel();
                SetReQuery();
            },
            data: form_data,
            cache: false,
            contentType: false,
            processData: false
        });
        
    });
}

function SetReQuery() {
    //$.post("EquMapSet.aspx",
    //    {},
    //    function (data) {
    //        $("#DataResult").html($(data).find("#DataResult").html());
    //        MainListInit();
    //    });
    parent.location.reload();
}

$("#btnCancel").click(function () {
    DoCancel();
});

function DoCancel() {
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
}

function MainListInit() {
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
        var that = $(this);
        $(this).find("#BtnEdit").click(function () {
            $.post("EquMapUpload.aspx",
                { "PicID": $(that).find("#EditID").val(), "PageEvent": "Edit" },
                function (data) {
                    OverlayContent(data);
                    SetInit();
                });
        });

        $(this).find("#BtnMapEdit").click(function () {
            $.post("EquMapSet2.aspx",
                { "PicID": $(that).find("#EditID").val(), "PageEvent": "Edit" },
                function (data) {
                    OverlayContent(data);
                    SetPointInit();
                });
        });
    });
}

function EditMap(PicID) {
    $.ajax({
        type: "POST",
        url: "QueryCardlog.aspx",
        data: { "RecordID": PicID, "PageEvent": "QueryOneLog" },
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
            if (data.card_log.CardPicPath != null && data.card_log.CardPicPath != "")
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

            var d = new Date(formatted);
            var yr = d.getFullYear();
            var mon = d.getMonth();
            var dt = d.getDate();
            var hr = d.getHours();
            var mins = d.getMinutes();
            var sc = d.getSeconds();
            //var utcDate = new Date(Date.UTC(yr, mon, dt, hr, mins, sc));
            var utcDate = new Date(yr, mon, dt, hr, mins, sc);
            var mms = Date.parse(utcDate); //"1586281576000";
            $("#EVOYN").val("1");
            bindurl(data.card_log.CamNo, mms, $("#EVOYN").val());
        },
        fail: function () {
            console.log("error");
        }
    });
}

function progressHandlingFunction(e) {
    if (e.lengthComputable) {
        $('progress').attr({ value: e.loaded, max: e.total });
    }
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
    $("#popOverlay").height($(document).height());
}


function ShowPage(page_index) {
    $("#PageIndex").val(page_index);
    SetReQuery();
}
