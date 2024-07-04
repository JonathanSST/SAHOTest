function SetPointInit() {
    $("div[id='dragx']").draggable({
        containment: "#box",
        scroll: false
    });

    $("#BtnSaveAndRefresh").click(function () {
        var g_left = parseInt($("#dragx").css("left")) + 23;
        var g_top = 768 + parseInt($("#dragx").css("top")) + 27;
        var picID = $("#PicID").val();
        $("#company").attr("src", "Map.ashx?Pic=" + $("#PicID").val() + "&top=" + g_top + "&left=" + g_left);
    });

    $("#BtnSave").click(function () {
        var g_left = parseInt($("#dragx").css("left")) + 23;
        var g_top = 768 + parseInt($("#dragx").css("top")) + 27;
        $("#company").attr("src", "Map.ashx?Pic=" + $("#PicID").val() + "&top=" + g_top + "&left=" + g_left + "&EquID=" + $("#EquNo").val() + "&MapRotate=" + $("#MapRotate").val());
    });

    $("#BtnMain").click(function () {
        $("#popOverlay").remove();
        $("#ParaExtDiv").remove();
    });
}

