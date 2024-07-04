// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_No.disabled = false;
            popInput_No.value = '';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            break;
        case 'Edit':
            popInput_No.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            break;
        case 'AuthEquGr':
            popInput_No4.disabled = true;
            popB_Auth2.style.display = "inline";
            ChangeText(DeleteLableText4, '');
            break;
        case 'Delete':
            popInput_No2.disabled = true;
            popB_OrgList3.disabled = true;
            popB_OrgListStr2.disabled = true;
            popDropDownList_Char.disabled = false;
            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_No.disabled = false;
            popInput_No.value = '';
            popInput_No2.disabled = false;
            popInput_No2.value = '';
            popInput_No3.disabled = true;
            popInput_No3.value = '';
            popInput_No4.disabled = false;
            popInput_No4.value = '';
            while (popB_OrgList2.length > 0)
                popB_OrgList2.remove(0);
            while (popDropDownList_Char.length > 0)
                popDropDownList_Char.remove(0);
            while (popB_ddlClass.length > 0)
                popB_ddlClass.remove(0);
            while (popDropDownList_Char.length > 0)
                popDropDownList_Char.remove(0);
            while (popB_OrgList3.length > 0)
                popB_OrgList3.remove(0);
            while (popB_OrgList1.length > 0)
                popB_OrgList1.remove(0);
            while (popB_OrgList4.length > 0)
                popB_OrgList4.remove(0);
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            break;
    }
}

function ChangeTextSet(Msg) {
    ChangeText(DeleteLableText, '');
    ChangeText(DeleteLableText2, '');
    ChangeText(DeleteLableText3, Msg);
}

//GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {    
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectNowNo.value);
}

function SelectState() {
    SelectValue.value = '';
    __doPostBack(QueryButton.id, 'NewQuery');
}

// 呼叫新增視窗
function CallAdd(Title) {
    ChangeText(L_popName1, Title);
    SetMode('Add');
    ShowDDLData();
    PopupTrigger1.click();
}

//執行新增動作
function AddExcute(UserID) {
    var vOrgIDList = '\\';
    if (popB_OrgList2.length > 0) {
        for (var i = 0; i < popB_OrgList2.length; i++) {
            var data = popB_OrgList2.options[i].value.split("|");
            vOrgIDList += data[0] + '\\';
        }
    }
    else
        vOrgIDList = '';
    PageMethods.Insert(popInput_No.value.trim(), vOrgIDList, UserID, Excutecomplete, onPageMethodsError);
}

// 呼叫編輯視窗
function CallEdit(Title, Msg) {
    ChangeText(L_popName1, Title);
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        SetMode('Edit');
        PageMethods.LoadData('ContentPlaceHolder1_popB_ddlClass', 'ContentPlaceHolder1_popB_OrgList2', SelectValue.value, hUserId.value, 'Edit', SetEditUI, onPageMethodsError);
    }
}

//執行更新動作
function EditExcute(UserID) {
    var vOrgIDList = '\\';
    if (popB_OrgList2.length > 0) {
        for (var i = 0; i < popB_OrgList2.length; i++) {
            var data = popB_OrgList2.options[i].value.split("|");
            vOrgIDList += data[0] + '\\';
        }
    }
    else
        vOrgIDList = '';
    PageMethods.Update(SelectValue.value, SelectNowNo.value, popInput_No.value.trim(), vOrgIDList, UserID, Excutecomplete, onPageMethodsError);
}

// 呼叫刪除視窗
function CallDelete(Title, DelLabel, Msg) {
    ChangeText(L_popName2, Title);
    ChangeText(DeleteLableText2, DelLabel);
    if (IsEmpty(SelectValue.value))
        NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PageMethods.LoadData('ContentPlaceHolder1_popDropDownList_Char', 'ContentPlaceHolder1_popB_OrgList3', SelectValue.value, hUserId.value, 'Delete', SetDeleteUI, onPageMethodsError);
    }
}

//執行刪除動作
function DeleteExcute(UserID, Msg) {
    var OrgStrucID = popDropDownList_Char.options[popDropDownList_Char.selectedIndex].value;
    if (OrgStrucID != '')
        PageMethods.Delete(SelectValue.value, OrgStrucID, UserID, Excutecomplete, onPageMethodsError);
    else
        alert(Msg);
}

// 呼叫權限視窗
function CallAuth(Title, DelLabel, Msg, Type) {
    ChangeText(L_popName3, Title);
    ChangeText(DeleteLableText3, DelLabel);
    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else {
        SetMode('');

        if (Type === "Equ") {
            PageMethods.LoadEquData(SelectValue.value, SetAuthUI, onPageMethodsError);
        }
        else if (Type === "Mgn") {
            PageMethods.LoadMgaData(SelectValue.value, SetAuthUI, onPageMethodsError);
        }
    }
}

