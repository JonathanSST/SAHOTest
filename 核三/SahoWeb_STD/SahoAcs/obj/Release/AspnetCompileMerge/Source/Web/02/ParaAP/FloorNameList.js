// 預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;
    var paraqueue = vArguments.split('&');

    hideEquID.value = paraqueue[0];
    hideEquParaID.value = paraqueue[1];
    hideParaValue.value = paraqueue[2];
    hideFloorName.value = decodeURIComponent(paraqueue[3]);
    __doPostBack(UpdatePanel1.id, 'StarePage');
}

function SelectState() {
    SelectValue.value = '';
    __doPostBack(Input_EquModel.id, '');
}

//設定樓層數
function SetElevatorRow() {
    __doPostBack(popB_SetElevatorButton.id, popInput_FloorCount.value);
}
