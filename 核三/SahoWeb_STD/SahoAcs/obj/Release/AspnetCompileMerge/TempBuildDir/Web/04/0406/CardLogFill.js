//*主要作業畫面：執行「查詢」動作
function SelectState() {
    SelectValue.value = "";
    Block();
    __doPostBack(QueryButton.id, '');
}

// 呼叫編輯視窗
function CallEdit(Title, Msg) {
    ChangeText(L_popName1, Title);
    Calendar_CardTime.value = "";

    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else {
        PageMethods.LoadData(SelectValue.value, SetEditUI, onPageMethodsError);
    }
}

//將資料帶回畫面UI
function SetEditUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();
        popInput_PsnID.value = DataArray[0];
        popInput_PsnNo.value = DataArray[1];
        popInput_PsnName.value = DataArray[2];
        popInput_CardNo.value = DataArray[3];
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//新增補登記錄
function FillCardLog() {
    PageMethods.FillCardLogData(popInput_PsnID.value, popDrop_EquName.options[popDrop_EquName.selectedIndex].value,
        popDrop_LogStatus.options[popDrop_LogStatus.selectedIndex].value, Calendar_CardTime.value, FillCardLogExcute, onPageMethodsError);
}

//補登記錄是否成功
function FillCardLogExcute(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        CancelTrigger1.click();
        alert(DataArray[1])
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//各動作完成
function Excutecomplete(sRet) {
    switch (sRet.result) {
        case false:
            CancelTrigger1.click();
            alert(sRet.message);
            break;
        default:
            CancelTrigger1.click();
            break;
    }
}