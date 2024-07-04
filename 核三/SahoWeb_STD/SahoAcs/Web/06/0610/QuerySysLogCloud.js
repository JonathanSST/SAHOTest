//************************************************** 網頁畫面設計(一)相關的JavaScript方法 **************************************************

//*主要作業畫面：執行「查詢」動作
function SelectState() {
    hComplexQueryWheresql.value = '';
    __doPostBack(QueryButton.id, '');
}

//*主要作業畫面：自動設置「GridView」控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, hSelectValue.value);
}

//*主要作業畫面：呼叫「複合查詢」視窗
function CallComplexQueryWindow(Title) {
    ChangeText(L_popName0, Title);

    if (IsEmpty(hSaveComplexQueryData.value))
        SetComplexQueryWindowMode('');
    else
        SetComplexQueryWindowMode('ReadComplexQueryData');

    PopupTrigger0.click();
}

//*主要作業畫面：呼叫「檢視」視窗
function CallViewWindow(Title, Msg) {
    ChangeText(L_popName1, Title);

    if (IsEmpty(hSelectValue.value))
        NotSelectForEdit(Msg);
    else {
        SetViewWindowMode('View');

        PopupTrigger1.click();
        PageMethods.LoadViewWindowData(hSelectValue.value, SetViewWindowData, onPageMethodsError);
    }
}

//主要作業畫面：完成動作設定網頁相關的參數內容
function ExcuteComplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case 'ComplexQuery':
                    hComplexQueryWheresql.value = objRet.message;
                    __doPostBack(ComplexQueryButton.id, '');
                    break;
            }

            CancelTrigger0.click();
            break;
        case false:
            alert(objRet.message);
            break;
        default:
            break;
    }
}

//************************************************** 網頁畫面設計(二)相關的JavaScript方法 **************************************************

//複合查詢畫面：儲存「複合查詢」視窗相關的欄位狀態與內容
function SaveComplexQueryData() {
    hSaveComplexQueryData.value = popInput0_STime.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_ETime.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_UserID.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_UserName.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_LogFrom.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_LogIP.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_EquNo.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_EquName.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_LogInfo.value.trim() + '|';
    hSaveComplexQueryData.value += popInput0_LogDesc.value.trim();
}

//複合查詢畫面：設定「複合查詢」視窗不同模式相關的欄位狀態與內容
function SetComplexQueryWindowMode(SetMode) {
    switch (SetMode) {
        case 'ReadComplexQueryData':
            var ComplexQueryData = hSaveComplexQueryData.value.split('|');

            if (ComplexQueryData.length > 0) {
                popInput0_STime.value = ComplexQueryData[0];
                popInput0_ETime.value = ComplexQueryData[1];
                popInput0_UserID.value = ComplexQueryData[2];
                popInput0_UserName.value = ComplexQueryData[3];
                popInput0_LogFrom.value = ComplexQueryData[4];
                popInput0_LogIP.value = ComplexQueryData[5];
                popInput0_EquNo.value = ComplexQueryData[6];
                popInput0_EquName.value = ComplexQueryData[7];
                popInput0_LogInfo.value = ComplexQueryData[8];
                popInput0_LogDesc.value = ComplexQueryData[9];
            }
            break;
        case '':
            hSaveComplexQueryData.value = '';

            //Value
            popInput0_STime.value = GetNowDate();
            popInput0_ETime.value = GetNowDate();
            popInput0_UserID.value = '';
            popInput0_UserName.value = '';
            popInput0_LogFrom.value = '';
            popInput0_LogIP.value = '';
            popInput0_EquNo.value = '';
            popInput0_EquName.value = '';
            popInput0_LogInfo.value = '';
            popInput0_LogDesc.value = '';

            //Disabled
            popInput0_STime.disabled = false;
            popInput0_ETime.disabled = false;
            popInput0_UserID.disabled = false;
            popInput0_UserName.disabled = false;
            popInput0_LogFrom.disabled = false;
            popInput0_LogIP.disabled = false;
            popInput0_EquNo.disabled = false;
            popInput0_EquName.disabled = false;
            popInput0_LogInfo.disabled = false;
            popInput0_LogDesc.disabled = false;

            //Display
            popBtn0_Query.style.display = 'inline';
            popBtn0_Cancel.style.display = 'inline';
            popBtn0_ClearQueryParam.style.display = 'inline';
            break;
    }
}

