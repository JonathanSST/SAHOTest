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
        }
    }
}

//執行「查詢」動作
function SelectState() {
    Block();
    __doPostBack(QueryButton.id, '');
}

//呼叫新增視窗
function CallAdd(Title) {
    L_popName1.innerText = Title;
    popTxt_LogEquGrpID.disabled = false;
    hMode.value = "Insert";
    PopupTrigger1.click();
    PageMethods.LoadData("", SetUI, onPageMethodsError);
}

//呼叫編輯視窗
function CallEdit(Title, Msg) {
    L_popName1.innerText = Title;
    popTxt_LogEquGrpID.disabled = true;
    hMode.value = "Update";

    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else {
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

//呼叫刪除視窗
function CallDelete(Title, Msg, DelLabel) {
    L_popName1.innerText = Title;
    DeleteLableText.innerText = DelLabel;
    popTxt_LogEquGrpID.disabled = false;
    hMode.value = "Delete";

    if (IsEmpty(SelectValue.value)) {
        NotSelectForDelete(Msg);
    }
    else {
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

//將資料帶回畫面UI
function SetUI(DataArray) {
    while (popB_EquList1.length > 0)
        popB_EquList1.remove(0);

    while (popB_EquList2.length > 0)
        popB_EquList2.remove(0);

    if (DataArray[0] !== "") {
        var arrGroup = DataArray[0].split('|');
        popTxt_LogEquGrpID.value = arrGroup[0];
        popTxt_LogEquGrpName.value = arrGroup[1];
    }
    else {
        popTxt_LogEquGrpID.value = "";
        popTxt_LogEquGrpName.value = "";
    }

    if (DataArray[1] !== "") {
        var arrSource = DataArray[1].split(',');

        for (i = 0; i < arrSource.length; i++) {
            var data = arrSource[i].split("|");
            popB_EquList1.add(new Option(data[1], data[0]));
        }
    }

    if (DataArray[2] !== "") {
        var arrSet = DataArray[2].split(',');

        for (i = 0; i < arrSet.length; i++) {
            var data = arrSet[i].split("|");
            popB_EquList2.add(new Option(data[1], data[0]));
        }
    }
}

//執行新增動作
function SaveExcute() {
    var vEquList = "";

    if (popB_EquList2.length > 0) {
        for (var i = 0; i < popB_EquList2.length; i++) {
            var data = popB_EquList2.options[i].value;
            vEquList += data + "|";
        }
    }
    else
        vEquList = '';

    vEquList = vEquList.substring(0, vEquList.length - 1);
    PageMethods.Save(popTxt_LogEquGrpID.value, popTxt_LogEquGrpName.value, vEquList, hMode.value, Excutecomplete, onPageMethodsError);
}

//控制項加入與移除的動作
function DataJoinRemove(str) {
    var option = null;

    if (str == 'Add') {
        for (var i = 0; i < popB_EquList1.options.length; i++) {
            if (popB_EquList1.options[i].selected) {
                popB_EquList2.add(new Option(popB_EquList1.options[i].text, popB_EquList1.options[i].value));
            }
        }

        for (var i = popB_EquList1.options.length - 1; i >= 0; i--) {
            if (popB_EquList1.options[i].selected) {
                popB_EquList1.remove(i);
            }
        }
    }
    else if (str == 'Del') {
        for (var i = 0; i < popB_EquList2.options.length; i++) {
            if (popB_EquList2.options[i].selected) {
                popB_EquList1.add(new Option(popB_EquList2.options[i].text, popB_EquList2.options[i].value));
            }
        }

        for (var i = popB_EquList2.options.length - 1; i >= 0; i--) {
            if (popB_EquList2.options[i].selected) {
                popB_EquList2.remove(i);
            }
        }
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
            __doPostBack(UpdatePanel1.id, 'popPagePost');
            CancelTrigger1.click();
            alert(sRet.message);
            break;
    }
}