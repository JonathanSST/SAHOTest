// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_HEDesc.disabled = false;
            popLabel_YYYY.disabled = false;
            popInput_MM.disabled = false;
            popInput_DD.disabled = false;
            popInput_HEDesc.value = '';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            ChangeText(DeleteLableText, '');
            break;
        case 'Delete':
            popInput_YMD.disabled = true;
            popInput_Desc.disabled = true;
            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_HEDesc.disabled = false;
            popLabel_YYYY.disabled = false;
            popInput_MM.disabled = false;
            popInput_DD.disabled = false;
            popInput_HEDesc.value = '';

            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            break;
    }
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

//GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectNowNo.value);
}

//取得DropDownList物件
function YYData() {
    if (Input_Year.length == 0) {
        PageMethods.YYData('ContentPlaceHolder1_Input_Year', SetYYData, onPageMethodsError);
    }
}

//將資料帶回畫面UI
function SetYYData(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        if (DataArray[DataArray.length - 1] != '') {
            var data = DataArray[DataArray.length - 1].split("|");
            var str = data[0];
            var option = null;
            option = document.createElement("option");
            option.text = $("#ddlSelectDefault").val();
            option.value = '';
            document.getElementById(str).options.add(option);

            for (var i = 1; i < data.length; i += 2) {
                var option = null;
                option = document.createElement("option");
                option.text = data[i + 1];
                option.value = data[i];
                document.getElementById(str).options.add(option);
            }
        }
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//將資料帶回畫面UI
function SetMMData(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        if (DataArray[DataArray.length - 1] != '') {
            var data = DataArray[DataArray.length - 1].split("|");
            var str = data[0];
            var option = null;
            for (var i = 1; i < data.length; i += 2) {
                var option = null;
                option = document.createElement("option");
                option.text = data[i + 1];
                option.value = data[i];
                document.getElementById(str).options.add(option);
            }
        }
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//將資料帶回畫面UI
function SetDDData(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        if (DataArray[DataArray.length - 1] != '') {
            var data = DataArray[DataArray.length - 1].split("|");
            var str = data[0];
            var option = null;
            for (var i = 1; i < data.length; i += 2) {
                var option = null;
                option = document.createElement("option");
                option.text = data[i + 1];
                option.value = data[i];
                document.getElementById(str).options.add(option);
            }
        }
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//將資料帶回畫面UI
function SetEditUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();
        SelectNowNo.value = DataArray[1];
        ChangeText(popLabel_YYYY, DataArray[1].substr(0, 4) + ' ');
        popInput_HEDesc.value = DataArray[2];
        for (var i = 0; i < popInput_MM.children.length; i++) {
            if (popInput_MM[i].value == DataArray[1].substr(5, 2)) {
                popInput_MM.selectedIndex = i;
                break;
            }
        }
        for (var i = 0; i < popInput_DD.children.length; i++) {
            if (popInput_DD[i].value == DataArray[1].substr(8, 2)) {
                popInput_DD.selectedIndex = i;
                break;
            }
        }
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//將資料帶回畫面UI
function SetDeleteUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger2.click();
        popInput_YMD.value = DataArray[1];
        popInput_Desc.value = DataArray[2];
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

function SelectState() {
    SelectYear.value = Input_Year.options[Input_Year.selectedIndex].value;
    SelectValue.value = '';
    __doPostBack(QueryButton.id, 'NewQuery');
}

function SelectState2() {
    SelectYear.value = Input_Year.options[Input_Year.selectedIndex].value;
    SelectValue.value = '';
    __doPostBack(BulidButton.id, 'Buliding');
}

// 呼叫新增視窗
function CallAdd(Title) {
    if (IsEmpty(SelectYear.value))
        return false;

    ChangeText(L_popName1, Title);
    SetMode('Add');

    while (popInput_MM.length > 0) {
        popInput_MM.remove(0);
    }
    while (popInput_DD.length > 0) {
        popInput_DD.remove(0);
    }

    PageMethods.MMData('ContentPlaceHolder1_popInput_MM', SetMMData, onPageMethodsError);
    popInput_MM.selectedIndex = 0;
    PageMethods.DDData('ContentPlaceHolder1_popInput_DD', SetDDData, onPageMethodsError);
    popInput_DD.selectedIndex = 0;

    ChangeText(popLabel_YYYY, SelectYear.value + ' ');
    PopupTrigger1.click();
}

//執行新增動作
function AddExcute(UserID) {
    PageMethods.Insert(Input_Year.options[Input_Year.selectedIndex].value + '-' + popInput_MM.options[popInput_MM.selectedIndex].value + '-' + popInput_DD.options[popInput_DD.selectedIndex].value, popInput_HEDesc.value, UserID, Excutecomplete, onPageMethodsError);
}

// 呼叫編輯視窗
function CallEdit(Title, Msg) {
    if (IsEmpty(SelectYear.value))
        return false;

    ChangeText(L_popName1, Title);

    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else {
        SetMode('Edit');

        while (popInput_MM.length > 0) {
            popInput_MM.remove(0);
        }
        while (popInput_DD.length > 0) {
            popInput_DD.remove(0);
        }
        var ajax1 = $.ajax({
            url: location.href + "/MMData",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: "{'CtrlID':'ContentPlaceHolder1_popInput_MM'}"
        });
        var ajax2 = $.ajax({
            url: location.href + "/DDData",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: "{'CtrlID':'ContentPlaceHolder1_popInput_DD'}"
        });
        //PageMethods.MMData('ContentPlaceHolder1_popInput_MM', SetMMData, onPageMethodsError);
        //PageMethods.DDData('ContentPlaceHolder1_popInput_DD', SetDDData, onPageMethodsError);
        $.when(ajax1, ajax2).done(function (id1, id2) {
            SetMMData(id1[0]['d']);
            SetDDData(id2[0]['d']);
            PageMethods.LoadData(SelectValue.value, hUserId.value, 'Edit', SetEditUI, onPageMethodsError);
        }).fail(function () { alert('error'); });
    }
}

//執行更新動作
function EditExcute(UserID) {
    PageMethods.Update(SelectValue.value, popInput_HEDesc.value, UserID, Excutecomplete, onPageMethodsError);
}

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

//執行刪除動作
function DeleteExcute(UserID) {
    PageMethods.Delete(SelectValue.value, UserID, Excutecomplete, onPageMethodsError);
}

//各動作完成
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
                    //SelectNowNo.value = popLabel_YYYY.value + '-' + popInput_MM.value + '-' + popInput_DD.value;
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

