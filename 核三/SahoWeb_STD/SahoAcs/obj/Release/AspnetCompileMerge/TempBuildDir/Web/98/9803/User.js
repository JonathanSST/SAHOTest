//*** 主共用元件：相關的方法 ***

//主作業畫面：####################################################################
function SelectState() {
    __doPostBack(QueryButton.id, '');
}

//GridView 自動設置位置(動作在 xObj.js 中) - r####################################################################
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectValue.value);
}

//DropDownList 選取 - r####################################################################
function DropDownListSelected(Obj, opMode) {
    if ((Obj != "") && (Obj != null)) {
        if (opMode == "-")
            document.getElementById('ContentPlaceHolder1_' + Obj).selectedIndex = 0;
        else if (opMode == "+")
            document.getElementById('ContentPlaceHolder1_' + Obj).selectedIndex = 1;
    }
}

//CheckBox 選取 - r####################################################################
function CheckBoxSelected(Obj) {
    if ((Obj != "") && (Obj != null)) {
        if (document.getElementById('ContentPlaceHolder1_' + Obj).checked)
            document.getElementById('ContentPlaceHolder1_' + Obj).checked = false;
        else
            document.getElementById('ContentPlaceHolder1_' + Obj).checked = true;
    }
}

//CheckBox 全選或全取消 - r####################################################################
function Checkall(input1, input2) {
    var objForm = document.forms[input1];
    var objLen = objForm.length;

    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "checkbox")
            objForm.elements[iCount].checked = false;
    }
}

//各動作完成 - r ？####################################################################
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                case "Edit":
                    SelectValue.value = popInput_ID.value;
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
            CancelTrigger3.click();
            CancelTrigger4.click();
            break;
        case false:
            CancelTrigger1.click();
            CancelTrigger2.click();
            CancelTrigger3.click();
            CancelTrigger4.click();
            alert(objRet.message);
            break;
        default:
            break;
    }
}

//各共用函數：使用者資料作業功能


//各共用元件


//******* 主作業畫面：使用者資料作業相關的方法 *******

//主作業畫面：呼叫使用者資料新增視窗
function CallAdd(title) {
    ChangeText(L_popName1, title);

    SetMode('Add');
    PopupTrigger1.click();
}

