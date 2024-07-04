// 執行←動作
function AddEqu() {
    __doPostBack(AddEquButton.id, '');
}

// 執行→動作
function RemoveEqu() {
    __doPostBack(RemoveEquButton.id, '');
}

// 執行查詢Pending清單
function PendingQuery() {
    __doPostBack(PendingButton.id, '');
}

// 執行查詢Query清單
function QueueQuery() {
    __doPostBack(QueueButton.id, '');
}

// 取得FloorNameList
function GetFloorNameList(sURL, vArguments, sFeatures, TriggerID) {
    var paraqueue = vArguments.split("&");
    //vArguments = "hiddenMaster=" + paraqueue[0]
    //           + "&EquID=" + paraqueue[1]
    //           + "&EquParaID=" + paraqueue[2];
    vArguments = "EquID=" + paraqueue[0]
               + "&CardExtData=" + paraqueue[1];

    if (sURL.indexOf("?") == -1)
        vArguments = "?" + vArguments;
    else
        vArguments = "&" + vArguments;

    var vReturnValue = window.showModalDialog(sURL + vArguments, '', sFeatures + ";center:yes;");
    if (vReturnValue != '' && typeof vReturnValue != "undefined") {
        __doPostBack(TriggerID, paraqueue[0] + "&" + vReturnValue);
    }
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                //case "ProcessFloorSetting":
                //    window.returnValue = objRet.message;
                //    window.close();
                //    break;
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
