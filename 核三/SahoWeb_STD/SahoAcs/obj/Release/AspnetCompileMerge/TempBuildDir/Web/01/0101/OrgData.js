// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_No_Title.disabled = false;
            popInput_No_Title.style.display = "none";
            popInput_No.setAttribute("style", "width:390px");
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_EName.disabled = false;
            popInput_Class.disabled = false;
            popInput_No_Title.value = '';
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_EName.value = '';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            popInput_No_Title.disabled = true;
            popInput_No_Title.style.display = "inline";
            popInput_No_Title.setAttribute("style", "width:20px;text-align: center");
            popInput_No.setAttribute("style", "width:350px");
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_EName.disabled = false;
            popInput_Class.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            ChangeText(DeleteLableText, '');
            break;
        case 'Delete':
            popInput_No2.disabled = true;
            popInput_Name2.disabled = true;
            popInput_EName2.disabled = true;
            popInput_Class2.disabled = true;
            popInput_Class3.disabled = false;
            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_Class.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_EName.value = '';
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            break;
    }
}

//GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectNowNo.value);
}

//取得DropDownList物件
function OrgClassData() {
    if (Input_Class.length == 0) {
        PageMethods.DDLData('ContentPlaceHolder1_Input_Class', SetClassData, onPageMethodsError);
    }
}

// 呼叫新增視窗
function CallAdd(Title) {
    ChangeText(L_popName1, Title);
    SetMode('Add');

    while (popInput_Class.length > 0) {
        popInput_Class.remove(0);
    }
    CheckGlobalSessionStatus().then(function (answer) {
        if (answer) {

            PageMethods.DDLData('ContentPlaceHolder1_popInput_Class', SetClassData, onPageMethodsError);
            popInput_Class.selectedIndex = 0;
            PopupTrigger1.click();
        }
    });
}

