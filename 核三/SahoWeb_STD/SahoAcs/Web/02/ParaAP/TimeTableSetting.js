// 預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;

    if (vArguments !== "") {
        var paraqueue = vArguments.split('&');
        hideEquID.value = paraqueue[0];
        hideEquParaID.value = paraqueue[1];
        hideParaValue.value = paraqueue[2];
    }

    __doPostBack(UpdatePanel1.id, 'StarePage');
}
