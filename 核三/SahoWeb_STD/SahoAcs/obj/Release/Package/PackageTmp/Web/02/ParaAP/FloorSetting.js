// 預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;
    var paraqueue = vArguments.split('&');

    hideEquID.value = paraqueue[0];
    hideEquParaID.value = paraqueue[1];
    hideParaValue.value = paraqueue[2];
    hideFloorName.value = decodeURIComponent(paraqueue[3]);
    __doPostBack(Elevator_UpdatePanel.id, 'StarePage');
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

function ChangeCheck() {
    $('input[name*="FloorControl"]').each(function () {
        $(this).prop("checked", !$(this).prop("checked"));
    });
}
