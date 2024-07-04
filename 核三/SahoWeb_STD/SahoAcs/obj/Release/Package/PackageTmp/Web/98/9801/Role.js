// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_EName.disabled = false;
            popInput_Desc.disabled = false;
            popInput_States.disabled = false;
            popInput_Remark.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_EName.value = '';
            popInput_Desc.value = '';
            popInput_States.value = '1';
            popInput_Remark.value = '';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Edit':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_EName.disabled = false;
            popInput_Desc.disabled = false;
            popInput_States.disabled = false;
            popInput_Remark.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Delete':
            popInput_No.disabled = true;
            popInput_Name.disabled = true;
            popInput_EName.disabled = true;
            popInput_Desc.disabled = true;
            popInput_States.disabled = true;
            popInput_Remark.disabled = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_EName.disabled = false;
            popInput_Desc.disabled = false;
            popInput_States.disabled = false;
            popInput_Remark.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_EName.value = '';
            popInput_Desc.value = '';
            popInput_States.value = '1';
            popInput_Remark.value = '';
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            break;
    }
}

// 清除Auth視窗中設定值
function ReSetAuth() {
    Checkall("form1", this);
    var oPanel = popAuthPanel;
    oPanel.scrollTop = 0;
}

// 呼叫新增視窗
function CallAdd(title) {
    L_popName1.innerText = title;
    SetMode('Add');
    PopupTrigger1.click();
}

// 執行新增動作
function AddExcute() {
    PageMethods.Insert(hideUserID.value, popInput_No.value, popInput_Name.value, popInput_EName.value, popInput_Desc.value, popInput_States.value, popInput_Remark.value, Excutecomplete, onPageMethodsError);
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
        popInput_EName.value = DataArray[3];
        popInput_Desc.value = DataArray[4];
        popInput_States.value = DataArray[5];
        popInput_Remark.value = DataArray[6];
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
    PageMethods.Update(hideUserID.value, SelectValue.value, popInput_No.value, popInput_Name.value, popInput_EName.value, popInput_Desc.value, popInput_States.value, popInput_Remark.value, Excutecomplete, onPageMethodsError);
}

// 呼叫刪除視窗
function CallDelete(title, Msg,Msg2) {
    L_popName1.innerText = title;
    //console.log(Msg2);
    DeleteLableText.innerText = Msg2;
    if (IsEmpty(SelectValue.value)) NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

// 執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(hideUserID.value, SelectValue.value, Excutecomplete, onPageMethodsError);
}

// 呼叫權限設定視窗
function CallAuth(title, Msg) {
    L_popName2.innerText = title;
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        PopupTrigger2.click();
        PageMethods.LoadMenuAuthTable(SelectValue.value, SetAuthUI, onPageMethodsError);
    }
}

// 執行權限儲存動作
function AuthSaveExcute() {
    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var checkcount = 0;

    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "checkbox") {
            checkcount++;
        }

        var Authid = new Array(checkcount);
        var Authact = new Array(checkcount);
        var Authchecked = new Array(checkcount);
        checkcount = 0;

        for (var iCount = 0; iCount < objLen; iCount++) {
            if (objForm.elements[iCount].type == "checkbox") {
                var IdAct = objForm.elements[iCount].id.split('_');
                if (Authid[checkcount] != IdAct[1] && Authid[checkcount] != null) {
                    checkcount++;
                }
                if (Authid[checkcount] == null)
                    Authid[checkcount] = IdAct[1];

                if (Authact[checkcount] == null)
                    Authact[checkcount] = IdAct[2];
                else
                    Authact[checkcount] += "," + IdAct[2];

                if (Authchecked[checkcount] == null)
                    Authchecked[checkcount] = objForm.elements[iCount].checked;
                else
                    Authchecked[checkcount] += "," + objForm.elements[iCount].checked;
            }
        }
    }
    PageMethods.SaveMenuAuth(hideUserID.value, SelectValue.value, Authid, Authact, Authchecked, Excutecomplete, onPageMethodsError);
}

// 將資料帶回Auth畫面UI
function SetAuthUI(DataArray) {
    Checkall("form1", this);
    var MenuName;
    var ActName;
    L_popName2.innerText = "「" + SelectNowName.value + "」" + L_popName2.innerText;
    console.log(DataArray);
    for (i in DataArray) {
        if (DataArray[i]=="None") {
            continue;
        }
        if (i % 2 == 0) {
            MenuName = DataArray[i];
        }
        else {
            var ActArray = DataArray[i].split(",");
            for (x in ActArray) {
                ActName = ActArray[x];
                if (ActName != null && ActName != '') {
                    CheckBoxSelected(MenuName + "_" + ActName);
                }
            }
        }
    }
    $('input[type="checkbox"]').css("width", 26).css("height", 26);
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
            CancelTrigger2.click();
            break;
        case false:
            CancelTrigger1.click();
            CancelTrigger2.click();
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

// CheckBox選取
function CheckBoxSelected(Obj) {
    if (document.getElementById('ContentPlaceHolder1_' + Obj).checked) {
        document.getElementById('ContentPlaceHolder1_' + Obj).checked = false;
    }
    else {
        document.getElementById('ContentPlaceHolder1_' + Obj).checked = true;
    }
}

// CheckBox 全選或全取消
function Checkall(input1, input2) {
    var objForm = document.forms[input1];
    var objLen = objForm.length;
    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "checkbox") {
            objForm.elements[iCount].checked = false;
        }
        //if (input2.checked == true) {
        //    if (objForm.elements[iCount].type == "checkbox") {
        //        objForm.elements[iCount].checked = true;
        //    }
        //}
        //else {
        //    if (objForm.elements[iCount].type == "checkbox") {
        //        objForm.elements[iCount].checked = false;
        //    }
        //}
    }
}

function SelectState() {
    __doPostBack(QueryButton.id, '');
}


function SetNowName(NowName) {
    SelectNowName.value = NowName;
}

function SetUserLevel(str) {
    //console.log(str);
    var data = str.split(",");
    $('[name*="AddButton"]').prop('disabled', true);
    $('[name*="EditButton"]').prop('disabled', true);
    $('[name*="DeleteButton"]').prop('disabled', true);
    //$('[name*="MenuButton"]').prop('disabled', true);
    //$('[name*="RoleButton"]').prop('disabled', true);
    $('[name*="AuthButton"]').prop('disabled', true);
    for (var i = 0; i < data.length; i++) {
        if (data[i] == 'Add') {
            $('[name*="AddButton"]').prop('disabled', false);
            //SetCheckMax();
        }
        if (data[i] == 'Edit') {
            $('[name*="EditButton"]').prop('disabled', false);
        }
        if (data[i] == 'Del') {
            $('[name*="DeleteButton"]').prop('disabled', false);
        }
        if (data[i] == "Auth") {            
            $('[name*="AuthButton"]').prop('disabled', false);
        }
    }
}