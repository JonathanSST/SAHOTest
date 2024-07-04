var twds = "";

function SetLineInit() {
    $("div[id='toolPanel']").draggable({
        containment: "#box",
        scroll: false
    });

    $("div[id='dragx']").draggable({
        containment: "#box",
        scroll: false,
        stop: function (event, ui) {
            //$("#toolPanel").css("left", $("#dragx").css("left"));
            //$("#toolPanel").css("top", parseInt($("#dragx").css("top")) + 840);
            if ($("#BtnStoryS").prop("disabled")) {
                var g_left = parseInt($("#dragx").css("left")) + 23;
                var g_top = 768 + parseInt($("#dragx").css("top")) + 27;
                $("#DataList").append(g_left + "," + g_top + "<br/>");
                if (twds != "") {
                    twds += "-";
                }
                twds += g_left + "," + g_top;
                $("#company").attr("src", "MapLine.ashx?Pic=" + $("#PicID").val() + "&EquNoS=" + $("#EquNoS").val() + "&EquNoE=" + $("#EquNoE").val() + "&twds=" + twds);
            }
        }
    });

    $("#BtnStoryS").click(function () {
        var startEqu = $("#EquNoS").val();
        var pointX = parseInt($("#" + startEqu + "X").val()) - 23;
        var pointY = parseInt($("#" + startEqu + "Y").val()) - 27 - 768;
        $("#dragx").css("top", pointY);
        $("#dragx").css("left", pointX);
        $("#company").attr("src", "MapLine.ashx?Pic=" + $("#PicID").val() + "&EquNoS=" + $("#EquNoS").val() + "&EquNoE=" + $("#EquNoE").val());
        $(this).prop("disabled", true);
        $("#BtnStoryE").prop("disabled", false);
        var g_left = parseInt($("#dragx").css("left")) + 23;
        var g_top = 768 + parseInt($("#dragx").css("top")) + 27;
        twds = g_left + "," + g_top;
    });


    $("#BtnStoryE").click(function () {
        var Date0 = new Date().getTime();
        $("#company").attr("src", "Map.ashx?Pic=" + $("#PicID").val() + "&now=" + Date0);
        $(this).prop("disabled", true);
        $("#BtnStoryS").prop("disabled", false);
    });

    $("#BtnSave").click(function () {
        $("#company").attr("src", "Map.ashx?Pic=" + $("#PicID").val());
        $("#BtnStoryS").prop("disabled", false);
        $("#BtnStoryE").prop("disabled", true);
        //透過ajax產生儲存地圖路線
        $.ajax({
            type: "POST",
            url: "DragLine.aspx",
            data: { "EquStart": $("#EquNoS").val(), "EquEnd": $("#EquNoE").val(), "PicID": $("#PicID").val(), "LineRoute": twds, "PageEvent": "SaveLine" },
            success: function (data) {

            }
        });
    });

    $("#BtnShowLine").click(function () {
        var Date0 = new Date().getTime();
        $("#company").attr("src", "MapShow.ashx?PicID=" + $("#PicID").val() + "&EquNoS=" + $("#EquNoS").val() + "&EquNoE=" + $("#EquNoE").val() + "&now=" + Date0);
    });

    $("#BtnShowMap").click(function () {
        var Date0 = new Date().getTime();
        $("#company").attr("src", "Map.ashx?Pic=" + $("#PicID").val() + "&now=" + Date0);
    });

    $("#BtnMain").click(function () {
        $("#popOverlay").remove();
        $("#ParaExtDiv").remove();
    });
}