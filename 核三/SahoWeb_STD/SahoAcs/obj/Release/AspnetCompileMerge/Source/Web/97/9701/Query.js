$(document).ready(function () {
    BindEvent();
    if ($("#PsnID").val() != "") {
        $(".ShowPsnInfo").css("display", "none");
    }
    SetUserLevel();
});



function PopURL(obj) {
   
}



function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {        
        $("#DateS").val($('[name*="_CardDateS"]').val());        
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
    $("#DateS").val($('[name*="$CardDateS$"]').val());
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}

function SetOpenLocation() {

}


function SetQuery() {    
    $("#DateS").val($('[name*="$CardDateS$"]').val());
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        //dataType: 'json',
        data: $("#form1").find("input,select").serialize(),
        success: function (data) {
            console.log(data);
            $("#UpdatePanel1").html($(data).find("#UpdatePanel1").html());
            //BindEvent();
            $.unblockUI();
            //SetUserLevel();
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



function SaveNoteList() {
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: $('#ContentPlaceHolder1_tablePanel').find('input').serialize()+"&PageEvent=Save",
        dataType: "json",
        success: function (data) {
            alert(data.message);
        }, fail: function () {
            console.log("error");
        }
    });
}

function SetUserLevel() {
    var str = $("#AuthList").val();
    var data = str.split(",");
    var DoEdit = false;
    for (var i = 0; i < data.length; i++) {
        if (data[i] == 'ShowMap') {
            DoEdit = true;
        }
    }
    console.log(DoEdit);
    if (DoEdit == false) {
        console.log(DoEdit);        
        $('[name*="BtnShowImage"]').prop('disabled', true);
        //$('.TableS1 th:eq(6)').html('');
        //$('.TableS1 th:eq(6)').remove();
        //$('.TableS1 th:eq(5)').css('width', '');
        //$("#ContentPlaceHolder1_MainGridView tr").each(function () {
        //    $(this).find('td:eq(6)').remove();
        //    $(this).find('td:eq(5)').css('width', '');
        //});
    }
}

function CancelOneLogArea() {
    $("#OneLogArea").css("display", "none");
    $("#popOverlay").css("display", "none");
    $("#map").remove();
    $("#ShowLogMap").css("display", "none");
}




function SetLocation(y, x) {
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $("#popOverlay").css("display", "block");
    $("#ShowLogMap").css("display", "block");
    //$("#ShowLogMap").css("display", "block");
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#ShowLogMap").css("left", 0);
    $("#ShowLogMap").css("top", 0);
    $("#ShowLogMap").css("left", ($(document).width() - $("#ShowLogMap").width()) / 2);
    $("#ShowLogMap").css("top", $(document).scrollTop() + 50);
    //$("#map").css("display", "block");
    $("#btnCloseMap").before('<div id="map" class="map"></div>')
    $('#map').width($('#ShowLogMap').width());
    $('#map').height($('#ShowLogMap').height());

    var cardpoint = ol.proj.fromLonLat([x, y])
    var map = new ol.Map({
        target: 'map',
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            })
        ],
        view: new ol.View({
            center: cardpoint,
            zoom: 18
        })
    });


    //初始化要素
    var iconFeature = new ol.Feature({
        //點要素
        geometry: new ol.geom.Point(cardpoint),
        //名稱屬性
        name: '打卡地點',
        //人口屬性
        population: 2115
    });
    //為點要素新增樣式
    iconFeature.setStyle(createLabelStyle(iconFeature));

    //初始化向量資料來源
    var vectorSource = new ol.source.Vector({
        //指定要素
        features: [iconFeature]
    });

    //初始化向量圖層
    var vectorLayer = new ol.layer.Vector({
        //資料來源
        source: vectorSource
    });
    //將向量圖層新增到map中
    map.addLayer(vectorLayer);

}



var createLabelStyle = function (feature) {
    //返回一個樣式
    return new ol.style.Style({
        //把點的樣式換成ICON圖示
        image: new ol.style.Icon({
            //控制標註圖片和文字之間的距離
            anchor: [0.5, 0.5],
            //標註樣式的起點位置
            anchorOrigin: 'top-right',
            //X方向單位：分數
            anchorXUnits: 'fraction',
            //Y方向單位：畫素
            anchorYUnits: 'pixels',
            //偏移起點位置的方向
            offsetOrigin: 'top-right',
            //透明度
            opacity: 0.75,
            //圖片路徑
            src: '/img/43.png',
            //size: [120, 120]
            scale: 1
        }),
        //文字樣式
        text: new ol.style.Text({
            //對齊方式
            textAlign: 'center',
            //文字基線
            textBaseline: 'middle',
            //字型樣式
            font: 'normal 14px 微軟雅黑',
            //文字內容
            text: feature.get('name'),
            //填充樣式
            fill: new ol.style.Fill({
                color: '#aa3300'
            }),
            //筆觸
            stroke: new ol.style.Stroke({
                color: '#ffcc33',
                width: 2
            })
        })
    });
};
