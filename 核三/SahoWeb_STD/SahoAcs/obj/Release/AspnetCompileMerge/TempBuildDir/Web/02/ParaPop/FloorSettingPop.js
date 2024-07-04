//儲存資料
function DoSave() {
    var floor_bin = "";
    var para_name = $("#hideParaName").val();
    $("#ContentPlaceHolder1_GridViewFloor").find("tr").each(function (index) {
        
        if ($(this).find('input[name*="CheckIO"]').prop("checked") == true) {
            floor_bin = "1" + floor_bin;
        } else {
            floor_bin = "0" + floor_bin;
        }
    });

    if (para_name == "ElevCtrlOnOpen") {        
        floor_bin = floor_bin.replaceAll("0", "F").replaceAll("1", "T");
        floor_bin = floor_bin.replaceAll("F", "1").replaceAll("T", "0");
    }
    
    var temp = parseInt(floor_bin, 2); // 先將字串以2進位方式解析為數值
    var result = temp.toString(16); // 將數值轉為16進位
    for (var s = result.length; s < 12; s++) {
        result = "0" + result;
    }
    
    $(target_para_obj).val(result.toUpperCase());



    DoClose();
}

function DoClose() {
    $("#ParaPopContent").remove();
}

function SetAllData(chkall) {
    $('input[name*="CheckIO"]').prop("checked", $(chkall).prop("checked"));
}


function SetLoadData() {
    var data = $('#hideFloorData').val().split("").reverse().join("");
    $("#ContentPlaceHolder1_GridViewFloor").find("tr").each(function (index) {
        var booCheck = false;
        if (data[index] == "1") {
            booCheck = true;
        }        
        if ($('#hideParaName').val() == "ElevCtrlOnOpen") {
            booCheck = !booCheck;
        }
        $(this).find('input[name*="CheckIO"]').prop("checked", booCheck);
    });
}


String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};