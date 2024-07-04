//******* 各公用方法：執行臨時卡借還作業相關的公用方法 *******

//*各公用方法：選擇查詢狀態
function SelectState() {
    __doPostBack(QueryButton.id, '');
}

//*各公用方法：自動設置GridView控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {

    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectValue.value);
}

//******* 主作業畫面：呼叫臨時卡借還作業相關的視窗方法 *******

//*主作業畫面：呼叫新增視窗
function CallAdd(Title) {
    ChangeText(L_popName1, Title);
    SetMode('Add');

    hRecordID.value = SelectValue.value;

    PopupTrigger1.click();
}

//*主作業畫面：呼叫編輯視窗
function CallEdit(Title, Msg) {
    ChangeText(L_popName1, Title);

    if (IsEmpty(SelectValue.value))
        NotSelectForEdit(Msg);
    else {
        SetMode('Edit');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, 'Edit', SetUI, onPageMethodsError);
    }
}

//*主作業畫面：呼叫刪除視窗
function CallDelete(Title, DelLabel, Msg) {
    ChangeText(L_popName1, Title);
    ChangeText(DeleteLableText, DelLabel);

    if (IsEmpty(SelectValue.value))
        NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, 'Delete', SetUI, onPageMethodsError);
    }
}

//主作業畫面：輸入模式設定視窗相關的欄位狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            //Value
            popInput_PsnNo.value      = '';
            popInput_PsnName.value    = '';
            popInput_OrigCardNo.value = '';
            popInput_CardNo.value     = '';
            popInput_BorrowTime.value = GetNowDate();
            popInput_ReturnTime.value = '';
            popInput_TempDesc.value   = '';

            //Disabled
            popInput_PsnNo.disabled       = false;
            popInput_PsnName.disabled     = true;
            popInput_OrigCardNo.disabled  = true;

            popInput_CardNo.disabled = true;
            ddlCardNo.disabled = false;

            popInput_BorrowTime.disabled  = false;
            popInput_ReturnTime.disabled  = true;
            popInput_TempDesc.disabled = false;

            //Display
            popLabel_PsnName.style.display = "inline";
            popInput_PsnName.style.display = "inline";
            popB_Add.style.display         = "inline";
            popB_Edit.style.display        = "none";
            popB_Delete.style.display = "none";

            popInput_CardNo.style.display = "none";
            ddlCardNo.style.display = "inline";
            
            document.getElementById('th_popInput_ReturnTime').style.display = "none";
            document.getElementById('td_popInput_ReturnTime').style.display = "none";

            // 使用者控制項 popInput_BorrowTime、popInput_ReturnTime 的時鐘圖示隱藏與否
            document.getElementById('ContentPlaceHolder1_popInput_BorrowTime_Clock').style.display = "inline";
            document.getElementById('ContentPlaceHolder1_popInput_ReturnTime_Clock').style.display = "none";

            //DeleteLabelText
            ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            //Disabled
            popInput_PsnNo.disabled      = true;
            popInput_PsnName.disabled    = true;
            popInput_OrigCardNo.disabled = true;

            popInput_CardNo.disabled = true;
            ddlCardNo.disabled = true;

            popInput_BorrowTime.disabled = true;
            popInput_ReturnTime.disabled = true;
            popInput_TempDesc.disabled = true;

            //Display
            popLabel_PsnName.style.display = "inline";
            popInput_PsnName.style.display = "inline";
            popB_Add.style.display         = "none";
            popB_Edit.style.display        = "inline";
            popB_Delete.style.display = "none";

            popInput_CardNo.style.display = "inline";
            ddlCardNo.style.display = "none";

            document.getElementById('th_popInput_ReturnTime').style.display = "inline";
            document.getElementById('td_popInput_ReturnTime').style.display = "inline";

            // 使用者控制項 popInput_BorrowTime、popInput_ReturnTime 的時鐘圖示隱藏與否
            document.getElementById('ContentPlaceHolder1_popInput_BorrowTime_Clock').style.display = "none";
            document.getElementById('ContentPlaceHolder1_popInput_ReturnTime_Clock').style.display = "inline";

            //DeleteLabelText
            ChangeText(DeleteLableText, '');
            break;
        case 'Delete':
            //Disabled
            popInput_PsnNo.disabled      = true;
            popInput_PsnName.disabled    = true;
            popInput_OrigCardNo.disabled = true;
            
            popInput_CardNo.disabled = true;
            ddlCardNo.disabled = true;

            popInput_BorrowTime.disabled = true;
            popInput_ReturnTime.disabled = true;
            popInput_TempDesc.disabled = true;

            //Display
            popLabel_PsnName.style.display = "inline";
            popInput_PsnName.style.display = "inline";
            popB_Add.style.display         = "none";
            popB_Edit.style.display        = "none";
            popB_Delete.style.display = "inline";

            popInput_CardNo.style.display = "inline";
            ddlCardNo.style.display = "none";

            document.getElementById('th_popInput_ReturnTime').style.display = "inline";
            document.getElementById('td_popInput_ReturnTime').style.display = "inline";

            // 使用者控制項 popInput_BorrowTime、popInput_ReturnTime 的時鐘圖示隱藏與否
            document.getElementById('ContentPlaceHolder1_popInput_BorrowTime_Clock').style.display = "none";
            document.getElementById('ContentPlaceHolder1_popInput_ReturnTime_Clock').style.display = "none";

            break;
        case '':
            //Value
            popInput_PsnNo.value      = '';
            popInput_PsnName.value    = '';
            popInput_OrigCardNo.value = '';
            popInput_CardNo.value     = '';
            popInput_BorrowTime.value = '';
            popInput_ReturnTime.value = '';
            popInput_TempDesc.value   = '';

            //Disabled
            popInput_PsnNo.disabled      = false;
            popInput_PsnName.disabled    = false;
            popInput_OrigCardNo.disabled = false;

            popInput_CardNo.disabled = true;
            ddlCardNo.disabled = true;

            popInput_BorrowTime.disabled = false;
            popInput_ReturnTime.disabled = true;
            popInput_TempDesc.disabled = false;

            //Display
            popLabel_PsnName.style.display = "none";
            popInput_PsnName.style.display = "none";
            popB_Add.style.display         = "none";
            popB_Edit.style.display        = "none";
            popB_Delete.style.display = "none";

            popInput_CardNo.style.display = "none";
            ddlCardNo.style.display = "inline";

            // 使用者控制項 popInput_BorrowTime、popInput_ReturnTime 的時鐘圖示隱藏與否
            document.getElementById('ContentPlaceHolder1_popInput_BorrowTime_Clock').style.display = "inline";
            document.getElementById('ContentPlaceHolder1_popInput_ReturnTime_Clock').style.display = "inline";

            //DeleteLabelText
            ChangeText(DeleteLableText, '');
            break;
    }
}

