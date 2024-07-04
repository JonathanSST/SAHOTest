$(document).ready(function () {
    MainListInit();
});

function MainListInit() {
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
        var that = $(this);
        $(this).find("#BtnMapShow").click(function () {
           
            $.post("EquMapShow.aspx",
                { "PicID": $(that).find("#EditID").val(), "PageEvent": "Edit" },
                function (data) {
                    OverlayContent(data);
                    SetPointInit();

                });
        });
    });
}

function SetPointInit() {
    $("div[id='dragx']").draggable({
        containment: "#box",
        scroll: false
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
    $("#popOverlay").height($(document).height());
}

function ShowPage(page_index) {
    $("#PageIndex").val(page_index);
    
}


