$("#BtnMain").click(function () {
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
});


function SetPointInit() {
    $("div[id='dragx']").draggable({
        containment: "#box",
        scroll: false
    });
}