//複合查詢畫面：執行複合查詢
function SetComplexQueryWheresql() {
    var qryParam = new Array(10);

    //設定複合查詢相關的參數資料
    qryParam[0] = popInput0_STime.value.trim();
    qryParam[1] = popInput0_ETime.value.trim();
    qryParam[2] = popInput0_UserID.value.trim();
    qryParam[3] = popInput0_UserName.value.trim();
    qryParam[4] = popInput0_LogFrom.value.trim();
    qryParam[5] = popInput0_LogIP.value.trim();
    qryParam[6] = popInput0_EquNo.value.trim();
    qryParam[7] = popInput0_EquName.value.trim();
    qryParam[8] = popInput0_LogInfo.value.trim();
    qryParam[9] = popInput0_LogDesc.value.trim();

    PageMethods.SetComplexQueryWheresql(qryParam, ExcuteComplete, onPageMethodsError);
}

//************************************************** 網頁畫面設計(三)相關的JavaScript方法 **************************************************

//彈出作業畫面：設定「檢視」視窗不同模式相關的欄位狀態與內容
function SetViewWindowMode(SetMode) {
    switch (SetMode) {
        case 'View':
            //Disabled
            popInput1_RecordID.disabled = true;
            popInput1_LogTime.disabled = true;
            popInput1_LogType.disabled = true;
            popInput1_UserID.disabled = true;
            popInput1_UserName.disabled = true;
            popInput1_LogFrom.disabled = true;
            popInput1_LogIP.disabled = true;
            popInput1_EquNo.disabled = true;
            popInput1_EquName.disabled = true;
            popInput1_LogInfo.disabled = true;
            popInput1_LogDesc.disabled = true;
          
            //Display
            popBtn1_Exit.style.display = 'inline';
            break;
        case '':
            //Value
            popInput1_RecordID.value = '';
            popInput1_LogTime.value = '';
            popInput1_LogType.value = '';
            popInput1_UserID.value = '';
            popInput1_UserName.value = '';
            popInput1_LogFrom.value = '';
            popInput1_LogIP.value = '';
            popInput1_EquNo.value = '';
            popInput1_EquName.value = '';
            popInput1_LogInfo.value = '';
            popInput1_LogDesc.value = '';
                        
            //Enable
            popInput1_RecordID.disabled = false;
            popInput1_LogTime.disabled = false;
            popInput1_LogType.disabled = false;
            popInput1_UserID.disabled = false;
            popInput1_UserName.disabled = false;
            popInput1_LogFrom.disabled = false;
            popInput1_LogIP.disabled = false;
            popInput1_EquNo.disabled = false;
            popInput1_EquName.disabled = false;
            popInput1_LogInfo.disabled = false;
            popInput1_LogDesc.disabled = false;

            //Display
            popBtn1_Exit.style.display = 'none';
            break;
    }
}

//彈出作業畫面：設定「檢視」視窗相關的欄位狀態與內容
function SetViewWindowData(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput1_RecordID.value = DataArray[0];
        popInput1_LogTime.value = DataArray[1];
        popInput1_LogType.value = DataArray[2];
        popInput1_UserID.value = DataArray[3];
        popInput1_UserName.value = DataArray[4];
        popInput1_LogFrom.value = DataArray[5];
        popInput1_LogIP.value = DataArray[6];
        popInput1_EquNo.value = DataArray[7];
        popInput1_EquName.value = DataArray[8];
        popInput1_LogInfo.value = DataArray[9];
        popInput1_LogDesc.value = DataArray[10];
    }
    else {
        var objRet = new Object;

        objRet.result = false;
        objRet.message = DataArray[1];

        ExcuteComplete(objRet);
    }
}



function GetNowDate() {
    var nt = new Date();
    var sYear = nt.getFullYear();
    var sMonth = nt.getMonth() + 1;
    var sDay = nt.getDate();
    var sHour = nt.getHours();
    var sMin = nt.getMinutes();
    var sSec = nt.getSeconds();

    if (sYear < 1911) sYear = 1911;
    if (sMonth < 10) sMonth = '0' + sMonth;
    if (sDay < 10) sDay = '0' + sDay;
    if (sHour < 10) sHour = '0' + sHour;
    if (sMin < 10) sMin = '0' + sMin;
    if (sSec < 10) sSec = '0' + sSec;

    var strNowTime = sYear + '/' + sMonth + '/' + sDay + ' ' + sHour + ':' + sMin + ':' + sSec;

    return strNowTime;
}