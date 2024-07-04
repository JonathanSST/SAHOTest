//******* 各公用方法：執行臨時卡建檔作業相關的公用方法 *******

//*各公用方法：選擇查詢狀態
function SelectState() {
    __doPostBack(QueryButton.id, '');
}

//*各公用方法：自動設置GridView控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 1, hSelectValue.value);
}

//******* 主作業畫面：呼叫臨時卡建檔作業相關的視窗方法 *******

//*主作業畫面：呼叫新增視窗
function CallAdd(Title) {
    ChangeText(L_popName1, Title);

    SetMode('Add');
    PopupTrigger1.click();
}

//*主作業畫面：呼叫編輯視窗
function CallEdit(Title, Msg) {
    ChangeText(L_popName1, Title);

    if (IsEmpty(hSelectValue.value))
        NotSelectForEdit(Msg);
    else {
        SetMode('Edit');
        PopupTrigger1.click();
        PageMethods.LoadData(hSelectValue.value, SetUI, onPageMethodsError);
    }
}

//*主作業畫面：呼叫刪除視窗
function CallDelete(Title, DelLabel, Msg) {
    ChangeText(L_popName1, Title);
    ChangeText(DeleteLableText, DelLabel);

    if (IsEmpty(hSelectValue.value))
        NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PopupTrigger1.click();
        PageMethods.LoadData(hSelectValue.value, SetUI, onPageMethodsError);
    }
}

//主作業畫面：輸入模式設定視窗相關的欄位狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            //Value
            popInput_CardNo.value = '';
            popInput_CardDesc.value = '';

            //Disabled
            popInput_CardNo.disabled = false;
            popInput_CardDesc.disabled = false;

            //Display
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";

            //DeleteLabelText
            ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            //Disabled
            popInput_CardNo.disabled = true;
            popInput_CardDesc.disabled = false;

            //Display
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";

            //DeleteLabelText
            ChangeText(DeleteLableText, '');
            break;
        case 'Delete':
            //Disabled
            popInput_CardNo.disabled = true;
            popInput_CardDesc.disabled = true;

            //Display
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            break;
        case '':
            //Value
            popInput_CardNo.value = '';
            popInput_CardDesc.value = '';

            //Disabled
            popInput_CardNo.disabled = false;
            popInput_CardDesc.disabled = false;

            //Display
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";

            //DeleteLabelText
            ChangeText(DeleteLableText, '');
            break;
    }
}

//主作業畫面：帶回資料設定視窗相關的欄位內容
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput_CardNo.value = DataArray[0];
        popInput_CardDesc.value = DataArray[1];
        $('[name*="ddlCardArea"]').val(DataArray[2]);
    }
    else {
        var objRet = new Object;

        objRet.result = false;
        objRet.message = DataArray[2];

        Excutecomplete(objRet);
    }
}

//主作業畫面：完成動作設定網頁相關的參數內容
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                    hSelectValue.value = objRet.message;
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Edit":
                    hSelectValue.value = popInput_CardNo.value;
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Delete":
                    hSelectValue.value = '';
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
            }

            SetMode('');
            CancelTrigger1.click();
            break;
        case false:
            alert(objRet.message);

            $.unblockUI();
            break;
        default:
            break;
    }
}

//******* 次作業畫面一：執行臨時卡建檔作業相關的動作方法 *******

//次作業畫面一：執行新增動作
function AddExcute() {
    PageMethods.Insert(popInput_CardNo.value, popInput_CardDesc.value, ddlCardArea.value, Excutecomplete, onPageMethodsError);
}

//次作業畫面一：執行編輯動作
function EditExcute() {
    PageMethods.Update(hSelectValue.value, popInput_CardDesc.value, ddlCardArea.value, Excutecomplete, onPageMethodsError);
}

//*次作業畫面一：執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(hSelectValue.value, popInput_CardNo.value, Excutecomplete, onPageMethodsError);
}