//主作業畫面： 呼叫使用者資料編輯視窗
function CallEdit(title, Msg) {
    ChangeText(L_popName1, title);

    if (IsEmpty(SelectValue.value))
        NotSelectForEdit(Msg);
    else {
        SetMode('Edit');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

//主作業畫面： 呼叫使用者資料刪除視窗
function CallDelete(title, Msg,Msg2) {
    ChangeText(L_popName1, title);
    ChangeText(DeleteLableText, Msg2);

    if (IsEmpty(SelectValue.value))
        NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

//主作業畫面：呼叫使用者功能清單權限設定視窗
function CallUserMenusAuth(title, Msg) {
    ChangeText(L_popName2, title);

    if (IsEmpty(SelectValue.value))
        NotSelectForEdit(Msg);
    else {
        PopupTrigger2.click();
        PageMethods.LoadUserMenusAuthTable(SelectValue.value, SetUserMenusAuthUI, onPageMethodsError);
    }
}

//主作業畫面：呼叫使用者角色清單權限設定視窗
function CallUserRolesAuth(title, Msg) {
    ChangeText(L_popName3, title);

    if (IsEmpty(SelectValue.value))
        NotSelectForEdit(Msg);
    else {
        PopupTrigger3.click();
        PageMethods.LoadUserRolesAuthTable(SelectValue.value, SetUserRolesAuthUI, onPageMethodsError);
    }
}

//主作業畫面：呼叫使用者管理區清單權限設定視窗
function CallUserMgnsAuth(title, Msg) {
    ChangeText(L_popName4, title);

    if (IsEmpty(SelectValue.value))
        NotSelectForEdit(Msg);
    else {
        PopupTrigger4.click();
        PageMethods.LoadUserMgnsAuthTable(SelectValue.value, SetUserMgnsAuthUI, onPageMethodsError);
    }
}

//******* 次作業畫面一：使用者資料新增與編輯視窗相關的方法 *******

//次作業畫面一：執行新增動作
function AddExcute() {
    PageMethods.Insert(popInput_ID.value, popInput_PW.value, popInput_Name.value, popInput_EName.value, popInput_STime.value, popInput_ETime.value, popInput_PWChgTime.value, popInput_IsOptAllow.value, popInput_EMail.value, popInput_Remark.value, Excutecomplete, onPageMethodsError);
}

//次作業畫面一：執行編輯動作
function EditExcute() {
    PageMethods.Update(SelectValue.value, popInput_ID.value, popInput_PW.value, popInput_Name.value, popInput_EName.value, popInput_PWCtrl.value, popInput_STime.value, popInput_ETime.value, popInput_PWChgTime.value, popInput_IsOptAllow.value, popInput_EMail.value, popInput_Remark.value, popInput_ID.value, Excutecomplete, onPageMethodsError);
}

//次作業畫面一：執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(SelectValue.value, Excutecomplete, onPageMethodsError);
}

//次作業畫面一：將資料帶回畫面UI ####################################################################
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput_ID.value = DataArray[1];
        popInput_PW.value = DataArray[2];
        popInput_Name.value = DataArray[3];
        popInput_EName.value = DataArray[4];
        popInput_PWCtrl.value = DataArray[5];
        //popInput_STime.value = ConvertDateToStr(new Date(DataArray[6]));
        //popInput_ETime.value = ConvertDateToStr(new Date(DataArray[7]));
        //popInput_PWChgTime.value = ConvertDateToStr(new Date(DataArray[8]));
        popInput_STime.value = DataArray[6];
        popInput_ETime.value = DataArray[7];
        popInput_PWChgTime.value = DataArray[8];
        popInput_IsOptAllow.value = DataArray[9];
        popInput_EMail.value = DataArray[10];
        popInput_Remark.value = ((typeof (data) !== 'undefined') && (data !== null) ? DataArray[11] : "");
        if (DataArray[1] === "User") {
            $('input[name*="popB_Edit"]').prop("disabled", true);
            $('input[name*="popB_Delete"]').prop("disabled", true);
        } else {
            $('input[name*="popB_Edit"]').prop("disabled", false);
            $('input[name*="popB_Delete"]').prop("disabled", false);
        }
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

//次作業畫面一：依照模式設定各按鈕的啟用狀態 - ####################################################################
function SetMode(sMode) {
    console.log(sMode);
    switch (sMode) {
        case 'Add':
            //Disabled
            popInput_ID.disabled = false;
            popInput_PW.disabled = false;
            popInput_Name.disabled = false;
            popInput_EName.disabled = false;
            popInput_PWCtrl.disabled = false;
            popInput_STime.disabled = false;
            popInput_ETime.disabled = false;
            popInput_PWChgTime.disabled = false;
            popInput_IsOptAllow.disabled = false;
            popInput_EMail.disabled = false;
            popInput_Remark.disabled = false;
            //Value
            popInput_ID.value = '';
            popInput_PW.value = '';
            popInput_Name.value = '';
            popInput_EName.value = '';
            popInput_PWCtrl.value = '';
            //popInput_STime.value = '';
            //popInput_ETime.value = '';
            //popInput_PWChgTime.value = '';
            popInput_IsOptAllow.value = '1';
            popInput_EMail.value = '';
            popInput_Remark.value = '';
            //Display
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            //LabelText
            ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            //Disabled
            popInput_ID.disabled = false;
            popInput_PW.disabled = false;
            popInput_Name.disabled = false;
            popInput_EName.disabled = false;
            popInput_PWCtrl.disabled = false;
            popInput_STime.disabled = false;
            popInput_ETime.disabled = false;
            popInput_PWChgTime.disabled = false;
            popInput_IsOptAllow.disabled = false;
            popInput_EMail.disabled = false;
            popInput_Remark.disabled = false;
            //Display
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            //LabelText
            ChangeText(DeleteLableText, '');
            break;
        case 'Delete':
            //Disabled
            popInput_ID.disabled = true;
            popInput_PW.disabled = true;
            popInput_Name.disabled = true;
            popInput_EName.disabled = true;
            popInput_PWCtrl.disabled = true;
            popInput_STime.disabled = true;
            popInput_ETime.disabled = true;
            popInput_PWChgTime.disabled = true;
            popInput_IsOptAllow.disabled = true;
            popInput_EMail.disabled = true;
            popInput_Remark.disabled = true;
            //Display
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            break;
        case '':
            //Disabled
            popInput_ID.disabled = false;

            popInput_PW.disabled = false;
            popInput_Name.disabled = false;
            popInput_EName.disabled = false;
            popInput_PWCtrl.disabled = false;
            popInput_STime.disabled = false;
            popInput_ETime.disabled = false;
            popInput_PWChgTime.disabled = false;
            popInput_IsOptAllow.disabled = false;
            popInput_EMail.disabled = false;
            popInput_Remark.disabled = false;
            //Value
            popInput_ID.value = '';
            popInput_PW.value = '';
            popInput_Name.value = '';
            popInput_EName.value = '';
            popInput_PWCtrl.value = '';
            //popInput_STime.value = '';
            //popInput_ETime.value = '';
            //popInput_PWChgTime.value = '';
            popInput_IsOptAllow.value = '1';
            popInput_EMail.value = '';
            popInput_Remark.value = '';
            //Display
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            break;
    }
}

//******* 次作業畫面二：使用者功能清單權限設定視窗相關的方法 *******

//次作業畫面二：清除使用者功能清單權限設定視窗相關的資料內容
function ReSetUserMenusAuth() {
    Checkall("form1", this);

    var oPanel = popUserMenusAuthPanel;
    oPanel.scrollTop = 0;
}

//次作業畫面二：取回使用者功能清單權限設定視窗相關的資料內容
function SetUserMenusAuthUI(DataArray) {
    Checkall("form1", this);
    $('[name*="UserMenusOPMode"]').val("-");
    var MenuID;
    var ActName;
    var OPMode;
    var Index = 0;
    L_popName2.innerText = "「" + SelectNowName.value + "」" + L_popName2.innerText;
    for (i in DataArray) {
        if (Index > 2)
            Index = 0;
        if (DataArray[i] == "None") {
            Index = 0;
            continue;            
        }
        if (Index == 0) {
            //取得MenuID
            MenuID = DataArray[i];
        }
        else if (Index == 1) {
            //設定功能選項每個權限類別CheckBox元件的勾選狀態
            var ActArray = DataArray[i].split(",");

            for (x in ActArray) {
                ActName = ActArray[x];

                if (MenuID != "" && MenuID != null) {
                    if (ActName != "" || ActName != null) {
                        //設定CheckBox元件被勾選的代碼
                        CheckBoxSelected(MenuID + "_" + ActName);
                    }
                }
            }
        }
        else if (Index == 2) {
            //設定功能選項的DropDownList元件的選擇狀態
            OPMode = DataArray[i];

            if (OPMode != "" && OPMode != null)
                DropDownListSelected(MenuID + "_" + "UserMenusOPMode", OPMode);
        }

        Index++;
    }
    $('input[type="checkbox"]').css("width", 26).css("height", 26);
}

//次作業畫面二：儲存使用者功能清單權限設定視窗相關的資料內容
function UserMenusAuthSaveExcute() {
    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var checkcount = 0;

    //取得UserMenusAuth的pop頁上DropDownList元件數量
    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "select-one") {
            var param = objForm.elements[iCount].id.split('_');

            if (param[2] == "UserMenusOPMode")
                checkcount++;
        }
    }

    var checkIndex = 0;
    var DropDownIndex = -1;
    var Authop = new Array(checkcount);
    var Authid = new Array(checkcount);
    var Authact = new Array(checkcount);
    var Authchecked = new Array(checkcount);

    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "select-one") {
            //取得網頁的DropDownList元件
            var IdAct = objForm.elements[iCount].id.split('_');

            //判斷是否UserMenusAuth頁的DropDownList元件
            if ((IdAct[2] == "UserMenusOPMode")) {
                DropDownIndex++;

                if (objForm.elements[iCount].selectedIndex == 0)
                    Authop[DropDownIndex] = "-";
                else if (objForm.elements[iCount].selectedIndex == 1)
                    Authop[DropDownIndex] = "+";
                else
                    Authop[DropDownIndex] = "*";
            }
        }
        else if (objForm.elements[iCount].type == "checkbox") {
            //取得網頁的CheckBox元件
            var IdAct = objForm.elements[iCount].id.split('_');

            //判斷是否UserMenusAuth頁的CheckBox元件
            if ((IdAct[2] != "UserRolesAuth") && (IdAct[2] != "UserMgnsAuth")) {
                if ((Authid[checkIndex] != IdAct[1]) && (Authid[checkIndex] != null))
                    checkIndex++;

                //設定相關的參數內容
                if (Authid[checkIndex] == null)
                    Authid[checkIndex] = IdAct[1];

                if (Authact[checkIndex] == null)
                    Authact[checkIndex] = IdAct[2];
                else
                    Authact[checkIndex] += "," + IdAct[2];

                if (Authchecked[checkIndex] == null)
                    Authchecked[checkIndex] = objForm.elements[iCount].checked;
                else
                    Authchecked[checkIndex] += "," + objForm.elements[iCount].checked;
            }
        }
    }

    PageMethods.SaveUserMenusAuth(SelectValue.value, Authid, Authop, Authact, Authchecked, Excutecomplete, onPageMethodsError);
}