//顯示組織類型
function ShowDDLData() {
    while (popB_ddlClass.length > 0)
        popB_ddlClass.remove(0);
    var str = '';
    if (popB_OrgList2.length > 0) {
        for (var i = 0; i < popB_OrgList2.length; i++)
            str += popB_OrgList2.options[i].text + '|' + popB_OrgList2.options[i].value + '|';
    }
    PageMethods.DDLData('ContentPlaceHolder1_popB_ddlClass', 'ContentPlaceHolder1_popB_OrgList2', str, SetClassData, onPageMethodsError);
    popB_ddlClass.selectedIndex = 0;
}

//將資料帶回畫面UI
function SetClassData(DataArray) {
    console.log(DataArray);
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        if (DataArray[DataArray.length - 2] != '') {
            var data = DataArray[DataArray.length - 2].split("|");
            var str = data[0]; 
            var ctrl = document.getElementById(str);
            if (data.length > 2) {
                ctrl.disabled = false;
                var option = null;
                option = document.createElement("option");
                option.text = $('#ddlSelectDefault').val();
                option.value = '';
                ctrl.options.add(option);
                for (var i = 1; i < data.length; i += 2) {
                    var option = null;
                    option = document.createElement("option");
                    option.text = data[i + 1];
                    option.value = data[i];
                    ctrl.options.add(option);
                }
            }
            else
                ctrl.disabled = true;
        }
    }
    else {
        var data = DataArray[DataArray.length - 2].split("|");
        var str = data[0];
        var ctrl = document.getElementById(str);
        ctrl.disabled = true;
    }

    if (DataArray[DataArray.length - 1] != '') {
        var data = DataArray[DataArray.length - 1].split("|");
        var str = data[0];
        var ctrl = document.getElementById(str);
        var option = null;
        var txt = '\\';
        if (data.length > 2) {
            while (ctrl.length > 0)
                ctrl.remove(0);
            for (var i = 1; i < data.length; i += 4) {
                option = document.createElement("option");
                option.text = data[i];
                option.value = data[i + 1] + '|' + data[i + 2] + '|' + data[i + 3];
                ctrl.options.add(option);
                txt += data[i] + '\\';
            }
            popB_OrgListStr.value = txt;
            popB_OrgListStr2.value = txt;
        }
        else {
            popB_OrgListStr.value = '';
            popB_OrgListStr2.value = '';
        }
    }        
}

//取得OrgData資料
function ShowOrgList() {
    PageMethods.ShowOrgData('ContentPlaceHolder1_popB_OrgList1', popB_ddlClass.options[popB_ddlClass.selectedIndex].value, ShowOrgItem, onPageMethodsError);
}

//將取得的OrgData寫入下拉式選單內
function ShowOrgItem(item) {
    if (item[0] != '') {
        var data = item[0].split("|");
        var str = data[0];
        var myCtrl = document.getElementById(str);
        while (myCtrl.length > 0)
            myCtrl.remove(0);
        if (data[1] != '') {
            for (var i = 1; i < data.length; i += 5) {
                var option = null;
                option = document.createElement("option");
                option.text = '[' + data[i] + ']' + data[i + 1];
                option.value = data[i + 2] + '|' + data[i + 3] + '|' + data[i + 4];
                myCtrl.add(option);
            }
        }
    }
}

//控制項加入與移除的動作
function DataEnterRemove(str) {
    var option = null;
    var num = null;
    var OrgClassVal = null;
    if (str == 'Add') {
        if (popB_OrgList1.selectedIndex > -1) {
            option = document.createElement("option");
            option.text = popB_OrgList1.options[popB_OrgList1.selectedIndex].text;
            option.value = popB_OrgList1.options[popB_OrgList1.selectedIndex].value;
            popB_OrgList2.add(option);
            num = option.value.split("|");
            while (popB_OrgList1.length > 0)
                popB_OrgList1.remove(0);
            ShowDDLData();
        }
    }
    else if (str == 'Del') {
        if (popB_OrgList2.selectedIndex > -1) {
            if (popB_ddlClass.length > 0)
                OrgClassVal = popB_ddlClass.options[popB_ddlClass.selectedIndex].value;
            option = document.createElement("option");
            option.text = popB_OrgList2.options[popB_OrgList2.selectedIndex].text;
            option.value = popB_OrgList2.options[popB_OrgList2.selectedIndex].value;
            num = option.value.split("|");
            if (popB_ddlClass.length > 0)
                if (num[2] == popB_ddlClass.options[popB_ddlClass.selectedIndex].value)
                    popB_OrgList1.add(option);
            popB_OrgList2.remove(popB_OrgList2.selectedIndex);
            while (popB_OrgList1.length > 0)
                popB_OrgList1.remove(0);
            ShowDDLData();
        }
    }
}

