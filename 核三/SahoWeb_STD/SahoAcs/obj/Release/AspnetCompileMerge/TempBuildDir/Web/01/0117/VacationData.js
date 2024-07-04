function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_VNo.disabled   = false;
            popInput_VName.disabled = false;

            popInput_VNo.value   = '';
            popInput_VName.value = '';

            popB_Add.style.display  = "inline";
            popB_Edit.style.display = "none";
            ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            popInput_VNo.disabled   = false;
            popInput_VName.disabled = false;

            popB_Add.style.display  = "none";
            popB_Edit.style.display = "inline";
            ChangeText(DeleteLableText, '');
            break;
        case 'Delete':           
            popInput_VNo2.disabled   = true;
            popInput_VName2.disabled = true;

            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_VNo.disabled   = false;
            popInput_VName.disabled = false;

            popInput_VNo.value   = '';
            popInput_VName.value = '';

            popB_Add.style.display    = "none";
            popB_Edit.style.display   = "none";
            popB_Delete.style.display = "none";
            break;
    }
}

function SetUserLevel(str) {
    AddButton.disabled  = true;
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

//******************** 新增作業 ********************

//呼叫新增視窗
function CallAdd(Title) {
    ChangeText(L_popName1, Title);
    SetMode('Add');
    PopupTrigger1.click();
}

//執行新增動作
function AddExcute(UserID) {
    let that = $('input[name$="popInput_VNo"]');
    if ($(that).val().trim().length != $(that).prop('maxlength')) {
        console.log($(that).val());
        alert($("#ContentPlaceHolder1_popLabel_VNo").text() + '長度必須為 '+$(that).prop('maxlength')+"碼");
        $('input[name$="popInput_VNo"]').focus();
        return false;
    }

    if ($('input[name$="popInput_VName"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_VName").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_VName"]').focus();
        return false;
    }

    PageMethods.Insert(popInput_VNo.value.trim(), popInput_VName.value, UserID, Excutecomplete, onPageMethodsError);
}

//******************** 編輯作業 ********************

// 呼叫編輯視窗
function CallEdit(Title, Msg) {
    ChangeText(L_popName1, Title);

    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else {
        SetMode('Edit');
        PageMethods.LoadData(SelectValue.value, hUserId.value, 'Edit', SetEditUI, onPageMethodsError);
    }
}

//設定資料至編輯頁面
function SetEditUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();

        popInput_VNo.value   = DataArray[1];
        popInput_VName.value = DataArray[2];
        SelectNowNo.value = DataArray[1];
    }
    else {
        var sRet = new Object;
        sRet.result  = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//執行更新動作
function EditExcute(UserID) {
    let that = $('input[name$="popInput_VNo"]');
    if ($(that).val().trim().length != $(that).prop('maxlength')) {
        console.log($(that).val());
        alert($("#ContentPlaceHolder1_popLabel_VNo").text() + '長度必須為 ' + $(that).prop('maxlength') + "碼");
        $('input[name$="popInput_VNo"]').focus();
        return false;
    }

    if ($('input[name$="popInput_VName"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_VName").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_VName"]').focus();
        return false;
    }
    PageMethods.Update(SelectValue.value, popInput_VNo.value, popInput_VName.value.trim(), UserID, SelectNowNo.value, Excutecomplete, onPageMethodsError);
}

//******************** 刪除作業 ********************

// 呼叫刪除視窗
function CallDelete(Title, DelLabel, Msg) {
    if (IsEmpty(SelectValue.value)) {
        NotSelectForDelete(Msg);
    }
    else {
        ChangeText(L_popName2, Title);
        ChangeText(DeleteLableText2, DelLabel);
        SetMode('Delete');
        PageMethods.LoadData(SelectValue.value, hUserId.value, 'Delete', SetDeleteUI, onPageMethodsError);
    }
}

//設定資料至刪除頁面
function SetDeleteUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger2.click();

        popInput_VNo2.value   = DataArray[1];
        popInput_VName2.value = DataArray[2];
    }
    else {
        var sRet= new Object;
        sRet.result  = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//執行刪除動作
function DeleteExcute(UserID) {
    PageMethods.Delete(SelectValue.value, UserID, Excutecomplete, onPageMethodsError);
}

//******************** 完成作業 ********************

//各個動作完成
function Excutecomplete(sRet) {
    switch (sRet.result) {
        case false:
            CancelTrigger1.click();
            CancelTrigger2.click();
            alert(sRet.message);
            break;
        default:
            switch (sRet.act) {
                case "Add":
                    var data = sRet.message.split("|");
                    SelectNowNo.value = data[0];
                    SelectValue.value = data[1];
                    break;
                case "Edit":
                    SelectNowNo.value = popInput_VNo.value.trim();
                    break;
                case "Delete":
                    SelectNowNo.value = '';
                    SelectValue.value = '';
                    break;
            }

            SetMode('');
            __doPostBack(UpdatePanel1.id, 'popPagePost');
            CancelTrigger1.click();
            CancelTrigger2.click();
            break;
    }
}

//自動設置GridView位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectNowNo.value);
}

//******************************

function SelectState() {
    SelectYear.value = Input_Year.options[Input_Year.selectedIndex].value;
    //SelectValue.value = '';
    __doPostBack(QueryButton.id, 'NewQuery');
}