// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_PassWD.disabled = false;
            popInput_Ip.disabled = false;
            popInput_Port.disabled = false;
            popInput_IsAssign.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_PassWD.value = '';
            popInput_Ip.value = '';
            popInput_Port.value = '';
            popInput_IsAssign.value = '1';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Edit':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_PassWD.disabled = false;
            popInput_Ip.disabled = false;
            popInput_Port.disabled = false;
            popInput_IsAssign.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Delete':
            popInput_No.disabled = true;
            popInput_Name.disabled = true;
            popInput_PassWD.disabled = true;
            popInput_Ip.disabled = true;
            popInput_Port.disabled = true;
            popInput_IsAssign.disabled = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_PassWD.disabled = false;
            popInput_Ip.disabled = false;
            popInput_Port.disabled = false;
            popInput_IsAssign.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_PassWD.value = '';
            popInput_Ip.value = '';
            popInput_Port.value = '';
            popInput_IsAssign.value = '1';
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            break;
    }
}

// 呼叫新增視窗
function CallAdd(title) {
    L_popName1.innerText = title;
    SetMode('Add');
    PopupTrigger1.click();
}

// 執行新增動作
function AddExcute() {
    PageMethods.Insert(hideUserID.value, popInput_No.value, popInput_Name.value, popInput_PassWD.value, popInput_Ip.value, popInput_Port.value, popInput_IsAssign.value, Excutecomplete, onPageMethodsError);
}

// 呼叫編輯視窗
function CallEdit(title, Msg) {
    L_popName1.innerText = title;
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        SetMode('Edit');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput_No.value = DataArray[1];
        popInput_Name.value = DataArray[2];
        popInput_PassWD.value = DataArray[6];
        popInput_Ip.value = DataArray[4];
        popInput_Port.value = DataArray[5];
        popInput_IsAssign.value = DataArray[3];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

// 執行編輯動作
function EditExcute() {
    PageMethods.Update(hideUserID.value, SelectValue.value, popInput_No.value, popInput_Name.value, popInput_PassWD.value, popInput_Ip.value, popInput_Port.value, popInput_IsAssign.value, Excutecomplete, onPageMethodsError);
}

// 呼叫刪除視窗
function CallDelete(title, Msg) {
    L_popName1.innerText = title;
    DeleteLableText.innerText = "您確定要將這筆資料刪除嗎？";
    if (IsEmpty(SelectValue.value)) NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

// 執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(SelectValue.value, Excutecomplete, onPageMethodsError);
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                    SelectValue.value = popInput_No.value.trim();
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Edit":
                    SelectValue.value = popInput_No.value.trim();
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

function SelectState() {
    __doPostBack(QueryButton.id, '');
}

function SetUserLevel(str) {
    AddButton.disabled = true;
    EditButton.disabled = true;
    DeleteButton.disabled = true;

    if (str != '') {
        var data = str.split(",");

        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'Add') {
                AddButton.disabled = false;
            }

            if (data[i] == 'Edit') {
                EditButton.disabled = false;
            }

            if (data[i] == 'Del') {
                DeleteButton.disabled = false;
            }
        }
    }
}

