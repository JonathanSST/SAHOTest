$(document).ready(function () {
    MainListInit();
});


function AddMap() {
    $.post("MapEdit.aspx",
           {},
           function (data) {
               OverlayContent(data);
               SetInit()
           });
}


function DoCancel() {
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
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

    $("#btnSave").click(function () {
        var form_data = new FormData($('#MapForm')[0]);
        $.ajax({
            url: 'MapEdit.aspx',
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

    $("#btnCancel").click(function () {
        DoCancel();
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


function SetReQuery() {
    $.post("MapList.aspx",
          {},
          function (data) {
              $("#DataResult").html($(data).find("#DataResult").html());
              MainListInit();
          });
}

function ShowLine(picid, start, end) {
    window.open("ShowLine.ashx?Pic="+picid+"&EquNoS="+start+"&EquNoE="+end)
}


function MainListInit() {
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
        var that = $(this);
        $(this).find("#BtnEdit").click(function () {
            $.post("MapEdit.aspx",
           { "PicID": $(that).find("#EditID").val(), "PageEvent": "Edit" },
           function (data) {
               OverlayContent(data);
               SetInit()
           });
        });
        $(this).find("#BtnPoint").click(function () {
            $.post("DragPoint.aspx",
           { "PicID": $(that).find("#EditID").val(), "PageEvent": "Edit" },
           function (data) {
               OverlayContent(data);
               SetPointInit();
           });
        });
        $(this).find("#BtnLine").click(function () {
            $.post("DragLine.aspx",
            { "PicID": $(that).find("#EditID").val(), "PageEvent": "Edit" },
              function (data) {
                  OverlayContent(data);
                  SetLineInit();
              });
        });
        $(this).find("#BtnRoute").click(function () {
            $.post("RouteList.aspx",
            { "PicID": $(that).find("#EditID").val(), "PageEvent": "Edit" },
              function (data) {
                  OverlayContent(data);
                  //SetLineInit();
              });
        });
    });
}