//各動作完成
function Excutecomplete(sRet) {
    switch (sRet.result) {
        case false:
            CancelTrigger1.click();
            CancelTrigger2.click();
            CancelTrigger4.click();
            alert(sRet.message);
            break;
        default:
            switch (sRet.act) {
                case "Add":
                    var data = sRet.message.split("|");
                    SelectNowNo.value = data[2];
                    SelectValue.value = data[1];
                    break;
                case "Edit":                    
                    SelectNowNo.value = popInput_No.value.trim();
                    break;
                case "AuthUpdate":
                    SelectNowNo.value = popInput_No4.value.trim();
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
            CancelTrigger4.click();
            break;
    }
}

//將資料帶回畫面UI
function SetEditUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();
        SelectNowNo.value = DataArray[1];
        popInput_No.value = DataArray[1];        
        SetClassData(DataArray);
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
        SelectNowNo.value = DataArray[1];
        popInput_No2.value = DataArray[1];
        SetClassData(DataArray);
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//將資料帶回畫面UI
function SetAuthUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger3.click();
        SelectNowNo.value = DataArray[0];
        popInput_No3.value = DataArray[0];
        while (popB_OrgList4.length > 0)
            popB_OrgList4.remove(0);
        if (DataArray[1] != '') {
            var data = DataArray[1].split("|");
            for (var i = 0; i < data.length; i += 3) {
                var option = null;
                option = document.createElement("option");
                option.text = '[' + data[i] + ']' + data[i + 1] + '(' + data[i + 2] + ')';
                option.value = '';
                popB_OrgList4.add(option);
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

//設定使用者按鈕的權限
function SetUserLevel(str) {
    AddButton.disabled = true;
    EditButton.disabled = true;
    DeleteButton.disabled = true;
    AuthButton.disabled = true;
    AuthEquGrButton.disabled = true;
    if (str != '') {
        var data = str.split(",");
        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'Add')
                AddButton.disabled = false;
            if (data[i] == 'Edit')
                EditButton.disabled = false;
            if (data[i] == 'Del')
                DeleteButton.disabled = false;
            if (data[i] == 'Auth') {
                AuthButton.disabled = false;
                AuthEquGrButton.disabled = false;
            }
        }
    }
}

// 呼叫設備群組視窗
function CallAuthEquGr(Title, Msg) {
    ChangeText(L_popName4, Title);

    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else {
        //SetMode('AuthEquGr');
        //PageMethods.LoadEquGrList(SelectValue.value, ShowEquGrList, onPageMethodsError);
        //加入自訂的pop function,開啟設備群組設定
        SetOrgGroupEdit();
    }
}

function ShowEquGrList(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        while (popB_EquGrList1.length > 0) {
            popB_EquGrList1.remove(0);
        }

        while (popB_EquGrList2.length > 0) {
            popB_EquGrList2.remove(0);
        }

        SelectNowNo.value = DataArray[0];
        popInput_No4.value = DataArray[0];

        if (DataArray[DataArray.length - 2] != '') {
            var data = DataArray[DataArray.length - 2].split("|");
            var option = null;

            if (data.length > 0) {
                for (var i = 0; i < data.length; i += 3) {
                    option = document.createElement("option");
                    option.text = '[' + data[i + 1] + ']' + data[i + 2];
                    option.value = data[i];
                    popB_EquGrList1.options.add(option);
                }
            }
        }

        if (DataArray[DataArray.length - 1] != '') {
            var data = DataArray[DataArray.length - 1].split("|");
            var option = null;

            if (data.length > 0) {
                for (var i = 0; i < data.length; i += 3) {
                    option = document.createElement("option");
                    option.text = '[' + data[i + 1] + ']' + data[i + 2];
                    option.value = data[i];
                    popB_EquGrList2.options.add(option);
                }
            }
        }

        PopupTrigger4.click();
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//執行設備群組更新動作
function AuthExcute(UserID) {
    var data = '';
    if (popB_EquGrList2.length > 0) {
        for (var i = 0; i < popB_EquGrList2.length; i++) {
            data += popB_EquGrList2.options[i].value + '|';
        }
    }
    PageMethods.AuthUpdate(SelectValue.value, data, UserID, Excutecomplete, onPageMethodsError);
}

//控制項加入與移除的動作
function DataEnterRemove2(str) {
    var option = null;
    var num = null;
    if (str == 'Add') {
        if (popB_EquGrList1.selectedIndex > -1) {
            option = document.createElement("option");
            option.text = popB_EquGrList1.options[popB_EquGrList1.selectedIndex].text;
            option.value = popB_EquGrList1.options[popB_EquGrList1.selectedIndex].value;
            popB_EquGrList2.add(option);
            popB_EquGrList1.remove(popB_EquGrList1.selectedIndex);
        }
    }
    else if (str == 'Del') {
        if (popB_EquGrList2.selectedIndex > -1) {
            option = document.createElement("option");
            option.text = popB_EquGrList2.options[popB_EquGrList2.selectedIndex].text;
            option.value = popB_EquGrList2.options[popB_EquGrList2.selectedIndex].value;
            popB_EquGrList1.add(option);
            popB_EquGrList2.remove(popB_EquGrList2.selectedIndex);
        }
    }
}