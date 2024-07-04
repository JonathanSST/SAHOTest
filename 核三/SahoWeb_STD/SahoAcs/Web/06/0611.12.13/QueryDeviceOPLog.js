// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case '':
            popInput_LogTime.innerText = '';
            popInput_DOPState.innerText = '';
            popInput_EquNo.innerText = '';
            popInput_UserID.innerText = '';
            popInput_UserIP.innerText = '';
            popInput_ResultMsg.value = '';
            break;
    }
}

//查詢動作
function SelectState() {
    Block();
    SelectValue.value = '';
    __doPostBack(QueryButton.id, '');
}

// 呼叫訊息視窗
function CallMsgWin(title, Msg) {
    L_popName1.innerText = title;
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        SetMode('');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput_LogTime.innerText = DataArray[0];
        popInput_DOPState.innerText = DataArray[1];
        popInput_EquNo.innerText = DataArray[2];
        popInput_UserID.innerText = DataArray[3];
        popInput_UserIP.innerText = DataArray[4];
        popInput_ResultMsg.value = DataArray[5];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}