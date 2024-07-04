// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_Building.disabled = false;
            popInput_Floor.disabled = false;
            popInput_EquModel.disabled = false;
            popInput_EquNo.disabled = false;
            popInput_EquName.disabled = false;
            popInput_EquEName.disabled = false;
            popInput_Dci.disabled = false;
            popInput_CardNoLen.disabled = false;
            popInput_Building.value = '';
            popInput_Floor.value = '';
            popInput_EquModel.value = '';
            popInput_EquNo.value = '';
            popInput_EquName.value = '';
            popInput_EquEName.value = '';
            popInput_Dci.value = '';
            popInput_CardNoLen.value = DefaultCardLen.value;
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            //popIsShowName.value="0";
            $('[name*="popIsShowName"]').prop("checked", true);
            break;
        case 'Edit':
            popInput_Building.disabled = false;
            popInput_Floor.disabled = false;
            popInput_EquModel.disabled = false;
            popInput_EquNo.disabled = false;
            popInput_EquName.disabled = false;
            popInput_EquEName.disabled = false;
            popInput_Dci.disabled = false;
            popInput_CardNoLen.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            $('[name*="popIsShowName"]').prop("checked", false);
            break;
        case 'Delete':
            popInput_Building.disabled = true;
            popInput_Floor.disabled = true;
            popInput_EquModel.disabled = true;
            popInput_EquNo.disabled = true;
            popInput_EquName.disabled = true;
            popInput_EquEName.disabled = true;
            popInput_Dci.disabled = true;
            popInput_CardNoLen.disabled = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            $('[name*="popIsShowName"]').prop("checked", false);
            break;
        case '':
            popInput_Building.value = '';
            popInput_Floor.value = '';
            popInput_EquModel.value = '';
            popInput_EquNo.value = '';
            popInput_EquName.value = '';
            popInput_EquEName.value = '';
            popInput_Dci.value = '';
            popInput_CardNoLen.value = '';
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            DeleteLableText.innerText = "";
            popIsShowName.value = false;
            $('[name*="popIsShowName"]').prop("checked", false);
            break;
    }
}

//設定使用者按鈕的權限
function SetUserLevel(str) {
    AddButton.disabled = true;
    EditButton.disabled = true;
    DeleteButton.disabled = true;
    ParaButton.disabled = true;
    SetButton.disabled = true;

    if (str != '') {
        var data = str.split(",");

        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'Add') {
                AddButton.disabled = false;
            }
            if (data[i] == 'Edit') {
                EditButton.disabled = false;
                ParaButton.disabled = false;
            }
            if (data[i] == 'Del') {
                DeleteButton.disabled = false;
            }
            if (data[i] == 'Auth') {
                SetButton.disabled = false;
            }
        }
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
    var is_show_name = "0";
    if ($('[name*="popIsShowName"]').prop("checked") == true) {
        is_show_name = "1";        
    }
    PageMethods.Insert(hideUserID.value, popInput_Building.value, popInput_Floor.value, popInput_EquModel.value,
        popInput_EquNo.value, popInput_EquName.value, popInput_EquEName.value, popInput_Dci.value, popInput_CardNoLen.value, is_show_name,
        Excutecomplete, onPageMethodsError);
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
        popInput_Building.value = DataArray[0];
        popInput_Floor.value = DataArray[1];
        popInput_EquModel.value = DataArray[2];
        popInput_EquNo.value = DataArray[3];
        popInput_EquName.value = DataArray[4];
        popInput_EquEName.value = DataArray[5];
        popInput_Dci.value = DataArray[6];
        popInput_CardNoLen.value = DataArray[7];        
        if (DataArray[8] == "0") {
            $('[name*="popIsShowName"]').prop("checked", false);            
        } else {
            $('[name*="popIsShowName"]').prop("checked", true);            
        }
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
    var is_show_name = "0";
    if ($('[name*="popIsShowName"]').prop("checked") == true) {
        is_show_name = "1";        
    }
    PageMethods.Update(hideUserID.value, SelectValue.value, popInput_Building.value, popInput_Floor.value, popInput_EquModel.value,
        popInput_EquNo.value, popInput_EquName.value, popInput_EquEName.value, popInput_Dci.value, popInput_CardNoLen.value, is_show_name,
        Excutecomplete, onPageMethodsError);
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

// 呼叫參數設定視窗
function CallParaSetting(Msg) {
    if (IsEmpty(SelectValue.value))
        NotSelectForEdit(Msg);
    else {
        $.ajax({
            type: "POST",
            url: '../ParaSettingBox.aspx',
            data: { 'EquNo': SelectValue.value },
            async: true,
            success: function (data) {
                $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                + ' -webkit-transform: translate3d(0,0,0);"></div>');
                $("#popOverlay").css("background", "#000");
                $("#popOverlay").css("opacity", "0.5");
                $("#popOverlay").width("100%");
                $("#popOverlay").height($(document).height());
                $(document.body).append('<div id="ParaDiv" style="position:absolute;z-index:1000000;background-color:#1275BC" ></div>');
                $("#ParaDiv").html(data);
                $("#ParaDiv").css("left", ($(document).width() - $("#ParaDiv").width()) / 2);
                $("#ParaDiv").css("top", $(document).scrollTop() + 50);
                $("#popB_Cancel").click(function () {
                    $("#ParaDiv").remove();
                    $("#popOverlay").remove();
                });
                $("#ParaGridView").find("tr").each(function () {
                    if ($(this).find("#ParaUI").val() == "2" && $(this).find("#M_ParaValue").val() != "") {
                        $(this).find('select[name*="ParaValue"]').val($(this).find("#M_ParaValue").val());
                    }
                });
            }
        });
    }
}


// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                    SelectValue.value = popInput_EquNo.value.trim();
                    BuildingValue.value = popInput_Building.value.trim();
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Edit":
                    SelectValue.value = popInput_EquNo.value.trim();
                    BuildingValue.value = popInput_Building.value.trim();
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Delete":
                    SelectValue.value = '';
                    BuildingValue.value = popInput_Building.value.trim();
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
    GridGoToRow(0, tablePanel.id, 'MainGridView', 1, SelectValue.value);
}

function SelectState() {
    SelectValue.value = '';
    __doPostBack(QueryButton.id, '');
}
