//儲存樓層設定資料
function ProcessFloorSetting() {
    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var textcount = 0;
    var TableItem = new Array();

    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "checkbox" && objForm.elements[iCount].name.slice(-12) == "FloorControl") {
            TableItem[textcount] = objForm.elements[iCount].checked;
            textcount++;
        }
    }
    PageMethods.ProcessFloorSetting(TableItem, Excutecomplete, onPageMethodsError);
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "ProcessFloorSetting":
                    window.returnValue = objRet.message;
                    window.close();
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

// CheckBox選取
function CheckBoxSelected(Obj) {
    if (document.getElementById(Obj).checked) {
        document.getElementById(Obj).checked = false;
    }
    else {
        document.getElementById(Obj).checked = true;
    }
}
