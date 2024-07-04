// 執行儲存動作
function AddExcute() {
    PageMethods.Add(hideUserID.value, PickDate_Holiday.value, $("#TimeDay").val(), Excutecomplete, onPageMethodsError);
}

// 執行刪除動作
function DeleteExcute() {    
    PageMethods.Delete(SelectValue.value, Excutecomplete, onPageMethodsError);
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                    alert('系統補班資料新增完成');
                    __doPostBack(AddButton.id, '');
                    PickDate_Holiday.value = '';
                    break;
                case "Delete":
                    alert('系統補班資料刪除完成');
                    __doPostBack(DeleteButton.id, '');
                    SelectValue.value = '';
                    break;
            }
            break;
        case false:
            alert(objRet.message);
            break;
        default:

            break;
    }
}

// TD選擇換色及處理
function onSelect(Target, DBValue) {
    var objTD = document.getElementById("ContentPlaceHolder1_" + Target);
    objTD.style.color = 'white';
    var Hex = (objTD.style.background == '') ? 'FFFFFF' : RGBToHex(objTD.style.background);
    if (Hex != 'FFFFFF') {
        objTD.style.background = '#FFFFFF';
        objTD.style.color = 'black';
        objTD.style.borderColor = '#999999';
        RemoveHolidayList(DBValue);
    }
    else {
        objTD.style.background = '#249AD1';
        objTD.style.color = 'white';
        objTD.style.borderColor = '#DDDDDD';
        AddHolidayList(DBValue);
    }

}

// 色碼轉換
function RGBToHex(rgb) {
    var hexDigits = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f");
    var hex = function (x) {
        return isNaN(x) ? "00" : hexDigits[(x - x % 16) / 16] + hexDigits[x % 16];
    };
    var tmp = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    var color = hex(tmp[1]) + hex(tmp[2]) + hex(tmp[3]);
    return color.toUpperCase();
}

// 新增至處理List
function AddHolidayList(Target) {
    var RepeatFlag = false;
    var ValueList = SelectValue.value.split(',');
    for (i = 0; i < ValueList.length; i++) {
        if (ValueList[i] == Target) {
            RepeatFlag = true;
            break;
        }
    }
    if (!RepeatFlag) {
        if (SelectValue.value != '') SelectValue.value += ',';
        SelectValue.value += Target;
    }
}

// 由處理List刪除
function RemoveHolidayList(Target) {
    var RepeatFlag = false;
    var ValueList = SelectValue.value.split(',');
    for (i = 0; i < ValueList.length; i++) {
        if (ValueList[i] == Target) {
            RepeatFlag = true;
            ValueList[i] = 'Remove';
            break;
        }
    }
    if (RepeatFlag) {
        SelectValue.value = '';
        for (i = 0; i < ValueList.length; i++) {
            if (ValueList[i] != 'Remove') {
                if (SelectValue.value != '') SelectValue.value += ',';
                SelectValue.value += ValueList[i];
            }
        }
    }
}

//執行快速設碼
function SetCode() {
    $.ajax({
        type: "POST",
        url: "Holiday.aspx",
        dataType: "json",
        data: { "Event": "SetCode" },
        success: function (data) {
            alert(data.Message);
        }
    });
}