//******* 次作業畫面三：使用者角色清單權限設定視窗相關的方法 *******

//次作業畫面三：清除使用者角色清單權限設定視窗相關的資料內容
function ReSetUserRolesAuth() {
    Checkall("form1", this);

    var oPanel = popUserRolesAuthPanel;
    oPanel.scrollTop = 0;
}

//次作業畫面三：取回使用者角色清單權限設定視窗相關的資料內容
function SetUserRolesAuthUI(DataArray) {
    Checkall("form1", this);

    var RoleID;
    var ActName;
    L_popName3.innerText = "「" + SelectNowName.value + "」" + L_popName3.innerText;

    for (i in DataArray) {
        if (i % 2 == 0)
            RoleID = DataArray[i];
        else {
            ActName = DataArray[i];

            if (RoleID == "" || RoleID == null)
                return;

            if (ActName == "" || ActName == null)
                return;

            //設定CheckBox元件被勾選的代碼
            CheckBoxSelected(RoleID + "_" + ActName);
        }
    }
    
}

//次作業畫面三：儲存使用者角色清單權限設定視窗相關的資料內容
function UserRolesAuthSaveExcute() {
    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var checkcount = 0;

    //取得UserRolesAuth的pop頁上CheckBox元件數量
    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "checkbox") {
            var param = objForm.elements[iCount].id.split('_');

            if (param[2] == "UserRolesAuth")
                checkcount++;
        }
    }

    var Authid = new Array(checkcount);
    var Authact = new Array(checkcount);
    var Authchecked = new Array(checkcount);

    checkcount = -1;
    for (var iCount = 0; iCount < objLen; iCount++) {
        //取得網頁的CheckBox元件
        if (objForm.elements[iCount].type == "checkbox") {
            var IdAct = objForm.elements[iCount].id.split('_');

            //判斷是否UserRolesAuth頁的CheckBox元件
            if (IdAct[2] == "UserRolesAuth") {
                checkcount++;

                //設定相關的參數內容
                Authid[checkcount] = IdAct[1];
                Authact[checkcount] = IdAct[2];
                Authchecked[checkcount] = objForm.elements[iCount].checked;
            }
        }
    }

    PageMethods.SaveUserRolesAuth(SelectValue.value, Authid, Authact, Authchecked, Excutecomplete, onPageMethodsError);
}