//主作業畫面：帶回資料設定視窗相關的欄位內容
function SetUI(DataArray) {

    if (DataArray[0] != 'Saho_SysErrorMassage') {
        var ActFunc = DataArray[0];

        hRecordID.value = DataArray[1];

        popInput_PsnNo.value      = DataArray[2];
        popInput_PsnName.value    = DataArray[8];
        popInput_OrigCardNo.value = DataArray[3];
        popInput_CardNo.value = DataArray[4];

        popInput_BorrowTime.value = DataArray[5];
        popInput_ReturnTime.value = DataArray[6];
        popInput_TempDesc.value   = DataArray[7];
       
        if (ActFunc == 'Edit') {
            ddlCardArea.value = DataArray[9];
            if (popInput_ReturnTime.value == '') {
                popInput_ReturnTime.disabled = false;
                popInput_TempDesc.disabled   = false;

                popB_Edit.style.display = "inline";                
                // 到此知道這筆是未歸還卡片的資料，在此補上現在時間，減少現場人員KEY IN
                popInput_ReturnTime.value = GetNowDate();
            }
            else { 
                popInput_ReturnTime.disabled = true;
                popInput_TempDesc.disabled   = true;

                popB_Edit.style.display = "none";

                // 確認該筆資料已經修改過了，所以歸還時間的圖示不必顯示
                document.getElementById('ContentPlaceHolder1_popInput_ReturnTime_Clock').style.display = "none";
            }
        }

        if (ActFunc == 'Delete') {
            if (popInput_ReturnTime.value == '') {
                popB_Delete.style.display     = "none";
                DeleteLableText.style.display = "none";
            }
            else {
                popB_Delete.style.display     = "inline";
                DeleteLableText.style.display = "inline";
            }
        }
    }
    else {
        var objRet = new Object;

        objRet.result = false;
        objRet.message = DataArray[1];

        Excutecomplete(objRet);
    }

}

//主作業畫面：完成動作設定網頁相關的參數內容
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                    SelectValue.value = objRet.message;
                    hRecordID.value = SelectValue.value;
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Edit":
                    SelectValue.value = hRecordID.value;
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Delete":
                    hRecordID.value = '';
                    SelectValue.value = '';
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Cancel":
                    SelectValue.value = hRecordID.value;
                    //hRecordID.value = '';
                    __doPostBack(UpdatePanel1.id, 'popPagePost');

                    //$.unblockUI();
                    break;
            }

            GetTempCardData();      // 重新讀取 ddlCardNo 的項目
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

//******* 次作業畫面一：執行臨時卡借還作業相關的動作方法 *******

