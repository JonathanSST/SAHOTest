var checkitem = new Array();
var textitem = new Array();

$(document).ready(function () {
    $("#BtnExport").click(function () {
        SetExport();
    });

    $("#BtnDef").click(function () {
        SetDefCardRule();
    });

})




function SetDefCardRule() {
    if (IsEmpty(SelectValue.value)) {
        var msg = $("#HiddenMsg").val();
        msg = msg.replace(/\|/g, '\n');
        NotSelectForEdit(msg);
    }
    else {
        $.ajax({
            type: "POST",
            url: "CopyRule.aspx",
            data: {
                "DoAction": "SetDef", "rule_no": SelectValue.value, "equ_model": hideEquModel.value, "user_id": hideUserID.value
            },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.success === true) {
                    alert("process done!!");
                }
            },
            fail: function () {
                alert("process error");
            }
        });
    }
}


function SetExport() {
    if (IsEmpty(SelectValue.value)) {
        var msg = $("#HiddenMsg").val();        
        msg = msg.replace(/\|/g, '\n');        
        NotSelectForEdit(msg);
    }
    else
    {
        $.post("CopyRule.aspx",
            { DoAction: "Query", rule_id: SelectValue.value},
            function (data) {
                $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                + ' -webkit-transform: translate3d(0,0,0);"></div>');
                $("#popOverlay").css("background", "#000");
                $("#popOverlay").css("opacity", "0.5");
                $("#popOverlay").width($(document).width());
                $("#popOverlay").height($(document).height());
                $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                    + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                $("#ParaExtDiv").html(data);
                $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
                $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
                $("#ParaExtDiv").find("#btnCancel").click(function () {
                    DoCancel();
                });
                $("#ParaExtDiv").find("#btnCopy").click(function () {
                    DoClone();
                });
            }//end get post data
        );//end post
    }
}


function DoClone() {
    if ($("#RuleName").val() == "") {
        alert("卡片規則名稱為必要欄位");
        return;
    }
    if ($("#RuleNo").val() == "") {
        alert("卡片規則編號為必要欄位");
        return;
    }
    $.ajax({
        type: "POST",
        url: "CopyRule.aspx",
        data: {
            "DoAction": "CheckData", "rule_id": $("#RuleID").val(), "rule_no": $("#RuleNo").val(), "rule_name": $("#RuleName").val(),
            "equ_model": $("#CopyEquModel").val(), "user_id": hideUserID.value
        },
        dataType: "json",
        success: function (data) {
            if (data.success === true) {
                alert("複製完成");
                $('input[name*="SelectValue"]').val($("#RuleNo").val());
                $('input[name*="hideEquModel"]').val($("#CopyEquModel").val());
                DoCancel();
                __doPostBack(UpdatePanel1.id, 'popPagePost');
                SetMode('');
            } else {
                alert("卡片規則編號重複定義");
            }
        },
        fail: function () {

        }
    });
}

function DoCancel() {
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}


// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var re = /^ctl00.ContentPlaceHolder1.[A-Z]{1}[a-z]{1}$/;
    var objName = ""
    switch (sMode) {
        case 'Add':
            for (var iCount = 0; iCount < objLen; iCount++) {
                objName = objForm.elements[iCount].name.split('_');
                if (objForm.elements[iCount].type == "checkbox" && re.test(objName[0])) {
                    //objForm.elements[iCount].checked = true;
                    objForm.elements[iCount].disabled = false;
                }
                if (objForm.elements[iCount].type == "text" && re.test(objName[0])) {
                    objForm.elements[iCount].value = '';
                    objForm.elements[iCount].disabled = false;
                }
            }
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Edit':
            for (var iCount = 0; iCount < objLen; iCount++) {
                objName = objForm.elements[iCount].name.split('_');
                if (objForm.elements[iCount].type == "checkbox" && re.test(objName[0])) {
                    objForm.elements[iCount].disabled = false;
                }
                if (objForm.elements[iCount].type == "text" && re.test(objName[0])) {
                    objForm.elements[iCount].disabled = false;
                }
            }
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Delete':
            for (var iCount = 0; iCount < objLen; iCount++) {
                objName = objForm.elements[iCount].name.split('_');
                if (objForm.elements[iCount].type == "checkbox" && re.test(objName[0])) {
                    objForm.elements[iCount].disabled = true;
                }
                if (objForm.elements[iCount].type == "text" && re.test(objName[0])) {
                    objForm.elements[iCount].disabled = true;
                }
            }
            popInput_No.disabled = true;
            popInput_Name.disabled = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            break;
        case '':
            for (var iCount = 0; iCount < objLen; iCount++) {
                objName = objForm.elements[iCount].name.split('_');
                if (objForm.elements[iCount].type == "checkbox" && re.test(objName[0])) {
                    objForm.elements[iCount].checked = false;
                    objForm.elements[iCount].disabled = false;
                }
                if (objForm.elements[iCount].type == "text" && re.test(objName[0])) {
                    objForm.elements[iCount].value = '';
                    objForm.elements[iCount].disabled = false;
                }
            }
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            break;
    }
    $('input[name*="PickTime1$PickTimeTextBox"]').prop("disabled", true);
    $(".Table01").find("tr:eq(1)").find("img").prop("disabled", true);
}

