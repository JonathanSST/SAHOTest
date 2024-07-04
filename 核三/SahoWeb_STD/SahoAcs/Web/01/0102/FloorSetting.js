$(document).ready(function () {
    var data = $('#FloorData').val().split("").reverse().join("");
    $("#ContentPlaceHolder1_GridViewFloor").find("tr").each(function (index) {
        if (data[index] == "1") {
            $(this).find('input[name*="CheckIO"]').prop("checked", true);
        }
    });
    $("#btnUpdate").click(function () {
        SaveFloor();
    });
    $("#btnCancel").click(function () {
        $("#ElevPop").remove();
        $("#popOverlay2").remove();
    });
    $('input[type="checkbox"]').css("width", 25).css("height", 25);
});

function SetCheckAll(chk_all) {
    if ($(chk_all).prop("checked") == true) {
        $('input[name*="CheckIO"]').prop("checked", true);
    } else {
        $('input[name*="CheckIO"]').prop("checked", false);
    }
}

function SaveFloor() {
    var ext_index = $("#EquIndex").val();
    //console.log(ext_index);
    var floor_bin = "";
    $("#ContentPlaceHolder1_GridViewFloor").find("tr").each(function (index) {
        if ($(this).find('input[name*="CheckIO"]').prop("checked") == true) {
            floor_bin = "1" + floor_bin;
        } else {
            floor_bin = "0" + floor_bin;
        }
    });
    for (var s = floor_bin.length; s < 48; s++) {
        floor_bin = "0" + floor_bin;
    }
    var temp = parseInt(floor_bin, 2); // 先將字串以2進位方式解析為數值
    var result = temp.toString(16); // 將數值轉為16進位
    for (var s = result.length; s < 12; s++) {
        result = "0" + result;
    }
    //console.log(result);
    $('input[name*="CardExtData"]:eq(' + ext_index + ')').val(result);
    $('input[name*="OpMode_"][value="1"]:eq(' + ext_index + ')').prop("checked", true);
    $("#ElevPop").remove();
    $("#popOverlay2").remove();
}