//執行新增動作
function AddExcute(UserID) {
    if ($('input[name$="popInput_No"]').val().trim() == "") {
        console.log($('input[name$="popInput_No"]').val());
        alert($("#ContentPlaceHolder1_popLabel_No").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_No"]').focus();
        return false;
    }
    if ($('input[name$="popInput_Name"]').val().trim() == "") {
        alert($("#ContentPlaceHolder1_popLabel_Name").text() + $("#MustInputMsg").val());
        $('input[name$="popInput_Name"]').focus();
        return false;
    }
    CheckGlobalSessionStatus().then(function (answer) {
        if (answer) {
            PageMethods.Insert(popInput_No.value.trim(), popInput_Name.value, popInput_Class.options[popInput_Class.selectedIndex].value, UserID, popInput_EName.value.trim(), Excutecomplete, onPageMethodsError);
        }
    });
}

// 呼叫編輯視窗
function CallEdit(Title, Msg) {
    ChangeText(L_popName1, Title);

    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else {
        SetMode('Edit');

        if (popInput_Class.length > 0) {
            while (popInput_Class.length != 0) {
                popInput_Class.remove(0);
            }
        }
        CheckGlobalSessionStatus().then(function (answer) {
            if (answer) {
                PageMethods.DDLData('ContentPlaceHolder1_popInput_Class', SetClassData, onPageMethodsError);
                PageMethods.LoadData('ContentPlaceHolder1_popInput_Class', SelectValue.value, hUserId.value, 'Edit', SetEditUI, onPageMethodsError);
            }
        });
    }
}

//執行更新動作
function EditExcute(UserID) {
    CheckGlobalSessionStatus().then(function (answer) {
        if (answer) {
            PageMethods.Update(SelectValue.value, SelectNowNo.value, popInput_No_Title.value.trim(), popInput_No.value.trim(), popInput_Name.value, popInput_Class.value, UserID, popInput_EName.value, Excutecomplete, onPageMethodsError);
        }
    });
}

// 呼叫刪除視窗
function CallDelete(Title, DelLabel, Msg) {
    if (IsEmpty(SelectValue.value)) {
        NotSelectForDelete(Msg);
    }
    else {
        //VerifyConfirmation("刪除組織資料所有相關資訊會一併刪除，是否繼續刪除?", function () {
        ChangeText(L_popName2, Title);
        ChangeText(DeleteLableText2, DelLabel);
        SetMode('Delete');
        CheckGlobalSessionStatus().then(function (answer) {
            if (answer) {
                if (popInput_Class3.length > 0) {
                    while (popInput_Class3.length != 0) {
                        popInput_Class3.remove(0);
                    }
                }

                PageMethods.LoadData('ContentPlaceHolder1_popInput_Class3', SelectValue.value, hUserId.value, 'Delete', SetDeleteUI, onPageMethodsError);
            }
        });
        //});
    }
}

//執行刪除動作
function DeleteExcute(UserID) {
    CheckGlobalSessionStatus().then(function (answer) {
        if (answer) {
            var OrgNo = popInput_Class3.options[popInput_Class3.selectedIndex].value;
            PageMethods.Delete(SelectValue.value, OrgNo, UserID, Excutecomplete, onPageMethodsError);
        }
    });
}

//將資料帶回畫面UI
function SetClassData(DataArray) {
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
function SetEditUI(DataArray) {
    console.log(DataArray);
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();
        SelectNowNo.value = DataArray[2];
        popInput_No_Title.value = DataArray[2].substr(0, 1);
        popInput_No.value = DataArray[2].substr(1, DataArray[2].length - 1);
        popInput_Name.value = DataArray[3];
        popInput_EName.value = DataArray[10];
        for (var i = 0; i < popInput_Class.children.length ; i++) {
            if (popInput_Class[i].value == DataArray[1]) {
                popInput_Class.selectedIndex = i;
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
    console.log(DataArray);
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger2.click();
        popInput_No2.value = DataArray[2];
        popInput_Name2.value = DataArray[3];
        popInput_Class2.value = DataArray[1];
        popInput_EName2.value = DataArray[10];

        if (DataArray[DataArray.length - 1] != '') {
            var data = DataArray[DataArray.length - 1].split("|");
            var str = data[0];
            var option = null;
            option = document.createElement("option");            
            option.text = $("#ddlSelectDefault").val();
            option.value = '';
            document.getElementById(str).options.add(option);

            if (data.length > 3) {
                for (var i = 1; i < data.length; i += 3) {
                    var option = null;
                    option = document.createElement("option");
                    option.text = '[' + data[i + 1] + ']' + data[i + 2];
                    option.value = data[i];
                    document.getElementById(str).options.add(option);
                }
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

function SelectState() {
    SelectClassNo.value = Input_Class.options[Input_Class.selectedIndex].value;
    SelectValue.value = '';
    __doPostBack(QueryButton.id, 'NewQuery');
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
                    SelectNowNo.value = popInput_No_Title.value.trim() + popInput_No.value.trim();
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

function ChangeTitle() {
    PageMethods.GetOrgTitle2(popInput_Class.options[popInput_Class.selectedIndex].value, ShowValue, onPageMethodsError);
}

function ShowValue(str) {
    popInput_No_Title.value = str;
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

function SetAddRow() { 
    var newrow = $('table[id="TableEmpty"]').find("tbody").last().html();            
    $("#ContentPlaceHolder1_MainGridView tr:last").after(newrow);
    $("#ContentPlaceHolder1_MainGridView tr:last").find("input[type='text']:eq(0)").focus();
    $("input[name*='SaveButton']").prop("disabled", false);
    $("#ContentPlaceHolder1_MainGridView tr:last").find("input[name='CHK_COL_0']:eq(0)").val($("#RowIndex").val());
    var rownum = parseInt($("#RowIndex").val());
    rownum = rownum + 1;
    $("#RowIndex").val(rownum);
    $("#ContentPlaceHolder1_MainGridView tr:last").find("input[type='text']").keyup(function () {
        if ($(this).val() != "") {
            var tr = JsFunFindParent(this, "TR");
            $(tr).find('input[name="CHK_COL_0"]').prop("checked", true);
        }        
    });
    funTabEnter();
    //alert(newrow);
}

function SetSave() {
    //取得Table裡面已經Checked的欄位
    var checkResult = true;
    var obj_focus = null;
    //e.preventDefault();
    if (JsFunBASE_VERTIFY()) {
        var form_data = $("#ContentPlaceHolder1_MainGridView").find(":input[type=text],input[type=checkbox],select").serialize();
        $.ajax({
            type: "POST",
            url: "OrgDataFun.ashx",
            dataType: "json",
            data: form_data + "&PageEvent=SaveOrgData",
            success: function (data) {
                window.location = window.location;
            },
            fail: function () {

            }
        });
    }
    return false;
    /*****先測試 JS_CHECK 的驗證方式
    $("#ContentPlaceHolder1_MainGridView").find(":input[type='checkbox']").each(function () {        
        if ($(this).prop("checked") === true) {
            var tr = JsFunFindParent(this, "TR");
            if ($(tr).find('input[name="NewOrgNo"]').val() === "") {
                checkResult = false;
                obj_focus = $(tr).find('input[name="NewOrgNo"]');
                $(tr).find('input[name="NewOrgNo"]').css({"border": "1px solid #ff0000"});
            }
            if ($(tr).find('input[name="NewOrgName"]').val() === "") {
                checkResult = false;
                obj_focus = $(tr).find('input[name="NewOrgName"]');
                $(tr).find('input[name="NewOrgName"]').css({"border": "1px solid #ff0000"});
            }
        }
    });    
    if (checkResult === false) {
        $(obj_focus).focus();        
        alert("組織名稱或編號尚未輸入");
    } else {
        $("#PageEvent").val("SaveOrgData");
        
    }
    */
}


function funTabEnter() {    
    var focusables = $(':input[type=text],select').not($("#NewRow").find(':input[type=text],select'));
    //alert(focusables.length);
    focusables.keydown(function (e) {
        if (e.keyCode == 13) {
            var current = focusables.index(this),
                                                 next = focusables.eq(current + 1).length ? focusables.eq(current + 1) : focusables.eq(0);
            //alert(focusables.eq(current + 1).length);
            next.focus();
            closeAllCompont();
        }
    });
}