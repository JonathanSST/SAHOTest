//設定使用者按鈕的權限
function SetUserLevel(str) {
    ViewButton.disabled = true;

    if (str != '') {

        var data = str.split(",");

        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'View') {
                ViewButton.disabled = false;
            }
        }
    }
}

//*主要作業畫面：執行「查詢」動作
function SelectState() {
    Block();
    SelectValue.value = '';
    __doPostBack(QueryButton.id, '');
}

//*主要作業畫面：執行「進階查詢」動作
function AVDQuery() {
    Block();
    SelectValue.value = '';
    __doPostBack(ADVQueryButton.id, '');
}

//*主要作業畫面：自動設置「GridView」控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, hSelectValue.value);
}

//*主要作業畫面：呼叫「進階查詢」視窗
function CallAdvancedQuery() {
        SetMode('AdvancedQuery');

        PopupTrigger2.click();
}

//*主要作業畫面：呼叫「檢視」視窗
function CallShowLogDetail() {
    if (IsEmpty(SelectValue.value))
        NotSelectForEdit(Msg);
    else {
        SetMode('View');

        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

//彈出作業畫面：設定「檢視」視窗不同模式相關的欄位狀態與內容
function SetMode(SetMode) {
    switch (SetMode) {
        case 'View':
            break;
        case '':
            break;
    }
}

// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        ADV_TextBox_CardTime.value = DataArray[0];
        ADV_TextBox_LogStatus.value = DataArray[1];
        ADV_TextBox_DepNo.value = DataArray[2];
        ADV_TextBox_DepName.value = DataArray[3];
        ADV_TextBox_PsnNo.value = DataArray[4];
        ADV_TextBox_PsnName.value = DataArray[5];
        ADV_TextBox_CardNo.value = DataArray[6];
        ADV_TextBox_CardVer.value = DataArray[7];
        ADV_TextBox_EquNo.value = DataArray[8];
        ADV_TextBox_EquName.value = DataArray[9];
        ADV_TextBox_LogTime.value = DataArray[10];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}