//設定使用者按鈕的權限
function SetUserLevel(str) {
    AddButton.disabled = true;
    EditButton.disabled = true;
    DeleteButton.disabled = true;
    popB_Edit.disabled = true;
    document.getElementById("BtnExport").disabled = true;
    if (str != '') {
        var data = str.split(",");

        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'Add') {
                AddButton.disabled = false;
            }
            if (data[i] == 'Edit') {
                EditButton.disabled = false;
                popB_Edit.disabled = false;
                document.getElementById("BtnExport").disabled = false;
            }
            if (data[i] == 'Del') {
                DeleteButton.disabled = false;
            }
            if (data[i] == 'Auth') {

            }
        }
    }
}

// 呼叫新增視窗
function CallAdd(title) {
    L_popName.innerText = title + " - " + Input_EquModel.value;
    PageMethods.CheckAdd(Input_EquModel.value, Excutecomplete, onPageMethodsError);    
}

// 執行新增動作
function AddExcute() {
    GetUIData();
    PageMethods.Insert(hideUserID.value, Input_EquModel.value, popInput_No.value, popInput_Name.value, checkitem, textitem, Excutecomplete, onPageMethodsError);
}

// 呼叫編輯視窗
function CallEdit(title, Msg) {
    L_popName.innerText = title + " - " + hideEquModel.value;
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        SetMode('Edit');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, hideEquModel.value, SetUI, onPageMethodsError);
    }
}

// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput_No.value = DataArray[2];
        popInput_Name.value = DataArray[3];

        var objForm = document.forms["form1"];
        var objLen = objForm.length;
        var re = /^ctl00.ContentPlaceHolder1.[A-Z]{1}[a-z]{1}$/;
        var objName = ""
        var checkcount = 0, textcount = 0;
        var checkitem = DataArray[11].split('&');
        var textitem = DataArray[12].split('&');

        for (var iCount = 0; iCount < objLen; iCount++) {
            objName = objForm.elements[iCount].name.split('_');
            if (objForm.elements[iCount].type == "checkbox" && re.test(objName[0])) {
                objForm.elements[iCount].checked = (checkitem[checkcount] == "01") ? true : false;
                checkcount++;
            }
            if (objForm.elements[iCount].type == "text" && re.test(objName[0])) {
                objForm.elements[iCount].value = (textitem[textcount] == "FFFF") ? "" : textitem[textcount].slice(0, 2) + ":" + textitem[textcount].slice(-2);
                textcount++;
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

// 執行編輯動作
function EditExcute() {
    GetUIData();
    PageMethods.Update(hideUserID.value, hideEquModel.value, SelectValue.value, popInput_No.value, popInput_Name.value, checkitem, textitem, Excutecomplete, onPageMethodsError);
}

// 呼叫刪除視窗
function CallDelete(title, Msg) {
    L_popName.innerText = title + " - " + hideEquModel.value;
    DeleteLableText.innerText = $("#AlertDelete").val();
    if (IsEmpty(SelectValue.value)) NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, hideEquModel.value, SetUI, onPageMethodsError);
    }
}

