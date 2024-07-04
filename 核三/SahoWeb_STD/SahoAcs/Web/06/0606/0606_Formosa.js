// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case '':
            break;
    }
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                    SelectValue.value = popInput_ParaName.value.trim();
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Edit":
                    SelectValue.value = popInput_ParaName.value.trim();
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Delete":
                    SelectValue.value = '';
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
            }
            SetMode('');
            CancelTrigger1.click();
            break;
        case false:
            alert(objRet.message);
            break;
        default:

            break;
    }
}

// GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectValue.value);
}

function SelectState(Objid) {
    SelectValue.value = '';
    var obj = document.getElementById(Objid);
    var objvalue = obj.options[obj.selectedIndex].value;
    hideEquModel.value = objvalue;
    __doPostBack(Input_EquModel.id, '');
}

// 依所選項目顯示輸入UI
function InputTypeChange(obj) {

    var div_Value = document.getElementById('ContentPlaceHolder1_div_Value');
    var div_Option = document.getElementById('ContentPlaceHolder1_div_Option');
    var div_URL = document.getElementById('ContentPlaceHolder1_div_URL');

    switch (obj.selectedIndex) {
        case 0:     //文字欄位
            div_Value.style.display = "none";
            div_Option.style.display = "none";
            div_URL.style.display = "none";
            break;
        case 1:     //數值範圍
            div_Value.style.display = "block";
            div_Option.style.display = "none";
            div_URL.style.display = "none";
            break;
        case 2:     //清單選項
            div_Value.style.display = "none";
            div_Option.style.display = "block";
            div_URL.style.display = "none";
            break;
        case 3:     //參考連結
            div_Value.style.display = "none";
            div_Option.style.display = "none";
            div_URL.style.display = "block";
            break;
        default:
            div_Value.style.display = "none";
            div_Option.style.display = "none";
            div_URL.style.display = "none";
            break
    }

}

// 依所選項目來清空UI上其餘數值
function ConfirmAct(act) {
    switch (act) {
        case 0:     //文字欄位
            popInput_MinValue.value = '';
            popInput_MaxValue.value = '';
            popInput_ValueOptions.value = '';
            popInput_EditFormURL.value = '';
            break;
        case 1:     //數值範圍
            popInput_ValueOptions.value = '';
            popInput_EditFormURL.value = '';
            break;
        case 2:     //清單選項
            popInput_MinValue.value = '';
            popInput_MaxValue.value = '';
            popInput_EditFormURL.value = '';
            break;
        case 3:     //參考連結
            popInput_MinValue.value = '';
            popInput_MaxValue.value = '';
            popInput_ValueOptions.value = '';
            break;
    }
}
