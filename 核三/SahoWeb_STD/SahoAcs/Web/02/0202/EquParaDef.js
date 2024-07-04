// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_InputType.selectedIndex = 0;
            InputTypeChange(popInput_InputType);
            popInput_InputType.disabled = false;
            popInput_ParaName.disabled = false;
            popInput_ParaDesc.disabled = false;
            popInput_MinValue.disabled = false;
            popInput_MaxValue.disabled = false;
            popInput_ValueOptions.disabled = false;
            popInput_EditFormURL.disabled = false;
            popInput_Height.value = false;
            popInput_Width.value = false;
            popInput_DefaultValue.value = false;
            popInput_ParaName.value = '';
            popInput_ParaDesc.value = '';
            popInput_MinValue.value = '';
            popInput_MaxValue.value = '';
            popInput_ValueOptions.value = '';
            popInput_EditFormURL.value = '';
            popInput_Height.value = '';
            popInput_Width.value = '';
            popInput_DefaultValue.value = '';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Edit':
            popInput_InputType.disabled = false;
            popInput_ParaName.disabled = false;
            popInput_ParaDesc.disabled = false;
            popInput_MinValue.disabled = false;
            popInput_MaxValue.disabled = false;
            popInput_ValueOptions.disabled = false;
            popInput_EditFormURL.disabled = false;
            popInput_Height.value = false;
            popInput_Width.value = false;
            popInput_DefaultValue.value = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Delete':
            popInput_InputType.disabled = true;
            popInput_ParaName.disabled = true;
            popInput_ParaDesc.disabled = true;
            popInput_MinValue.disabled = true;
            popInput_MaxValue.disabled = true;
            popInput_ValueOptions.disabled = true;
            popInput_EditFormURL.disabled = true;
            popInput_Height.value = true;
            popInput_Width.value = true;
            popInput_DefaultValue.value = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_InputType.selectedIndex = 0;
            InputTypeChange(popInput_InputType);
            popInput_ParaName.value = '';
            popInput_ParaDesc.value = '';
            popInput_MinValue.value = '';
            popInput_MaxValue.value = '';
            popInput_ValueOptions.value = '';
            popInput_EditFormURL.value = '';
            popInput_Height.value = '';
            popInput_Width.value = '';
            popInput_DefaultValue.value = '';
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            DeleteLableText.innerText = "";
            break;
    }
}

//設定使用者按鈕的權限
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
            if (data[i] == 'Auth') {

            }
        }
    }
}

// 呼叫新增視窗
function CallAdd(title, ErrorMsg) {
    L_popName1.innerText = title;
    if (hideEquModel.value != '') {
        SetMode('Add');
        PopupTrigger1.click();
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = ErrorMsg;
        Excutecomplete(objRet);
    }
}

// 執行新增動作
function AddExcute() {
    ConfirmAct(popInput_InputType.selectedIndex);
    PageMethods.Insert(hideUserID.value, popInput_InputType.value, hideEquModel.value, popInput_ParaName.value, popInput_ParaDesc.value, popInput_MinValue.value, popInput_MaxValue.value, popInput_ValueOptions.value, popInput_EditFormURL.value, popInput_Height.value, popInput_Width.value, popInput_DefaultValue.value, Excutecomplete, onPageMethodsError);
}

// 呼叫編輯視窗
function CallEdit(title, Msg) {
    L_popName1.innerText = title;
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        SetMode('Edit');
        PopupTrigger1.click();
        PageMethods.LoadData(hideEquModel.value, SelectValue.value, SetUI, onPageMethodsError);
    }
}

// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput_InputType.selectedIndex = DataArray[4];
        InputTypeChange(popInput_InputType);
        popInput_ParaName.value = DataArray[2];
        popInput_ParaDesc.value = DataArray[3];
        popInput_MinValue.value = DataArray[6];
        popInput_MaxValue.value = DataArray[7];
        popInput_ValueOptions.value = DataArray[5];
        popInput_EditFormURL.value = DataArray[8];
        if (DataArray[9] == "") {
            popInput_Height.value = 0;
            popInput_Width.value = 0;
        }
        else {
            var para = DataArray[9].split(";")
            popInput_Height.value = para[0].replace("dialogHeight:", "").replace("px", "");
            popInput_Width.value = para[1].replace("dialogWidth:", "").replace("px", "");
        }

        //dialogHeight:400px
        //dialogWidth:600px
        popInput_DefaultValue.value = DataArray[10];
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
    ConfirmAct(popInput_InputType.selectedIndex);
    PageMethods.Update(hideUserID.value, SelectValue.value, popInput_InputType.value, hideEquModel.value, popInput_ParaName.value, popInput_ParaDesc.value, popInput_MinValue.value, popInput_MaxValue.value, popInput_ValueOptions.value, popInput_EditFormURL.value, popInput_Height.value, popInput_Width.value, popInput_DefaultValue.value, Excutecomplete, onPageMethodsError);
}

// 呼叫刪除視窗
function CallDelete(title, Msg) {
    L_popName1.innerText = title;
    DeleteLableText.innerText = "您確定要將這筆資料刪除嗎？";
    if (IsEmpty(SelectValue.value)) NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PopupTrigger1.click();
        PageMethods.LoadData(hideEquModel.value, SelectValue.value, SetUI, onPageMethodsError);
    }
}

// 執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(SelectValue.value, hideEquModel.value, Excutecomplete, onPageMethodsError);
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
            div_Value.style.display = "inline";
            div_Option.style.display = "none";
            div_URL.style.display = "none";
            break;
        case 2:     //清單選項
            div_Value.style.display = "none";
            div_Option.style.display = "inline";
            div_URL.style.display = "none";
            break;
        case 3:     //參考連結
            div_Value.style.display = "none";
            div_Option.style.display = "none";
            div_URL.style.display = "inline";
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