//次作業畫面一：執行新增動作
function AddExcute() {
    //debugger;
    //PageMethods.Insert(popInput_PsnNo.value, popInput_OrigCardNo.value, popInput_CardNo.value, popInput_BorrowTime.value, popInput_TempDesc.value, GridSelect, onPageMethodsError);


    //CancelTrigger1.click();

    PageMethods.Insert(popInput_PsnNo.value, popInput_OrigCardNo.value, ddlCardNo.value, popInput_BorrowTime.value, popInput_TempDesc.value, Excutecomplete, onPageMethodsError);
}

//次作業畫面一：執行編輯動作
function EditExcute() {
    if ($('input[name*="popInput_BorrowTime"]').val() >= $('input[name*="popInput_ReturnTime"]').val()) {
        alert($("#AlertMsg").val())
        return false;
    }
    Block();

    PageMethods.Update(SelectValue.value, popInput_ReturnTime.value, popInput_TempDesc.value, Excutecomplete, onPageMethodsError);

    $.unblockUI();
}

//*次作業畫面一：執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(SelectValue.value, Excutecomplete, onPageMethodsError);
}

//*次作業畫面一：執行取消動作
function CancelExcute() {
    Block();

    PageMethods.CancelAction(SelectValue.value, Excutecomplete, onPageMethodsError);

    $.unblockUI();
}

//*次作業畫面一：讀取卡片資料1
function GetCardData() {
    if (popInput_PsnNo.value != '') {
        PageMethods.GetCardData(popInput_PsnNo.value, SetCardNo, onPageMethodsError);
    }
    else {
        popInput_OrigCardNo.value = '';
    }
}

//*次作業畫面一：讀取卡片資料2
function SetCardNo(DataArray) {
    if (DataArray[0] == 'Saho_SysErrorMassage') {
        alert(DataArray[1]);
        popInput_PsnNo.value = '';          //人員編號
        popInput_OrigCardNo.value = '';     // 卡號
        popInput_PsnName.value = '';        // 姓名
    }
    else {
        var strAry = DataArray[1].split("___");

        popInput_OrigCardNo.value = strAry[0];  // 卡號
        popInput_PsnName.value = strAry[1];     // 姓名
    }
}

//*次作業畫面一：讀取臨時卡片資料1
function GetTempCardData() {
    PageMethods.GetTempCardNo(SetTempCardData, onPageMethodsError);
}

//*次作業畫面一：讀取臨時卡片資料2
function SetTempCardData(DataArray) {
    if (DataArray[0] == 'Saho_SysErrorMassage') {
        alert(DataArray[1]);
    }
    else if (DataArray[0] == 'Saho_DbHaveNoData') {
        while (ddlCardNo.options.length > 0)
            ddlCardNo.options.remove(0);
    }
    else {
        while (ddlCardNo.options.length > 0)
            ddlCardNo.options.remove(0);

        // 索引 0 是訊息，內容從 1 開始，第 1 項是起始項目，內容是[選取資料]或[Selected]
        // 所以實際可選擇的項目從 2 開始
        for (i = 1; i < DataArray.length; i++) {
            option = document.createElement("option");

            if (i == 1) {
                option.text = DataArray[i];
                option.value = "";
            }
            else {
                option.text = DataArray[i];
                option.value = DataArray[i];
            }
            ddlCardNo.options.add(option);
        }
    }
}

function GetNowDate() {
    var nt = new Date();
    var sYear = nt.getFullYear();
    var sMonth = nt.getMonth() + 1;
    var sDay = nt.getDate();
    var sHour = nt.getHours();
    var sMin = nt.getMinutes();
    var sSec = nt.getSeconds();

    if (sYear < 1911) sYear = 1911;
    if (sMonth < 10) sMonth = '0' + sMonth;
    if (sDay < 10) sDay = '0' + sDay;
    if (sHour < 10) sHour = '0' + sHour;
    if (sMin < 10) sMin = '0' + sMin;
    if (sSec < 10) sSec = '0' + sSec;

    var strNowTime = sYear + '/' + sMonth + '/' + sDay + ' ' + sHour + ':' + sMin + ':' + sSec;

    return strNowTime;
}



function SelectArea() {
    $.ajax({
        type: "POST",
        url: "CardListByArea.ashx",
        dataType: 'json',
        data: JSON.stringify({ "Area": $('[name*=ddlCardArea]').val() }),
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var options = $('[name*=ddlCardNo]').find('option:eq(0)').clone();
            $('[name*=ddlCardNo] option').remove();
            $('[name*=ddlCardNo]').append(options);
            $(data).each(function (index, prop) {
                $('[name*=ddlCardNo]').append("<option value=\"" + prop.CardNo + "\">" + prop.CardNo + "</option>");
            });
            //$("#UserNo option:eq(1)").prop("selected", true);
        },
        fail: function () {
            alert("failed");
        }
    });
}