//******* 次作業畫面四：使用者管理區清單權限設定視窗相關的方法 *******

//次作業畫面四：清除使用者管理區清單權限設定視窗相關的資料內容
function ReSetUserMgnsAuth() {
    Checkall("form1", this);

    var oPanel = popUserMgnsAuthPanel;
    oPanel.scrollTop = 0;
}

//次作業畫面四：取回使用者管理區清單權限設定視窗相關的資料內容
function SetUserMgnsAuthUI(DataArray) {
    Checkall("form1", this);

    var MgaID;
    var ActName;
    L_popName4.innerText = "「" + SelectNowName.value + "」" + L_popName4.innerText;
    for (i in DataArray) {
        if (i % 2 == 0)
            MgaID = DataArray[i];
        else {
            ActName = DataArray[i];

            if (MgaID == "" || MgaID == null)
                return;

            if (ActName == "" || ActName == null)
                return;

            //設定CheckBox元件被勾選的代碼
            CheckBoxSelected(MgaID + "_" + ActName);
        }
    }
}

//次作業畫面四：儲存使用者管理區清單權限設定視窗相關的資料內容
function UserMgnsAuthSaveExcute() {
    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var checkcount = 0;

    //取得UserMgnsAuth的pop頁上CheckBox元件數量
    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "checkbox") {
            var param = objForm.elements[iCount].id.split('_');

            if (param[2] == "UserMgnsAuth")
                checkcount++;
        }
    }

    var Authid = new Array(checkcount);
    var Authact = new Array(checkcount);
    var Authchecked = new Array(checkcount);

    checkcount = -1;
    for (var iCount = 0; iCount < objLen; iCount++) {
        //取得網頁的CheckBox元件
        if (objForm.elements[iCount].type == "checkbox") {
            var IdAct = objForm.elements[iCount].id.split('_');

            //判斷是否UserMgnsAuth頁的CheckBox元件
            if (IdAct[2] == "UserMgnsAuth") {
                checkcount++;

                //設定相關的參數內容
                Authid[checkcount] = IdAct[1];
                Authact[checkcount] = IdAct[2];
                Authchecked[checkcount] = objForm.elements[iCount].checked;
            }
        }
    }

    PageMethods.SaveUserMgnsAuth(SelectValue.value, Authid, Authact, Authchecked, Excutecomplete, onPageMethodsError);
}