// 執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(hideEquModel.value, SelectValue.value, hideUserID.value, Excutecomplete, onPageMethodsError);
}

// 各動作完成
function Excutecomplete(objRet) {
    
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                    SelectValue.value = popInput_No.value.trim();
                    hideEquModel.value = Input_EquModel.value;
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    SetMode('');
                    CancelTrigger1.click();
                    break;
                case "Edit":
                    SelectValue.value = popInput_No.value.trim();
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    SetMode('');
                    CancelTrigger1.click();
                    break;
                case "Delete":
                    SelectValue.value = '';
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    SetMode('');
                    CancelTrigger1.click();
                    break;
                case "CheckAdd":
                    SetMode('Add');
                    PopupTrigger1.click();
                    $("input[name*='CheckBox1']").prop("checked", true);
                    $('input[name*="PickTime1$PickTimeTextBox"]').val("00:00");
                    break;
            }
            //$("input[name*='CheckBox1']").prop("disabled", true);
            break;
        case false:
            alert(objRet.message);
            break;
        default:

            break;
    }
}

// 取得畫面資料
function GetUIData() {
    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var re = /^ctl00.ContentPlaceHolder1.[A-Z]{1}[a-z]{1}$/;
    var objName = ""
    var checkcount = 0, textcount = 0;

    for (var iCount = 0; iCount < objLen; iCount++) {
        objName = objForm.elements[iCount].name.split('_');
        if (objForm.elements[iCount].type == "checkbox" && re.test(objName[0])) {
            checkitem[checkcount] = objForm.elements[iCount].checked;
            checkcount++;
        }
        if (objForm.elements[iCount].type == "text" && re.test(objName[0])) {
            textitem[textcount] = objForm.elements[iCount].value.replace(':', '');
            textcount++;
        }
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


function DoCopy(obj) {
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
    $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:200000; overflow:hidden;'
                + ' -webkit-transform: translate3d(0,0,0);"></div>');
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#popOverlay").width("100%");
    $("#popOverlay").height($(document).height());

    var obj_index = $(".IconExport").index(obj);
    $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:200001;background-color:#ffffff;'
        + 'border-style:solid; border-width:2px; border-color:#069" ></div>');

    var data = "";
    data = "<table><tr><td>選擇要進行複製的天數</td></tr></table>";
    data += '<table><tr>'

    for (var i = 0; i < 8; i++) {
        data += '<td><input type="checkbox" style="width:20px;height:20px"/></td><td>' + $(".Table01").find("th:eq(" + (i + 1) + ")").html() + '&nbsp;&nbsp;&nbsp;</td>'
    }

    data += '</tr></table>'
    data += '<table><tr><td><input type="button" id="BtnCopySave" value="複製" class="IconSave"/>';
    data += '<input type="button" id="BtnCopyCancel" value="取消" class="IconCancel"/></td></tr></table>';

    $("#ParaExtDiv").html(data);
    $("#ParaExtDiv").css("left", $(document).width() / 4);
    $("#ParaExtDiv").css("top", $(document).scrollTop() + 300);
    $("#ParaExtDiv").find("input[type='checkbox']:eq(" + obj_index + ")").first().prop("disabled", true);

    $("#BtnCopySave").click(function () {
        $("#ParaExtDiv").find("input[type='checkbox']:checked").each(function () {
            var chk_index = $("#ParaExtDiv").find("input[type='checkbox']").index($(this));
            for (var i = 1; i <= 5; i++) {
                $('input[name*="PickTime' + i + '"]:eq(' + chk_index + ')').val($('input[name*="PickTime' + i + '"]:eq(' + obj_index + ')').val());
                $('input[name*="CheckBox' + i + '"]:eq(' + chk_index + ')').prop("checked",
				$('input[name*="CheckBox' + i + '"]:eq(' + obj_index + ')').prop("checked"));
            }
        });
        alert("複製完成!!");
        $("#ParaExtDiv").remove();
        $("#popOverlay").remove();
    });

    $("#BtnCopyCancel").click(function () {
        $("#ParaExtDiv").remove();
        $("#popOverlay").remove();
    });

}


function SelectState() {
    hideEquModel.value = Input_EquModel.value;
    __doPostBack(QueryButton.id, '');
}
