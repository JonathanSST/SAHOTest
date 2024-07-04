function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_CNo.disabled     = false;
            popInput_CName.disabled   = false;
            popInput_WBTime.disabled  = false;
            popInput_WETime.disabled  = false;
            popInput_BERange.disabled = false;
            //popInput_IsCOW.disabled   = false;
            popInput_WTMin.disabled   = false;
            //popInput_RTType.disabled  = false;

            popInput_CNo.value     = '';
            popInput_CName.value   = '';
            popInput_WBTime.value  = '08:30';
            popInput_WETime.value  = '17:00';
            popInput_BERange.value = '30';
            //popInput_IsCOW.value   = '1';
            popInput_WTMin.value   = '480';
            //popInput_RTType.value  = '1';

            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            popInput_CNo.disabled     = false;
            popInput_CName.disabled   = false;
            popInput_WBTime.disabled  = false;
            popInput_WETime.disabled  = false;
            popInput_BERange.disabled = false;
            //popInput_IsCOW.disabled   = false;
            popInput_WTMin.disabled   = false;
            //popInput_RTType.disabled  = false;

            popB_Add.style.display  = "none";
            popB_Edit.style.display = "inline";
            ChangeText(DeleteLableText, '');
            break;
        case 'Delete':           
            popInput_CNo2.disabled     = true;
            popInput_CName2.disabled   = true;
            popInput_WBTime2.disabled  = true;
            popInput_WETime2.disabled  = true;
            popInput_BERange2.disabled = true;
            //popInput_IsCOW2.disabled   = true;
            popInput_WTMin2.disabled   = true;
            //popInput_RTType2.disabled  = true;

            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_CNo.disabled     = false;
            popInput_CName.disabled   = false;
            popInput_WBTime.disabled  = false;
            popInput_WETime.disabled  = false;
            popInput_BERange.disabled = false;
            //popInput_IsCOW.disabled   = false;
            popInput_WTMin.disabled   = false;
            //popInput_RTType.disabled  = false;

            popInput_CNo.value     = '';
            popInput_CName.value   = '';
            popInput_WBTime.value  = '';
            popInput_WETime.value  = '';
            popInput_BERange.value = '';
            //popInput_IsCOW.value   = '';
            popInput_WTMin.value   = '';
            //popInput_RTType.value  = '';

            popB_Add.style.display    = "none";
            popB_Edit.style.display   = "none";
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

//******************** 新增作業 ********************

//呼叫新增視窗
function CallAdd(Title) {
    ChangeText(L_popName1, Title);
    SetMode('Add');
    PopupTrigger1.click();
}

//執行新增動作
function AddExcute(UserID) {
    if ($('input[name$="popInput_CNo"]').val().trim() == "") {
        console.log($('input[name$="popInput_CNo"]').val());
        alert($("#ContentPlaceHolder1_popLabel_CNo").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_CNo"]').focus();
        return false;
    }

    if ($('input[name$="popInput_CName"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_CName").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_CName"]').focus();
        return false;
    }

    if ($('input[name$="popInput_WBTime"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_WBTime").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_WBTime"]').focus();
        return false;
    }

    if ($('input[name$="popInput_WETime"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_WETime").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_WETime"]').focus();
        return false;
    }

    if ($('input[name$="popInput_BERange"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_BERange").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_BERange"]').focus();
        return false;
    }
    /*
    if ($('input[name$="popInput_IsCOW"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_IsCOW").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_IsCOW"]').focus();
        return false;
    }
    */

    if ($('input[name$="popInput_WTMin"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_WTMin").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_WTMin"]').focus();
        return false;
    }

    /*
    if ($('input[name$="popInput_RTType"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_RTType").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_RTType"]').focus();
        return false;
    }
    */
    PageMethods.Insert(popInput_CNo.value.trim(), popInput_CName.value, popInput_WBTime.value, popInput_WETime.value, popInput_BERange.value,'1', popInput_WTMin.value,'1', UserID, Excutecomplete, onPageMethodsError);
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

        popInput_CNo.value     = DataArray[1];
        popInput_CName.value   = DataArray[2];
        popInput_WBTime.value  = DataArray[3];
        popInput_WETime.value  = DataArray[4];
        popInput_BERange.value = DataArray[5];
        //popInput_IsCOW.value   = DataArray[6];
        popInput_WTMin.value   = DataArray[7];
        //popInput_RTType.value  = DataArray[8];
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
    PageMethods.Update(SelectValue.value, popInput_CNo.value, popInput_CName.value.trim(), popInput_WBTime.value.trim(), popInput_WETime.value.trim(), popInput_BERange.value.trim(),'1', popInput_WTMin.value.trim(),'1', UserID, Excutecomplete, onPageMethodsError);
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

        popInput_CNo2.value     = DataArray[1];
        popInput_CName2.value   = DataArray[2];
        popInput_WBTime2.value  = DataArray[3];
        popInput_WETime2.value  = DataArray[4];
        popInput_BERange2.value = DataArray[5];
        popInput_IsCOW2.value   = DataArray[6];
        //popInput_WTMin2.value   = DataArray[7];
        //popInput_RTType2.value  = DataArray[8];
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
                    SelectNowNo.value = popInput_CNo.value.trim();
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