function SetNowName(NowName) {
    SelectNowName.value = NowName;
}


function SetCheckMax() {
    if (parseInt($('input[name*="CurrentUser"]:eq(0)').val()) >= parseInt($('input[name*="MaxUser"]:eq(0)').val())) {
        $('[name*="AddButton"]').prop('disabled', true);
    } else {
        $('[name*="AddButton"]').prop('disabled', false);
    }
}



//設定使用者按鈕的權限
function SetUserLevel(str) {
    var data = str.split(",");
    $('[name*="AddButton"]').prop('disabled', true);
    $('[name*="EditButton"]').prop('disabled', true);
    $('[name*="DeleteButton"]').prop('disabled', true);
    $('[name*="MenuButton"]').prop('disabled', true);
    $('[name*="RoleButton"]').prop('disabled', true);
    $('[name*="MgnaButton"]').prop('disabled', true);
    for (var i = 0; i < data.length; i++) {
        if (data[i] == 'Add') {
            $('[name*="AddButton"]').prop('disabled', false);
            SetCheckMax();
        }
        if (data[i] == 'Edit') {
            $('[name*="EditButton"]').prop('disabled', false);
        }
        if (data[i] == 'Del') {
            $('[name*="DeleteButton"]').prop('disabled', false);
        }
        if (data[i] == "Auth") {
            $('[name*="MenuButton"]').prop('disabled', false);
            $('[name*="RoleButton"]').prop('disabled', false);
            $('[name*="MgnaButton"]').prop('disabled', false);
        }
    }
}//完成權限判斷    